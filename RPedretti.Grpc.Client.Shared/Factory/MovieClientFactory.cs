using Grpc.Core;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Reflection;

namespace RPedretti.Grpc.Client.Shared.Factory
{
    public class MovieClientFactory
    {
        public static Movies.MoviesClient CreateClient(string host)
        {
            var manifestEmbeddedProvider =
                new EmbeddedFileProvider(Assembly.GetCallingAssembly());
            var certFile = manifestEmbeddedProvider.GetFileInfo("Certs/cert.pem");
            using var reader = new StreamReader(certFile.CreateReadStream());
            var cert = reader.ReadToEnd();

            var channel = new Channel(host, new SslCredentials(cert));
            return new Movies.MoviesClient(channel);
        }
    }
}
