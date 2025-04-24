using Microsoft.AspNetCore.Mvc;
using Project.Model;
using Project.Service;
using System.Data;
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
        public async Task<List<Gaming_Tables>> GetTable() => await _service.GetAllTable();
        [HttpGet("/api/gamingtable/{id:length(24)}")]
        public async Task<Gaming_Tables> GetTable(string id) => await _service.GetTable(id);
        [HttpPost("/api/gaming-tables")]
        public async Task AddTable(string role,string name,string tbnum,string pit_id,string sub_company_id,string gameType,int minBet,int maxBet, List<string> chipsType, List<int> denominations,string status) 
        {
            Gaming_Tables newTable = new Gaming_Tables
            {
                Pit_id = pit_id,
                sub_company_id = sub_company_id,
                table_number = tbnum,
                game_type = gameType,
                min_bet = minBet,
                max_bet = maxBet,
                chipset_configuration = new Chips
                {
                    chip_types = chipsType,
                    denominations = denominations
                },
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
        public async Task UpdateTable(string id, Gaming_Tables table,string role,string name)
        {
            DateTime time = DateTime.UtcNow;
            operation_logs log = new operation_logs
            {
                gaming_table_id = table._id,
                operation_type = "Update",
                performed_by = role+"_"+name,
                operation_timestamp = time,
                details = "Update gaming table" + " " + table.table_number.ToString()
            };
            await _service.UpdateTable(id, table);
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
            if (status.Equals("enable", StringComparison.OrdinalIgnoreCase))
            {
                log.operation_type = "enable";
                await _service.UpdateTableStatus(id, status);
                await _Logservice.WriteLog(log);
            }
            else if (status.Equals("disable", StringComparison.OrdinalIgnoreCase))
            {
                log.operation_type = "disable";
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
