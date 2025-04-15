using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace F1DatabaseLogin.Pages
{
    public class ManageDriverCarRaceDetailsModel : PageModel
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

        [BindProperty]
        public DriverCarRaceDetail CurrentDetail { get; set; } = new DriverCarRaceDetail();

        public List<DriverCarRaceDetail> DriverCarRaceDetails { get; set; } = new List<DriverCarRaceDetail>();

        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {
            var role = HttpContext.Session.GetInt32("UserRole");
            if (role != 1)
            {
                TempData["ErrorMessage"] = "Access Denied. You do not have sufficient permissions.";
                Response.Redirect("/AccessDenied");
            }

            LoadDriverCarRaceDetails();
        }

        public IActionResult OnPostAdd()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Race_Participation (Position_Finished, Points, Car_ID, Driver_ID, Team_ID, Race_ID)
                                     VALUES (@PositionFinished, @Points, @CarID, @DriverID, @TeamID, @RaceID);";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PositionFinished", CurrentDetail.Position_Finished);
                    command.Parameters.AddWithValue("@Points", CurrentDetail.Points ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CarID", CurrentDetail.Car_ID);
                    command.Parameters.AddWithValue("@DriverID", CurrentDetail.Driver_ID);
                    command.Parameters.AddWithValue("@TeamID", CurrentDetail.Team_ID);
                    command.Parameters.AddWithValue("@RaceID", CurrentDetail.Race_ID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Participation added successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error adding participation: " + ex.Message;
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Race_Participation WHERE Participation_ID = @ParticipationID";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ParticipationID", id);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Participation deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error deleting participation: " + ex.Message;
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
                    string query = "SELECT Participation_ID, Position_Finished, Points, Car_ID, Driver_ID, Team_ID, Race_ID FROM Race_Participation WHERE Participation_ID = @ParticipationID";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ParticipationID", id);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CurrentDetail = new DriverCarRaceDetail
                            {
                                Participation_ID = reader.GetInt32(0),
                                Position_Finished = reader.GetString(1),
                                Points = reader.IsDBNull(2) ? null : reader.GetDecimal(2),
                                Car_ID = reader.GetInt32(3),
                                Driver_ID = reader.GetInt32(4),
                                Team_ID = reader.GetInt32(5),
                                Race_ID = reader.GetInt32(6)
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
                TempData["SuccessMessage"] = "Participation loaded for editing.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading participation: " + ex.Message;
            }

            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = @"UPDATE Race_Participation SET
                                     Position_Finished = @PositionFinished,
                                     Points = @Points,
                                     Car_ID = @CarID,
                                     Driver_ID = @DriverID,
                                     Team_ID = @TeamID,
                                     Race_ID = @RaceID
                                     WHERE Participation_ID = @ParticipationID;";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@PositionFinished", CurrentDetail.Position_Finished);
                    command.Parameters.AddWithValue("@Points", CurrentDetail.Points ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CarID", CurrentDetail.Car_ID);
                    command.Parameters.AddWithValue("@DriverID", CurrentDetail.Driver_ID);
                    command.Parameters.AddWithValue("@TeamID", CurrentDetail.Team_ID);
                    command.Parameters.AddWithValue("@RaceID", CurrentDetail.Race_ID);
                    command.Parameters.AddWithValue("@ParticipationID", CurrentDetail.Participation_ID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Participation updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating participation: " + ex.Message;
            }

            return RedirectToPage();
        }

        private void LoadDriverCarRaceDetails()
        {
            DriverCarRaceDetails.Clear();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = @"SELECT RP.Participation_ID, D.Driver_ID, 
                                     CONCAT(D.First_Name, ' ', COALESCE(D.Middle_Name + ' ', '') + D.Last_Name) AS Driver_Name,
                                     C.Car_ID, C.Model AS Car_Model, R.Race_ID, R.Race_Name, R.Date_of_Race,
                                     RP.Team_ID, RP.Position_Finished, RP.Points
                                     FROM Race_Participation RP
                                     JOIN Drivers D ON RP.Driver_ID = D.Driver_ID
                                     JOIN Cars C ON RP.Car_ID = C.Car_ID
                                     JOIN Races R ON RP.Race_ID = R.Race_ID;";
                    var command = new SqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DriverCarRaceDetails.Add(new DriverCarRaceDetail
                            {
                                Participation_ID = reader.GetInt32(0),
                                Driver_ID = reader.GetInt32(1),
                                Driver_Name = reader.GetString(2),
                                Car_ID = reader.GetInt32(3),
                                Car_Model = reader.GetString(4),
                                Race_ID = reader.GetInt32(5),
                                Race_Name = reader.GetString(6),
                                Date_of_Race = reader.GetDateTime(7),
                                Team_ID = reader.GetInt32(8),
                                Position_Finished = reader.GetString(9),
                                Points = reader.IsDBNull(10) ? null : reader.GetDecimal(10)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading race participation details: " + ex.Message;
            }
        }
    }

    public class DriverCarRaceDetail
    {
        public int Participation_ID { get; set; }
        public int Driver_ID { get; set; }
        public string Driver_Name { get; set; } = string.Empty;
        public int Car_ID { get; set; }
        public string Car_Model { get; set; } = string.Empty;
        public int Race_ID { get; set; }
        public string Race_Name { get; set; } = string.Empty;
        public DateTime Date_of_Race { get; set; }
        public int Team_ID { get; set; }
        public string Position_Finished { get; set; } = string.Empty;
        public decimal? Points { get; set; }
    }
}
