using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Kamina.BL.Models
{
    public class SelectionContainer<T>
    {
        [JsonProperty("count")]
        public Int32 Count { get; set; }

        [JsonProperty("skip")]
        public Int32 Skip { get; set; }

        [JsonProperty("items")]
        public List<T> Items { get; set; }
        
        public SelectionContainer()
        {
            Items = new List<T>();
        }
    }
}
