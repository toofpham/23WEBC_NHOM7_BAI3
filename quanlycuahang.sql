-- Tạo database và sử dụng
CREATE DATABASE QuanLyBanHang;
GO
USE QuanLyBanHang;
GO

-- Bảng DanhMuc
CREATE TABLE DanhMuc (
  MaDanhMuc INT PRIMARY KEY,
  TenDanhMuc NVARCHAR(255),
  TrangThai TINYINT
);

-- Bảng Tag
CREATE TABLE Tag (
  TagID INT PRIMARY KEY,
  TagName NVARCHAR(100)
);

-- Bảng SanPham
CREATE TABLE SanPham (
  MaSP INT PRIMARY KEY,
  TenSP NVARCHAR(255),
  Gia DECIMAL(10,2),
  GiaKM DECIMAL(10,2),
  MaDanhMuc INT,
  Hinh NVARCHAR(255),
  MoTa NVARCHAR(MAX),
  TagName NVARCHAR(100),
  FOREIGN KEY (MaDanhMuc) REFERENCES DanhMuc(MaDanhMuc)
);

-- Bảng User (Admin/Staff)
CREATE TABLE [User] (
  UserID INT PRIMARY KEY,
  UserName NVARCHAR(100),
  [Pass] NVARCHAR(255),
  TrangThai TINYINT,
  Role NVARCHAR(50)
);

-- Bảng BinhLuan
CREATE TABLE BinhLuan (
  BinhLuanID INT PRIMARY KEY,
  UserID INT,
  MaSP INT,
  Content NVARCHAR(MAX),
  NgayTao DATETIME,
  FOREIGN KEY (UserID) REFERENCES [User](UserID),
  FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);

-- Bảng HoaDon
CREATE TABLE HoaDon (
  MaHD INT PRIMARY KEY,
  UserID INT,
  TenNguoiNhan NVARCHAR(255),
  SDT NVARCHAR(20),
  DiaChi NVARCHAR(255),
  NgayLap DATETIME,
  TongTien DECIMAL(10,2),
  TrangThai NVARCHAR(50),
  FOREIGN KEY (UserID) REFERENCES [User](UserID)
);

-- Bảng ChiTietHoaDon
CREATE TABLE ChiTietHoaDon (
  ChiTietID INT PRIMARY KEY,
  MaHD INT,
  MaSP INT,
  SoLuong INT,
  DonGia DECIMAL(10,2),
  ThanhTien DECIMAL(10,2),
  FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD),
  FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);

-- Bảng WebSetting
CREATE TABLE WebSetting (
  WebSettingID INT PRIMARY KEY,
  TenSite NVARCHAR(255),
  Logo NVARCHAR(255),
  DiaChi NVARCHAR(255),
  Email NVARCHAR(255),
  Hotline NVARCHAR(20),
  DungLuongToiDa INT,
  RequestToiDa INT
);


-- Thêm Danh Mục
INSERT INTO DanhMuc (MaDanhMuc, TenDanhMuc, TrangThai) VALUES
(1, N'Hoa tươi', 1),
(2, N'Hoa khô', 1),
(3, N'Quà tặng', 1);

-- Thêm Sản Phẩm
INSERT INTO SanPham (MaSP, TenSP, Gia, GiaKM, MaDanhMuc, Hinh, MoTa, TagName) VALUES
(1, N'Hoa Hồng', 200000, 180000, 1, '/images/product/01.jpg', N'Hoa hồng tượng trưng cho tình yêu và sự lãng mạn', 'love,rose'),
(2, N'Hoa Cúc', 120000, 100000, 1, '/images/product/02.jpg', N'Hoa cúc mang ý nghĩa trường thọ và sự thanh khiết', 'fresh,flower'),
(3, N'Hoa Sen', 150000, 130000, 1, '/images/product/03.jpg', N'Hoa sen tượng trưng cho sự thanh cao và thuần khiết', 'lotus'),
(4, N'Hoa Lan', 250000, 220000, 1, '/images/product/04.jpg', N'Hoa lan sang trọng và quý phái', 'orchid'),
(5, N'Hoa Tulip', 300000, 270000, 1, '/images/product/05.jpg', N'Hoa tulip biểu tượng của sự kiêu sa và tình yêu hoàn hảo', 'tulip');

-- Thêm User
INSERT INTO [User] (UserID, UserName, [Pass], TrangThai, Role) VALUES
(1, 'admin', '123456', 1, 'Admin'),
(2, 'staff1', '123456', 1, 'Staff'),
(3, 'user1', '123456', 1, 'Customer');

-- Thêm Hóa Đơn
INSERT INTO HoaDon (MaHD, UserID, TenNguoiNhan, SDT, DiaChi, NgayLap, TongTien, TrangThai) VALUES
(1, 3, N'Nguyễn Văn A', '0909123456', N'Hà Nội', GETDATE(), 480000, N'Chưa thanh toán'),
(2, 3, N'Lê Thị B', '0912345678', N'Hồ Chí Minh', GETDATE(), 270000, N'Đã thanh toán');

-- Thêm Chi tiết hóa đơn
INSERT INTO ChiTietHoaDon (ChiTietID, MaHD, MaSP, SoLuong, DonGia, ThanhTien) VALUES
(1, 1, 1, 1, 180000, 180000),
(2, 1, 2, 3, 100000, 300000),
(3, 2, 5, 1, 270000, 270000);

-- Thêm Bình luận
INSERT INTO BinhLuan (BinhLuanID, UserID, MaSP, Content, NgayTao) VALUES
(1, 3, 1, N'Hoa hồng rất đẹp và tươi', GETDATE()),
(2, 3, 5, N'Tulip giao hàng nhanh, chất lượng', GETDATE());
ALTER TABLE SanPham ADD SoLuong INT DEFAULT 0;

INSERT INTO WebSetting (WebSettingID, TenSite, Logo, DiaChi, Email, Hotline, DungLuongToiDa, RequestToiDa)
VALUES 
(1, N'BT3 Website', N'/uploads/logo.png', N'273 An Dương Vương, Quận 5, TP. Hồ Chí Minh', 
N'admin@bt3.vn', N'0909000000', 10485760, 1000);