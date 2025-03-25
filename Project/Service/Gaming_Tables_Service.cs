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
        public async Task<Gaming_Tables> GetTable(string id) => await _service.Find(x => x._id == id).FirstOrDefaultAsync();
        public async Task UpdateTable(string id, Gaming_Tables t)
        {
            var table = await _service.Find(x => x._id == id).FirstOrDefaultAsync();
            t.created_at = table.created_at;
            t.updated_at = DateTime.UtcNow;
            //need to write this operation to operation log
            await _service.ReplaceOneAsync(x => x._id == id, t);
        }
        public async Task UpdateTableStatus(string id, string status) 
        {
            var table = await _service.Find(x => x._id == id).FirstOrDefaultAsync();
            table.status = status;
            table.updated_at = DateTime.UtcNow;
            //need to write this operation to operation log
            await _service.ReplaceOneAsync(x => x._id == id, table);
        }
        public async Task DeleteTable(string id) => await _service.DeleteOneAsync(x => x._id == id);
        public async Task CreateTable(Gaming_Tables table) 
        {
            table.created_at = DateTime.UtcNow;
            table.updated_at = DateTime.UtcNow;
            //need to write this operation to operation log
            await _service.InsertOneAsync(table);
        }
    }
}
