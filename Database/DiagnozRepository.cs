using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Database
{
    public class DiagnozRepository
    {
        private DatabaseConnection dbConnection = DatabaseConnection.Instance;

        public List<DocDiagnoz> GetAllDiagnozes()
        {
            List<DocDiagnoz> diagnozes = new List<DocDiagnoz>();
            try
            {
                DataTable dataTable = dbConnection.ExecuteQuery("SELECT * FROM DocDiagnoz ORDER BY Cod_Diagnoz");
                
                int rowNumber = 1;
                foreach (DataRow row in dataTable.Rows)
                {
                    diagnozes.Add(new DocDiagnoz
                    {
                        Cod_Diagnoz = Convert.ToInt32(row["Cod_Diagnoz"]),
                        Diagnoz = row["Diagnoz"].ToString(),
                        RowNumber = rowNumber++
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении диагнозов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return diagnozes;
        }

        public DocDiagnoz GetDiagnozById(int id)
        {
            DocDiagnoz diagnoz = null;
            try
            {
                string query = $"SELECT * FROM DocDiagnoz WHERE Cod_Diagnoz = {id}";
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    diagnoz = new DocDiagnoz
                    {
                        Cod_Diagnoz = Convert.ToInt32(row["Cod_Diagnoz"]),
                        Diagnoz = row["Diagnoz"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении диагноза: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return diagnoz;
        }

        public List<DocDiagnoz> SearchDiagnozes(string searchText)
        {
            List<DocDiagnoz> diagnozes = new List<DocDiagnoz>();
            try
            {
                string query = $"SELECT * FROM DocDiagnoz WHERE Diagnoz LIKE '%{searchText}%' ORDER BY Cod_Diagnoz";
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                int rowNumber = 1;
                foreach (DataRow row in dataTable.Rows)
                {
                    diagnozes.Add(new DocDiagnoz
                    {
                        Cod_Diagnoz = Convert.ToInt32(row["Cod_Diagnoz"]),
                        Diagnoz = row["Diagnoz"].ToString(),
                        RowNumber = rowNumber++
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске диагнозов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return diagnozes;
        }

        public bool AddDiagnoz(DocDiagnoz diagnoz)
        {
            try
            {
                string query = $"INSERT INTO DocDiagnoz (Cod_Diagnoz, Diagnoz) VALUES ({diagnoz.Cod_Diagnoz}, '{diagnoz.Diagnoz}')";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении диагноза: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool UpdateDiagnoz(DocDiagnoz diagnoz)
        {
            try
            {
                string query = $"UPDATE DocDiagnoz SET Diagnoz = '{diagnoz.Diagnoz}' WHERE Cod_Diagnoz = {diagnoz.Cod_Diagnoz}";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении диагноза: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool DeleteDiagnoz(int id)
        {
            try
            {
                // First check if the diagnoz is used in Priem table
                string checkQuery = $"SELECT COUNT(*) FROM Priem WHERE Cod_Diagnoz = {id}";
                int count = Convert.ToInt32(dbConnection.ExecuteScalar(checkQuery));
                
                if (count > 0)
                {
                    // If diagnoz is used, we need to delete related records first
                    string[] queries = new string[]
                    {
                        // Delete from Lechenie where the diagnoz is used in prescriptions
                        $@"DELETE FROM Lechenie WHERE Nr_Retsepta IN 
                           (SELECT r.Nr_Retsepta FROM Retsept r 
                            INNER JOIN Priem p ON r.Cod_Priema = p.Cod_Priema 
                            WHERE p.Cod_Diagnoz = {id})",
                        
                        // Delete from Retsept where the diagnoz is used in appointments
                        $@"DELETE FROM Retsept WHERE Cod_Priema IN 
                           (SELECT Cod_Priema FROM Priem WHERE Cod_Diagnoz = {id})",
                        
                        // Delete from Priem where the diagnoz is used
                        $"DELETE FROM Priem WHERE Cod_Diagnoz = {id}",
                        
                        // Finally delete the diagnoz
                        $"DELETE FROM DocDiagnoz WHERE Cod_Diagnoz = {id}"
                    };
                    
                    return dbConnection.ExecuteTransaction(queries);
                }
                else
                {
                    // If diagnoz is not used, simply delete it
                    string query = $"DELETE FROM DocDiagnoz WHERE Cod_Diagnoz = {id}";
                    int result = dbConnection.ExecuteNonQuery(query);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении диагноза: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public int GetNextDiagnozId()
        {
            try
            {
                string query = "SELECT MAX(Cod_Diagnoz) + 1 FROM DocDiagnoz";
                object result = dbConnection.ExecuteScalar(query);
                return result == DBNull.Value ? 1001 : Convert.ToInt32(result);
            }
            catch (Exception)
            {
                return 1001; // Default starting ID
            }
        }
    }
}
