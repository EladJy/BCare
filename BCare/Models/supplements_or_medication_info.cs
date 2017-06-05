using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class supplements_or_medication_info
    {
        //primary key
        public int SomID { get; set; }
        //foreign key
        public int PharmID { get; set; }
        public string SOMName { get; set; }
        public int ServingAmountInBox { get; set; }
        public ServingType ServingFormType { get; set; }
        public MeasurementUnit ServingFormUnit { get; set; }
        public InHealthPlan InHealthPlan { get; set; }
        public WithMedicalPrescription WithMedicalPrescription { get; set; }
        public string MoreInformation { get; set; }
        public string ProductImageURL { get; set; }
    }

    public enum ServingType
    {
        Liquid,
        Powder,
        Pill,
        Tablets,
        Spray
    }
    public enum InHealthPlan
    {
        Y,
        N
    }
    public enum WithMedicalPrescription
    {
        Y,
        N
    }
}
