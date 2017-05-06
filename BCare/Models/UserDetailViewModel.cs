using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class UserDetailViewModel
    {
        public User user { get; set; }
        public premission_for_users pfu { get; set; }
        public health_maintenance_organizations hmo { get; set; }
        public Boolean isDoctor { get; set; }
    }
}
