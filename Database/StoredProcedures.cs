using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace WpfApp1.Database
{
    public class StoredProcedures
    {
        private DatabaseConnection dbConnection = DatabaseConnection.Instance;

        public bool CreateStoredProcedures()
        {
            try
            {
                string[] procedures = new string[]
                {
                    @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetPacientDetails')
                    DROP PROCEDURE GetPacientDetails
                    GO

                    CREATE PROCEDURE GetPacientDetails
                        @PacientId INT
                    AS
                    BEGIN
                        SELECT 
                            pa.Cod_Pacient, pa.FIO_Pacient, pa.Adress, pa.IDNP, pa.Strahovka, pa.Nr_Uchastka,
                            p.Cod_Priema, p.Data_Priema, p.Time_Priema,
                            d.FIO_Doctor, dd.Diagnoz
                        FROM Pacient pa
                        LEFT JOIN Priem p ON pa.Cod_Pacient = p.Cod_Pacient
                        LEFT JOIN Doctor d ON p.Cod_Doctor = d.Cod_Doctor
                        LEFT JOIN DocDiagnoz dd ON p.Cod_Diagnoz = dd.Cod_Diagnoz
                        WHERE pa.Cod_Pacient = @PacientId
                        ORDER BY p.Data_Priema DESC, p.Time_Priema
                    END
                    GO",


                    @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetDoctorWorkload')
                    DROP PROCEDURE GetDoctorWorkload
                    GO

                    CREATE PROCEDURE GetDoctorWorkload
                        @StartDate DATE = NULL,
                        @EndDate DATE = NULL
                    AS
                    BEGIN
                        IF @StartDate IS NULL
                            SET @StartDate = '1900-01-01'
                        
                        IF @EndDate IS NULL
                            SET @EndDate = '2100-12-31'
                            
                        SELECT 
                            d.Cod_Doctor, d.FIO_Doctor, d.Nr_Uchastka_DOC, d.Nr_Cabinet,
                            COUNT(p.Cod_Priema) AS AppointmentCount,
                            MIN(p.Data_Priema) AS FirstAppointment,
                            MAX(p.Data_Priema) AS LastAppointment
                        FROM Doctor d
                        LEFT JOIN Priem p ON d.Cod_Doctor = p.Cod_Doctor AND p.Data_Priema BETWEEN @StartDate AND @EndDate
                        GROUP BY d.Cod_Doctor, d.FIO_Doctor, d.Nr_Uchastka_DOC, d.Nr_Cabinet
                        ORDER BY AppointmentCount DESC
                    END
                    GO",


                    @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetDiagnosisStatistics')
                    DROP PROCEDURE GetDiagnosisStatistics
                    GO

                    CREATE PROCEDURE GetDiagnosisStatistics
                        @StartDate DATE = NULL,
                        @EndDate DATE = NULL
                    AS
                    BEGIN
                        IF @StartDate IS NULL
                            SET @StartDate = '1900-01-01'
                        
                        IF @EndDate IS NULL
                            SET @EndDate = '2100-12-31'
                            
                        SELECT 
                            dd.Cod_Diagnoz, dd.Diagnoz,
                            COUNT(p.Cod_Priema) AS AppointmentCount
                        FROM DocDiagnoz dd
                        LEFT JOIN Priem p ON dd.Cod_Diagnoz = p.Cod_Diagnoz AND p.Data_Priema BETWEEN @StartDate AND @EndDate
                        GROUP BY dd.Cod_Diagnoz, dd.Diagnoz
                        ORDER BY AppointmentCount DESC
                    END
                    GO",


                    @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetPrescriptionDetails')
                    DROP PROCEDURE GetPrescriptionDetails
                    GO

                    CREATE PROCEDURE GetPrescriptionDetails
                        @RetseptId INT
                    AS
                    BEGIN
                        SELECT 
                            r.Nr_Retsepta, r.Cod_Priema,
                            p.Data_Priema, p.Time_Priema,
                            pa.Cod_Pacient, pa.FIO_Pacient,
                            d.Cod_Doctor, d.FIO_Doctor,
                            dd.Cod_Diagnoz, dd.Diagnoz,
                            lk.Cod_Lekarstva, lk.Name_Lekarstva, lk.Dozirovka, lk.Type_Upakovka, lk.Gruppa
                        FROM Retsept r
                        INNER JOIN Priem p ON r.Cod_Priema = p.Cod_Priema
                        INNER JOIN Pacient pa ON p.Cod_Pacient = pa.Cod_Pacient
                        INNER JOIN Doctor d ON p.Cod_Doctor = d.Cod_Doctor
                        INNER JOIN DocDiagnoz dd ON p.Cod_Diagnoz = dd.Cod_Diagnoz
                        LEFT JOIN Lechenie l ON r.Nr_Retsepta = l.Nr_Retsepta
                        LEFT JOIN Lekarstvo lk ON l.Cod_Lekarstva = lk.Cod_Lekarstva
                        WHERE r.Nr_Retsepta = @RetseptId
                    END
                    GO",


                    @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'AddAppointmentWithPrescription')
                    DROP PROCEDURE AddAppointmentWithPrescription
                    GO

                    CREATE PROCEDURE AddAppointmentWithPrescription
                        @Cod_Priema INT,
                        @Cod_Doctor INT,
                        @Cod_Pacient INT,
                        @Cod_Diagnoz INT,
                        @Data_Priema DATE,
                        @Time_Priema DECIMAL(5,2),
                        @Nr_Retsepta INT,
                        @Lekarstva VARCHAR(MAX)
                    AS
                    BEGIN
                        BEGIN TRANSACTION
                        
                        BEGIN TRY

                            INSERT INTO Priem (Cod_Priema, Cod_Doctor, Cod_Pacient, Cod_Diagnoz, Data_Priema, Time_Priema)
                            VALUES (@Cod_Priema, @Cod_Doctor, @Cod_Pacient, @Cod_Diagnoz, @Data_Priema, @Time_Priema)
                            

                            INSERT INTO Retsept (Nr_Retsepta, Cod_Priema)
                            VALUES (@Nr_Retsepta, @Cod_Priema)
                            

                            DECLARE @Lekarstvo INT
                            DECLARE @Pos INT
                            DECLARE @Str VARCHAR(MAX)
                            
                            SET @Str = @Lekarstva
                            
                            WHILE LEN(@Str) > 0
                            BEGIN
                                SET @Pos = CHARINDEX(',', @Str)
                                
                                IF @Pos = 0
                                BEGIN
                                    SET @Lekarstvo = CAST(@Str AS INT)
                                    SET @Str = ''
                                END
                                ELSE
                                BEGIN
                                    SET @Lekarstvo = CAST(LEFT(@Str, @Pos - 1) AS INT)
                                    SET @Str = RIGHT(@Str, LEN(@Str) - @Pos)
                                END
                                
                                INSERT INTO Lechenie (Cod_Lekarstva, Nr_Retsepta)
                                VALUES (@Lekarstvo, @Nr_Retsepta)
                            END
                            
                            COMMIT TRANSACTION
                            SELECT 1 AS Result
                        END TRY
                        BEGIN CATCH
                            ROLLBACK TRANSACTION
                            SELECT 0 AS Result
                        END CATCH
                    END
                    GO",


                    @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'DeletePacientWithRelatedRecords')
                    DROP PROCEDURE DeletePacientWithRelatedRecords
                    GO

                    CREATE PROCEDURE DeletePacientWithRelatedRecords
                        @Cod_Pacient INT
                    AS
                    BEGIN
                        BEGIN TRANSACTION
                        
                        BEGIN TRY

                            DELETE FROM Lechenie
                            WHERE Nr_Retsepta IN (
                                SELECT r.Nr_Retsepta
                                FROM Retsept r
                                INNER JOIN Priem p ON r.Cod_Priema = p.Cod_Priema
                                WHERE p.Cod_Pacient = @Cod_Pacient
                            )
                            
                            DELETE FROM Retsept
                            WHERE Cod_Priema IN (
                                SELECT Cod_Priema
                                FROM Priem
                                WHERE Cod_Pacient = @Cod_Pacient
                            )
                            

                            DELETE FROM Priem
                            WHERE Cod_Pacient = @Cod_Pacient
                            
                            DELETE FROM Pacient
                            WHERE Cod_Pacient = @Cod_Pacient
                            
                            COMMIT TRANSACTION
                            SELECT 1 AS Result
                        END TRY
                        BEGIN CATCH
                            ROLLBACK TRANSACTION
                            SELECT 0 AS Result
                        END CATCH
                    END
                    GO",
                    

                    @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetDoctorWorkloadStatistics')
                    DROP PROCEDURE GetDoctorWorkloadStatistics
                    GO

                    CREATE PROCEDURE GetDoctorWorkloadStatistics
                    AS
                    BEGIN
                        SELECT 
                            d.Cod_Doctor, 
                            d.FIO_Doctor, 
                            d.Nr_Uchastka_DOC, 
                            d.Nr_Cabinet,
                            COUNT(p.Cod_Priema) AS TotalAppointments,
                            COUNT(DISTINCT p.Cod_Pacient) AS UniquePatients,
                            COUNT(DISTINCT p.Cod_Diagnoz) AS UniqueDiagnoses
                        FROM Doctor d
                        LEFT JOIN Priem p ON d.Cod_Doctor = p.Cod_Doctor
                        GROUP BY d.Cod_Doctor, d.FIO_Doctor, d.Nr_Uchastka_DOC, d.Nr_Cabinet
                        ORDER BY TotalAppointments DESC
                    END
                    GO",
                    

                    @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetDiagnosisStatistics')
                    DROP PROCEDURE GetDiagnosisStatistics
                    GO

                    CREATE PROCEDURE GetDiagnosisStatistics
                    AS
                    BEGIN
                        SELECT 
                            dd.Cod_Diagnoz, 
                            dd.Diagnoz,
                            COUNT(p.Cod_Priema) AS TotalAppointments,
                            COUNT(DISTINCT p.Cod_Pacient) AS UniquePatients,
                            COUNT(DISTINCT p.Cod_Doctor) AS UniqueDoctors
                        FROM DocDiagnoz dd
                        LEFT JOIN Priem p ON dd.Cod_Diagnoz = p.Cod_Diagnoz
                        GROUP BY dd.Cod_Diagnoz, dd.Diagnoz
                        ORDER BY TotalAppointments DESC
                    END
                    GO",
                    

                    @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetMedicationUsageStatistics')
                    DROP PROCEDURE GetMedicationUsageStatistics
                    GO

                    CREATE PROCEDURE GetMedicationUsageStatistics
                    AS
                    BEGIN
                        SELECT 
                            l.Cod_Lekarstva,
                            lk.Name_Lekarstva AS 'Название лекарства',
                            COUNT(l.Nr_Retsepta) AS 'Количество использований',
                            COUNT(DISTINCT r.Cod_Priema) AS 'Количество приемов',
                            COUNT(DISTINCT p.Cod_Pacient) AS 'Количество пациентов'
                        FROM Lechenie l
                        INNER JOIN Lekarstvo lk ON l.Cod_Lekarstva = lk.Cod_Lekarstva
                        INNER JOIN Retsept r ON l.Nr_Retsepta = r.Nr_Retsepta
                        INNER JOIN Priem p ON r.Cod_Priema = p.Cod_Priema
                        GROUP BY l.Cod_Lekarstva, lk.Name_Lekarstva
                        ORDER BY 'Количество использований' DESC
                    END
                    GO"
                };

                foreach (string procedure in procedures)
                {
                    int result = dbConnection.ExecuteNonQuery(procedure);
                    if (result < 0)
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании хранимых процедур: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public DataTable GetMedicationUsageStatistics()
        {
            try
            {
                if (dbConnection.OpenConnection())
                {
                    using (SqlCommand command = new SqlCommand("GetMedicationUsageStatistics", dbConnection.GetConnection()))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении процедуры GetMedicationUsageStatistics: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
            return new DataTable();
        }

        public DataTable ExecuteGetPacientDetails(int pacientId)
        {
            try
            {
                if (dbConnection.OpenConnection())
                {
                    using (SqlCommand command = new SqlCommand("GetPacientDetails", dbConnection.GetConnection()))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PacientId", pacientId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении процедуры GetPacientDetails: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
            return new DataTable();
        }

        public DataTable ExecuteGetDoctorWorkload(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                if (dbConnection.OpenConnection())
                {
                    using (SqlCommand command = new SqlCommand("GetDoctorWorkload", dbConnection.GetConnection()))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        
                        if (startDate.HasValue)
                            command.Parameters.AddWithValue("@StartDate", startDate.Value);
                        else
                            command.Parameters.AddWithValue("@StartDate", DBNull.Value);
                            
                        if (endDate.HasValue)
                            command.Parameters.AddWithValue("@EndDate", endDate.Value);
                        else
                            command.Parameters.AddWithValue("@EndDate", DBNull.Value);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении процедуры GetDoctorWorkload: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
            return new DataTable();
        }

        public DataTable ExecuteGetDiagnosisStatistics(DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                if (dbConnection.OpenConnection())
                {
                    using (SqlCommand command = new SqlCommand("GetDiagnosisStatistics", dbConnection.GetConnection()))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        
                        if (startDate.HasValue)
                            command.Parameters.AddWithValue("@StartDate", startDate.Value);
                        else
                            command.Parameters.AddWithValue("@StartDate", DBNull.Value);
                            
                        if (endDate.HasValue)
                            command.Parameters.AddWithValue("@EndDate", endDate.Value);
                        else
                            command.Parameters.AddWithValue("@EndDate", DBNull.Value);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении процедуры GetDiagnosisStatistics: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
            return new DataTable();
        }

        public DataTable ExecuteGetPrescriptionDetails(int retseptId)
        {
            try
            {
                if (dbConnection.OpenConnection())
                {
                    using (SqlCommand command = new SqlCommand("GetPrescriptionDetails", dbConnection.GetConnection()))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@RetseptId", retseptId);

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении процедуры GetPrescriptionDetails: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
            return new DataTable();
        }

        public bool ExecuteAddAppointmentWithPrescription(int codPriema, int codDoctor, int codPacient, int codDiagnoz, 
                                                         DateTime dataPriema, decimal timePriema, int nrRetsepta, string lekarstva)
        {
            try
            {
                if (dbConnection.OpenConnection())
                {
                    using (SqlCommand command = new SqlCommand("AddAppointmentWithPrescription", dbConnection.GetConnection()))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Cod_Priema", codPriema);
                        command.Parameters.AddWithValue("@Cod_Doctor", codDoctor);
                        command.Parameters.AddWithValue("@Cod_Pacient", codPacient);
                        command.Parameters.AddWithValue("@Cod_Diagnoz", codDiagnoz);
                        command.Parameters.AddWithValue("@Data_Priema", dataPriema);
                        command.Parameters.AddWithValue("@Time_Priema", timePriema);
                        command.Parameters.AddWithValue("@Nr_Retsepta", nrRetsepta);
                        command.Parameters.AddWithValue("@Lekarstva", lekarstva);

                        object result = command.ExecuteScalar();
                        return result != null && Convert.ToInt32(result) == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении процедуры AddAppointmentWithPrescription: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
            return false;
        }

        public bool ExecuteDeletePacientWithRelatedRecords(int pacientId)
        {
            try
            {
                if (dbConnection.OpenConnection())
                {
                    using (SqlCommand command = new SqlCommand("DeletePacientWithRelatedRecords", dbConnection.GetConnection()))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Cod_Pacient", pacientId);

                        object result = command.ExecuteScalar();
                        return result != null && Convert.ToInt32(result) == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении процедуры DeletePacientWithRelatedRecords: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
            return false;
        }
        
        // Add methods for new stored procedures
        public DataTable ExecuteGetDoctorWorkloadStatistics()
        {
            try
            {
                if (dbConnection.OpenConnection())
                {
                    using (SqlCommand command = new SqlCommand("GetDoctorWorkloadStatistics", dbConnection.GetConnection()))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении процедуры GetDoctorWorkloadStatistics: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
            return new DataTable();
        }
        
        public DataTable ExecuteGetDiagnoseStatistics()
        {
            try
            {
                if (dbConnection.OpenConnection())
                {
                    using (SqlCommand command = new SqlCommand("GetDiagnosisStatistics", dbConnection.GetConnection()))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            DataTable dataTable = new DataTable();
                            adapter.Fill(dataTable);
                            return dataTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении процедуры GetDiagnosisStatistics: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                dbConnection.CloseConnection();
            }
            return new DataTable();
        }
    }
}
