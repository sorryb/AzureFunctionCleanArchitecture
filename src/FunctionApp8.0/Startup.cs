using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Presentation.FunctionApp8
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection collection)
        {
            collection.AddApplicationServices();

            // TODO The Azure Function should not have any dependency on Infrastructure other than DI
            // Determine a better technique
            collection.AddInfrastructureServices(this.configuration);
        }
    }
}
