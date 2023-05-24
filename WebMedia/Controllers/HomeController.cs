using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using WebMedia.Models;

namespace WebMedia.Controllers
{
    public class HomeController : Controller
    {
        private WebstudioEntities1 db = new WebstudioEntities1();

        MultiDataModels multidata = new MultiDataModels();

        // GET: ChuDes
        public ActionResult Index()
        {
            multidata.isLogin = Request.Cookies.Get("Account") == null ? false : true;
            ViewBag.islogin = multidata.isLogin;
            string message = TempData["Message"] as string;
            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Message = message;
            }

            var chudes = getChuDes();
            var albums = GetAlbums();
            var nguoidungs = GetNguoiChup();
            var goidichvu = GetGoiDichVu();
            var chitetgoi = GetChiTietGoi();

            multidata.chudes = chudes;
            multidata.albums = albums;
            multidata.nguoidungs = nguoidungs;
            multidata.goidichvus = goidichvu;
            multidata.ctgois = chitetgoi;
            return View(multidata);
        }
        public List<ChuDe> getChuDes()
        {
            return db.ChuDes.ToList();
        }
        public List<Album> GetAlbums()
        {
            return db.Albums.ToList();
        }
        public List<NguoiDung> GetNguoiChup()
        {
            return db.NguoiDungs.Where(x => x.ID_Quyen.ToString() == "nguoichup").ToList();
        }
        public List<GoiDichVu> GetGoiDichVu()
        {
            return db.GoiDichVus.ToList();
        }
        public List<ChiTietGoi> GetChiTietGoi()
        {
            return db.ChiTietGois.Include(x => x.DichVu).ToList();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string TaiKhoan, string MatKhau)
        {
            var user = db.NguoiDungs.Where(x => x.TaiKhoan == TaiKhoan).Where(x => x.MatKhau == MatKhau).ToList();
            if (user.Count >= 1)
            {
                Response.Cookies.Add(CreateAccountCookie(TaiKhoan));
                TempData["Message"] = "Đăng nhập thành công, chào " + TaiKhoan + " !";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Message"] = "Đăng nhập thất bại, kiểm tra lại tài khoản hoặc mật khẩu !";
                ViewBag.Message = "Đăng nhập thất bại, kiểm tra lại tài khoản hoặc mật khẩu !";
                return View();
            }
        }

        private HttpCookie CreateAccountCookie(string x)
        {
            HttpCookie Account = new HttpCookie("Account");
            Account.Value = x;
            return Account;
        }

        public ActionResult Logout()
        {
            HttpCookie myCookie = new HttpCookie("Account");
            myCookie.Expires = DateTime.Now.AddDays(-1d);
            Response.Cookies.Add(myCookie);
            TempData["Message"] = "Đăng xuất thành công !";
            return RedirectToAction("Index");
        }

        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy([Bind(Include = "HoTen, STD, DiaChi, AnhDaiDien, TaiKhoan, MatKhau, ID_Quyen")] NguoiDung nd)
        {
            //System.Web.HttpPostedFileBase Avatar;
            var anhdaidien = Request.Files["Avatar"];
            //Lấy thông tin từ input type=file có tên Avatar
            string postedFileName = System.IO.Path.GetFileName(anhdaidien.FileName);
            //Lưu hình đại diện về Server
            var path = Server.MapPath("/Images/Photographer/" + postedFileName);
            anhdaidien.SaveAs(path);

            nd.AnhDaiDien = postedFileName;
            nd.ID_Quyen = "khachhang";
            db.NguoiDungs.Add(nd);
            db.SaveChanges();
            return RedirectToAction("Login");
        }

