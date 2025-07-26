// custom-table-vn.js
// Script này sẽ tự động chỉnh sửa bảng cuộc hẹn theo yêu cầu tiếng Việt đặc biệt
// Đảm bảo nhúng file này vào cuối trang appointment.html

(function() {
    function updateTable() {
        // Xác định các tab
        const tabPaneList = document.querySelectorAll('.tab-pane');
        const statusList = ['Chờ khám', 'Đã khám', 'Đã hủy'];
        // Format chuẩn header
        const headerFormat = [
            'STT',
            'Họ và tên bệnh nhân',
            'Tên bác sĩ',
            'Phòng khám',
            'Ngày & Giờ',
            'Lý do',
            'Trạng thái',
            'Hành động'
        ];
        // Hàm chuẩn hóa header bảng
        function normalizeTableHeader(table) {
            if (!table) return;
            const thead = table.querySelector('thead');
            if (!thead) return;
            let tr = thead.querySelector('tr');
            if (!tr) {
                tr = document.createElement('tr');
                thead.appendChild(tr);
            }
            // Xóa hết th hiện tại
            while (tr.firstChild) tr.removeChild(tr.firstChild);
            // Thêm lại đúng format
            headerFormat.forEach(txt => {
                const th = document.createElement('th');
                th.textContent = txt;
                tr.appendChild(th);
            });
        }
        // Tab Sắp tới
        if (tabPaneList[0]) {
            const table = tabPaneList[0].querySelector('table');
            if (table) {
                normalizeTableHeader(table);
            }
        }
        // Tab Hoàn thành
        if (tabPaneList[1]) {
            const table = tabPaneList[1].querySelector('table');
            if (table) {
                normalizeTableHeader(table);
            }
        }
        // Tab Đã hủy
        if (tabPaneList[2]) {
            const table = tabPaneList[2].querySelector('table');
            if (table) {
                normalizeTableHeader(table);
            }
        }
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', updateTable);
    } else {
        updateTable();
    }
})(); 