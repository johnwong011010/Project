using Microsoft.AspNetCore.Mvc;
using Project.Model;
using Project.Service;

namespace Project.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class EmployeeController : Controller
    {
        private readonly EmployeeService _service;
        private readonly JwtService _jwtService;
        public EmployeeController(EmployeeService service,JwtService jwtService) 
        {
            _service = service;
            _jwtService = jwtService;
        }
        [HttpGet]

        public async Task<List<Employee>> GetAllEmployee() => await _service.GetAllEmployee();
        [HttpGet("/api/[controller]/employee")]
        public async Task<Employee> GetEmployee(string role, string name) => await _service.GetEmployeeByRole(role, name);
        [HttpPost("/api/[controller]/employee")]
        public async Task AddEmployee(Employee emp) => await _service.AddEmployee(emp);
        [HttpPut("/api/[controller]/{id:length(24)}/status")]//change the employee status etc. active inactive
        public async Task ChnageEmployeeStatus(string id, string status) => await _service.ChangeEmployeeStatus(id, status);
        [HttpPost("/api/[controller]/login")]
        public async Task<ActionResult<EmployeeLoginModel>> Login(EmployeeLoginModel loginModel)
        {
            var result = await _jwtService.EmployeeLogin(loginModel);
            Response.Headers.Add("Authorization", "Bearer "+ result.Token);
            return Ok(result);
        }
        [HttpPost("/api/[controller]/refresh")]
        public async Task<ActionResult> Refresh(EmployeeLoginModel model)
        {
            EmployeeLoginModel el = await _jwtService.
        }
    }
}
