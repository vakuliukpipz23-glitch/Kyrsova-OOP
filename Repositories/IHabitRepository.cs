using System.Collections.Generic;
using Kyrsova_OOP.Models;

namespace Kyrsova_OOP.Repositories
{
    public interface IHabitRepository
    {
        List<Habit> GetAll();
        Habit? GetById(int id);
        void Add(Habit habit);
        void Update(Habit habit);
        void Delete(int id);
    }
}