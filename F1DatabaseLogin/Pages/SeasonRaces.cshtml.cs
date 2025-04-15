using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;


public class RaceTrackSeasonDetailsModel : PageModel
{
    public List<RaceTrackSeasonDetail> RaceTrackSeasonDetails { get; set; } = new List<RaceTrackSeasonDetail>();

    [BindProperty(SupportsGet = true)]
    public int? SeasonYearFilter { get; set; }

    public void OnGet()
    {
        string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";
        string query = @"SELECT Race_Name, Date_of_Race, Path_Name, Path_Country, Location, Length, 
                         Turns, Direction, Season_Year, Season_Start_Date, Season_End_Date 
                         FROM RaceTrackSeasonDetails";

        if (SeasonYearFilter.HasValue)
        {
            query += " WHERE Season_Year = @SeasonYear";
        }

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);

            if (SeasonYearFilter.HasValue)
            {
                command.Parameters.AddWithValue("@SeasonYear", SeasonYearFilter.Value);
            }

            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    RaceTrackSeasonDetails.Add(new RaceTrackSeasonDetail
                    {
                        RaceName = reader.GetString(0),                // varchar => string
                        DateOfRace = reader.GetDateTime(1),           // date => DateTime
                        PathName = reader.GetString(2),               // varchar => string
                        PathCountry = reader.GetString(3),            // varchar => string
                        Location = reader.GetString(4),               // varchar => string
                        Length = reader.GetDecimal(5),                // decimal => decimal
                        Turns = reader.GetInt32(6),                   // int => int
                        Direction = reader.GetString(7),              // varchar => string
                        SeasonYear = reader.GetInt32(8),              // int => int
                        SeasonStartDate = reader.GetDateTime(9),      // date => DateTime
                        SeasonEndDate = reader.GetDateTime(10)        // date => DateTime
                    });
                }
            }
        }
    }

    public class RaceTrackSeasonDetail
    {
        public string RaceName { get; set; }
        public DateTime DateOfRace { get; set; }
        public string PathName { get; set; }
        public string PathCountry { get; set; }
        public string Location { get; set; }
        public decimal Length { get; set; }
        public int Turns { get; set; }
        public string Direction { get; set; }
        public int SeasonYear { get; set; }
        public DateTime SeasonStartDate { get; set; }
        public DateTime SeasonEndDate { get; set; }
    }
}
