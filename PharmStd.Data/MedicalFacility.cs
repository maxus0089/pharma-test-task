using System;

namespace PharmStd.Data
{
    public class MedicalFacility
    {
        public int FacilityId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public Patient[] Patients { get; set; }
    }

    
}