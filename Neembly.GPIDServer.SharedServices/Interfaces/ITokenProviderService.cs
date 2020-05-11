using System.Threading.Tasks;


namespace Neembly.BOIDServer.SharedServices.Interfaces
{
    public interface ITokenProviderService
    {
        Task<bool> ValidateToken(string authToken);
        Task<string> GetClaimsPermission(string authToken, string moduleName);
        Task<bool> HasValidPermission(string authToken, string moduleName, string claimValues);
    }
}
