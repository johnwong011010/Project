using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Project.Model;
namespace Project.Service
{
    public class AreaService
    {
        private readonly IMongoCollection<Area> _service;
        public AreaService(IOptions<AreaDB> connection)
        {
            var mongoclient = new MongoClient(connection.Value.ConnectionString);
            var dbname = mongoclient.GetDatabase(connection.Value.DataBaseName);
            _service = dbname.GetCollection<Area>(connection.Value.CollectionName);
        }
        public async Task AddArea(Area area) => await _service.InsertOneAsync(area);
        public async Task<List<Area>> GetAllArea() => await _service.Find(_ => true).ToListAsync();
        public async Task<Area> GetAreaByID(string id) => await _service.Find(x => x._id == id).FirstOrDefaultAsync();
        public async Task<List<Area>> GetAreaByCompanyID(string cid) => await _service.Find(x => x.sub_company_id == cid).ToListAsync();//may have many area in this company
        public async Task<Area> GetAreaByName(string name) => await _service.AsQueryable().Where(x => x.name.ToLower().Contains(name)).FirstOrDefaultAsync();
        public async Task UpdateArea(string id, Area a) => await _service.ReplaceOneAsync(x => x._id == id, a);
        public async Task DeleteArea(string id, string isDelete)//markdown this area is delete already
        {
            var area = await _service.Find(x => x._id == id).FirstOrDefaultAsync();
            area.isDeleted = isDelete;
            await _service.ReplaceOneAsync(x => x._id == id, area);
        }
    }
}
