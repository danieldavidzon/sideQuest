using Microsoft.EntityFrameworkCore;
using SideQuestAPI.Data;
using SideQuestAPI.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddScoped<WeeklyQuestService>();
builder.Services.AddHostedService<WeeklyQuestScheduler>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<SideQuestDbContext>(options =>
    options.UseSqlite("Data Source=sidequest.db"));
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<SideQuestDbContext>();

    db.Database.EnsureCreated();
}
app.Urls.Add("http://0.0.0.0:5094");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
