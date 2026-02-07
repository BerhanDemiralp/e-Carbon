using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using eCarbon.Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.ActivityRecords.CreateActivityRecord;

public class CreateActivityRecordHandler : IRequestHandler<CreateActivityRecordCommand, CreateActivityRecordResponse>
{
    private readonly AppDbContext _dbContext;

    public CreateActivityRecordHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CreateActivityRecordResponse> Handle(CreateActivityRecordCommand request, CancellationToken cancellationToken)
    {
        // Verify facility exists
        var facilityExists = await _dbContext.Facilities
            .AnyAsync(f => f.Id == request.FacilityId, cancellationToken);

        if (!facilityExists)
        {
            throw new NotFoundException("Facility", request.FacilityId);
        }

        // Verify emission factor exists for this activity type and unit
        var factorExists = await _dbContext.EmissionFactors
            .AnyAsync(ef => ef.ActivityType == request.ActivityType 
                && ef.Unit == request.Unit 
                && ef.IsActive, cancellationToken);

        if (!factorExists)
        {
            throw new EmissionFactorNotFoundException(request.ActivityType.ToString(), request.Unit);
        }

        var activityRecord = new ActivityRecord
        {
            Id = Guid.NewGuid(),
            FacilityId = request.FacilityId,
            ActivityDate = request.ActivityDate,
            Scope = request.Scope,
            ActivityType = request.ActivityType,
            Quantity = request.Quantity,
            Unit = request.Unit,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        _dbContext.ActivityRecords.Add(activityRecord);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateActivityRecordResponse(
            activityRecord.Id,
            activityRecord.FacilityId,
            activityRecord.ActivityDate,
            activityRecord.Scope,
            activityRecord.ActivityType,
            activityRecord.Quantity,
            activityRecord.Unit,
            activityRecord.CreatedAt);
    }
}