-- Chạy trên SQL Server Management Studio (SSMS)
-- LƯU Ý: backup trước khi chạy trên production

-- Nếu db đã tồn tại, bỏ (chỉ dùng khi dev/test)
IF DB_ID(N'QuanLyBanHang') IS NOT NULL
BEGIN
    DROP DATABASE QuanLyBanHang;
END
GO

-- Tạo database mới
CREATE DATABASE QuanLyBanHang;
GO
USE QuanLyBanHang;
GO

/********** Tạo bảng: DanhMuc, Tag, SanPham, User, BinhLuan, HoaDon, ChiTietHoaDon, WebSetting **********/

-- Bảng DanhMuc
CREATE TABLE DanhMuc (
    MaDanhMuc INT IDENTITY(1,1) PRIMARY KEY,
    TenDanhMuc NVARCHAR(255) NOT NULL,
    TrangThai TINYINT NOT NULL DEFAULT 1
);

-- Bảng Tag
CREATE TABLE [Tag] (
    TagID INT IDENTITY(1,1) PRIMARY KEY,
    TagName NVARCHAR(100) NOT NULL
);

-- Bảng SanPham
CREATE TABLE SanPham (
    MaSP INT IDENTITY(1,1) PRIMARY KEY,
    TenSP NVARCHAR(255) NOT NULL,
    Gia DECIMAL(18,2) NOT NULL DEFAULT 0,
    GiaKM DECIMAL(18,2) NULL,
    MaDanhMuc INT NULL,
    Hinh NVARCHAR(255) NULL,
    MoTa NVARCHAR(MAX) NULL,
    TagName NVARCHAR(255) NULL,
    SoLuong INT NOT NULL DEFAULT 0,
    CONSTRAINT FK_SanPham_DanhMuc FOREIGN KEY (MaDanhMuc) REFERENCES DanhMuc(MaDanhMuc)
);

-- Bảng [User]
CREATE TABLE [User] (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(100) NOT NULL,
    [Pass] NVARCHAR(255) NOT NULL,
    TrangThai TINYINT NOT NULL DEFAULT 1,
    [Role] NVARCHAR(50) NOT NULL DEFAULT 'Customer'
);

-- Bảng BinhLuan
CREATE TABLE BinhLuan (
    BinhLuanID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NULL,
    MaSP INT NULL,
    Content NVARCHAR(MAX) NULL,
    NgayTao DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_BinhLuan_User FOREIGN KEY (UserID) REFERENCES [User](UserID),
    CONSTRAINT FK_BinhLuan_SanPham FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);

-- Bảng HoaDon (header)
CREATE TABLE HoaDon (
    MaHD INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NULL,
    TenNguoiNhan NVARCHAR(255) NULL,
    SDT NVARCHAR(20) NULL,
    DiaChi NVARCHAR(255) NULL,
    NgayLap DATETIME NOT NULL DEFAULT GETDATE(),
    TongTien DECIMAL(18,2) NOT NULL DEFAULT 0,
    TrangThai NVARCHAR(50) NOT NULL DEFAULT N'Chờ xử lý',
    CONSTRAINT FK_HoaDon_User FOREIGN KEY (UserID) REFERENCES [User](UserID)
);

-- Bảng ChiTietHoaDon (detail) - ChiTietID là identity
CREATE TABLE ChiTietHoaDon (
    ChiTietID INT IDENTITY(1,1) PRIMARY KEY,
    MaHD INT NOT NULL,
    MaSP INT NOT NULL,
    SoLuong INT NOT NULL DEFAULT 1,
    DonGia DECIMAL(18,2) NOT NULL DEFAULT 0,
    ThanhTien AS (SoLuong * DonGia) PERSISTED, -- tính toán tự động
    CONSTRAINT FK_ChiTietHoaDon_HoaDon FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD),
    CONSTRAINT FK_ChiTietHoaDon_SanPham FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);

