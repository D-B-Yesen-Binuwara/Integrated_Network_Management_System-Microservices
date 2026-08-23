using Microsoft.EntityFrameworkCore;
using INMS.Identity.Infrastructure.Persistence;
using INMS.Identity.Infrastructure.Repositories;
using INMS.Identity.Domain.Interfaces;
using INMS.Identity.Application.Interfaces;
using INMS.Identity.Application.Services;

var builder = WebApplication.CreateBuilder(args);
var useHttpsRedirection = builder.Configuration.GetValue("Security:UseHttpsRedirection", !builder.Environment.IsDevelopment());

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

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
var allowCredentials = builder.Configuration.GetValue("Cors:AllowCredentials", false);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE", "OPTIONS")
            .AllowAnyHeader()
            .SetPreflightMaxAge(TimeSpan.FromMinutes(10));

        // Do not allow cross-origin cookies unless the deployment explicitly
        // opts in after configuring its authentication and CSRF strategy.
        if (allowCredentials)
        {
            policy.AllowCredentials();
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (useHttpsRedirection)
{
    app.UseHttpsRedirection();
}
app.UseCors("Frontend");
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
    db.Database.Migrate();
}


app.Run();
