using CMS;
using CMS.DataEngine;
using CustomModules.CMSModules.DoctorAppointments;
using DoctorAppointments;

[assembly: RegisterModule(typeof(DoctorAppointmentsModule))]
namespace CustomModules.CMSModules.DoctorAppointments
{
    class DoctorAppointmentsModule : Module
    {
        public DoctorAppointmentsModule() : base("DoctorAppointments")
        {
        }

        protected override void OnInit()
        {
            base.OnInit();

            // Custom event handler executed after the appointment is created
            AppointmentInfo.TYPEINFO.Events.Insert.After += AppointmentEvents.Insert_After;
        }
    }
}
