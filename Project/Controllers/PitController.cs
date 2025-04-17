using Microsoft.AspNetCore.Mvc;
using Project.Model;
using Project.Service;

namespace Project.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PitController : Controller
    {
        private readonly Pit_Service _service;
        public PitController(Pit_Service service) => _service = service;
        [HttpGet]
        public async Task<ActionResult> GetPits(int current,int pagesize,string? name)
        {
            if (name is null || name == null)
            {
                var result = await _service.GetAllPit();
                return Ok(result);
            }
            else
            {
                var result = await _service.GetPitByName(name);
                return Ok(result);
            }
        }
        [HttpGet("/api/[controller]/{zid:length(24)}")]
        public async Task<List<Pit>> GetPits(string zid) => await _service.GetPitByZone(zid);
        [HttpGet("/api/[controller]/pit/{id:length(24)}")]
        public async Task<Pit> GetPit(string id) => await _service.GetPitByID(id);
        [HttpPost("/api/[controller]")]
        public async Task<ActionResult> AddPit(string name,string description,string zone_id)
        {
            Pit newPit = new Pit
            {
                name = name,
                description = description,
                zone_id = zone_id,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
            };
            await _service.AddPit(newPit);
            return Ok();
        }
        [HttpPut("/api/[controller]/{id:length(24)}")]
        public async Task UpdatePit(string id,Pit p)
        {
            var pit = await _service.GetPitByID(id);
            p.created_at = pit.created_at;
            p.updated_at = DateTime.UtcNow;
            await _service.UpdatePit(id, p);
        }
        [HttpPut("/api/[controller]/pit/{id:length(24)}")]
        public async Task DeletePit(string id) => await _service.DeletePit(id,"isDeleted");
    }
}
