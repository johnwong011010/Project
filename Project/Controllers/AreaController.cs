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
        public async Task<ActionResult> GetAreas(int current, int pageSize, string? name)
        {
            if (name is null || name == "")
            {
                var result = await _service.GetAllArea();
                return Ok(result);
            }
            else
            {
                var result = await _service.GetAreaByName(name);
                Area[] areas = new Area[] { result };//需要將取得的單筆資料強制轉換成array或者list再回傳
                return Ok(areas);
            }
        }
        [HttpGet("/api/Area/{id:length(24)}")]
        public async Task<Area> GetAreas(string id) => await _service.GetAreaByID(id);
        [HttpGet("/api/Area/Company/")]
        public async Task<List<Area>> GetAreaByC(string cid) => await _service.GetAreaByCompanyID(cid);
        [HttpGet("/api/Area/{name}")]
        public async Task<ActionResult> GetArea(string name)
        {
            var result = await _service.GetAreaByName(name);
            if (result is null)
            {
                return NotFound();
            }
            else
            {
                return Ok(result);
            }
        }
        [HttpPost("/api/Area")]
        public async Task<ActionResult> AddArea(string name,string description,string sub_company_id)
        {
            Area newArea = new Area
            {
                name = name,
                description = description,
                sub_company_id = sub_company_id,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow,
            };
            await _service.AddArea(newArea);
            return Ok();
        }
        [HttpPut("/api/Area/{id:length(24)}")]
        public async Task<ActionResult> UpdateArea(string id,string name,string description)
        {
            var area = await _service.GetAreaByID(id);
            var result = area;
            result.name = name;
            result.description = description;
            result.updated_at = DateTime.Now;
            await _service.UpdateArea(id, result);
            return Ok();
        }
        [HttpPut("/api/Area/area/{id:length(24)}")]
        public async Task<ActionResult> DeleteArea(string id)
        {
            await _service.DeleteArea(id, "isDeleted");
            return NoContent();
        }
        [HttpGet("/api/[controller]/item")]
        public async Task<ActionResult> GetAreaAsItem()
        {
            var result = await _service.GetAllArea();
            SelectItem[] item = new SelectItem[result.Count];
            for (int i = 0; i < result.Count; i++)
            {
                item[i] = new SelectItem
                {
                    _id = result[i]._id,
                    name = result[i].name
                };
            }
            return Ok(item);
        }
    }
}
