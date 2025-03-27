using Project.Model;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace Project.Model
{
    public class RefreshTokenRepositoryClass : IRefreshTokenRepository
    {
        private readonly IMongoCollection<Employee> _service;
        public RefreshTokenRepositoryClass(IOptions<EmployeeDB> connection)
        {
            var mongoclient = new MongoClient(connection.Value.ConnectionString);
            var dbname = mongoclient.GetDatabase(connection.Value.DataBaseName);
            _service = dbname.GetCollection<Employee>(connection.Value.CollectionName);
        }
        public async Task<RefreshToken?> GetByToken(string token)//get the employee refresh token
        {
            var emp = await _service.Find(x => x.refreshToken.Token == token).FirstOrDefaultAsync();
            return emp.refreshToken;
        }
        public async Task Add(string id, RefreshToken token)//push the refresh token to db
        {
            var emp = await _service.Find(x => x._id == id).FirstOrDefaultAsync();
            emp.refreshToken = token;
            await _service.ReplaceOneAsync(x=>x._id==id,emp);
        }
        public async Task Update(string id, RefreshToken token)//update the refresh token in db
        {
            var emp = await _service.Find(x => x._id == id).FirstOrDefaultAsync();
            emp.refreshToken = token;
            await _service.ReplaceOneAsync(x => x._id == id, emp);
        }
        /*public async Task RevokeDescendantToken(string id, RefreshToken token, string ipaddress, string reason)
        {

        }*/


    }
}
