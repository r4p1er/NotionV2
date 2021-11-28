using Newtonsoft.Json;

namespace NotionV2.API.DTOs
{
    public class UserDTO
    {
        [JsonRequired]
        public string Login { get; set; }
        
        [JsonRequired]
        public string Password { get; set; }
    }
}