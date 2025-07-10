using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Project.Model;
namespace Project.Service
{
    public class Sub_Company_Service
    {
        private readonly IMongoCollection<sub_companies> _service;
        public Sub_Company_Service(IOptions<sub_companiesDB> connection)
        {
            var mongoclient = new MongoClient(connection.Value.ConnectionString);
            var dbname = mongoclient.GetDatabase(connection.Value.DataBaseName);
            _service = dbname.GetCollection<sub_companies>(connection.Value.CollectionName);
        }
        public async Task<List<sub_companies>> GetAllCompany() => await _service.Find(_ => true).ToListAsync();
        public async Task<sub_companies> GetCompany(string id) => await _service.Find(x => x._id == id).FirstOrDefaultAsync();
        public async Task<sub_companies> GetCompanyByName(string name) => await _service.AsQueryable().Where(x => x.name.ToLower().Contains(name.ToLower())).FirstOrDefaultAsync();
        public async Task UpdateCompany(string id, sub_companies companies) => await _service.ReplaceOneAsync(x => x._id == id, companies);
        public async Task AddCompany(sub_companies companies) => await _service.InsertOneAsync(companies);
        public async Task DeleteCompany(string id) => await _service.DeleteOneAsync(x => x._id == id);
    }
}
