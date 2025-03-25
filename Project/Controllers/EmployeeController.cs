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
        public EmployeeController(EmployeeService service) => _service = service;
        [HttpGet]

        public async Task<List<Employee>> GetAllEmployee() => await _service.GetAllEmployee();
        [HttpGet]
        public async Task<Employee> GetEmployee(string role, string name) => await _service.GetEmployeeByRole(role, name);
        [HttpPost]
        public async Task AddEmployee(Employee emp) => await _service.AddEmployee(emp);
        [HttpPut]//change the employee status etc. active inactive
        public async Task ChnageEmployeeStatus(string id, string status) => await _service.ChangeEmployeeStatus(id, status);
    }
}
