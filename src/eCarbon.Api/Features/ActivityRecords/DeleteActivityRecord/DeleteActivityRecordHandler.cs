using eCarbon.Api.Common.Exceptions;
using eCarbon.Api.Common.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eCarbon.Api.Features.ActivityRecords.DeleteActivityRecord;

public class DeleteActivityRecordHandler : IRequestHandler<DeleteActivityRecordCommand, DeleteActivityRecordResponse>
{
    private readonly AppDbContext _dbContext;

    public DeleteActivityRecordHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<DeleteActivityRecordResponse> Handle(DeleteActivityRecordCommand request, CancellationToken cancellationToken)
    {
        var record = await _dbContext.ActivityRecords
            .FirstOrDefaultAsync(ar => ar.Id == request.Id, cancellationToken);

        if (record == null)
        {
            throw new NotFoundException("ActivityRecord", request.Id);
        }

        // Soft delete
        record.IsDeleted = true;
        record.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new DeleteActivityRecordResponse(
            record.Id,
            "Activity record successfully deleted (soft delete)");
    }
}