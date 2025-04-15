using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace F1Database.Pages
{
    public class ManageCarOwnershipModel : PageModel
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

        [BindProperty]
        public CarOwnershipDetail CurrentDetail { get; set; }

        public List<CarOwnershipDetail> CarOwnershipDetails { get; set; } = new List<CarOwnershipDetail>();

        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {
            var role = HttpContext.Session.GetInt32("UserRole");
if (role != 1) {
  Response.Redirect("/AccessDenied");
}

            LoadCarOwnershipDetails();
        }

        public IActionResult OnPostAdd()
        {
            if (CurrentDetail.Car_ID <= 0 || CurrentDetail.Team_ID <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid Car or Team ID.");
                return Page();
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO Car_Ownership (Car_ID, Team_ID) VALUES (@CarID, @TeamID)";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CarID", CurrentDetail.Car_ID);
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

        public IActionResult OnPostDelete(int carId, int teamId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Car_Ownership WHERE Car_ID = @CarID AND Team_ID = @TeamID";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CarID", carId);
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

        public IActionResult OnPostEdit(int carId, int teamId)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT Car_ID, Team_ID FROM Car_Ownership WHERE Car_ID = @CarID AND Team_ID = @TeamID";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CarID", carId);
                    command.Parameters.AddWithValue("@TeamID", teamId);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CurrentDetail = new CarOwnershipDetail
                            {
                                Car_ID = reader.GetInt32(0),
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
            if (CurrentDetail.Car_ID <= 0 || CurrentDetail.Team_ID <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid Car or Team ID.");
                return Page();
            }

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Car_Ownership SET Team_ID = @TeamID WHERE Car_ID = @CarID";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CarID", CurrentDetail.Car_ID);
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

        private void LoadCarOwnershipDetails()
        {
            CarOwnershipDetails.Clear();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT C.Car_ID, C.Model AS Car_Model, C.Engine_Type, C.Weight, 
                               T.Team_ID, T.Team_Name 
                        FROM Car_Ownership CO
                        JOIN Cars C ON CO.Car_ID = C.Car_ID
                        JOIN Teams T ON CO.Team_ID = T.Team_ID";

                    var command = new SqlCommand(query, connection);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CarOwnershipDetails.Add(new CarOwnershipDetail
                            {
                                Car_ID = reader.GetInt32(0),
                                Car_Model = reader.GetString(1),
                                Engine_Type = reader.GetString(2),
                                Weight = reader.GetDecimal(3),
                                Team_ID = reader.GetInt32(4),
                                Team_Name = reader.GetString(5)
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

    public class CarOwnershipDetail
    {
        public int Car_ID { get; set; }
        public string Car_Model { get; set; } = string.Empty;
        public string Engine_Type { get; set; } = string.Empty;
        public decimal Weight { get; set; }
        public int Team_ID { get; set; }
        public string Team_Name { get; set; } = string.Empty;
    }
}
