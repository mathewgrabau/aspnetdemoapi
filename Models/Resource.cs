using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoApi.Models
{
    public abstract class Resource : Link
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Link Self { get; set; }
    }
}
