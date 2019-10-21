using System;
using System.Collections.Generic;

namespace DoctorAppointments
{
    public partial class DoctorInfoProvider
    {
        public static int SetDoctors(List<DoctorInfo> doctors)
        {
            int count = 0;

            foreach (DoctorInfo doctor in doctors)
            {
                DoctorInfo doc = GetDoctorInfo(doctor.DoctorCodeName);
                if (doc == null)
                {
                    SetDoctorInfo(doctor);
                }
                else
                {
                    doc.DoctorFirstName = doctor.DoctorFirstName;
                    doc.DoctorLastName = doctor.DoctorLastName;
                    doc.DoctorEmail = doctor.DoctorEmail;
                    doc.DoctorLastModified = DateTime.Now;
                    SetDoctorInfo(doc);
                    count++;
                }
            }

            return count;
        }
    }
}
