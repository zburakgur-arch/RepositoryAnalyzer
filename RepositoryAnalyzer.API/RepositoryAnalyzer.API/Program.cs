using MediatR;
using RepositoryAnalyzer.API.Queries;
using RepositoryAnalyzer.Domain.Services;
using RepositoryAnalyzer.Application.Settings;
using RepositoryAnalyzer.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR registration
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

// Add a pipeline behavior to handle exceptions
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>));

// Bind GitSettings from configuration and register as singleton
var gitSettings = builder.Configuration.GetSection("GitSettings").Get<GitSettings>() ?? new GitSettings();
builder.Services.AddSingleton(gitSettings);

// Git service registration
builder.Services.AddScoped<IGitService, GitService>();

// Register IHttpContextAccessor for dependency injection
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/analyze", async (string url, IMediator mediator) =>
{
    return Results.Ok(await mediator.Send(new AnalyzeRepositoryQuery(url)));
})
.WithName("AnalyzeRepository")
.WithOpenApi();

app.Run();