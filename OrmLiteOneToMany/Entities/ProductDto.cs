using System;
using System.Collections.Generic;
using System.Text;

namespace OrmLiteOneToMany.Entities
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string EanNumber { get; set; }
        public string Title { get; set; }

        public List<EBook> Ebooks { get; set; }
    }
}
