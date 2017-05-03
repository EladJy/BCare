using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class premission_type

    {

        //primary key//foreign key
        public int PremID { get; set; }
        public PremissionName PremissionName { get; set; }
    }

    public enum PremissionName
    {
        Anonym,
        User,
        Doctor,
        Developer,
        Admin
        
    };
}
