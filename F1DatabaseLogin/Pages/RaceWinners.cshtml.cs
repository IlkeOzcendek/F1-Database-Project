using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class RaceResultsModel : PageModel
{
    public List<RaceWinner> RaceWinners { get; set; } = new List<RaceWinner>();

    public void OnGet()
    {
        string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";
        string query = "SELECT Race_Name, Date_of_Race, Driver_Name, Team_Name, Position_Finished, Points FROM RaceWinners";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    RaceWinners.Add(new RaceWinner
                    {
                        RaceName = reader.GetString(0),       
                        Date = reader.GetDateTime(1),            
                        DriverName = reader.GetString(2),     
                        TeamName = reader.GetString(3),          
                        Position = reader.IsDBNull(4) ? 0 : int.Parse(reader.GetString(4)),
                        Points = reader.GetDecimal(5)            
                    });
                }
            }
        }
    }

    public class RaceWinner
    {
        public string RaceName { get; set; }
        public DateTime Date { get; set; }
        public string DriverName { get; set; }
        public string TeamName { get; set; }
        public int Position { get; set; }
        public decimal Points { get; set; }
    }
}
