using System;
using System.IO;
using Nancy;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Enbilulu
{
    public class ApiHandler : NancyModule
    {
        public ApiHandler()
        {
            Get("/streams", p=>
            {
                var streams = GetDb().ListStreams();

                 var response =  new Nancy.Responses.JsonResponse<IList<string>>(streams, new JsonSerialiser(), this.Context.Environment);
                response.ContentType = "application/json";

                return response;
            });

            Get("/streams/{stream}", p =>
            {
                var stream = GetDb().GetStream(p.stream);
                if (stream == null)
                {
                    return new Nancy.Responses.HtmlResponse(HttpStatusCode.NotFound);
                }

                var response =  new Nancy.Responses.JsonResponse<Stream>(stream, new JsonSerialiser(), this.Context.Environment);
                response.ContentType = "application/json";

                return response;
            });

            Get("/streams/{stream}/{point}/{limit}", p =>
            {
                var data = GetDb().GetRecords(p.stream, p.point, p.limit);
                var response = new Nancy.Responses.JsonResponse<Section>(data, new JsonSerialiser(), this.Context.Environment);
                response.ContentType = "application/json";

                return response;
            });

            Post("/streams/{stream}", p =>
            {
                var stream = GetDb().CreateStream(p.stream);
                var response = new Nancy.Responses.JsonResponse<Stream>(stream, new JsonSerialiser(), this.Context.Environment);
                response.ContentType = "application/json";

                return response;
            });

            Post("/streams/{stream}/point", p =>
            {
                string data = "";

                using (StreamReader reader = new StreamReader(this.Request.Body))
                {
                    data = reader.ReadToEnd();
                }

                var Db = GetDb();
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

        private Db GetDb()
        {
            return new Db(Environment.GetEnvironmentVariable("DataFolder"));
        }
    }
}
