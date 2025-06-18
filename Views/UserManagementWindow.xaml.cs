using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp1.Database;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class UserManagementWindow : Window
    {
        private UserRepository userRepository;
        private List<User> allUsers;
        private User selectedUser;
        private bool isEditMode = false;

        public UserManagementWindow()
        {
            InitializeComponent();
            userRepository = new UserRepository();
            LoadUsers();
            ResetForm();
        }

        private void LoadUsers()
        {
            try
            {
                allUsers = userRepository.GetAllUsers();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ApplyFilter()
        {
            string searchText = txtSearchUser.Text.ToLower();
            var filteredUsers = allUsers;

            if (!string.IsNullOrEmpty(searchText))
            {
                filteredUsers = allUsers.Where(u => 
                    u.Username.ToLower().Contains(searchText) || 
                    u.FullName.ToLower().Contains(searchText)).ToList();
            }

            dgUsers.ItemsSource = filteredUsers;
        }

        private void ResetForm()
        {
            txtFormTitle.Text = "Новый пользователь";
            txtUsername.Text = "";
            txtPassword.Password = "";
            txtFullName.Text = "";
            cmbRole.SelectedIndex = 0;
            chkIsActive.IsChecked = true;
            txtFormError.Text = "";
            isEditMode = false;
            selectedUser = null;
            btnDelete.IsEnabled = false;
        }

        private void FillForm(User user)
        {
            txtFormTitle.Text = "Редактирование пользователя";
            txtUsername.Text = user.Username;
            txtPassword.Password = "";
            txtFullName.Text = user.FullName;
            cmbRole.SelectedIndex = (int)user.Role - 1;
            chkIsActive.IsChecked = user.IsActive;
            txtFormError.Text = "";
            isEditMode = true;
            selectedUser = user;
            btnDelete.IsEnabled = true;
        }

        private void txtSearchUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilter();
        }

        private void btnClearSearch_Click(object sender, RoutedEventArgs e)
        {
            txtSearchUser.Text = "";
            ApplyFilter();
        }

        private void dgUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgUsers.SelectedItem is User user)
            {
                FillForm(user);
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Password.Trim();
                string fullName = txtFullName.Text.Trim();
                int roleTag = int.Parse(((ComboBoxItem)cmbRole.SelectedItem).Tag.ToString());
                bool isActive = chkIsActive.IsChecked ?? true;

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(fullName))
                {
                    txtFormError.Text = "Пожалуйста, заполните все обязательные поля";
                    return;
                }

                if (!isEditMode && string.IsNullOrEmpty(password))
                {
                    txtFormError.Text = "Пожалуйста, введите пароль для нового пользователя";
                    return;
                }

                if (isEditMode)
                {
                    // Обновление существующего пользователя
                    selectedUser.Username = username;
                    selectedUser.FullName = fullName;
                    selectedUser.Role = (UserRole)roleTag;
                    selectedUser.IsActive = isActive;
                    
                    // Обновляем пароль только если он был введен
                    if (!string.IsNullOrEmpty(password))
                    {
                        selectedUser.Password = password;
                    }

                    if (userRepository.UpdateUser(selectedUser))
                    {
                        MessageBox.Show("Пользователь успешно обновлен", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadUsers();
                        ResetForm();
                    }
                }
                else
                {
                    // Добавление нового пользователя
                    User newUser = new User
                    {
                        Username = username,
                        Password = password,
                        FullName = fullName,
                        Role = (UserRole)roleTag,
                        CreatedDate = DateTime.Now,
                        IsActive = isActive
                    };

                    if (userRepository.AddUser(newUser))
                    {
                        MessageBox.Show("Пользователь успешно добавлен", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadUsers();
                        ResetForm();
                    }
                }
            }
            catch (Exception ex)
            {
                txtFormError.Text = $"Ошибка: {ex.Message}";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedUser == null) return;

            // Не позволяем удалить текущего пользователя
            if (UserSession.CurrentUser != null && UserSession.CurrentUser.UserId == selectedUser.UserId)
            {
                MessageBox.Show("Невозможно удалить текущего пользователя", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Вы уверены, что хотите удалить пользователя {selectedUser.Username}?", 
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    if (userRepository.DeleteUser(selectedUser.UserId))
                    {
                        MessageBox.Show("Пользователь успешно удален", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadUsers();
                        ResetForm();
                    }
                }
                catch (Exception ex)
                {
                    txtFormError.Text = $"Ошибка при удалении: {ex.Message}";
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
