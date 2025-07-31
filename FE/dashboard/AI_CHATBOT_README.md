# AI Chatbot - Hệ thống đặt lịch khám bệnh viện

## 📋 Mô tả

AI Chatbot là một hệ thống trí tuệ nhân tạo được thiết kế để hỗ trợ bệnh nhân đặt lịch khám bệnh một cách tự động và thân thiện. Chatbot có khả năng hiểu và xử lý ngôn ngữ tự nhiên bằng tiếng Việt.

## ✨ Tính năng chính

### 🤖 AI Assistant
- **Xử lý ngôn ngữ tự nhiên**: Hiểu và phản hồi bằng tiếng Việt
- **Nhận diện ý định**: Tự động phân loại yêu cầu của người dùng
- **Hội thoại thông minh**: Duy trì ngữ cảnh cuộc trò chuyện
- **Phản hồi nhanh**: Giao diện với nút trả lời nhanh

### 📅 Quản lý lịch hẹn
- **Đặt lịch khám**: Hướng dẫn từng bước đặt lịch
- **Kiểm tra lịch hẹn**: Xem lịch hẹn hiện tại
- **Hủy lịch hẹn**: Hủy lịch hẹn không cần thiết
- **Xác nhận thông tin**: Kiểm tra và xác nhận thông tin trước khi đặt

### 🏥 Thông tin bệnh viện
- **Thông tin liên hệ**: Địa chỉ, số điện thoại, email
- **Giờ làm việc**: Lịch làm việc các ngày trong tuần
- **Hướng dẫn quy trình**: Quy trình khám bệnh chi tiết
- **Phương tiện di chuyển**: Thông tin xe buýt, bãi đỗ xe

## 🚀 Cách sử dụng

### 1. Truy cập chatbot
Mở file `ai-chatbot.html` trong trình duyệt web.

### 2. Tương tác với chatbot
- **Nhập tin nhắn**: Gõ tin nhắn vào ô nhập liệu
- **Sử dụng nút nhanh**: Chọn các thao tác nhanh bên trái
- **Trả lời nhanh**: Click vào các nút trả lời nhanh

### 3. Đặt lịch khám
1. Chọn "Đặt lịch khám" hoặc gõ "tôi muốn đặt lịch"
2. Cung cấp họ tên
3. Nhập số điện thoại
4. Chọn khoa khám
5. Chọn ngày khám (định dạng DD/MM/YYYY)
6. Chọn giờ khám
7. Xác nhận thông tin

### 4. Kiểm tra lịch hẹn
- Cung cấp số điện thoại hoặc mã lịch hẹn
- Xem thông tin lịch hẹn hiện tại

### 5. Hủy lịch hẹn
- Cung cấp mã lịch hẹn cần hủy
- Xác nhận hủy lịch hẹn

## 🛠️ Cấu trúc dự án

```
FE/dashboard/
├── ai-chatbot.html              # Giao diện chính
├── assets/
│   ├── css/
│   │   └── ai-chatbot.css       # Styles cho chatbot
│   └── js/
│       └── ai-chatbot.js        # Logic xử lý AI
└── AI_CHATBOT_README.md         # Hướng dẫn sử dụng
```

## 🔧 Tích hợp API

### API Endpoints cần thiết:

#### 1. Tạo lịch hẹn
```javascript
POST /api/appointments
{
  "patientName": "string",
  "phoneNumber": "string", 
  "department": "string",
  "appointmentDate": "string",
  "appointmentTime": "string"
}
```

#### 2. Lấy danh sách lịch hẹn
```javascript
GET /api/appointments?phone={phoneNumber}
GET /api/appointments?code={appointmentCode}
```

#### 3. Hủy lịch hẹn
```javascript
PUT /api/appointments/{appointmentCode}/cancel
```

### Cách tích hợp:
1. Thay thế các hàm mock API trong `ai-chatbot.js`
2. Cập nhật URL endpoint trong các hàm API
3. Xử lý authentication nếu cần

## 🎨 Tùy chỉnh giao diện

### Thay đổi màu sắc:
Chỉnh sửa biến CSS trong `ai-chatbot.css`:
```css
:root {
    --primary-color: #007bff;    /* Màu chủ đạo */
    --secondary-color: #6c757d;  /* Màu phụ */
    --success-color: #28a745;    /* Màu thành công */
    --warning-color: #ffc107;    /* Màu cảnh báo */
    --danger-color: #dc3545;     /* Màu lỗi */
}
```

### Thay đổi thông tin bệnh viện:
Cập nhật thông tin trong hàm `handleHospitalInfo()` trong `ai-chatbot.js`.

## 📱 Responsive Design

Chatbot được thiết kế responsive và hoạt động tốt trên:
- Desktop (PC, Laptop)
- Tablet
- Mobile (Điện thoại)

## 🔒 Bảo mật

- Dữ liệu chat được lưu locally trong localStorage
- Không lưu trữ thông tin nhạy cảm
- Có thể xuất và xóa lịch sử chat

## 🚀 Tính năng nâng cao

### 1. Machine Learning
- Có thể tích hợp với các model AI như GPT, BERT
- Cải thiện khả năng hiểu ngôn ngữ tự nhiên
- Học từ lịch sử tương tác

### 2. Tích hợp đa nền tảng
- Facebook Messenger
- Telegram Bot
- WhatsApp Business API
- Website chat widget

### 3. Analytics và báo cáo
- Thống kê số lượng đặt lịch
- Phân tích hành vi người dùng
- Báo cáo hiệu suất chatbot

## 🐛 Xử lý lỗi

### Lỗi thường gặp:
1. **Không kết nối được API**: Kiểm tra URL endpoint
2. **Lỗi hiển thị**: Kiểm tra console browser
3. **Không lưu được chat**: Kiểm tra localStorage

### Debug:
- Mở Developer Tools (F12)
- Xem tab Console để kiểm tra lỗi
- Kiểm tra tab Network để xem API calls

## 📞 Hỗ trợ

Nếu gặp vấn đề, vui lòng:
1. Kiểm tra file README này
2. Xem console browser để tìm lỗi
3. Liên hệ team phát triển

## 🔄 Cập nhật

### Version 1.0.0
- ✅ Giao diện chatbot cơ bản
- ✅ Xử lý đặt lịch khám
- ✅ Kiểm tra và hủy lịch hẹn
- ✅ Thông tin bệnh viện
- ✅ Responsive design

### Roadmap
- 🔄 Tích hợp AI thực tế
- 🔄 Đa ngôn ngữ
- 🔄 Voice recognition
- 🔄 Tích hợp payment
- 🔄 Notification system

---

**Tác giả:** AI Development Team  
**Ngày tạo:** 2024  
**Phiên bản:** 1.0.0 