// TaskController.cs  
// API controller for task management operations
// Provides REST endpoints for CRUD operations: Create, Read, Update, Delete tasks
// Handles assignee and priority management via API calls
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Models;
using TaskTracker.Services;

namespace TaskTracker.Controllers
{
    [ApiController]
    [Route("api/task")]
    [Route("api/tasks")]
    [Route("api/v1/tasks")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        // GET: api/tasks
        [HttpGet]
        public IActionResult GetAllTasks()
        {
            var tasks = _taskService.GetAllTasks();
            return Ok(tasks);
        }

        // GET: api/tasks/5
        [HttpGet("{id}")]
        public IActionResult GetTask(int id)
        {
            var task = _taskService.GetTaskById(id);
            if (task == null) return NotFound();
            return Ok(task);
        }

        // GET: api/tasks/search?term=john
        [HttpGet("search")]
        public IActionResult SearchTasks([FromQuery] string? term)
        {
            var tasks = _taskService.SearchTasks(term ?? string.Empty);
            return Ok(tasks);
        }

        // POST: api/tasks
        [HttpPost]
        public IActionResult CreateTask([FromBody] TaskItem? task)
        {
            if (task == null)
            {
                return BadRequest(new { error = "Request body is required." });
            }

            Console.WriteLine($"Received task creation request: {task.Title}");

            if (string.IsNullOrWhiteSpace(task.Title))
            {
                return BadRequest(new { error = "Title is required." });
            }

            var createdTask = _taskService.CreateTask(task);
            Console.WriteLine($"Task created with ID: {createdTask.Id}");

            return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
        }

        // PUT: api/task/5
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] TaskItem? task)
        {
            if (task == null)
            {
                return BadRequest(new { error = "Request body is required." });
            }

            if (task.Id == 0)
            {
                task.Id = id;
            }

            Console.WriteLine($"Updating task {id} with data: {System.Text.Json.JsonSerializer.Serialize(task)}");

            // Make sure the ID from the URL matches the task object
            if (id != task.Id)
            {
                Console.WriteLine($"ID mismatch: URL id={id}, Task id={task.Id}");
                return BadRequest(new { error = "Route ID does not match payload ID." });
            }

            if (string.IsNullOrWhiteSpace(task.Title))
            {
                return BadRequest(new { error = "Title is required." });
            }

            var success = _taskService.UpdateTask(task);
            if (!success)
            {
                Console.WriteLine($"Task {id} not found for update");
                return NotFound(new { error = "Task not found." });
            }

            Console.WriteLine($"Task {id} updated successfully");
            return Ok(task);
        }

        // DELETE: api/tasks/5
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            var success = _taskService.DeleteTask(id);
            if (!success) return NotFound(new { error = "Task not found." });

            return NoContent();
        }

        // PUT: api/tasks/5/assignee
        [HttpPut("{id}/assignee")]
        public IActionResult UpdateAssignee(int id, [FromBody] string? assignee)
        {
            if (assignee != null && string.IsNullOrWhiteSpace(assignee))
            {
                return BadRequest(new { error = "Assignee cannot be empty." });
            }

            var success = _taskService.UpdateAssignee(id, assignee);
            if (!success) return NotFound(new { error = "Task not found." });

            return Ok(new { message = "Assignee updated successfully" });
        }

        // DELETE: api/tasks/5/assignee
        [HttpDelete("{id}/assignee")]
        public IActionResult RemoveAssignee(int id)
        {
            var success = _taskService.UpdateAssignee(id, null);
            if (!success) return NotFound(new { error = "Task not found." });

            return Ok(new { message = "Assignee removed successfully" });
        }

        // PUT: api/tasks/5/priority
        [HttpPut("{id}/priority")]
        public IActionResult UpdatePriority(int id, [FromBody] string? priority)
        {
            if (priority != null && string.IsNullOrWhiteSpace(priority))
            {
                return BadRequest(new { error = "Priority cannot be empty." });
            }

            var success = _taskService.UpdatePriority(id, priority);
            if (!success) return NotFound(new { error = "Task not found." });

            return Ok(new { message = "Priority updated successfully" });
        }

        // DELETE: api/tasks/5/priority
        [HttpDelete("{id}/priority")]
        public IActionResult RemovePriority(int id)
        {
            var success = _taskService.UpdatePriority(id, null);
            if (!success) return NotFound(new { error = "Task not found." });

            return Ok(new { message = "Priority removed successfully" });
        }
    }
}
