using System;
using System.Data;
using System.Runtime.Serialization;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using DoctorAppointments;

[assembly: RegisterObjectType(typeof(AppointmentInfo), AppointmentInfo.OBJECT_TYPE)]

namespace DoctorAppointments
{
    /// <summary>
    /// Data container class for <see cref="AppointmentInfo"/>.
    /// </summary>
    [Serializable]
    public partial class AppointmentInfo : AbstractInfo<AppointmentInfo>
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "doctorappointments.appointment";


        /// <summary>
        /// Type information.
        /// </summary>
#warning "You will need to configure the type info."
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(AppointmentInfoProvider), OBJECT_TYPE, "DoctorAppointments.Appointment", "AppointmentsID", "AppointmentsLastModified", "AppointmentsGuid", "AppointmentPatientFirstName", null, null, null, null, null)
        {
            ModuleName = "DoctorAppointments",
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// Appointments ID.
        /// </summary>
        [DatabaseField]
        public virtual int AppointmentsID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("AppointmentsID"), 0);
            }
            set
            {
                SetValue("AppointmentsID", value);
            }
        }


        /// <summary>
        /// Appointment patient first name.
        /// </summary>
        [DatabaseField]
        public virtual string AppointmentPatientFirstName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("AppointmentPatientFirstName"), String.Empty);
            }
            set
            {
                SetValue("AppointmentPatientFirstName", value);
            }
        }


        /// <summary>
        /// Appointment patient last name.
        /// </summary>
        [DatabaseField]
        public virtual string AppointmentPatientLastName
        {
            get
            {
                return ValidationHelper.GetString(GetValue("AppointmentPatientLastName"), String.Empty);
            }
            set
            {
                SetValue("AppointmentPatientLastName", value);
            }
        }


        /// <summary>
        /// Appointment patient email.
        /// </summary>
        [DatabaseField]
        public virtual string AppointmentPatientEmail
        {
            get
            {
                return ValidationHelper.GetString(GetValue("AppointmentPatientEmail"), String.Empty);
            }
            set
            {
                SetValue("AppointmentPatientEmail", value);
            }
        }


        /// <summary>
        /// Appointment patient phone number.
        /// </summary>
        [DatabaseField]
        public virtual string AppointmentPatientPhoneNumber
        {
            get
            {
                return ValidationHelper.GetString(GetValue("AppointmentPatientPhoneNumber"), String.Empty);
            }
            set
            {
                SetValue("AppointmentPatientPhoneNumber", value, String.Empty);
            }
        }


        /// <summary>
        /// Appointment date.
        /// </summary>
        [DatabaseField]
        public virtual DateTime AppointmentDate
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("AppointmentDate"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("AppointmentDate", value, DateTimeHelper.ZERO_TIME);
            }
        }


        /// <summary>
        /// Appointment doctor ID.
        /// </summary>
        [DatabaseField]
        public virtual int AppointmentDoctorID
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("AppointmentDoctorID"), 0);
            }
            set
            {
                SetValue("AppointmentDoctorID", value);
            }
        }


        /// <summary>
        /// Appointment patient birth date.
        /// </summary>
        [DatabaseField]
        public virtual DateTime AppointmentPatientBirthDate
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("AppointmentPatientBirthDate"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("AppointmentPatientBirthDate", value, DateTimeHelper.ZERO_TIME);
            }
        }


        /// <summary>
        /// Appointments guid.
        /// </summary>
        [DatabaseField]
        public virtual Guid AppointmentsGuid
        {
            get
            {
                return ValidationHelper.GetGuid(GetValue("AppointmentsGuid"), Guid.Empty);
            }
            set
            {
                SetValue("AppointmentsGuid", value);
            }
        }


        /// <summary>
        /// Appointments last modified.
        /// </summary>
        [DatabaseField]
        public virtual DateTime AppointmentsLastModified
        {
            get
            {
                return ValidationHelper.GetDateTime(GetValue("AppointmentsLastModified"), DateTimeHelper.ZERO_TIME);
            }
            set
            {
                SetValue("AppointmentsLastModified", value);
            }
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            AppointmentInfoProvider.DeleteAppointmentInfo(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            AppointmentInfoProvider.SetAppointmentInfo(this);
        }


        /// <summary>
        /// Constructor for de-serialization.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Streaming context.</param>
        protected AppointmentInfo(SerializationInfo info, StreamingContext context)
            : base(info, context, TYPEINFO)
        {
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="AppointmentInfo"/> class.
        /// </summary>
        public AppointmentInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="AppointmentInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public AppointmentInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}