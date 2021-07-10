using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;

namespace BookMan.MVC.Models
{
    public class Service
    {
        private readonly string _dataFile = @"Data/data.xml";
        private readonly XmlSerializer _serializer = new XmlSerializer(typeof(HashSet<Book>));
        public HashSet<Book> Books { set; get; }

        public Service()
        {
            if (!File.Exists(_dataFile))
            {
                Books = new HashSet<Book>()
                {
                    new Book{Id = 1, Name = "ASP.NET Core for dummy", Authors = "Trump D.", Publisher = "Washington", Year = 2020},
                    new Book{Id = 2, Name = "Pro ASP.NET Core", Authors = "Putin V.", Publisher = "Moscow", Year = 2020},
                    new Book{Id = 3, Name = "ASP.NET Core video course", Authors = "Obama B.", Publisher = "Washington", Year = 2020},
                    new Book{Id = 4, Name = "Programing ASP.NET Core MVC", Authors = "Clinton B.", Publisher = "Washington", Year = 2020},
                    new Book{Id = 5, Name = "ASP.NET Core Razor Pages", Authors = "Yelstin B.", Publisher = "Moscow", Year = 2020},
                };
            }
            else
            {
                using var stream = File.OpenRead(_dataFile);
                Books = _serializer.Deserialize(stream) as HashSet<Book>;
            }
        }

        public Book[] Get() => Books.ToArray();

        public Book Get(int id) => Books.FirstOrDefault(b => b.Id == id);

        public bool Add(Book book) => Books.Add(book);

        public Book Create()
        {
            var max = Books.Max(b => b.Id);
            var b = new Book()
            {
                Id = max + 1,
                Year = DateTime.Now.Year
            };

            return b;
        }

        public bool Update(Book book)
        {
            var b = Get(book.Id);
            return b != null && Books.Remove(b) && Books.Add(book);
        }

        public bool Delete(int id)
        {
            var b = Get(id);
            return b != null && Books.Remove(b);
        }

        public void SaveChanges()
        {
            using var stream = File.Create(_dataFile);
            _serializer.Serialize(stream, Books);
        }
    }
}
