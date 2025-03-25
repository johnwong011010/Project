using Microsoft.AspNetCore.Mvc;
using Project.Model;
using Project.Service;


namespace Project.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class GamingTablesController : Controller
    {
        private readonly Gaming_Tables_Service _service;
        public GamingTablesController(Gaming_Tables_Service service) => _service = service;
        [HttpGet
            ]
        public async Task<List<Gaming_Tables>> GetTable() => await _service.GetAllTable();
        [HttpGet("/api/gamingtable/{id:length(24)}")]
        public async Task<Gaming_Tables> GetTable(string id) => await _service.GetTable(id);
        [HttpPost("/api/gaming-tables")]
        public async Task AddTable(Gaming_Tables table) => await _service.CreateTable(table);
        [HttpPut("/api/gaming-tables/{id:length(24)}")]
        public async Task UpdateTable(string id, Gaming_Tables table) => await _service.UpdateTable(id, table);
        [HttpPatch("/api/gaming-tables/{id:length(24)}/status")]
        public async Task UpdateTableStatus(string id, string status) => await _service.UpdateTableStatus(id, status);
        [HttpDelete("/api/gaming-tables/{id:length(24)}")]
        public async Task DeleteTable(string id) => await _service.DeleteTable(id);
    }
}
