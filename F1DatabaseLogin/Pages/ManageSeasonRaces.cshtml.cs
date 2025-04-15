using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace F1DatabaseLogin.Pages
{
    public class ManageRaceTrackSeasonDetailsModel : PageModel
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

        [BindProperty]
        public RaceTrackSeasonDetail CurrentDetail { get; set; } = new RaceTrackSeasonDetail();

        public List<RaceTrackSeasonDetail> RaceTrackSeasonDetails { get; set; } = new List<RaceTrackSeasonDetail>();

        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {
            var role = HttpContext.Session.GetInt32("UserRole");
            if (role != 1)
            {
                TempData["ErrorMessage"] = "Access Denied. You do not have sufficient permissions.";
                Response.Redirect("/AccessDenied");
            }

            LoadRaceTrackSeasonDetails();
        }

        public IActionResult OnPostAdd()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Race_Track (Race_ID, Path_ID) VALUES (@RaceID, @PathID); INSERT INTO Season_Inclusion (Race_ID, Season_ID) VALUES (@RaceID, @SeasonID);";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RaceID", CurrentDetail.Race_ID);
                    command.Parameters.AddWithValue("@PathID", CurrentDetail.Path_ID);
                    command.Parameters.AddWithValue("@SeasonID", CurrentDetail.Season_ID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Race track season detail added successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error adding race track season detail: " + ex.Message;
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Season_Inclusion WHERE Race_ID = @RaceID; DELETE FROM Race_Track WHERE Race_ID = @RaceID;";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RaceID", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Race track season detail deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting race track season detail: " + ex.Message;
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid ID for edit operation.";
                return RedirectToPage();
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT RT.Race_ID, RT.Path_ID, SI.Season_ID 
                                     FROM Race_Track RT 
                                     JOIN Race_Path P ON RT.Path_ID = P.Path_ID 
                                     JOIN Season_Inclusion SI ON RT.Race_ID = SI.Race_ID 
                                     JOIN Seasons S ON SI.Season_ID = S.Season_ID 
                                     WHERE RT.Race_ID = @RaceID;";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RaceID", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CurrentDetail = new RaceTrackSeasonDetail
                            {
                                Race_ID = reader.GetInt32(0),
                                Path_ID = reader.GetInt32(1),
                                Season_ID = reader.GetInt32(2)
                            };
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "No record found with the specified ID.";
                            return RedirectToPage();
                        }
                    }
                }

                IsEditing = true;
                TempData["SuccessMessage"] = "Race track season detail loaded for editing.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading race track season detail: " + ex.Message;
            }

            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Race_Track SET Path_ID = @PathID WHERE Race_ID = @RaceID; 
                                     UPDATE Season_Inclusion SET Season_ID = @SeasonID WHERE Race_ID = @RaceID;";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@RaceID", CurrentDetail.Race_ID);
                    command.Parameters.AddWithValue("@PathID", CurrentDetail.Path_ID);
                    command.Parameters.AddWithValue("@SeasonID", CurrentDetail.Season_ID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Race track season detail updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating race track season detail: " + ex.Message;
            }

            return RedirectToPage();
        }

        private void LoadRaceTrackSeasonDetails()
        {
            RaceTrackSeasonDetails.Clear();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT R.Race_ID, R.Race_Name, R.Date_of_Race, P.Path_ID, P.Path_Name, P.Path_Country, P.Location, 
                                     P.Length, P.Turns, P.Direction, S.Season_ID, S.Season_Year, S.Start_Date AS Season_Start_Date, 
                                     S.End_Date AS Season_End_Date 
                                     FROM Races R 
                                     JOIN Race_Track RT ON R.Race_ID = RT.Race_ID 
                                     JOIN Race_Path P ON RT.Path_ID = P.Path_ID 
                                     JOIN Season_Inclusion SI ON R.Race_ID = SI.Race_ID 
                                     JOIN Seasons S ON SI.Season_ID = S.Season_ID;";
                    var command = new SqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RaceTrackSeasonDetails.Add(new RaceTrackSeasonDetail
                            {
                                Race_ID = reader.GetInt32(0),
                                Race_Name = reader.GetString(1),
                                Date_of_Race = reader.GetDateTime(2),
                                Path_ID = reader.GetInt32(3),
                                Path_Name = reader.GetString(4),
                                Path_Country = reader.GetString(5),
                                Location = reader.GetString(6),
                                Length = reader.GetDecimal(7),
                                Turns = reader.GetInt32(8),
                                Direction = reader.GetString(9),
                                Season_ID = reader.GetInt32(10),
                                Season_Year = reader.GetInt32(11),
                                Season_Start_Date = reader.GetDateTime(12),
                                Season_End_Date = reader.GetDateTime(13)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading race track season details: " + ex.Message;
            }
        }
    }

    public class RaceTrackSeasonDetail
    {
        public int Race_ID { get; set; }
        public string Race_Name { get; set; } = string.Empty;
        public DateTime Date_of_Race { get; set; }
        public int Path_ID { get; set; }
        public string Path_Name { get; set; } = string.Empty;
        public string Path_Country { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal Length { get; set; }
        public int Turns { get; set; }
        public string Direction { get; set; } = string.Empty;
        public int Season_ID { get; set; }
        public int Season_Year { get; set; }
        public DateTime Season_Start_Date { get; set; }
        public DateTime Season_End_Date { get; set; }
    }
}
