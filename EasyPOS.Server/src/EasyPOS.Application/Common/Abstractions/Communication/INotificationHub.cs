using EasyPOS.Application.Features.Admin.AppNotifications.Queries;

namespace EasyPOS.Application.Common.Abstractions.Communication;

public interface INotificationHub
{
    Task ReceiveNotification(AppNotificationModel notification);
    Task ReceiveRolePermissionNotify();
    Task ReceiveMenuOrderChangeNotify();
}
