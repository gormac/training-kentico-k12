using System;
using System.Collections.Generic;

using Business.Identity.Models;

namespace Business.Services.ViewModel
{
    // TODO: Document.
    public interface IUserModelService : IService
    {
        object MapToViewModel(MedioClinicUser user, Type targetViewModelType, Dictionary<(string propertyName, Type propertyType), object> customMappings);

        MedioClinicUser MapToMedioClinicUser(object viewModel, MedioClinicUser userToMapTo, Dictionary<(string propertyName, Type propertyType), object> customMappings);
    }
}
