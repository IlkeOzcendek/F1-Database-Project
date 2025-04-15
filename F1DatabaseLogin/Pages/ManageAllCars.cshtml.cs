using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace F1DatabaseLogin.Pages
{
    public class ManageCarsModel : PageModel
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

        [BindProperty]
        public Car CurrentCar { get; set; }

        public List<Car> Cars { get; set; } = new List<Car>();

        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {
            var role = HttpContext.Session.GetInt32("UserRole");
if (role != 1) {
  Response.Redirect("/AccessDenied");
}

            LoadCars();
        }

        public IActionResult OnPostAdd()
        {
            if (string.IsNullOrWhiteSpace(CurrentCar.Model) || string.IsNullOrWhiteSpace(CurrentCar.Engine_Type) || CurrentCar.Weight <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid car details.");
                return Page();
            }

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Cars (Model, Engine_Type, Weight) VALUES (@Model, @EngineType, @Weight)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Model", CurrentCar.Model);
                command.Parameters.AddWithValue("@EngineType", CurrentCar.Engine_Type);
                command.Parameters.AddWithValue("@Weight", CurrentCar.Weight);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Cars WHERE Car_ID = @CarID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CarID", id);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Car_ID, Model, Engine_Type, Weight FROM Cars WHERE Car_ID = @CarID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CarID", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        CurrentCar = new Car
                        {
                            Car_ID = reader.GetInt32(0),
                            Model = reader.GetString(1),
                            Engine_Type = reader.GetString(2),
                            Weight = reader.GetDecimal(3)
                        };
                    }
                }
            }

            IsEditing = true;
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            if (string.IsNullOrWhiteSpace(CurrentCar.Model) || string.IsNullOrWhiteSpace(CurrentCar.Engine_Type) || CurrentCar.Weight <= 0)
            {
                ModelState.AddModelError(string.Empty, "Invalid car details.");
                return Page();
            }

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Cars SET Model = @Model, Engine_Type = @EngineType, Weight = @Weight WHERE Car_ID = @CarID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CarID", CurrentCar.Car_ID);
                command.Parameters.AddWithValue("@Model", CurrentCar.Model);
                command.Parameters.AddWithValue("@EngineType", CurrentCar.Engine_Type);
                command.Parameters.AddWithValue("@Weight", CurrentCar.Weight);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        private void LoadCars()
        {
            Cars.Clear();

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Car_ID, Model, Engine_Type, Weight FROM Cars";
                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cars.Add(new Car
                        {
                            Car_ID = reader.GetInt32(0),
                            Model = reader.GetString(1),
                            Engine_Type = reader.GetString(2),
                            Weight = reader.GetDecimal(3)
                        });
                    }
                }
            }
        }
    }

    public class Car
    {
        public int Car_ID { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Engine_Type { get; set; } = string.Empty;
        public decimal Weight { get; set; }
    }
}
