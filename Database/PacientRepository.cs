using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Database
{
    public class PacientRepository
    {
        private DatabaseConnection dbConnection = DatabaseConnection.Instance;

        public List<Pacient> GetAllPacients()
        {
            List<Pacient> pacients = new List<Pacient>();
            try
            {
                DataTable dataTable = dbConnection.ExecuteQuery("SELECT * FROM Pacient ORDER BY Cod_Pacient");
                
                int rowNumber = 1;
                foreach (DataRow row in dataTable.Rows)
                {
                    pacients.Add(new Pacient
                    {
                        Cod_Pacient = Convert.ToInt32(row["Cod_Pacient"]),
                        FIO_Pacient = row["FIO_Pacient"].ToString(),
                        Adress = row["Adress"].ToString(),
                        IDNP = row["IDNP"].ToString(),
                        Strahovka = row["Strahovka"].ToString(),
                        Nr_Uchastka = Convert.ToInt32(row["Nr_Uchastka"]),
                        RowNumber = rowNumber++
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении пациентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return pacients;
        }

        public Pacient GetPacientById(int id)
        {
            Pacient pacient = null;
            try
            {
                string query = $"SELECT * FROM Pacient WHERE Cod_Pacient = {id}";
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    pacient = new Pacient
                    {
                        Cod_Pacient = Convert.ToInt32(row["Cod_Pacient"]),
                        FIO_Pacient = row["FIO_Pacient"].ToString(),
                        Adress = row["Adress"].ToString(),
                        IDNP = row["IDNP"].ToString(),
                        Strahovka = row["Strahovka"].ToString(),
                        Nr_Uchastka = Convert.ToInt32(row["Nr_Uchastka"])
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении пациента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return pacient;
        }

        public List<Pacient> SearchPacients(string searchText)
        {
            List<Pacient> pacients = new List<Pacient>();
            try
            {
                string query = $"SELECT * FROM Pacient WHERE FIO_Pacient LIKE '%{searchText}%' OR Adress LIKE '%{searchText}%' OR IDNP LIKE '%{searchText}%' ORDER BY Cod_Pacient";
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                int rowNumber = 1;
                foreach (DataRow row in dataTable.Rows)
                {
                    pacients.Add(new Pacient
                    {
                        Cod_Pacient = Convert.ToInt32(row["Cod_Pacient"]),
                        FIO_Pacient = row["FIO_Pacient"].ToString(),
                        Adress = row["Adress"].ToString(),
                        IDNP = row["IDNP"].ToString(),
                        Strahovka = row["Strahovka"].ToString(),
                        Nr_Uchastka = Convert.ToInt32(row["Nr_Uchastka"]),
                        RowNumber = rowNumber++
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске пациентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return pacients;
        }

        public bool AddPacient(Pacient pacient)
        {
            try
            {
                string query = $"INSERT INTO Pacient (Cod_Pacient, FIO_Pacient, Adress, IDNP, Strahovka, Nr_Uchastka) VALUES ({pacient.Cod_Pacient}, '{pacient.FIO_Pacient}', '{pacient.Adress}', '{pacient.IDNP}', '{pacient.Strahovka}', {pacient.Nr_Uchastka})";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пациента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool UpdatePacient(Pacient pacient)
        {
            try
            {
                string query = $"UPDATE Pacient SET FIO_Pacient = '{pacient.FIO_Pacient}', Adress = '{pacient.Adress}', IDNP = '{pacient.IDNP}', Strahovka = '{pacient.Strahovka}', Nr_Uchastka = {pacient.Nr_Uchastka} WHERE Cod_Pacient = {pacient.Cod_Pacient}";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении пациента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool DeletePacient(int id)
        {
            try
            {
                // First check if the pacient is used in Priem table
                string checkQuery = $"SELECT COUNT(*) FROM Priem WHERE Cod_Pacient = {id}";
                int count = Convert.ToInt32(dbConnection.ExecuteScalar(checkQuery));
                
                if (count > 0)
                {
                    // If pacient is used, we need to delete related records first
                    string[] queries = new string[]
                    {
                        // Delete from Lechenie where the pacient is used in prescriptions
                        $@"DELETE FROM Lechenie WHERE Nr_Retsepta IN 
                           (SELECT r.Nr_Retsepta FROM Retsept r 
                            INNER JOIN Priem p ON r.Cod_Priema = p.Cod_Priema 
                            WHERE p.Cod_Pacient = {id})",
                        
                        // Delete from Retsept where the pacient is used in appointments
                        $@"DELETE FROM Retsept WHERE Cod_Priema IN 
                           (SELECT Cod_Priema FROM Priem WHERE Cod_Pacient = {id})",
                        
                        // Delete from Priem where the pacient is used
                        $"DELETE FROM Priem WHERE Cod_Pacient = {id}",
                        
                        // Finally delete the pacient
                        $"DELETE FROM Pacient WHERE Cod_Pacient = {id}"
                    };
                    
                    return dbConnection.ExecuteTransaction(queries);
                }
                else
                {
                    // If pacient is not used, simply delete it
                    string query = $"DELETE FROM Pacient WHERE Cod_Pacient = {id}";
                    int result = dbConnection.ExecuteNonQuery(query);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении пациента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public int GetNextPacientId()
        {
            try
            {
                string query = "SELECT MAX(Cod_Pacient) + 1 FROM Pacient";
                object result = dbConnection.ExecuteScalar(query);
                return result == DBNull.Value ? 1 : Convert.ToInt32(result);
            }
            catch (Exception)
            {
                return 1; // Default starting ID
            }
        }
    }
}
