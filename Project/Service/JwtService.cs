using Microsoft.IdentityModel.Tokens;
using Project.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.HttpResults;

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
        //先驗證傳入的token是否合法
        //再來驗證該token的加密演算法
        //然後檢查payload的資料
        //傳回一個新的token
        private async Task<EmployeeLoginModel> RefreshToekn(EmployeeLoginModel model)
        {
            JwtSecurityTokenHandler tokenhandler = new JwtSecurityTokenHandler();
            var jwtsetting = _configuartion.GetSection("Jwt");
            try
            {
                if (model.Detail == null)
                {
                    return null;
                }
                var validsetting = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,//active the jwt lifetime
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtsetting["Jwt:Issuer"],//config the issuser
                    //ValidAudience = builder.Configuration["Jwt:Audience"],//config the audience
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtsetting["Jwt:Key"]))
                };
                ClaimsPrincipal tokenverify = tokenhandler.ValidateToken(model.Token, validsetting, out SecurityToken securityToken);
                if (securityToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (result == false)
                    {
                        return null;
                    }
                }
                string acc = tokenverify.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;//取得jwt中account的值
                string pm = tokenverify.Claims.SingleOrDefault(x => x.Type == ClaimTypes.UserData).Value;
                var emp = await _service.Find(x => x.Account == model.Account).FirstOrDefaultAsync();
                if (acc.Equals(emp.Account)&&pm.Equals(emp.Permission))
                {
                    var newToken = GenerateToken(emp);
                    model.Token = newToken;
                    return model;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception e)
            {
                return new EmployeeLoginModel { Detail = e.Message };
            }
        }
    }
}
