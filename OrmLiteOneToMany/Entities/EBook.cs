using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrmLiteOneToMany.Entities
{
    public class EBook
    {
        [AutoIncrement]
        public int Id { get; set; }
        [Index(Unique = true)]
        [Alias("ean")]
        public string EAN { get; set; }
        public string Title { get; set; }
        public EBookFormat Format { get; set; }

        public string PhysicalBookEan { get; set; }
    }

    public enum EBookFormat
    {
        PDF = 1,
        Mp3,
        Doc
    }
}
