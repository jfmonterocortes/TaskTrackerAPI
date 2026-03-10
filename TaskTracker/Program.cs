// Program.cs
// Application startup and configuration
// Sets up services, middleware, and routing for the ASP.NET Core application

using TaskTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ITaskService, TaskService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.Use(async (context, next) =>
{
    app.Logger.LogInformation("HTTP {Method} {Path}", context.Request.Method, context.Request.Path);
    await next();
});

app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "TaskTracker",
    timestamp = DateTime.UtcNow
}));
app.MapGet("/api/v1/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "TaskTracker",
    timestamp = DateTime.UtcNow
}));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
