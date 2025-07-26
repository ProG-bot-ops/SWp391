// load-appointments.js
(function() {
    let allAppointments = [];

    function renderTable(status) {
        // Find the active tab-pane
        var activeTab = document.querySelector('.tab-pane.active');
        if (!activeTab) return;
        var tbody = activeTab.querySelector('tbody');
        if (!tbody) return;
        tbody.innerHTML = '';
        // Lọc theo trạng thái
        const filtered = allAppointments.filter(item => 
            item.status && item.status.toLowerCase() === status
        );
        if (filtered.length === 0) {
            var tr = document.createElement('tr');
            tr.innerHTML = `<td colspan="8" style="text-align:center;color:#888;">Không có dữ liệu lịch hẹn</td>`;
            tbody.appendChild(tr);
            return;
        }
        // Sắp xếp theo ngày và giờ tăng dần
        filtered.sort((a, b) => {
            const dateA = new Date(a.appointmentDate);
            const dateB = new Date(b.appointmentDate);
            // Nếu ngày giống nhau thì so sánh giờ
            if (dateA.getTime() === dateB.getTime()) {
                const timeA = a.startTime ? a.startTime.substring(0,5) : '';
                const timeB = b.startTime ? b.startTime.substring(0,5) : '';
                return timeA.localeCompare(timeB);
            }
            return dateA - dateB;
        });
        filtered.forEach((item, idx) => {
            // Badge màu cho trạng thái
            let statusColor = '#888';
            let statusBg = '#eee';
            let statusText = item.status || '';
            if (statusText.toLowerCase() === 'completed') {
                statusColor = '#fff'; statusBg = '#28a745';
            } else if (statusText.toLowerCase() === 'scheduled') {
                statusColor = '#fff'; statusBg = '#007bff';
            } else if (statusText.toLowerCase() === 'cancelled') {
                statusColor = '#fff'; statusBg = '#dc3545';
            }
            var tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${idx + 1}</td>
                <td><span style="font-weight:bold;color:#2d3a4b;">${item.patientName || (item.patient && item.patient.name) || ''}</span></td>
                <td><span style="font-weight:bold;color:#6f42c1;">${item.doctorName || ''}</span></td>
                <td><span style="color:#17a2b8;">${item.clinic || ''}</span></td>
                <td><span style="color:#fd7e14;">${item.appointmentDate && item.startTime ?
                    (() => {
                        const d = new Date(item.appointmentDate);
                        const [h, m] = item.startTime.split(":");
                        d.setHours(h, m);
                        return d.toLocaleString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit', hour12: false });
                    })() : ''}
                </span></td>
                <td><span style="font-weight:bold;color:#20c997;">${item.reason || ''}</span></td>
                <td><span style="display:inline-block;padding:2px 10px;border-radius:12px;font-weight:bold;background:${statusBg};color:${statusColor};min-width:90px;text-align:center;">${statusText}</span></td>
                <td>
                    <button class="btn btn-sm btn-primary me-1 btn-edit-appointment" data-id="${item.id}" data-bs-toggle="offcanvas" data-bs-target="#offcanvasAppointmentEdit">Sửa</button>
                    <button class="btn btn-sm btn-secondary btn-detail-appointment" data-id="${item.id}" data-bs-toggle="offcanvas" data-bs-target="#offcanvasAppointmentDetail">Chi tiết</button>
                </td>
            `;
            tbody.appendChild(tr);
        });

        // Gắn sự kiện cho nút Sửa
        tbody.querySelectorAll('.btn-edit-appointment').forEach(btn => {
            btn.addEventListener('click', function() {
                const id = btn.getAttribute('data-id');
                fetch(`https://localhost:7097/api/appointment/detail/${id}`)
                    .then(res => res.json())
                    .then(data => {
                        // Hiển thị dữ liệu vào form sửa trong offcanvas/modal
                        if (data && data.id) {
                            document.getElementById('edit-appointment-id').value = data.id;
                            document.getElementById('edit-patient-name').value = data.patientName || '';
                            document.getElementById('edit-doctor-name').value = data.doctorName || '';
                            document.getElementById('edit-clinic').value = data.clinic || '';
                            document.getElementById('edit-date').value = data.appointmentDate ? new Date(data.appointmentDate).toISOString().slice(0,10) : '';
                            document.getElementById('edit-time').value = data.startTime ? data.startTime.substring(0,5) : '';
                            document.getElementById('edit-reason').value = data.reason || '';
                            // Hiển thị offcanvas
                            var offcanvas = new bootstrap.Offcanvas(document.getElementById('offcanvasAppointmentEdit'));
                            offcanvas.show();
                        }
                    });
            });
        });

        // Gắn sự kiện cho nút Chi tiết
        tbody.querySelectorAll('.btn-detail-appointment').forEach(btn => {
            btn.addEventListener('click', function() {
                const id = btn.getAttribute('data-id');
                fetch(`https://localhost:7097/api/appointment/detail/${id}`)
                    .then(res => res.json())
                    .then(data => {
                        // Hiển thị dữ liệu vào popup/modal chi tiết
                        if (data && data.id) {
                            document.getElementById('detail-appointment-id').textContent = data.id;
                            document.getElementById('detail-patient-name').textContent = data.patientName || '';
                            document.getElementById('detail-doctor-name').textContent = data.doctorName || '';
                            document.getElementById('detail-clinic').textContent = data.clinic || '';
                            document.getElementById('detail-date').textContent = data.appointmentDate ? new Date(data.appointmentDate).toLocaleDateString('vi-VN') : '';
                            document.getElementById('detail-time').textContent = data.startTime ? data.startTime.substring(0,5) : '';
                            document.getElementById('detail-reason').textContent = data.reason || '';
                            // Hiển thị offcanvas
                            var offcanvas = new bootstrap.Offcanvas(document.getElementById('offcanvasAppointmentDetail'));
                            offcanvas.show();
                        }
                    });
            });
        });
    }

    document.addEventListener('DOMContentLoaded', function() {
        fetch('https://localhost:7097/api/appointment/list')
            .then(res => {
                if (!res.ok) throw new Error('API error: ' + res.status);
                return res.json();
            })
            .then(data => {
                allAppointments = data;
                // Mặc định hiển thị tab "Sắp tới"
                renderTable('scheduled');
            })
            .catch(err => {
                var activeTab = document.querySelector('.tab-pane.active');
                var tbody = activeTab ? activeTab.querySelector('tbody') : null;
                if (tbody) {
                    tbody.innerHTML = `<tr><td colspan="8" style="text-align:center;color:red;">Lỗi tải dữ liệu lịch hẹn</td></tr>`;
                }
            });

        // Gắn sự kiện cho các tab
        document.querySelectorAll('[data-bs-toggle="tab"]').forEach(btn => {
            btn.addEventListener('shown.bs.tab', function(e) {
                // Xác định trạng thái cần lọc dựa vào tab
                let status = 'scheduled';
                const target = e.target.getAttribute('data-bs-target');
                if (target === '#upcoming') status = 'scheduled';
                else if (target === '#request') status = 'completed';
                else if (target === '#cancelled') status = 'cancelled';
                renderTable(status);
            });
        });
    });
})(); 