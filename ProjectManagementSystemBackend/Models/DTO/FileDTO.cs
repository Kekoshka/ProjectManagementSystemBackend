namespace ProjectManagementSystemBackend.Models.DTO
{
    public class FileDTO
    {
        public byte[] FileContent { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
