//Copyright(c) 2021 Alexandra Kravtsova (aleksylum)

using Newtonsoft.Json;
using System;

namespace Meditation.Data
{
    public static class Serializer
    {
        public static String Serialize<T>(T value) where T : class
        {
	        return JsonConvert.SerializeObject(value);
        }

        public static T Deserialize<T>(String obj) where T : class
        {
	        return JsonConvert.DeserializeObject<T>(obj);
        }
    }
}
