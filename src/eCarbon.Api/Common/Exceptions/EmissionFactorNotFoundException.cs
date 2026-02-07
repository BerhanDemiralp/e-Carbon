namespace eCarbon.Api.Common.Exceptions;

public class EmissionFactorNotFoundException : Exception
{
    public EmissionFactorNotFoundException(string activityType, string unit) 
        : base($"No active emission factor found for activity type '{activityType}' with unit '{unit}'")
    {
        ActivityType = activityType;
        Unit = unit;
    }

    public string ActivityType { get; }
    public string Unit { get; }
}