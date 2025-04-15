using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace F1DatabaseLogin.Pages
{
    public class ManageTeamsModel : PageModel
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

        [BindProperty]
        public Team CurrentTeam { get; set; }

        public List<Team> Teams { get; set; } = new List<Team>();

        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {
            var role = HttpContext.Session.GetInt32("UserRole");
if (role != 1) {
  Response.Redirect("/AccessDenied");
}

            LoadTeams();
        }

        public IActionResult OnPostAdd()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO [dbo].[Teams] (Team_Name, Team_Country, Established_Year) VALUES (@TeamName, @TeamCountry, @EstablishedYear)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@TeamName", CurrentTeam.Team_Name);
                command.Parameters.AddWithValue("@TeamCountry", CurrentTeam.Team_Country);
                command.Parameters.AddWithValue("@EstablishedYear", CurrentTeam.Established_Year);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM [dbo].[Teams] WHERE Team_ID = @Id";
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
                string query = "SELECT Team_ID, Team_Name, Team_Country, Established_Year FROM [dbo].[Teams] WHERE Team_ID = @Id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        CurrentTeam = new Team
                        {
                            Team_ID = reader.GetInt32(0),
                            Team_Name = reader.GetString(1),
                            Team_Country = reader.GetString(2),
                            Established_Year = reader.GetInt32(3)
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
                string query = "UPDATE [dbo].[Teams] SET Team_Name = @TeamName, Team_Country = @TeamCountry, Established_Year = @EstablishedYear WHERE Team_ID = @Id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", CurrentTeam.Team_ID);
                command.Parameters.AddWithValue("@TeamName", CurrentTeam.Team_Name);
                command.Parameters.AddWithValue("@TeamCountry", CurrentTeam.Team_Country);
                command.Parameters.AddWithValue("@EstablishedYear", CurrentTeam.Established_Year);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        private void LoadTeams()
        {
            Teams.Clear();

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Team_ID, Team_Name, Team_Country, Established_Year FROM [dbo].[Teams]";
                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Teams.Add(new Team
                        {
                            Team_ID = reader.GetInt32(0),
                            Team_Name = reader.GetString(1),
                            Team_Country = reader.GetString(2),
                            Established_Year = reader.GetInt32(3)
                        });
                    }
                }
            }
        }
    }

    public class Team
    {
        public int Team_ID { get; set; }
        public string Team_Name { get; set; } = string.Empty;
        public string Team_Country { get; set; } = string.Empty;
        public int Established_Year { get; set; }
    }
}