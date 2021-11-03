using AptDealzBuyer.Model.Reponse;
using AptDealzBuyer.Model.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AptDealzBuyer.Repository
{
    public interface IProfileRepository
    {
        Task<bool> ValidPincode(string pinCode, string pinCodeName = null);

        Task<List<Category>> GetCategory();

        Task<List<SubCategory>> GetSubCategory(string CategortyId);

        Task<BuyerDetails> GetMyProfileData();

        Task DeactivateAccount();
    }
}
