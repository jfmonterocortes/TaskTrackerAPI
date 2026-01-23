// ITaskService.cs
// Interface defining the contract for task management operations
// Ensures consistent implementation of task-related business logic
using TaskTracker.Models;

namespace TaskTracker.Services
{
    public interface ITaskService
    {
        IEnumerable<TaskItem> GetAllTasks();
        TaskItem? GetTaskById(int id);
        IEnumerable<TaskItem> SearchTasks(string searchTerm);
        TaskItem CreateTask(TaskItem task);
        bool UpdateTask(TaskItem task);
        bool DeleteTask(int id);
        bool UpdateAssignee(int taskId, string? assignee);
        bool UpdatePriority(int taskId, string? priority);
    }
}