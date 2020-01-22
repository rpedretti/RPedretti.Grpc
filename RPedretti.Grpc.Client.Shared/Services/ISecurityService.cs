using System.Threading.Tasks;

namespace RPedretti.Grpc.Client.Shared.Services
{
    public interface ISecurityService
    {
        Task<string> Login(string username);
    }
}