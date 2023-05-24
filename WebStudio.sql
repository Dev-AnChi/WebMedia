create database Webstudio
go
use Webstudio
go
create table Quyen(
	ID_Quyen varchar(50) primary key not null,
	TenQuyen nvarchar(100) not null
)
go
create table NguoiDung(
	ID_NguoiDung int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	HoTen nvarchar(200) not null,
	STD varchar(10),
	DiaChi varchar(250),
	AnhDaiDien varchar(50),
	TaiKhoan varchar(50),
	MatKhau varchar(250),
	ID_Quyen varchar(50) NOT NULL FOREIGN KEY REFERENCES Quyen(ID_Quyen)
)
go
create table ChuDe(
	ID_ChuDe int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	TenChuDe nvarchar(250) not null,
	NoiDung ntext,
	AnhMH nvarchar(250)
)
go
create table Album(
	ID_Album int IDENTITY(1,1) PRIMARY KEY NOT NULL,
	TenAlbum nvarchar(250) not null,
	SoLuongAnh int,
	NoiDung ntext,
	AnhBia nvarchar(250),
	ID_ChuDe int NOT NULL FOREIGN KEY REFERENCES ChuDe(ID_ChuDe),
	ID_NguoiChup int NOT NULL FOREIGN KEY REFERENCES NguoiDung(ID_NguoiDung)
)
go
create table Anh(
	ID_Anh int IDENTITY(1,1) primary key not null,
	TenAnh Nvarchar(250) not null,
	ID_Album int NOT NULL FOREIGN KEY REFERENCES Album(ID_Album)
)
go
create table GoiDichVu(
	ID_Goi int IDENTITY(1,1) primary key not null,
	TenGoi nvarchar(100),
	Gia float,
	SoLuongAnh int,
)
go
create table DichVu(
	ID_DichVu int IDENTITY(1,1) primary key not null,
	TenDichVu nvarchar(250),
)
go
create table LichChup(
	ID_LichChup int IDENTITY(1,1) primary key not null,
	ID_KhachHang int NOT NULL FOREIGN KEY REFERENCES NguoiDung(ID_NguoiDung),
	ID_NguoiChup int NOT NULL FOREIGN KEY REFERENCES NguoiDung(ID_NguoiDung),
	NgayDat datetime,
	NgayChup datetime,
	Dia_Diem nvarchar(250),
	ID_Goi int NOT NULL FOREIGN KEY REFERENCES GoiDichVu(ID_Goi),
	ID_ChuDe int NOT NULL FOREIGN KEY REFERENCES ChuDe(ID_ChuDe),
	TrangThaiChup BIT DEFAULT(0)
)
go
create table HoaDon(
	ID_HoaDon int IDENTITY(1,1) primary key not null,
	ID_LichChup int NOT NULL FOREIGN KEY REFERENCES LichChup(ID_LichChup),
	Gia float not null,
	TrangThaiThanhToan BIT DEFAULT(0)
)
go
/*
SELECT
    OBJECT_NAME(f.parent_object_id) AS LichChup,
    f.name AS foreign_key_name,
    OBJECT_NAME(f.referenced_object_id) AS referenced_table_name
FROM
    sys.foreign_keys AS f
WHERE
    f.parent_object_id = OBJECT_ID('LichChup');
*/
create table ChiTietGoi(
	ID_ChiTetGoi int IDENTITY(1,1) primary key not null,
	ID_Goi int NOT NULL FOREIGN KEY REFERENCES GoiDichVu(ID_Goi),
	ID_DichVu int NOT NULL FOREIGN KEY REFERENCES DichVu(ID_DichVu),
)
go
insert into Quyen(ID_Quyen, TenQuyen) 
values ('admin', N'Quản trị viên'),('nguoichup',N'Người chụp'),('khachhang', N'Khách hàng')
go
insert into ChuDe(TenChuDe, NoiDung, AnhMH)
values (N'Kỷ yếu', N'Mang đến những bức ảnh giá trị nhất , cam kết chất lượng trên từng khung hình', 'img1.jpg'),
		(N'Sự kiện', N'Với Đội Ngũ chất lượng nhất , Lượn Media cam kết mang đến cho khách hàng những trãi nghiệm hài lòng', '_MG_1550.JPG'),
		(N'Concept', N'Các cặp đôi có những tấm ảnh đẹp là niềm tự hào của Lượn', 'doi.JPG'),
		(N'Cá nhân', N'Lượn tự hào khi mang đến cho mỗi bạn những tấm hình đẹp', 'don.jpg')
