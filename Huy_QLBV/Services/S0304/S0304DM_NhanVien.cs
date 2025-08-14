using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using M0304NhanVien.Models;

namespace S0304NhanVien.Services
{
    public class S0304NhanVienService
    {
        private readonly string _filePath;

        public S0304NhanVienService(IWebHostEnvironment env)
        {
            // Ghép đường dẫn vật lý
            _filePath = Path.Combine(env.WebRootPath, "dist", "data", "json", "W0304", "W0304Dm_NhanVien.json");
        }

        public string GetFilePath()
        {
            return _filePath;
        }

        public List<M0304NhanVienModel> GetAllNhanVien()
        {
            if (!File.Exists(_filePath))
                return new List<M0304NhanVienModel>();

            var json = File.ReadAllText(_filePath);
            return JsonConvert.DeserializeObject<List<M0304NhanVienModel>>(json);
        }
    }
}
