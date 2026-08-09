using CardService.Api.Services;
using CardService.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ICardService, CardService.Api.Services.CardService>();
builder.Services.AddSingleton<IAllowedActionsResolver, AllowedActionsResolver>();

builder.Services.AddHealthChecks();

builder.Services.AddProblemDetails();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CardService.Api.Middleware.CorrelationIdMiddleware>();

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
