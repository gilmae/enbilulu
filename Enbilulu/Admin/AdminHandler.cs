using System;
using Nancy;
namespace Enbilulu.Admin
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

            Get("/admin/{stream}", p =>
            {
                Stream stream = GetDb().GetStream(p.stream);

                var model = new ViewModels.Stream
                {
                    Name = p.stream,
                    NumberOfPoints = stream.Points,
                    LastPoint = stream.Last_Point
                };

                return View["Admin/view.html", model];
            });

            Post("/admin/stream/create", p => {
                var streamName = this.Request.Form["stream"];

                GetDb().CreateStream(streamName);

                return Response.AsRedirect("/admin/");
            });
        }

        private Db GetDb()
        {
            return new Db(Environment.GetEnvironmentVariable("DataFolder"));
        }
    }
}
