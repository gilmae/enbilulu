using System;
using System.IO;
using Nancy;
using System.Collections.Generic;

namespace Enbilulu
{
    public class ApiHandler : NancyModule
    {
        public ApiHandler()
        {
            Get("/streams", p=>
            {
                var streams = new Db(Environment.GetEnvironmentVariable("DataFolder")).ListStreams();

                 var response =  new Nancy.Responses.JsonResponse<IList<string>>(streams, new JsonSerialiser(), this.Context.Environment);
                response.ContentType = "application/json";

                return response;
            });

            Get("/streams/{stream}", p =>
            {
                var stream = new Db(Environment.GetEnvironmentVariable("DataFolder")).GetStream(p.stream);
                if (stream == null)
                {
                    return new Nancy.Responses.HtmlResponse(HttpStatusCode.NotFound);
                }

                var response =  new Nancy.Responses.JsonResponse<Stream>(stream, new JsonSerialiser(), this.Context.Environment);
                response.ContentType = "application/json";

                return response;
            });

            Get("/points/{stream}/{point}/{limit}", p =>
            {
                var data = new Db(Environment.GetEnvironmentVariable("DataFolder")).GetRecords(p.stream, p.point, p.limit);
                var response = new Nancy.Responses.JsonResponse<Section>(data, new JsonSerialiser(), this.Context.Environment);
                response.ContentType = "application/json";

                return response;
            });

            Post("/streams/{stream}", p =>
            {
                var stream = new Db(Environment.GetEnvironmentVariable("DataFolder")).CreateStream(p.stream);
                var response = new Nancy.Responses.JsonResponse<Stream>(stream, new JsonSerialiser(), this.Context.Environment);
                response.ContentType = "application/json";

                return response;
            });

            Post("/points/{stream}", p =>
            {
                string data = "";

                using (StreamReader reader = new StreamReader(this.Request.Body))
                {
                    data = reader.ReadToEnd();
                }

                var Db = new Db(Environment.GetEnvironmentVariable("DataFolder"));
                try
                {
                    var point = Db.PutRecord(p.stream, data);
                }
                catch (ArgumentException)
                {
                    return new Nancy.Responses.TextResponse(HttpStatusCode.NotFound, "Stream Not Found");
                }

                var stream = Db.GetStream(p.stream);

                var response = new Nancy.Responses.JsonResponse<Stream>(stream, new JsonSerialiser(), this.Context.Environment);
                response.ContentType = "application/json";

                return response;
            });
        }
    }
}
