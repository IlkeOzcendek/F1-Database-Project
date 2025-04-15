using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace F1Database.Pages
{
    public class TeamInsightsModel : PageModel
    {
        public List<Team> Teams { get; set; } = new List<Team>();

        public void OnGet()
        {
            string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";
            string query = "SELECT Team_ID, Team_Name, Team_Country, Established_Year FROM dbo.Teams";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Teams.Add(new Team
                        {
                            TeamID = reader.GetInt32(0),
                            TeamName = reader.GetString(1),
                            TeamCountry = reader.GetString(2),
                            EstablishedYear = reader.GetInt32(3)
                        });
                    }
                }
            }
        }
    }

    public class Team
    {
        public int TeamID { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string TeamCountry { get; set; } = string.Empty;
        public int EstablishedYear { get; set; }
    }
}
