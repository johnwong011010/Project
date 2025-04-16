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
        public async Task<ActionResult> GetZones(int current, int pageSize, string? name)
        {
            if (name is null || name == null)
            {
                var result = await _service.GetAllZone();
                return Ok(result);
            }
            else
            {
                var result = await _service.GetZoneByName(name);
                Zone[] zones = new Zone[] { result };
                return Ok(result);
            }
        }
        [HttpGet("/api/[controller]/area")]
        public async Task<List<Zone>> GetZones(string aid) => await _service.GetZoneByArea(aid);
        [HttpGet("/api/[controller]/name")]
        public async Task<ActionResult> GetZone(string name)
        {
            var result = await _service.GetZoneByName(name);
            return Ok(result);
        }
        [HttpGet("api/[controller]/{id:length(24)}")]
        public async Task<Zone> GetSinZone(string id) => await _service.GetZoneByID(id);
        [HttpPost("api/[controller]/")]
        public async Task<ActionResult> AddZone(string name,string decsription,string area_id)
        {
            Zone newZone = new Zone
            {
                name = name,
                description = decsription,
                area_id = area_id,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
            };
            await _service.AddZone(newZone);
            return Ok();
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
