using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Project.Model;
namespace Project.Service
{
    public class OperationLogService
    {
        private readonly IMongoCollection<operation_logs> _service;
        public OperationLogService(IOptions<operationDB> connection)
        {
            var mongoclient = new MongoClient(connection.Value.ConnectionString);
            var dbname = mongoclient.GetDatabase(connection.Value.DataBaseName);
            _service = dbname.GetCollection<operation_logs>(connection.Value.CollectionName);
        }
        public async Task<List<operation_logs>> GetAllLogs() => await _service.Find(_ => true).ToListAsync();
        public async Task<List<operation_logs>> GetTableAllLogs(string tid) => await _service.Find(x => x.gaming_table_id == tid).ToListAsync();
        public async Task<List<operation_logs>> GetUserAllLogs(string user) => await _service.Find(x => x.performed_by == user).ToListAsync();
        public async Task WriteLog(operation_logs log) => await _service.InsertOneAsync(log);
    }
}
