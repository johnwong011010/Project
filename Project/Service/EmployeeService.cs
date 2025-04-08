using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Project.Model;
namespace Project.Service
{
    public class EmployeeService
    {
        private readonly IMongoCollection<Employee> _service;
        public EmployeeService(IOptions<EmployeeDB> connection)
        {
            var mongoclient = new MongoClient(connection.Value.ConnectionString);
            var dbname = mongoclient.GetDatabase(connection.Value.DataBaseName);
            _service = dbname.GetCollection<Employee>(connection.Value.CollectionName);
        }
        public async Task<List<Employee>> GetAllEmployee() => await _service.Find(_ => true).ToListAsync();
        public async Task<Employee> GetEmployeeByRole(string role, string name) => await _service.Find(x => x.Role == role && x.Name == name).FirstOrDefaultAsync();
        public async Task<Employee> GetEmployeeByEid(string eid) => await _service.Find(x => x.Employee_Id == eid).FirstOrDefaultAsync();
        public async Task AddEmployee(Employee emp) => await _service.InsertOneAsync(emp);
        public async Task Update(string id,string role,string permission)//permission change still need to think
        {
            var emp = await _service.Find(x => x.Employee_Id == id).FirstOrDefaultAsync();
            if (!emp.Role.Equals(role))//when the input role not same to database,thats mean the role need to change
            {
                emp.Role = role;
            }
            emp.Permission = permission;
            await _service.ReplaceOneAsync(x => x.Employee_Id == id, emp);
        }
        public async Task ChangeEmployeeStatus(string id,string status)
        {
            var emp = await _service.Find(x => x.Employee_Id == id).FirstOrDefaultAsync();
            emp.Status = status;//change this employee status
            await _service.ReplaceOneAsync(x => x.Employee_Id == id, emp);
        }
    }
}
