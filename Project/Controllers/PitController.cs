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
        public async Task<List<Pit>> GetPits() => await _service.GetAllPit();
        [HttpGet("/api/[controller]/{zid:length(24)}")]
        public async Task<List<Pit>> GetPits(string zid) => await _service.GetPitByZone(zid);
        [HttpGet("/api/[controller]/pit/{id:length(24)}")]
        public async Task<Pit> GetPit(string id) => await _service.GetPitByID(id);
        [HttpPost("/api/[controller]")]
        public async Task AddPit(Pit p)
        {
            p.created_at = DateTime.UtcNow;
            p.updated_at = DateTime.UtcNow;
            await _service.AddPit(p);
        }
        [HttpPut("/api/[controller]/{id:length(24)}")]
        public async Task UpdatePit(string id,Pit p)
        {
            var pit = await _service.GetPitByID(id);
            p.created_at = pit.created_at;
            p.updated_at = DateTime.UtcNow;
            await _service.UpdatePit(id, p);
        }
        [HttpDelete("/api/[controller]/{id:length(24)}")]
        public async Task DeletePit(string id) => await _service.DeletePit(id);
    }
}
