using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CameraCapture.Utilities
{
    public class WritablePropertiesOnlyResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base
                .CreateProperties(type, memberSerialization)
                .Where(p => p.Writable)
                .ToList();
        }
    }
}