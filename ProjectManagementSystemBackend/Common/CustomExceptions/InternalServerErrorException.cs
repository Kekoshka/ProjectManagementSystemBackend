namespace ProjectManagementSystemBackend.Common.CustomExceptions
{
    public class InternalServerErrorException : Exception
    {
        public InternalServerErrorException() { }
        public InternalServerErrorException(string message) : base(message) { }
    }
}
