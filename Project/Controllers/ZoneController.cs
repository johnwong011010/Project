using Microsoft.AspNetCore.Mvc;
using Project.Model;
using Project.Service;

namespace Project.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class ZoneController : Controller
    {
        private readonly Zone_Service _service;
        public ZoneController(Zone_Service service) => _service = service;
        [HttpGet]
        public async Task<List<Zone>> GetZones() => await _service.GetAllZone();
        [HttpGet("/api/[controller]/area")]
        public async Task<List<Zone>> GetZones(string aid) => await _service.GetZoneByArea(aid);
        [HttpGet("/api/[controller]/name")]
        public async Task<List<Zone>> GetZone(string name) => await _service.GetZoneByName(name);//may have many
        [HttpGet("api/[controller]/{id:length(24)}")]
        public async Task<Zone> GetSinZone(string id) => await _service.GetZoneByID(id);
        [HttpPost("api/[controller]/")]
        public async Task AddZone(Zone z)
        {
            /*z.created_at = DateTime.UtcNow;
            z.update_at = DateTime.UtcNow;*/
            await _service.AddZone(z);
        }
        [HttpPut("/api/[controller]/{id:length(24)}")]
        public async Task UpdateZone(string id,Zone z)
        {
            var zone = await _service.GetZoneByID(id);
            z.created_at = zone.created_at;
            z.updated_at = DateTime.UtcNow;
            await _service.UpdateZone(id, z);
        }
        [HttpPut("/api/[controller]/zone/{id:length(24)}")]
        public async Task DeleteZone(string id) => await _service.DeleteZone(id, "isDeleted");
    }
}
