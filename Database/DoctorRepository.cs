using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Database
{
    public class DoctorRepository
    {
        private DatabaseConnection dbConnection = DatabaseConnection.Instance;

        public List<Doctor> GetAllDoctors()
        {
            List<Doctor> doctors = new List<Doctor>();
            try
            {
                DataTable dataTable = dbConnection.ExecuteQuery("SELECT * FROM Doctor ORDER BY Cod_Doctor");
                
                foreach (DataRow row in dataTable.Rows)
                {
                    doctors.Add(new Doctor
                    {
                        Cod_Doctor = Convert.ToInt32(row["Cod_Doctor"]),
                        FIO_Doctor = row["FIO_Doctor"].ToString(),
                        Nr_Uchastka_DOC = Convert.ToInt32(row["Nr_Uchastka_DOC"]),
                        Nr_Cabinet = Convert.ToInt32(row["Nr_Cabinet"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении врачей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return doctors;
        }

        public Doctor GetDoctorById(int id)
        {
            Doctor doctor = null;
            try
            {
                string query = $"SELECT * FROM Doctor WHERE Cod_Doctor = {id}";
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                if (dataTable.Rows.Count > 0)
                {
                    DataRow row = dataTable.Rows[0];
                    doctor = new Doctor
                    {
                        Cod_Doctor = Convert.ToInt32(row["Cod_Doctor"]),
                        FIO_Doctor = row["FIO_Doctor"].ToString(),
                        Nr_Uchastka_DOC = Convert.ToInt32(row["Nr_Uchastka_DOC"]),
                        Nr_Cabinet = Convert.ToInt32(row["Nr_Cabinet"])
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении врача: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return doctor;
        }

        public List<Doctor> SearchDoctors(string searchText)
        {
            List<Doctor> doctors = new List<Doctor>();
            try
            {
                string query = $"SELECT * FROM Doctor WHERE FIO_Doctor LIKE '%{searchText}%' ORDER BY Cod_Doctor";
                DataTable dataTable = dbConnection.ExecuteQuery(query);
                
                foreach (DataRow row in dataTable.Rows)
                {
                    doctors.Add(new Doctor
                    {
                        Cod_Doctor = Convert.ToInt32(row["Cod_Doctor"]),
                        FIO_Doctor = row["FIO_Doctor"].ToString(),
                        Nr_Uchastka_DOC = Convert.ToInt32(row["Nr_Uchastka_DOC"]),
                        Nr_Cabinet = Convert.ToInt32(row["Nr_Cabinet"])
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске врачей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return doctors;
        }

        public bool AddDoctor(Doctor doctor)
        {
            try
            {
                string query = $"INSERT INTO Doctor (Cod_Doctor, FIO_Doctor, Nr_Uchastka_DOC, Nr_Cabinet) VALUES ({doctor.Cod_Doctor}, '{doctor.FIO_Doctor}', {doctor.Nr_Uchastka_DOC}, {doctor.Nr_Cabinet})";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении врача: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool UpdateDoctor(Doctor doctor)
        {
            try
            {
                string query = $"UPDATE Doctor SET FIO_Doctor = '{doctor.FIO_Doctor}', Nr_Uchastka_DOC = {doctor.Nr_Uchastka_DOC}, Nr_Cabinet = {doctor.Nr_Cabinet} WHERE Cod_Doctor = {doctor.Cod_Doctor}";
                int result = dbConnection.ExecuteNonQuery(query);
                return result > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении врача: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public bool DeleteDoctor(int id)
        {
            try
            {
                // First check if the doctor is used in Priem table
                string checkQuery = $"SELECT COUNT(*) FROM Priem WHERE Cod_Doctor = {id}";
                int count = Convert.ToInt32(dbConnection.ExecuteScalar(checkQuery));
                
                if (count > 0)
                {
                    // If doctor is used, we need to delete related records first
                    string[] queries = new string[]
                    {
                        // Delete from Lechenie where the doctor is used in prescriptions
                        $@"DELETE FROM Lechenie WHERE Nr_Retsepta IN 
                           (SELECT r.Nr_Retsepta FROM Retsept r 
                            INNER JOIN Priem p ON r.Cod_Priema = p.Cod_Priema 
                            WHERE p.Cod_Doctor = {id})",
                        
                        // Delete from Retsept where the doctor is used in appointments
                        $@"DELETE FROM Retsept WHERE Cod_Priema IN 
                           (SELECT Cod_Priema FROM Priem WHERE Cod_Doctor = {id})",
                        
                        // Delete from Priem where the doctor is used
                        $"DELETE FROM Priem WHERE Cod_Doctor = {id}",
                        
                        // Finally delete the doctor
                        $"DELETE FROM Doctor WHERE Cod_Doctor = {id}"
                    };
                    
                    return dbConnection.ExecuteTransaction(queries);
                }
                else
                {
                    // If doctor is not used, simply delete it
                    string query = $"DELETE FROM Doctor WHERE Cod_Doctor = {id}";
                    int result = dbConnection.ExecuteNonQuery(query);
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении врача: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public int GetNextDoctorId()
        {
            try
            {
                string query = "SELECT MAX(Cod_Doctor) + 1 FROM Doctor";
                object result = dbConnection.ExecuteScalar(query);
                return result == DBNull.Value ? 3001 : Convert.ToInt32(result);
            }
            catch (Exception)
            {
                return 3001; // Default starting ID
            }
        }
    }
}
