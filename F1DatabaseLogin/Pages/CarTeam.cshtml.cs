using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class CarTeamDetailsModel : PageModel
{
    public List<CarTeamDetail> CarTeamDetails { get; set; } = new List<CarTeamDetail>();

    public void OnGet()
    {
        string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";
        string query = @"SELECT Car_ID, Car_Model, Engine_Type, Weight, Team_ID, Team_Name 
                         FROM CarTeamDetails";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    CarTeamDetails.Add(new CarTeamDetail
                    {
                        CarID = reader.GetInt32(0),           // int => int
                        CarModel = reader.GetString(1),      // varchar => string
                        EngineType = reader.GetString(2),    // varchar => string
                        Weight = reader.GetDecimal(3),       // decimal => decimal
                        TeamID = reader.GetInt32(4),         // int => int
                        TeamName = reader.GetString(5)       // varchar => string
                    });
                }
            }
        }
    }

    public class CarTeamDetail
    {
        public int CarID { get; set; }
        public string CarModel { get; set; }
        public string EngineType { get; set; }
        public decimal Weight { get; set; }
        public int TeamID { get; set; }
        public string TeamName { get; set; }
    }
}
