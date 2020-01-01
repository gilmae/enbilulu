using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Dapper;
using Newtonsoft.Json;

namespace Enbilulu
{
    public class Db
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

        public enum FromPositionType
        {
            after_point,
            before_point,
            start,
            end
        }

        public struct GetStreamPositionConfig
        {
            public FromPositionType Type { get; set; }
            public int Position { get; set; }
        }

        private static int GetPointAfterX(IDbConnection conn, GetStreamPositionConfig config)
        {
            return conn.QueryFirstOrDefault<int>(GET_POINT_AFTER_X, new { id = config.Position });
        }

        private static int GetPointBeforeX(IDbConnection conn, GetStreamPositionConfig config)
        {
            return conn.QueryFirstOrDefault<int>(GET_POINT_BEFORE_X, new { id = config.Position });
        }

        private static int GetEarliestPoint(IDbConnection conn, GetStreamPositionConfig config)
        {
            return conn.QueryFirstOrDefault<int>(GET_EARLIEST_POINT);
        }

        private static int GetLastPoint(IDbConnection conn, GetStreamPositionConfig config)
        {
            return conn.QueryFirstOrDefault<int>(GET_LAST_POINT);
        }

        private readonly Dictionary<FromPositionType, Func<IDbConnection, GetStreamPositionConfig, int>> MapFromPositionToQuery = new Dictionary<FromPositionType, Func<IDbConnection, GetStreamPositionConfig, int>> {
            [FromPositionType.after_point] = GetPointAfterX,
            [FromPositionType.before_point] = GetPointBeforeX,
            [FromPositionType.start] = GetEarliestPoint,
            [FromPositionType.end] = GetLastPoint
        } ;

        private string _workingDirectory;

        public Db() : this(Directory.GetCurrentDirectory()) { }

        public Db(string workingDirectory)
        {
            _workingDirectory = Directory.GetCurrentDirectory();

            if (Directory.Exists(workingDirectory))
            {
                _workingDirectory = workingDirectory;
            }
        }


        private string ValidateStreamExists(string streamName)
        {
            if (string.IsNullOrEmpty(streamName))
            {
                throw new ArgumentNullException(nameof(streamName), $"{nameof(streamName)} cannot be null");
            }

            string path = Path.Join(_workingDirectory, $"{streamName}.db");

            if (!File.Exists(path))
            {
                throw new ArgumentException($"{nameof(streamName)} does not exists", nameof(streamName));
            }

            return path;
        }

        private string ValidateStreamDoesNotExist(string streamName)
        {
            if (string.IsNullOrEmpty(streamName))
            {
                throw new ArgumentNullException(nameof(streamName), $"{nameof(streamName)} cannot be null");
            }

            string path = Path.Join(_workingDirectory, $"{streamName}.db");

            if (File.Exists(path))
            {
                throw new ArgumentException($"{nameof(streamName)} already exists", nameof(streamName));
            }
            return path;
        }

        private IDbConnection GetConnection(string path)
        {
            return new Microsoft.Data.Sqlite.SqliteConnection($"DataSource={path}");
        }

        public Stream CreateStream(string streamName)
        {
            string path = ValidateStreamDoesNotExist(streamName);

            File.WriteAllBytes(path, new byte[0]);

            using (var conn = GetConnection(path))
            {
                conn.Open();
                conn.Execute(STREAM_INIT);
            }

            return new Stream();
        }

        public int GetStreamPoint(string streamName, GetStreamPositionConfig config)
        {
            string path = ValidateStreamExists(streamName);

            using (var conn = GetConnection(path))
            {
                conn.Open();

                return GetStreamPoint(conn, config);
            }
        }

        private int GetStreamPoint(IDbConnection conn, GetStreamPositionConfig config)
        {
  
                var task = MapFromPositionToQuery[config.Type];

                if (task == null)
                {
                    return 0;
                }

                return task.Invoke(conn, config);
            
        }

        public Stream GetStream(string streamName)
        {
            string path = ValidateStreamExists(streamName);

            using (var conn = GetConnection(path))
            {
                conn.Open();
                return conn.Query<Stream>(STREAM_DETAILS).FirstOrDefault();
            }
        }

        public int PutRecord(string streamName, object data)
        {
            string path = ValidateStreamExists(streamName);

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), $"{nameof(data)} cannot be null");
            }

            using (var conn = GetConnection(path))
            {
                conn.Open();
                string payload = JsonConvert.SerializeObject(data);
                conn.Execute(INSERT_POINT, new { payload });

                return conn.ExecuteScalar<int>("select last_insert_rowid();");
            }
        }

        public Section GetRecords(string streamName, int id, int limit)
        {
            string path = ValidateStreamExists(streamName);

            using (var conn = GetConnection(path))
            {
                conn.Open();

                var points = conn.Query<Point>(GET_POINTS, new { id, limit });
                

                if (points?.Count() > 0)
                {
                    var lastPoint = points.Last().Id;
                    var nextPoint = GetStreamPoint(conn, new GetStreamPositionConfig { Type = FromPositionType.after_point, Position = lastPoint });
                    var millisecondsBehind = (int)(DateTime.Now - points.Last().Created_At).TotalMilliseconds;

                    return new Section { LastPoint = lastPoint, NextPoint = nextPoint, MillisecondsBehind = millisecondsBehind, Records = points };
                }
                else
                {
                    return new Section { LastPoint = null, NextPoint = id, MillisecondsBehind = 0, Records = points };
                }
            }

        }
    }
}
