using System.Linq;
using Kyrsova_OOP.Repositories;

namespace Kyrsova_OOP.Services
{
    public class GlobalStatisticsService
    {
        private readonly IHabitRepository repository;

        public GlobalStatisticsService(IHabitRepository repository)
        {
            this.repository = repository;
        }

        public int TotalHabits()
        {
            return repository.GetAll().Count();
        }

        public int TotalCompletions()
        {
            return repository.GetAll()
                .Sum(h => h.Records.Count);
        }
    }
}