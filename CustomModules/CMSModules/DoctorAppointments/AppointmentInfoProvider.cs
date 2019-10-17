using System;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;

namespace DoctorAppointments
{
    /// <summary>
    /// Class providing <see cref="AppointmentInfo"/> management.
    /// </summary>
    public partial class AppointmentInfoProvider : AbstractInfoProvider<AppointmentInfo, AppointmentInfoProvider>
    {
        /// <summary>
        /// Creates an instance of <see cref="AppointmentInfoProvider"/>.
        /// </summary>
        public AppointmentInfoProvider()
            : base(AppointmentInfo.TYPEINFO)
        {
        }


        /// <summary>
        /// Returns a query for all the <see cref="AppointmentInfo"/> objects.
        /// </summary>
        public static ObjectQuery<AppointmentInfo> GetAppointments()
        {
            return ProviderObject.GetObjectQuery();
        }


        /// <summary>
        /// Returns <see cref="AppointmentInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="AppointmentInfo"/> ID.</param>
        public static AppointmentInfo GetAppointmentInfo(int id)
        {
            return ProviderObject.GetInfoById(id);
        }


        /// <summary>
        /// Returns <see cref="AppointmentInfo"/> with specified name.
        /// </summary>
        /// <param name="name"><see cref="AppointmentInfo"/> name.</param>
        public static AppointmentInfo GetAppointmentInfo(string name)
        {
            return ProviderObject.GetInfoByCodeName(name);
        }


        /// <summary>
        /// Sets (updates or inserts) specified <see cref="AppointmentInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="AppointmentInfo"/> to be set.</param>
        public static void SetAppointmentInfo(AppointmentInfo infoObj)
        {
            ProviderObject.SetInfo(infoObj);
        }


        /// <summary>
        /// Deletes specified <see cref="AppointmentInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="AppointmentInfo"/> to be deleted.</param>
        public static void DeleteAppointmentInfo(AppointmentInfo infoObj)
        {
            ProviderObject.DeleteInfo(infoObj);
        }


        /// <summary>
        /// Deletes <see cref="AppointmentInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="AppointmentInfo"/> ID.</param>
        public static void DeleteAppointmentInfo(int id)
        {
            AppointmentInfo infoObj = GetAppointmentInfo(id);
            DeleteAppointmentInfo(infoObj);
        }
    }
}