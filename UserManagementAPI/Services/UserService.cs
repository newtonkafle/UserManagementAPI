using Microsoft.EntityFrameworkCore;
using UserManagementAPI.DTO;
using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public class UserService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    //get list of users or user by id
   public async Task<ICollection<UserResponseDto>> GetUsers()
   { 
       return await (from u in _context.Users
                    join d in _context.Departments
                        on u.DepartmentId equals d.DepartmentId
                        select new UserResponseDto
                        {
                            UserId = u.UserId,
                            FullName = u.FullName,
                            Email = u.Email,
                            DepartmentName = d.Name
                            
                        }).ToListAsync();
       
   }
   //get user by id
   public async Task<OperationalResult<User>> GetUser(int id)
   {
       var user = await _context.Users.FindAsync(id);
       return user == null ? OperationalResult<User>.Fail($"User not found with id {id}") : OperationalResult<User>.Ok(user);
   }
   
   //get department by name
   private async Task<Department?> GetDepartment(string name)
   {
       return await _context.Departments.Where(d => d.Name == name).FirstOrDefaultAsync();
   }
  
   //add user to db and return OperationalResult<User> with data or error message.
   public async Task<OperationalResult<User>> AddUser(UserAddDto userAddDto)
   {
       var department = await GetDepartment(userAddDto.DepartmentName);

       if (department is null)
           return OperationalResult<User>.Fail($"Unable to add user. No department exists with {userAddDto.DepartmentName}");
       var user = new User
           {
               FullName = userAddDto.FullName,
               Email = userAddDto.Email,
               DepartmentId = department.DepartmentId
           };
       _context.Users.Add(user);
       await _context.SaveChangesAsync();
       return OperationalResult<User>.Ok(user);

   }
  
   // update user if valid input and return OperationalResult<User> with data or error message.
   public async Task<OperationalResult<User>> UpdateUser(int id, UserUpdateDto userUpdateDto)
   {
       
       var user = await _context.Users.FindAsync(id);
       // if no user return error no user exists
       if (user is null)
           return OperationalResult<User>.Fail($"No user exists with id {id}");
       
       var department = await GetDepartment(userUpdateDto.DepartmentName);
       //if no department return error no department with that name
       if (department is null)
           return OperationalResult<User>.Fail($"No department exists with {userUpdateDto.DepartmentName} name");
       
       user.FullName = userUpdateDto.FullName;
       user.Email = userUpdateDto.Email;
       user.DepartmentId = department.DepartmentId;
       _context.Users.Update(user);
       await _context.SaveChangesAsync();
       //return user when update completes
       return OperationalResult<User>.Ok(user);
   }
   
   // remove a user by id
   public async Task<OperationalResult<User>> DeleteUser(int id)
   {
       var user = await _context.Users.FindAsync(id); 
       if (user is null)
           return OperationalResult<User>.Fail($"No user exists with id {id}");
       _context.Users.Remove(user);
       await _context.SaveChangesAsync();
       return OperationalResult<User>.Ok(user);
   }
}