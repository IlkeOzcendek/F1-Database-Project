using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class DriverCarRaceDetailsModel : PageModel
{
    public List<DriverCarRaceDetail> DriverCarRaceDetails { get; set; } = new List<DriverCarRaceDetail>();

    [BindProperty(SupportsGet = true)]
    public int? RaceIDFilter { get; set; }

    public void OnGet()
    {
        string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";
        string query = @"SELECT Driver_ID, Driver_Name, Car_ID, Car_Model, Race_ID, Race_Name, Date_of_Race, 
                         Position_Finished, Points FROM DriverCarRaceDetails";

        if (RaceIDFilter.HasValue)
        {
            query += " WHERE Race_ID = @RaceID";
        }

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);

            if (RaceIDFilter.HasValue)
            {
                command.Parameters.AddWithValue("@RaceID", RaceIDFilter.Value);
            }

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    DriverCarRaceDetails.Add(new DriverCarRaceDetail
                    {
                        DriverID = reader.GetInt32(0),                 
                        DriverName = reader.GetString(1),             
                        CarID = reader.GetInt32(2),                   
                        CarModel = reader.GetString(3),               
                        RaceID = reader.GetInt32(4),                
                        RaceName = reader.GetString(5),              
                        DateOfRace = reader.GetDateTime(6),           
                        PositionFinished = reader.GetString(7),   
                        Points = reader.GetDecimal(8)                 
                    });
                }
            }
        }
    }

    public class DriverCarRaceDetail
    {
        public int DriverID { get; set; }
        public string DriverName { get; set; }
        public int CarID { get; set; }
        public string CarModel { get; set; }
        public int RaceID { get; set; }
        public string RaceName { get; set; }
        public DateTime DateOfRace { get; set; }
        public string PositionFinished { get; set; }
        public decimal Points { get; set; }
    }
}
