using System.Diagnostics;
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
            using var conn = new ClickHouseConnection(_connStr);
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
            INSERT INTO user_activity_log (EventTime, UserId, Controller, Action)
            VALUES (@time, @user, @controller, @action)";

            foreach (var act in activities)
            {
                cmd.Parameters.Add(new ClickHouseDbParameter { ParameterName = "time", Value = act.EventTime });
                cmd.Parameters.Add(new ClickHouseDbParameter { ParameterName = "user", Value = act.UserId });
                cmd.Parameters.Add(new ClickHouseDbParameter { ParameterName = "controller", Value = act.Controller });
                cmd.Parameters.Add(new ClickHouseDbParameter { ParameterName = "action", Value = act.Action });
            }

            await cmd.ExecuteNonQueryAsync();
        }

    }

}
