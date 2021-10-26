
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using PeliculasAPI.Helpers;
using PeliculasAPI.Servicios;
using System.Text.Json.Serialization;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace PeliculasAPI
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAutoMapper(typeof(Startup));

            //services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosAzure>();
            services.AddTransient<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
            services.AddHttpContextAccessor();

            services.AddSingleton<GeometryFactory>(NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
            services.AddSingleton(provider =>

               new MapperConfiguration(config =>
              {
                  var geometryFactory = provider.GetRequiredService<GeometryFactory>();
                  config.AddProfile(new AutoMappersProfiles(geometryFactory));
              }).CreateMapper()
            );


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptions => sqlServerOptions.UseNetTopologySuite() 
            ));

            services.AddControllers()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
                .AddNewtonsoftJson();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "WebApiAutores", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app,
                               IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAutores v1");
            });

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


    }
}
