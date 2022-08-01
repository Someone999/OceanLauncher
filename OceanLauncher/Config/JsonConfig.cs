using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OceanLauncher.Config.Converters;

namespace OceanLauncher.Config.Json
{
    public class JsonConfig : IConfigElement, ICanSaveConfigElement
    {
        private string _path;
        private Dictionary<string, object> _config;

        public JsonConfig(string path)
        {
            _config = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(_path = path));
        }

        public object GetValue() => _config;

        public T GetValue<T>() => typeof(T) != typeof(JObject)
            ? throw new InvalidCastException()
            : (T)(object)_config;
       

        public IConfigElement this[string key]
        {
            get => new DefaultConfigElement(_config[key]);
            set => _config[key] = value.GetValue();

        }

        public T ConvertTo<T>(IConfigConverter<T> converter)
        {
            throw new NotSupportedException();
        }
        
        public bool ContainsKey(string key)
        {
            return _config.ContainsKey(key);
        }
        
        public void Save()
        {
            string json = JsonConvert.SerializeObject(_config);
            File.WriteAllText(_path, json);
        }
    }
}