        public ActionResult DatLich(int ID_Goi)
        {
            string myValue = "";
            HttpCookie myCookie = Request.Cookies["Account"];
            if (myCookie != null)
            {
                myValue = myCookie.Value;
                // Xử lý giá trị của cookie
            }

            LichChup lc = new LichChup();

            lc.ID_NguoiChup = 1;
            var nguoichup = db.NguoiDungs.SingleOrDefault(x => x.ID_NguoiDung == lc.ID_NguoiChup);
            var nguoiDung = db.NguoiDungs.SingleOrDefault(x => x.TaiKhoan == myValue);
            var goi = db.GoiDichVus.SingleOrDefault(x => x.ID_Goi == ID_Goi);
            if (nguoiDung != null)
            {
                lc.ID_KhachHang = nguoiDung.ID_NguoiDung;
                ViewBag.HoTen = nguoiDung.HoTen;
                ViewBag.TenGoi = goi.TenGoi;
            }
            lc.ID_Goi = ID_Goi;
            ViewBag.nguoichup = nguoichup.HoTen;
            ViewBag.ID_ChuDe = new SelectList(db.ChuDes, "ID_ChuDe", "TenChuDe");

            return View(lc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DatLichPost")]
        public ActionResult DatLich([Bind(Include = "ID_KhachHang, ID_NguoiChup, NgayDat, NgayChup, ID_ChuDe, Dia_Diem, TrangThaiChup, ID_Goi")] LichChup lc)
        {
            DateTime now = DateTime.Now;
            lc.NgayDat = now;
            lc.TrangThaiChup = false;
            db.LichChups.Add(lc);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult HoSo()
        {
            string myValue = "";
            HttpCookie myCookie = Request.Cookies["Account"];
            if (myCookie != null)
            {
                myValue = myCookie.Value;
                // Xử lý giá trị của cookie
            }

            var nguoiDung = db.NguoiDungs.SingleOrDefault(x => x.TaiKhoan == myValue);
            var quyen = db.Quyens.SingleOrDefault(x => x.ID_Quyen == nguoiDung.ID_Quyen);
            ViewBag.TenQuyen = quyen.TenQuyen;
            return View(nguoiDung);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HoSo([Bind(Include = "ID_NguoiDung, HoTen, STD, DiaChi, AnhDaiDien, TaiKhoan, MatKhau, ID_Quyen")] NguoiDung nd, string MatKhauInput)
        {
            var postedFile = Request.Files["Avatar"];
            if (postedFile != null && postedFile.ContentLength > 0)
            {
                // Lấy tên file ảnh mới
                var newFileName = Path.GetFileName(postedFile.FileName);

                // Lưu ảnh vào thư mục
                var path = Server.MapPath("/Images/Photographer/" + newFileName);
                postedFile.SaveAs(path);

                // Cập nhật tên file ảnh mới vào model NguoiDung
                nd.AnhDaiDien = newFileName;
            }
            else if (string.IsNullOrEmpty(postedFile.FileName))
            {
                // Nếu người dùng không chọn ảnh mới, giữ nguyên tên file ảnh cũ
                nd.AnhDaiDien = db.NguoiDungs.Where(x => x.ID_NguoiDung == nd.ID_NguoiDung).Select(x => x.AnhDaiDien).SingleOrDefault();
            }

            if (!string.IsNullOrEmpty(MatKhauInput))
            {
                nd.MatKhau = MatKhauInput;
            }

            if (ModelState.IsValid)
            {
                db.Entry(nd).State = EntityState.Modified;
                db.SaveChanges();
                TempData["EditSuccess"] = "Success !!";
                return RedirectToAction("HoSo");
            }
            TempData["EditSuccess"] = null;

            return View("HoSo", nd);
        }

        public ActionResult MyAlbum()
        {
            multidata.isLogin = Request.Cookies.Get("Account") == null ? false : true;
            ViewBag.islogin = multidata.isLogin;

            string myValue = "";
            HttpCookie myCookie = Request.Cookies["Account"];
            if (myCookie != null)
            {
                myValue = myCookie.Value;
                // Xử lý giá trị của cookie
            }

            var nguoidung = db.NguoiDungs.SingleOrDefault(x => x.TaiKhoan.ToString() == myValue);
            var myAlbum = GetAlbum(nguoidung.ID_NguoiDung);
            var listAnh = GetAllAnh();

            multidata.myAlbum = myAlbum;
            multidata.anhs = listAnh;
            
            return View(multidata);
        }

        public List<Album> GetAlbum(int id)
        {
            return db.Albums.Where(x => x.ID_NguoiChup == id).ToList();
        }
        public List<Anh> GetAllAnh()
        {
            return db.Anhs.ToList();
        }

        public ActionResult LichChup()
        {
            multidata.isLogin = Request.Cookies.Get("Account") == null ? false : true;
            ViewBag.islogin = multidata.isLogin;

            string myValue = "";
            HttpCookie myCookie = Request.Cookies["Account"];
            if (myCookie != null)
            {
                myValue = myCookie.Value;
                // Xử lý giá trị của cookie
            }

            var nguoidung = db.NguoiDungs.SingleOrDefault(x => x.TaiKhoan.ToString() == myValue);

           var lichchup = GetLichChups(nguoidung.ID_NguoiDung);

            multidata.lichChups = lichchup;
            return View(multidata);
        }
        public List<LichChup> GetLichChups(int id)
        {
            return db.LichChups.Where(x => x.ID_NguoiChup == id).ToList();
        }
    }
}