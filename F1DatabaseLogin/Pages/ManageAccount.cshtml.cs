using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

public class ManageAccountModel : PageModel
{
    [BindProperty]
    public string Email { get; set; }
    
    [BindProperty]
    public string Password { get; set; }

    public string UpdateMessage { get; set; }

    public void OnGet()
    {
        // Sayfa ilk yüklendiğinde yapılacak işlemler
    }

    public IActionResult OnPostUpdateEmail()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Email))
            {
                UpdateDatabase("Email", Email);
                UpdateMessage = "Email updated successfully!";
            }
            else
            {
                UpdateMessage = "Email cannot be empty!";
            }
        }
        catch (Exception ex)
        {
            UpdateMessage = $"Error updating email: {ex.Message}";
        }
        return Page();
    }

    public IActionResult OnPostUpdatePassword()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Password))
            {
                UpdateDatabase("Password", Password);
                UpdateMessage = "Password updated successfully!";
            }
            else
            {
                UpdateMessage = "Password cannot be empty!";
            }
        }
        catch (Exception ex)
        {
            UpdateMessage = $"Error updating password: {ex.Message}";
        }
        return Page();
    }

    private void UpdateDatabase(string column, string value)
    {
        string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";
        string query = $"UPDATE Users SET {column} = @value WHERE Id = @userID";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@value", value);
            command.Parameters.AddWithValue("@userID", GetCurrentUserID());
            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    private int GetCurrentUserID()
    {
        return 1;
    }
}
