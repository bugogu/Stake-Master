using UnityEngine;
using Unity.Notifications.Android;

public class MobileNotification : MonoBehaviour
{
    private void Awake()
    {
        SendNotification();
    }
    private void SendNotification()
    {
        // Generate Channel
        var channel = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        // Send Notification
        var notification = new AndroidNotification();
        notification.Title = "Stake Master";
        notification.Text = "Hey Master, Targets waiting to be destroyed 💪";
        notification.SmallIcon = "icon_0";
        notification.LargeIcon = "icon_1";
        notification.FireTime = System.DateTime.Now.AddMinutes(60);

        AndroidNotificationCenter.SendNotification(notification, "channel_id");
    }
}
