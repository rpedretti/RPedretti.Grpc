using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RPedretti.Grpc.BlazorWasm.Client.Configuration
{
    public static class ConfigurationExtensions
    {
        private static readonly Assembly runningAssembly = Assembly.GetExecutingAssembly();
        private static readonly string[] ResourceNames = runningAssembly.GetManifestResourceNames();
        private static readonly string AssemblyName = runningAssembly.GetName().Name;
        private static readonly ConfigurationBuilder configBuilder = new ConfigurationBuilder();

        public static IServiceCollection AddEmbeddedJson(this IServiceCollection serviceCollection, string fileName, bool required = false)
        {
            var resourceName = $"{AssemblyName}.{fileName.Replace("/", ".")}";
            if (ResourceNames.Contains(resourceName))
            {
                configBuilder.AddJsonStream(runningAssembly.GetManifestResourceStream(resourceName));
            } 
            else if(required)
            {
                throw new FileNotFoundException("Could not find embeded file", fileName);
            }

            return serviceCollection;
        }

        public static ConfigurationBuilder GetJsonConfigurationBuilderBuilder(this IServiceCollection _) => configBuilder;
    }
}
