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
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Hosting;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

// -------------------------------------------------------------
// CONFIGURATION
// -------------------------------------------------------------
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.UseCors(p => p
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    app.Logger.LogInformation("HTTP {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
});

app.MapGet("/", () => "TaskTracker API running");

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "TaskTrackerApi_SQ",
    timestamp = DateTime.UtcNow
}));

app.MapGet("/api/v1/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "TaskTrackerApi_SQ",
    timestamp = DateTime.UtcNow
}));

MapTaskEndpoints(app.MapGroup("/api/tasks"));
MapTaskEndpoints(app.MapGroup("/api/v1/tasks"));

app.Run();

static void MapTaskEndpoints(RouteGroupBuilder group)
{
    group.MapPost("", (CreateTaskRequest clientRequest) =>
    {
        if (string.IsNullOrWhiteSpace(clientRequest.Title) || clientRequest.Title.Length < 3)
        {
            return Results.BadRequest(new { error = "Title must be at least 3 characters long." });
        }

        var priorityParse = ParsePriority(clientRequest.Priority);
        if (!priorityParse.IsValid)
        {
            return Results.BadRequest(new { error = "Priority must be Low, Medium, High, or Critical." });
        }

        var service = new InMemoryTaskService();
        var task = new TaskModel
        {
            Title = clientRequest.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(clientRequest.Description) ? null : clientRequest.Description.Trim(),
            Assignee = string.IsNullOrWhiteSpace(clientRequest.Assignee) ? null : clientRequest.Assignee.Trim(),
            Priority = priorityParse.Value
        };

        var created = service.Create(task);
        return Results.Created($"/api/tasks/{created.Id}", created);
    });

    group.MapPut("/{id:int}", (int id, UpdateTaskRequest clientRequest) =>
    {
        if (string.IsNullOrWhiteSpace(clientRequest.Title) || clientRequest.Title.Length < 3)
        {
            return Results.BadRequest(new { error = "Title must be at least 3 characters long." });
        }

        var priorityParse = ParsePriority(clientRequest.Priority);
        if (!priorityParse.IsValid)
        {
            return Results.BadRequest(new { error = "Priority must be Low, Medium, High, or Critical." });
        }

        var service = new InMemoryTaskService();
        var updated = service.Update(
            id,
            clientRequest.Title.Trim(),
            string.IsNullOrWhiteSpace(clientRequest.Description) ? null : clientRequest.Description.Trim(),
            string.IsNullOrWhiteSpace(clientRequest.Assignee) ? null : clientRequest.Assignee.Trim(),
            priorityParse.Value
        );

        return updated is null
            ? Results.NotFound(new { error = "Task not found." })
            : Results.Ok(updated);
    });

    group.MapPatch("/{id:int}/assign", (int id, AssignTaskRequest clientRequest) =>
    {
        if (string.IsNullOrWhiteSpace(clientRequest.Assignee))
        {
            return Results.BadRequest(new { error = "Assignee name cannot be empty." });
        }

        var service = new InMemoryTaskService();
        var task = service.GetById(id);
        if (task is null)
        {
            return Results.NotFound(new { error = "Task not found." });
        }

        task.Assignee = clientRequest.Assignee.Trim();
        return Results.Ok(task);
    });

    group.MapGet("/{id:int}", (int id) =>
    {
        var service = new InMemoryTaskService();
        var task = service.GetById(id);
        return task is null
            ? Results.NotFound(new { error = "Task not found." })
            : Results.Ok(task);
    });

    group.MapDelete("/{id:int}", (int id) =>
    {
        var service = new InMemoryTaskService();
        var removed = service.Delete(id);
        return removed ? Results.NoContent() : Results.NotFound(new { error = "Task not found." });
    });

    group.MapGet("", () =>
    {
        var service = new InMemoryTaskService();
        return Results.Ok(service.GetAll());
    });
}

static (bool IsValid, Priority? Value) ParsePriority(string? priority)
{
    if (string.IsNullOrWhiteSpace(priority))
    {
        return (true, null);
    }

    var isValid = Enum.TryParse<Priority>(priority, true, out var parsed);
    return isValid ? (true, parsed) : (false, null);
}

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
