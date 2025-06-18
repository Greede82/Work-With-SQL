using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Database
{
    public class LechenieRepository
    {
        private DatabaseConnection dbConnection = DatabaseConnection.Instance;

        public List<Lechenie> GetAllLechenies()
        {
            List<Lechenie> lechenies = new List<Lechenie>();
            try
            {
                string query = @"
                    SELECT l.*, lk.Name_Lekarstva, p.Cod_Pacient, pa.FIO_Pacient
                    FROM Lechenie l
                    INNER JOIN Lekarstvo lk ON l.Cod_Lekarstva = lk.Cod_Lekarstva
                    INNER JOIN Retsept r ON l.Nr_Retsepta = r.Nr_Retsepta
                    INNER JOIN Priem p ON r.Cod_Priema = p.Cod_Priema
                    INNER JOIN Pacient pa ON p.Cod_Pacient = pa.Cod_Pacient
                    ORDER BY l.Nr_Retsepta, l.Cod_Lekarstva";
                
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                foreach (DataRow row in dataTable.Rows)
                {
                    lechenies.Add(new Lechenie
                    {
                        Cod_Lekarstva = Convert.ToInt32(row["Cod_Lekarstva"]),
                        Nr_Retsepta = Convert.ToInt32(row["Nr_Retsepta"]),
                        LekarstvoName = row["Name_Lekarstva"].ToString(),
                        PacientName = row["FIO_Pacient"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении лечений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return lechenies;
        }

        public List<Lechenie> GetLecheniesByRetsept(int retseptId)
        {
            List<Lechenie> lechenies = new List<Lechenie>();
            try
            {
                string query = $@"
                    SELECT l.*, lk.Name_Lekarstva, p.Cod_Pacient, pa.FIO_Pacient
                    FROM Lechenie l
                    INNER JOIN Lekarstvo lk ON l.Cod_Lekarstva = lk.Cod_Lekarstva
                    INNER JOIN Retsept r ON l.Nr_Retsepta = r.Nr_Retsepta
                    INNER JOIN Priem p ON r.Cod_Priema = p.Cod_Priema
                    INNER JOIN Pacient pa ON p.Cod_Pacient = pa.Cod_Pacient
                    WHERE l.Nr_Retsepta = {retseptId}
                    ORDER BY l.Cod_Lekarstva";
                
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                foreach (DataRow row in dataTable.Rows)
                {
                    lechenies.Add(new Lechenie
                    {
                        Cod_Lekarstva = Convert.ToInt32(row["Cod_Lekarstva"]),
                        Nr_Retsepta = Convert.ToInt32(row["Nr_Retsepta"]),
                        LekarstvoName = row["Name_Lekarstva"].ToString(),
                        PacientName = row["FIO_Pacient"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении лечений по рецепту: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return lechenies;
        }

        public List<Lechenie> SearchLechenies(string searchText)
        {
            List<Lechenie> lechenies = new List<Lechenie>();
            try
            {
                string query = $@"
                    SELECT l.*, lk.Name_Lekarstva, p.Cod_Pacient, pa.FIO_Pacient
                    FROM Lechenie l
                    INNER JOIN Lekarstvo lk ON l.Cod_Lekarstva = lk.Cod_Lekarstva
                    INNER JOIN Retsept r ON l.Nr_Retsepta = r.Nr_Retsepta
                    INNER JOIN Priem p ON r.Cod_Priema = p.Cod_Priema
                    INNER JOIN Pacient pa ON p.Cod_Pacient = pa.Cod_Pacient
                    WHERE lk.Name_Lekarstva LIKE '%{searchText}%' 
                    OR pa.FIO_Pacient LIKE '%{searchText}%'
                    OR CAST(l.Nr_Retsepta AS VARCHAR) LIKE '%{searchText}%'
                    ORDER BY l.Nr_Retsepta, l.Cod_Lekarstva";
                
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                foreach (DataRow row in dataTable.Rows)
                {
                    lechenies.Add(new Lechenie
                    {
                        Cod_Lekarstva = Convert.ToInt32(row["Cod_Lekarstva"]),
                        Nr_Retsepta = Convert.ToInt32(row["Nr_Retsepta"]),
                        LekarstvoName = row["Name_Lekarstva"].ToString(),
                        PacientName = row["FIO_Pacient"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске лечений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return lechenies;
        }

        public bool AddLechenie(Lechenie lechenie)
        {
            try
            {
                string query = $"INSERT INTO Lechenie (Cod_Lekarstva, Nr_Retsepta) VALUES ({lechenie.Cod_Lekarstva}, {lechenie.Nr_Retsepta})";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении лечения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool DeleteLechenie(int lekarstvoId, int retseptId)
        {
            try
            {
                string query = $"DELETE FROM Lechenie WHERE Cod_Lekarstva = {lekarstvoId} AND Nr_Retsepta = {retseptId}";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении лечения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public DataTable GetMostPrescribedMedications()
        {
            try
            {
                string query = @"
                    SELECT lk.Name_Lekarstva AS [Название лекарства], COUNT(*) AS [Количество использований]
                    FROM Lechenie l
                    INNER JOIN Lekarstvo lk ON l.Cod_Lekarstva = lk.Cod_Lekarstva
                    GROUP BY lk.Name_Lekarstva
                    ORDER BY [Количество использований] DESC";
                return dbConnection.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении статистики по назначенным лекарствам: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new DataTable();
            }
        }

        public DataTable GetMedicationsByDiagnosis()
        {
            try
            {
                string query = @"
                    SELECT dd.Diagnoz, lk.Name_Lekarstva, COUNT(*) AS [Count]
                    FROM Lechenie l
                    INNER JOIN Lekarstvo lk ON l.Cod_Lekarstva = lk.Cod_Lekarstva
                    INNER JOIN Retsept r ON l.Nr_Retsepta = r.Nr_Retsepta
                    INNER JOIN Priem p ON r.Cod_Priema = p.Cod_Priema
                    INNER JOIN DocDiagnoz dd ON p.Cod_Diagnoz = dd.Cod_Diagnoz
                    GROUP BY dd.Diagnoz, lk.Name_Lekarstva
                    ORDER BY dd.Diagnoz, [Count] DESC";
                return dbConnection.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении статистики по назначенным лекарствам по диагнозам: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new DataTable();
            }
        }
    }
}
