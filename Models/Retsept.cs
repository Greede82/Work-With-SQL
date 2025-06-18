using System;

namespace WpfApp1.Models
{
    public class Retsept
    {
        public int Nr_Retsepta { get; set; }
        public int Cod_Priema { get; set; }
        public int RowNumber { get; set; }
        
        // Navigation property for UI display
        public string PacientName { get; set; }
        public string DiagnozName { get; set; }
    }
}
