using System.Linq;
using System.Text;

namespace Kyrsova_OOP.Services
{
    public class HabitReminderService
    {
        private readonly HabitManager manager;

        public HabitReminderService(HabitManager manager)
        {
            this.manager = manager;
        }

        public bool TryBuildReminder(out string subject, out string body)
        {
            subject = "Нагадування про звички";

            var incomplete = manager.GetAll()
                .Where(h => !h.IsCompletedToday())
                .ToList();

            if (incomplete.Count == 0)
            {
                body = string.Empty;
                return false;
            }

            var builder = new StringBuilder();
            builder.AppendLine("Ти не виконав деякі звички сьогодні:");
            foreach (var habit in incomplete)
            {
                builder.AppendLine($"- {habit.Name}");
            }
            builder.AppendLine();
            builder.AppendLine("Будь ласка, виконай їх сьогодні.");

            body = builder.ToString().TrimEnd();
            return true;
        }
    }
}
