using System.Threading.Tasks;

namespace AptDealzBuyer.Repository
{
    public interface IFileUploadRepository
    {
        Task<string> UploadFile(int fileUploadCategory);
    }
}
