/*
FILE          : Program.cs
PROJECT       : SENG2020 – Project 1 – TaskTracker (Backend API)
PROGRAMMER    : Juan Felipe Montero Cortes
FIRST VERSION : 2025-10-24
DESCRIPTION   :
      • Domain model (Task and Priority)
      • In-memory data service
      • REST endpoints for task management
      • CORS and JSON enum serialization enabled
*/

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

// -------------------------------------------------------------
// CONFIGURATION
// -------------------------------------------------------------
var builder = WebApplication.CreateBuilder(args);

// Enable CORS
builder.Services.AddCors();

// Make enums (Priority) return as strings in JSON ("High", "Low", etc.)
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

// Enable CORS for all requests
app.UseCors(p => p
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

// Redirect HTTP ? HTTPS
app.UseHttpsRedirection();

/* ---------------------------------------------------------------------------
ROUTE         : GET /
FUNCTION      : Landing page (check if backend is running)
---------------------------------------------------------------------------- */
app.MapGet("/", () => "TaskTracker API running");

/* ============================== ENDPOINTS ================================= */

// POST /api/tasks  -> Create
app.MapPost("/api/tasks", (CreateTaskRequest clientRequest) =>
{
    if (string.IsNullOrWhiteSpace(clientRequest.Title) || clientRequest.Title.Length < 3)
        return Results.BadRequest(new { error = "Title must be at least 3 characters long." });

    var service = new InMemoryTaskService();

    var task = new TaskModel
    {
        Title = clientRequest.Title.Trim(),
        Description = string.IsNullOrWhiteSpace(clientRequest.Description) ? null : clientRequest.Description.Trim(),
        Assignee = string.IsNullOrWhiteSpace(clientRequest.Assignee) ? null : clientRequest.Assignee.Trim(),
        Priority = Enum.TryParse<Priority>(clientRequest.Priority, true, out var p) ? p : null
    };

    var created = service.Create(task);
    return Results.Created($"/api/tasks/{created.Id}", created);
});

// PUT /api/tasks/{id}  -> Update
app.MapPut("/api/tasks/{id:int}", (int id, UpdateTaskRequest clientRequest) =>
{
    if (string.IsNullOrWhiteSpace(clientRequest.Title) || clientRequest.Title.Length < 3)
        return Results.BadRequest(new { error = "Title must be at least 3 characters long." });

    Priority? parsedPriority = null;
    if (!string.IsNullOrWhiteSpace(clientRequest.Priority) &&
        Enum.TryParse<Priority>(clientRequest.Priority, true, out var p))
    {
        parsedPriority = p;
    }

    var service = new InMemoryTaskService();
    var updated = service.Update(
        id,
        clientRequest.Title.Trim(),
        string.IsNullOrWhiteSpace(clientRequest.Description) ? null : clientRequest.Description.Trim(),
        string.IsNullOrWhiteSpace(clientRequest.Assignee) ? null : clientRequest.Assignee.Trim(),
        parsedPriority
    );

    if (updated is null)
        return Results.NotFound(new { error = "Task not found." });

    return Results.Ok(updated);
});

// PATCH /api/tasks/{id}/assign  -> Assign or change assignee
app.MapPatch("/api/tasks/{id:int}/assign", (int id, AssignTaskRequest clientRequest) =>
{
    if (string.IsNullOrWhiteSpace(clientRequest.Assignee))
        return Results.BadRequest(new { error = "Assignee name cannot be empty." });

    var service = new InMemoryTaskService();
    var task = service.GetById(id);
    if (task is null)
        return Results.NotFound(new { error = "Task not found." });

    task.Assignee = clientRequest.Assignee.Trim();
    return Results.Ok(task);
});

// GET /api/tasks/{id}  -> Get by Id
app.MapGet("/api/tasks/{id:int}", (int id) =>
{
    var service = new InMemoryTaskService();
    var task = service.GetById(id);
    return task is null
        ? Results.NotFound(new { error = "Task not found." })
        : Results.Ok(task);
});

// DELETE /api/tasks/{id}  -> Delete
app.MapDelete("/api/tasks/{id:int}", (int id) =>
{
    var service = new InMemoryTaskService();
    var removed = service.Delete(id);
    return removed ? Results.NoContent() : Results.NotFound(new { error = "Task not found." });
});

// GET /api/tasks  -> List all
app.MapGet("/api/tasks", () =>
{
    var service = new InMemoryTaskService();
    return Results.Ok(service.GetAll());
});

app.Run();

/* =============================== MODELS =================================== */

public enum Priority { Low, Medium, High, Critical }

public class TaskModel
{

    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Assignee { get; set; }
    public Priority? Priority { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}

public class InMemoryTaskService
{

    private static readonly ConcurrentDictionary<int, TaskModel> Store = new();
    private static int _nextId = 1;

    public TaskModel Create(TaskModel task)
    {
        task.Id = _nextId++;
        Store[task.Id] = task;
        return task;
    }

    public TaskModel? GetById(int id)
        => Store.TryGetValue(id, out var task) ? task : null;

    public TaskModel? Update(int id, string title, string? description, string? assignee, Priority? priority)
    {
        if (!Store.TryGetValue(id, out var existing)) return null;
        existing.Title = title;
        existing.Description = description;
        existing.Assignee = assignee;
        existing.Priority = priority;
        return existing;
    }

    public bool Delete(int id) => Store.TryRemove(id, out _);

    public IReadOnlyList<TaskModel> GetAll() => Store.Values.ToList();
}

/* ================================ DTOs ==================================== */

public class CreateTaskRequest
{
    [Required, MinLength(3)]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Assignee { get; set; }
    public string? Priority { get; set; }
}

public class UpdateTaskRequest
{
    [Required, MinLength(3)]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Assignee { get; set; }
    public string? Priority { get; set; }
}

public class AssignTaskRequest
{
    [Required, MinLength(2)]
    public string Assignee { get; set; } = string.Empty;
}
