using System.ComponentModel.DataAnnotations;

namespace UserManagementAPI.DTO;

public class BaseUserDto
{
    [Required(ErrorMessage = "User name is required")]
    [MaxLength(50)]
    public required string FullName { get; set; }
   
    [EmailAddress(ErrorMessage = "Invalid email format")]
    [Required(ErrorMessage = "Email is required")]
    [MaxLength(50)]
    public required string Email { get; set; } 
    
    [Required(ErrorMessage = "Department is required")]
    [MaxLength(50)]
    public required string DepartmentName{ get; set; }  
}
public class UserAddDto:BaseUserDto
{ }

public class UserUpdateDto  : BaseUserDto
{
// add anything if you need more item during the update.
}

public class UserResponseDto : BaseUserDto
{
    public int UserId { get; set; }
}