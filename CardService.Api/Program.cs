using CardService.Api.Exceptions;
using CardService.Api.Models;
using CardService.Api.Services;
using CardService.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ICardService, CardService.Api.Services.CardService>();
builder.Services.AddSingleton<IAllowedActionsResolver, AllowedActionsResolver>();

builder.Services.Configure<CardServiceOptions>(
    builder.Configuration.GetSection(CardServiceOptions.SectionName));

builder.Services.AddHealthChecks();

builder.Services.AddExceptionHandler<OperationCanceledExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<CardService.Api.Middleware.CorrelationIdMiddleware>();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//in case of development I'm skipping SSL security layer
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

//app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();

public partial class Program { }