using System;

namespace Kyrsova_OOP.Strategies
{
    public class EmailNotification : INotificationStrategy
    {
        public void Send(string message)
        {
            Console.WriteLine("Email notification: " + message);
        }
    }
}