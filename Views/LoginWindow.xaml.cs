using System;
using System.Windows;
using WpfApp1.Database;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class LoginWindow : Window
    {
        private UserRepository userRepository;

        public LoginWindow()
        {
            InitializeComponent();
            userRepository = new UserRepository();
            txtUsername.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Пожалуйста, введите имя пользователя и пароль";
                return;
            }

            try
            {
                if (userRepository.ValidateUser(username, password))
                {
                    User user = userRepository.GetUserByUsername(username);
                    UserSession.Login(user);
                    
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                else
                {
                    txtError.Text = "Неверное имя пользователя или пароль";
                    txtPassword.Password = "";
                }
            }
            catch (Exception ex)
            {
                txtError.Text = $"Ошибка при входе: {ex.Message}";
            }
        }
    }
}
