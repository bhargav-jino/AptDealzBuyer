using AptDealzBuyer.Model.Request;
using System.Threading.Tasks;

namespace AptDealzBuyer.Repository
{
    public interface IQuoteRepository
    {
        Task<string> RevealContact(RevealSellerContact mRevealSellerContact);
        Task<Quote> GetQuoteById(string QuoteId);
    }
}
