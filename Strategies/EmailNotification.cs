using System;

namespace Kyrsova_OOP.Strategies
{
    public class EmailNotification : INotificationStrategy
    {
        public void Notify(string message)
        {
            Console.WriteLine("Email notification: " + message);
        }
    }
}