using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ApiClient
{
    public class PersonWrapper
    {
        [JsonProperty("count")]
        public int PageCount { get; set; }
        [JsonProperty("number")]
        public int PageNumber { get; set; }
        [JsonProperty("size")]
        public int PageSize { get; set; }
        [JsonProperty("total")]
        public int TotalRecords { get; set; }

        [JsonProperty("people")]
        public IEnumerable<Person> People { get; set; }
    }
}
