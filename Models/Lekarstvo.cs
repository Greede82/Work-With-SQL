using System;

namespace WpfApp1.Models
{
    public class Lekarstvo
    {
        public int Cod_Lekarstva { get; set; }
        public string Name_Lekarstva { get; set; }
        public int Dozirovka { get; set; }
        public string Type_Upakovka { get; set; }
        public string Gruppa { get; set; }
        public int RowNumber { get; set; }

        public override string ToString()
        {
            return Name_Lekarstva;
        }
    }
}
