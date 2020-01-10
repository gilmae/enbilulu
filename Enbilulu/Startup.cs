﻿using System;
using Enbilulu.Engine;
using Enbilulu.Engine.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace Enbilulu
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Nancy.Owin;

    public class Startup
    {
        private readonly IConfiguration config;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                              //.AddJsonFile("appsettings.json")
                              .SetBasePath(env.ContentRootPath);

            config = builder.Build();
            
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new Bootstrapper(app.ApplicationServices)));
            
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IEnbiluluEngine, SqliteEngine>();
        }
    }
}
