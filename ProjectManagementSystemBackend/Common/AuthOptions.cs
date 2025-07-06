using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ProjectManagementSystemBackend.Common
{
    public static class AuthOptions
    {
        public readonly static string Issuer = "PMSBackend";
        readonly static string Key = "]VG#GgD6t0.x%GDc;yyz2Zi-v.En)NTBW]{5X_W!UE9O@;t/*~s73u8{xB7'";
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));

    }
}
