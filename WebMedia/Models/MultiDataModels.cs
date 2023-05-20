using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMedia.Models
{
    public class MultiDataModels
    {
        public bool isLogin { get; set; }
        public List<ChuDe> chudes { get; set; }
        public List<Album> albums { get; set; }
        public List<NguoiDung> nguoidungs { get; set; }
        public List<GoiDichVu> goidichvus { get; set; }
        public List<ChiTietGoi> ctgois { get; set; }
        public List<Album> myAlbum { get; set; }
        public List<Anh> anhs { get; set; }  
    }
}