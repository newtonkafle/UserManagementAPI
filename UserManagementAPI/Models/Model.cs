namespace UserManagementAPI.Models;

public class Department
{
    public int DepartmentId { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    // Navigation property (one department → many users)
    public ICollection<User> Users { get; set; } = new List<User>();
}

public class User
{
    public int UserId { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; } 

    // Foreign key to Department
    public int DepartmentId { get; set; }
}