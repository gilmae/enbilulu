using System;
using Enbilulu.Engine;
using Enbilulu.Engine.Sqlite;
using Nancy;
namespace Enbilulu.Admin
{
    public class AdminHandler : NancyModule
    {
        private IEnbiluluEngine _engine;

        public AdminHandler(IEnbiluluEngine engine)
        {
            _engine = engine;

            Get("/admin", p =>
            {
                var streams = _engine.ListStreams();

                return View["Admin/index.html", streams];
            });

            Get("/admin/{stream}", p =>
            {
                Stream stream = _engine.GetStream(p.stream);

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

                _engine.CreateStream(streamName);

                return Response.AsRedirect("/admin/");
            });
        }
    }
}
