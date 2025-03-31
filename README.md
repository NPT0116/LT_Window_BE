# 📱 Phone Selling POS - Hệ thống quản lý bán hàng cửa hàng điện thoại

Ứng dụng bao gồm:

- ✅ Backend chạy bằng Docker
- ✅ Ứng dụng Desktop (.msix) cài trực tiếp trên Windows
- ✅ Quản lý sản phẩm, tồn kho, khách hàng, hóa đơn...

---

## 🚀 1. Chuẩn bị môi trường

### ⚙️ Yêu cầu:

- Docker + Docker Compose đã cài sẵn
- Windows 10 trở lên để cài `.msix`
- Powershell (Run as Administrator nếu cần)

---

## 🐳 2. Setup Backend bằng Docker

### 🔁 Bước 1: Clone mã nguồn back end

```bash
git clone https://github.com/NPT0116/LT_Window_BE.git
cd LT_Window_BE
```

````

### 🔧 Bước 2: Build & Run

```bash
docker-compose up --build
```

> 📌 Mặc định backend sẽ chạy ở địa chỉ: `http://localhost:5142`

---

## 💻 3. Cài đặt ứng dụng desktop (.msix)

### 📦 Bước 1: Giải nén file cài đặt

Giải nén file:

```
PhoneSellingPos.zip
```

Bên trong sẽ có:

- `PhoneSelling.Presentation_1.0.3.0_x64_Debug.msix`
- `PhoneSelling.Presentation_1.0.3.0_x64_Debug.cer`

---

### 🔐 Bước 2: Cài đặt chứng chỉ xác thực (chỉ làm 1 lần)

1. Click chuột phải vào file `.cer` → **Install Certificate**
2. Chọn:
   - `Local Machine` → **Next**
   - `Place all certificates in the following store` → chọn **Trusted Root Certification Authorities**
3. Bấm **Finish** để hoàn tất

> 🔒 Mục đích: giúp Windows tin tưởng chứng chỉ để cài đặt ứng dụng.


### 📥 Bước 3: Cài đặt ứng dụng

1. Nhấn đúp chuột vào file:

```
PhoneSelling.Presentation_1.0.3.0_x64_Debug.msix
```
- Sau khi nhấn đúp vào thì sẽ có cài đặt install ứng dụng
2. Làm theo hướng dẫn để hoàn tất cài đặt.

---

## ✅ 4. Sử dụng ứng dụng

- Mở app từ **Start Menu**
- Đảm bảo backend đã chạy (`docker-compose up`)
- Đăng nhập với tài khoản:
    Tài khoản: admin
    Mật khẩu: 123
- App có thể sử dụng các tính năng: bán hàng, quản lý sản phẩm, khách hàng, hóa đơn, tồn kho...

---


## 📞 Liên hệ & hỗ trợ

Mọi thắc mắc xin liên hệ nhóm phát triển:
📧 Email: `thanh1612004@gmail.com`

---

**Made with ❤️ by LT_Window Team**

```
````
