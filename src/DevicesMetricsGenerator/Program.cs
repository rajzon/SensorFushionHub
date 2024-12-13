using DevicesMetricsGenerator.Startup;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddStartupServices(builder.Configuration);

var host = builder.Build();
host.Run();