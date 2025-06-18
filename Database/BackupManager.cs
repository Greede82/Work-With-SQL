using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace WpfApp1.Database
{
    public class BackupManager
    {
        private static string backupFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "Dispanserizatsia", "Backups");

        public BackupManager()
        {
            
            if (!Directory.Exists(backupFolder))
            {
                Directory.CreateDirectory(backupFolder);
            }
        }

        
        
        
        
        public string CreateBackup()
        {
            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFileName = $"Dispanserizatsia_Backup_{timestamp}.bak";
                string backupPath = Path.Combine(backupFolder, backupFileName);

                string query = $@"BACKUP DATABASE [Dispanserizatsia] TO DISK = N'{backupPath}' 
                                WITH NOFORMAT, NOINIT, NAME = N'Dispanserizatsia-Full Database Backup', 
                                SKIP, NOREWIND, NOUNLOAD, STATS = 10";

                using (SqlConnection masterConnection = new SqlConnection(@"Data Source=.;Initial Catalog=master;Integrated Security=True"))
                {
                    masterConnection.Open();
                    using (SqlCommand command = new SqlCommand(query, masterConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                return backupPath;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public bool RestoreBackup(string backupPath)
        {
            try
            {
                if (!File.Exists(backupPath))
                {
                    System.Windows.MessageBox.Show("Файл резервной копии не найден", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                string setDatabaseSingleUser = @"
                    USE [master]
                    ALTER DATABASE [Dispanserizatsia] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";

                string restoreDatabase = $@"
                    USE [master]
                    RESTORE DATABASE [Dispanserizatsia] FROM DISK = N'{backupPath}' 
                    WITH FILE = 1, NOUNLOAD, REPLACE, STATS = 5";

                string setDatabaseMultiUser = @"
                    USE [master]
                    ALTER DATABASE [Dispanserizatsia] SET MULTI_USER";

                using (SqlConnection masterConnection = new SqlConnection(@"Data Source=.;Initial Catalog=master;Integrated Security=True"))
                {
                    masterConnection.Open();
                    
                    using (SqlCommand command = new SqlCommand(setDatabaseSingleUser, masterConnection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (SqlCommand command = new SqlCommand(restoreDatabase, masterConnection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (SqlCommand command = new SqlCommand(setDatabaseMultiUser, masterConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при восстановлении базы данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public string[] GetBackupsList()
        {
            try
            {
                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                    return new string[0];
                }

                return Directory.GetFiles(backupFolder, "*.bak");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при получении списка резервных копий: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return new string[0];
            }
        }

        public string SelectBackupFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Файлы резервных копий (*.bak)|*.bak",
                Title = "Выберите файл резервной копии",
                InitialDirectory = backupFolder
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }
    }
}
