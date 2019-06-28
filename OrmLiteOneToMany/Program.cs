using NUnit.Framework;
using OrmLiteOneToMany.Entities;
using ServiceStack;
using ServiceStack.DataAnnotations;
using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrmLiteOneToMany
{
    class Program
    {
        static void Main(string[] args)
        {
            #region setup
            //run "docker-compose up" for db
            var connectionFactory = new OrmLiteConnectionFactory("Server=localhost;Port=5321;Database=ormtest;User Id=ormtest;Password=password;", PostgreSqlDialect.Provider);

            var db = connectionFactory.Open();

            Seed.RunSeed(db);
            #endregion

            //I need to populate ProductDto with a nested list of ebooks.  Not all books have ebooks
            // so I used a left join.  I understand that OrmLite has the [Reference] feature but it's not 
            // always possible to use it easily when working on large data set with lots of relationships
            var q = db.From<Book>()
                .LeftJoin<Book, EBook>((b, e) => b.EanNumber == e.PhysicalBookEan);

            //doing a normal select multi gives me a collection I can condense
            var books = db.SelectMulti<Book, EBook>(
                    q.Select<Book, EBook>((b, e) => new { b, e })
                );

            var response = CondenseBooksTuple(books);

            //This works fine if we are using the entire data set but what about
            // if your front end has to page the data?

            var q2 = q.Skip(20).Limit(20);

            var books2 = db.SelectMulti<Book, EBook>(
                    q.Select<Book, EBook>((b, e) => new { b, e })
                );

            var response2 = CondenseBooksTuple(books2);

            // This doesn't work as it will break the paging.  For paging to work these must be true
            //Assert.AreEqual(response2.Count(), 20); //frontend has asked for 20 records, we should respond with 20
            //Assert.AreEqual(response2[0].Id, 21); //we have skipped first 20 products, ID should be 21

            // OrmLite has a GroupBy method but I cant get it to work in any meaningful way.
            // If I group the EBooks it will give and error. What I need is something like this

            //var q3 = "select b.*, array_agg(e) ebooks from book b " +
            //         "left join e_book e on e.physical_book_ean = b.ean_number " +
            //         "group by b.id";

            //var books3 = db.SqlList<object>(q3);

            //I commented it out because it doesn't work.  If I run this direct in postgresql then I will get a
            //result set that has the book record with a nested array of ebooks and the paging will match properly

            //There is no array_agg mthod I can see but even the Sql.Count() doesn't seem to work

            //var q4 = db.From<Book>()
            //    .LeftJoin<Book, EBook>((b, e) => b.EanNumber == e.PhysicalBookEan)
            //    .GroupBy(x => x.Id)
            //    .Select<Book, EBook>((b,e) => new { b, EbookCount = Sql.Count(e) });


            //var books4 = db.SelectMulti<Book, EBook>(q4); //exception because no aggregate method used

            //My main goal is to be able to use AutoQuery with joined tables and return all the data.  My workaround
            // at the moment is very poor performance and involves me looping over every result to get related items.
            // this adds a massive overhead meaning over 100 queries for one API request.

            

        }

        private static List<ProductDto> CondenseBooksTuple(List<Tuple<Book,EBook>> books)
        {
            var booksMap = new Dictionary<int, ProductDto>();

            books.ForEach(b =>
            {
                if (!booksMap.TryGetValue(b.Item1.Id, out var dto))
                {
                    booksMap[b.Item1.Id] = dto = b.Item1.ConvertTo<ProductDto>();
                }
                if (dto.Ebooks == null) dto.Ebooks = new List<EBook>();

                if (b.Item2.Id > 0) dto.Ebooks.Add(b.Item2);
            });

            var response = booksMap.Values.ToList();

            return response;
        }
    }


}
