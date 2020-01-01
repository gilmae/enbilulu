using System;
using Nancy;

namespace Enbilulu
{
    public class StreamHandler : NancyModule
    {
        public StreamHandler()
        {
            Get("/stream/{stream}", p =>
            {
                var stream = new Db(Environment.GetEnvironmentVariable("DataFolder")).GetStream(p.stream);


                var response =  new Nancy.Responses.JsonResponse<Stream>(stream, new JsonSerialiser(), this.Context.Environment);
                response.ContentType = "application/json";

                return response;


            });

            Post("/stream/{stream}", p =>
            {
                var stream = new Db(Environment.GetEnvironmentVariable("DataFolder")).CreateStream(p.stream);
                return Newtonsoft.Json.JsonConvert.SerializeObject(stream);
            });
        }
    }
}
