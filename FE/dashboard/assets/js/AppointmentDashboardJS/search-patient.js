// search-patient.js
(function() {
    // Đợi DOM sẵn sàng
    document.addEventListener('DOMContentLoaded', function() {
        // 1. Thêm thanh tìm kiếm cạnh nút Add Appointment
        var addBtn = document.querySelector('a.btn.btn-primary[data-bs-toggle="offcanvas"][href="#offcanvasAppointmentAdd"]');
        if (addBtn && !document.getElementById('search-patient-form')) {
            var form = document.createElement('form');
            form.className = 'd-inline-block ms-2';
            form.id = 'search-patient-form';
            form.style.maxWidth = '250px';
            form.innerHTML = `
                <div class="input-group">
                    <input type="text" class="form-control" id="search-patient-input" placeholder="Tìm kiếm bệnh nhân...">
                    <button class="btn btn-outline-secondary" type="submit" tabindex="-1">
                        <svg width="16" height="16" fill="currentColor" class="bi bi-search" viewBox="0 0 16 16">
                            <path d="M11.742 10.344a6.5 6.5 0 1 0-1.397 1.398h-.001l3.85 3.85a1 1 0 0 0 1.415-1.415l-3.85-3.85zm-5.242 1.106a5 5 0 1 1 0-10 5 5 0 0 1 0 10z"/>
                        </svg>
                    </button>
                </div>
            `;
            addBtn.parentNode.insertBefore(form, addBtn.nextSibling);
        }

        // 2. Lọc bảng bệnh nhân khi tìm kiếm
        function filterPatientTable(keyword) {
            // Lọc tất cả các bảng trong tab (upcoming, request, ...)
            var tables = document.querySelectorAll('.tab-pane.active table, .tab-pane.show table');
            tables.forEach(function(table) {
                var rows = table.querySelectorAll('tbody tr');
                rows.forEach(function(row) {
                    var text = row.innerText.toLowerCase();
                    if (text.includes(keyword)) {
                        row.style.display = '';
                    } else {
                        row.style.display = 'none';
                    }
                });
            });
        }

        // 3. Sự kiện submit và input
        var formEl = document.getElementById('search-patient-form');
        if (formEl) {
            formEl.addEventListener('submit', function(e) {
                e.preventDefault();
                var keyword = document.getElementById('search-patient-input').value.trim().toLowerCase();
                filterPatientTable(keyword);
            });
            // Lọc realtime khi nhập
            var inputEl = document.getElementById('search-patient-input');
            if (inputEl) {
                inputEl.addEventListener('input', function() {
                    var keyword = this.value.trim().toLowerCase();
                    filterPatientTable(keyword);
                });
            }
        }
    });
})(); 