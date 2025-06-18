# Medical Institution Management Application (WPF)

## Overview
This is a desktop application for managing data in a medical institution, built with Windows Presentation Foundation (WPF). The application is designed to streamline the management of diagnoses, patients, doctors, medicines, appointments, and prescriptions. It also features a comprehensive reporting module for statistical analysis and workload management.

## Features
- **Data Management**: CRUD operations for patients, doctors, diagnoses, medicines, appointments, and prescriptions.
- **Reporting Module**:
  - Doctor workload report
  - Diagnosis statistics
  - Medicine statistics
  - Reports can be viewed as tables (DataGrid) and charts (using `System.Windows.Forms.DataVisualization.Charting`).
  - Integrated print functionality: print both the table and chart on a single page using `System.Windows.Documents`, `System.Windows.Xps`, and `System.Printing`.
- **Database**:
  - Local SQL Server database named `Dispanserizatsia`.
  - Automatic database creation if it does not exist.
  - Automatic creation of required tables and stored procedures (with existence checks).
- **Backup & Restore**: Database backup and restore functions (see `Database/DbBackupManager.cs`).
- **User Authentication**: User management and authentication.

## Technology Stack
- **Frontend**: WPF (C#)
- **Database**: Microsoft SQL Server (LocalDB)
- **Data Access**: ADO.NET, SQL stored procedures
- **Reporting**: DataGrid, Chart (System.Windows.Forms.DataVisualization.Charting)
- **Printing**: System.Windows.Documents, System.Windows.Xps, System.Printing

## Project Structure
```
WpfApp1/
├── Installer/                # Installer files
├── work_with_sql/
│   └── WpfApp1/
│       └── work_with_sql/
│           ├── App.config
│           ├── App.xaml
│           ├── App.xaml.cs
│           ├── Database/    # Database logic, backup/restore, scripts
│           ├── Models/      # Data models
│           ├── Views/       # UI views (windows, dialogs)
```

## Database Setup
- The application uses a local SQL Server database named `Dispanserizatsia`.
- On startup, the application checks if the database exists and creates it if necessary.
- Table creation scripts use `IF NOT EXISTS` to prevent duplicate primary key errors.
- The structure of the `Doctor` table has been corrected (fields: `Nr_Uchastka_DOC`, `Nr_Cabinet`).
- Stored procedures `GetDoctorWorkload` and `GetDiagnosisStatistics` are created automatically if missing.
- User table existence is checked before creation to avoid conflicts.

## Reports Module
The application provides three types of reports:
1. **Doctor Workload Report**: Shows the workload distribution among doctors.
2. **Diagnosis Statistics**: Displays statistics on diagnoses across patients.
3. **Medication Statistics**: Presents data on medication usage and inventory.

Each report can be viewed as a DataGrid table and as a chart. Both views are combined for printing.

## Printing Functionality
- Reports can be printed with both the table and chart on the same page.
- Utilizes WPF document and printing APIs for high-quality printouts.
- Printing is accessible from the reports module.

## How to Run
1. **Requirements:**
   - Windows OS
   - .NET Framework (compatible with WPF)
   - SQL Server LocalDB installed

2. **Build and Run:**
   - Open the solution in Visual Studio.
   - Restore NuGet packages if prompted.
   - Build the solution (`Ctrl+Shift+B`).
   - Run the application (`F5`).

3. **First Launch:**
   - On first launch, the application will automatically create the database and tables if they do not exist.

## Troubleshooting
- **Database Connection Issues:**
  - Ensure SQL Server LocalDB is installed and running.
  - Check connection string in the configuration file if you encounter errors.
- **Printing Issues:**
  - Ensure your printer is properly installed and configured.
- **Missing Charts:**
  - Make sure the `System.Windows.Forms.DataVisualization` assembly is referenced in the project.

## License
This project is provided for educational and internal use in medical institutions. Please contact the author for other use cases.

---

If you have any questions or encounter issues, please open an issue or contact the maintainer.
