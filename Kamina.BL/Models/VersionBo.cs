using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kamina.BL.Models
{
    public class VersionBo
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        
        [JsonProperty("versionNumber")]
        public Int32 VersionNumber { get; set; }

        [JsonProperty("versionSize")]
        public Int64 VersionSize { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }
    }
}
