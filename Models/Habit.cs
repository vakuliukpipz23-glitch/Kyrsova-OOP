using System;
using System.Collections.Generic;

namespace Kyrsova_OOP.Models
{
    public class Habit
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public int Priority { get; set; }

        public List<CompletionRecord> Records { get; set; } = new();

        public void Complete()
        {
            Records.Add(new CompletionRecord
            {
                Date = DateTime.Now,
                Completed = true
            });
        }
    }
}