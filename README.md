# Bug Logger API
This project is a very small ASP.NET Core Minimal API that stores bug reports in memory. It does not use a database. The API supports simple `GET` and `POST` requests, and will later be used by an Android Jetpack Compose app.

## What this project demonstrates
- creating a Minimal API in .NET
- defining simple endpoints
- returning JSON data
- accepting JSON input
- storing data in memory
- testing an API with Swagger
- containerising the API with Docker
- deploying the container to Render

## 1. Create the project

Create a new Minimal API project in a single folder:
```bash
dotnet new web -n BugLoggerApi
cd BugLoggerApi
```

## 2. Add Swagger

Install Swagger support - Swagger allows us to document our API
```
dotnet add package Swashbuckle.AspNetCore
```

## 3. Replace Program.cs

Replace the contents of Program.cs with:
```
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var bugs = new List<Bug>
{
    new Bug
    {
        Id = 1,
        Title = "Login button not working",
        Description = "Tapping the login button does nothing on Android 14.",
        Severity = "High",
        ReportedBy = "Amina",
        CreatedAt = DateTime.UtcNow,
        IsResolved = false
    },
    new Bug
    {
        Id = 2,
        Title = "Dark mode text invisible",
        Description = "Some labels become unreadable in dark mode.",
        Severity = "Medium",
        ReportedBy = "Yusuf",
        CreatedAt = DateTime.UtcNow,
        IsResolved = false
    }
};

app.MapGet("/", () => Results.Ok(new
{
    message = "Bug Logger API is running."
}));

app.MapGet("/bugs", () => Results.Ok(bugs));

app.MapGet("/bugs/{id:int}", (int id) =>
{
    var bug = bugs.FirstOrDefault(b => b.Id == id);
    return bug is null ? Results.NotFound() : Results.Ok(bug);
});

app.MapPost("/bugs", (Bug bug) =>
{
    var nextId = bugs.Count == 0 ? 1 : bugs.Max(b => b.Id) + 1;

    bug.Id = nextId;
    bug.CreatedAt = DateTime.UtcNow;
    bug.IsResolved = false;

    bugs.Add(bug);

    return Results.Created($"/bugs/{bug.Id}", bug);
});

app.Run();

public class Bug
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Severity { get; set; } = "";
    public string ReportedBy { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public bool IsResolved { get; set; }
}
```

## 4. Run the API locally
Run the API:
```
dotnet run
```
Open Swagger in your browser using the URL shown in the terminal, for example: http://localhost:5137/swagger

## 5. Test the endpoints
Available endpoints in Swagger or Postman
- `GET /`
- `GET /bugs`
- `GET /bugs/{id}`
- `POST /bugs`

## 6. Add a .gitignore

Create a .gitignore file in the project folder:
```
bin/
obj/
.vs/
.idea/
.vscode/
*.user
*.suo
*.log
appsettings.Development.json
.env
publish/
out/
TestResults/
coverage/
.DS_Store
Thumbs.db
```

## 7. Add a Dockerfile
Create a file called Dockerfile in the root of the project:
```
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BugLoggerApi.dll"]
```

## 8. Add a .dockerignore

Create a file called .dockerignore:
```
bin/
obj/
.vs/
.git/
.gitignore
Dockerfile*
docker-compose*
README.md
```

## 9. Build and run the Docker container locally

Build the image:
```
docker build -t buglogger-api .
```
Then, Run the container:
```
docker run -p 8080:8080 buglogger-api
```
Test in the browser:
- http://localhost:8080/
- http://localhost:8080/bugs
- http://localhost:8080/swagger


## 10. Push the project to GitHub

Initialise Git and push to GitHub. You may use your own repo

## 11. Deploy to Render
- Log in to Render.
- Create a new Web Service.
- Connect your GitHub repository.
- Choose the repository for this project.
- Set the runtime to Docker.
- Choose the free plan - DO NOT PAY!!!
- Deploy the service.

Render will detect the port from the Docker container.

After deployment, test:
- https://your-service-name.onrender.com/
- https://your-service-name.onrender.com/bugs
- https://your-service-name.onrender.com/swagger

## 12. Important notes
- This API stores data in memory only.
- If the app restarts or redeploys, posted bugs will be lost.
- The free Render plan may put the service to sleep after inactivity, so the first request may take longer.