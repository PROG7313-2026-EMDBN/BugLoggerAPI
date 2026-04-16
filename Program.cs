var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

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

app.MapGet("/", () =>
{
    return Results.Ok(new
    {
        message = "Bug Logger API is running."
    });
});

app.MapGet("/bugs", () => Results.Ok(bugs));

app.MapGet("/bugs/{id:int}", (int id) =>
{
    var bug = bugs.FirstOrDefault(b => b.Id == id);

    return bug is null
        ? Results.NotFound()
        : Results.Ok(bug);
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