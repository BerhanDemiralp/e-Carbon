using eCarbon.Api.Common.Behaviors;
using eCarbon.Api.Common.Middleware;
using eCarbon.Api.Common.Persistence;
using eCarbon.Api.Common.Persistence.Seeding;
using eCarbon.Api.Features.Companies.CreateCompany;
using eCarbon.Api.Features.Companies.GetCompany;
using eCarbon.Api.Features.Companies.ListCompanies;
using eCarbon.Api.Features.Companies.UpdateCompany;
using eCarbon.Api.Features.Companies.DeleteCompany;
using eCarbon.Api.Features.Facilities.CreateFacility;
using eCarbon.Api.Features.Facilities.GetFacility;
using eCarbon.Api.Features.Facilities.ListFacilitiesByCompany;
using eCarbon.Api.Features.Facilities.ListAllFacilities;
using eCarbon.Api.Features.Facilities.UpdateFacility;
using eCarbon.Api.Features.ActivityRecords.CreateActivityRecord;
using eCarbon.Api.Features.ActivityRecords.ListActivityRecordsByFacility;
using eCarbon.Api.Features.ActivityRecords.ListAllActivityRecords;
using eCarbon.Api.Features.ActivityRecords.UpdateActivityRecord;
using eCarbon.Api.Features.ActivityRecords.DeleteActivityRecord;
using eCarbon.Api.Features.Calculations.PreviewMonthlyEmissions;
using eCarbon.Api.Features.Snapshots.CreateSnapshot;
using eCarbon.Api.Features.Snapshots.GetSnapshot;
using eCarbon.Api.Features.Snapshots.FreezeSnapshot;
using eCarbon.Api.Features.Snapshots.ListSnapshots;
using eCarbon.Api.Features.Reports.GenerateReport;
using eCarbon.Api.Features.Reports.DownloadReport;
using eCarbon.Api.Features.Reports.VerifyReport;
using eCarbon.Api.Features.Reports.ListReports;
using eCarbon.Api.Features.AuditLogs.ListAuditLogs;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure JSON to accept camelCase from frontend
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
           .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// Add MediatR with behaviors
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuditingBehavior<,>));
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimingMiddleware>();
app.UseCors("AllowAll");

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
    
    // Seed emission factors
    await EmissionFactorsSeeder.SeedAsync(dbContext);
}

// Map Company endpoints
app.MapCreateCompany();
app.MapGetCompany();
app.MapListCompanies();
app.MapUpdateCompany();
app.MapDeleteCompany();

// Map Facility endpoints
app.MapCreateFacility();
app.MapGetFacility();
app.MapListAllFacilities();
app.MapListFacilitiesByCompany();
app.MapUpdateFacility();

// Map Activity Record endpoints
app.MapCreateActivityRecord();
app.MapListActivityRecordsByFacility();
app.MapListAllActivityRecords();
app.MapUpdateActivityRecord();
app.MapDeleteActivityRecord();

// Map Calculations endpoints
app.MapPreviewMonthlyEmissions();

// Map Snapshot endpoints
app.MapCreateSnapshot();
app.MapGetSnapshot();
app.MapFreezeSnapshot();
app.MapListSnapshots();

// Map Report endpoints
app.MapGenerateReport();
app.MapDownloadReport();
app.MapVerifyReport();
app.MapListReports();

// Map Audit Log endpoints
app.MapListAuditLogs();

app.MapGet("/", () => Results.Ok(new { message = "eCarbon API is running", version = "1.0" }));

app.Run();