go
insert into NguoiDung(HoTen, STD, DiaChi, AnhDaiDien, TaiKhoan, MatKhau, ID_Quyen)
values (N'Phùng Sỹ Hoàng Sơn', '0161654165',N'Nha Trang', 'son.png', 'son', '123', 'nguoichup'),
		(N'Nguyễn Phi Hùng', '064654645',N'Nha Trang', 'Hung.png', 'hung', '123', 'nguoichup'),
		(N'Anh Kiên', '098484565',N'Nha Trang', 'Kien.png', 'kien', '123', 'nguoichup')
go
insert into Album(TenAlbum, SoLuongAnh, NoiDung, AnhBia, ID_NguoiChup, ID_ChuDe)
values (N'Kỷ yếu nhóm', 2, N'Đặc biệt: khi lớp đặt lịch trước ngày 1/5/2023 sẽ nhận được nhiều phần quà TẶNG hấp dẫn đến từ LƯỢN MEDIA.',N'Nhom.JPG',1,1),
		(N'Kỷ yếu nhóm', 2, N'Tuổi trẻ mà ...... hãy tỏa sáng rực rỡ. Tôi và bạn chỉ có thể cháy cùng nhau một lần. Và chúng ta sẽ mãi mãi không bao giờ quên.',N'IMG_1922.JPG',1,1),
		(N'Kỷ yếu cặp đôi', 2, N'Bạn đã kịp lưu giữ lại điều tốt đẹp ấy chưa? Tình yêu năm 17 tuổi, đẹp đẽ và đáng yêu biết bao nhiêu.',N'doi.JPG',2,1),
		(N'Hội Trại 26.03.2023', 2, N'Dư âm Hội Trại 26.03.2023 THPT Nguyễn Huệ Nguyễn Huệ ơiii ....... !!!',N'Trai1.jpg',2,2),
		(N'Hội Chợ Truyền Thống', 2, N'Tan chảy với sự đáng yêu này của THPT Nguyễn Trãi - kỷ niệm 2023 đáng để nhớ về!',N'HoiCho1.jpg',2,2),
		(N'Chào mừng ngày 26-3', 2, N'Một chút đáng yêu bất ngờ đến từ các quý thầy cô. Sự xuất hiện bất ngờ của các thầy cô trên sàn CATWALK.',N'SuKien1.jpg',3,2)
go
insert into GoiDichVu(TenGoi, Gia, SoLuongAnh)
values (N'Đồng', 69000, 100),
		(N'Bạc', 500000, 1000),
		(N'Vàng', 800000, 1500)
go
insert into DichVu(TenDichVu)
values (N'Không được thêm'),
		(N'Gợi ý concept'),
		(N'Phục vụ tốt'),
		(N'Makeup')
go
insert into ChiTietGoi(ID_Goi, ID_DichVu)
values (1,1),(1,1),(1,1),(2,2),(2,3),(2,1),(3,2),(3,3),(3,4)
go
insert into Anh(TenAnh, ID_Album)
values ('Nhom.JPG', 1), ('IMG_1922.JPG',1), ('doi.JPG', 2), ('Trai1.jpg', 2),('doi.JPG', 1),
('Trai1.JPG', 1),('Nhom.JPG', 1),('IMG_1922.JPG', 1), ('Trai1.jpg', 2),('doi.JPG', 2),('IMG_1922.JPG', 2)
go
insert into LichChup(ID_Khachhang, ID_NguoiChup, NgayDat, NgayChup, Dia_Diem, ID_Goi, ID_ChuDe, TrangThaiChup)
values (3,1,CAST('2022-4-19' AS DATE),CAST('2022-4-21' AS DATE), 'Nha Trang', 1,1,0),
		(2,1,CAST('2022-4-20' AS DATE),CAST('2022-4-21' AS DATE), 'Nha Trang', 1,1,0)