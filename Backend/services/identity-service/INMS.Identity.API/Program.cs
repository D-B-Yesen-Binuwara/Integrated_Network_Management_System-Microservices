using Microsoft.EntityFrameworkCore;
using INMS.Identity.Infrastructure.Persistence;
using INMS.Identity.Infrastructure.Repositories;
using INMS.Identity.Domain.Interfaces;
using INMS.Identity.Application.Interfaces;
using INMS.Identity.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddDbContext<IdentityDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("IdentityConnection");

    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IAccountRequestRepository, AccountRequestRepository>();
builder.Services.AddScoped<IUserAreaAssignmentRepository, UserAreaAssignmentRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountRequestService, AccountRequestService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<UserAreaAssignmentService>();

builder.Services.AddScoped<IAreaValidator, DefaultAreaValidator>();

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    db.Database.Migrate();
}


app.Run();
