namespace Business.Dtos;

public class ClientDto
{
    public int Id { get; set; }
    public string ClientName { get; set; } = null!;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
