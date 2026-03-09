using System.Collections.Generic;
using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Repositories
{
    public interface IHabitRepository
    {
        void Add(Habit habit);

        void Remove(int id);

        Habit? GetById(int id);

        List<Habit> GetAll();
    }
}