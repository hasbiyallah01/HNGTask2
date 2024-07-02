using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("*",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    ForwardLimit = 1 // Set the limit for how many entries in the headers are processed
};

try
{
    var networkIp = "192.168.0.0"; // This is the base IP of your network (subnet)
    int subnetMask = 24; // Based on the subnet mask 255.255.255.0

    if (IPAddress.TryParse(networkIp, out IPAddress networkIpAddress))
    {
        forwardedHeadersOptions.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(networkIpAddress, subnetMask)); // Adjust the subnet mask as needed
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("*");

app.MapControllers();

app.Run();

