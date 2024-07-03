using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Greeting API", Version = "v1" });
});
builder.Services.AddHttpClient();
builder.Services.AddCors(cors =>
{
    cors.AddPolicy("HNG", pol =>
    {
        pol.AllowAnyOrigin() // Allows all origins
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
});

var app = builder.Build();

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    ForwardLimit = 1
};

try
{
    var networkIp = "192.168.0.0";
    int subnetMask = 24; 

    if (IPAddress.TryParse(networkIp, out IPAddress networkIpAddress))
    {
        forwardedHeadersOptions.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(networkIpAddress, subnetMask));
    }
    else
    {
        Console.WriteLine($"Invalid network IP address: {networkIp}");
    }

    app.UseForwardedHeaders(forwardedHeadersOptions);
}
catch (FormatException ex)
{
    Console.WriteLine($"Error parsing IP address: {ex.Message}");
}

app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Greeting API v1"));
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseCors("HNG");

app.UseAuthorization();

app.MapControllers();

app.Run();
