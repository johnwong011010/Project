using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Project.Model;
namespace Project.Service
{
    public class Pit_Service
    {
        private readonly IMongoCollection<Pit> _service;
        public Pit_Service(IOptions<PitDB> connection)
        {
            var mongoclient = new MongoClient(connection.Value.ConnectionString);
            var dbname = mongoclient.GetDatabase(connection.Value.DataBaseName);
            _service = dbname.GetCollection<Pit>(connection.Value.CollectionName);
        }
        public async Task AddPit(Pit p) => await _service.InsertOneAsync(p);
        public async Task<List<Pit>> GetAllPit() => await _service.Find(_ => true).ToListAsync();
        public async Task<List<Pit>> GetPitByZone(string zid) => await _service.Find(x => x.zone_id == zid).ToListAsync();
        public async Task<Pit> GetPitByName(string name) => await _service.AsQueryable().Where(x => x.name.ToLower().Contains(name)).FirstOrDefaultAsync();
        public async Task<Pit> GetPitByID(string id) => await _service.Find(x => x._id == id).FirstOrDefaultAsync();
        public async Task UpdatePit(string id, Pit p) => await _service.ReplaceOneAsync(x => x._id == id, p);
        public async Task DeletePit(string id,string isDelete)
        {
            var pit = await _service.Find(x => x._id == id).FirstOrDefaultAsync();
            pit.isDeleted = isDelete;
            await _service.ReplaceOneAsync(x => x._id == id, pit);
        }
    }
}
