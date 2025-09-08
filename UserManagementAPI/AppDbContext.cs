using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Models;

namespace UserManagementAPI;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
   // registering teh department table to the database and it will be used to query the database as well.
   public DbSet<Department> Departments { get; set; }
   public DbSet<User> Users { get; set; }
    
   //seeding data to the database table
   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
       modelBuilder.Entity<Department>().HasData(
           new Department { DepartmentId = 1, Name = "Information Technology", Type = "IT" },
           new Department { DepartmentId = 2, Name = "Human Resources", Type = "HR" },
           new Department { DepartmentId = 3, Name = "Finance", Type = "Finance" },
           new Department { DepartmentId = 4, Name = "Marketing", Type = "Marketing" }
        
       );

       modelBuilder.Entity<User>().HasData(
           new User { UserId = 1, FullName = "Alice Johnson", Email = "alice@company.com", DepartmentId = 1 },
           new User { UserId = 2, FullName = "Bob Smith", Email = "bob@company.com", DepartmentId = 2 },
           new User { UserId = 3, FullName = "Charlie Brown", Email = "charlie@company.com", DepartmentId = 3 },
           new User { UserId = 4, FullName = "Diana Prince", Email = "diana@company.com", DepartmentId = 1 },
           new User { UserId = 5, FullName = "Ethan Hunt", Email = "ethan@company.com", DepartmentId = 4 }
       );
   }
}