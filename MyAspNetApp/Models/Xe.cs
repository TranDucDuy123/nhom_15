using System.ComponentModel.DataAnnotations;

namespace MyAspNetApp.Models
{
    public class Xe
    {
        [Key]  // Định nghĩa Maxe là khóa chính
        public string Maxe { get; set; }
        public string Mohinh { get; set; }
        public string Nhasanxuat { get; set; }
        public DateTime Namsanxuat { get; set; }
        public decimal Gia { get; set; }
        public int Soxecotrongkho { get; set; }
    }
}
