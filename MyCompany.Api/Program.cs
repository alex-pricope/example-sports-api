using System.Text;
using MyCompany.Domain.Interfaces;
using MyCompany.Domain.Repositories;
using MyCompany.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCompany.Api.Mapping;
using MyCompany.Api.Middleware;
using MyCompany.Data;
using Serilog;

namespace MyCompany.Api;

public class Program
{
    public static void Main(string[] args)
    {
        // Instantiate logger
        var logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Filter.ByExcluding(@"RequestPath like '%/swagger%' 
                                or RequestPath like '%/_vs%'
                                or RequestPath like '%/_framework%'")
            .WriteTo.Console()
            .CreateLogger();
        Log.Logger = logger;
        
        var builder = WebApplication.CreateBuilder(args);
        
        // Register Serilog logger
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);
        builder.Services.AddLogging();

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddTransient<ErrorHandlingMiddleware>();
        builder.Services.AddScoped<ITeamManagementService, TeamManagementService>();
        builder.Services.AddScoped<IPlayerManagementService, PlayerManagementService>();
        builder.Services.AddScoped<ITeamRepository, TeamRepository>();
        builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
        builder.Services.AddAutoMapper(typeof(ApplicationMappingProfile));
        builder.Services.AddDbContext<ApplicationDbContext>(options => options
            .UseSqlite(builder.Configuration.GetConnectionString("LocalConnection")));
        
        // Custom ErrorDetails for ValidationResult error
        // https://stackoverflow.com/questions/51145243/how-do-i-customize-asp-net-core-model-binding-errors
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                // Gather the errors from the model state and create the custom ErrorDetails
                var errors = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .SelectMany(kvp => kvp.Value.Errors.Select(e => new { Field = kvp.Key, Message = e.ErrorMessage }));

                var stringBuilder = new StringBuilder("Validation errors occurred:");

                foreach (var error in errors)
                    stringBuilder.AppendLine($" Field: {error.Field}, Error: {error.Message}");

                var errorResponse = new ErrorDetails
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = stringBuilder.ToString(),
                };

                return new BadRequestObjectResult(errorResponse);
            };
        });
        
        // Add swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Example Sports API",
                Description = "Teams&Players API",
                Version = "v1"
            });
        });

        var app = builder.Build();
        
        // Register middleware
        app.UseMiddleware<ErrorHandlingMiddleware>();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        
        // Auto-create database and tables
        using (var scope = app.Services.CreateScope())
        {
            // Creates database and tables if they do not exist
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.EnsureCreated(); 
        }
        
        app.Run();
    }
}