using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dapper;

namespace Enbilulu.Engine.Sqlite
{
    public class SqliteEngine : IEnbiluluEngine
    {
        private const string STREAM_INIT = @"create table droplets (
           id integer PRIMARY KEY AUTOINCREMENT, 
           created_at datetime default (strftime('%Y-%m-%dT%H:%M:%SZ', 'now')), 
           payload text
        );";

        private const string STREAM_DETAILS = @"select count(*) as Points, max(id) as Last_Point from droplets;";

        private const string INSERT_POINT = @"insert into droplets (payload)
        values (@payload)";

        private const string GET_POINTS = @"select id, created_at, payload
        from droplets
        where id >= @id
        limit @limit";

        private const string GET_POINT_BEFORE_X = @"select id from droplets where id< @id order by id desc limit 1";

        private const string GET_POINT_AFTER_X = @"select id from droplets where id>@id order by id limit 1";

        private const string GET_EARLIEST_POINT = @"select min(id) as id from droplets";

        private const string GET_LAST_POINT = @"select max(id) as id from droplets";

        private static async Task<int?> GetPointAfterX(IDbConnection conn, GetStreamPositionConfig config)
        {
            return await conn.QueryFirstOrDefaultAsync<int?>(GET_POINT_AFTER_X, new { id = config.Position });
        }

        private static async Task<int?> GetPointBeforeX(IDbConnection conn, GetStreamPositionConfig config)
        {
            return await conn.QueryFirstOrDefaultAsync<int?>(GET_POINT_BEFORE_X, new { id = config.Position });
        }

        private static async Task<int?> GetEarliestPoint(IDbConnection conn, GetStreamPositionConfig config)
        {
            return await conn.QueryFirstOrDefaultAsync<int?>(GET_EARLIEST_POINT);
        }

        private static async Task<int?> GetLastPoint(IDbConnection conn, GetStreamPositionConfig config)
        {
            return await conn.QueryFirstOrDefaultAsync<int?>(GET_LAST_POINT);
        }

        private readonly Dictionary<FromPositionType, Func<IDbConnection, GetStreamPositionConfig, Task<int?>>> MapFromPositionToQuery = new Dictionary<FromPositionType, Func<IDbConnection, GetStreamPositionConfig, Task<int?>>>
        {
            [FromPositionType.after_point] = GetPointAfterX,
            [FromPositionType.before_point] = GetPointBeforeX,
            [FromPositionType.start] = GetEarliestPoint,
            [FromPositionType.end] = GetLastPoint
        };

        private string _workingDirectory;

        public SqliteEngine() : this(Environment.GetEnvironmentVariable("EnbiluluDataFolder")) { }

        public SqliteEngine(string workingDirectory)
        {
            _workingDirectory = Directory.GetCurrentDirectory();

            if (Directory.Exists(workingDirectory))
            {
                _workingDirectory = workingDirectory;
            }
        }

        private string GetStreamPath(string streamName)
        {
            return Path.Join(_workingDirectory, $"{streamName}.db");
        }

        private IDbConnection GetConnection(string path)
        {
            return new Microsoft.Data.Sqlite.SqliteConnection($"DataSource={path}");
        }

        public async Task<IList<string>> ListStreams()
        {
            var files = Directory.GetFiles(_workingDirectory, "*.db").Select(s => Path.GetFileName(s).Replace(".db", "")).ToList();
            return await Task.FromResult<IList<string>>(files);
        }

        public async Task<Stream> CreateStream(string streamName)
        {
            if (string.IsNullOrEmpty(streamName))
            {
                throw new ArgumentNullException(nameof(streamName), $"{nameof(streamName)} cannot be null");
            }

            string path = GetStreamPath(streamName);

            if (File.Exists(path))
            {
                return await GetStream(streamName);
            }

            File.WriteAllBytes(path, new byte[0]);

            using (var conn = GetConnection(path))
            {
                conn.Open();
                await conn.ExecuteAsync(STREAM_INIT);
            }

            return new Stream();
        }

        private async Task<int?> GetStreamPoint(IDbConnection conn, GetStreamPositionConfig config)
        {

            var task = MapFromPositionToQuery[config.Type];

            if (task == null)
            {
                return 0;
            }

            return await task.Invoke(conn, config);

        }

        public async Task<Stream> GetStream(string streamName)
        {
            if (string.IsNullOrEmpty(streamName))
            {
                throw new ArgumentNullException(nameof(streamName), $"{nameof(streamName)} cannot be null");
            }

            string path = GetStreamPath(streamName);

            if (!File.Exists(path))
            {
                return null;
            }

            using (var conn = GetConnection(path))
            {
                conn.Open();
                return await conn.QueryFirstOrDefaultAsync<Stream>(STREAM_DETAILS);
            }
        }

        public async Task<int> PutRecord(string streamName, string data)
        {
            if (string.IsNullOrEmpty(streamName))
            {
                throw new ArgumentNullException(nameof(streamName), $"{nameof(streamName)} cannot be null");
            }

            string path = GetStreamPath(streamName);

            if (!File.Exists(path))
            {
                throw new ArgumentException(nameof(streamName), $"{nameof(streamName)} not found");
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), $"{nameof(data)} cannot be null");
            }

            using (var conn = GetConnection(path))
            {
                conn.Open();

                conn.Execute(INSERT_POINT, new { payload = data });

                return await conn.ExecuteScalarAsync<int>("select last_insert_rowid();");
            }
        }

        public async Task<Section> GetRecords(string streamName, int id, int limit)
        {
            if (string.IsNullOrEmpty(streamName))
            {
                throw new ArgumentNullException(nameof(streamName), $"{nameof(streamName)} cannot be null");
            }

            string path = GetStreamPath(streamName);

            if (!File.Exists(path))
            {
                throw new ArgumentException(nameof(streamName), $"{nameof(streamName)} not found");
            }

            using (var conn = GetConnection(path))
            {
                conn.Open();

                var points = await conn.QueryAsync<Point>(GET_POINTS, new { id, limit });
                var lastPointInStream = await GetStreamPoint(conn, new GetStreamPositionConfig { Type = FromPositionType.end });

                if (points?.Count() > 0 && limit > 0)
                {
                    var lastPoint = points.Last().Id;
                    var nextPoint = await GetStreamPoint(conn, new GetStreamPositionConfig { Type = FromPositionType.after_point, Position = lastPoint });
                    var millisecondsBehind = (int)(DateTime.Now - points.Last().Created_At).TotalMilliseconds;

                    if (!nextPoint.HasValue)
                    {
                        return new Section { LastPoint = lastPoint, MillisecondsBehind = millisecondsBehind, Records = points };
                    }
                    return new Section { LastPoint = lastPoint, NextPoint = nextPoint, MillisecondsBehind = millisecondsBehind, Records = points };
                }
                
                if (id > lastPointInStream)
                {
                    return new Section { LastPoint = null, NextPoint = lastPointInStream, MillisecondsBehind = 0, Records = points };
                }
                return new Section { LastPoint = null, NextPoint = id, MillisecondsBehind = 0, Records = points };

            }

        }
    }
}
