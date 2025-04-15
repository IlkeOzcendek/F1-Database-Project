using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

public class DriverTeamDetailsModel : PageModel
{
    public List<DriverTeamDetail> DriverTeamDetails { get; set; } = new List<DriverTeamDetail>();

    public void OnGet()
    {
        string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";
        string query = @"SELECT Driver_ID, Driver_Name, Team_ID, Team_Name, Team_Country, Established_Year 
                         FROM DriverTeamDetails";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    DriverTeamDetails.Add(new DriverTeamDetail
                    {
                        DriverID = reader.GetInt32(0),         
                        DriverName = reader.GetString(1),     
                        TeamID = reader.GetInt32(2),         
                        TeamName = reader.GetString(3),        
                        TeamCountry = reader.GetString(4),     
                        EstablishedYear = reader.GetInt32(5)   
                    });
                }
            }
        }
    }

    public class DriverTeamDetail
    {
        public int DriverID { get; set; }
        public string DriverName { get; set; }
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public string TeamCountry { get; set; }
        public int EstablishedYear { get; set; }
    }
}
