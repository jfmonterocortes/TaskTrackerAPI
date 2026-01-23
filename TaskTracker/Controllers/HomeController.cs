// HomeController.cs
// Handles the main web pages for the TaskTracker application
// Manages the Index (task list), CreateTask, ViewTask, and SearchTasks views
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TaskTracker.Models;

namespace TaskTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateTask()
        {
            return View();
        }

        public IActionResult ViewTask(int id)
        {
            ViewBag.TaskId = id;
            return View();
        }

        public IActionResult SearchTasks()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}