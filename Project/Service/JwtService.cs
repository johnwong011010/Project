using Microsoft.IdentityModel.Tokens;
using Project.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace Project.Service
{
    public class JwtService
    {
        private readonly IConfiguration _configuartion;
        private readonly IMongoCollection<Employee> _service;
        public JwtService(IConfiguration configuartion,IOptions<EmployeeDB> connection) 
        {
            var mongoclient = new MongoClient(connection.Value.ConnectionString);
            var dbname = mongoclient.GetDatabase(connection.Value.DataBaseName);
            _service = dbname.GetCollection<Employee>(connection.Value.CollectionName);
            _configuartion = configuartion;
        }
        public async Task<EmployeeLoginModel> EmployeeLogin(EmployeeLoginModel model)
        {
            var employee = await _service.Find(x => x.Account == model.Account).FirstOrDefaultAsync();
            EmployeeLoginModel result = new EmployeeLoginModel { Account = model.Account, Password = model.Password};
            if (employee is null)
            {
                result.Detail = "Employee not found";
                return result;
            }
            //bool checkPassword = BCrypt.Net.BCrypt.Verify(model.Password, employee.Password);
            if (model.Password.Equals(employee.Password))
            {
                result.Detail = "Login success";
                result.Token = GenerateToken(employee);
                return result;
            }
            else
            {
                result.Detail = "Invalid Password";
                return result;
            }
        }
            
        public string GenerateToken(Employee employee)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuartion["Jwt:Key"]));//生成public key和private key
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);//用private key簽署證書

            var claim = new[] //jwt中儲存的資料
            {
                new Claim(ClaimTypes.NameIdentifier,employee.Account),
                new Claim(ClaimTypes.Name,employee.Name),
                new Claim(ClaimTypes.Role,employee.Role),
                new Claim(ClaimTypes.UserData,employee.Permission)//may need change
            };

            var token = new JwtSecurityToken(
                _configuartion["Jwt:Issuer"],
                _configuartion["Jwt:Audience"],
                claim,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
