```mermaid
classDiagram
direction LR

%% =====================
%% DOMAIN MODEL
%% =====================

class Habit {
    -id: int
    -title: string
    -priority: int
    -records: List<CompletionRecord>
    +complete(): void
    +subscribe(observer: IObserver): void
    +notify(): void
}

class CompletionRecord {
    -date: DateTime
    -completed: bool
}

Habit *-- CompletionRecord


%% =====================
%% SERVICE LAYER
%% =====================

class HabitService {
    +createHabit(title: string, priority: int): void
    +markCompleted(habitId: int): void
    +deleteHabit(habitId: int): void
}

class StatisticsService {
    +calculateCurrentStreak(habit: Habit): int
    +calculateLongestStreak(habit: Habit): int
}

HabitService --> Habit
StatisticsService --> Habit


%% =====================
%% REPOSITORY PATTERN
%% =====================

class IHabitRepository {
    <<interface>>
    +add(habit: Habit): void
    +remove(id: int): void
    +getAll(): List<Habit>
}

class HabitRepository {
    -habits: List<Habit>
    +add(habit: Habit): void
    +remove(id: int): void
    +getAll(): List<Habit>
}

IHabitRepository <|.. HabitRepository
HabitService --> IHabitRepository


%% =====================
%% OBSERVER PATTERN
%% =====================

class IObserver {
    <<interface>>
    +update(habit: Habit): void
}

class StatisticsObserver {
    +update(habit: Habit): void
}

IObserver <|.. StatisticsObserver
Habit --> IObserver


%% =====================
%% STRATEGY PATTERN
%% =====================

class INotificationStrategy {
    <<interface>>
    +notify(message: string): void
}

class EmailNotification {
    +notify(message: string): void
}

class PushNotification {
    +notify(message: string): void
}

INotificationStrategy <|.. EmailNotification
INotificationStrategy <|.. PushNotification

StatisticsObserver --> INotificationStrategy
```
![alt text](image.png)

![alt text](image-1.png)

![alt text](image-2.png)

![alt text](image-3.png)

![alt text](image-4.png)