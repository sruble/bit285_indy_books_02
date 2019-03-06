﻿using System;
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
            // // //// // //  below block is new 

            if((search.Title == null) && (search.MinPrice > 0 && search.MaxPrice > 0) && (search.AuthorLastName == null))
            {
                _db.Books.Include(b => b.Author.Name) //I don't understand why this won't display the Authors Name


                    ;
                //Have tried below
                //.Include(b => b.Author)
                //.Include(w => w.Author)
                //.Include(b => b.Author.Name);
            }

            //Partial Title Search
            if (search.Title != null)
            {
                foundBooks = foundBooks
                             .Where(b => b.Title.Contains(search.Title))
                             .OrderBy(b => b.Title)
                             ;
            }

            //Author's Last Name Search
            if (search.AuthorLastName != null)
            {
                //TODO:Update to use the Name property of the Book's Author entity///added 
                foundBooks = foundBooks
                               //.Include(b => b.Author.Name)
                                //.Include(b => b.Author.Name == StringComparison.CurrentCulture)
                             .Where(b => b.Author.Name.EndsWith(search.AuthorLastName, StringComparison.CurrentCulture))
                           

                             ;
            }
            //Priced Between Search (min and max price entered)
            // //  
            if (search.MinPrice > 0 && search.MaxPrice > 0)
            {
                foundBooks = foundBooks
                             //.Include(b => b.Author) //does for .Include matter?
                            .Where(b => b.Price >= search.MinPrice && b.Price <= search.MaxPrice) //filter out books to 100 to 150
                            .Include(b => b.Author)// include the author                       
                           // .Select(b => new Book { Author = b.Author})/// Create a new book to Hold the Author Info, While Eliminating theduplicate Prices //
                            //.Distinct() //Filter the top results even further by ensuring One of each Author name is Displayed 
                        
                             ;
            }
            //Highest Priced Book Search (only max price entered) 
            // // // this works// do not alter
            if (search.MinPrice == 0 && search.MaxPrice > 0)
            {
                decimal max = _db.Books.Max(b => b.Price);

                foundBooks = foundBooks
                    .Where(b => b.Price ==  max) ///added this line //pulls max priced book in DB
                             ;

            }
            //Composite Search Results
            return View("SearchResults", foundBooks);
        }
    }
}
