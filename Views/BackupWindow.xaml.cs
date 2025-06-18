using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using Microsoft.Win32;
using WpfApp1.Database;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class BackupWindow : Window
    {
        private DbBackupManager backupManager;
        private List<BackupInfo> backupsList;

        public BackupWindow()
        {
            InitializeComponent();
            
            // Проверяем права пользователя
            if (!UserSession.HasPermission(UserRole.Administrator))
            {
                btnCreateBackup.IsEnabled = false;
                btnRestoreBackup.IsEnabled = false;
                btnSelectExternalBackup.IsEnabled = false;
                System.Windows.MessageBox.Show("Для выполнения операций резервного копирования и восстановления необходимы права администратора", 
                    "Ограничение доступа", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
            backupManager = new DbBackupManager();
            LoadBackupsList();
        }

        private void LoadBackupsList()
        {
            try
            {
                string[] backupFiles = backupManager.GetBackupsList();
                backupsList = new List<BackupInfo>();

                foreach (string filePath in backupFiles)
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    backupsList.Add(new BackupInfo
                    {
                        FileName = Path.GetFileNameWithoutExtension(filePath),
                        FilePath = filePath,
                        CreationDate = $"Создан: {fileInfo.CreationTime.ToString("dd.MM.yyyy HH:mm:ss")}",
                        FileSize = $"Размер: {FormatFileSize(fileInfo.Length)}"
                    });
                }

                // Сортируем по дате создания (новые сверху)
                backupsList = backupsList.OrderByDescending(b => b.FilePath).ToList();
                lstBackups.ItemsSource = backupsList;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при загрузке списка резервных копий: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "Б", "КБ", "МБ", "ГБ" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void btnCreateBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (System.Windows.MessageBox.Show("Вы уверены, что хотите создать резервную копию базы данных?", 
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    string backupPath = backupManager.CreateBackup();
                    System.Windows.Input.Mouse.OverrideCursor = null;

                    if (!string.IsNullOrEmpty(backupPath))
                    {
                        System.Windows.MessageBox.Show($"Резервная копия успешно создана: {backupPath}", 
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadBackupsList();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Input.Mouse.OverrideCursor = null;
                System.Windows.MessageBox.Show($"Ошибка при создании резервной копии: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRestoreBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BackupInfo selectedBackup = lstBackups.SelectedItem as BackupInfo;
                if (selectedBackup == null) return;

                if (System.Windows.MessageBox.Show("ВНИМАНИЕ! Восстановление базы данных приведет к замене всех текущих данных данными из резервной копии.\n\n" +
                    "Вы уверены, что хотите продолжить?", 
                    "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    bool success = backupManager.RestoreBackup(selectedBackup.FilePath);
                    System.Windows.Input.Mouse.OverrideCursor = null;

                    if (success)
                    {
                        System.Windows.MessageBox.Show("База данных успешно восстановлена из резервной копии.\n" +
                            "Приложение будет перезапущено для применения изменений.", 
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                        // Перезапускаем приложение
                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                        Application.Current.Shutdown();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Input.Mouse.OverrideCursor = null;
                System.Windows.MessageBox.Show($"Ошибка при восстановлении базы данных: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSelectExternalBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string backupPath = backupManager.SelectBackupFile();
                if (!string.IsNullOrEmpty(backupPath))
                {
                    if (System.Windows.MessageBox.Show("ВНИМАНИЕ! Восстановление базы данных приведет к замене всех текущих данных данными из резервной копии.\n\n" +
                        "Вы уверены, что хотите продолжить?", 
                        "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                        bool success = backupManager.RestoreBackup(backupPath);
                        System.Windows.Input.Mouse.OverrideCursor = null;

                        if (success)
                        {
                            System.Windows.MessageBox.Show("База данных успешно восстановлена из резервной копии.\n" +
                                "Приложение будет перезапущено для применения изменений.", 
                                "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                            
                            // Перезапускаем приложение
                            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                            Application.Current.Shutdown();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Input.Mouse.OverrideCursor = null;
                System.Windows.MessageBox.Show($"Ошибка при восстановлении базы данных: {ex.Message}", 
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRefreshList_Click(object sender, RoutedEventArgs e)
        {
            LoadBackupsList();
        }

        private void lstBackups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnRestoreBackup.IsEnabled = lstBackups.SelectedItem != null && UserSession.HasPermission(UserRole.Administrator);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }

    public class BackupInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string CreationDate { get; set; }
        public string FileSize { get; set; }
    }
}
