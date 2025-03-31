using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Writers;
using Project.Model;
using Project.Service;

namespace Project.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AreaController : Controller
    {
        private readonly AreaService _service;
        public AreaController(AreaService service) => _service = service;
        [HttpGet]
        public async Task<List<Area>> GetAreas() => await _service.GetAllArea();
        [HttpGet("/api/Area/{id:length(24)}")]
        public async Task<Area> GetAreas(string id) => await _service.GetAreaByID(id);
        [HttpGet("/api/Area/Company/")]
        public async Task<List<Area>> GetAreaByC(string cid) => await _service.GetAreaByCompanyID(cid);
        [HttpGet("/api/Area/{name}")]
        public async Task<Area> GetArea(string name) => await _service.GetAreaByName(name);
        [HttpPost("/api/Area")]
        public async Task AddArea(Area area) => await _service.AddArea(area);
        [HttpPut("/api/Area/{id:length(24)}")]
        public async Task UpdateArea(string id,Area a)
        {
            var area = await _service.GetAreaByID(id);
            a.created_at = area.created_at;
            a.updated_at = DateTime.UtcNow;
            await _service.UpdateArea(id, a);
        }
        [HttpPut("/api/Area/area/{id:length(24)}")]
        public async Task DeleteArea(string id) => await _service.DeleteArea(id,"isDeleted");
    }
}
