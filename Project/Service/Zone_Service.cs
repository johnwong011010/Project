using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Project.Model;
namespace Project.Service
{
    public class Zone_Service
    {
        private readonly IMongoCollection<Zone> _service;
        public Zone_Service(IOptions<ZoneDB> connection)
        {
            var mongoclient = new MongoClient(connection.Value.ConnectionString);
            var dbname = mongoclient.GetDatabase(connection.Value.DataBaseName);
            _service = dbname.GetCollection<Zone>(connection.Value.CollectionName);
        }
        public async Task AddZone(Zone z) => await _service.InsertOneAsync(z);
        public async Task<List<Zone>> GetAllZone() => await _service.Find(_ => true).ToListAsync();
        public async Task<List<Zone>> GetZoneByArea(string aid) => await _service.Find(x => x.area_id == aid).ToListAsync();
        public async Task<List<Zone>> GetZoneByName(string name) => await _service.Find(x => x.name == name).ToListAsync();
        public async Task<Zone> GetZoneByID(string id) => await _service.Find(x => x._id == id).FirstOrDefaultAsync();//expect id is only
        public async Task UpdateZone(string id, Zone z) => await _service.ReplaceOneAsync(x => x._id == id, z);
        public async Task DeleteZone(string id) => await _service.DeleteOneAsync(x => x._id == id);
    }
}
