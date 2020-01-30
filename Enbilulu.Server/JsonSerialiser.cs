using System;
using System.Collections.Generic;
using System.IO;
using Nancy.Responses.Negotiation;

namespace EnbiluluServer
{
    public class JsonSerialiser : Nancy.ISerializer
    {
        public JsonSerialiser()
        {
        }

        public IEnumerable<string> Extensions => throw new NotImplementedException();

        public bool CanSerialize(MediaRange mediaRange)
        {
            return true;
        }

        public void Serialize<TModel>(MediaRange mediaRange, TModel model, System.IO.Stream outputStream)
        {

            using (var writer = new Newtonsoft.Json.JsonTextWriter(new StreamWriter(outputStream)))
            {

                new Newtonsoft.Json.JsonSerializer().Serialize(writer, model);
            }
        }
    }
}
