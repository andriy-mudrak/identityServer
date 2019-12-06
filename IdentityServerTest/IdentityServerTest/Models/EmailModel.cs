using System.Collections.Generic;
using Newtonsoft.Json;

namespace AuthServer.Models
{
    public class EmailModel
    {
        public List<Handle> elements { get; set; }
    }

    public class Handle
    {
        [JsonProperty(PropertyName = "handle~")]
        public EmailAddress handleTilde { get; set; }
        public string handle { get; set; }

    }

    public class EmailAddress
    {
        public string emailAddress { get; set; }
    }
}