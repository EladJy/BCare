using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class BloodTestViewModel
    {
        public int User_ID { get; set; }
        public DateTime BT_Date { get; set; }
        public String Doctor_Name { get; set; }
        public String UserGender { get; set; }
        public String IsPregnant { get; set; }
        public List<BloodTestCompnentViewModel> BTC { get; set; }
    }

    public class BloodTestCompnentViewModel
    {
        public blood_test_data btData { get; set; }
        public blood_or_additive_component BOAComp { get; set; }
    }
}
