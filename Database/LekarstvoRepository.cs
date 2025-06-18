using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Database
{
    public class LekarstvoRepository
    {
        private DatabaseConnection dbConnection = DatabaseConnection.Instance;

        public List<Lekarstvo> GetAllLekarstva()
        {
            List<Lekarstvo> lekarstva = new List<Lekarstvo>();
            try
            {
                DataTable dataTable = dbConnection.ExecuteQuery("SELECT * FROM Lekarstvo ORDER BY Cod_Lekarstva");
                
                foreach (DataRow row in dataTable.Rows)
                {
                    lekarstva.Add(new Lekarstvo
                    {
                        Cod_Lekarstva = Convert.ToInt32(row["Cod_Lekarstva"]),
                        Name_Lekarstva = row["Name_Lekarstva"].ToString(),
                        Dozirovka = Convert.ToInt32(row["Dozirovka"]),
                        Type_Upakovka = row["Type_Upakovka"].ToString(),
                        Gruppa = row["Gruppa"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении лекарств: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return lekarstva;
        }

        public Lekarstvo GetLekarstvoById(int id)
        {
            Lekarstvo lekarstvo = null;
            try
            {
                string query = $"SELECT * FROM Lekarstvo WHERE Cod_Lekarstva = {id}";
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    lekarstvo = new Lekarstvo
                    {
                        Cod_Lekarstva = Convert.ToInt32(row["Cod_Lekarstva"]),
                        Name_Lekarstva = row["Name_Lekarstva"].ToString(),
                        Dozirovka = Convert.ToInt32(row["Dozirovka"]),
                        Type_Upakovka = row["Type_Upakovka"].ToString(),
                        Gruppa = row["Gruppa"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении лекарства: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return lekarstvo;
        }

        public List<Lekarstvo> SearchLekarstva(string searchText)
        {
            List<Lekarstvo> lekarstva = new List<Lekarstvo>();
            try
            {
                string query = $"SELECT * FROM Lekarstvo WHERE Name_Lekarstva LIKE '%{searchText}%' OR Gruppa LIKE '%{searchText}%' ORDER BY Cod_Lekarstva";
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                foreach (DataRow row in dataTable.Rows)
                {
                    lekarstva.Add(new Lekarstvo
                    {
                        Cod_Lekarstva = Convert.ToInt32(row["Cod_Lekarstva"]),
                        Name_Lekarstva = row["Name_Lekarstva"].ToString(),
                        Dozirovka = Convert.ToInt32(row["Dozirovka"]),
                        Type_Upakovka = row["Type_Upakovka"].ToString(),
                        Gruppa = row["Gruppa"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске лекарств: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return lekarstva;
        }

        public bool AddLekarstvo(Lekarstvo lekarstvo)
        {
            try
            {
                string query = $"INSERT INTO Lekarstvo (Cod_Lekarstva, Name_Lekarstva, Dozirovka, Type_Upakovka, Gruppa) VALUES ({lekarstvo.Cod_Lekarstva}, '{lekarstvo.Name_Lekarstva}', {lekarstvo.Dozirovka}, '{lekarstvo.Type_Upakovka}', '{lekarstvo.Gruppa}')";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении лекарства: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool UpdateLekarstvo(Lekarstvo lekarstvo)
        {
            try
            {
                string query = $"UPDATE Lekarstvo SET Name_Lekarstva = '{lekarstvo.Name_Lekarstva}', Dozirovka = {lekarstvo.Dozirovka}, Type_Upakovka = '{lekarstvo.Type_Upakovka}', Gruppa = '{lekarstvo.Gruppa}' WHERE Cod_Lekarstva = {lekarstvo.Cod_Lekarstva}";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении лекарства: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool DeleteLekarstvo(int id)
        {
            try
            {
                // First check if the lekarstvo is used in Lechenie table
                string checkQuery = $"SELECT COUNT(*) FROM Lechenie WHERE Cod_Lekarstva = {id}";
                int count = Convert.ToInt32(dbConnection.ExecuteScalar(checkQuery));
                
                if (count > 0)
                {
                    // If lekarstvo is used, we need to delete related records first
                    string[] queries = new string[]
                    {
                        // Delete from Lechenie where the lekarstvo is used
                        $"DELETE FROM Lechenie WHERE Cod_Lekarstva = {id}",
                        
                        // Finally delete the lekarstvo
                        $"DELETE FROM Lekarstvo WHERE Cod_Lekarstva = {id}"
                    };
                    
                    return dbConnection.ExecuteTransaction(queries);
                }
                else
                {
                    // If lekarstvo is not used, simply delete it
                    string query = $"DELETE FROM Lekarstvo WHERE Cod_Lekarstva = {id}";
                    int result = dbConnection.ExecuteNonQuery(query);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении лекарства: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public int GetNextLekarstvoId()
        {
            try
            {
                string query = "SELECT MAX(Cod_Lekarstva) + 1 FROM Lekarstvo";
                object result = dbConnection.ExecuteScalar(query);
                return result == DBNull.Value ? 4001 : Convert.ToInt32(result);
            }
            catch (Exception)
            {
                return 4001; // Default starting ID
            }
        }
    }
}
