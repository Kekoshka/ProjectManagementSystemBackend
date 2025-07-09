namespace ProjectManagementSystemBackend.Interfaces
{
    public interface IPasswordHasherService
    {
        string Hash(string password);
        bool Verify(string text, string hash);
    }
}
