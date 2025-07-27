// clear-static-table.js
// Xóa dữ liệu tĩnh trong bảng và chuẩn bị cho dữ liệu động từ API
(function() {
    function clearStaticData() {
        // Xóa dữ liệu tĩnh trong tất cả các tab
        const tabPanes = document.querySelectorAll('.tab-pane');
        
        tabPanes.forEach(tabPane => {
            const tbody = tabPane.querySelector('tbody');
            if (tbody) {
                // Giữ lại header, xóa tất cả dữ liệu
                tbody.innerHTML = `
                    <tr>
                        <td colspan="7" class="text-center text-muted">
                            <i class="fas fa-spinner fa-spin me-2"></i>
                            Đang tải dữ liệu...
                        </td>
                    </tr>
                `;
            }
        });

        // Xóa dữ liệu tĩnh trong các form
        const forms = document.querySelectorAll('form');
        forms.forEach(form => {
            const inputs = form.querySelectorAll('input[type="text"], input[type="email"], textarea, select');
            inputs.forEach(input => {
                if (input.id !== 'searchPatientInput') { // Giữ lại search input
                    input.value = '';
                }
            });
        });

        // Xóa dữ liệu tĩnh trong các modal/offcanvas
        const modals = document.querySelectorAll('.modal, .offcanvas');
        modals.forEach(modal => {
            const inputs = modal.querySelectorAll('input[type="text"], input[type="email"], textarea, select');
            inputs.forEach(input => {
                input.value = '';
            });
        });

        console.log('✅ Đã xóa dữ liệu tĩnh, sẵn sàng cho dữ liệu động');
    }

    // Chạy khi DOM sẵn sàng
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', clearStaticData);
    } else {
        clearStaticData();
    }
})(); 