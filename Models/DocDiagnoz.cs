using System;

namespace WpfApp1.Models
{
    public class DocDiagnoz
    {
        public int Cod_Diagnoz { get; set; }
        public string Diagnoz { get; set; }
        public int RowNumber { get; set; }

        public override string ToString()
        {
            return Diagnoz;
        }
    }
}
