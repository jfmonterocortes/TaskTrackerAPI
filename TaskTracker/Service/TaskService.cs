// TaskService.cs
// Implementation of task management business logic
// Handles in-memory storage of tasks with sample data
// Provides methods for CRUD operations, search, and task modifications
using TaskTracker.Models;

namespace TaskTracker.Services
{
    public class TaskService : ITaskService
    {
        private static int _instanceCount = 0;
        private readonly List<TaskItem> _tasks = new();
        private int _nextId = 1;

        public TaskService()
        {
            _instanceCount++;
            Console.WriteLine($" TaskService instance created: #{_instanceCount}");

            // Add some sample data for testing
            _tasks.Add(new TaskItem { Id = _nextId++, Title = "Complete Project Proposal", Assignee = "John", Priority = "High" });
            _tasks.Add(new TaskItem { Id = _nextId++, Title = "Review Code", Assignee = "Sarah", Priority = "Medium" });
        }

        public IEnumerable<TaskItem> GetAllTasks() => _tasks;

        public TaskItem? GetTaskById(int id) => _tasks.FirstOrDefault(t => t.Id == id);

        public IEnumerable<TaskItem> SearchTasks(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _tasks;

            return _tasks.Where(t =>
                t.Id.ToString().Contains(searchTerm) ||
                (t.Assignee?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) == true) ||
                t.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
            );
        }

        public TaskItem CreateTask(TaskItem task)
        {
            task.Id = _nextId++;
            task.CreatedDate = DateTime.Now;
            _tasks.Add(task);
            Console.WriteLine($" Task created: ID={task.Id}, Title={task.Title}");
            Console.WriteLine($" Total tasks now: {_tasks.Count}");
            return task;
        }

        public bool UpdateTask(TaskItem updatedTask)
        {
            var existingTask = GetTaskById(updatedTask.Id);
            if (existingTask == null) return false;

            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;
            existingTask.Assignee = updatedTask.Assignee;
            existingTask.Priority = updatedTask.Priority;
            existingTask.DueDate = updatedTask.DueDate;

            return true;
        }

        public bool DeleteTask(int id)
        {
            var task = GetTaskById(id);
            if (task == null) return false;

            return _tasks.Remove(task);
        }

        public bool UpdateAssignee(int taskId, string? assignee)
        {
            var task = GetTaskById(taskId);
            if (task == null) return false;

            task.Assignee = assignee;
            return true;
        }

        public bool UpdatePriority(int taskId, string? priority)
        {
            var task = GetTaskById(taskId);
            if (task == null) return false;

            task.Priority = priority;
            return true;
        }
    }
}