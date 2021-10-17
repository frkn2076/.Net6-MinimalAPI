using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("SampleDB"));
services.AddScoped<ApiContext>();
var app = builder.Build();

app.Use(async (context, next) =>
{
    System.Console.WriteLine(context.Request.Path);
    await next();
});

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
}

app.MapGet("/user", (ApiContext db) => db.People?.AsNoTracking().ToList());

app.MapPost("/user", (HttpContext context, ApiContext db, Person person) => {
    db.People?.Add(person);
    db.SaveChanges();
    return Results.Created("items", new { Id = person.Id});
});

app.Map("/error", () => Results.Problem("An error occurred.", statusCode: 500));

app.Run();

class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }
    public DbSet<Person>? People { get; set; }
}

record Person(Guid Id, [Required] string FirstName, [Required] string LastName);
