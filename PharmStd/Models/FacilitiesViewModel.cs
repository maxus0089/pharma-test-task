using System.Collections.Generic;
using PharmStd.Data;

namespace PharmStd.Models
{
    public class FacilitiesViewModel
    {
        public IEnumerable<MedicalFacility> Facilities { get; set; }
        public IEnumerable<MedicalFacility> AllFacilities{ get; set; }
        public IEnumerable<Patient> Patients{ get; set; }
    }
}