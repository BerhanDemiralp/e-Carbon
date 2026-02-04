namespace eCarbon.Api.Features.Companies.CreateCompany;

public class CreateCompanyRequest
{
    public string Name { get; set; } = string.Empty;
    
    public CreateCompanyCommand ToCommand() => new(Name);
}