using System.ComponentModel.DataAnnotations;

namespace Project.Model
{
    public class EmployeeLoginModel
    {
        [Required]
        public string? Account { get; set; }
        [Required]
        public string? Password { get; set; }
        //public string? Ipaddress { get; set; }
        public bool? autoLogin { get; set; }
        public string? Detail { get; set; }
        public string? Role { get; set; }
        public string? Name { get; set; }
        public string? Token { get; set; }
        public RefreshToken? RefreshToken { get; set; }
    }
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Create { get; set; }
        public string? CreatedIP { get; set; }
        public DateTime? Revoked { get; set; }
        public string? RevokedIP { get; set; }
        public string? ReplaceToken { get; set; }
        public string? UserAcc { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsActive => Revoked == null && !IsExpired;//檢查這個refresh token是不是已經過期又或者是已經被使用過了
    }
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByToken(string token);
        Task Add(string id,RefreshToken token);
        Task Update(string id,RefreshToken token);
        //Task RevokeDescendantToken(string id, RefreshToken token, string ipaddress, string reason);
    }
    public class RefreshTokenRequset
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
    public class AuthResponse
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
    }
}
