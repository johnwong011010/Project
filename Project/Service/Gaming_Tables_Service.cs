using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Project.Model;

namespace Project.Service
{
    public class Gaming_Tables_Service
    {
        private readonly IMongoCollection<Gaming_Tables> _service;
        private readonly IMongoCollection<operation_logs> _Logservice;
        private readonly IMongoCollection<BsonDocument> _chipColor;
        private readonly IMongoCollection<BsonDocument> _denomination;
        public Gaming_Tables_Service(IOptions<Gaming_TablesDB> gamingdb, IOptions<operationDB> operationdb, IOptionsMonitor<ChipTypeDb> chipColorMonitor, IOptionsMonitor<denominationDB> monitor)
        {
            var mongoclient = new MongoClient(gamingdb.Value.ConnectionString);
            var dbname = mongoclient.GetDatabase(gamingdb.Value.DataBaseName);
            _service = dbname.GetCollection<Gaming_Tables>(gamingdb.Value.CollectionName);
            var opclient = new MongoClient(operationdb.Value.ConnectionString);
            var opdb = opclient.GetDatabase(operationdb.Value.DataBaseName);
            _Logservice = opdb.GetCollection<operation_logs>(operationdb.Value.CollectionName);
            var chipsClient = new MongoClient(chipColorMonitor.CurrentValue.ConnectionString);
            var chipDB = chipsClient.GetDatabase(chipColorMonitor.CurrentValue.DataBaseName);
            _chipColor = chipDB.GetCollection<BsonDocument>(chipColorMonitor.CurrentValue.CollectionName);
            var denominationClient = new MongoClient(monitor.CurrentValue.ConnectionString);
            var denominationDB = denominationClient.GetDatabase(monitor.CurrentValue.DataBaseName);
            _denomination = denominationDB.GetCollection<BsonDocument>(monitor.CurrentValue.CollectionName);
        }
        public async Task<List<Gaming_Tables>> GetAllTable() => await _service.Find(_ => true).ToListAsync();
        public async Task<Gaming_Tables> GetTable(string id) => await _service.Find(x => x._id == id).FirstOrDefaultAsync();
        public async Task<Gaming_Tables> getTable(string number) => await _service.Find(x => x.table_number == number).FirstOrDefaultAsync();
        public async Task UpdateTable(string id, Gaming_Tables t)
        {
            var table = await _service.Find(x => x._id == id).FirstOrDefaultAsync();
            DateTime time = DateTime.UtcNow;
            t.created_at = table.created_at;
            t.updated_at = time;
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
        public async Task<Dictionary<string, string>> GetChipsColor()
        {
            var bsonFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId("68099fb0f8495b183d214973"));
            var result = await _chipColor.Find(bsonFilter).FirstOrDefaultAsync();
            if (result is null) return null;
            return result.Elements.Where(e => e.Name != "_id").ToDictionary(e => e.Name, e => e.Value.AsString);
        }
        public async Task<Dictionary<string, int>> GetDenomination()
        {
            var bsonFilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId("68099ff3f8495b183d21497a"));
            var result = await _denomination.Find(bsonFilter).FirstOrDefaultAsync();
            if (result is null) return null;
            return result.Elements.Where(e => e.Name != "_id").ToDictionary(e => e.Name, e => e.Value.AsInt32);
        }
    }
}
