using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrmLiteOneToMany.Entities
{
    public class Book
    {
        [AutoIncrement]
        public int Id { get; set; }
        public string EanNumber { get; set; }
        public string Title { get; set; }
    }
}
