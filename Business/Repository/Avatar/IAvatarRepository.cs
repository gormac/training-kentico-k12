using System;

using Business.Identity.Models;

namespace Business.Repository.Avatar
{
    // TODO: Document.
    public interface IAvatarRepository : IRepository
    {
        (string fileName, byte[] binary) GetUserAvatar(MedioClinicUser user);

        void UploadUserAvatar(MedioClinicUser user, byte[] avatarBinary);
    }
}