using CMS.Membership;

using Business.Identity.Models;

namespace Business.Repository.Avatar
{
    public class AvatarRepository : IAvatarRepository
    {
        /// <summary>
        /// Gets a computed unique file name and a binary of a user's avatar.
        /// </summary>
        /// <param name="user">The user to find an avatar for.</param>
        /// <returns>If a user's avatar is found, named tuple with a filename and binary. Otherwise a tuple of <see langword="null"/>.</returns>
        public (string fileName, byte[] binary) GetUserAvatar(MedioClinicUser user)
        {
            var avatarInfo = AvatarInfoProvider.GetAvatarInfo(user.AvatarId);

            if (avatarInfo != null)
            {
                return ($"{avatarInfo.AvatarGUID}{avatarInfo.AvatarFileExtension}", avatarInfo.AvatarBinary);
            }

            return (null, null);
        }
        
        // TODO: Document.
        public void UploadUserAvatar(MedioClinicUser user, byte[] avatarBinary)
        {
            var avatarInfo = AvatarInfoProvider.GetAvatarInfo(user.AvatarId);

            if (avatarInfo != null)
            {
                avatarInfo.AvatarBinary = avatarBinary;
                AvatarInfoProvider.SetAvatarInfo(avatarInfo);
            }
        }

        public int CreateUserAvatar(string filePath, string avatarName)
        {
            var newAvatar = new AvatarInfo(filePath);
            newAvatar.AvatarName = avatarName ?? string.Empty;
            newAvatar.AvatarType = AvatarInfoProvider.GetAvatarTypeString(AvatarTypeEnum.User);
            newAvatar.AvatarIsCustom = true;
            AvatarInfoProvider.SetAvatarInfo(newAvatar);

            return newAvatar.AvatarID;
        }
    }
}
