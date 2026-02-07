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
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
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

app.MapGet("/", () => Results.Ok(new { message = "eCarbon API is running", version = "1.0" }));

app.Run();