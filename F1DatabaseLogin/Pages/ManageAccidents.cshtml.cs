using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace F1Database.Pages
{
    public class ManageAccidentsModel : PageModel
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

        [BindProperty]
        public Accident CurrentAccident { get; set; } = new Accident();

        public List<Accident> Accidents { get; set; } = new List<Accident>();
        public List<Race> Races { get; set; } = new List<Race>();

        public void OnGet()
        {
            LoadAccidents();
            LoadRaces();
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid || CurrentAccident.Race_ID <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid input data. Please check your form.");
                return Page();
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Accidents (Race_ID, Date_of_Accident, Accident_Description, Severity_Description) VALUES (@RaceID, @DateOfAccident, @Description, @Severity)";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RaceID", CurrentAccident.Race_ID);
                    command.Parameters.AddWithValue("@DateOfAccident", CurrentAccident.Date_of_Accident);
                    command.Parameters.AddWithValue("@Description", CurrentAccident.Accident_Description ?? string.Empty);
                    command.Parameters.AddWithValue("@Severity", CurrentAccident.Severity_Description);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, $"Database error: {ex.Message}");
            }

            return RedirectToPage();
        }

        private void LoadAccidents()
        {
            Accidents.Clear();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT A.Date_of_Accident, A.Accident_Description, A.Severity_Description, R.Race_Name FROM Accidents A JOIN Races R ON A.Race_ID = R.Race_ID";
                    var command = new SqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Accidents.Add(new Accident
                            {
                                Date_of_Accident = reader.GetDateTime(0),
                                Accident_Description = reader.GetString(1),
                                Severity_Description = reader.GetString(2),
                                Race_Name = reader.GetString(3)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, $"Database error: {ex.Message}");
            }
        }

        private void LoadRaces()
        {
            Races.Clear();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT Race_ID, Race_Name, Date_of_Race FROM Races";
                    var command = new SqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Races.Add(new Race
                            {
                                Race_ID = reader.GetInt32(0),
                                Race_Name = reader.GetString(1),
                                Date_of_Race = reader.GetDateTime(2)
                            });
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, $"Database error: {ex.Message}");
            }
        }

        public class Accident
        {
            public int Race_ID { get; set; }
            public DateTime Date_of_Accident { get; set; }
            public string? Accident_Description { get; set; }
            public string Severity_Description { get; set; } = string.Empty;
            public string? Race_Name { get; set; }
        }

        public class Race
        {
            public int Race_ID { get; set; }
            public string Race_Name { get; set; } = string.Empty;
            public DateTime Date_of_Race { get; set; }
        }
    }
}
