using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class premission_for_users
    {
        //primary key
        public int PremID { get; set; }
        //primary key//foreign key
        public int UserID { get; set; }
        public string UserName { get; set; }
        //password
        public string PWHash { get; set; }

    }
}
