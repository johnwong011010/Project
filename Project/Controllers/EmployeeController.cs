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
using Microsoft.EntityFrameworkCore;
using System.Security;

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
                    Join_in = alldata[i].Join_in,
                    Quit_in = alldata[i].Quit_in,
                    Permission = alldata[i].Permission,
                    Status = alldata[i].Status,
                };
            }
            return data;
        }
        [HttpGet("/api/[controller]/employee")]
        public async Task<ActionResult<Employee>> GetEmployee(string role, string name)
        {
            var emp = await _service.GetEmployeeByRole(role, name);
            var permit = GetPermission().Result;//return a array of permission
            string[] keys = permit.Keys.ToArray();
            string ep = "";//格式化的權限
            foreach (string key in keys)
            {
                if (emp.Permission.Contains(key))
                {
                    ep += permit.GetValueOrDefault(key);
                    ep += "_";
                }
            }
            if (ep.EndsWith("_"))
            {
                ep = ep.Remove(ep.Length - 1);//檢查是否到最後,不是到最後也要加底線做區分
            }
            emp.Permission = ep;
            return Ok(emp);
        }
        [HttpGet("/api/[controller]/eid")]
        public async Task<ActionResult<EmployeeDTO?>> GetEmployee(string eid)
        {
            var employee = await _service.GetEmployeeByEid(eid);
            var permit = GetPermission().Result;
            string[] keys = permit.Keys.ToArray(); //將權限的代表數字取出 
            string ep = "";//格式化的權限
            foreach(string key in keys)
            {
                if (employee.Permission.Contains(key))
                {
                    ep += permit.GetValueOrDefault(key);
                    ep += "_";
                }
            }
            if (ep.EndsWith("_"))
            {
                ep = ep.Remove(ep.Length - 1);//檢查是否到最後,不是到最後也要加底線做區分
            }
            if (employee != null)
            {
                EmployeeDTO data = new EmployeeDTO
                {
                    _id = employee._id,
                    Employee_Id = employee.Employee_Id,
                    Account = employee.Account,
                    Name = employee.Name,
                    Role = employee.Role,
                    Permission = ep,
                    Join_in = employee.Join_in,
                    Quit_in = employee.Quit_in,
                    Status = employee.Status,
                };
                return Ok(data);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpGet("/api/[controller]/permission")]
        private async Task<Dictionary<string,string>> GetPermission()
        {
            var result = await _service.GetPermission();
            return result;
        }
        [HttpGet("/api/[controller]/permit")]
        public async Task<ActionResult> GetPermit()
        {
            var result = await _service.GetPermit();
            return Ok(result.ToJson());
        }
        [HttpPost("/api/[controller]/employee")]
        public async Task<ActionResult> AddEmployee(Employee emp)
        {
            emp.Password = BCrypt.Net.BCrypt.HashPassword(emp.Password, workFactor: 12);;
            emp.Quit_in = DateTime.Now;
            var permit = GetPermission().Result;
            string p = "";
            foreach (var per in permit)
            {
                if (emp.Permission.ToLower().Contains(per.Value)) //因為UI側的第一個英文字母為大寫，因此轉換成小寫再對比
                {
                    p += per.Key;
                    p += "_";
                }
            }
            if (p.EndsWith("_")) p = p.Remove(p.Length - 1);
            emp.Permission = p;
            await _service.AddEmployee(emp);
            return Ok();
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
            await _service.Update(eid, role, employee.Permission);
            return Ok();
        }
        [HttpPatch("/api/[controller]/{eid}/permit")]//change employee role
        public async Task<ActionResult> ChangeEmployeePermission(string eid, string permission)
        {
            var employee = await _service.GetEmployeeByEid(eid);
            var permit = GetPermission().Result;
            string p = "";
            foreach(var per in permit)
            {
                if (permission.Contains(per.Value))
                {
                    p += per.Key;
                    p += "_";
                }
            }
            if (p.EndsWith("_")) p = p.Remove(p.Length - 1); // 移除不必要的底線
            await _service.Update(eid, employee.Role, p);
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
