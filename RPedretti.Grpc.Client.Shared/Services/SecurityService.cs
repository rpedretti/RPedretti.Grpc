using RPedretti.Grpc.Client.Shared.Configuration;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;

namespace RPedretti.Grpc.Client.Shared.Services
{
    public sealed class SecurityService : ISecurityService
    {
        private static readonly int _numProcs = Environment.ProcessorCount;
        private static readonly int _concurrencyLevel = _numProcs * 2;

        private static readonly ConcurrentDictionary<string, string> _tokens = new ConcurrentDictionary<string, string>(_concurrencyLevel, 100);
        private readonly HttpClient _httpClient;
        private readonly IGrpcServerConfig _grpcsConfig;

        public SecurityService(HttpClient httpClient, IGrpcServerConfig grpcsConfig)
        {
            _httpClient = httpClient;
            _grpcsConfig = grpcsConfig;
        }

        public async Task<string> Login(string username)
        {
            if (!_tokens.TryGetValue(username, out var token))
            {
                token = await _httpClient.GetStringAsync($"{_grpcsConfig?.Url}/generateJwtToken?name={username}");
                _tokens.TryAdd(username, token);
            }

            return token;
        }
    }
}
