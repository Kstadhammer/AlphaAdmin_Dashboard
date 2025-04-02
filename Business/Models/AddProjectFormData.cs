using System;

namespace Business.Models;

public class AddProjectFormData
{
    public string Name { get; set; }
    public string ClientName { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Budget { get; set; }
}
