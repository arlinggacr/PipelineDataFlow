using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using PipelineDataFlow.Data;
using PipelineDataFlow.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Postgresql Connection
builder
    .Services
    .AddDbContext<AppDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("LocalDb"));
    })
    .AddDbContext<Local2AppDbContext>(options =>
    {
        options.UseNpgsql(builder.Configuration.GetConnectionString("Local2Db"));
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseForwardedHeaders(
    new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    }
);

app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();
