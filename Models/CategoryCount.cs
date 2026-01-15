using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korean_Vocabulary_new.Models
{
    public class CategoryCount
    {
        public Category data { get; set; }
        public int Count { get; set; }
        public CategoryCount(Category category, int count)
        {
            data = category;
            Count = count;
        }
    }
}
