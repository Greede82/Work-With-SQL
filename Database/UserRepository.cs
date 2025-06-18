using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Database
{
    public class UserRepository
    {
        private DatabaseConnection dbConnection;

        public UserRepository()
        {
            dbConnection = DatabaseConnection.Instance;
            EnsureUserTableExists();
        }

        private void EnsureUserTableExists()
        {
            try
            {
                string scriptPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", "CreateDatabase.sql");
                
                if (!System.IO.File.Exists(scriptPath))
                {
                    scriptPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\Database", "CreateDatabase.sql");
                }

                if (System.IO.File.Exists(scriptPath))
                {
                    dbConnection.ExecuteSqlScript(scriptPath);
                }
                else
                {
                    string hashedPassword = HashPassword("admin");
                    string checkTableQuery = @"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Users')
                        BEGIN
                            CREATE TABLE Users (
                                UserId INT IDENTITY(1,1) PRIMARY KEY,
                                Username NVARCHAR(50) NOT NULL UNIQUE,
                                Password NVARCHAR(100) NOT NULL,
                                FullName NVARCHAR(100) NOT NULL,
                                Role INT NOT NULL,
                                CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
                                IsActive BIT NOT NULL DEFAULT 1
                            );

                            -- Создаем администратора по умолчанию
                            INSERT INTO Users (Username, Password, FullName, Role, CreatedDate, IsActive)
                            VALUES ('admin', '" + hashedPassword + @"', 'Администратор системы', 1, GETDATE(), 1);
                        END";

                    dbConnection.ExecuteNonQuery(checkTableQuery);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при инициализации базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            try
            {
                string query = "SELECT UserId, Username, Password, FullName, Role, CreatedDate, IsActive FROM Users";
                DataTable dataTable = dbConnection.ExecuteQuery(query);

                foreach (DataRow row in dataTable.Rows)
                {
                    User user = new User
                    {
                        UserId = Convert.ToInt32(row["UserId"]),
                        Username = row["Username"].ToString(),
                        Password = row["Password"].ToString(),
                        FullName = row["FullName"].ToString(),
                        Role = (UserRole)Convert.ToInt32(row["Role"]),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        IsActive = Convert.ToBoolean(row["IsActive"])
                    };
                    users.Add(user);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении списка пользователей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return users;
        }

        public User GetUserById(int userId)
        {
            User user = null;
            try
            {
                string query = $"SELECT UserId, Username, Password, FullName, Role, CreatedDate, IsActive FROM Users WHERE UserId = {userId}";
                DataTable dataTable = dbConnection.ExecuteQuery(query);

                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    user = new User
                    {
                        UserId = Convert.ToInt32(row["UserId"]),
                        Username = row["Username"].ToString(),
                        Password = row["Password"].ToString(),
                        FullName = row["FullName"].ToString(),
                        Role = (UserRole)Convert.ToInt32(row["Role"]),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        IsActive = Convert.ToBoolean(row["IsActive"])
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return user;
        }

        public User GetUserByUsername(string username)
        {
            User user = null;
            try
            {
                string query = $"SELECT UserId, Username, Password, FullName, Role, CreatedDate, IsActive FROM Users WHERE Username = '{username}'";
                DataTable dataTable = dbConnection.ExecuteQuery(query);

                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    user = new User
                    {
                        UserId = Convert.ToInt32(row["UserId"]),
                        Username = row["Username"].ToString(),
                        Password = row["Password"].ToString(),
                        FullName = row["FullName"].ToString(),
                        Role = (UserRole)Convert.ToInt32(row["Role"]),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]),
                        IsActive = Convert.ToBoolean(row["IsActive"])
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return user;
        }

        public bool AddUser(User user)
        {
            try
            {
                if (GetUserByUsername(user.Username) != null)
                {
                    MessageBox.Show("Пользователь с таким именем уже существует", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                string query = $@"
                    INSERT INTO Users (Username, Password, FullName, Role, CreatedDate, IsActive)
                    VALUES ('{user.Username}', '{HashPassword(user.Password)}', '{user.FullName}', {(int)user.Role}, '{user.CreatedDate.ToString("yyyy-MM-dd HH:mm:ss")}', {(user.IsActive ? 1 : 0)})";

                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool UpdateUser(User user)
        {
            try
            {
                string passwordUpdate = string.IsNullOrEmpty(user.Password) ? "" : $", Password = '{HashPassword(user.Password)}'";
                
                string query = $@"
                    UPDATE Users
                    SET Username = '{user.Username}',
                        FullName = '{user.FullName}',
                        Role = {(int)user.Role},
                        IsActive = {(user.IsActive ? 1 : 0)}
                        {passwordUpdate}
                    WHERE UserId = {user.UserId}";

                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                string query = $"DELETE FROM Users WHERE UserId = {userId}";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool ValidateUser(string username, string password)
        {
            try
            {
                User user = GetUserByUsername(username);
                if (user != null && user.IsActive)
                {
                    string hashedPassword = HashPassword(password);
                    return user.Password == hashedPassword;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при проверке пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
