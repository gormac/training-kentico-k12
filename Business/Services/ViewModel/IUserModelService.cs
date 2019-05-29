using System;
using System.Collections.Generic;

using Business.Identity.Models;

namespace Business.Services.ViewModel
{
    /// <summary>
    /// Custom mapper between user models and user view models.
    /// </summary>
    public interface IUserModelService : IService
    {
        /// <summary>
        /// Maps properties of a <see cref="MedioClinicUser"/> object to properties with the same name and type in the <paramref name="viewModelType"/> object.
        /// </summary>
        /// <param name="user">The source user object.</param>
        /// <param name="targetViewModelType">The type of the output object.</param>
        /// <param name="customMappings">Custom mappings of properties with different names and/or types.</param>
        /// <returns>The <paramref name="viewModelType"/> object with mapped properties.</returns>
        object MapToViewModel(MedioClinicUser user, Type targetViewModelType, Dictionary<(string propertyName, Type propertyType), object> customMappings = null);

        /// <summary>
        /// Maps properties of the <paramref name="viewModel"/> object to properties of the same name and type in the <see cref="MedioClinicUser"/> object.
        /// </summary>
        /// <param name="viewModel">The source view model object.</param>
        /// <param name="userToMapTo">The target user object.</param>
        /// <param name="customMappings">Custom mappings of properties with different names and/or types.</param>
        /// <returns>The mapped <see cref="MedioClinicUser"/> object.</returns>
        /// <remarks>Maps properties by reference, does not copy them by value.</remarks>
        MedioClinicUser MapToMedioClinicUser(object viewModel, MedioClinicUser userToMapTo, Dictionary<(string propertyName, Type propertyType), object> customMappings = null);
    }
}
