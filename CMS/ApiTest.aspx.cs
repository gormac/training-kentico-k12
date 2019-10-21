using DoctorAppointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CMSApp
{
    public partial class ApiTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ApiTestBtn_Click(object sender, EventArgs e)
        {
            DoctorInfo di = new DoctorInfo();
            di.DoctorCodeName = "JillD";
            di.DoctorFirstName = "Jilly";
            di.DoctorLastName = "Doe";
            di.DoctorEmail = "Jill.Doe@nowhere.local";
            di.DoctorSpecialty = "Pediatrics";
            di.DoctorLastModified = DateTime.Now;

            List<DoctorInfo> doctors = new List<DoctorInfo>
            {
                di
            };

            DoctorInfoProvider.SetDoctors(doctors);

            Response.Write(di.DoctorFirstName + " " + di.DoctorLastName + " was added or modified.");
        }
    }
}