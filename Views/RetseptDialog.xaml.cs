using System;
using System.Collections.Generic;
using System.Windows;
using WpfApp1.Database;
using WpfApp1.Models;

namespace WpfApp1.Views
{
    public partial class RetseptDialog : Window
    {
        private Retsept retsept;
        private bool isNew;
        private PriemRepository priemRepository;

        public RetseptDialog(Retsept retsept, bool isNew)
        {
            InitializeComponent();
            this.retsept = retsept;
            this.isNew = isNew;

            priemRepository = new PriemRepository();

            LoadComboBoxData();

            txtNrRetsepta.Text = retsept.Nr_Retsepta.ToString();
            cmbPriem.SelectedValue = retsept.Cod_Priema;


            Title = isNew ? "Добавление рецепта" : "Редактирование рецепта";
        }

        private void LoadComboBoxData()
        {
            try
            {

                List<Priem> priems = priemRepository.GetAllPriems();
                cmbPriem.ItemsSource = priems;
                cmbPriem.DisplayMemberPath = "Cod_Priema";
                cmbPriem.SelectedValuePath = "Cod_Priema";
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

                if (cmbPriem.SelectedItem == null)
                {
                    MessageBox.Show("Выберите прием", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                if (!int.TryParse(txtNrRetsepta.Text, out int nrRetsepta))
                {
                    MessageBox.Show("Введите корректный номер рецепта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                retsept.Nr_Retsepta = nrRetsepta;
                retsept.Cod_Priema = (int)cmbPriem.SelectedValue;


                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

            DialogResult = false;
        }
    }
}
