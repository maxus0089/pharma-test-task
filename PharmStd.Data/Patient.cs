namespace PharmStd.Data
{
    public class Patient
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Drug[] PrescribedDrugs { get; set; }
    }
}