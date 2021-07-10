using System;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BookMan.MVC.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required, DisplayName("Tiêu đề")]
        public string Name { get; set; }
        [Required, DisplayName("Tác giả")]
        public string Authors { get; set; }
        [Required, DisplayName("Nhà xuất bản")]
        public string Publisher { get; set; }
        [Required, DisplayName("Năm xuất bản"), Range(1990, int.MaxValue)]
        public int Year { get; set; }
        [DisplayName("Tóm tắt")]
        public string Description { get; set; }
        [DisplayName("File")]
        public string DataFile { get; set; }

    }
}
