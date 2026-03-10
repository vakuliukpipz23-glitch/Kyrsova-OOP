using System;
using Kyrsova_OOP.Models;
using Kyrsova_OOP.Strategies;

namespace Kyrsova_OOP.Observers
{
    public class StatisticsObserver : IObserver
    {
        private readonly INotificationStrategy strategy;

        public StatisticsObserver(INotificationStrategy strategy)
        {
            this.strategy = strategy;
        }

        public void Update(Habit habit)
        {
            strategy.Notify($"Habit completed: {habit.Title}");
        }
    }
}