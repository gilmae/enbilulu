using System;
using System.IO;
using Nancy;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Enbilulu.Engine;
using Enbilulu.Engine.Sqlite;

namespace Enbilulu
{
    public class ApiHandler : NancyModule
    {
        private IEnbiluluEngine _engine;
        public ApiHandler(IEnbiluluEngine engine)
        {

            _engine = engine;
            Get("/streams", async (p, ct) =>
            {
                var streams = await _engine.ListStreams();

                var response = new Nancy.Responses.JsonResponse<IList<string>>(streams, new JsonSerialiser(), this.Context.Environment);
                response.ContentType = "application/json";

                return response;
            });

            Get("/streams/{stream}", async (p, ct) =>
            {
                var stream = await _engine.GetStream(p.stream);
                if (stream == null)
                {
                    return new Nancy.Responses.HtmlResponse(HttpStatusCode.NotFound);
                }

                var response =
                    new Nancy.Responses.JsonResponse<Stream>(stream, new JsonSerialiser(), this.Context.Environment)
                    {
                        ContentType = "application/json"
                    };

                return response;
            });

            Get("/streams/{stream}/{point}/{limit}", async (p, ct) =>
            {
                var data = await _engine.GetRecords(p.stream, p.point, p.limit);
                var response =
                    new Nancy.Responses.JsonResponse<Section>(data, new JsonSerialiser(), this.Context.Environment)
                    {
                        ContentType = "application/json"
                    };

                return response;
            });

            Post("/streams/{stream}", async (p, ct) =>
            {
                var stream = await _engine.CreateStream(p.stream);
                var response =
                    new Nancy.Responses.JsonResponse<Stream>(stream, new JsonSerialiser(), this.Context.Environment)
                    {
                        ContentType = "application/json"
                    };

                return response;
            });

            Post("/streams/{stream}/point", async (p, ct) =>
            {
                string data = "";

                using (StreamReader reader = new StreamReader(this.Request.Body))
                {
                    data = reader.ReadToEnd();
                }

                try
                {
                    var point = await _engine.PutRecord(p.stream, data);
                }
                catch (ArgumentException)
                {
                    return new Nancy.Responses.TextResponse(HttpStatusCode.NotFound, "Stream Not Found");
                }

                var stream = await _engine.GetStream(p.stream);

                var response =
                    new Nancy.Responses.JsonResponse<Stream>(stream, new JsonSerialiser(), this.Context.Environment)
                    {
                        ContentType = "application/json"
                    };

                return response;
            });


        }
    }
}
