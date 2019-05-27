using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Business.Identity.Models;

using CMS.Membership;

namespace Business.Repository.Avatar
{
    public class AvatarRepository : IAvatarRepository
    {
        public (string fileName, byte[] binary) GetUserAvatar(MedioClinicUser user)
        {
            var avatarInfo = AvatarInfoProvider.GetAvatarInfo(user.AvatarId);

            return ($"{avatarInfo.AvatarGUID}{avatarInfo.AvatarFileExtension}", avatarInfo.AvatarBinary);
        }

        public void UploadUserAvatar(MedioClinicUser user, byte[] avatarBinary)
        {
            var avatarInfo = AvatarInfoProvider.GetAvatarInfo(user.AvatarId);
            avatarInfo.AvatarBinary = avatarBinary;
            AvatarInfoProvider.SetAvatarInfo(avatarInfo);
        }
    }
}
