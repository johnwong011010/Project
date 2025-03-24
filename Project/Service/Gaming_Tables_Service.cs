using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Project.Model;

namespace Project.Service
{
    public class Gaming_Tables_Service
    {
        private readonly IMongoCollection<Gaming_Tables> _service;
        public Gaming_Tables_Service(IOptions<Gaming_TablesDB> gamingdb)
        {
            var mongoclient = new MongoClient(gamingdb.Value.ConnectionString);
            var dbname = mongoclient.GetDatabase(gamingdb.Value.DataBaseName);
            _service = dbname.GetCollection<Gaming_Tables>(gamingdb.Value.CollectionName);
        }
        public async Task<List<Gaming_Tables>> GetAllTable() => await _service.Find(_ => true).ToListAsync();
        public async Task UpdateTable(string id, Gaming_Tables t) => await _service.ReplaceOneAsync(x => x.Table_number == id, t);
        public async Task UpdateTableStatus(string id, string status) 
        {
            var table = await _service.Find(x => x.Table_number == id).FirstOrDefaultAsync();
            table.Status = status;
            await _service.ReplaceOneAsync(x => x.Table_number == id, table);
        }
        public async Task DeleteTable(string id) => await _service.DeleteOneAsync(x => x.Table_number == id);
    }
}
