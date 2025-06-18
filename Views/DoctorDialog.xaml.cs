using System;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class DoctorDialog : Window
    {
        private Doctor doctor;
        private bool isNew;

        public DoctorDialog(Doctor doctor, bool isNew)
        {
            InitializeComponent();
            this.doctor = doctor;
            this.isNew = isNew;

            // Заполнение полей данными
            txtCod.Text = doctor.Cod_Doctor.ToString();
            txtFIO.Text = doctor.FIO_Doctor;
            txtNrUchastka.Text = doctor.Nr_Uchastka_DOC.ToString();
            txtNrCabinet.Text = doctor.Nr_Cabinet.ToString();

            // Установка заголовка окна
            Title = isNew ? "Добавление врача" : "Редактирование врача";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация данных
                if (string.IsNullOrWhiteSpace(txtFIO.Text))
                {
                    MessageBox.Show("Поле 'ФИО' не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(txtNrUchastka.Text, out int nrUchastka) || nrUchastka <= 0)
                {
                    MessageBox.Show("Поле '№ Участка' должно быть положительным числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(txtNrCabinet.Text, out int nrCabinet) || nrCabinet <= 0)
                {
                    MessageBox.Show("Поле '№ Кабинета' должно быть положительным числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Обновление данных объекта
                doctor.FIO_Doctor = txtFIO.Text;
                doctor.Nr_Uchastka_DOC = nrUchastka;
                doctor.Nr_Cabinet = nrCabinet;

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
