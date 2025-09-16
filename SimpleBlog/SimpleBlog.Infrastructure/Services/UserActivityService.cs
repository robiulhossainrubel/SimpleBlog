using System.Diagnostics;
using System.Text;
using ClickHouse.Client.ADO;
using ClickHouse.Client.ADO.Parameters;
using SimpleBlog.Application.Interface;
using SimpleBlog.Domain.Entities;

namespace SimpleBlog.Infrastructure.Services
{
    public class UserActivityService : IUserActivityService
    {
        private readonly string _connStr;

        public UserActivityService()
        {
            _connStr = "Host=ik8ltycloj.me-central-1.aws.clickhouse.cloud;Port=8443;User=default;Password=TTEl_vuaP~DT1;Database=default;Protocol=https;";
            //_connStr = "Host=localhost;Port=8123;Username=default;Password=;Database=default";
        }

        public void EnsureTables()
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

        public void LogActivity(UserActivityLog activity)
        {
            using var conn = new ClickHouseConnection(_connStr);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            INSERT INTO user_activity_log (EventTime, UserId, Controller, Action)
            VALUES (@time, @user, @controller, @action)";

            cmd.Parameters.Add(new ClickHouseDbParameter { ParameterName = "time", Value = activity.EventTime });
            cmd.Parameters.Add(new ClickHouseDbParameter { ParameterName = "user", Value = activity.UserId });
            cmd.Parameters.Add(new ClickHouseDbParameter { ParameterName = "controller", Value = activity.Controller });
            cmd.Parameters.Add(new ClickHouseDbParameter { ParameterName = "action", Value = activity.Action });

            cmd.ExecuteNonQuery();
        }

        public async Task LogActivityBulkAsync(List<UserActivityLog> activities)
        {
            if (activities == null || activities.Count == 0)
                return;

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
        }

    }

}
