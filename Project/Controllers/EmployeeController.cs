using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Project.Model;
using Project.Service;
using BCrypt.Net;
using Microsoft.AspNetCore.Cors;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace Project.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [EnableCors("ReactAppPolicy")]
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

        public async Task<EmployeeDTO[]> GetAllEmployee()
        {
            var alldata = await _service.GetAllEmployee();
            EmployeeDTO[] data= new EmployeeDTO[alldata.Count()];
            for (int i = 0; i < alldata.Count(); i++)
            {
                data[i] = new EmployeeDTO
                {
                    _id = alldata[i]._id,
                    Employee_Id = alldata[i].Employee_Id,
                    Account = alldata[i].Account,
                    Name = alldata[i].Name,
                    Role = alldata[i].Role,
                    //Join_in = alldata[i].Join_in,
                    //Quit_in = alldata[i].Quit_in,
                    Permission = alldata[i].Permission,
                    Status = alldata[i].Status,
                };
            }
            return data;
        }
        [HttpGet("/api/[controller]/employee")]
        public async Task<Employee> GetEmployee(string role, string name) => await _service.GetEmployeeByRole(role, name);
        [HttpGet("/api/[controller]/eid")]
        public async Task<ActionResult<EmployeeDTO?>> GetEmployee(string eid)
        {
            var employee = await _service.GetEmployeeByEid(eid);
            if (employee != null)
            {
                EmployeeDTO data = new EmployeeDTO
                {
                    _id = employee._id,
                    Employee_Id = employee.Employee_Id,
                    Account = employee.Account,
                    Name = employee.Name,
                    Role = employee.Role,
                    Permission = employee.Permission,
                    Status = employee.Status,
                };
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost("/api/[controller]/employee")]
        public async Task AddEmployee(Employee emp)
        {
            emp.Password = BCrypt.Net.BCrypt.HashPassword(emp.Password, workFactor: 12);;
            await _service.AddEmployee(emp);
        }
        [HttpPatch("/api/[controller]/{id}/status")]//change the employee status etc. active inactive
        public async Task<ActionResult> ChnageEmployeeStatus(string id, string status)
        {
            await _service.ChangeEmployeeStatus(id, status);
            return Ok();
        }
        [HttpPatch("/api/[controller]/{eid}/role")]//change employee role
        public async Task<ActionResult> ChangeEmployeeRole(string eid,string role)
        {
            var employee = await _service.GetEmployeeByEid(eid);
            employee.Role = role;
            await _service.Update(eid, role, employee.Permission);
            return Ok();
        }
        /*[HttpOptions("/api/[controller]/login")]
        [EnableCors("ReactAppPolicy")]
        public ActionResult Login()
        {
            return Ok();
        }*/
        [HttpPost("/api/[controller]/login")]
        [EnableCors("ReactAppPolicy")]
        public async Task<ActionResult<EmployeeLoginModel?>> Login([FromBody] EmployeeLoginModel employee)
        {
            var result = await _jwtService.EmployeeLogin(employee.Account,employee.Password);
            if (result.Detail == "Employee not found")
            {
                return NotFound(result);
            }
            if (result.Detail == "Invalid Password")
            {
                return Unauthorized(result);
            }
            Response.Headers.Add("Authorization", "Bearer "+ result);
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
