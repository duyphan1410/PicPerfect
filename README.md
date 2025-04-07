# PicPerfect

PicPerfect là một ứng dụng web được phát triển bằng ASP.NET Core 8.0, cho phép người dùng quản lý và chia sẻ hình ảnh một cách hiệu quả.

## Tính năng chính

- Quản lý hình ảnh
- Chia sẻ hình ảnh
- Xác thực người dùng
- Lưu trữ hình ảnh trên Cloudinary

## Công nghệ sử dụng

- ASP.NET Core 8.0
- Entity Framework Core
- SQLite
- Bootstrap
- Cloudinary

## Yêu cầu hệ thống

- .NET 8.0 SDK
- Visual Studio 2022 hoặc Visual Studio Code
- SQLite

## Cài đặt và chạy

1. Clone repository:

```bash
git clone [URL repository]
```

2. Cài đặt các package cần thiết:

```bash
dotnet restore
```

3. Chạy migrations:

```bash
dotnet ef database update
```

4. Chạy ứng dụng:

```bash
dotnet run
```

## Cấu trúc dự án

- `Controllers/`: Chứa các controller xử lý request
- `Models/`: Chứa các model dữ liệu
- `Views/`: Chứa các view Razor
- `Services/`: Chứa các service xử lý business logic
- `Interface/`: Chứa các interface
- `wwwroot/`: Chứa các file tĩnh (CSS, JavaScript, hình ảnh)
- `Migrations/`: Chứa các file migration của Entity Framework

## Đóng góp

Mọi đóng góp đều được hoan nghênh! Vui lòng tạo pull request hoặc issue để đóng góp vào dự án.

## Giấy phép

[MIT License](LICENSE)
