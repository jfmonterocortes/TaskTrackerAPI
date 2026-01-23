
// TaskItem.cs
// Data model representing a task in the system
// Defines properties: Id, Title, Description, Assignee, Priority, Dates
// Used for both database storage and API data transferusing System;

namespace TaskTracker.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Assignee { get; set; }
        public string? Priority { get; set; } // "Low", "Medium", "High", or null
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? DueDate { get; set; }
    }
}