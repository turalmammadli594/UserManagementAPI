using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// InMemory DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("UserManagementDb"));

var app = builder.Build();

// 1) Error-handling middleware first
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 2) Authentication middleware next
app.UseMiddleware<TokenAuthMiddleware>();

// 3) Logging middleware last (endpoint-ə ən yaxın, controllers-dan əvvəl)
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
