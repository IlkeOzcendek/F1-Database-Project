using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace F1DatabaseLogin.Pages
{
    public class ManageUsersModel : PageModel
    {
        private readonly string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

        [BindProperty]
        public User CurrentUser { get; set; }

        public List<User> Users { get; set; } = new List<User>();

        public bool IsEditing { get; set; } = false;

        public void OnGet()
        {
            var role = HttpContext.Session.GetInt32("UserRole");
if (role != 1) {
  Response.Redirect("/AccessDenied");
}

            LoadUsers();
        }

        public IActionResult OnPostAdd()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO [dbo].[Users] (Email, Password, Role) VALUES (@Email, @Password, @Role)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", CurrentUser.Email);
                command.Parameters.AddWithValue("@Password", CurrentUser.Password);
                command.Parameters.AddWithValue("@Role", CurrentUser.Role);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM [dbo].[Users] WHERE Id = @Id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEdit(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Email, Password, Role FROM [dbo].[Users] WHERE Id = @Id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        CurrentUser = new User
                        {
                            Id = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            Password = reader.GetString(2),
                            Role = reader.GetInt32(3)
                        };
                    }
                }
            }

            IsEditing = true;
            return Page();
        }

        public IActionResult OnPostUpdate()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE [dbo].[Users] SET Email = @Email, Password = @Password, Role = @Role WHERE Id = @Id";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Id", CurrentUser.Id);
                command.Parameters.AddWithValue("@Email", CurrentUser.Email);
                command.Parameters.AddWithValue("@Password", CurrentUser.Password);
                command.Parameters.AddWithValue("@Role", CurrentUser.Role);

                connection.Open();
                command.ExecuteNonQuery();
            }

            return RedirectToPage();
        }

        private void LoadUsers()
        {
            Users.Clear();

            using (var connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Id, Email, Role FROM [dbo].[Users]";
                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            Role = reader.GetInt32(2)
                        });
                    }
                }
            }
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty; // Varsayılan değer
public string? Password { get; set; } // Nullable işaretleme

        public int Role { get; set; } // 1: Admin, 2: User
    }
}
