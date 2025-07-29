# Hướng dẫn sử dụng tính năng thêm lịch hẹn

## Tổng quan
Tính năng thêm lịch hẹn đã được cải thiện với quy trình tìm kiếm và tạo bệnh nhân tự động theo số điện thoại.

## Quy trình sử dụng

### 1. Tìm kiếm bệnh nhân hiện có
1. Mở form thêm lịch hẹn
2. Nhập số điện thoại vào trường "Số điện thoại"
3. Hệ thống sẽ tự động tìm kiếm sau 1 giây hoặc nhấn nút "Tìm kiếm"
4. Nếu tìm thấy bệnh nhân:
   - Thông tin sẽ được tự động điền vào các trường
   - Các trường thông tin sẽ bị khóa (disabled)
   - Hiển thị thông báo thành công

### 2. Tạo bệnh nhân mới
1. Nếu không tìm thấy bệnh nhân với số điện thoại đã nhập
2. Điền đầy đủ thông tin bệnh nhân:
   - Họ và tên (bắt buộc)
   - Email (bắt buộc)
   - Ngày sinh (tùy chọn)
   - Giới tính (tùy chọn)
   - Địa chỉ (tùy chọn)
3. Nhấn nút "Tạo bệnh nhân mới"
4. Hệ thống sẽ:
   - Tạo user mới với mật khẩu ngẫu nhiên
   - Tạo bệnh nhân mới với thông tin đã nhập
   - Gửi email chứa mật khẩu đến email đã nhập
   - Khóa các trường thông tin sau khi tạo thành công

### 3. Chọn thông tin lịch hẹn
1. Chọn phòng khám (bắt buộc)
2. Chọn bác sĩ (chỉ hiển thị bác sĩ của phòng khám đã chọn)
3. Chọn dịch vụ (chỉ hiển thị dịch vụ của bác sĩ đã chọn)
4. Chọn ngày khám (không cho phép chọn ngày trong quá khứ)
5. Chọn ca khám (chỉ hiển thị ca khám có sẵn trong ngày đã chọn)
6. Nhập ghi chú (tùy chọn)

### 4. Lưu lịch hẹn
1. Nhấn nút "Lưu +" để tạo lịch hẹn
2. Hệ thống sẽ kiểm tra:
   - Đã có thông tin bệnh nhân chưa
   - Các trường bắt buộc đã được điền chưa
3. Nếu thành công:
   - Hiển thị thông báo thành công
   - Đóng form
   - Reload danh sách lịch hẹn
   - Cập nhật số liệu thống kê

## Validation

### Số điện thoại
- Định dạng số điện thoại Việt Nam hợp lệ
- Regex: `^(0|\+84)(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$`

### Email
- Định dạng email hợp lệ
- Regex: `^[^\s@]+@[^\s@]+\.[^\s@]+$`

### Ngày khám
- Không cho phép chọn ngày trong quá khứ
- Chỉ hiển thị ca khám có sẵn trong ngày đã chọn

## API Endpoints

### Patient
- `GET /api/patient/search?phone={phone}` - Tìm kiếm bệnh nhân theo số điện thoại
- `POST /api/patient/create` - Tạo bệnh nhân mới

### Auth
- `POST /api/auth/register` - Đăng ký user mới
- `POST /api/auth/send-password-email` - Gửi email chứa mật khẩu

### Appointment
- `GET /api/appointment/clinic/list` - Danh sách phòng khám
- `GET /api/appointment/doctor/list?clinicId={id}` - Danh sách bác sĩ theo phòng khám
- `GET /api/appointment/service/list?doctorId={id}` - Danh sách dịch vụ theo bác sĩ
- `GET /api/appointment/available-shifts?doctorId={id}&date={date}` - Ca khám có sẵn
- `POST /api/appointment/create` - Tạo lịch hẹn mới

## Files liên quan

### JavaScript Files
- `api-endpoints.js` - Định nghĩa các API endpoints
- `search-patient.js` - Xử lý tìm kiếm và tạo bệnh nhân
- `add-appointment.js` - Xử lý thêm lịch hẹn

### HTML Files
- `appointment.html` - Giao diện chính

## Lưu ý
- Mật khẩu được tạo ngẫu nhiên 12 ký tự
- Email chứa mật khẩu sẽ được gửi tự động khi tạo bệnh nhân mới
- Các trường thông tin bệnh nhân sẽ bị khóa sau khi tìm thấy hoặc tạo thành công
- Form sẽ được reset khi đóng hoặc sau khi tạo lịch hẹn thành công