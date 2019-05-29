using System;
using System.Collections.Generic;
using System.Linq;

using Business.Identity.Models;

namespace Business.Services.ViewModel
{
    class UserModelService : BaseService, IUserModelService
    {
        public object MapToViewModel(
            MedioClinicUser user,
            Type targetViewModelType,
            Dictionary<(string propertyName, Type propertyType), object> customMappings = null)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (targetViewModelType == null)
            {
                throw new ArgumentNullException(nameof(targetViewModelType));
            }

            var userProperties = user.GetType().GetProperties();
            var targetProperties = targetViewModelType.GetProperties();
            var viewModel = Activator.CreateInstance(targetViewModelType);

            foreach (var targetProperty in targetProperties)
            {
                var propertyToMatch = (propertyName: targetProperty.Name, propertyType: targetProperty.PropertyType);

                var sourceProperty = userProperties.FirstOrDefault(
                    prop => prop.Name.Equals(targetProperty.Name, StringComparison.OrdinalIgnoreCase)
                    && prop.PropertyType == targetProperty.PropertyType);

                if (customMappings != null && customMappings.Keys.Contains(propertyToMatch))
                {
                    targetProperty.SetValue(viewModel, customMappings[propertyToMatch]);
                }
                else if (sourceProperty != null)
                {
                    targetProperty.SetValue(viewModel, sourceProperty.GetValue(user));
                }
            }

            return viewModel;
        }

        public MedioClinicUser MapToMedioClinicUser(
            object viewModel,
            MedioClinicUser userToMapTo,
            Dictionary<(string propertyName, Type propertyType), object> customMappings = null)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (userToMapTo == null)
            {
                throw new ArgumentNullException(nameof(userToMapTo));
            }

            var viewModelProperties = viewModel.GetType().GetProperties();
            var userProperties = userToMapTo.GetType().GetProperties();

            foreach (var userProperty in userProperties)
            {
                var propertyToMatch = (propertyName: userProperty.Name, propertyType: userProperty.PropertyType);

                var sourceProperty = viewModelProperties.FirstOrDefault(prop =>
                    prop.Name.Equals(userProperty.Name, StringComparison.OrdinalIgnoreCase)
                    && prop.PropertyType == userProperty.PropertyType);

                if (customMappings != null && customMappings.Keys.Contains(propertyToMatch))
                {
                    userProperty.SetValue(userToMapTo, customMappings[propertyToMatch]);
                }
                else if (sourceProperty != null)
                {
                    userProperty.SetValue(userToMapTo, sourceProperty.GetValue(viewModel));
                }
            }

            return userToMapTo;
        }
    }
}
