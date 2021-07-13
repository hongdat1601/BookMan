using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http;
using System.Linq.Dynamic.Core;

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

        public string GetDataPath(string file) => $"Data/{file}";

        public void Upload(Book book, IFormFile file)
        {
            if(file != null)
            {
                var path = GetDataPath(file.FileName);
                using var stream = new FileStream(path, FileMode.Create);
                file.CopyTo(stream);
                book.DataFile = file.FileName;
            }
        }

        public (Stream, string) Download(Book b)
        {
            var memory = new MemoryStream();
            using var stream = new FileStream(GetDataPath(b.DataFile), FileMode.Open);
            stream.CopyTo(memory);
            memory.Position = 0;
            var type = Path.GetExtension(b.DataFile) switch
            {
                "pdf" => "application/pdf",
                "docx" => "application/vnd.ms-word",
                "doc" => "application/vnd.ms-word",
                "txt" => "text/plain",
                _ => "application/pdf"
            };

            return (memory, type);
        }

        public Book[] Get(string search)
        {
            if (string.IsNullOrEmpty(search)) return Books.ToArray();

            var s = search.ToLower();
            return Books.Where(b =>
                b.Name.ToLower().Contains(s) ||
                b.Authors.ToLower().Contains(s) ||
                b.Publisher.ToLower().Contains(s) ||
                (string.IsNullOrEmpty(b.Description) ? false : b.Description.Contains(s)) ||
                b.Year.ToString() == s
            ).ToArray();
        }

        public (Book[] books, int pages, int page) Panging(int page, string orderBy="Name", bool dsc = false)
        {
            int size = 5;
            int pages = (int)Math.Ceiling(Books.Count * 1.0 / size);
            var books = Books.Skip((page - 1) * size).Take(size).AsQueryable().OrderBy($"{orderBy} {(dsc ? "descending" : "")}").ToArray();

            return (books, pages, page);
        }
    }
}
