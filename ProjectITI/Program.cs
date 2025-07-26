using Microsoft.EntityFrameworkCore;
using ProjectITI.Data;
using ProjectITI.Model;

namespace ProjectITI
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var context = new AppDBContext();
            context.Database.EnsureCreated();

            bool exit = false;
            while (!exit)
            {
                Console.WriteLine("1. Add Employee");
                Console.WriteLine("2. Add Department");
                Console.WriteLine("3. Add Project");
                Console.WriteLine("4. Edit Employee");
                Console.WriteLine("5. Edit Department");
                Console.WriteLine("6. Edit Project");
                Console.WriteLine("7. Delete Employee");
                Console.WriteLine("8. Delete Department");
                Console.WriteLine("9. Delete Project");
                Console.WriteLine("10. Display Employees");
                Console.WriteLine("11. Display Departments");
                Console.WriteLine("12. Display Projects");
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");

                switch (Console.ReadLine())
                {
                    case "1": await AddEmployee(); break;
                    case "2": await AddDepartment(); break;
                    case "3": await AddProject(); break;
                    case "4": await EditEmployee(); break;
                    case "5": await EditDepartment(); break;
                    case "6": await EditProject(); break;
                    case "7": await DeleteEmployee(); break;
                    case "8": await DeleteDepartment(); break;
                    case "9": await DeleteProject(); break;
                    case "10": await DisplayEmployees(); break;
                    case "11": await DisplayDepartments(); break;
                    case "12": await DisplayProjects(); break;
                    case "0": exit = true; break;
                    default: Console.WriteLine("Invalid option"); break;
                }
            }


            async Task AddEmployee()
            {
                Console.Clear();
                Console.Clear();
                Console.Write("Employee name: ");
                string name = Console.ReadLine();
                var emp = new Employee { Name = name };
                context.Employees.Add(emp);
                await context.SaveChangesAsync();
                Console.WriteLine("Employee added.");
                Console.ReadKey();
                Console.Clear();

            }

            async Task AddDepartment()
            {
                Console.Clear();
                Console.Write("Department name: ");
                string name = Console.ReadLine();
                var dept = new Department { Name = name };
                await context.Departments.AddAsync(dept);
                await context.SaveChangesAsync();
                Console.WriteLine("Department added.");
                Console.ReadKey();
                Console.Clear();
            }

            async Task AddProject()
            {
                Console.Clear();
                Console.Write("Project name: ");
                string name = Console.ReadLine();
                var proj = new Project { Name = name };
                await context.Projects.AddAsync(proj);
                await context.SaveChangesAsync();
                Console.WriteLine("Project added.");
                Console.ReadKey();
                Console.Clear();
            }

            async Task EditEmployee()
            {
                Console.Clear();
                Console.Write("Employee ID: ");
                int id = int.Parse(Console.ReadLine());
                var emp = await context.Employees.Include(e => e.EmployeeProjects).FirstOrDefaultAsync(e => e.EmployeeId == id);

                if (emp == null) { Console.WriteLine("Not found."); return; }

                Console.Write("New name : ");
                string name = Console.ReadLine();
                emp.Name = name ?? emp.Name;

                Console.Write("Assign to Department ID : ");
                int deptId = int.Parse(Console.ReadLine());
                if (await context.Departments.AnyAsync(d => d.DepartmentId == deptId))
                    emp.DepartmentId = deptId;

                Console.WriteLine("Manage Projects separated for comma,  '-' for remove):");
                string[] inputs = Console.ReadLine().Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (var token in inputs)
                {
                    if (token.StartsWith("-"))
                    {
                        int pid = int.Parse(token.TrimStart('-'));
                        var rel = emp.EmployeeProjects.FirstOrDefault(ep => ep.ProjectId == pid);
                        if (rel != null) emp.EmployeeProjects.Remove(rel);
                    }
                    else
                    {
                        int pid = int.Parse(token);
                        if (!emp.EmployeeProjects.Any(ep => ep.ProjectId == pid))
                            emp.EmployeeProjects.Add(new EmployeeProject { ProjectId = pid });
                    }
                }

                await context.SaveChangesAsync();
                Console.WriteLine("Employee updated.");
                Console.ReadKey();
                Console.Clear();
            }

            async Task EditDepartment()
            {
                Console.Clear();
                Console.Write("Department ID: ");
                int id = int.Parse(Console.ReadLine());
                var dept = await context.Departments.FirstOrDefaultAsync(d => d.DepartmentId == id);
                if (dept == null) { Console.WriteLine("Not found."); return; }

                Console.Write("New name : ");
                string name = Console.ReadLine();
                dept.Name = name ?? dept.Name;

                await context.SaveChangesAsync();
                Console.WriteLine("Department updated.");
                Console.ReadKey();
                Console.Clear();
            }

            async Task EditProject()
            {
                Console.Clear();
                Console.Write("Project ID: ");
                int id = int.Parse(Console.ReadLine());
                var proj = await context.Projects.FirstOrDefaultAsync(p => p.ProjectId == id);
                if (proj == null) { Console.WriteLine("Not found."); return; }

                Console.Write("New name : ");
                string name = Console.ReadLine();
                proj.Name = name ?? proj.Name;

                await context.SaveChangesAsync();

                Console.WriteLine("Project updated.");
                Console.ReadKey();
                Console.Clear();
            }

            async Task DeleteEmployee()
            {
                Console.Clear();
                Console.Write("Employee ID: ");
                int id = int.Parse(Console.ReadLine());
                var emp = await context.Employees.FindAsync(id);
                if (emp == null)
                {
                    Console.WriteLine("Employee Not Found");
                    return;


                }
                context.Employees.Remove(emp);
                await context.SaveChangesAsync();
                Console.WriteLine("Employee deleted.");
                Console.ReadKey();
                Console.Clear();
            }

            async Task DeleteDepartment()
            {
                Console.Clear();
                Console.Write("Department ID: ");
                int id = int.Parse(Console.ReadLine());
                var dept = await context.Departments.Include(d => d.Employees).FirstOrDefaultAsync(d => d.DepartmentId == id);
                if (dept != null)
                {
                    Console.WriteLine("Department NotFound.");
                    return;

                }
                if (dept.Employees.Any())
                {
                    Console.WriteLine("Cannot delete department with employees.");
                    return;
                }
                context.Departments.Remove(dept);
                await context.SaveChangesAsync();
                Console.WriteLine("Department deleted.");
                Console.ReadKey();
                Console.Clear();
            }

            async Task DeleteProject()
            {
                Console.Clear();
                Console.Write("Project ID: ");
                int id = int.Parse(Console.ReadLine());
                var proj = await context.Projects.FindAsync(id);
                if (proj != null)
                {
                    context.Projects.Remove(proj);
                    await context.SaveChangesAsync();
                    Console.WriteLine("Project deleted.");
                }

                Console.ReadKey();
                Console.Clear();
            }

            async Task DisplayEmployees()
            {
                Console.Clear();
                var emps = await context.Employees.Include(e => e.Department).Include(e => e.EmployeeProjects).ThenInclude(ep => ep.Project).ToListAsync();
                foreach (var emp in emps)
                {
                    Console.WriteLine($"ID: {emp.EmployeeId}, Name: {emp.Name}, Department: {emp.Department?.Name ?? "None"}");
                    Console.WriteLine("Projects: " + string.Join(", ", emp.EmployeeProjects.Select(ep => ep.Project.Name)));
                }

                Console.ReadKey();
                Console.Clear();
            }

            async Task DisplayDepartments()
            {
                Console.Clear();
                var depts = await context.Departments.Include(d => d.Employees).ToListAsync();
                foreach (var dept in depts)
                {
                    Console.WriteLine($"ID: {dept.DepartmentId}, Name: {dept.Name}, Employees: {dept.Employees.Count}");
                }
                Console.ReadKey();
                Console.Clear();
            }

            async Task DisplayProjects()
            {
                Console.Clear();
                var projs = await context.Projects.Include(p => p.EmployeeProjects).ThenInclude(ep => ep.Employee).ToListAsync();
                foreach (var proj in projs)
                {
                    Console.WriteLine($"ID: {proj.ProjectId}, Name: {proj.Name}, Employees: " + string.Join(", ", proj.EmployeeProjects.Select(ep => ep.Employee.Name)));
                }
                Console.ReadKey();
                Console.Clear();
            }
        }




    }










}
