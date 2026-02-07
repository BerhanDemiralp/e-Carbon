using eCarbon.Api.Domain.Entities;
using eCarbon.Api.Domain.Enums;

namespace eCarbon.Api.Common.Persistence.Seeding;

public static class EmissionFactorsSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (context.EmissionFactors.Any())
        {
            return; // Already seeded
        }

        var factors = new List<EmissionFactor>
        {
            // Scope 1 - Direct emissions (fuel combustion)
            new EmissionFactor
            {
                Id = Guid.NewGuid(),
                ActivityType = ActivityType.Diesel,
                Unit = "L",
                KgCo2ePerUnit = 2.68m,
                Source = "IPCC 2021",
                Year = 2024,
                Region = "Global",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new EmissionFactor
            {
                Id = Guid.NewGuid(),
                ActivityType = ActivityType.Gasoline,
                Unit = "L",
                KgCo2ePerUnit = 2.31m,
                Source = "IPCC 2021",
                Year = 2024,
                Region = "Global",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new EmissionFactor
            {
                Id = Guid.NewGuid(),
                ActivityType = ActivityType.NaturalGas,
                Unit = "m³",
                KgCo2ePerUnit = 2.0m,
                Source = "IPCC 2021",
                Year = 2024,
                Region = "Global",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new EmissionFactor
            {
                Id = Guid.NewGuid(),
                ActivityType = ActivityType.Propane,
                Unit = "L",
                KgCo2ePerUnit = 1.51m,
                Source = "IPCC 2021",
                Year = 2024,
                Region = "Global",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new EmissionFactor
            {
                Id = Guid.NewGuid(),
                ActivityType = ActivityType.Coal,
                Unit = "kg",
                KgCo2ePerUnit = 2.86m,
                Source = "IPCC 2021",
                Year = 2024,
                Region = "Global",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new EmissionFactor
            {
                Id = Guid.NewGuid(),
                ActivityType = ActivityType.Biogas,
                Unit = "m³",
                KgCo2ePerUnit = 0.05m,
                Source = "IPCC 2021",
                Year = 2024,
                Region = "Global",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },

            // Scope 2 - Indirect emissions (purchased electricity)
            new EmissionFactor
            {
                Id = Guid.NewGuid(),
                ActivityType = ActivityType.Electricity,
                Unit = "kWh",
                KgCo2ePerUnit = 0.5m,
                Source = "EPA eGRID 2023",
                Year = 2024,
                Region = "Global",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },

            // Renewable energy (near zero emissions)
            new EmissionFactor
            {
                Id = Guid.NewGuid(),
                ActivityType = ActivityType.Solar,
                Unit = "kWh",
                KgCo2ePerUnit = 0.05m,
                Source = "IPCC 2021",
                Year = 2024,
                Region = "Global",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new EmissionFactor
            {
                Id = Guid.NewGuid(),
                ActivityType = ActivityType.Wind,
                Unit = "kWh",
                KgCo2ePerUnit = 0.02m,
                Source = "IPCC 2021",
                Year = 2024,
                Region = "Global",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.EmissionFactors.AddRangeAsync(factors);
        await context.SaveChangesAsync();
    }
}