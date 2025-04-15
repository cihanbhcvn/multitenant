using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models;

[Keyless]
public class EmployeeDepartment
{
    public int EmployeeId { get; set; }
    [JsonIgnore]
    public  virtual Employee Employee { get; set; }

    public int DepartmentId { get; set; }
    public  virtual Department Department { get; set; }
}