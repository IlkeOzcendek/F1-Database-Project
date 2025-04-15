using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace F1DatabaseLogin.Pages
{
    public class ManageDriversModel : PageModel
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

        [BindProperty]
        public Driver CurrentDriver { get; set; }

        public List<Driver> Drivers { get; set; } = new List<Driver>();

        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {

            var role = HttpContext.Session.GetInt32("UserRole");
if (role != 1) {
  Response.Redirect("/AccessDenied");
}

            LoadDrivers();
        }

        public IActionResult OnPostAdd()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO [dbo].[Drivers] (First_Name, Middle_Name, Last_Name, Nationality, Date_of_Birth, Date_of_Death) VALUES (@FirstName, @MiddleName, @LastName, @Nationality, @DateOfBirth, @DateOfDeath)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@FirstName", CurrentDriver.First_Name);
                command.Parameters.AddWithValue("@MiddleName", (object)CurrentDriver.Middle_Name ?? DBNull.Value);
                command.Parameters.AddWithValue("@LastName", CurrentDriver.Last_Name);
                command.Parameters.AddWithValue("@Nationality", CurrentDriver.Nationality);
                command.Parameters.AddWithValue("@DateOfBirth", CurrentDriver.Date_of_Birth);
                command.Parameters.AddWithValue("@DateOfDeath", (object)CurrentDriver.Date_of_Death ?? DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM [dbo].[Drivers] WHERE Driver_ID = @Id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Driver_ID, First_Name, Middle_Name, Last_Name, Nationality, Date_of_Birth, Date_of_Death FROM [dbo].[Drivers] WHERE Driver_ID = @Id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        CurrentDriver = new Driver
                        {
                            Driver_ID = reader.GetInt32(0),
                            First_Name = reader.GetString(1),
                            Middle_Name = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Last_Name = reader.GetString(3),
                            Nationality = reader.GetString(4),
                            Date_of_Birth = reader.GetDateTime(5),
                            Date_of_Death = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                        };
                    }
                }
            }

            IsEditing = true;
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE [dbo].[Drivers] SET First_Name = @FirstName, Middle_Name = @MiddleName, Last_Name = @LastName, Nationality = @Nationality, Date_of_Birth = @DateOfBirth, Date_of_Death = @DateOfDeath WHERE Driver_ID = @Id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", CurrentDriver.Driver_ID);
                command.Parameters.AddWithValue("@FirstName", CurrentDriver.First_Name);
                command.Parameters.AddWithValue("@MiddleName", (object)CurrentDriver.Middle_Name ?? DBNull.Value);
                command.Parameters.AddWithValue("@LastName", CurrentDriver.Last_Name);
                command.Parameters.AddWithValue("@Nationality", CurrentDriver.Nationality);
                command.Parameters.AddWithValue("@DateOfBirth", CurrentDriver.Date_of_Birth);
                command.Parameters.AddWithValue("@DateOfDeath", (object)CurrentDriver.Date_of_Death ?? DBNull.Value);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        private void LoadDrivers()
        {
            Drivers.Clear();

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Driver_ID, First_Name, Middle_Name, Last_Name, Nationality, Date_of_Birth, Date_of_Death FROM [dbo].[Drivers]";
                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Drivers.Add(new Driver
                        {
                            Driver_ID = reader.GetInt32(0),
                            First_Name = reader.GetString(1),
                            Middle_Name = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Last_Name = reader.GetString(3),
                            Nationality = reader.GetString(4),
                            Date_of_Birth = reader.GetDateTime(5),
                            Date_of_Death = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6)
                        });
                    }
                }
            }
        }
    }

    public class Driver
    {
        public int Driver_ID { get; set; }
        public string First_Name { get; set; } = string.Empty;
        public string? Middle_Name { get; set; }
        public string Last_Name { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public DateTime Date_of_Birth { get; set; }
        public DateTime? Date_of_Death { get; set; }
    }
}