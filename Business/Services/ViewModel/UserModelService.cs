using System;
using System.Collections.Generic;
using System.Linq;

using CMS.EventLog;
using CMS.SiteProvider;
using Business.Identity.Models;

namespace Business.Services.ViewModel
{
    class UserModelService : BaseService, IUserModelService
    {
        /// <summary>
        /// Maps properties of a <see cref="MedioClinicUser"/> object to properties with the same name and type of the <paramref name="viewModelType"/> object.
        /// </summary>
        /// <param name="user">The user object to map.</param>
        /// <param name="viewModelType">The type of the output object.</param>
        /// <param name="customMappings">Custom mappings of properties with different names and/or types.</param>
        /// <returns>The <paramref name="viewModelType"/> object with mapped properties.</returns>
        public object MapToViewModel(MedioClinicUser user, Type viewModelType, Dictionary<(string propertyName, Type propertyType), object> customMappings)
        {
            var userProperties = user.GetType().GetProperties();
            var targetProperties = viewModelType.GetProperties();
            var viewModel = Activator.CreateInstance(viewModelType);

            foreach (var targetProperty in targetProperties)
            {
                var propertyToMatch = (propertyName: targetProperty.Name, propertyType: targetProperty.PropertyType);

                if (customMappings.Keys.Contains(propertyToMatch))
                {
                    try
                    {
                        targetProperty.SetValue(viewModel, customMappings[propertyToMatch]);
                    }
                    catch (Exception ex)
                    {
                        LogException(ex, nameof(MapToViewModel));
                    }
                }
                else
                {
                    var property = userProperties.FirstOrDefault(
                        prop => prop.Name.Equals(targetProperty.Name, StringComparison.OrdinalIgnoreCase)
                        && prop.PropertyType == targetProperty.PropertyType);

                    try
                    {
                        targetProperty.SetValue(viewModel, property.GetValue(user));
                    }
                    catch (Exception ex)
                    {
                        LogException(ex, nameof(MapToViewModel));
                    }
                }
            }

            return viewModel;
        }

        // TODO: Document
        public MedioClinicUser MapToMedioClinicUser(object viewModel, MedioClinicUser userToMapTo, Dictionary<(string propertyName, Type propertyType), object> customMappings)
        {
            var viewModelProperties = viewModel.GetType().GetProperties();
            var userProperties = userToMapTo.GetType().GetProperties();

            foreach (var userProperty in userProperties)
            {
                var propertyToMatch = (propertyName: userProperty.Name, propertyType: userProperty.PropertyType);

                if (customMappings.Keys.Contains(propertyToMatch))
                {
                    try
                    {
                        userProperty.SetValue(userToMapTo, customMappings[propertyToMatch]);
                    }
                    catch (Exception ex)
                    {
                        LogException(ex, nameof(MapToMedioClinicUser));
                    }
                }
                else
                {
                    var sourceProperty = viewModelProperties.FirstOrDefault(prop =>
                        prop.Name.Equals(userProperty.Name, StringComparison.OrdinalIgnoreCase)
                        && prop.PropertyType == userProperty.PropertyType);

                    if (sourceProperty != null)
                    {
                        try
                        {
                            userProperty.SetValue(userToMapTo, sourceProperty.GetValue(viewModel));
                        }
                        catch (Exception ex)
                        {
                            LogException(ex, nameof(MapToMedioClinicUser));
                        }
                    }
                }
            }

            return userToMapTo;
        }

        protected void LogException(Exception ex, string methodName) =>
            EventLogProvider.LogException(nameof(UserModelService), methodName, ex, SiteContext.CurrentSiteID);
    }
}
