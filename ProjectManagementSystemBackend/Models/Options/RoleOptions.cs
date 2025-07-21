namespace ProjectManagementSystemBackend.Models.Options
{
    public class RoleOptions
    {
        public int[] OwnerRoles { get; set; } = Array.Empty<int>();
        public int[] AdminRoles { get; set; } = Array.Empty<int>();
        public int[] UserRoles { get; set; } = Array.Empty<int>();

    }
}
