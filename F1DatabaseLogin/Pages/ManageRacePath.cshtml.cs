using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace F1DatabaseLogin.Pages
{
    public class ManageRacePathsModel : PageModel
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

        [BindProperty]
        public RacePath CurrentRacePath { get; set; }

        public List<RacePath> RacePaths { get; set; } = new List<RacePath>();

        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {
            var role = HttpContext.Session.GetInt32("UserRole");
if (role != 1) {
  Response.Redirect("/AccessDenied");
}

            LoadRacePaths();
        }

        public IActionResult OnPostAdd()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Race_Path (Path_Name, Path_Country, Location, Length, Turns, Direction) VALUES (@PathName, @PathCountry, @Location, @Length, @Turns, @Direction)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PathName", CurrentRacePath.Path_Name);
                command.Parameters.AddWithValue("@PathCountry", CurrentRacePath.Path_Country);
                command.Parameters.AddWithValue("@Location", CurrentRacePath.Location);
                command.Parameters.AddWithValue("@Length", CurrentRacePath.Length);
                command.Parameters.AddWithValue("@Turns", CurrentRacePath.Turns);
                command.Parameters.AddWithValue("@Direction", CurrentRacePath.Direction);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Race_Path WHERE Path_ID = @PathID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PathID", id);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Path_ID, Path_Name, Path_Country, Location, Length, Turns, Direction FROM Race_Path WHERE Path_ID = @PathID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PathID", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        CurrentRacePath = new RacePath
                        {
                            Path_ID = reader.GetInt32(0),
                            Path_Name = reader.GetString(1),
                            Path_Country = reader.GetString(2),
                            Location = reader.GetString(3),
                            Length = reader.GetDecimal(4),
                            Turns = reader.GetInt32(5),
                            Direction = reader.GetString(6)
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
                string query = "UPDATE Race_Path SET Path_Name = @PathName, Path_Country = @PathCountry, Location = @Location, Length = @Length, Turns = @Turns, Direction = @Direction WHERE Path_ID = @PathID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PathID", CurrentRacePath.Path_ID);
                command.Parameters.AddWithValue("@PathName", CurrentRacePath.Path_Name);
                command.Parameters.AddWithValue("@PathCountry", CurrentRacePath.Path_Country);
                command.Parameters.AddWithValue("@Location", CurrentRacePath.Location);
                command.Parameters.AddWithValue("@Length", CurrentRacePath.Length);
                command.Parameters.AddWithValue("@Turns", CurrentRacePath.Turns);
                command.Parameters.AddWithValue("@Direction", CurrentRacePath.Direction);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        private void LoadRacePaths()
        {
            RacePaths.Clear();

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Path_ID, Path_Name, Path_Country, Location, Length, Turns, Direction FROM Race_Path";
                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RacePaths.Add(new RacePath
                        {
                            Path_ID = reader.GetInt32(0),
                            Path_Name = reader.GetString(1),
                            Path_Country = reader.GetString(2),
                            Location = reader.GetString(3),
                            Length = reader.GetDecimal(4),
                            Turns = reader.GetInt32(5),
                            Direction = reader.GetString(6)
                        });
                    }
                }
            }
        }
    }

    public class RacePath
    {
        public int Path_ID { get; set; }
        public string Path_Name { get; set; } = string.Empty;
        public string Path_Country { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Length { get; set; }
        public int Turns { get; set; }
        public string Direction { get; set; } = string.Empty;
    }
}