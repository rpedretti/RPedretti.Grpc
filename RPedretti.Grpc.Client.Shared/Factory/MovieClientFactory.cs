using Grpc.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RPedretti.Grpc.Client.Shared.Factory
{
    public class MovieClientFactory
    {
        public static Movies.MoviesClient CreateClient(string host)
        {
            var assembly = Assembly.GetEntryAssembly();
            var resourceNames = assembly.GetManifestResourceNames();
            var resourcePaths = resourceNames.Where(x => x.EndsWith("cert.pem", StringComparison.CurrentCultureIgnoreCase)).ToArray();
            var certFile = assembly.GetManifestResourceStream(resourcePaths.Single());
            using var reader = new StreamReader(certFile);
            var cert = reader.ReadToEnd();

            var channel = new Channel(host, new SslCredentials(cert));
            return new Movies.MoviesClient(channel);
        }
    }
}
