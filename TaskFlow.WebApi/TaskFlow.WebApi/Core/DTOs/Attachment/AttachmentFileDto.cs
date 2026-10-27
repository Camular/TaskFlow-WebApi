namespace TaskFlow.WebApi.Core.DTOs.Attachment
{
    public class AttachmentFileDto
    {
        public required byte[] Content { get; set; }
        public required string ContentType { get; set; }
        public required string OriginalFileName { get; set; }
    }
}
