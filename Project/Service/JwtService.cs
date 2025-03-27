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
using System.Security.Cryptography;

namespace Project.Service
{
    public class JwtService
    {
        private readonly IConfiguration _configuartion;
        private readonly IRefreshTokenRepository _repository;
        private readonly IMongoCollection<Employee> _service;
        public JwtService(IConfiguration configuartion,IOptions<EmployeeDB> connection,IRefreshTokenRepository repository) 
        {
            var mongoclient = new MongoClient(connection.Value.ConnectionString);
            var dbname = mongoclient.GetDatabase(connection.Value.DataBaseName);
            _service = dbname.GetCollection<Employee>(connection.Value.CollectionName);
            _configuartion = configuartion;
            _repository = repository;
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
                var refrehtoken = GenerateRefreshToken(model.Account, model.Ipaddress);
                employee.refreshToken = refrehtoken;
                await _service.ReplaceOneAsync(x => x._id == employee._id, employee);//store the refresh token
                result.Detail = "Login success";
                result.Token = GenerateToken(employee);
                result.RefreshToken = refrehtoken;
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
                expires: DateTime.UtcNow.AddMinutes(10),//該token的有效時間
                signingCredentials: credentials
            );
            var accestoken = new JwtSecurityTokenHandler().WriteToken(token);
            return accestoken;//該Token只是允許短時間訪問服務的token
        }
        public RefreshToken GenerateRefreshToken(string useracc,string userIP)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7),
                Create = DateTime.UtcNow,
                CreatedIP = userIP,
                UserAcc = useracc
            };
            return refreshToken;
        }
        //先驗證傳入的token是否合法和refresh token是否過期和是否被使用過
        //再來驗證該token的加密演算法
        //然後檢查payload的資料
        //傳回一個新的token
        public async Task<AuthResponse> RefreshToken(string token,string refreshtoken,string ipaddress)
        {
            var principal = GetClaimsPrincipalFromExpiredToken(token);
            var useracc = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var storedRefreshToken = await _repository.GetByToken(refreshtoken);//get the refresh token from db
            if (storedRefreshToken == null || storedRefreshToken.UserAcc != useracc || 
                storedRefreshToken.Expires< DateTime.UtcNow || storedRefreshToken.Revoked != null)
            {
                throw new SecurityTokenException("Invalid refresh token");
            }
            var emp = await _service.Find(x => x.Name == principal.FindFirstValue(ClaimTypes.Name) && 
            x.Role == principal.FindFirstValue(ClaimTypes.Role)).FirstOrDefaultAsync();
            var newToken = GenerateToken(emp);
            var newRefreshToken = GenerateRefreshToken(useracc, ipaddress);
            await _repository.Update(emp._id,storedRefreshToken);
            await _repository.Add(emp._id,newRefreshToken);
            return new AuthResponse
            {
                AccessToken = newToken,
                RefreshToken = newRefreshToken.Token
            };

        }
        private ClaimsPrincipal GetClaimsPrincipalFromExpiredToken(string token)//檢查access token有沒有遭到修改
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuartion["Jwt:Key"])),
                ValidateLifetime = false // 故意關閉有效期驗證
            };
            var tokenhandler = new JwtSecurityTokenHandler();
            var principal = tokenhandler.ValidateToken(token, tokenValidationParameters, out _);
            return principal;//return an object 
        }
    }
}
