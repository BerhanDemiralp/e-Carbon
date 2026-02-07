using MediatR;

namespace eCarbon.Api.Features.ActivityRecords.DeleteActivityRecord;

public record DeleteActivityRecordCommand(Guid Id) : IRequest<DeleteActivityRecordResponse>;

public record DeleteActivityRecordResponse(
    Guid Id,
    string Message);