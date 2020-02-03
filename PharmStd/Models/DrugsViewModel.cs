using System.Collections.Generic;
using PharmStd.Data;
using PharmStd.Data.Models;

namespace PharmStd.Models
{
    public class  PatientsViewModel
    {
        public IEnumerable<Drug> Drugs { get; set; }
        public IEnumerable<Patient> Patients { get; set; }
        public IEnumerable<Patient> AllPatients { get; set; }

    }
}