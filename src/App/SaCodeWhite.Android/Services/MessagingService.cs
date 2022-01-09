using Android.App;
using Android.Content;
using Android.Graphics;
using AndroidX.Core.App;
using Firebase.Messaging;
using SaCodeWhite.Services;
using SaCodeWhite.Shared.Models;
using System;

namespace SaCodeWhite.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            if (message.GetNotification() != null)
            {
                SendNotification(
                    message.GetNotification().Title,
                    message.GetNotification().Body);
            }
        }

        public override void OnNewToken(string token)
        {
            IoC.Resolve<IDeviceInstallationService>().Token = token;
        }

        private void SendNotification(string title, string body)
        {
            var notificationManager = NotificationManagerCompat.From(this);

            var intent = new Intent(this, typeof(MainActivity));
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);

            var builder = new NotificationCompat.Builder(this, MainActivity.CodeWhiteChannelId)
                    .SetSmallIcon(Resource.Drawable.ic_hospital)
                    //.SetColor(GetColor(Resource.Color.colorPrimary))
                    .SetContentTitle(title)
                    .SetContentText(body)
                    .SetContentIntent(pendingIntent)
                    .SetPriority(NotificationCompat.PriorityDefault);

            notificationManager.Notify((int)DateTime.UtcNow.Ticks, builder.Build());
        }

        private Bitmap AlertToBitmap(AlertStatusType alertStatus)
        {
            const int diameter = 256;
            const float radius = diameter / 2f;

            var bitmap = Bitmap.CreateBitmap(diameter, diameter, Bitmap.Config.Argb8888);
            var canvas = new Canvas(bitmap);

            var paint = new Paint();
            paint.Color = alertStatus switch
            {
                AlertStatusType.Green => Color.ParseColor("#8AC54B"),
                AlertStatusType.Amber => Color.ParseColor("#F2B600"),
                AlertStatusType.Red => Color.ParseColor("#B60000"),
                AlertStatusType.White => Color.White,
                _ => Color.LightGray
            };
            paint.SetStyle(Paint.Style.Fill);
            paint.AntiAlias = true;
            paint.Dither = true;

            canvas.DrawCircle(radius, radius, radius, paint);

            return bitmap;
        }
    }
}