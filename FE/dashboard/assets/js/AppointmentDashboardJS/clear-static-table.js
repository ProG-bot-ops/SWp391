// clear-static-table.js
// Script này sẽ tự động xóa toàn bộ các dòng dữ liệu tĩnh trong <tbody> của các bảng trên trang appointment.html
// Đảm bảo nhúng file này vào cuối trang

(function() {
    function clearAllTableBodies() {
        // Lấy tất cả các tbody trong trang (cả 2 tab)
        document.querySelectorAll('table tbody').forEach(tbody => {
            while (tbody.firstChild) {
                tbody.removeChild(tbody.firstChild);
            }
        });
    }
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', clearAllTableBodies);
    } else {
        clearAllTableBodies();
    }
})(); 