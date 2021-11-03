using AptDealzBuyer.Model.Reponse;
using System.Threading.Tasks;

namespace AptDealzBuyer.Repository
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderDetails(string orderId);

        Task CancelOrder(string orderId);

        Task ConfirmDelivery(string orderId);

        Task<string> GenerateQRCodeImage(string orderId);
    }
}
