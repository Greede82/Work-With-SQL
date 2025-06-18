using System;
using System.Collections.Generic;
using System.Windows;
using WpfApp1.Database;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class PriemDialog : Window
    {
        private Priem priem;
        private bool isNew;
        private DiagnozRepository diagnozRepository;
        private PacientRepository pacientRepository;
        private DoctorRepository doctorRepository;

        public PriemDialog(Priem priem, bool isNew)
        {
            InitializeComponent();
            this.priem = priem;
            this.isNew = isNew;

            // Инициализация репозиториев
            diagnozRepository = new DiagnozRepository();
            pacientRepository = new PacientRepository();
            doctorRepository = new DoctorRepository();

            // Загрузка данных для комбобоксов
            LoadComboBoxData();

            // Заполнение полей данными
            txtCodPriema.Text = priem.Cod_Priema.ToString();
            cmbDoctor.SelectedValue = priem.Cod_Doctor;
            cmbPacient.SelectedValue = priem.Cod_Pacient;
            cmbDiagnoz.SelectedValue = priem.Cod_Diagnoz;
            dpDataPriema.SelectedDate = priem.Data_Priema;
            txtTimePriema.Text = priem.Time_Priema.ToString();

            // Установка заголовка окна
            Title = isNew ? "Добавление приема" : "Редактирование приема";
        }

        private void LoadComboBoxData()
        {
            try
            {
                // Загрузка врачей
                List<Doctor> doctors = doctorRepository.GetAllDoctors();
                cmbDoctor.ItemsSource = doctors;

                // Загрузка пациентов
                List<Pacient> pacients = pacientRepository.GetAllPacients();
                cmbPacient.ItemsSource = pacients;

                // Загрузка диагнозов
                List<DocDiagnoz> diagnozes = diagnozRepository.GetAllDiagnozes();
                cmbDiagnoz.ItemsSource = diagnozes;
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
                if (cmbDoctor.SelectedItem == null)
                {
                    MessageBox.Show("Выберите врача", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (cmbPacient.SelectedItem == null)
                {
                    MessageBox.Show("Выберите пациента", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (cmbDiagnoz.SelectedItem == null)
                {
                    MessageBox.Show("Выберите диагноз", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!dpDataPriema.SelectedDate.HasValue)
                {
                    MessageBox.Show("Выберите дату приема", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!decimal.TryParse(txtTimePriema.Text, out decimal timePriema) || timePriema < 0 || timePriema >= 24)
                {
                    MessageBox.Show("Время приема должно быть числом в формате ЧЧ.ММ (например, 12.30)", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Обновление данных объекта
                priem.Cod_Doctor = (int)cmbDoctor.SelectedValue;
                priem.Cod_Pacient = (int)cmbPacient.SelectedValue;
                priem.Cod_Diagnoz = (int)cmbDiagnoz.SelectedValue;
                priem.Data_Priema = dpDataPriema.SelectedDate.Value;
                priem.Time_Priema = timePriema;

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