-- Bảng WebSetting
CREATE TABLE WebSetting (
    WebSettingID INT IDENTITY(1,1) PRIMARY KEY,
    TenSite NVARCHAR(255) NULL,
    Logo NVARCHAR(255) NULL,
    DiaChi NVARCHAR(255) NULL,
    Email NVARCHAR(255) NULL,
    Hotline NVARCHAR(20) NULL,
    DungLuongToiDa INT NULL,
    RequestToiDa INT NULL
);


-- ----------------------
-- Thêm dữ liệu mẫu
-- ----------------------

-- Danh mục
INSERT INTO DanhMuc (TenDanhMuc, TrangThai) VALUES
(N'Hoa tươi', 1),
(N'Hoa khô', 1),
(N'Quà tặng', 1);

-- Tag (nếu cần)
INSERT INTO [Tag] (TagName) VALUES
('love'), ('fresh'), ('lotus'), ('orchid'), ('tulip');

-- Sản phẩm (lưu ý: không chèn MaSP - identity sẽ tự sinh)
INSERT INTO SanPham (TenSP, Gia, GiaKM, MaDanhMuc, Hinh, MoTa, TagName, SoLuong) VALUES
(N'Hoa Hồng', 200000, 180000, 1, '/images/product/01.jpg', N'Hoa hồng tượng trưng cho tình yêu', 'love,rose', 10),
(N'Hoa Cúc', 120000, 100000, 1, '/images/product/02.jpg', N'Hoa cúc mang ý nghĩa trường thọ', 'fresh,flower', 20),
(N'Hoa Sen', 150000, 130000, 1, '/images/product/03.jpg', N'Hoa sen tượng trưng cho sự thanh cao', 'lotus', 15),
(N'Hoa Lan', 250000, 220000, 1, '/images/product/04.jpg', N'Hoa lan sang trọng', 'orchid', 8),
(N'Hoa Tulip', 300000, 270000, 1, '/images/product/05.jpg', N'Hoa tulip biểu tượng', 'tulip', 5);

-- Users
INSERT INTO [User] (UserName, [Pass], TrangThai, [Role]) VALUES
('admin', '123456', 1, 'Admin'),
('staff1', '123456', 1, 'Staff'),
('user1', '123456', 1, 'Customer');

-- Tạo 2 hóa đơn mẫu và lưu MaHD vào biến để chèn chi tiết tương ứng
DECLARE @MaHD1 INT, @MaHD2 INT;

INSERT INTO HoaDon (UserID, TenNguoiNhan, SDT, DiaChi, NgayLap, TongTien, TrangThai)
VALUES (3, N'Nguyễn Văn A', '0909123456', N'Hà Nội', GETDATE(), 480000, N'Chưa thanh toán');

SET @MaHD1 = SCOPE_IDENTITY();

INSERT INTO HoaDon (UserID, TenNguoiNhan, SDT, DiaChi, NgayLap, TongTien, TrangThai)
VALUES (3, N'Lê Thị B', '0912345678', N'Hồ Chí Minh', GETDATE(), 270000, N'Đã thanh toán');

SET @MaHD2 = SCOPE_IDENTITY();

-- Thêm chi tiết hóa đơn dựa trên MaHD vừa sinh
INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia) VALUES
(@MaHD1, 1, 1, 180000),
(@MaHD1, 2, 3, 100000),
(@MaHD2, 5, 1, 270000);

-- Bình luận
INSERT INTO BinhLuan (UserID, MaSP, Content) VALUES
(3, 1, N'Hoa hồng rất đẹp và tươi'),
(3, 5, N'Tulip giao hàng nhanh, chất lượng');

-- WebSetting
INSERT INTO WebSetting (TenSite, Logo, DiaChi, Email, Hotline, DungLuongToiDa, RequestToiDa) VALUES
(N'BT3 Website', N'/uploads/logo.png', N'273 An Dương Vương, Quận 5, TP. Hồ Chí Minh', N'admin@bt3.vn', N'0909000000', 10485760, 1000);

