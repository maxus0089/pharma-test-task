using System.Collections.Generic;
using System.Threading.Tasks;
using PharmStd.Data.Models;

namespace PharmStd.Data
{
    public interface IPostgresContext
    {
        Task CreateDrug(Drug drug);
        Task<IEnumerable<Drug>> DrugsGet();
        Task<IEnumerable<Patient>> PatientsGet();
        Task<IEnumerable<MedicalFacility>> FacilitiesGet();
        Task AddPrescription(int patientId, int drugId);
        Task AddPatient(string firstName, string lastName);
        Task AddFacility(string name, string address, string phone);
        Task AddPatientInFacility(int patientId, int facilityId);
    }
}