using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Database
{
    public class PriemRepository
    {
        private DatabaseConnection dbConnection = DatabaseConnection.Instance;

        public List<Priem> GetAllPriems()
        {
            List<Priem> priems = new List<Priem>();
            try
            {
                string query = @"
                    SELECT p.*, d.FIO_Doctor, pa.FIO_Pacient, dd.Diagnoz
                    FROM Priem p
                    INNER JOIN Doctor d ON p.Cod_Doctor = d.Cod_Doctor
                    INNER JOIN Pacient pa ON p.Cod_Pacient = pa.Cod_Pacient
                    INNER JOIN DocDiagnoz dd ON p.Cod_Diagnoz = dd.Cod_Diagnoz
                    ORDER BY p.Cod_Priema";
                
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                foreach (DataRow row in dataTable.Rows)
                {
                    priems.Add(new Priem
                    {
                        Cod_Priema = Convert.ToInt32(row["Cod_Priema"]),
                        Cod_Doctor = Convert.ToInt32(row["Cod_Doctor"]),
                        Cod_Pacient = Convert.ToInt32(row["Cod_Pacient"]),
                        Cod_Diagnoz = Convert.ToInt32(row["Cod_Diagnoz"]),
                        Data_Priema = Convert.ToDateTime(row["Data_Priema"]),
                        Time_Priema = Convert.ToDecimal(row["Time_Priema"]),
                        DoctorName = row["FIO_Doctor"].ToString(),
                        PacientName = row["FIO_Pacient"].ToString(),
                        DiagnozName = row["Diagnoz"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении приемов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return priems;
        }

        public Priem GetPriemById(int id)
        {
            Priem priem = null;
            try
            {
                string query = $@"
                    SELECT p.*, d.FIO_Doctor, pa.FIO_Pacient, dd.Diagnoz
                    FROM Priem p
                    INNER JOIN Doctor d ON p.Cod_Doctor = d.Cod_Doctor
                    INNER JOIN Pacient pa ON p.Cod_Pacient = pa.Cod_Pacient
                    INNER JOIN DocDiagnoz dd ON p.Cod_Diagnoz = dd.Cod_Diagnoz
                    WHERE p.Cod_Priema = {id}";
                
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    priem = new Priem
                    {
                        Cod_Priema = Convert.ToInt32(row["Cod_Priema"]),
                        Cod_Doctor = Convert.ToInt32(row["Cod_Doctor"]),
                        Cod_Pacient = Convert.ToInt32(row["Cod_Pacient"]),
                        Cod_Diagnoz = Convert.ToInt32(row["Cod_Diagnoz"]),
                        Data_Priema = Convert.ToDateTime(row["Data_Priema"]),
                        Time_Priema = Convert.ToDecimal(row["Time_Priema"]),
                        DoctorName = row["FIO_Doctor"].ToString(),
                        PacientName = row["FIO_Pacient"].ToString(),
                        DiagnozName = row["Diagnoz"].ToString()
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении приема: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return priem;
        }

        public List<Priem> SearchPriems(string searchText)
        {
            List<Priem> priems = new List<Priem>();
            try
            {
                string query = $@"
                    SELECT p.*, d.FIO_Doctor, pa.FIO_Pacient, dd.Diagnoz
                    FROM Priem p
                    INNER JOIN Doctor d ON p.Cod_Doctor = d.Cod_Doctor
                    INNER JOIN Pacient pa ON p.Cod_Pacient = pa.Cod_Pacient
                    INNER JOIN DocDiagnoz dd ON p.Cod_Diagnoz = dd.Cod_Diagnoz
                    WHERE d.FIO_Doctor LIKE '%{searchText}%' 
                    OR pa.FIO_Pacient LIKE '%{searchText}%' 
                    OR dd.Diagnoz LIKE '%{searchText}%'
                    OR CONVERT(VARCHAR, p.Data_Priema, 103) LIKE '%{searchText}%'
                    ORDER BY p.Cod_Priema";
                
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                foreach (DataRow row in dataTable.Rows)
                {
                    priems.Add(new Priem
                    {
                        Cod_Priema = Convert.ToInt32(row["Cod_Priema"]),
                        Cod_Doctor = Convert.ToInt32(row["Cod_Doctor"]),
                        Cod_Pacient = Convert.ToInt32(row["Cod_Pacient"]),
                        Cod_Diagnoz = Convert.ToInt32(row["Cod_Diagnoz"]),
                        Data_Priema = Convert.ToDateTime(row["Data_Priema"]),
                        Time_Priema = Convert.ToDecimal(row["Time_Priema"]),
                        DoctorName = row["FIO_Doctor"].ToString(),
                        PacientName = row["FIO_Pacient"].ToString(),
                        DiagnozName = row["Diagnoz"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске приемов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return priems;
        }

        public List<Priem> FilterPriemsByDate(DateTime startDate, DateTime endDate)
        {
            List<Priem> priems = new List<Priem>();
            try
            {
                string query = $@"
                    SELECT p.*, d.FIO_Doctor, pa.FIO_Pacient, dd.Diagnoz
                    FROM Priem p
                    INNER JOIN Doctor d ON p.Cod_Doctor = d.Cod_Doctor
                    INNER JOIN Pacient pa ON p.Cod_Pacient = pa.Cod_Pacient
                    INNER JOIN DocDiagnoz dd ON p.Cod_Diagnoz = dd.Cod_Diagnoz
                    WHERE p.Data_Priema BETWEEN '{startDate:yyyy-MM-dd}' AND '{endDate:yyyy-MM-dd}'
                    ORDER BY p.Data_Priema, p.Time_Priema";
                
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                foreach (DataRow row in dataTable.Rows)
                {
                    priems.Add(new Priem
                    {
                        Cod_Priema = Convert.ToInt32(row["Cod_Priema"]),
                        Cod_Doctor = Convert.ToInt32(row["Cod_Doctor"]),
                        Cod_Pacient = Convert.ToInt32(row["Cod_Pacient"]),
                        Cod_Diagnoz = Convert.ToInt32(row["Cod_Diagnoz"]),
                        Data_Priema = Convert.ToDateTime(row["Data_Priema"]),
                        Time_Priema = Convert.ToDecimal(row["Time_Priema"]),
                        DoctorName = row["FIO_Doctor"].ToString(),
                        PacientName = row["FIO_Pacient"].ToString(),
                        DiagnozName = row["Diagnoz"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации приемов по дате: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return priems;
        }

        public List<Priem> FilterPriemsByDoctor(int doctorId)
        {
            List<Priem> priems = new List<Priem>();
            try
            {
                string query = $@"
                    SELECT p.*, d.FIO_Doctor, pa.FIO_Pacient, dd.Diagnoz
                    FROM Priem p
                    INNER JOIN Doctor d ON p.Cod_Doctor = d.Cod_Doctor
                    INNER JOIN Pacient pa ON p.Cod_Pacient = pa.Cod_Pacient
                    INNER JOIN DocDiagnoz dd ON p.Cod_Diagnoz = dd.Cod_Diagnoz
                    WHERE p.Cod_Doctor = {doctorId}
                    ORDER BY p.Data_Priema, p.Time_Priema";
                
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                foreach (DataRow row in dataTable.Rows)
                {
                    priems.Add(new Priem
                    {
                        Cod_Priema = Convert.ToInt32(row["Cod_Priema"]),
                        Cod_Doctor = Convert.ToInt32(row["Cod_Doctor"]),
                        Cod_Pacient = Convert.ToInt32(row["Cod_Pacient"]),
                        Cod_Diagnoz = Convert.ToInt32(row["Cod_Diagnoz"]),
                        Data_Priema = Convert.ToDateTime(row["Data_Priema"]),
                        Time_Priema = Convert.ToDecimal(row["Time_Priema"]),
                        DoctorName = row["FIO_Doctor"].ToString(),
                        PacientName = row["FIO_Pacient"].ToString(),
                        DiagnozName = row["Diagnoz"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации приемов по врачу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return priems;
        }

        public bool AddPriem(Priem priem)
        {
            try
            {
                string query = $"INSERT INTO Priem (Cod_Priema, Cod_Doctor, Cod_Pacient, Cod_Diagnoz, Data_Priema, Time_Priema) VALUES ({priem.Cod_Priema}, {priem.Cod_Doctor}, {priem.Cod_Pacient}, {priem.Cod_Diagnoz}, '{priem.Data_Priema:yyyy-MM-dd}', {priem.Time_Priema})";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении приема: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool UpdatePriem(Priem priem)
        {
            try
            {
                string query = $"UPDATE Priem SET Cod_Doctor = {priem.Cod_Doctor}, Cod_Pacient = {priem.Cod_Pacient}, Cod_Diagnoz = {priem.Cod_Diagnoz}, Data_Priema = '{priem.Data_Priema:yyyy-MM-dd}', Time_Priema = {priem.Time_Priema} WHERE Cod_Priema = {priem.Cod_Priema}";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении приема: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool DeletePriem(int id)
        {
            try
            {
                // First check if the priem is used in Retsept table
                string checkQuery = $"SELECT COUNT(*) FROM Retsept WHERE Cod_Priema = {id}";
                int count = Convert.ToInt32(dbConnection.ExecuteScalar(checkQuery));
                
                if (count > 0)
                {
                    // If priem is used, we need to delete related records first
                    string[] queries = new string[]
                    {
                        // Delete from Lechenie where the priem is used in prescriptions
                        $@"DELETE FROM Lechenie WHERE Nr_Retsepta IN 
                           (SELECT Nr_Retsepta FROM Retsept WHERE Cod_Priema = {id})",
                        
                        // Delete from Retsept where the priem is used
                        $"DELETE FROM Retsept WHERE Cod_Priema = {id}",
                        
                        // Finally delete the priem
                        $"DELETE FROM Priem WHERE Cod_Priema = {id}"
                    };
                    
                    return dbConnection.ExecuteTransaction(queries);
                }
                else
                {
                    // If priem is not used, simply delete it
                    string query = $"DELETE FROM Priem WHERE Cod_Priema = {id}";
                    int result = dbConnection.ExecuteNonQuery(query);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении приема: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public int GetNextPriemId()
        {
            try
            {
                string query = "SELECT MAX(Cod_Priema) + 1 FROM Priem";
                object result = dbConnection.ExecuteScalar(query);
                return result == DBNull.Value ? 1 : Convert.ToInt32(result);
            }
            catch (Exception)
            {
                return 1; // Default starting ID
            }
        }

        // Statistics methods
        public int GetTotalPriems()
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Priem";
                object result = dbConnection.ExecuteScalar(query);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении общего количества приемов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public int GetPriemCountByDiagnoz(int diagnozId)
        {
            try
            {
                string query = $"SELECT COUNT(*) FROM Priem WHERE Cod_Diagnoz = {diagnozId}";
                object result = dbConnection.ExecuteScalar(query);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении количества приемов по диагнозу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public int GetPriemCountByDoctor(int doctorId)
        {
            try
            {
                string query = $"SELECT COUNT(*) FROM Priem WHERE Cod_Doctor = {doctorId}";
                object result = dbConnection.ExecuteScalar(query);
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении количества приемов по врачу: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return 0;
            }
        }

        public DataTable GetPriemCountByMonth()
        {
            try
            {
                string query = @"
                    SELECT MONTH(Data_Priema) AS [Month], COUNT(*) AS [Count]
                    FROM Priem
                    GROUP BY MONTH(Data_Priema)
                    ORDER BY [Month]";
                return dbConnection.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении статистики приемов по месяцам: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new DataTable();
            }
        }

        public DataTable GetDiagnozStatistics()
        {
            try
            {
                string query = @"
                    SELECT dd.Diagnoz, COUNT(p.Cod_Priema) AS [Count]
                    FROM Priem p
                    INNER JOIN DocDiagnoz dd ON p.Cod_Diagnoz = dd.Cod_Diagnoz
                    GROUP BY dd.Diagnoz
                    ORDER BY [Count] DESC";
                return dbConnection.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении статистики по диагнозам: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new DataTable();
            }
        }

        public DataTable GetDoctorWorkloadStatistics()
        {
            try
            {
                string query = @"
                    SELECT d.FIO_Doctor, COUNT(p.Cod_Priema) AS [Count]
                    FROM Priem p
                    INNER JOIN Doctor d ON p.Cod_Doctor = d.Cod_Doctor
                    GROUP BY d.FIO_Doctor
                    ORDER BY [Count] DESC";
                return dbConnection.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении статистики загрузки врачей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new DataTable();
            }
        }
    }
}
