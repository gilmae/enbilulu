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

                return View["WebServer/index.html", streams];
            });
        }

        private Db GetDb()
        {
            return new Db(Environment.GetEnvironmentVariable("DataFolder"));
        }
    }
}
