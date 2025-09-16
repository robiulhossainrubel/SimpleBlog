using System.Diagnostics;
using System.Text;
using ClickHouse.Client.ADO;
using ClickHouse.Client.ADO.Parameters;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.Services.ActivityLogging
{
    public class UserActivityService : IUserActivityService
    {
        private readonly string _connStr;
        private readonly SemaphoreSlim _semaphore;

        public UserActivityService()
        {
            _connStr = "Host=ik8ltycloj.me-central-1.aws.clickhouse.cloud;Port=8443;User=default;Password=TTEl_vuaP~DT1;Database=default;Protocol=https;";
            //_connStr = "Host=localhost;Port=8123;Username=default;Password=;Database=default";
            
            // Limit concurrent database operations to prevent resource exhaustion
            _semaphore = new SemaphoreSlim(5, 5);
        }

        public void EnsureTables()
        {
            try
            {
                using var conn = new ClickHouseConnection(_connStr);
                conn.Open();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS user_activity_log
                (
                    Id UUID DEFAULT generateUUIDv4(),
                    EventTime DateTime DEFAULT now(),
                    UserId UInt64,
                    Controller String,
                    Action String
                )
                ENGINE = MergeTree()
                PARTITION BY toYYYYMM(EventTime)
                ORDER BY (EventTime, UserId);";

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to ensure ClickHouse tables: {ex.Message}");
            }
        }

        public async Task LogActivityBulkAsync(List<UserActivityLog> activities)
        {
            if (activities == null || activities.Count == 0)
                return;

            // Don't block if too many operations are already in progress
            if (!await _semaphore.WaitAsync(TimeSpan.FromMilliseconds(500)))
            {
                Console.WriteLine($"Skipping bulk log insert due to high load. Queue size: {activities.Count}");
                return;
            }

            try
            {
                var sql = new StringBuilder();
                sql.Append("INSERT INTO user_activity_log (EventTime, UserId, Controller, Action) VALUES ");

                for (int i = 0; i < activities.Count; i++)
                {
                    var act = activities[i];

                    string eventTime = act.EventTime.ToString("yyyy-MM-dd HH:mm:ss");
                    string userId = act.UserId.ToString();
                    string controller = act.Controller.Replace("'", "''");
                    string action = act.Action.Replace("'", "''");

                    if (i > 0)
                        sql.Append(", ");

                    sql.Append($"('{eventTime}', {userId}, '{controller}', '{action}')");
                }

                string commandText = sql.ToString();

                using var conn = new ClickHouseConnection(_connStr);
                await conn.OpenAsync();

                using var cmd = conn.CreateCommand();
                cmd.CommandText = commandText;

                await cmd.ExecuteNonQueryAsync();

                Console.WriteLine($"Inserted {activities.Count} logs at {DateTime.UtcNow:HH:mm:ss.fff}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ClickHouse bulk insert failed: {ex.Message}");
            }
            finally
            {
                try
                {
                    _semaphore.Release();
                }
                catch
                {
                    // Ignore semaphore release errors
                }
            }
        }

        public void Dispose()
        {
            try
            {
                _semaphore?.Dispose();
            }
            catch
            {
                // Ignore disposal errors
            }
        }
    }
}
