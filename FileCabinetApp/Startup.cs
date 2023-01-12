using System;
using FileCabinetApp.FileCabinetService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileCabinetApp
{
    public class Startup
    {
        public Startup()
        {
            this.Configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            this.ServiceProvider = this.ConfigureServices(new ServiceCollection())
                .BuildServiceProvider();
        }

        public IConfiguration Configuration { get; }

        public IServiceProvider ServiceProvider { get; }

        private IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services
                .AddSingleton(s => new FileCabinetFilesystemService(this.Configuration["PathToDataBase"]));
        }
    }
}
