using System;
using Nancy;
namespace Enbilulu.WebServer
{
    public class AdminHandler : NancyModule
    {
        public AdminHandler()
        {
            Get("/admin", p =>
            {
                var streams =  GetDb().ListStreams();

                return View["Admin/index.html", streams];
            });

            Post("/admin/stream/create", p => {
                var streamName = this.Request.Form["stream"];

                GetDb().CreateStream(streamName);

                return Response.AsRedirect("/admin");
            });
        }

        private Db GetDb()
        {
            return new Db(Environment.GetEnvironmentVariable("DataFolder"));
        }
    }
}
