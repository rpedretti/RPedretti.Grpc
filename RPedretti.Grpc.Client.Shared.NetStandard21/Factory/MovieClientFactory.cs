using Grpc.Core;
using Grpc.Net.Client;
using Grpc.Net.Client.Web;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace RPedretti.Grpc.Client.Shared.Factory.NetStandard21
{
    public class MovieClientFactory
    {
        public static Movies.MoviesClient CreateClient(string host, Assembly assembly)
        {
            var resourceNames = assembly.GetManifestResourceNames();

            var resourcePaths = resourceNames.Where(x => x.EndsWith("cert.pem", StringComparison.CurrentCultureIgnoreCase)).ToArray();
            //var certFile = assembly.GetManifestResourceStream(resourcePaths.Single());
            //using var reader = new StreamReader(certFile);
            //var cert = reader.ReadToEnd();
            //var _token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoicmFmYSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlVzZXIiLCJleHAiOjE1Nzk1MTQxMzcsImlzcyI6IkV4YW1wbGVTZXJ2ZXIiLCJhdWQiOiJFeGFtcGxlQ2xpZW50cyJ9.sF9GlsJbEZ4PTh41Rap__MZcvZ2ipUw5HtqM7Y_C8Ic";
            
            //var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            //{
            //    if (!string.IsNullOrEmpty(_token))
            //    {
            //        metadata.Add("Authorization", $"Bearer {_token}");
            //    }
            //    return Task.CompletedTask;
            //});
            var wasmHttpMessageHandlerType = Assembly.Load("WebAssembly.Net.Http").GetType("WebAssembly.Net.Http.HttpClient.WasmHttpMessageHandler");
            var wasmHttpMessageHandler = (HttpMessageHandler)Activator.CreateInstance(wasmHttpMessageHandlerType);
            var httpClient = new HttpClient(new GrpcWebHandler(GrpcWebMode.GrpcWeb, wasmHttpMessageHandler));

            var channel = GrpcChannel.ForAddress(host, new GrpcChannelOptions { 
                HttpClient = httpClient
            });

            return new Movies.MoviesClient(channel);
        }
    }
}
