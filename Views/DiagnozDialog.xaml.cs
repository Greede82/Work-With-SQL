using System;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class DiagnozDialog : Window
    {
        private DocDiagnoz diagnoz;
        private bool isNew;

        public DiagnozDialog(DocDiagnoz diagnoz, bool isNew)
        {
            InitializeComponent();
            this.diagnoz = diagnoz;
            this.isNew = isNew;

            // Заполнение полей данными
            txtCod.Text = diagnoz.Cod_Diagnoz.ToString();
            txtDiagnoz.Text = diagnoz.Diagnoz;

            // Установка заголовка окна
            Title = isNew ? "Добавление диагноза" : "Редактирование диагноза";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация данных
                if (string.IsNullOrWhiteSpace(txtDiagnoz.Text))
                {
                    MessageBox.Show("Поле 'Диагноз' не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Обновление данных объекта
                diagnoz.Diagnoz = txtDiagnoz.Text;

                // Закрытие окна с результатом true (успешно)
                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Закрытие окна с результатом false (отмена)
            DialogResult = false;
        }
    }
}
