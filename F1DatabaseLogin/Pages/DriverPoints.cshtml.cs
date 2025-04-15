using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class DriverPointsSummaryModel : PageModel
{
    public List<DriverPointsSummary> DriverPoints { get; set; } = new List<DriverPointsSummary>();

    public void OnGet()
    {
        string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";
        string query = @"SELECT DPS.Driver_ID, DPS.Full_Name, DPS.Total_Points, 
                                COALESCE(DC.Championships_Won, 0) AS Championships_Won
                         FROM Driver_Points_Summary DPS
                         LEFT JOIN Driver_Championships DC ON DPS.Driver_ID = DC.Driver_ID
                         ORDER BY DPS.Total_Points DESC";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    DriverPoints.Add(new DriverPointsSummary
                    {
                        DriverID = reader.GetInt32(0),                
                        FullName = reader.GetString(1),            
                        TotalPoints = reader.GetDecimal(2),      
                        ChampionshipsWon = reader.GetInt32(3)     
                    });
                }
            }
        }
    }

    public class DriverPointsSummary
    {
        public int DriverID { get; set; }
        public string FullName { get; set; }
        public decimal TotalPoints { get; set; }
        public int ChampionshipsWon { get; set; }
    }
}
