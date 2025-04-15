using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Models;
using WebAPI.Models.DTOs;

namespace WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        public async Task<IActionResult> GetEmployees([FromQuery] int page = 1, int pageSize = 10)
        {
            var tenantId = User.Claims.FirstOrDefault(c => c.Type == "TenantId")?.Value;
            if (tenantId == null) return Unauthorized();

            var employees = await _context.Employees
                .Where(e => e.TenantId == tenantId)
                .Include(e => e.EmployeeDepartments)
                .ThenInclude(ed => ed.Department)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return Ok(employees);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetEmployeeById([FromQuery] int employeeId)
        {
            var tenantId = User.Claims.FirstOrDefault(c => c.Type == "TenantId")?.Value;
            if (tenantId == null) return Unauthorized();

            var employees = await _context.Employees
                .Where(e => e.TenantId == tenantId && e.EmployeeId == employeeId)
                .Include(e => e.EmployeeDepartments)
                .ThenInclude(ed => ed.Department)
                .FirstOrDefaultAsync();

            return Ok(employees);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeReq createEmployeeReq)
        {
            var exists = await _context.Employees.AnyAsync(x =>
                x.Name == createEmployeeReq.Name && x.Email == createEmployeeReq.Email && x.TenantId == x.TenantId);

            if (exists)
            {
                return BadRequest("Employee already exists");
            }
            else
            {
                _context.Employees.Add(new Employee()
                {
                    Name = createEmployeeReq.Name,
                    Email = createEmployeeReq.Email,
                    TenantId = createEmployeeReq.TenantId
                });
                await _context.SaveChangesAsync();

                return Ok();
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("update")]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeReq updateEmployeeReq)
        {
            var exists = await _context.Employees.AnyAsync(x => x.EmployeeId == updateEmployeeReq.EmployeeId);

            if (exists)
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == updateEmployeeReq.EmployeeId);

                employee.Name = updateEmployeeReq.Name;
                employee.Email = updateEmployeeReq.Email;

               _context.Entry(employee).State = EntityState.Modified;
               await _context.SaveChangesAsync();

               return Ok();
            }
            else
            {
                return BadRequest("Employee doesn't exist");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("delete")]
        public async Task<IActionResult> DeleteEmployee([FromBody] int employeeId)
        {
            var exists = await _context.Employees.AnyAsync(x => x.EmployeeId == employeeId);

            if (exists)
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employeeId);

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                return Ok();
            }
            else
            {
                return BadRequest("Employee doesn't exist");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignEmployee([FromBody] EmployeeDepartment assignment)
        {
            var employee = await _context.Employees.FindAsync(assignment.EmployeeId);
            var department = await _context.Departments.FindAsync(assignment.DepartmentId);

            if (employee == null || department == null || employee.TenantId != department.TenantId)
            {
                return BadRequest("Invalid assignment or cross-tenant operation.");
            }

            _context.EmployeeDepartments.Add(assignment);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
