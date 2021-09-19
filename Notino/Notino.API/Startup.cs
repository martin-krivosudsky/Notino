using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.OpenApi.Models;
using Notino.Common.Service;
using Notino.Common.Service.FileConvert;
using Notino.Data;
using Notino.Service;
using Notino.Service.FileConvert;
using Notino.Services;

namespace Notino.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Notino.API", Version = "v1" });
            });

            services.AddSingleton<IFileWriter, FileWriter>();
            services.AddSingleton<IFileReader, FileReader>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IFileConverter, FileConverter>();
            services.AddSingleton<IConverter, XmlToJsonConverter>();
            services.AddSingleton<IConverter, JsonToXmlConverter>();
            services.AddSingleton<IMailService, MailService>();
            services.AddSingleton<IWebService, WebService>();

            services.AddLogging(config =>
            {
                config.AddDebug();
                config.AddConsole();
            })
            .Configure<LoggerFilterOptions>(options =>
            {
                options.AddFilter<DebugLoggerProvider>(null, LogLevel.Information);
                options.AddFilter<ConsoleLoggerProvider>(null, LogLevel.Warning);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Notino.API v1"));
            }

            app.UseCors(builder => builder
               .WithOrigins("https://localhost:44340")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
