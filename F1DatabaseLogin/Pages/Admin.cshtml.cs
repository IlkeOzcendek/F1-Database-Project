using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace F1DatabaseLogin.Pages
{
    public class AdminModel : PageModel
    {
        public int UserCount { get; set; }
        public int DriverCount { get; set; }
        public int RaceCount { get; set; }
        public int TeamCount { get; set; }

        public void OnGet()
        {
            
var role = HttpContext.Session.GetInt32("UserRole");
if (role != 1) {
  Response.Redirect("/AccessDenied");
}

            // Veritabanı bağlantı dizesi
            string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Users
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Users", connection))
                {
                    UserCount = (int)command.ExecuteScalar();
                }

                // Drivers
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Drivers", connection))
                {
                    DriverCount = (int)command.ExecuteScalar();
                }

                // Races
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Races", connection))
                {
                    RaceCount = (int)command.ExecuteScalar();
                }

                // Teams
                using (SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM Teams", connection))
                {
                    TeamCount = (int)command.ExecuteScalar();
                }
            }
        }
    }
}
