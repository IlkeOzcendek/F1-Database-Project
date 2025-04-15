using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Data.SqlClient;

public class AddUserModel : PageModel
{
    private readonly string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

    [BindProperty]
public string Email { get; set; } = string.Empty;

[BindProperty]
public string Password { get; set; } = string.Empty;


    [BindProperty]
    public int Role { get; set; }

    public List<User> Users { get; set; } = new List<User>();

    public void OnGet()
    {
        Users = GetUsers();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        AddUser(new User
        {
            Email = Email,
            Password = Password,
            Role = Role
        });

        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        DeleteUser(id);
        return RedirectToPage();
    }

   private void AddUser(User user)
{
    string connectionString = "Server=127.0.0.1;Database=Formula1;User Id=sa;Password=MyP@ssw0rd123!;TrustServerCertificate=True;";

    try
    {
        using (var connection = new SqlConnection(connectionString))
        {
            // Veritabanı bağlantısını test et
            connection.Open();
            Console.WriteLine("Database connection successful.");

            // Kullanıcı ekleme sorgusu
            string query = "INSERT INTO [dbo].[Users] (Email, Password, Role) VALUES (@Email, @Password, @Role)";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Password", user.Password);
            command.Parameters.AddWithValue("@Role", user.Role);

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine($"User added successfully. Rows affected: {rowsAffected}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database connection or query execution failed: {ex.Message}");
    }
}


    private void DeleteUser(int id)
    {
        using (var connection = new SqlConnection(connectionString))
        {
            string query = "DELETE FROM [dbo].[Users] WHERE Id = @Id";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    private List<User> GetUsers()
    {
        var users = new List<User>();

        using (var connection = new SqlConnection(connectionString))
        {
            string query = "SELECT Id, Email, Role FROM [dbo].[Users]";
            var command = new SqlCommand(query, connection);

            connection.Open();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(0),
                        Email = reader.GetString(1),
                        Role = reader.GetInt32(2)
                    });
                }
            }
        }

        return users;
    }
}

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int Role { get; set; } // 1: Admin, 2: User
}
