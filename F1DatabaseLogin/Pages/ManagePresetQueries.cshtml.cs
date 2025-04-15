using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace F1DatabaseLogin.Pages
{
    public class ManagePresetQueriesModel : PageModel
    {
        public List<PresetQuery> PresetQueries { get; set; } = new List<PresetQuery>();

        public void OnGet()
        {
            var role = HttpContext.Session.GetInt32("UserRole");
if (role != 1) {
  Response.Redirect("/AccessDenied");
}

            string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT QueryID, QueryName FROM PresetQueries";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                PresetQueries.Add(new PresetQuery
                                {
                                    QueryID = reader.GetInt32(0),
                                    QueryName = reader.GetString(1)
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

        public class PresetQuery
        {
            public int QueryID { get; set; }
            public string QueryName { get; set; }
        }
    }
}
