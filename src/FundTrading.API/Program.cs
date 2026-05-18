using FundTrading.API.Jobs;
using FundTrading.API.Middlewares;
using FundTrading.Application.Queries;
using FundTrading.Data.Repository;
using FundTrading.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using Quartz;
using Serilog;
using Serilog.Events;

//Log.Logger = new LoggerConfiguration()
//    .Enrich.FromLogContext()
//    .WriteTo.Console()
//    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
//    .CreateLogger();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(
        outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] [CorrelationId: {CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
    "logs/app-.txt",
    rollingInterval: RollingInterval.Day,
    outputTemplate:
    "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] [CorrelationId: {CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        "logs/error-.txt",
        restrictedToMinimumLevel: LogEventLevel.Error,
        rollingInterval: RollingInterval.Day,
        outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] [CorrelationId: {CorrelationId}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FundTradingContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(FundTrading.Application.AssemblyReference).Assembly);
});

//builder.Services.Configure<TeamsSettings>(builder.Configuration.GetSection("Teams"));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IInvestmentFundRepository, InvestmentFundRepository>();
builder.Services.AddScoped<IFundOrderRepository, FundOrderRepository>();
builder.Services.AddScoped<ICustomerFundPositionRepository, CustomerFundPositionRepository>();
builder.Services.AddScoped<IFundOrderQuery, FundOrderQuery>();

//builder.Services.AddScoped<INotificationService, NotificationService>();
//builder.Services.AddHttpClient<INotificationChannel, TeamsNotificationChannel>();

builder.Services.AddQuartz(options =>
{
    var jobKey = JobKey.Create(nameof(ProcessScheduledOrdersJob));

    options
        .AddJob<ProcessScheduledOrdersJob>(jobKey)
        .AddTrigger(trigger =>
            trigger
                .ForJob(jobKey)
                .WithIdentity($"{nameof(ProcessScheduledOrdersJob)}-trigger")
                .WithCronSchedule(
                    "0 0 9 ? * MON-FRI",
                    cron => cron.InTimeZone(
                        TimeZoneInfo.FindSystemTimeZoneById(
                            "E. South America Standard Time"))));
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
