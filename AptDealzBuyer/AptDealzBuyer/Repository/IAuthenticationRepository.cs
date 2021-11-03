using AptDealzBuyer.Model.Reponse;
using System.Net.Http;
using System.Threading.Tasks;

namespace AptDealzBuyer.Repository
{
    public interface IAuthenticationRepository
    {
        Task<bool> RefreshToken();

        Task DoLogout();

        Task<Response> APIResponse(HttpResponseMessage httpResponseMessage);
    }
}
