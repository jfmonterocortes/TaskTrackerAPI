# TaskTrackerAPI

TaskTracker – ASP.NET Core REST API + MVC Frontend



Project Overview

TaskTracker is a simple task management application built in C# using ASP.NET Core.

It allows users to create, update, delete, and search for tasks through a web interface connected to a REST API.

The project was developed as a group assignment and managed using Azure DevOps for sprint tracking, tasks, and bug reporting.



Features

\- Add new tasks

\- Update existing tasks

\- Delete tasks

\- Search tasks by ID or Assignee

\- Add or remove assignee

\- Set, change, or remove task priority

\- Display all tasks



Technologies Used

\- Frontend: ASP.NET Core MVC (HTML, CSS, JavaScript, Razor Views)

\- Backend: ASP.NET Core Web API (C#)

\- Storage: In-Memory using ConcurrentDictionary

\- Version Control: GitHub Classroom

\- Project Management: Azure DevOps Boards

\- Testing Tools: Postman, Browser Developer Tools



How to Run the Application



Backend (API)

1\. Open the backend project in Visual Studio 2022.

2\. Run the project or use the command:

&nbsp;  dotnet run

3\. The API will start at:

&nbsp;  https://localhost:7134



Frontend (UI)

1\. Open the frontend project in Visual Studio 2022.

2\. Run the project or use:

&nbsp;  dotnet run

3\. The web interface will start at:

&nbsp;  https://localhost:7145



Important: Keep both projects running at the same time.



API Endpoints

Method   | Endpoint              | Description              | Response Codes

POST     | /api/tasks            | Create a new task        | 201 Created

GET      | /api/tasks            | Get all tasks            | 200 OK

GET      | /api/tasks/{id}       | Get task by ID           | 200 OK / 404 Not Found

PUT      | /api/tasks/{id}       | Update existing task     | 200 OK / 400 Bad Request

DELETE   | /api/tasks/{id}       | Delete a task            | 204 No Content / 404 Not Found

PATCH    | /api/tasks/{id}/assign| Assign or change assignee| 200 OK / 400 Bad Request



Testing Summary

\- Tested all API endpoints with Postman.

\- Confirmed proper status codes: 200, 201, 204, 400, and 404.

\- Reported and tracked bugs in Azure DevOps Boards.

\- Fixed main issues such as:

&nbsp; - Missing IDs in update requests.

&nbsp; - Wrong endpoint paths.

&nbsp; - Priority displayed as numbers instead of text.

&nbsp; - No form validation for empty fields.



Known Issues / Future Improvements

\- Use a persistent database instead of in-memory storage.

\- Improve UI error messages for failed API calls.

\- Add a field for the last updated date on tasks.



