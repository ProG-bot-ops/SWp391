// extra-ui-cleanup.js
// Thêm các tính năng UI bổ sung và làm sạch giao diện
(function() {
    function enhanceUI() {
        // 1. Thêm loading spinner cho các nút
        addLoadingStates();
        
        // 2. Thêm tooltips
        addTooltips();
        
        // 3. Thêm confirm dialogs
        addConfirmDialogs();
        
        // 4. Thêm keyboard shortcuts
        addKeyboardShortcuts();
        
        // 5. Thêm auto-refresh
        setupAutoRefresh();
        
        // 6. Thêm export functionality
        addExportFunctionality();
        
        console.log('✅ Đã hoàn thiện giao diện');
    }

    // Thêm loading states cho các nút
    function addLoadingStates() {
        // Add appointment button
        const addBtn = document.querySelector('[data-bs-target="#addAppointmentModal"]');
        if (addBtn) {
            addBtn.addEventListener('click', function() {
                this.innerHTML = '<i class="fas fa-spinner fa-spin me-2"></i>Đang tải...';
                this.disabled = true;
                
                setTimeout(() => {
                    this.innerHTML = '<i class="fas fa-plus me-2"></i>Thêm cuộc hẹn';
                    this.disabled = false;
                }, 1000);
            });
        }
    }

    // Thêm tooltips
    function addTooltips() {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    // Thêm confirm dialogs
    function addConfirmDialogs() {
        // Override delete buttons
        document.addEventListener('click', function(e) {
            if (e.target.closest('.delete-btn')) {
                e.preventDefault();
                if (confirm('Bạn có chắc chắn muốn xóa mục này?')) {
                    // Proceed with deletion
                    const deleteBtn = e.target.closest('.delete-btn');
                    const appointmentId = deleteBtn.getAttribute('data-id');
                    if (appointmentId && window.deleteAppointment) {
                        window.deleteAppointment(appointmentId);
                    }
                }
            }
        });
    }

    // Thêm keyboard shortcuts
    function addKeyboardShortcuts() {
        document.addEventListener('keydown', function(e) {
            // Ctrl/Cmd + N: Add new appointment
            if ((e.ctrlKey || e.metaKey) && e.key === 'n') {
                e.preventDefault();
                const addBtn = document.querySelector('[data-bs-target="#addAppointmentModal"]');
                if (addBtn) addBtn.click();
            }
            
            // Ctrl/Cmd + F: Focus search
            if ((e.ctrlKey || e.metaKey) && e.key === 'f') {
                e.preventDefault();
                const searchInput = document.getElementById('searchPatientInput');
                if (searchInput) searchInput.focus();
            }
            
            // Escape: Close modals
            if (e.key === 'Escape') {
                const modals = document.querySelectorAll('.modal.show, .offcanvas.show');
                modals.forEach(modal => {
                    const modalInstance = bootstrap.Modal.getInstance(modal) || bootstrap.Offcanvas.getInstance(modal);
                    if (modalInstance) modalInstance.hide();
                });
            }
        });
    }

    // Setup auto-refresh
    function setupAutoRefresh() {
        // Auto refresh every 30 seconds
        setInterval(() => {
            if (window.loadAppointments) {
                window.loadAppointments();
            }
        }, 30000);
        
        // Add refresh button
        const refreshBtn = document.createElement('button');
        refreshBtn.className = 'btn btn-outline-secondary btn-sm ms-2';
        refreshBtn.innerHTML = '<i class="fas fa-sync-alt"></i>';
        refreshBtn.title = 'Làm mới dữ liệu (Ctrl+R)';
        refreshBtn.onclick = function() {
            this.innerHTML = '<i class="fas fa-spinner fa-spin"></i>';
            if (window.loadAppointments) {
                window.loadAppointments();
            }
            setTimeout(() => {
                this.innerHTML = '<i class="fas fa-sync-alt"></i>';
            }, 1000);
        };
        
        // Add to header
        const header = document.querySelector('.card-header');
        if (header) {
            header.appendChild(refreshBtn);
        }
    }

    // Thêm export functionality
    function addExportFunctionality() {
        const exportBtn = document.createElement('button');
        exportBtn.className = 'btn btn-outline-success btn-sm ms-2';
        exportBtn.innerHTML = '<i class="fas fa-download me-1"></i>Xuất Excel';
        exportBtn.title = 'Xuất danh sách lịch hẹn ra Excel';
        exportBtn.onclick = exportToExcel;
        
        // Add to header
        const header = document.querySelector('.card-header');
        if (header) {
            header.appendChild(exportBtn);
        }
    }

    // Export to Excel function
    function exportToExcel() {
        const activeTab = document.querySelector('.tab-pane.active');
        const table = activeTab ? activeTab.querySelector('table') : null;
        
        if (!table) {
            alert('Không tìm thấy bảng để xuất');
            return;
        }

        // Get table data
        const rows = table.querySelectorAll('tbody tr');
        let csvContent = "data:text/csv;charset=utf-8,";
        
        // Add headers
        const headers = ['STT', 'Họ và tên', 'Email', 'Ngày & Giờ', 'Ca', 'Loại', 'Hành động'];
        csvContent += headers.join(',') + '\n';
        
        // Add data rows
        rows.forEach((row, index) => {
            const cells = row.querySelectorAll('td, th');
            const rowData = [];
            cells.forEach(cell => {
                // Clean text content
                let text = cell.textContent.trim().replace(/,/g, ';');
                rowData.push(`"${text}"`);
            });
            csvContent += rowData.join(',') + '\n';
        });
        
        // Create download link
        const encodedUri = encodeURI(csvContent);
        const link = document.createElement('a');
        link.setAttribute('href', encodedUri);
        link.setAttribute('download', `appointments_${new Date().toISOString().split('T')[0]}.csv`);
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        
        showToast('success', 'Đã xuất dữ liệu thành công');
    }

    // Show toast notification
    function showToast(type, message) {
        const toastHtml = `
            <div class="toast align-items-center text-white bg-${type === 'success' ? 'success' : 'danger'} border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        ${message}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
            </div>
        `;
        
        let toastContainer = document.getElementById('toastContainer');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.id = 'toastContainer';
            toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
            toastContainer.style.zIndex = '9999';
            document.body.appendChild(toastContainer);
        }
        
        toastContainer.insertAdjacentHTML('beforeend', toastHtml);
        
        const toastElement = toastContainer.lastElementChild;
        const toast = new bootstrap.Toast(toastElement);
        toast.show();
        
        toastElement.addEventListener('hidden.bs.toast', function() {
            toastElement.remove();
        });
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', enhanceUI);
    } else {
        enhanceUI();
    }
})(); 