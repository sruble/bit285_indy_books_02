using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndyBooks.Models
{
    public class Writer
    {
        public long Id { get; set; }
        public string Name { get; set; }

        ///A Writer can have a number of books 
        public ICollection <Book> Books {get; set; } /// Creating Collection of Book entity//named Books
    }
}
