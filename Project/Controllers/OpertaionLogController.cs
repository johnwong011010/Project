using Microsoft.AspNetCore.Mvc;
using Project.Model;
using Project.Service;

namespace Project.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class OpertaionLogController : Controller
    {
        private readonly OperationLogService _service;
        public OpertaionLogController(OperationLogService service) => _service = service;
        [HttpGet]
        public async Task<List<operation_logs>> GetAllLogs() => await _service.GetAllLogs();
        [HttpGet("/api/[controller]/{tid:length(24)}")]
        public async Task<List<operation_logs>> GetTableLogs(string tid) => await _service.GetTableAllLogs(tid);
        [HttpGet("/api/[controller]/user")]
        public async Task<List<operation_logs>> GetUserLogs(string user) => await _service.GetUserAllLogs(user);
        [HttpPost("/api/[controller]")]
        public async Task WriteLog(operation_logs log) => await _service.WriteLog(log);
    }
}
