using System.Text.Json.Serialization;

namespace WebAPI.Models;

public class Department
{
    public int DepartmentId { get; set; }
    public string DepartmentName { get; set; }

    public string TenantId { get; set; }
    [JsonIgnore]
    public virtual ICollection<EmployeeDepartment> EmployeeDepartments { get; set; }
}