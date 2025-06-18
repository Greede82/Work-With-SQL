using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps;
using System.Diagnostics;
using WpfApp1.Database;
using WpfApp1.Models;
using WpfApp1.Views;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private DiagnozRepository diagnozRepository;
        private PacientRepository pacientRepository;
        private DoctorRepository doctorRepository;
        private LekarstvoRepository lekarstvoRepository;
        private PriemRepository priemRepository;
        private RetseptRepository retseptRepository;
        private LechenieRepository lechenieRepository;
        private StoredProcedures storedProcedures;
        private UserRepository userRepository;

        private DocDiagnoz selectedDiagnoz;
        private Pacient selectedPacient;
        private Doctor selectedDoctor;
        private Lekarstvo selectedLekarstvo;
        private Priem selectedPriem;
        private Retsept selectedRetsept;
        private Lechenie selectedLechenie;

        private Chart chart;

        public MainWindow()
        {
            InitializeComponent();
            
            if (!UserSession.IsLoggedIn)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();
                this.Close();
                return;
            }
            
            UpdateUserInfo();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                diagnozRepository = new DiagnozRepository();
                pacientRepository = new PacientRepository();
                doctorRepository = new DoctorRepository();
                lekarstvoRepository = new LekarstvoRepository();
                priemRepository = new PriemRepository();
                retseptRepository = new RetseptRepository();
                lechenieRepository = new LechenieRepository();
                storedProcedures = new StoredProcedures();
                userRepository = new UserRepository();


                bool connected = DatabaseConnection.Instance.OpenConnection();
                DatabaseConnection.Instance.CloseConnection();

                if (connected)
                {
                    txtConnectionStatus.Text = "Статус подключения: Подключено";
                    txtConnectionStatus.Foreground = System.Windows.Media.Brushes.Green;


                    LoadAllData();
                    

                    ConfigureAccessByRole();
                }
                else
                {
                    txtConnectionStatus.Text = "Статус подключения: Ошибка подключения";
                    txtConnectionStatus.Foreground = System.Windows.Media.Brushes.Red;
                    MessageBox.Show("Не удалось подключиться к базе данных. Проверьте настройки подключения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }


                InitializeChart();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке приложения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeChart()
        {
            chart = new Chart();
            chart.Dock = System.Windows.Forms.DockStyle.Fill;
            
            ChartArea chartArea = new ChartArea("MainChartArea");
            chart.ChartAreas.Add(chartArea);
            
            Legend legend = new Legend("MainLegend");
            chart.Legends.Add(legend);
            
            chartHost.Child = chart;
        }

        private void LoadAllData()
        {
            LoadDiagnozes();
            LoadPacients();
            LoadDoctors();
            LoadLekarstva();
            LoadPriems();
            LoadRetsepts();
            LoadLechenies();
        }


        private void LoadDiagnozes()
        {
            try
            {
                var diagnozes = diagnozRepository.GetAllDiagnozes();
                

                for (int i = 0; i < diagnozes.Count; i++)
                {
                    var diagnoz = diagnozes[i];
                    diagnoz.GetType().GetProperty("RowNumber")?.SetValue(diagnoz, i + 1);
                }
                
                dgDiagnozes.ItemsSource = diagnozes;
                txtDiagnozCount.Text = $"Всего диагнозов: {diagnozes.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке диагнозов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPacients()
        {
            try
            {
                var pacients = pacientRepository.GetAllPacients();
                

                for (int i = 0; i < pacients.Count; i++)
                {
                    var pacient = pacients[i];
                    pacient.GetType().GetProperty("RowNumber")?.SetValue(pacient, i + 1);
                }
                
                dgPacients.ItemsSource = pacients;
                txtPacientCount.Text = $"Всего пациентов: {pacients.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пациентов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDoctors()
        {
            try
            {
                var doctors = doctorRepository.GetAllDoctors();
                

                for (int i = 0; i < doctors.Count; i++)
                {
                    var doctor = doctors[i];
                    doctor.GetType().GetProperty("RowNumber")?.SetValue(doctor, i + 1);
                }
                
                dgDoctors.ItemsSource = doctors;
                txtDoctorCount.Text = $"Всего врачей: {doctors.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке врачей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLekarstva()
        {
            try
            {
                var lekarstva = lekarstvoRepository.GetAllLekarstva();
                

                for (int i = 0; i < lekarstva.Count; i++)
                {
                    var lekarstvo = lekarstva[i];
                    lekarstvo.GetType().GetProperty("RowNumber")?.SetValue(lekarstvo, i + 1);
                }
                
                dgLekarstva.ItemsSource = lekarstva;
                txtLekarstvoCount.Text = $"Всего лекарств: {lekarstva.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке лекарств: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadPriems()
        {
            try
            {
                var priems = priemRepository.GetAllPriems();
                

                for (int i = 0; i < priems.Count; i++)
                {
                    var priem = priems[i];
                    priem.GetType().GetProperty("RowNumber")?.SetValue(priem, i + 1);
                }
                
                dgPriems.ItemsSource = priems;
                txtPriemCount.Text = $"Всего приемов: {priems.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке приемов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadRetsepts()
        {
            try
            {
                var retsepts = retseptRepository.GetAllRetsepts();
                

                for (int i = 0; i < retsepts.Count; i++)
                {
                    var retsept = retsepts[i];
                    retsept.GetType().GetProperty("RowNumber")?.SetValue(retsept, i + 1);
                }
                
                dgRetsepts.ItemsSource = retsepts;
                txtRetseptCount.Text = $"Всего рецептов: {retsepts.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке рецептов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadLechenies()
        {
            try
            {
                var lechenies = lechenieRepository.GetAllLechenies();
                

                for (int i = 0; i < lechenies.Count; i++)
                {
                    var lechenie = lechenies[i];
                    lechenie.GetType().GetProperty("RowNumber")?.SetValue(lechenie, i + 1);
                }
                
                dgLechenies.ItemsSource = lechenies;
                txtLechenieCount.Text = $"Всего назначений: {lechenies.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке лечений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчики событий выбора элементов в DataGrid
        private void dgDiagnozes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedDiagnoz = dgDiagnozes.SelectedItem as DocDiagnoz;
        }

        private void dgPacients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPacient = dgPacients.SelectedItem as Pacient;
        }

        private void dgDoctors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedDoctor = dgDoctors.SelectedItem as Doctor;
        }

        private void dgLekarstva_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedLekarstvo = dgLekarstva.SelectedItem as Lekarstvo;
        }

        private void dgPriems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPriem = dgPriems.SelectedItem as Priem;
        }

        private void dgRetsepts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedRetsept = dgRetsepts.SelectedItem as Retsept;
        }

        private void dgLechenies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedLechenie = dgLechenies.SelectedItem as Lechenie;
        }

        // Методы поиска
        private void txtSearchDiagnoz_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearchDiagnoz.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadDiagnozes();
            }
            else
            {
                var diagnozes = diagnozRepository.SearchDiagnozes(searchText);
                

                for (int i = 0; i < diagnozes.Count; i++)
                {
                    var diagnoz = diagnozes[i];
                    diagnoz.GetType().GetProperty("RowNumber")?.SetValue(diagnoz, i + 1);
                }
                
                dgDiagnozes.ItemsSource = diagnozes;
                txtDiagnozCount.Text = $"Найдено диагнозов: {diagnozes.Count}";
            }
        }

        private void txtSearchPacient_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearchPacient.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadPacients();
            }
            else
            {
                var pacients = pacientRepository.SearchPacients(searchText);
                

                for (int i = 0; i < pacients.Count; i++)
                {
                    var pacient = pacients[i];
                    pacient.GetType().GetProperty("RowNumber")?.SetValue(pacient, i + 1);
                }
                
                dgPacients.ItemsSource = pacients;
                txtPacientCount.Text = $"Найдено пациентов: {pacients.Count}";
            }
        }

        private void txtSearchDoctor_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearchDoctor.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadDoctors();
            }
            else
            {
                var doctors = doctorRepository.SearchDoctors(searchText);
                

                for (int i = 0; i < doctors.Count; i++)
                {
                    var doctor = doctors[i];
                    doctor.GetType().GetProperty("RowNumber")?.SetValue(doctor, i + 1);
                }
                
                dgDoctors.ItemsSource = doctors;
                txtDoctorCount.Text = $"Найдено врачей: {doctors.Count}";
            }
        }

        private void txtSearchLekarstvo_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearchLekarstvo.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadLekarstva();
            }
            else
            {
                var lekarstva = lekarstvoRepository.SearchLekarstva(searchText);
                

                for (int i = 0; i < lekarstva.Count; i++)
                {
                    var lekarstvo = lekarstva[i];
                    lekarstvo.GetType().GetProperty("RowNumber")?.SetValue(lekarstvo, i + 1);
                }
                
                dgLekarstva.ItemsSource = lekarstva;
                txtLekarstvoCount.Text = $"Найдено лекарств: {lekarstva.Count}";
            }
        }

        private void txtSearchPriem_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearchPriem.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadPriems();
            }
            else
            {
                var priems = priemRepository.SearchPriems(searchText);
                

                for (int i = 0; i < priems.Count; i++)
                {
                    var priem = priems[i];
                    priem.GetType().GetProperty("RowNumber")?.SetValue(priem, i + 1);
                }
                
                dgPriems.ItemsSource = priems;
                txtPriemCount.Text = $"Найдено приемов: {priems.Count}";
            }
        }

        private void txtSearchRetsept_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearchRetsept.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadRetsepts();
            }
            else
            {
                var retsepts = retseptRepository.SearchRetsepts(searchText);
                

                for (int i = 0; i < retsepts.Count; i++)
                {
                    var retsept = retsepts[i];
                    retsept.GetType().GetProperty("RowNumber")?.SetValue(retsept, i + 1);
                }
                
                dgRetsepts.ItemsSource = retsepts;
                txtRetseptCount.Text = $"Найдено рецептов: {retsepts.Count}";
            }
        }

        private void txtSearchLechenie_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearchLechenie.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                LoadLechenies();
            }
            else
            {
                var lechenies = lechenieRepository.SearchLechenies(searchText);
                

                for (int i = 0; i < lechenies.Count; i++)
                {
                    var lechenie = lechenies[i];
                    lechenie.GetType().GetProperty("RowNumber")?.SetValue(lechenie, i + 1);
                }
                
                dgLechenies.ItemsSource = lechenies;
                txtLechenieCount.Text = $"Найдено назначений: {lechenies.Count}";
            }
        }

        // Обработчики кнопок очистки поиска
        private void btnClearSearchDiagnoz_Click(object sender, RoutedEventArgs e)
        {
            txtSearchDiagnoz.Clear();
            LoadDiagnozes();
        }

        private void btnClearSearchPacient_Click(object sender, RoutedEventArgs e)
        {
            txtSearchPacient.Clear();
            LoadPacients();
        }

        private void btnClearSearchDoctor_Click(object sender, RoutedEventArgs e)
        {
            txtSearchDoctor.Clear();
            LoadDoctors();
        }

        private void btnClearSearchLekarstvo_Click(object sender, RoutedEventArgs e)
        {
            txtSearchLekarstvo.Clear();
            LoadLekarstva();
        }

        private void btnClearSearchPriem_Click(object sender, RoutedEventArgs e)
        {
            txtSearchPriem.Clear();
            LoadPriems();
        }

        private void btnClearSearchRetsept_Click(object sender, RoutedEventArgs e)
        {
            txtSearchRetsept.Clear();
            LoadRetsepts();
        }

        private void btnClearSearchLechenie_Click(object sender, RoutedEventArgs e)
        {
            txtSearchLechenie.Clear();
            LoadLechenies();
        }

        // Фильтрация по дате
        private void dpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterPriemsByDate();
        }

        private void btnClearDateFilter_Click(object sender, RoutedEventArgs e)
        {
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            LoadPriems();
        }

        private void FilterPriemsByDate()
        {
            if (dpStartDate.SelectedDate.HasValue || dpEndDate.SelectedDate.HasValue)
            {
                DateTime startDate = dpStartDate.SelectedDate ?? DateTime.MinValue;
                DateTime endDate = dpEndDate.SelectedDate ?? DateTime.MaxValue;
                
                var priems = priemRepository.FilterPriemsByDate(startDate, endDate);
                

                for (int i = 0; i < priems.Count; i++)
                {
                    var priem = priems[i];
                    priem.GetType().GetProperty("RowNumber")?.SetValue(priem, i + 1);
                }
                
                dgPriems.ItemsSource = priems;
                txtPriemCount.Text = $"Найдено приемов: {priems.Count}";
            }
            else
            {
                LoadPriems();
            }
        }

        // Методы для работы с отчетами
        private void btnReportDoctorWorkload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение данных для отчета через прямой SQL-запрос
                DataTable dataTable = priemRepository.GetDoctorWorkloadStatistics();
                
                // Отображение данных в таблице
                dgReportResults.ItemsSource = dataTable.DefaultView;
                
                // Построение графика
                chart.Series.Clear();
                Series series = new Series("Нагрузка врачей");
                series.ChartType = SeriesChartType.Column;
                
                foreach (DataRow row in dataTable.Rows)
                {
                    string doctorName = row["FIO_Doctor"].ToString();
                    int appointmentCount = Convert.ToInt32(row["Count"]);
                    series.Points.AddXY(doctorName, appointmentCount);
                }
                
                chart.Series.Add(series);
                
                // Переключение на вкладку с отчетами
                tabControl.SelectedIndex = 7; // Индекс вкладки "Отчеты"
                tabReports.SelectedIndex = 1; // Индекс вкладки "График"
                
                txtStatus.Text = "Отчет по нагрузке врачей сформирован";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReportDiagnozStatistics_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение данных для отчета
                DataTable dataTable = priemRepository.GetDiagnozStatistics();
                
                // Отображение данных в таблице
                dgReportResults.ItemsSource = dataTable.DefaultView;
                
                // Построение графика
                chart.Series.Clear();
                Series series = new Series("Статистика по диагнозам");
                series.ChartType = SeriesChartType.Pie;
                
                foreach (DataRow row in dataTable.Rows)
                {
                    string diagnozName = row["Diagnoz"].ToString();
                    int appointmentCount = Convert.ToInt32(row["Count"]);
                    series.Points.AddXY(diagnozName, appointmentCount);
                }
                
                chart.Series.Add(series);
                
                // Переключение на вкладку с отчетами
                tabControl.SelectedIndex = 7; // Индекс вкладки "Отчеты"
                tabReports.SelectedIndex = 1; // Индекс вкладки "График"
                
                txtStatus.Text = "Отчет по статистике диагнозов сформирован";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnReportMedicationUsage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получение данных о статистике использования лекарств
                DataTable medicationUsageData = lechenieRepository.GetMostPrescribedMedications();

                // Отображение данных в таблице
                dgReportResults.ItemsSource = medicationUsageData.DefaultView;
                
                // Построение графика
                chart.Series.Clear();
                Series series = new Series("Количество использований");
                series.ChartType = SeriesChartType.Column;
                
                foreach (DataRow row in medicationUsageData.Rows)
                {
                    string medicationName = row["Название лекарства"].ToString();
                    int usageCount = Convert.ToInt32(row["Количество использований"]);
                    series.Points.AddXY(medicationName, usageCount);
                }
                
                chart.Series.Add(series);
                chart.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
                chart.ChartAreas[0].AxisX.Interval = 1;
                chart.ChartAreas[0].AxisY.Title = "Количество использований";
                chart.ChartAreas[0].AxisX.Title = "Лекарство";
                chart.Titles.Clear();
                chart.Titles.Add(new Title("Статистика использования лекарств", Docking.Top, new System.Drawing.Font("Arial", 14, System.Drawing.FontStyle.Bold), System.Drawing.Color.Black));
                
                // Переключение на вкладку с результатами
                tabReports.SelectedIndex = 0;
                
                txtStatus.Text = "Отчет по статистике использования лекарств сформирован";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnPrintReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Просто показываем диалог печати и печатаем текущий вид
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    // Создаем визуальный элемент для печати
                    StackPanel printPanel = new StackPanel();
                    printPanel.Width = printDialog.PrintableAreaWidth;
                    printPanel.Margin = new Thickness(20);
                    
                    // Добавляем заголовок
                    TextBlock title = new TextBlock();
                    title.Text = "Медицинский отчет";
                    if (chart != null && chart.Series.Count > 0)
                    {
                        title.Text = chart.Series[0].Name;
                    }
                    title.FontSize = 20;
                    title.FontWeight = FontWeights.Bold;
                    title.TextAlignment = TextAlignment.Center;
                    title.Margin = new Thickness(0, 0, 0, 20);
                    printPanel.Children.Add(title);
                    
                    // Добавляем дату
                    TextBlock dateText = new TextBlock();
                    dateText.Text = $"Дата: {DateTime.Now.ToString("dd.MM.yyyy HH:mm")}";
                    dateText.TextAlignment = TextAlignment.Right;
                    dateText.Margin = new Thickness(0, 0, 0, 20);
                    printPanel.Children.Add(dateText);
                    
                    // Добавляем данные из таблицы
                    TextBlock dataTitle = new TextBlock();
                    dataTitle.Text = "Данные отчета:";
                    dataTitle.FontWeight = FontWeights.Bold;
                    dataTitle.Margin = new Thickness(0, 0, 0, 10);
                    printPanel.Children.Add(dataTitle);
                    
                    // Создаем таблицу для печати
                    if (dgReportResults.Items.Count > 0)
                    {
                        try
                        {
                            // Делаем снимок таблицы
                            dgReportResults.UpdateLayout();
                            
                            // Создаем визуальный бруш для снимка
                            VisualBrush brush = new VisualBrush(dgReportResults);
                            
                            // Создаем прямоугольник с содержимым таблицы
                            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle();
                            rect.Width = dgReportResults.ActualWidth;
                            rect.Height = dgReportResults.ActualHeight;
                            rect.Fill = brush;
                            rect.Margin = new Thickness(0, 0, 0, 20);
                            
                            // Добавляем прямоугольник в панель печати
                            printPanel.Children.Add(rect);
                        }
                        catch
                        {
                            // Если не удалось сделать снимок, добавляем текстовое описание
                            TextBlock dataText = new TextBlock();
                            dataText.Text = "Данные отчета доступны в приложении";
                            dataText.Margin = new Thickness(0, 0, 0, 20);
                            printPanel.Children.Add(dataText);
                        }
                    }
                    else
                    {
                        TextBlock noData = new TextBlock();
                        noData.Text = "Нет данных для отображения";
                        noData.Margin = new Thickness(0, 0, 0, 20);
                        printPanel.Children.Add(noData);
                    }
                    
                    // Добавляем график
                    if (chart != null && chart.Series.Count > 0)
                    {
                        try
                        {
                            // Создаем заголовок для графика
                            TextBlock chartTitle = new TextBlock();
                            chartTitle.Text = "График:";
                            chartTitle.FontWeight = FontWeights.Bold;
                            chartTitle.Margin = new Thickness(0, 0, 0, 10);
                            printPanel.Children.Add(chartTitle);
                            
                            // Создаем изображение из графика
                            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(
                                chart.Width > 0 ? (int)chart.Width : 600, 
                                chart.Height > 0 ? (int)chart.Height : 300);
                            chart.DrawToBitmap(bitmap, new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height));
                            
                            // Сохраняем изображение в поток
                            using (MemoryStream ms = new MemoryStream())
                            {
                                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                ms.Position = 0;

                                // Создаем WPF изображение из потока
                                BitmapImage bitmapImage = new BitmapImage();
                                bitmapImage.BeginInit();
                                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                                bitmapImage.StreamSource = ms;
                                bitmapImage.EndInit();
                                bitmapImage.Freeze(); // Важно для предотвращения ошибок доступа к потоку

                                // Добавляем изображение
                                Image chartImage = new Image();
                                chartImage.Source = bitmapImage;
                                chartImage.Width = 500;
                                chartImage.Height = 300;
                                chartImage.Stretch = Stretch.Uniform;
                                printPanel.Children.Add(chartImage);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Если не удалось добавить график
                            TextBlock errorText = new TextBlock();
                            errorText.Text = $"Не удалось добавить график: {ex.Message}";
                            errorText.Foreground = Brushes.Red;
                            printPanel.Children.Add(errorText);
                        }
                    }
                    
                    // Создаем визуальный элемент для печати
                    FixedDocument document = new FixedDocument();
                    PageContent pageContent = new PageContent();
                    FixedPage fixedPage = new FixedPage();
                    fixedPage.Width = printDialog.PrintableAreaWidth;
                    fixedPage.Height = printDialog.PrintableAreaHeight;
                    
                    // Добавляем панель на страницу
                    fixedPage.Children.Add(printPanel);
                    ((IAddChild)pageContent).AddChild(fixedPage);
                    document.Pages.Add(pageContent);
                    
                    // Печатаем документ
                    printDialog.PrintDocument(document.DocumentPaginator, "Медицинский отчет");
                    
                    txtStatus.Text = "Отчет отправлен на печать";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при печати отчета: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        

        


        // Методы для добавления, редактирования и удаления данных
        private void btnAddDiagnoz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создание нового диагноза
                DocDiagnoz newDiagnoz = new DocDiagnoz
                {
                    Cod_Diagnoz = diagnozRepository.GetNextDiagnozId(),
                    Diagnoz = ""
                };
                
                // Отображение диалога для ввода данных
                DiagnozDialog dialog = new DiagnozDialog(newDiagnoz, true);
                if (dialog.ShowDialog() == true)
                {
                    // Добавление диагноза в базу данных
                    if (diagnozRepository.AddDiagnoz(newDiagnoz))
                    {
                        LoadDiagnozes();
                        txtStatus.Text = "Диагноз успешно добавлен";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении диагноза: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditDiagnoz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedDiagnoz != null)
                {
                    // Отображение диалога для редактирования данных
                    DiagnozDialog dialog = new DiagnozDialog(selectedDiagnoz, false);
                    if (dialog.ShowDialog() == true)
                    {
                        // Обновление диагноза в базе данных
                        if (diagnozRepository.UpdateDiagnoz(selectedDiagnoz))
                        {
                            LoadDiagnozes();
                            txtStatus.Text = "Диагноз успешно обновлен";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите диагноз для редактирования", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании диагноза: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteDiagnoz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedDiagnoz != null)
                {
                    // Подтверждение удаления
                    MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить диагноз '{selectedDiagnoz.Diagnoz}'?", 
                        "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    
                    if (result == MessageBoxResult.Yes)
                    {
                        // Удаление диагноза из базы данных
                        if (diagnozRepository.DeleteDiagnoz(selectedDiagnoz.Cod_Diagnoz))
                        {
                            LoadDiagnozes();
                            txtStatus.Text = "Диагноз успешно удален";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите диагноз для удаления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении диагноза: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddPacient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Создание нового пациента
                Pacient newPacient = new Pacient
                {
                    Cod_Pacient = pacientRepository.GetNextPacientId(),
                    FIO_Pacient = "",
                    Adress = "",
                    IDNP = "",
                    Strahovka = "",
                    Nr_Uchastka = 1
                };
                
                // Отображение диалога для ввода данных
                PacientDialog dialog = new PacientDialog(newPacient, true);
                if (dialog.ShowDialog() == true)
                {
                    // Добавление пациента в базу данных
                    if (pacientRepository.AddPacient(newPacient))
                    {
                        LoadPacients();
                        txtStatus.Text = "Пациент успешно добавлен";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении пациента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditPacient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedPacient != null)
                {
                    // Отображение диалога для редактирования данных
                    PacientDialog dialog = new PacientDialog(selectedPacient, false);
                    if (dialog.ShowDialog() == true)
                    {
                        // Обновление пациента в базе данных
                        if (pacientRepository.UpdatePacient(selectedPacient))
                        {
                            LoadPacients();
                            txtStatus.Text = "Пациент успешно обновлен";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите пациента для редактирования", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании пациента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeletePacient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedPacient != null)
                {
                    // Подтверждение удаления
                    MessageBoxResult result = MessageBox.Show($"Вы действительно хотите удалить пациента '{selectedPacient.FIO_Pacient}'?", 
                        "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    
                    if (result == MessageBoxResult.Yes)
                    {
                        // Удаление пациента из базы данных
                        if (pacientRepository.DeletePacient(selectedPacient.Cod_Pacient))
                        {
                            LoadPacients();
                            txtStatus.Text = "Пациент успешно удален";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите пациента для удаления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении пациента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Doctor Event Handlers
        private void btnAddDoctor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Doctor newDoctor = new Doctor();
                DoctorDialog dialog = new DoctorDialog(newDoctor, true);
                if (dialog.ShowDialog() == true)
                {
                    if (doctorRepository.AddDoctor(newDoctor))
                    {
                        LoadDoctors();
                        txtStatus.Text = "Врач успешно добавлен";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении врача: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditDoctor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedDoctor != null)
                {
                    Doctor doctorToEdit = new Doctor
                    {
                        Cod_Doctor = selectedDoctor.Cod_Doctor,
                        FIO_Doctor = selectedDoctor.FIO_Doctor,
                        Nr_Uchastka_DOC = selectedDoctor.Nr_Uchastka_DOC,
                        Nr_Cabinet = selectedDoctor.Nr_Cabinet
                    };

                    DoctorDialog dialog = new DoctorDialog(doctorToEdit, false);
                    if (dialog.ShowDialog() == true)
                    {
                        if (doctorRepository.UpdateDoctor(doctorToEdit))
                        {
                            LoadDoctors();
                            txtStatus.Text = "Врач успешно обновлен";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите врача для редактирования", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании врача: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteDoctor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedDoctor != null)
                {
                    if (MessageBox.Show($"Вы уверены, что хотите удалить врача {selectedDoctor.FIO_Doctor}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (doctorRepository.DeleteDoctor(selectedDoctor.Cod_Doctor))
                        {
                            LoadDoctors();
                            txtStatus.Text = "Врач успешно удален";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите врача для удаления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении врача: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Lekarstvo Event Handlers
        private void btnAddLekarstvo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Lekarstvo newLekarstvo = new Lekarstvo();
                LekarstvoDialog dialog = new LekarstvoDialog(newLekarstvo, true);
                if (dialog.ShowDialog() == true)
                {
                    if (lekarstvoRepository.AddLekarstvo(newLekarstvo))
                    {
                        LoadLekarstva();
                        txtStatus.Text = "Лекарство успешно добавлено";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении лекарства: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditLekarstvo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedLekarstvo != null)
                {
                    Lekarstvo lekarstvoToEdit = new Lekarstvo
                    {
                        Cod_Lekarstva = selectedLekarstvo.Cod_Lekarstva,
                        Name_Lekarstva = selectedLekarstvo.Name_Lekarstva,
                        Dozirovka = selectedLekarstvo.Dozirovka,
                        Type_Upakovka = selectedLekarstvo.Type_Upakovka,
                        Gruppa = selectedLekarstvo.Gruppa
                    };

                    LekarstvoDialog dialog = new LekarstvoDialog(lekarstvoToEdit, false);
                    if (dialog.ShowDialog() == true)
                    {
                        if (lekarstvoRepository.UpdateLekarstvo(lekarstvoToEdit))
                        {
                            LoadLekarstva();
                            txtStatus.Text = "Лекарство успешно обновлено";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите лекарство для редактирования", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании лекарства: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteLekarstvo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedLekarstvo != null)
                {
                    if (MessageBox.Show($"Вы уверены, что хотите удалить лекарство {selectedLekarstvo.Name_Lekarstva}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (lekarstvoRepository.DeleteLekarstvo(selectedLekarstvo.Cod_Lekarstva))
                        {
                            LoadLekarstva();
                            txtStatus.Text = "Лекарство успешно удалено";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите лекарство для удаления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении лекарства: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Priem Event Handlers
        private void btnAddPriem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Priem newPriem = new Priem
                {
                    Data_Priema = DateTime.Today
                };
                PriemDialog dialog = new PriemDialog(newPriem, true);
                if (dialog.ShowDialog() == true)
                {
                    if (priemRepository.AddPriem(newPriem))
                    {
                        LoadPriems();
                        txtStatus.Text = "Прием успешно добавлен";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении приема: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditPriem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedPriem != null)
                {
                    Priem priemToEdit = new Priem
                    {
                        Cod_Priema = selectedPriem.Cod_Priema,
                        Cod_Doctor = selectedPriem.Cod_Doctor,
                        Cod_Pacient = selectedPriem.Cod_Pacient,
                        Cod_Diagnoz = selectedPriem.Cod_Diagnoz,
                        Data_Priema = selectedPriem.Data_Priema,
                        Time_Priema = selectedPriem.Time_Priema
                    };

                    PriemDialog dialog = new PriemDialog(priemToEdit, false);
                    if (dialog.ShowDialog() == true)
                    {
                        if (priemRepository.UpdatePriem(priemToEdit))
                        {
                            LoadPriems();
                            txtStatus.Text = "Прием успешно обновлен";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите прием для редактирования", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании приема: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeletePriem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedPriem != null)
                {
                    if (MessageBox.Show($"Вы уверены, что хотите удалить прием пациента {selectedPriem.PacientName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (priemRepository.DeletePriem(selectedPriem.Cod_Priema))
                        {
                            LoadPriems();
                            txtStatus.Text = "Прием успешно удален";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите прием для удаления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении приема: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Retsept Event Handlers
        private void btnAddRetsept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Retsept newRetsept = new Retsept();
                RetseptDialog dialog = new RetseptDialog(newRetsept, true);
                if (dialog.ShowDialog() == true)
                {
                    if (retseptRepository.AddRetsept(newRetsept))
                    {
                        LoadRetsepts();
                        txtStatus.Text = "Рецепт успешно добавлен";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnEditRetsept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedRetsept != null)
                {
                    Retsept retseptToEdit = new Retsept
                    {
                        Nr_Retsepta = selectedRetsept.Nr_Retsepta,
                        Cod_Priema = selectedRetsept.Cod_Priema
                    };

                    RetseptDialog dialog = new RetseptDialog(retseptToEdit, false);
                    if (dialog.ShowDialog() == true)
                    {
                        if (retseptRepository.UpdateRetsept(retseptToEdit))
                        {
                            LoadRetsepts();
                            txtStatus.Text = "Рецепт успешно обновлен";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите рецепт для редактирования", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при редактировании рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteRetsept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedRetsept != null)
                {
                    if (MessageBox.Show($"Вы уверены, что хотите удалить рецепт №{selectedRetsept.Nr_Retsepta}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (retseptRepository.DeleteRetsept(selectedRetsept.Nr_Retsepta))
                        {
                            LoadRetsepts();
                            txtStatus.Text = "Рецепт успешно удален";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите рецепт для удаления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении рецепта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Lechenie Event Handlers
        private void btnAddLechenie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Lechenie newLechenie = new Lechenie();
                LechenieDialog dialog = new LechenieDialog(newLechenie, true);
                if (dialog.ShowDialog() == true)
                {
                    if (lechenieRepository.AddLechenie(newLechenie))
                    {
                        LoadLechenies();
                        txtStatus.Text = "Лечение успешно добавлено";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении лечения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteLechenie_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (selectedLechenie != null)
                {
                    if (MessageBox.Show($"Вы уверены, что хотите удалить лечение для пациента {selectedLechenie.PacientName}?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (lechenieRepository.DeleteLechenie(selectedLechenie.Nr_Retsepta, selectedLechenie.Cod_Lekarstva))
                        {
                            LoadLechenies();
                            txtStatus.Text = "Лечение успешно удалено";
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Выберите лечение для удаления", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении лечения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion
        
        #region User Management
        
        private void UpdateUserInfo()
        {
            if (UserSession.IsLoggedIn)
            {
                txtUserInfo.Text = $"Пользователь: {UserSession.CurrentUser.FullName} ({UserSession.CurrentUser.Role})";
                btnUserManagement.Visibility = UserSession.HasPermission(UserRole.Administrator) ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                txtUserInfo.Text = "Пользователь: Не авторизован";
                btnUserManagement.Visibility = Visibility.Collapsed;
            }
        }
        
        private void ConfigureAccessByRole()
        {
            // Настройка доступа в зависимости от роли пользователя
            if (!UserSession.IsLoggedIn) return;
            
            bool isAdmin = UserSession.HasPermission(UserRole.Administrator);
            bool isDoctor = UserSession.HasPermission(UserRole.Doctor);
            
            // Администратор имеет полный доступ ко всем функциям
            if (isAdmin) return;
            
            // Врач имеет доступ к редактированию диагнозов, приемов и рецептов
            if (isDoctor)
            {
                // Ограничиваем доступ к некоторым функциям
                btnAddDoctor.Visibility = Visibility.Collapsed;
                btnEditDoctor.Visibility = Visibility.Collapsed;
                btnDeleteDoctor.Visibility = Visibility.Collapsed;
                
                btnAddLekarstvo.Visibility = Visibility.Collapsed;
                btnEditLekarstvo.Visibility = Visibility.Collapsed;
                btnDeleteLekarstvo.Visibility = Visibility.Collapsed;
                
                return;
            }
            
            // Медсестра имеет доступ к просмотру и редактированию рецептов и лечений
            if (UserSession.CurrentUser.Role == UserRole.Nurse)
            {
                // Скрываем вкладки, к которым нет доступа
                for (int i = tabControl.Items.Count - 1; i >= 0; i--)
                {
                    TabItem tab = tabControl.Items[i] as TabItem;
                    if (tab != null)
                    {
                        string header = tab.Header.ToString();
                        if (header != "Рецепты" && header != "Лечение" && header != "Отчеты")
                        {
                            tab.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                
                // Ограничиваем доступ к кнопкам отчетов
                btnReportDoctorWorkload.IsEnabled = false;
                btnReportDiagnozStatistics.IsEnabled = false;
                
                return;
            }
            
            // Регистратор имеет доступ только к пациентам и базовым отчетам
            if (UserSession.CurrentUser.Role == UserRole.Receptionist)
            {
                // Скрываем вкладки, к которым нет доступа
                for (int i = tabControl.Items.Count - 1; i >= 0; i--)
                {
                    TabItem tab = tabControl.Items[i] as TabItem;
                    if (tab != null)
                    {
                        string header = tab.Header.ToString();
                        if (header != "Пациенты" && header != "Отчеты")
                        {
                            tab.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                
                // Ограничиваем доступ к кнопкам отчетов
                btnReportDiagnozStatistics.IsEnabled = false;
                btnReportMedicationUsage.IsEnabled = false;
            }
        }
        
        private void btnUserManagement_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Проверяем, имеет ли пользователь права администратора
                if (!UserSession.HasPermission(UserRole.Administrator))
                {
                    System.Windows.MessageBox.Show("У вас нет прав для управления пользователями", "Доступ запрещен", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                UserManagementWindow userManagementWindow = new UserManagementWindow();
                userManagementWindow.Owner = this;
                userManagementWindow.ShowDialog();
                
                // Обновляем информацию о пользователе после закрытия окна управления пользователями
                UpdateUserInfo();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при открытии окна управления пользователями: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (System.Windows.MessageBox.Show("Вы уверены, что хотите выйти из системы?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    UserSession.Logout();
                    
                    // Перезапускаем приложение для входа
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при выходе из системы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnBackupRestore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BackupWindow backupWindow = new BackupWindow();
                backupWindow.Owner = this;
                backupWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при открытии окна резервного копирования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUserGuide_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GuideWindow guideWindow = new GuideWindow();
                guideWindow.Owner = this;
                guideWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Ошибка при открытии руководства пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        #endregion
    }
}
