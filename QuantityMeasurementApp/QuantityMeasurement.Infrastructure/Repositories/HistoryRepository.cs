using QuantityMeasurement.Infrastructure.Interfaces;
using QuantityMeasurement.Infrastructure.Models;

using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;


namespace QuantityMeasurement.Infrastructure.Repositories;

public class HistoryRepository : IHistoryRepository
{

    private readonly IDistributedCache _cache;
    private const string HistoryCacheKey = "history:all";

    private readonly string _connectionString;

    private static readonly DistributedCacheEntryOptions CacheOptions = new DistributedCacheEntryOptions
    {
      AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)  // Cache expiration set to 30 minutes
    };

    public HistoryRepository( IConfiguration config , IDistributedCache cache)
    {

        _cache = cache;
        _connectionString = config.GetConnectionString("DefaultConnection");
    }

    public void Save(HistoryRecord historyRecord)
    {
        using var conn = new SqlConnection(_connectionString);
        string query = @"INSERT INTO History 
        (Operation, Value1, Unit1, Value2, Unit2, TargetUnit, _Scalar, Result, ResultUnit, CreatedAt)
        VALUES 
        (@Operation, @Value1, @Unit1, @Value2, @Unit2, @TargetUnit, @Scalar, @Result, @ResultUnit, @CreatedAt)";
        using var cmd = new SqlCommand ( query , conn);

        cmd.Parameters.AddWithValue("@Operation" ,  historyRecord.Operation );
        cmd.Parameters.AddWithValue("@Value1" , historyRecord.Value1);
        cmd.Parameters.AddWithValue("@Unit1" , historyRecord.Unit1);
        cmd.Parameters.AddWithValue("@Value2" , (object?)historyRecord.Value2 ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Unit2" , (object?)historyRecord.Unit2 ?? DBNull.Value); 
        cmd.Parameters.AddWithValue("@TargetUnit" , (object?)historyRecord.TargetUnit ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Scalar" , (object?)historyRecord.Scalar ?? DBNull.Value ) ;
        cmd.Parameters.AddWithValue("@Result" , historyRecord.Result);
        cmd.Parameters.AddWithValue("@ResultUnit" , (object?)historyRecord.ResultUnit ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CreatedAt" , historyRecord.CreatedAt);
        conn.Open();
        cmd.ExecuteNonQuery();
        // Invalidation of cache after write opertaion
        _cache.Remove(HistoryCacheKey);
    }

    public List<HistoryRecord> GetHistory()
    {
        // Try first to get from cache
        var cachedHistory = _cache.Get(HistoryCacheKey);
        if (cachedHistory != null)
        {
            var cachedJson = Encoding.UTF8.GetString(cachedHistory);
            try
            {
                var cachedList = JsonSerializer.Deserialize<List<HistoryRecord>>(cachedJson);
                if (cachedList != null) return cachedList;
            }
            catch
            {
                // Failed to deserialize : fall through To Db read and refresh  cache
            }
        }
        // Cached not found, read from Db and refresh cache
        var result = new List<HistoryRecord>();
        using var conn = new SqlConnection(_connectionString);
        string query = "SELECT Id, Operation, Value1, Unit1, Value2, Unit2, TargetUnit, _Scalar, Result, ResultUnit, CreatedAt FROM History Order By CreatedAt DESC";
        using var cmd = new SqlCommand(query, conn);
        conn.Open();
        using SqlDataReader reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new HistoryRecord
            {
                Id = reader.GetInt32(0),
                Operation = reader.GetString(1),
                Value1 = reader.GetDouble(2),
                Unit1 = reader.GetString(3),
                Value2 = reader.IsDBNull(4) ? null : reader.GetDouble(4),
                Unit2 = reader.IsDBNull(5) ? null : reader.GetString(5),
                TargetUnit = reader.IsDBNull(6) ? null : reader.GetString(6),
                Scalar = reader.IsDBNull(7) ? null : reader.GetDouble(7),
                Result = reader.GetDouble(8),
                ResultUnit = reader.IsDBNull(9) ? null : reader.GetString(9),
                CreatedAt = reader.GetDateTime(10)
            });
        }
        
        // Also store it in cache for future use
        var json = JsonSerializer.Serialize(result);
        _cache.Set(HistoryCacheKey , Encoding.UTF8.GetBytes(json), CacheOptions);

        return result; // after saving it in cache safely now return it
    }

    public void ClearHistory()
    {
        // As table is small, remove all rows
        // _db.Histories.RemoveRange(_db.Histories);
        // _db.SaveChanges();

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("DELETE FROM History", conn);
        conn.Open();
        cmd.ExecuteNonQuery();

        // Also invalidate or clear the cache as well
        _cache.Remove(HistoryCacheKey);
    }
}