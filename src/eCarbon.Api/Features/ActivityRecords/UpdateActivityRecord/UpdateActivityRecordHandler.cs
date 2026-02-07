using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.ActivityRecords.UpdateActivityRecord;

public class UpdateActivityRecordHandler : IRequestHandler<UpdateActivityRecordCommand, UpdateActivityRecordResponse>
{
    private readonly AppDbContext _dbContext;

    public UpdateActivityRecordHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UpdateActivityRecordResponse> Handle(UpdateActivityRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ActivityRecords
            .FirstOrDefaultAsync(ar => ar.Id == request.Id, cancellationToken);

        if (record == null)
        {
            throw new NotFoundException("ActivityRecord", request.Id);
        }

        // Verify emission factor exists for new activity type and unit
        var factorExists = await _dbContext.EmissionFactors
            .AnyAsync(ef => ef.ActivityType == request.ActivityType 
                && ef.Unit == request.Unit 
                && ef.IsActive, cancellationToken);

        if (!factorExists)
        {
            throw new EmissionFactorNotFoundException(request.ActivityType.ToString(), request.Unit);
        }

        record.ActivityDate = request.ActivityDate;
        record.Scope = request.Scope;
        record.ActivityType = request.ActivityType;
        record.Quantity = request.Quantity;
        record.Unit = request.Unit;
        record.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new UpdateActivityRecordResponse(
            record.Id,
            record.FacilityId,
            record.ActivityDate,
            record.Scope,
            record.ActivityType,
            record.Quantity,
            record.Unit,
            record.UpdatedAt);
    }
}