using System;
using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Observers
{
    public class StatisticsObserver : IObserver
    {
        public void Update(Habit habit)
        {
            Console.WriteLine($"Habit updated: {habit.Name}");
        }
    }
}