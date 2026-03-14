using System;

namespace Kyrsova_OOP.Strategies
{
    public class PushNotification : INotificationStrategy
    {
        public void Send(string message)
        {
            Console.WriteLine("Push notification: " + message);
        }
    }
}