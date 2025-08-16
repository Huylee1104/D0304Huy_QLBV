using M0304HTTT.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace S0304HTTT.Services
{
    public class S0304HTTTService
    {
        private readonly string _filePath;

        public S0304HTTTService(IWebHostEnvironment env)
        {
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
            var htttList = JsonConvert.DeserializeObject<List<M0304DM_HTTT>>(json);

            return htttList.OrderBy(httt => httt.ten).ToList();
        }
    }
}
