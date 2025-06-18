using System;

namespace WpfApp1.Models
{
    public class Doctor
    {
        public int Cod_Doctor { get; set; }
        public string FIO_Doctor { get; set; }
        public int Nr_Uchastka_DOC { get; set; }
        public int Nr_Cabinet { get; set; }
        public int RowNumber { get; set; }

        public override string ToString()
        {
            return FIO_Doctor;
        }
    }
}
