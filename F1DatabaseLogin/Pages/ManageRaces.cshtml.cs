using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace F1DatabaseLogin.Pages
{
    public class ManageRacesModel : PageModel
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

        [BindProperty]
        public Race CurrentRace { get; set; }

        public List<Race> Races { get; set; } = new List<Race>();

        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {
            var role = HttpContext.Session.GetInt32("UserRole");
            if (role != 1)
            {
                Response.Redirect("/AccessDenied");
            }

            LoadRaces();
        }

        public IActionResult OnPostAdd()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Races (Race_Name, Date_of_Race, Laps) VALUES (@RaceName, @DateOfRace, @Laps)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RaceName", CurrentRace.Race_Name);
                command.Parameters.AddWithValue("@DateOfRace", CurrentRace.Date_of_Race);
                command.Parameters.AddWithValue("@Laps", CurrentRace.Laps);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Races WHERE Race_ID = @RaceID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RaceID", id);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Race_ID, Race_Name, Date_of_Race, Laps FROM Races WHERE Race_ID = @RaceID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RaceID", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        CurrentRace = new Race
                        {
                            Race_ID = reader.GetInt32(0),
                            Race_Name = reader.GetString(1),
                            Date_of_Race = reader.GetDateTime(2),
                            Laps = reader.GetInt32(3)
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
                string query = "UPDATE Races SET Race_Name = @RaceName, Date_of_Race = @DateOfRace, Laps = @Laps WHERE Race_ID = @RaceID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@RaceID", CurrentRace.Race_ID);
                command.Parameters.AddWithValue("@RaceName", CurrentRace.Race_Name);
                command.Parameters.AddWithValue("@DateOfRace", CurrentRace.Date_of_Race);
                command.Parameters.AddWithValue("@Laps", CurrentRace.Laps);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        private void LoadRaces()
        {
            Races.Clear();

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Race_ID, Race_Name, Date_of_Race, Laps FROM Races";
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
                            Date_of_Race = reader.GetDateTime(2),
                            Laps = reader.GetInt32(3)
                        });
                    }
                }
            }
        }
    }

    public class Race
    {
        public int Race_ID { get; set; }
        public string Race_Name { get; set; } = string.Empty;
        public DateTime Date_of_Race { get; set; }
        public int Laps { get; set; }
    }
}
