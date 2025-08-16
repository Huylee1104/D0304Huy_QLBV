using M0304.Models.BangKeThu;

namespace M0304.Models.PhanTrang
{
    public class M0304PhanTrang
    {
        public List<M0304BangKeThu> Data { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
    }
}