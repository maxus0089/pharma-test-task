using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace PharmStd.Data
{
    public class PostgresContext
    {
        private NpgsqlConnection Connection =>
            new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=postgres;User Id=root;Password=123qweasd;");
       

        public async Task CreateDrug(Drug drug)
        {
            await using NpgsqlConnection cnn =
                Connection;
            
            await cnn.OpenAsync();
            try
            {
                await cnn.ExecuteAsync(
                    "INSERT INTO pharm.drugs ( name, pharmgroup, activecomponent, atxcode, category) VALUES (@name,@pharmgroup,@activecomponent,@atxcode,@category) ON CONFLICT DO NOTHING",
                    new
                    {
                        name = drug.Name, pharmgroup = drug.PharmGroup, activecomponent = drug.ActiveComponent,
                        atxcode = drug.AtxCode, category = drug.Category
                    });
            }
            finally
            {
                cnn.Close();
            }
        }

        public async Task<IEnumerable<Drug>> DrugsGet()
        {
            await using NpgsqlConnection cnn = Connection;
            try
            {
                await cnn.OpenAsync();
                return await cnn.QueryAsync<Drug>(
                    "SELECT * FROM pharm.drugs ");
            }
            finally
            {
                cnn.Close();
            }
        }

        public async Task<IEnumerable<Patient>> PatientsGet()
        {
            await using NpgsqlConnection cnn = Connection;
            try
            {
                await cnn.OpenAsync();
                var patients = (await cnn.QueryAsync<Patient>("SELECT * FROM pharm.patients ")).ToArray();

                foreach (var patient in patients)
                {
                    patient.PrescribedDrugs = (await cnn.QueryAsync<Drug>(
                        @"SELECT * FROM pharm.prescribeddrugs RIGHT JOIN pharm.drugs ON 
                    pharm.prescribeddrugs.drugid = pharm.drugs.drugid  WHERE patientid = @patientid",
                        new {patientid = patient.PatientId})).ToArray();
                }

                return patients;
            }
            finally
            {
                cnn.Close();
            }
        }

        public async Task<IEnumerable<MedicalFacility>> FacilitiesGet()
        {
            await using NpgsqlConnection cnn = Connection;
            try
            {
                await cnn.OpenAsync();
                var facilities = (await cnn.QueryAsync<MedicalFacility>("SELECT * FROM pharm.medicalfacilities "))
                    .ToArray();

                foreach (var facility in facilities)
                {
                    facility.Patients = (await cnn.QueryAsync<Patient>(
                        @"SELECT * FROM pharm.patientsinfacilities RIGHT JOIN pharm.patients ON 
                    pharm.patientsinfacilities.patientid = pharm.patients.patientid  WHERE facilityid= @facilityid",
                        new {facilityid = facility.FacilityId})).ToArray();
                    foreach (var patient in facility.Patients)
                    {
                        patient.PrescribedDrugs = (await cnn.QueryAsync<Drug>(
                            @"SELECT * FROM pharm.prescribeddrugs RIGHT JOIN pharm.drugs ON 
                    pharm.prescribeddrugs.drugid = pharm.drugs.drugid  WHERE patientid = @patientid",
                            new {patientid = patient.PatientId})).ToArray();
                    }
                }

                return facilities;
            }
            finally
            {
                cnn.Close();
            }
        }

        public async Task AddPrescription(int patientId, int drugId)
        {
            await using NpgsqlConnection cnn =
                Connection;
            await cnn.OpenAsync();
            try
            {
                await cnn.ExecuteAsync(
                    "INSERT INTO pharm.prescribeddrugs (patientid, drugid) VALUES (@patientId,@drugId) ON CONFLICT DO NOTHING",
                    new
                    {
                        patientId,drugId
                    });
            }
            finally
            {
                cnn.Close();
            }
        }

        public async Task AddPatient(string firstName, string lastName)
        {
            await using NpgsqlConnection cnn =
                Connection;
            await cnn.OpenAsync();
            try
            {
                await cnn.ExecuteAsync(
                    "INSERT INTO pharm.patients (firstname, lastname) VALUES (@firstName,@lastName) ON CONFLICT DO NOTHING;",
                    new
                    {
                        firstName,lastName
                    });
            }
            finally
            {
                cnn.Close();
            }
        }

        public async Task AddFacility(string name, string address, string phone)
        {
            await using NpgsqlConnection cnn =
                Connection;
            await cnn.OpenAsync();
            try
            {
                await cnn.ExecuteAsync(
                    "INSERT INTO pharm.medicalfacilities (name, address,phonenumber) VALUES (@name,@address,@phone)",
                    new
                    {
                        name,address,phone
                    });
            }
            finally
            {
                cnn.Close();
            }
        }

        public async Task AddPatientInFacility(int patientId, int facilityId)
        {
            await using NpgsqlConnection cnn =
                Connection;
            await cnn.OpenAsync();
            try
            {
                await cnn.ExecuteAsync(
                    "INSERT INTO pharm.patientsinfacilities (facilityid, patientid) VALUES (@fid,@pid);",
                    new
                    {
                        pid=patientId,fid=facilityId
                    });
            }
            finally
            {
                cnn.Close();
            }
        }
    }
}