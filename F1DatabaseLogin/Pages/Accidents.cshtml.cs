using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class DriverAccidentDetailsModel : PageModel
{
    public List<DriverAccidentDetail> DriverAccidentDetails { get; set; } = new List<DriverAccidentDetail>();

    public void OnGet()
    {
        string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";
        string query = @"SELECT Driver_ID, Driver_Name, Car_ID, Car_Model, Race_ID, Race_Name, Date_of_Race, 
                         Accident_ID, Date_of_Accident, Accident_Description, Severity_Description 
                         FROM DriverAccidentDetails";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    DriverAccidentDetails.Add(new DriverAccidentDetail
                    {
                        DriverID = reader.GetInt32(0),                // int => int
                        DriverName = reader.GetString(1),            // varchar => string
                        CarID = reader.GetInt32(2),                  // int => int
                        CarModel = reader.GetString(3),              // varchar => string
                        RaceID = reader.GetInt32(4),                 // int => int
                        RaceName = reader.GetString(5),              // varchar => string
                        DateOfRace = reader.GetDateTime(6),          // date => DateTime
                        AccidentID = reader.GetInt32(7),             // int => int
                        DateOfAccident = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8), // date => DateTime?
                        AccidentDescription = reader.GetString(9),   // varchar => string
                        SeverityDescription = reader.GetString(10)   // varchar => string
                    });
                }
            }
        }
    }

    public class DriverAccidentDetail
    {
        public int DriverID { get; set; }
        public string DriverName { get; set; }
        public int CarID { get; set; }
        public string CarModel { get; set; }
        public int RaceID { get; set; }
        public string RaceName { get; set; }
        public DateTime DateOfRace { get; set; }
        public int AccidentID { get; set; }
        public DateTime? DateOfAccident { get; set; }
        public string AccidentDescription { get; set; }
        public string SeverityDescription { get; set; }
    }
}
