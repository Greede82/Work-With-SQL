using System;

namespace WpfApp1.Models
{
    public class Pacient
    {
        public int Cod_Pacient { get; set; }
        public string FIO_Pacient { get; set; }
        public string Adress { get; set; }
        public string IDNP { get; set; }
        public string Strahovka { get; set; }
        public int Nr_Uchastka { get; set; }
        public int RowNumber { get; set; }

        public override string ToString()
        {
            return FIO_Pacient;
        }
    }
}
