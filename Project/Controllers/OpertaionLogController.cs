using Microsoft.AspNetCore.Mvc;
using Project.Model;
using Project.Service;
using System.Text.Json;

namespace Project.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class OpertaionLogController : Controller
    {
        private readonly OperationLogService _service;
        public OpertaionLogController(OperationLogService service) => _service = service;
        [HttpGet]
        public async Task<ActionResult> GetAllLogs(string? current,int pageSize)
        {
            var logs = await _service.GetAllLogs();
            var json = new
            {
                data = logs,
                current = current,
                pagesize = pageSize,
                total = logs.Count
            };
            string jsonItem = JsonSerializer.Serialize(json);
            return Ok(jsonItem);
        }
        [HttpGet("/api/[controller]/{tid:length(24)}")]
        public async Task<List<operation_logs>> GetTableLogs(string tid) => await _service.GetTableAllLogs(tid);
        [HttpGet("/api/[controller]/user")]
        public async Task<List<operation_logs>> GetUserLogs(string user) => await _service.GetUserAllLogs(user);
        [HttpPost("/api/[controller]")]
        public async Task WriteLog(operation_logs log) => await _service.WriteLog(log);
    }
}
