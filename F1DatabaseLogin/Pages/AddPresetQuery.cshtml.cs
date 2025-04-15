using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace F1DatabaseLogin.Pages
{
    public class AddPresetQueryModel : PageModel
    {
        [BindProperty]
        public string QueryName { get; set; }

        [BindProperty]
        public string SQLQuery { get; set; }

        public string Message { get; set; }

        public void OnPost()
        {
            if (string.IsNullOrWhiteSpace(QueryName) || string.IsNullOrWhiteSpace(SQLQuery))
            {
                Message = "Query Name and SQL Query are required.";
                return;
            }

            string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO PresetQueries (QueryName, SQLQuery) VALUES (@QueryName, @SQLQuery)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@QueryName", QueryName);
                        cmd.Parameters.AddWithValue("@SQLQuery", SQLQuery);
                        cmd.ExecuteNonQuery();
                    }

                    Message = "Query successfully added!";
                }
            }
            catch (SqlException ex)
            {
                Message = $"SQL Error: {ex.Message}";
            }
        }
    }
}
