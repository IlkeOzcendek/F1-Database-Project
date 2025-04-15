using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

public class DriverDetailsModel : PageModel
{
    public List<DriverDetail> DriverDetails { get; set; } = new List<DriverDetail>();

    public void OnGet()
    {
        
        string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";
        string query = @"SELECT d.Driver_ID, d.First_Name + ' ' + COALESCE(d.Middle_Name + ' ', '') + d.Last_Name AS Full_Name, 
                                a.Age, d.Nationality, d.Date_of_Birth, d.Date_of_Death 
                         FROM Drivers d
                         LEFT JOIN Driver_Age a ON d.Driver_ID = a.Driver_ID
                         ORDER BY a.Age DESC";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    DriverDetails.Add(new DriverDetail
                    {
                        DriverID = reader.GetInt32(0),
                        FullName = reader.GetString(1),
                        Age = reader.IsDBNull(2) ? (decimal?)null : reader.GetDecimal(2),
                        Nationality = reader.GetString(3),
                        DateOfBirth = reader.GetDateTime(4),
                        DateOfDeath = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5)
                    });
                }
            }
        }
    }

    public class DriverDetail
    {
        public int DriverID { get; set; }
        public string FullName { get; set; }
        public decimal? Age { get; set; }
        public string Nationality { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime? DateOfDeath { get; set; }
    }
}
