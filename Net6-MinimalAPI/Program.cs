var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("SampleDB"));
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
    return Results.Created("user", new { Id = person.Id });
});
app.Map("/error", (HttpContext context) => Results.Problem(context.Features.Get<IExceptionHandlerFeature>()?.Error.Message, statusCode: 500));

app.MapSwagger();
app.UseSwaggerUI();

app.Run();

class ApiContext : DbContext
{
    public ApiContext(DbContextOptions<ApiContext> options) : base(options) { }
    public DbSet<Person>? People { get; set; }
}

record Person(Guid Id, string FirstName, string LastName);