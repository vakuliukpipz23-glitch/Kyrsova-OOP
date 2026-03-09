using System;

namespace Kyrsova_OOP.Models
{
    public class CompletionRecord
    {
        public DateTime Date { get; set; }
        public bool Completed { get; set; }

        public CompletionRecord(DateTime date, bool completed)
        {
            Date = date;
            Completed = completed;
        }
    }
}