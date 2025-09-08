namespace UserManagementAPI.Services;

public class OperationalResult<T>
{
   public bool Success { get; set; } = false;
   public T? Data { get; set; }
   public string? ErrorMessage { get; set; }

   // helper method to return data when operation succeed.
   public static OperationalResult<T> Ok(T data)
   {
      return new OperationalResult<T>
      {
         Success = true,
         Data = data,
      };
   }

   // helper method to return error message if operation failed.
   public static OperationalResult<T> Fail(string errorMessage)
   {
      return new OperationalResult<T>
      {
         Success = false,
         ErrorMessage = errorMessage,
      };
   }
}