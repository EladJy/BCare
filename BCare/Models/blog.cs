using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class blog
    {
        public int PostID { get; set; }
        public int PostAuthorID { get; set; }
        public DateTime PostDate { get; set; }
        public string PostContent { get; set; }
        public string PostTitle { get; set; }
        public DateTime PostModified { get; set; }
    }
}
