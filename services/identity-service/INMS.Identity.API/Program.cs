using Microsoft.EntityFrameworkCore;
using INMS.Identity.Infrastructure.Persistence;
using INMS.Identity.Infrastructure.Repositories;
using INMS.Identity.Domain.Interfaces;
using INMS.Identity.Application.Interfaces;
using INMS.Identity.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
var identityConn = builder.Configuration.GetConnectionString("IdentityConnection");
if (!string.IsNullOrWhiteSpace(identityConn))
{
    builder.Services.AddDbContext<IdentityDbContext>(options =>
        options.UseSqlServer(identityConn));
}
else
{
    // Fallback to a local SQLite file for development/migration convenience
    builder.Services.AddDbContext<IdentityDbContext>(options =>
        options.UseSqlite("Data Source=identity.db"));
}

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAccountRequestRepository, AccountRequestRepository>();
builder.Services.AddScoped<IUserAreaAssignmentRepository, UserAreaAssignmentRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountRequestService, AccountRequestService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<UserAreaAssignmentService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
