using Microsoft.Extensions.DependencyInjection;

namespace Chromely.Mvc
{


    public static class MvcConfigurationBuilder
    {
        private static IServiceCollection serviceCollection;

        static MvcConfigurationBuilder()
        {
            serviceCollection = new ServiceCollection();
        }

        public static IServiceCollection Create()
        {
            return serviceCollection;
        }
    }

}
