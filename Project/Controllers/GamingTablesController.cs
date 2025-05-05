using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Project.Model;
using Project.Service;
using System.Data;
using System.Text.Json;
using System.Xml.Linq;


namespace Project.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class GamingTablesController : Controller
    {
        private readonly Gaming_Tables_Service _service;
        private readonly OperationLogService _Logservice;
        public GamingTablesController(Gaming_Tables_Service service, OperationLogService logService)
        {
            _service = service;
            _Logservice = logService;
        }
        [HttpGet]
        public async Task<ActionResult> GetTable(string? current,string? pageSize,string? tbnum,string? gameType,string? status,string? min_bet,string? max_bet)
        {
            var allParams = new[] { tbnum, gameType, status, min_bet, max_bet };
            var filterBuilder = Builders<Gaming_Tables>.Filter;// add search filter
            var filter = new List<FilterDefinition<Gaming_Tables>>(); //detail of filter
            if (current != null && pageSize != null & allParams.All(string.IsNullOrEmpty)) //沒有查詢參數就回傳所有資料
            {
                var result = await _service.GetAllTable();
                /* 制作json字串 */
                var jsonItem = new
                {
                    data = result,
                    current = current,
                    pagesize = pageSize,
                    total = result.Count
                };
                string json = JsonSerializer.Serialize(jsonItem);
                return Ok(json);
            }
            if (!string.IsNullOrEmpty(tbnum))
            {
                filter.Add(filterBuilder.Eq(x => x.table_number, tbnum));
            }
            if (!string.IsNullOrEmpty(gameType))
            {
                filter.Add(filterBuilder.Eq(x => x.game_type, gameType));
            }
            if (!string.IsNullOrEmpty(status))
            {
                filter.Add(filterBuilder.Eq(x => x.status, status));
            }
            if (!string.IsNullOrEmpty(min_bet))
            {
                int convertString = Int32.Parse(min_bet);
                filter.Add(filterBuilder.Gte(x=>x.min_bet,convertString));
            }
            if (!string.IsNullOrEmpty(max_bet))
            {
                var convertString = Int32.Parse(max_bet);
                filter.Add(filterBuilder.Gte(x=>x.max_bet,convertString));
            }
            var combinedFilter = filter.Any() ? filterBuilder.And(filter) : FilterDefinition<Gaming_Tables>.Empty;//有任意條件時根據條件尋找,沒有就直接返回所有table
            var FilterTable = await _service.GetTableByFilter(combinedFilter);
            var FliterItem = new
            {
                data = FilterTable,
                current = current,
                pagesize = pageSize,
                total = FilterTable.Count(),
            };
            string Filterjson = JsonSerializer.Serialize(FliterItem);
            return Ok(FliterItem);//search table fit to filter
        }
        [HttpGet("/api/gamingtable/{id:length(24)}")]
        public async Task<Gaming_Tables> GetTable(string id) => await _service.GetTable(id);
        [HttpPost("/api/gaming-tables")]
        //public async Task AddTable(string role,string name,string tbnum,string pit_id,string sub_company_id,string gameType,int min_bet,int max_bet,[FromForm] List<string> chips_types,[FromForm] List<int> denomination,string status)
        public async Task AddTable(string role, string name, string tbnum, string pit_id, string sub_company_id, string gameType, int min_bet, int max_bet, [FromBody] Chips chipsSet, string status)
        {
            //Console.WriteLine(chips_types.Count);
            //Console.WriteLine(denomination.Count);
            //List<int> Denominations = denomination.Select(int.Parse).ToList();
            Gaming_Tables newTable = new Gaming_Tables
            {
                pit_id = pit_id,
                sub_company_id = sub_company_id,
                table_number = tbnum,
                game_type = gameType,
                min_bet = min_bet,
                max_bet = max_bet,
                chipset_configuration = chipsSet,
                /*chipset_configuration = new Chips
                {
                    chip_types = chips_types,
                    denominations = denomination
                },*/
                status = status,
                created_at = DateTime.Now,
                updated_at = DateTime.Now
            };
            await _service.CreateTable(newTable);
            var t = await _service.getTable(newTable.table_number);
            operation_logs log = new operation_logs
            {
                gaming_table_id = t._id,
                operation_type = "Create",
                performed_by = role + "_" + name,
                operation_timestamp = DateTime.Now,
                details = "Create gaming table" + " " + newTable.table_number.ToString()
            };
            await _Logservice.WriteLog(log);
        } 
        [HttpPut("/api/gaming-tables/{id:length(24)}")]
        public async Task UpdateTable(string id, string role,string name,string sub_company_id, string pit_id, string tbnum, string gameType,int min_bet, int max_bet)
        {
            Gaming_Tables originalTable = await _service.GetTable(id);
            Gaming_Tables ChangedTable = await _service.GetTable(id);//get original table
            ChangedTable.table_number = tbnum;
            ChangedTable.pit_id = pit_id;
            ChangedTable.sub_company_id = sub_company_id;
            ChangedTable.game_type = gameType;
            ChangedTable.min_bet = min_bet;
            ChangedTable.max_bet = max_bet;
            ChangedTable.updated_at = DateTime.Now;
            operation_logs log = new operation_logs
            {
                gaming_table_id = ChangedTable._id,
                operation_type = "Update",
                performed_by = role+"_"+name,
                operation_timestamp = DateTime.Now,
                details = "Update gaming table" + " " + originalTable.table_number.ToString()
            };
            await _service.UpdateTable(id, ChangedTable);
            await _Logservice.WriteLog(log);
        } 
        [HttpPatch("/api/gaming-tables/{id:length(24)}/status")]
        public async Task UpdateTableStatus(string id, string status,string role,string name)
        {
            var table = await _service.GetTable(id);
            DateTime time = DateTime.UtcNow;
            operation_logs log = new operation_logs
            {
                gaming_table_id = table._id,
                performed_by = role + "_" + name,
                operation_timestamp = time,
                details = "Update gaming table" + " " + table.table_number.ToString()
            };
            if (status.Equals("active", StringComparison.OrdinalIgnoreCase))
            {
                log.operation_type = "enable";
                await _service.UpdateTableStatus(id, status);
                await _Logservice.WriteLog(log);
            }
            else if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
            {
                log.operation_type = "disable";
                await _service.UpdateTableStatus(id, status);
                await _Logservice.WriteLog(log);
            }
            else
            {
                log.operation_type = "maintaince";
                await _service.UpdateTableStatus(id, status);
                await _Logservice.WriteLog(log);
            }
        }
        [HttpDelete("/api/gaming-tables/{id:length(24)}")]
        public async Task DeleteTable(string id,string role,string name)
        {
            var table = await _service.GetTable(id);
            DateTime time = DateTime.UtcNow;
            operation_logs log = new operation_logs
            {
                gaming_table_id = table._id,
                operation_type = "Delete",
                performed_by = role + "_" + name,
                operation_timestamp = time,
                details = "Delete Gaming table" + " " + table.table_number.ToString()
            };
            await _Logservice.WriteLog(log);
            await _service.DeleteTable(id);
        }
        [HttpGet("/api/gaming-table/chips-color")]
        public async Task<ActionResult> GetChipsColor()
        {
            var color = await _service.GetChipsColor();
            return Ok(color);
        }
        [HttpGet("/api/gaming-table/denomination")]
        public async Task<ActionResult> GetDenomination()
        {
            var denomination = await _service.GetDenomination();
            return Ok(denomination);
        }
    }
}
