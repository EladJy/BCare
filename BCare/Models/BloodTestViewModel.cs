using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class BloodTestViewModel
    {
        public User user { get; set; }
        public blood_test bloodTest { get; set; }
        public blood_test_data btData { get; set; }
        public blood_or_additive_component BOAComp { get; set; }
    }
}
