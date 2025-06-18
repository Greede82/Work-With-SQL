using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Database
{
    public class RetseptRepository
    {
        private DatabaseConnection dbConnection = DatabaseConnection.Instance;

        public List<Retsept> GetAllRetsepts()
        {
            List<Retsept> retsepts = new List<Retsept>();
            try
            {
                string query = @"
                    SELECT r.*, p.Cod_Pacient, p.Cod_Diagnoz, pa.FIO_Pacient, dd.Diagnoz
                    FROM Retsept r
                    INNER JOIN Priem p ON r.Cod_Priema = p.Cod_Priema
                    INNER JOIN Pacient pa ON p.Cod_Pacient = pa.Cod_Pacient
                    INNER JOIN DocDiagnoz dd ON p.Cod_Diagnoz = dd.Cod_Diagnoz
                    ORDER BY r.Nr_Retsepta";
                
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                foreach (DataRow row in dataTable.Rows)
                {
                    retsepts.Add(new Retsept
                    {
                        Nr_Retsepta = Convert.ToInt32(row["Nr_Retsepta"]),
                        Cod_Priema = Convert.ToInt32(row["Cod_Priema"]),
                        PacientName = row["FIO_Pacient"].ToString(),
                        DiagnozName = row["Diagnoz"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении рецептов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return retsepts;
        }

        public Retsept GetRetseptById(int id)
        {
            Retsept retsept = null;
            try
            {
                string query = $@"
                    SELECT r.*, p.Cod_Pacient, p.Cod_Diagnoz, pa.FIO_Pacient, dd.Diagnoz
                    FROM Retsept r
                    INNER JOIN Priem p ON r.Cod_Priema = p.Cod_Priema
                    INNER JOIN Pacient pa ON p.Cod_Pacient = pa.Cod_Pacient
                    INNER JOIN DocDiagnoz dd ON p.Cod_Diagnoz = dd.Cod_Diagnoz
                    WHERE r.Nr_Retsepta = {id}";
                
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    retsept = new Retsept
                    {
                        Nr_Retsepta = Convert.ToInt32(row["Nr_Retsepta"]),
                        Cod_Priema = Convert.ToInt32(row["Cod_Priema"]),
                        PacientName = row["FIO_Pacient"].ToString(),
                        DiagnozName = row["Diagnoz"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return retsept;
        }

        public List<Retsept> SearchRetsepts(string searchText)
        {
            List<Retsept> retsepts = new List<Retsept>();
            try
            {
                string query = $@"
                    SELECT r.*, p.Cod_Pacient, p.Cod_Diagnoz, pa.FIO_Pacient, dd.Diagnoz
                    FROM Retsept r
                    INNER JOIN Priem p ON r.Cod_Priema = p.Cod_Priema
                    INNER JOIN Pacient pa ON p.Cod_Pacient = pa.Cod_Pacient
                    INNER JOIN DocDiagnoz dd ON p.Cod_Diagnoz = dd.Cod_Diagnoz
                    WHERE pa.FIO_Pacient LIKE '%{searchText}%' 
                    OR dd.Diagnoz LIKE '%{searchText}%'
                    OR CAST(r.Nr_Retsepta AS VARCHAR) LIKE '%{searchText}%'
                    ORDER BY r.Nr_Retsepta";
                
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                foreach (DataRow row in dataTable.Rows)
                {
                    retsepts.Add(new Retsept
                    {
                        Nr_Retsepta = Convert.ToInt32(row["Nr_Retsepta"]),
                        Cod_Priema = Convert.ToInt32(row["Cod_Priema"]),
                        PacientName = row["FIO_Pacient"].ToString(),
                        DiagnozName = row["Diagnoz"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске рецептов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return retsepts;
        }

        public bool AddRetsept(Retsept retsept)
        {
            try
            {
                // Проверка существования рецепта с таким номером
                string checkQuery = $"SELECT COUNT(*) FROM Retsept WHERE Nr_Retsepta = {retsept.Nr_Retsepta}";
                int count = Convert.ToInt32(dbConnection.ExecuteScalar(checkQuery));
                
                if (count > 0)
                {
                    MessageBox.Show($"Рецепт с номером {retsept.Nr_Retsepta} уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
                // Проверка существования приема
                string checkPriemQuery = $"SELECT COUNT(*) FROM Priem WHERE Cod_Priema = {retsept.Cod_Priema}";
                int priemCount = Convert.ToInt32(dbConnection.ExecuteScalar(checkPriemQuery));
                
                if (priemCount == 0)
                {
                    MessageBox.Show($"Прием с кодом {retsept.Cod_Priema} не существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
                string query = $"INSERT INTO Retsept (Nr_Retsepta, Cod_Priema) VALUES ({retsept.Nr_Retsepta}, {retsept.Cod_Priema})";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool UpdateRetsept(Retsept retsept)
        {
            try
            {
                // Проверка существования приема
                string checkPriemQuery = $"SELECT COUNT(*) FROM Priem WHERE Cod_Priema = {retsept.Cod_Priema}";
                int priemCount = Convert.ToInt32(dbConnection.ExecuteScalar(checkPriemQuery));
                
                if (priemCount == 0)
                {
                    MessageBox.Show($"Прием с кодом {retsept.Cod_Priema} не существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                
                // Проверка, используется ли рецепт в таблице Lechenie
                string checkLechenieQuery = $"SELECT COUNT(*) FROM Lechenie WHERE Nr_Retsepta = {retsept.Nr_Retsepta}";
                int lechenieCount = Convert.ToInt32(dbConnection.ExecuteScalar(checkLechenieQuery));
                
                if (lechenieCount > 0)
                {
                    // Если рецепт используется в лечении, выполняем транзакцию для обновления
                    string[] queries = new string[]
                    {
                        // Обновляем рецепт
                        $"UPDATE Retsept SET Cod_Priema = {retsept.Cod_Priema} WHERE Nr_Retsepta = {retsept.Nr_Retsepta}"
                    };
                    
                    return dbConnection.ExecuteTransaction(queries);
                }
                else
                {
                    // Если рецепт не используется, просто обновляем его
                    string query = $"UPDATE Retsept SET Cod_Priema = {retsept.Cod_Priema} WHERE Nr_Retsepta = {retsept.Nr_Retsepta}";
                    int result = dbConnection.ExecuteNonQuery(query);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool DeleteRetsept(int id)
        {
            try
            {
                // First check if the retsept is used in Lechenie table
                string checkQuery = $"SELECT COUNT(*) FROM Lechenie WHERE Nr_Retsepta = {id}";
                int count = Convert.ToInt32(dbConnection.ExecuteScalar(checkQuery));
                
                if (count > 0)
                {
                    // If retsept is used, we need to delete related records first
                    string[] queries = new string[]
                    {
                        // Delete from Lechenie where the retsept is used
                        $"DELETE FROM Lechenie WHERE Nr_Retsepta = {id}",
                        
                        // Finally delete the retsept
                        $"DELETE FROM Retsept WHERE Nr_Retsepta = {id}"
                    };
                    
                    return dbConnection.ExecuteTransaction(queries);
                }
                else
                {
                    // If retsept is not used, simply delete it
                    string query = $"DELETE FROM Retsept WHERE Nr_Retsepta = {id}";
                    int result = dbConnection.ExecuteNonQuery(query);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public int GetNextRetseptId()
        {
            try
            {
                string query = "SELECT MAX(Nr_Retsepta) + 1 FROM Retsept";
                object result = dbConnection.ExecuteScalar(query);
                return result == DBNull.Value ? 5001 : Convert.ToInt32(result);
            }
            catch (Exception)
            {
                return 5001; // Default starting ID
            }
        }
    }
}
