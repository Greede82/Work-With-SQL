using System;
using System.Collections.Generic;
using System.Windows;
using WpfApp1.Database;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class LechenieDialog : Window
    {
        private Lechenie lechenie;
        private bool isNew;
        private RetseptRepository retseptRepository;
        private LekarstvoRepository lekarstvoRepository;

        public LechenieDialog(Lechenie lechenie, bool isNew)
        {
            InitializeComponent();
            this.lechenie = lechenie;
            this.isNew = isNew;

            // Инициализация репозиториев
            retseptRepository = new RetseptRepository();
            lekarstvoRepository = new LekarstvoRepository();

            // Загрузка данных для комбобоксов
            LoadComboBoxData();

            // Заполнение полей данными
            cmbRetsept.SelectedValue = lechenie.Nr_Retsepta;
            cmbLekarstvo.SelectedValue = lechenie.Cod_Lekarstva;

            // Установка заголовка окна
            Title = isNew ? "Добавление лечения" : "Редактирование лечения";
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Загрузка рецептов
                List<Retsept> retsepts = retseptRepository.GetAllRetsepts();
                cmbRetsept.ItemsSource = retsepts;
                cmbRetsept.DisplayMemberPath = "Nr_Retsepta";
                cmbRetsept.SelectedValuePath = "Nr_Retsepta";

                // Загрузка лекарств
                List<Lekarstvo> lekarstva = lekarstvoRepository.GetAllLekarstva();
                cmbLekarstvo.ItemsSource = lekarstva;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Валидация данных
                if (cmbRetsept.SelectedItem == null)
                {
                    MessageBox.Show("Выберите рецепт", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (cmbLekarstvo.SelectedItem == null)
                {
                    MessageBox.Show("Выберите лекарство", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Обновление данных объекта
                lechenie.Nr_Retsepta = (int)cmbRetsept.SelectedValue;
                lechenie.Cod_Lekarstva = (int)cmbLekarstvo.SelectedValue;

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
