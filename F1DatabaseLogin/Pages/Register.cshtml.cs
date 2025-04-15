using Microsoft.AspNetCore.Mvc.RazorPages; 

using Microsoft.Data.SqlClient; 

using Microsoft.AspNetCore.Mvc; 

 

 

public class RegisterModel : PageModel 

{ 

[BindProperty] 

public string Email { get; set; } 

[BindProperty] 

public string Password { get; set; } 

public string Message { get; set; } 

public void OnGet()
    {
        HttpContext.Session.Clear();
    }

 

public void OnPost() 

{ 

string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;"; 

 

try 

{ 

using (SqlConnection connection = new SqlConnection(connectionString)) 

{ 

connection.Open(); 

 

string query = @"INSERT INTO [dbo].[Users] (Email, Password) VALUES (@Email, @Password)"; 

using (SqlCommand command = new SqlCommand(query, connection)) 

{ 

command.Parameters.AddWithValue("@Email", Email); 

command.Parameters.AddWithValue("@Password", Password); 

 

int rowsAffected = command.ExecuteNonQuery(); 

if (rowsAffected > 0) 

{ 

Message = "User registered successfully."; 

} 

else 

{ 

Message = "Registration failed. Please try again."; 

} 

} 

} 

} 

catch (Exception ex) 

{ 

Message = $"An error occurred: {ex.Message}"; 

} 

} 

} 

 