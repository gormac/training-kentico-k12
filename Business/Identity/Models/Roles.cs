using System;

namespace Business.Identity.Models
{
    [Flags]
    public enum Roles
    {
        None = 1,
        Patient = 1 << 1,
        Doctor = 1 << 2
    }
}
