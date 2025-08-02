// translate-vi.js
// Script này sẽ tự động dịch các thành phần giao diện chính trên trang appointment.html sang tiếng Việt
// Đảm bảo nhúng file này vào cuối trang hoặc sau khi DOM đã sẵn sàng

(function() {
    // Danh sách các cặp từ/cụm từ cần dịch
    const translations = [
        // Tiêu đề, menu, bảng
        ["Appointments", "Cuộc hẹn"],
        ["Add Appointment", "Thêm cuộc hẹn"],
        ["Upcoming", "Sắp tới"],
        ["Request", "Yêu cầu"],
        ["Name", "Họ và tên"],
        ["Email", "Email"],
        ["Date And Time", "Ngày"],
        ["Reason", "Ca"],
        ["Type", "Loại"],
        ["New Patient", "Bệnh nhân mới"],
        ["Old Patient", "Bệnh nhân cũ"],
        ["Action", "Thao tác"],
        ["Accept", "Chấp nhận"],
        ["Cancel", "Hủy"],
        ["Update", "Cập nhật"],
        ["Save", "Lưu"],
        ["Close", "Đóng"],
        ["Edit Appointments", "Chỉnh sửa cuộc hẹn"],
        ["Add Appointments", "Thêm cuộc hẹn"],
        ["Search...", "Tìm kiếm..."],
        ["Previous", "Trước"],
        ["Next", "Sau"],
        ["Showing 01 to 07 of 20 entries", "Hiển thị 01 đến 07 trong tổng số 20 mục"],
        ["Profile Image", "Ảnh đại diện"],
        ["Select", "Chọn"],
        ["Dashboard", "Bảng điều khiển"],
        ["patient dashboard", "Bảng điều khiển bệnh nhân"],
        ["Report", "Báo cáo"],
        ["Doctors", "Bác sĩ"],
        ["Patient", "Bệnh nhân"],
        ["Category", "Danh mục"],
        ["Products", "Sản phẩm"],
        ["Payment", "Thanh toán"],
        ["Help & Support", "Hỗ trợ"],
        ["Setting", "Cài đặt"],
        ["User", "Người dùng"],
        ["Utilities", "Tiện ích"],
        ["Widgets", "Widget"],
        ["Map", "Bản đồ"],
        ["Form", "Biểu mẫu"],
        ["Table", "Bảng"],
        ["Icons", "Biểu tượng"],
        ["Home", "Trang chủ"],
        ["Privacy Policy", "Chính sách bảo mật"],
        ["Terms of Use", "Điều khoản sử dụng"],
        // Các trạng thái mẫu
        ["Regular", "Thông thường"],
        ["Fever", "Sốt"],
        ["Malaria", "Sốt rét"],
        ["Eye Checkup", "Khám mắt"],
        ["Tooth Cleaning", "Làm sạch răng"],
        ["Skin Whitening", "Làm trắng da"],
        ["Acne Treatment", "Điều trị mụn"],
        // ... có thể bổ sung thêm nếu cần
    ];

    // Hàm dịch text node
    function translateTextNode(node) {
        if (node.nodeType === 3) { // text node
            let text = node.nodeValue;
            translations.forEach(([en, vi]) => {
                // Chỉ thay thế nếu là từ/cụm từ độc lập hoặc đầu/cuối câu
                const regex = new RegExp(`\\b${en.replace(/([.*+?^=!:${}()|[\]\\\/])/g, "\\$1")}\\b`, 'g');
                text = text.replace(regex, vi);
            });
            node.nodeValue = text;
        }
    }

    // Duyệt toàn bộ DOM và dịch các text node
    function translateDOM(element) {
        for (let node of element.childNodes) {
            if (node.nodeType === 3) {
                translateTextNode(node);
            } else if (node.nodeType === 1 && node.childNodes.length > 0) {
                // Không dịch script/style
                if (node.tagName !== 'SCRIPT' && node.tagName !== 'STYLE') {
                    translateDOM(node);
                }
            }
        }
    }

    // Dịch placeholder, value, title, aria-label...
    function translateAttributes() {
        const attrs = ['placeholder', 'value', 'title', 'aria-label'];
        attrs.forEach(attr => {
            document.querySelectorAll(`[${attr}]`).forEach(el => {
                translations.forEach(([en, vi]) => {
                    if (el.getAttribute(attr) === en) {
                        el.setAttribute(attr, vi);
                    }
                });
            });
        });
    }

    // Chờ DOM sẵn sàng
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', runTranslate);
    } else {
        runTranslate();
    }

    function runTranslate() {
        translateDOM(document.body);
        translateAttributes();
    }
})(); 