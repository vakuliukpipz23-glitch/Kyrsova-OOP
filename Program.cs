using System;
using System.Collections.Generic;
using System.Linq;

public interface IObserver
{
    void Update(Habit habit);
}

public interface INotificationStrategy
{
    void Notify(string message);
}

public interface IHabitRepository
{
    void Add(Habit habit);
    void Remove(int id);
    List<Habit> GetAll();
}

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

public class Habit
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int Priority { get; set; }

    public List<CompletionRecord> Records { get; set; } = new List<CompletionRecord>();

    private List<IObserver> observers = new List<IObserver>();

    public Habit(int id, string title, int priority)
    {
        Id = id;
        Title = title;
        Priority = priority;
    }

    public void Complete()
    {
        Records.Add(new CompletionRecord(DateTime.Now, true));
        Notify();
    }

    public void Subscribe(IObserver observer)
    {
        observers.Add(observer);
    }

    public void Notify()
    {
        foreach (var observer in observers)
        {
            observer.Update(this);
        }
    }
}

public class HabitRepository : IHabitRepository
{
    private List<Habit> habits = new List<Habit>();

    public void Add(Habit habit)
    {
        habits.Add(habit);
    }

    public void Remove(int id)
    {
        habits.RemoveAll(h => h.Id == id);
    }

    public List<Habit> GetAll()
    {
        return habits;
    }
}

public class HabitService
{
    private IHabitRepository repository;

    public HabitService(IHabitRepository repository)
    {
        this.repository = repository;
    }

    public void CreateHabit(string title, int priority)
    {
        int id = repository.GetAll().Count + 1;
        Habit habit = new Habit(id, title, priority);
        repository.Add(habit);
    }

    public void MarkCompleted(int id)
    {
        var habit = repository.GetAll().FirstOrDefault(h => h.Id == id);

        if (habit != null)
        {
            habit.Complete();
        }
    }

    public void DeleteHabit(int id)
    {
        repository.Remove(id);
    }
}

public class EmailNotification : INotificationStrategy
{
    public void Notify(string message)
    {
        Console.WriteLine("Email: " + message);
    }
}

public class StatisticsObserver : IObserver
{
    private INotificationStrategy notification;

    public StatisticsObserver(INotificationStrategy strategy)
    {
        notification = strategy;
    }

    public void Update(Habit habit)
    {
        notification.Notify($"Habit completed: {habit.Title}");
    }
}

class Program
{
    static void Main()
    {
        IHabitRepository repo = new HabitRepository();
        HabitService service = new HabitService(repo);

        service.CreateHabit("Read book", 1);

        Habit habit = repo.GetAll()[0];

        StatisticsObserver observer =
            new StatisticsObserver(new EmailNotification());

        habit.Subscribe(observer);

        service.MarkCompleted(1);

        Console.WriteLine($"Habit: {habit.Title}");
        Console.WriteLine($"Records: {habit.Records.Count}");
    }
}