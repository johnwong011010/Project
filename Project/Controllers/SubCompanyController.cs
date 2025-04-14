using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Project.Model;
using Project.Service;

namespace Project.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [EnableCors("ReactAppPolicy")]
    public class SubCompanyController : Controller
    {
        private readonly Sub_Company_Service _service;
        public SubCompanyController(Sub_Company_Service service)
        {
            _service = service;
        }
        [HttpGet]
        public async Task<ActionResult> GetAllCompany(int current,int pageSize,string? name)
        {
            if (name is null || name == "")
            {
                var result = await _service.GetAllCompany();
                return Ok(result);
            }
            else
            {
                var result = await _service.GetCompanyByName(name);
                sub_companies[] companies = new sub_companies[] { result }; //需要將取得的單筆資料強制轉換成array或者list再回傳
                return Ok(companies);
            }
        }
        [HttpGet("/api/[controller]/company")]
        public async Task<ActionResult> GetCompanyByName(string name)
        {
            return Ok(await _service.GetCompanyByName(name));
        }
        [HttpPost("/api/[controller]/company")]
        public async Task<ActionResult> AddCompany(string name,string description)
        {
            sub_companies newCompany = new sub_companies
            {
                name = name,
                description = description,
                created_at = DateTime.Now,
                updated_at = DateTime.Now,
            };
            await _service.AddCompany(newCompany);
            return Ok();
        }
        [HttpPut("/api/[controller]/company")]
        public async Task<ActionResult> UpdateCompany(string name,string description,string company_id)
        {
            var company = await _service.GetCompany(company_id);
            company.name = name;
            company.description = description;
            company.updated_at = DateTime.Now;
            await _service.UpdateCompany(company_id, company);
            return Ok();
        }
    }
}
