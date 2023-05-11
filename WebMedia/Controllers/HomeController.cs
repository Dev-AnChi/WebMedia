using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using WebMedia.Models;

namespace WebMedia.Controllers
{
    public class HomeController : Controller
    {
        private WebstudioEntities db = new WebstudioEntities();

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
            return db.ChiTietGois.Include(x=>x.DichVu).ToList();
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
    }
}