using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Bogus;
using OrmLiteOneToMany.Entities;
using ServiceStack.OrmLite;

namespace OrmLiteOneToMany
{
    public class Seed
    {
        public static void RunSeed(IDbConnection db)
        {
            db.DropAndCreateTable<Book>();
            db.DropAndCreateTable<EBook>();

            //db.CreateTable<Book>();
            //db.CreateTable<EBook>();
            var books = new List<Book>();
            var eBooks = new List<EBook>();
            Random rnd = new Random();
            if (db.Count<Book>() == 0)
            {
                var faker = new Faker<Book>()
                    .RuleFor(x => x.EanNumber, f=> f.Commerce.Ean13())
                    .RuleFor(x => x.Title, f => f.Commerce.ProductName());
                books = faker.Generate(100);
                db.SaveAll(books);
            }
            else
            {
                books = db.Select<Book>();
            }

            if(db.Count<EBook>() == 0)
            {
                var faker = new Faker<EBook>()
                    .RuleFor(x => x.EAN, f => f.Commerce.Ean13())
                    .RuleFor(x => x.Format, f => f.PickRandom<EBookFormat>())
                    .RuleFor(x => x.Title, f => f.Commerce.ProductName());


                foreach(var book in books)
                {
                    if (book.Id % 2 == 0) continue;

                    var relatedEBooks = faker.Generate(rnd.Next(1, 5));
                    relatedEBooks.ForEach(e => e.PhysicalBookEan = book.EanNumber);

                    eBooks.AddRange(relatedEBooks);
                }

                db.SaveAll(eBooks);
            }
        }


    }
}
