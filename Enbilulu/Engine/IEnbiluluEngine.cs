using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Enbilulu.Engine
{
    public interface IEnbiluluEngine
    {
        Task<IList<string>> ListStreams();

        Task<Stream> CreateStream(string streamName);

        Task<Stream> GetStream(string streamName);

        Task<int> PutRecord(string streamName, string data);

        Task<Section> GetRecords(string streamName, int id, int limit);
    }
}
