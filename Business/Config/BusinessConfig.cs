namespace Business.Config
{
    public static class BusinessConfig
    {
        public const string DoctorRoleName = "Doctor";
        public const string PatientRoleName = "Patient";
        public const string AuthenticatedUserNames = DoctorRoleName + ", " + PatientRoleName;
    }
}
