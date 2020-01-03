using System;
using System.IO;
using Nancy;

namespace Enbilulu
{
    public class StreamHandler : NancyModule
    {
        public StreamHandler()
        {
            Get("/streams/{stream}", p =>
            {
                var stream = new Db(Environment.GetEnvironmentVariable("DataFolder")).GetStream(p.stream);


                var response =  new Nancy.Responses.JsonResponse<Stream>(stream, new JsonSerialiser(), this.Context.Environment);
                response.ContentType = "application/json";

                return response;
            });

            Get("/points/{stream}/{point}/{limit}", p =>
            {
                var data = new Db(Environment.GetEnvironmentVariable("DataFolder")).GetRecords(p.stream, p.from, p.limit);
                return Newtonsoft.Json.JsonConvert.SerializeObject(data);
            });

            Post("/streams/{stream}", p =>
            {
                var stream = new Db(Environment.GetEnvironmentVariable("DataFolder")).CreateStream(p.stream);
                return Newtonsoft.Json.JsonConvert.SerializeObject(stream);
            });

            Post("/points/{stream}", p =>
            {
                string data = "";

                using (StreamReader reader = new StreamReader(this.Request.Body))
                {
                    data = reader.ReadToEnd();
                }

                var Db = new Db(Environment.GetEnvironmentVariable("DataFolder"));
                var point = Db.PutRecord(p.stream, data);

                var stream = Db.GetStream(p.stream);

                var response = new Nancy.Responses.JsonResponse<Stream>(stream, new JsonSerialiser(), this.Context.Environment);
                response.ContentType = "application/json";

                return response;
            });
        }
    }
}
