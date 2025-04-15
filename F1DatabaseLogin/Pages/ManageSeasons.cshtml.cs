using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace F1DatabaseLogin.Pages
{
    public class ManageSeasonsModel : PageModel
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

        [BindProperty]
        public Season CurrentSeason { get; set; }

        public List<Season> Seasons { get; set; } = new List<Season>();

        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {
            var role = HttpContext.Session.GetInt32("UserRole");
if (role != 1) {
  Response.Redirect("/AccessDenied");
}

            LoadSeasons();
        }

        public IActionResult OnPostAdd()
        {
            if (CurrentSeason.Season_Year <= 0 || CurrentSeason.Start_Date == default || CurrentSeason.End_Date == default)
            {
                ModelState.AddModelError(string.Empty, "Invalid season details.");
                return Page();
            }

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Seasons (Season_Year, Start_Date, End_Date) VALUES (@SeasonYear, @StartDate, @EndDate)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SeasonYear", CurrentSeason.Season_Year);
                command.Parameters.AddWithValue("@StartDate", CurrentSeason.Start_Date);
                command.Parameters.AddWithValue("@EndDate", CurrentSeason.End_Date);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Seasons WHERE Season_ID = @SeasonID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SeasonID", id);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Season_ID, Season_Year, Start_Date, End_Date FROM Seasons WHERE Season_ID = @SeasonID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SeasonID", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        CurrentSeason = new Season
                        {
                            Season_ID = reader.GetInt32(0),
                            Season_Year = reader.GetInt32(1),
                            Start_Date = reader.GetDateTime(2),
                            End_Date = reader.GetDateTime(3)
                        };
                    }
                }
            }

            IsEditing = true;
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            if (CurrentSeason.Season_Year <= 0 || CurrentSeason.Start_Date == default || CurrentSeason.End_Date == default)
            {
                ModelState.AddModelError(string.Empty, "Invalid season details.");
                return Page();
            }

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Seasons SET Season_Year = @SeasonYear, Start_Date = @StartDate, End_Date = @EndDate WHERE Season_ID = @SeasonID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SeasonID", CurrentSeason.Season_ID);
                command.Parameters.AddWithValue("@SeasonYear", CurrentSeason.Season_Year);
                command.Parameters.AddWithValue("@StartDate", CurrentSeason.Start_Date);
                command.Parameters.AddWithValue("@EndDate", CurrentSeason.End_Date);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        private void LoadSeasons()
        {
            Seasons.Clear();

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Season_ID, Season_Year, Start_Date, End_Date FROM Seasons";
                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Seasons.Add(new Season
                        {
                            Season_ID = reader.GetInt32(0),
                            Season_Year = reader.GetInt32(1),
                            Start_Date = reader.GetDateTime(2),
                            End_Date = reader.GetDateTime(3)
                        });
                    }
                }
            }
        }
    }

    public class Season
    {
        public int Season_ID { get; set; }
        public int Season_Year { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
    }
}
