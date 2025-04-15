using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace F1DatabaseLogin.Pages
{
    public class RacePathsModel : PageModel
    {
        public List<RacePath> RacePaths { get; set; } = new List<RacePath>();

        public void OnGet()
        {
            string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT Path_ID, Path_Name, Path_Country, Location, Length, Turns, Direction FROM Race_Path ORDER BY Path_Name";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                RacePaths.Add(new RacePath
                                {
                                    PathID = reader.GetInt32(0),
                                    PathName = reader.GetString(1),
                                    PathCountry = reader.GetString(2),
                                    Location = reader.GetString(3),
                                    Length = reader.GetDecimal(4),
                                    Turns = reader.GetInt32(5),
                                    Direction = reader.GetString(6)
                                });
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error: {ex.Message}");
            }
        }

        public class RacePath
        {
            public int PathID { get; set; }
            public string PathName { get; set; }
            public string PathCountry { get; set; }
            public string Location { get; set; }
            public decimal Length { get; set; }
            public int Turns { get; set; }
            public string Direction { get; set; }
        }
    }
}
