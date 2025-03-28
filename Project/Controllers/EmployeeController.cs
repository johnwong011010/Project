using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Project.Model;
using Project.Service;
using BCrypt.Net;

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
        public async Task AddEmployee(Employee emp)
        {
            emp.Password = BCrypt.Net.BCrypt.HashPassword(emp.Password, workFactor: 12);;
            await _service.AddEmployee(emp);
        }
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
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequset requset)
        {
            try
            {
                var ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString();//取得發起requset的ip地址
                var response = await _jwtService.RefreshToken(
                    requset.AccessToken,
                    requset.RefreshToken,
                    ipaddress
                    );
                return Ok(response);
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(new { message = e });
            }
        }
    }
}
