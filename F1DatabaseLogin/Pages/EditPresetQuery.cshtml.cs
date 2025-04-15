using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace F1DatabaseLogin.Pages
{
    public class EditPresetQueryModel : PageModel
    {
        [BindProperty]
        public int QueryID { get; set; }

        [BindProperty]
        public string QueryName { get; set; }

        [BindProperty]
        public string SQLQuery { get; set; }

        public string Message { get; set; }

        public void OnGet(int id)
        {
            Console.WriteLine($"Fetching QueryID: {id}");

            string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT QueryName, SQLQuery FROM PresetQueries WHERE QueryID = @QueryID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@QueryID", id);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                QueryName = reader.GetString(0);
                                SQLQuery = reader.GetString(1);
                                QueryID = id;

                                Console.WriteLine($"QueryName: {QueryName}, SQLQuery: {SQLQuery}");
                            }
                            else
                            {
                                Message = "Query not found.";
                                Console.WriteLine(Message);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Message = $"SQL Error: {ex.Message}";
                Console.WriteLine(Message);
            }
        }

        public IActionResult OnPost()
        {
            Console.WriteLine($"Updating QueryID: {QueryID}, QueryName: {QueryName}, SQLQuery: {SQLQuery}");

            if (string.IsNullOrWhiteSpace(QueryName) || string.IsNullOrWhiteSpace(SQLQuery))
            {
                Message = "Query Name and SQL Query cannot be empty.";
                return Page();
            }

            string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "UPDATE PresetQueries SET QueryName = @QueryName, SQLQuery = @SQLQuery WHERE QueryID = @QueryID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@QueryName", QueryName);
                        cmd.Parameters.AddWithValue("@SQLQuery", SQLQuery);
                        cmd.Parameters.AddWithValue("@QueryID", QueryID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Query updated successfully.");
                            return RedirectToPage("/ManagePresetQueries");
                        }
                        else
                        {
                            Message = "No rows affected. Update failed.";
                            Console.WriteLine(Message);
                            return Page();
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Message = $"SQL Error: {ex.Message}";
                Console.WriteLine(Message);
                return Page();
            }
        }
    }
}
