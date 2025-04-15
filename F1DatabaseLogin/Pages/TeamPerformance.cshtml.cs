using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class TeamPerformanceModel : PageModel
{
    public List<TeamPerformance> TeamPerformances { get; set; } = new List<TeamPerformance>();

    public void OnGet()
    {
        string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";
        string query = "SELECT Team_ID, Team_Name, Total_Team_Points FROM Team_Performance ORDER BY Total_Team_Points DESC";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    TeamPerformances.Add(new TeamPerformance
                    {
                        TeamID = reader.GetInt32(0),               
                        TeamName = reader.GetString(1),          
                        TotalTeamPoints = reader.GetDecimal(2)    
                    });
                }
            }
        }
    }

    public class TeamPerformance
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public decimal TotalTeamPoints { get; set; }
    }
}
