namespace LedConnector.Models.Dto
{
    public class MessageDto
    {
        public int Id { get; set; }
        public string RawMessage { get; set; }
        public string BinaryMessage { get; set; }
        public List<TagDto> Tags { get; set; }
    }
}
