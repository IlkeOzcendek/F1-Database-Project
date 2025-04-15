using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

public class LoginModel : PageModel
{
    [BindProperty]
    public string? Email { get; set; } = string.Empty;

    [BindProperty]
    public string? Password { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public void OnGet()
    {
        HttpContext.Session.Clear();
    }

    public void OnPost()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            Message = "Email and Password are required.";
            return;
        }

        string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;";
        string query = "SELECT Role FROM users WHERE email = @Email AND password = @Password";

        try
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@Password", Password);

                    object? roleObj = cmd.ExecuteScalar();

                    if (roleObj != null)
                    {
                        int role = Convert.ToInt32(roleObj);
HttpContext.Session.SetString("UserEmail", Email);
        HttpContext.Session.SetInt32("UserRole", role);
                        // Role kontrolü ve yönlendirme
                        if (role == 1)
                        {
                            // Admin sayfasına yönlendir
                            Response.Redirect("/Admin");
                        }
                        else if (role == 2)
                        {
                            // Start Page'e yönlendir
                            Response.Redirect("/StartPage");
                        }
                        else
                        {
                            Message = "Unexpected role value. Please contact support.";
                        }
                    }
                    else
                    {
                        Message = "Invalid email or password.";
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            Message = "An error occurred while connecting to the database. Please try again later.";
            Console.WriteLine($"SQL Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Message = "An unexpected error occurred. Please try again.";
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
