using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IndyBooks.Models;
using IndyBooks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndyBooks.Controllers
{
    public class AdminController : Controller
    {
        private IndyBooksDataContext _db;
        public AdminController(IndyBooksDataContext db) { _db = db; }

        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel search)
        {
            //Full Collection Search
            IQueryable<Book> foundBooks = _db.Books; // start with entire collection
            // // //// // // seems good 

            if (search.Title == null)
            {
                    foundBooks = foundBooks
                    .Include(b => b.Author) //Inlcude the author in every block// or page wont display
                  ;

            }

            //Partial Title Search
            //seems good
            if (search.Title != null)
            {
                foundBooks = foundBooks
                    .Include(b => b.Author)
                             .Where(b => b.Title.Contains(search.Title))
                             .OrderBy(b => b.Title)
                             ;
            }

            //Author's Last Name Search
            ///seems good
            if (search.AuthorLastName != null)
            {
                //TODO:Update to use the Name property of the Book's Author entity///added 
                foundBooks = foundBooks
                             .Include(b => b.Author)
                             .Where(b => b.Author.Name.EndsWith(search.AuthorLastName, StringComparison.CurrentCulture))
                           

                             ;
            }
            //Priced Between Search (min and max price entered)
            // //  seems good 
            if (search.MinPrice > 0 && search.MaxPrice > 0)
            {
                foundBooks = foundBooks
                            // .Include(b => b.Author) //does placement for .Include() matter?
                            .Where(b => b.Price >= search.MinPrice && b.Price <= search.MaxPrice) //filter out books to 100 to 150
                            .Include(b => b.Author)// include the author                       
                            .Select(b => new Book { Author = b.Author})/// Create a new book to Hold the Author Info, While Eliminating theduplicate Prices //
                            .Distinct() //Filter the top results even further by ensuring One of each Author name is Displayed 
                        
                             ;
            }
            //Highest Priced Book Search (only max price entered) 
            // // // this works// do not alter
            if (search.MinPrice == 0 && search.MaxPrice > 0)
            {
                decimal max = _db.Books.Max(b => b.Price);

                foundBooks = foundBooks
                    .Include(b => b.Author)
                    .Where(b => b.Price ==  max) ///added this line //pulls max priced book in DB
                             ;

            }
            //Composite Search Results
            return View("SearchResults", foundBooks);
        }
    }
}
