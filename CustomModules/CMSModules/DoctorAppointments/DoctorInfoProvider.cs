using System;
using System.Data;

using CMS.Base;
using CMS.DataEngine;
using CMS.Helpers;

namespace DoctorAppointments
{
    /// <summary>
    /// Class providing <see cref="DoctorInfo"/> management.
    /// </summary>
    public partial class DoctorInfoProvider : AbstractInfoProvider<DoctorInfo, DoctorInfoProvider>
    {
        /// <summary>
        /// Creates an instance of <see cref="DoctorInfoProvider"/>.
        /// </summary>
        public DoctorInfoProvider()
            : base(DoctorInfo.TYPEINFO)
        {
        }


        /// <summary>
        /// Returns a query for all the <see cref="DoctorInfo"/> objects.
        /// </summary>
        public static ObjectQuery<DoctorInfo> GetDoctors()
        {
            return ProviderObject.GetObjectQuery();
        }


        /// <summary>
        /// Returns <see cref="DoctorInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="DoctorInfo"/> ID.</param>
        public static DoctorInfo GetDoctorInfo(int id)
        {
            return ProviderObject.GetInfoById(id);
        }


        /// <summary>
        /// Returns <see cref="DoctorInfo"/> with specified name.
        /// </summary>
        /// <param name="name"><see cref="DoctorInfo"/> name.</param>
        public static DoctorInfo GetDoctorInfo(string name)
        {
            return ProviderObject.GetInfoByCodeName(name);
        }


        /// <summary>
        /// Sets (updates or inserts) specified <see cref="DoctorInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="DoctorInfo"/> to be set.</param>
        public static void SetDoctorInfo(DoctorInfo infoObj)
        {
            ProviderObject.SetInfo(infoObj);
        }


        /// <summary>
        /// Deletes specified <see cref="DoctorInfo"/>.
        /// </summary>
        /// <param name="infoObj"><see cref="DoctorInfo"/> to be deleted.</param>
        public static void DeleteDoctorInfo(DoctorInfo infoObj)
        {
            ProviderObject.DeleteInfo(infoObj);
        }


        /// <summary>
        /// Deletes <see cref="DoctorInfo"/> with specified ID.
        /// </summary>
        /// <param name="id"><see cref="DoctorInfo"/> ID.</param>
        public static void DeleteDoctorInfo(int id)
        {
            DoctorInfo infoObj = GetDoctorInfo(id);
            DeleteDoctorInfo(infoObj);
        }
    }
}