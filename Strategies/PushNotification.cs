using System;

namespace Kyrsova_OOP.Strategies
{
    public class PushNotification : INotificationStrategy
    {
        public void Notify(string message)
        {
            Console.WriteLine("Push notification: " + message);
        }
    }
}