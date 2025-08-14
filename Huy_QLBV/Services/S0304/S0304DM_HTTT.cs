using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using M0304HTTT.Models;

namespace S0304HTTT.Services
{
    public class S0304HTTTService
    {
        private readonly string _filePath;

        public S0304HTTTService(IWebHostEnvironment env)
        {
            // Ghép đường dẫn vật lý tới file
            _filePath = Path.Combine(env.WebRootPath, "dist", "data", "json", "W0304", "W0304DM_HTTT.json");
        }

        public string GetFilePath()
        {
            return _filePath;
        }

        public List<M0304DM_HTTT> GetAllHTTT()
        {
            if (!File.Exists(_filePath))
                return new List<M0304DM_HTTT>();

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<M0304DM_HTTT>>(json);
        }
    }
}
