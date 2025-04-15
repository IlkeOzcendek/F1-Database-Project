using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace F1Database.Pages
{
    public class ManageDriverTeamDetailsModel : PageModel
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

        [BindProperty]
        public DriverTeamDetail CurrentDetail { get; set; }

        public List<DriverTeamDetail> DriverTeamDetails { get; set; } = new List<DriverTeamDetail>();

        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {
            var role = HttpContext.Session.GetInt32("UserRole");
if (role != 1) {
  Response.Redirect("/AccessDenied");
}

            LoadDriverTeamDetails();
        }

        public IActionResult OnPostAdd()
        {
            if (CurrentDetail.Driver_ID <= 0 || CurrentDetail.Team_ID <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid Driver or Team ID.");
                return Page();
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Team_Membership (Driver_ID, Team_ID) VALUES (@DriverID, @TeamID)";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DriverID", CurrentDetail.Driver_ID);
                    command.Parameters.AddWithValue("@TeamID", CurrentDetail.Team_ID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, $"Database error: {ex.Message}");
                return Page();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int driverId, int teamId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Team_Membership WHERE Driver_ID = @DriverID AND Team_ID = @TeamID";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DriverID", driverId);
                    command.Parameters.AddWithValue("@TeamID", teamId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, $"Database error: {ex.Message}");
                return Page();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int driverId, int teamId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT Driver_ID, Team_ID FROM Team_Membership WHERE Driver_ID = @DriverID AND Team_ID = @TeamID";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DriverID", driverId);
                    command.Parameters.AddWithValue("@TeamID", teamId);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CurrentDetail = new DriverTeamDetail
                            {
                                Driver_ID = reader.GetInt32(0),
                                Team_ID = reader.GetInt32(1)
                            };
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, $"Database error: {ex.Message}");
                return Page();
            }

            IsEditing = true;
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            if (CurrentDetail.Driver_ID <= 0 || CurrentDetail.Team_ID <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid Driver or Team ID.");
                return Page();
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Team_Membership SET Team_ID = @TeamID WHERE Driver_ID = @DriverID";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@DriverID", CurrentDetail.Driver_ID);
                    command.Parameters.AddWithValue("@TeamID", CurrentDetail.Team_ID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, $"Database error: {ex.Message}");
                return Page();
            }

            return RedirectToPage();
        }

        private void LoadDriverTeamDetails()
        {
            DriverTeamDetails.Clear();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT D.Driver_ID, CONCAT(D.First_Name, ' ', COALESCE(D.Middle_Name + ' ', '') + D.Last_Name) AS Driver_Name, 
                               T.Team_ID, T.Team_Name 
                        FROM Team_Membership TM
                        JOIN Drivers D ON TM.Driver_ID = D.Driver_ID
                        JOIN Teams T ON TM.Team_ID = T.Team_ID";

                    var command = new SqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DriverTeamDetails.Add(new DriverTeamDetail
                            {
                                Driver_ID = reader.GetInt32(0),
                                Driver_Name = reader.GetString(1),
                                Team_ID = reader.GetInt32(2),
                                Team_Name = reader.GetString(3)
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
    }

    public class DriverTeamDetail
    {
        public int Driver_ID { get; set; }
        public string Driver_Name { get; set; } = string.Empty;
        public int Team_ID { get; set; }
        public string Team_Name { get; set; } = string.Empty;
    }
}
