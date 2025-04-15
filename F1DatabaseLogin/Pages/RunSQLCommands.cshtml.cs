using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

public class RunSQLCommandsModel : PageModel
{
    [BindProperty]
    public string SQLQuery { get; set; } = string.Empty;

    public DataTable Results { get; set; } = new DataTable();
    public string ErrorMessage { get; set; } = string.Empty;

    public IActionResult OnGet()
    {
        // Kullanıcı rolünü kontrol et
        var role = HttpContext.Session.GetInt32("UserRole");
        if (role != 1)
        {
            return Redirect("/AccessDenied");
        }

        return Page();
    }

    public void OnPost()
    {
        // Kullanıcı rolünü kontrol et
        var role = HttpContext.Session.GetInt32("UserRole");
        if (role != 1)
        {
            Response.Redirect("/AccessDenied");
            return;
        }

        if (string.IsNullOrWhiteSpace(SQLQuery))
        {
            ErrorMessage = "SQL query cannot be empty.";
            return;
        }

        string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;";

        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(SQLQuery, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(Results);
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            ErrorMessage = $"Syntax Error: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Unexpected Error: {ex.Message}";
        }
    }
}
