using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace WpfApp1.Database
{
    public class DatabaseConnection
    {
        private static string connectionString = @"Data Source=.;Initial Catalog=Dispanserizatsia;Integrated Security=True";
        private static DatabaseConnection instance;
        private SqlConnection connection;

        private DatabaseConnection()
        {
            connection = new SqlConnection(connectionString);
        }

        public static DatabaseConnection Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseConnection();
                }
                return instance;
            }
        }

        public bool OpenConnection()
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                return true;
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void CloseConnection()
        {
            if (connection.State != ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public SqlConnection GetConnection()
        {
            return connection;
        }

        public DataTable ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();
            try
            {
                if (OpenConnection())
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения запроса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CloseConnection();
            }
            return dataTable;
        }

        public int ExecuteNonQuery(string query)
        {
            int result = 0;
            try
            {
                if (OpenConnection())
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        result = command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения запроса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                result = -1;
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        public object ExecuteScalar(string query)
        {
            object result = null;
            try
            {
                if (OpenConnection())
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        result = command.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка выполнения запроса: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        public bool ExecuteTransaction(string[] queries)
        {
            bool success = false;
            if (OpenConnection())
            {
                SqlTransaction transaction = connection.BeginTransaction();
                try
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;

                        foreach (string query in queries)
                        {
                            command.CommandText = query;
                            command.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        success = true;
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Ошибка выполнения транзакции: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    CloseConnection();
                }
            }
            return success;
        }
        
        public bool ExecuteSqlScript(string scriptPath)
        {
            try
            {
                if (!System.IO.File.Exists(scriptPath))
                {
                    MessageBox.Show($"Файл скрипта не найден: {scriptPath}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
                string scriptContent = System.IO.File.ReadAllText(scriptPath);
                
                // Разделяем скрипт на команды по GO
                string[] batches = scriptContent.Split(new string[] { "\nGO", "\r\nGO", "\ngo", "\r\ngo" }, StringSplitOptions.RemoveEmptyEntries);
                
                // Выполняем каждую команду отдельно
                foreach (string batch in batches)
                {
                    if (!string.IsNullOrWhiteSpace(batch))
                    {
                        // Определяем, к какой базе данных обращается команда
                        if (batch.Contains("USE [master]") || batch.Contains("CREATE DATABASE") || 
                            batch.Contains("Use master") || batch.Contains("Create DataBase"))
                        {
                            // Для команд к master используем прямое подключение к master
                            using (SqlConnection masterConnection = new SqlConnection(@"Data Source=.;Initial Catalog=master;Integrated Security=True"))
                            {
                                masterConnection.Open();
                                using (SqlCommand command = new SqlCommand(batch, masterConnection))
                                {
                                    try
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        // Игнорируем ошибки существования базы данных
                                        if (!ex.Message.Contains("already exists"))
                                        {
                                            MessageBox.Show($"Ошибка выполнения команды: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Для команд к Dispanserizatsia используем текущее подключение
                            if (OpenConnection())
                            {
                                using (SqlCommand command = new SqlCommand(batch, connection))
                                {
                                    try
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        // Игнорируем ошибки существования объектов
                                        if (!ex.Message.Contains("already exists"))
                                        {
                                            MessageBox.Show($"Ошибка выполнения команды: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                        }
                                    }
                                }
                                CloseConnection();
                            }
                        }
                    }
                }
                
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении SQL-скрипта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
