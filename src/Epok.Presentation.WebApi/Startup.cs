using System;
using System.IO;
using Epok.Persistence.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleInjector;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Epok.Presentation.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Epok", Version = "v1"});
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Epok.Presentation.WebApi.xml"));
                //ToDo:4 find a way to generate enum schema with proper description
            });

            services.AddSimpleInjector(Composition.Root.Container, options =>
            {
                options.AddAspNetCore()
                    .AddControllerActivation()
                    .AddViewComponentActivation();
                options.AddLogging();
            });
            services.BuildServiceProvider(validateScopes: true)
                .UseSimpleInjector(Composition.Root.Container);

            var optionsBuilder = new DbContextOptionsBuilder(new DbContextOptions<DomainContext>());
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("ErpDb"));
            Composition.Root.InitializeContainer(optionsBuilder.Options as DbContextOptions<DomainContext>);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Epok API V1");
                c.RoutePrefix = string.Empty;
                c.DocExpansion(DocExpansion.None);
            });

            app.UseSimpleInjector(Composition.Root.Container);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
