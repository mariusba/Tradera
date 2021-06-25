using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using Tradera.Binance;
using Tradera.Models;
using Tradera.Models.WebSockets;
using Tradera.Services;
using Tradera.Services.BackgroundServices;

namespace Tradera.Api
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
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "Tradera.Api", Version = "v1"});
            });
            services.AddSingleton<BackgroundJobs>();
            services.AddSingleton<IQueuedBackgroundService, QueuedBackgroundService>();
            services.AddSingleton<IBackgroundWorkerManager, BackgroundWorkerManager>();
            services.AddSingleton<IBackgroundJobService, BackgroundJobService>();

            services.AddSingleton<IWebSocketConfigurator, BinanceWebsocketConfigurator>();
            services.AddSingleton<IWebSocketConfigurator, RandomConf>();
            
            services.AddSingleton<DataProvider>();

            services.AddSingleton<IPriceService, PriceService>();
            services.AddSingleton<INotificationsService, NotificationsService>();
            services.AddSingleton<IExchangeAgent, ExchangeAgent>();
            
            services.AddSingleton<IMapper, BinanceMapper>();
            services.AddSingleton<IDataProcessor, DataProcessor>();

        }
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IQueuedBackgroundService queuedBackgroundService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tradera.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
            Log.Information("starting queued background services");
            Task.Run(() => queuedBackgroundService.StartAsync(new CancellationToken()));
            Task.Run(() => queuedBackgroundService.StopAsync(new CancellationToken()));
            Log.Information("started queued background services");
        }

        public static void ConfigureLogging(HostBuilderContext context)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }
    }
}