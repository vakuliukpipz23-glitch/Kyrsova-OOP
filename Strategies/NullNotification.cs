using System;

namespace Kyrsova_OOP.Strategies
{
    public class NullNotification : INotificationStrategy
    {
        public void Send(string message)
        {
            // no-op for tests
        }
    }
}
