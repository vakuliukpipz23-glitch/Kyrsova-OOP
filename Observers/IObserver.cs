using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Observers
{
    public interface IObserver
    {
        void Update(Habit habit);
    }
}