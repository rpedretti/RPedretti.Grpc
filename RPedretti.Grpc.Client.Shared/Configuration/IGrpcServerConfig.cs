using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RPedretti.Grpc.Client.Shared.Configuration
{
    public interface IGrpcServerConfig
    {
        public string? Url { get; set; }
    }
}
