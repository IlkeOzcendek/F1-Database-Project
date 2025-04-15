using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

namespace F1DatabaseLogin.Pages
{
    public class RunPresetQueryModel : PageModel
    {
        public string QueryName { get; set; }
        public DataTable Results { get; set; } = new DataTable();
        public string ErrorMessage { get; set; }

        public void OnGet(int id)
        {
            string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Preset Query'yi Veritabanından Çek
                    string query = "SELECT QueryName, SQLQuery FROM PresetQueries WHERE QueryID = @QueryID";
                    string sqlQuery = string.Empty;

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@QueryID", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                QueryName = reader.GetString(0);
                                sqlQuery = reader.GetString(1);
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(sqlQuery))
                    {
                        ErrorMessage = "Query not found.";
                        return;
                    }

                    // Çekilen Query'yi Çalıştır
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(Results);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                ErrorMessage = $"SQL Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Unexpected Error: {ex.Message}";
            }
        }
    }
}
