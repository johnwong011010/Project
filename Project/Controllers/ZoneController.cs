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
        [HttpPost]
        public async Task<ActionResult> AddZone(string name,string description,string area_id)
        {
            Zone newZone = new Zone
            {
                name = name,
                description = description,
                area_id = area_id,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
            };
            await _service.AddZone(newZone);
            return Ok();
        }
        [HttpPut("/api/[controller]/{id:length(24)}")]
        public async Task<ActionResult> UpdateZone(string id,string name,string description,string area_id)
        {
            var zone = await _service.GetZoneByID(id);
            zone.name = name;
            zone.area_id = area_id;
            zone.description = description;
            zone.updated_at = DateTime.UtcNow;
            await _service.UpdateZone(id, zone);
            return Ok();
        }
        [HttpPut("/api/[controller]/zone/{id:length(24)}")]
        public async Task DeleteZone(string id) => await _service.DeleteZone(id, "isDeleted");
        [HttpGet("/api/[controller]/zone/item")]
        public async Task<ActionResult> GetZoneAsItem()
        {
            var items = await _service.GetAllZone();
            SelectItem[] result = new SelectItem[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                result[i] = new SelectItem
                {
                    _id = items[i]._id,
                    name = items[i].name
                };
            }
            return Ok(result);
        }
    }
}
