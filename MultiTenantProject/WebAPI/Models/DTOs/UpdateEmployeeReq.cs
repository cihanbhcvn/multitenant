namespace WebAPI.Models.DTOs;

public class UpdateEmployeeReq
{
    public int EmployeeId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}