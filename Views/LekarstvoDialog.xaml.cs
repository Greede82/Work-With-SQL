using System;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class LekarstvoDialog : Window
    {
        private Lekarstvo lekarstvo;
        private bool isNew;

        public LekarstvoDialog(Lekarstvo lekarstvo, bool isNew)
        {
            InitializeComponent();
            this.lekarstvo = lekarstvo;
            this.isNew = isNew;

            // Заполнение полей данными
            txtCod.Text = lekarstvo.Cod_Lekarstva.ToString();
            txtName.Text = lekarstvo.Name_Lekarstva;
            txtDozirovka.Text = lekarstvo.Dozirovka.ToString();
            txtTypeUpakovka.Text = lekarstvo.Type_Upakovka;
            txtGruppa.Text = lekarstvo.Gruppa;

            // Установка заголовка окна
            Title = isNew ? "Добавление лекарства" : "Редактирование лекарства";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация данных
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("Поле 'Название' не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(txtDozirovka.Text, out int dozirovka) || dozirovka <= 0)
                {
                    MessageBox.Show("Поле 'Дозировка' должно быть положительным числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTypeUpakovka.Text))
                {
                    MessageBox.Show("Поле 'Тип упаковки' не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtGruppa.Text))
                {
                    MessageBox.Show("Поле 'Группа' не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Обновление данных объекта
                lekarstvo.Name_Lekarstva = txtName.Text;
                lekarstvo.Dozirovka = dozirovka;
                lekarstvo.Type_Upakovka = txtTypeUpakovka.Text;
                lekarstvo.Gruppa = txtGruppa.Text;

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
