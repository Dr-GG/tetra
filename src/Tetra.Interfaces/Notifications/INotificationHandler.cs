using System.Threading.Tasks;
using Tetra.Domain.Notifications;

namespace Tetra.Interfaces.Notifications
{
    public interface INotificationHandler
    {
        Task Notify(NotificationEvent notification);
    }
}
