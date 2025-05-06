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
                if (result is null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
        }
        [HttpGet("/api/[controller]/{zid:length(24)}")]
        public async Task<List<Pit>> GetPits(string zid) => await _service.GetPitByZone(zid);
        [HttpGet("/api/[controller]/pit/{id:length(24)}")]
        public async Task<Pit> GetPit(string id) => await _service.GetPitByID(id);
        [HttpGet("/api/[controller]/pit/{name}")]
        public async Task<ActionResult> GetPitByName(string name)
        {
            var result = await _service.GetPitByName(name);
            return Ok(result);
        }
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
        public async Task<ActionResult> UpdatePit(string id,string name,string description,string zone_id)
        {
            var pit = await _service.GetPitByID(id);
            Pit newPit = new Pit 
            {
                _id = id,
                name = name,
                description = description,
                zone_id = zone_id,
                created_at = pit.created_at,
                updated_at = DateTime.Now
            };
            await _service.UpdatePit(id, newPit);
            return Ok();
        }
        [HttpPut("/api/[controller]/pit/{id:length(24)}")]
        public async Task DeletePit(string id) => await _service.DeletePit(id,"isDeleted");
        [HttpGet("/api/[controller]/pit/item")]
        public async Task<ActionResult> GetPitItem()
        {
            try
            {
                var result = await _service.GetAllPit();
                SelectItem[] company = new SelectItem[result.Count];
                for (int i = 0; i < result.Count; i++)
                {
                    company[i] = new SelectItem
                    {
                        _id = result[i]._id,
                        name = result[i].name,
                    };
                }
                if (company != null)
                {
                    return Ok(company);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception E)
            {
                return NotFound(E);
            }
        }
    }
}
