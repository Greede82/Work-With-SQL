using System;
using System.Windows;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class PacientDialog : Window
    {
        private Pacient pacient;
        private bool isNew;

        public PacientDialog(Pacient pacient, bool isNew)
        {
            InitializeComponent();
            this.pacient = pacient;
            this.isNew = isNew;

            // Заполнение полей данными
            txtCod.Text = pacient.Cod_Pacient.ToString();
            txtFIO.Text = pacient.FIO_Pacient;
            txtAdress.Text = pacient.Adress;
            txtIDNP.Text = pacient.IDNP;
            txtStrahovka.Text = pacient.Strahovka;
            txtNrUchastka.Text = pacient.Nr_Uchastka.ToString();

            // Установка заголовка окна
            Title = isNew ? "Добавление пациента" : "Редактирование пациента";
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

                if (string.IsNullOrWhiteSpace(txtIDNP.Text) || txtIDNP.Text.Length != 13)
                {
                    MessageBox.Show("Поле 'IDNP' должно содержать 13 символов", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(txtNrUchastka.Text, out int nrUchastka) || nrUchastka <= 0)
                {
                    MessageBox.Show("Поле '№ Участка' должно быть положительным числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Обновление данных объекта
                pacient.FIO_Pacient = txtFIO.Text;
                pacient.Adress = txtAdress.Text;
                pacient.IDNP = txtIDNP.Text;
                pacient.Strahovka = txtStrahovka.Text;
                pacient.Nr_Uchastka = nrUchastka;

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
