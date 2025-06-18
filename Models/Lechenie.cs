using System;

namespace WpfApp1.Models
{
    public class Lechenie
    {
        public int Cod_Lekarstva { get; set; }
        public int Nr_Retsepta { get; set; }
        public int RowNumber { get; set; }
        
        // Navigation properties for UI display
        public string LekarstvoName { get; set; }
        public string PacientName { get; set; }
    }
}
