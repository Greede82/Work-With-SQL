using System;

namespace WpfApp1.Models
{
    public class Priem
    {
        public int Cod_Priema { get; set; }
        public int Cod_Doctor { get; set; }
        public int Cod_Pacient { get; set; }
        public int Cod_Diagnoz { get; set; }
        public DateTime Data_Priema { get; set; }
        public decimal Time_Priema { get; set; }
        public int RowNumber { get; set; }
        
        // Navigation properties for UI display
        public string DoctorName { get; set; }
        public string PacientName { get; set; }
        public string DiagnozName { get; set; }
    }
}
