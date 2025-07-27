# Hướng dẫn sử dụng JavaScript cho Appointment Dashboard

## Tổng quan

Bộ JavaScript này cung cấp đầy đủ chức năng cho trang quản lý lịch hẹn (appointment dashboard), bao gồm:

- **CRUD Operations**: Thêm, sửa, xóa, xem lịch hẹn
- **Status Management**: Chấp nhận, hủy lịch hẹn
- **Data Loading**: Load doctors, clinics, services cho dropdowns
- **Patient Search**: Tìm kiếm bệnh nhân theo tên, email, phone
- **Statistics**: Thống kê lịch hẹn
- **Debug Tools**: Công cụ debug và test API

## Cách thêm vào appointment.html

Thêm đoạn code sau vào cuối file `FE/dashboard/appointment.html`, ngay trước thẻ `</body>`:

```html
<!-- Appointment Dashboard JavaScript Files -->
<script src="./assets/js/AppointmentDashboardJS/debug-api.js"></script>
<script src="./assets/js/AppointmentDashboardJS/api-endpoints.js"></script>
<script src="./assets/js/AppointmentDashboardJS/clear-static-table.js"></script>
<script src="./assets/js/AppointmentDashboardJS/translate-vi.js"></script>
<script src="./assets/js/AppointmentDashboardJS/custom-table-vn.js"></script>
<script src="./assets/js/AppointmentDashboardJS/add-tabs-vn.js"></script>
<script src="./assets/js/AppointmentDashboardJS/load-appointments.js"></script>
<script src="./assets/js/AppointmentDashboardJS/add-appointment.js"></script>
<script src="./assets/js/AppointmentDashboardJS/search-patient.js"></script>
<script src="./assets/js/AppointmentDashboardJS/extra-ui-cleanup.js"></script>
```

## Thứ tự load quan trọng

1. `debug-api.js` - Công cụ debug và test API
2. `api-endpoints.js` - Định nghĩa API endpoints
3. `clear-static-table.js` - Xóa dữ liệu tĩnh
4. `translate-vi.js` - Dịch giao diện
5. `custom-table-vn.js` - Tùy chỉnh bảng
6. `add-tabs-vn.js` - Thêm tab mới
7. `load-appointments.js` - Load dữ liệu lịch hẹn
8. `add-appointment.js` - Thêm lịch hẹn mới
9. `search-patient.js` - Tìm kiếm bệnh nhân
10. `extra-ui-cleanup.js` - Tính năng UI bổ sung

## Kiểm tra kết quả

### 1. Mở Developer Tools (F12)
### 2. Kiểm tra Console để xem các log:
- `🔧 Debug API tools loaded` - Debug tools đã sẵn sàng
- `✅ API endpoints đã được cấu hình` - API endpoints đã sẵn sàng
- `✅ Đã xóa dữ liệu tĩnh` - Dữ liệu tĩnh đã được xóa
- `✅ Đã hoàn thiện giao diện` - UI đã được cải thiện

### 3. Kiểm tra giao diện:
- Bảng sẽ hiển thị dữ liệu từ API
- Các label đã được dịch sang tiếng Việt
- Có tab "Đã hủy" mới
- Có nút "Làm mới" và "Xuất Excel" ở header

### 4. Test các chức năng:
- Click nút "Thêm cuộc hẹn" để test form thêm mới
- Click nút "Sửa" để test form chỉnh sửa
- Click nút "Accept/Cancel" để test thay đổi trạng thái
- Sử dụng thanh tìm kiếm để test tìm kiếm bệnh nhân

## Debug nếu có lỗi

### 1. Kiểm tra Console để xem lỗi cụ thể
### 2. Sử dụng Debug Tools (góc trên bên phải):
- Click "Test API" để kiểm tra kết nối API
- Click "Test All" để test tất cả endpoints
- Click "Create Test" để tạo lịch hẹn mẫu
- Click "Update Test" để test cập nhật
- Click "Delete Test" để test xóa

### 3. Kiểm tra Network tab để xem các request API

## Đảm bảo Backend API đang chạy

1. **Đảm bảo backend API đang chạy** tại `https://localhost:7097`
2. **Kiểm tra các API endpoints** đã được định nghĩa trong `api-endpoints.js`
3. **Test API connection** bằng cách click "Test API" trong debug tools

## Lưu ý quan trọng

- **API thật** sẽ được sử dụng từ backend
- **Debug tools** sẽ hiển thị ở góc trên bên phải màn hình
- **Auto-refresh** sẽ làm mới dữ liệu mỗi 30 giây
- **Keyboard shortcuts**:
  - `Ctrl/Cmd + N`: Thêm lịch hẹn mới
  - `Ctrl/Cmd + F`: Focus vào ô tìm kiếm
  - `Escape`: Đóng modal/offcanvas

## Cấu trúc dữ liệu mong đợi

Backend API cần trả về dữ liệu với cấu trúc:

```javascript
{
  id: number,
  name: string,           // Tên bệnh nhân
  email: string,          // Email bệnh nhân
  phone: string,          // Số điện thoại
  doctorName: string,     // Tên bác sĩ
  clinic: string,         // Tên phòng khám
  date: string,           // Ngày hẹn (YYYY-MM-DD)
  time: string,           // Giờ hẹn (HH:mm)
  shift: string,          // Lý do/ca
  type: string,           // Loại bệnh nhân
  status: string,         // Trạng thái (scheduled/completed/cancelled)
  note: string            // Ghi chú
}
```

## API Endpoints được sử dụng

- `GET /api/appointment/list` - Lấy danh sách lịch hẹn
- `GET /api/appointment/detail/{id}` - Lấy chi tiết lịch hẹn
- `PUT /api/appointment/update` - Cập nhật lịch hẹn
- `DELETE /api/appointment/delete/{id}` - Xóa lịch hẹn
- `PUT /api/appointment/accept/{id}` - Chấp nhận lịch hẹn
- `PUT /api/appointment/cancel/{id}` - Hủy lịch hẹn
- `GET /api/appointment/doctor/list` - Lấy danh sách bác sĩ
- `GET /api/appointment/clinic/list` - Lấy danh sách phòng khám
- `GET /api/appointment/service/list` - Lấy danh sách dịch vụ
- `GET /api/appointment/patient/search` - Tìm kiếm bệnh nhân
- `GET /api/appointment/statistics` - Lấy thống kê
- `GET /api/appointment/test` - Test API connection

## Troubleshooting

### Lỗi thường gặp:

1. **API không kết nối được**: Kiểm tra backend có đang chạy không
2. **CORS error**: Đảm bảo backend đã cấu hình CORS
3. **404 Not Found**: Kiểm tra URL API có đúng không
4. **500 Internal Server Error**: Kiểm tra log backend để debug

### Cách debug:

1. Sử dụng Debug Tools để test từng endpoint
2. Kiểm tra Network tab trong Developer Tools
3. Xem Console log để tìm lỗi
4. Kiểm tra backend logs

## Tính năng bổ sung

- **Auto-refresh**: Tự động làm mới dữ liệu mỗi 30 giây
- **Export Excel**: Xuất danh sách lịch hẹn ra file CSV
- **Keyboard shortcuts**: Phím tắt để thao tác nhanh
- **Toast notifications**: Thông báo kết quả thao tác
- **Loading states**: Hiển thị trạng thái loading
- **Error handling**: Xử lý lỗi một cách thân thiện 