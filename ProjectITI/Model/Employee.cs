﻿namespace ProjectITI.Model
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }

        public int? DepartmentId { get; set; }
        public Department Department { get; set; }

        public ICollection<EmployeeProject> EmployeeProjects { get; set; } = new List<EmployeeProject>();
    }










}
