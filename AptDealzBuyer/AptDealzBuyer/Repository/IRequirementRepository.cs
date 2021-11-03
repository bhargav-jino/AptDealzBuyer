using AptDealzBuyer.Model.Request;
using System.Threading.Tasks;

namespace AptDealzBuyer.Repository
{
    public interface IRequirementRepository
    {
        Task<Requirement> GetRequirementById(string RequirmentId);
        Task<bool> DeleteRequirement(string RequirmentID);
    }
}
