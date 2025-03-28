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
        public async Task AddTable(Gaming_Tables table,string role,string name) 
        {
            DateTime time = DateTime.UtcNow;
            await _service.CreateTable(table);
            var t = await _service.getTable(table.table_number);
            operation_logs log = new operation_logs
            {
                gaming_table_id = t._id,
                operation_type = "Create",
                performed_by = role + "_" + name,
                operation_timestamp = time,
                details = "Create gaming table" + " " + table.table_number.ToString()
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
        public async Task UpdateTableStatus(string id, string status) => await _service.UpdateTableStatus(id, status);
        [HttpDelete("/api/gaming-tables/{id:length(24)}")]
        public async Task DeleteTable(string id) => await _service.DeleteTable(id);
    }
}
