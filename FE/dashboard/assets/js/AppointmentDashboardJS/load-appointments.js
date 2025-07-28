// load-appointments.js
(function() {
    let allAppointments = [];
    const API_BASE_URL = 'https://localhost:7097';

    // Function chuyển đổi status sang tiếng Việt
    function getStatusText(status) {
        if (!status) return 'N/A';
        
        switch (status.toLowerCase()) {
            case 'scheduled':
                return 'Đã lên lịch';
            case 'inprogress':
                return 'Đang khám';
            case 'late':
                return 'Đến muộn';
            case 'completed':
                return 'Đã hoàn thành';
            case 'cancelled':
                return 'Đã hủy';
            case 'pending':
                return 'Chờ xác nhận';
            default:
                return status;
        }
    }

    // Function chuyển đổi ca sang tiếng Việt
    function getShiftText(shift) {
        if (!shift) return 'N/A';
        
        switch (shift.toLowerCase()) {
            case 'morning':
                return 'Sáng';
            case 'afternoon':
                return 'Chiều';
            case 'evening':
                return 'Tối';
            case 'sáng':
                return 'Sáng';
            case 'chiều':
                return 'Chiều';
            case 'tối':
                return 'Tối';
            default:
                return shift;
        }
    }

    function renderTable(status) {
        // Find the active tab-pane
        var activeTab = document.querySelector('.tab-pane.active');
        if (!activeTab) return;
        var tbody = activeTab.querySelector('tbody');
        if (!tbody) return;
        tbody.innerHTML = '';
        
        // Lọc theo trạng thái
        let filtered;
        if (status === 'scheduled') {
            // Tab "Sắp tới" bao gồm cả "Đã lên lịch" và "Đến muộn"
            filtered = allAppointments.filter(item => 
                item.status && (item.status.toLowerCase() === 'scheduled' || item.status.toLowerCase() === 'late')
            );
        } else {
            filtered = allAppointments.filter(item => 
                item.status && item.status.toLowerCase() === status
            );
        }
        
        if (filtered.length === 0) {
            var tr = document.createElement('tr');
            tr.innerHTML = `<td colspan="7" style="text-align:center;color:#888;">Không có dữ liệu lịch hẹn</td>`;
            tbody.appendChild(tr);
            return;
        }
        
        // Sắp xếp theo ngày và giờ tăng dần
        filtered.sort((a, b) => {
            const dateA = new Date(a.date || a.appointmentDate);
            const dateB = new Date(b.date || b.appointmentDate);
            // Nếu ngày giống nhau thì so sánh giờ
            if (dateA.getTime() === dateB.getTime()) {
                const timeA = a.time || a.startTime ? (a.time || a.startTime).substring(0,5) : '';
                const timeB = b.time || b.startTime ? (b.time || b.startTime).substring(0,5) : '';
                return timeA.localeCompare(timeB);
            }
            return dateA - dateB;
        });
        
        filtered.forEach((item, idx) => {
            var tr = document.createElement('tr');
            
            // Format ngày giờ - hỗ trợ cả field cũ và mới
            let dateTimeText = '';
            const appointmentDate = item.date || item.appointmentDate;
            const appointmentTime = item.time || item.startTime;
            
            if (appointmentDate && appointmentTime) {
                const d = new Date(appointmentDate);
                const [h, m] = appointmentTime.split(":");
                d.setHours(h, m);
                dateTimeText = d.toLocaleString('vi-VN', { 
                    day: '2-digit', 
                    month: '2-digit', 
                    year: 'numeric', 
                    hour: '2-digit', 
                    minute: '2-digit', 
                    hour12: false 
                });
            }
            
            // Lấy thông tin bệnh nhân - hỗ trợ cả field cũ và mới
            const patientName = item.patientName || item.patient?.name || item.name || 'Unknown';
            const patientEmail = item.patientEmail || item.patient?.email || item.email || 'N/A';
            
            // Lấy thông tin bác sĩ - hỗ trợ cả field cũ và mới
            const doctorName = item.doctorName || item.doctor?.name || item.doctorName || 'N/A';
            
            // Lấy thông tin phòng khám - hỗ trợ cả field cũ và mới
            const clinic = item.clinic || item.clinicName || 'N/A';
            
            // Lấy ca (sáng/chiều) - hỗ trợ cả field cũ và mới
            const shift = item.shift || item.ca || 'N/A';
            
            // Xác định loại bệnh nhân
            const patientType = item.patientType || item.type || 'New Patient';
            
            // Tạo action buttons dựa trên trạng thái
            let actionButtons = '';
            
            // Nút "Xem chi tiết" cho tất cả trạng thái
            const viewDetailButton = `
                <a class="d-inline-block pe-2" href="#" onclick="viewAppointmentDetail(${item.id})" title="Xem chi tiết">
                    <span class="text-primary">
                        <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                            <path d="M8 3.5C5.5 3.5 3.5 5.5 3.5 8C3.5 10.5 5.5 12.5 8 12.5C10.5 12.5 12.5 10.5 12.5 8C12.5 5.5 10.5 3.5 8 3.5ZM8 11C6.625 11 5.5 9.875 5.5 8.375C5.5 6.875 6.625 5.75 8 5.75C9.375 5.75 10.5 6.875 10.5 8.375C10.5 9.875 9.375 11 8 11Z" fill="currentColor"/>
                            <path d="M8 6.75C7.25 6.75 6.75 7.25 6.75 8C6.75 8.75 7.25 9.25 8 9.25C8.75 9.25 9.25 8.75 9.25 8C9.25 7.25 8.75 6.75 8 6.75Z" fill="currentColor"/>
                        </svg>
                    </span>
                </a>
            `;
            
            if (status === 'scheduled' || status === 'late') {
                // Tab "Sắp tới" (bao gồm cả "Đến muộn") - có nút Xem chi tiết, Sửa và Hủy
                actionButtons = `
                    ${viewDetailButton}
                    <a class="d-inline-block pe-2" data-bs-toggle="offcanvas" href="#offcanvasAppointmentEdit" role="button" aria-controls="offcanvasAppointmentEdit" onclick="loadAppointmentForEdit(${item.id})" title="Sửa">
                        <span class="text-success">
                            <svg width="16" height="16" viewBox="0 0 16 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path d="M9.31055 14.3321H14.75" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                                <path fill-rule="evenodd" clip-rule="evenodd" d="M8.58501 1.84609C9.16674 1.15084 10.2125 1.04889 10.9222 1.6188C10.9614 1.64972 12.2221 2.62909 12.2221 2.62909C13.0017 3.10039 13.244 4.10233 12.762 4.86694C12.7365 4.90789 5.60896 13.8234 5.60896 13.8234C5.37183 14.1192 5.01187 14.2938 4.62718 14.298L1.89765 14.3323L1.28265 11.7292C1.1965 11.3632 1.28265 10.9788 1.51978 10.683L8.58501 1.84609Z" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                                <path d="M7.26562 3.50073L11.3548 6.64108" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                            </svg>
                        </span>
                    </a>
                    <a href="#" class="d-inline-block ps-2" onclick="cancelAppointment(${item.id})" title="Hủy">
                        <span class="text-danger">
                            <svg width="15" height="16" viewBox="0 0 15 16" fill="none" xmlns="http://www.w3.org/2000/svg">
                                <path d="M12.4938 6.10107C12.4938 6.10107 12.0866 11.1523 11.8503 13.2801C11.7378 14.2963 11.1101 14.8918 10.0818 14.9106C8.12509 14.9458 6.16609 14.9481 4.21009 14.9068C3.22084 14.8866 2.60359 14.2836 2.49334 13.2853C2.25559 11.1388 1.85059 6.10107 1.85059 6.10107" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                                <path d="M13.5312 3.67969H0.812744" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                                <path d="M11.0804 3.67974C10.4917 3.67974 9.98468 3.26349 9.86918 2.68674L9.68693 1.77474C9.57443 1.35399 9.19343 1.06299 8.75918 1.06299H5.58443C5.15018 1.06299 4.76918 1.35399 4.65668 1.77474L4.47443 2.68674C4.35893 3.26349 3.85193 3.67974 3.26318 3.67974" stroke="currentColor" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
                            </svg>
                        </span>
                    </a>
                `;
            } else if (status === 'inprogress') {
                // Tab "Đang khám" - có nút Xem chi tiết, Hoàn thành, Tạm dừng và Hủy
                actionButtons = `
                    ${viewDetailButton}
                    <button type="button" class="badge rounded-0 bg-success-subtle fw-500 px-3 py-2 border-0 me-2" onclick="completeAppointment(${item.id})">Hoàn thành</button>
                    <button type="button" class="badge rounded-0 bg-warning-subtle fw-500 px-3 py-2 border-0 me-2" onclick="pauseAppointment(${item.id})">Tạm dừng</button>
                    <button type="button" class="badge rounded-0 bg-danger-subtle fw-500 px-3 py-2 border-0" onclick="cancelAppointment(${item.id})">Hủy</button>
                `;
            } else if (status === 'completed') {
                // Tab "Hoàn thành" - có nút Xem chi tiết, Accept và Cancel
                actionButtons = `
                    ${viewDetailButton}
                    <button type="button" class="badge rounded-0 bg-success-subtle fw-500 px-3 py-2 border-0 me-2" onclick="acceptAppointment(${item.id})">Accept</button>
                    <button type="button" class="badge rounded-0 bg-danger-subtle fw-500 px-3 py-2 border-0" onclick="cancelAppointment(${item.id})">Cancel</button>
                `;
            } else if (status === 'cancelled') {
                // Tab "Đã hủy" - chỉ có nút Xem chi tiết
                actionButtons = `
                    ${viewDetailButton}
                    <span class="badge bg-secondary">Đã hủy</span>
                `;
            }
            
            // Đảm bảo chỉ tạo 8 cột theo thứ tự: STT, Tên BN, Tên BS, Phòng khám, Ngày, Ca, Trạng thái, Hành động
            tr.innerHTML = `
                <th scope="row">${idx + 1}</th>
                <td>
                    <div class="d-flex align-items-center gap-3">
                        <img src="./assets/images/table/${10 + (idx % 4)}.png" class="img-fluid flex-shrink-0 icon-40 object-fit-cover" alt="icon">
                        <h5 class="mb-0">${patientName}</h5>
                    </div>
                </td>
                <td>${doctorName}</td>
                <td>${clinic}</td>
                <td>${appointmentDate ? (() => {
                    try {
                        return new Date(appointmentDate).toLocaleDateString('vi-VN');
                    } catch (e) {
                        return appointmentDate;
                    }
                })() : 'N/A'}</td>
                <td>${getShiftText(shift)}</td>
                <td>${getStatusText(item.status)}</td>
                <td>${actionButtons}</td>
            `;
            
            tbody.appendChild(tr);
        });
    }

    // Load appointment data for editing
    window.loadAppointmentForEdit = function(appointmentId) {
        fetch(`${API_BASE_URL}/api/appointment/detail/${appointmentId}`)
            .then(res => res.json())
            .then(data => {
                if (data && data.id) {
                    // Fill the edit form - hỗ trợ cả field cũ và mới
                    const editForm = document.getElementById('editAppointmentForm');
                    if (editForm) {
                        editForm.querySelector('#edit-appointment-id').value = data.id;
                        editForm.querySelector('#edit-patient-name').value = data.patientName || data.name || '';
                        editForm.querySelector('#edit-patient-email').value = data.patientEmail || data.email || '';
                        editForm.querySelector('#edit-doctor-name').value = data.doctorName || data.doctor?.name || '';
                        editForm.querySelector('#edit-clinic').value = data.clinic || data.clinicName || '';
                        
                        const appointmentDate = data.date || data.appointmentDate;
                        editForm.querySelector('#edit-date').value = appointmentDate ? new Date(appointmentDate).toISOString().slice(0,10) : '';
                        
                        const appointmentTime = data.time || data.startTime;
                        editForm.querySelector('#edit-time').value = appointmentTime ? appointmentTime.substring(0,5) : '';
                        
                        editForm.querySelector('#edit-reason').value = data.reason || data.shift || data.ca || '';
                        editForm.querySelector('#edit-type').value = data.patientType || data.type || 'New Patient';
                    }
                }
            })
            .catch(err => {
                console.error('Error loading appointment for edit:', err);
                alert('Lỗi khi tải thông tin lịch hẹn');
            });
    };

    // Delete appointment
    window.deleteAppointment = function(appointmentId) {
        if (confirm('Bạn có chắc chắn muốn xóa lịch hẹn này?')) {
            fetch(`${API_BASE_URL}/api/appointment/delete/${appointmentId}`, {
                method: 'DELETE'
            })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    alert('Xóa lịch hẹn thành công');
                    loadAppointments(); // Reload data
                } else {
                    alert('Lỗi khi xóa lịch hẹn: ' + data.message);
                }
            })
            .catch(err => {
                console.error('Error deleting appointment:', err);
                alert('Lỗi khi xóa lịch hẹn');
            });
        }
    };

    // Accept appointment
    window.acceptAppointment = function(appointmentId) {
        fetch(`${API_BASE_URL}/api/appointment/accept/${appointmentId}`, {
            method: 'PUT'
        })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                alert('Chấp nhận lịch hẹn thành công');
                loadAppointments(); // Reload data
            } else {
                alert('Lỗi khi chấp nhận lịch hẹn: ' + data.message);
            }
        })
        .catch(err => {
            console.error('Error accepting appointment:', err);
            alert('Lỗi khi chấp nhận lịch hẹn');
        });
    };

    // Cancel appointment
    window.cancelAppointment = function(appointmentId) {
        const reason = prompt('Nhập lý do hủy lịch hẹn:');
        if (reason !== null) {
            fetch(`${API_BASE_URL}/api/appointment/cancel/${appointmentId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ reason: reason })
            })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    alert('Hủy lịch hẹn thành công');
                    loadAppointments(); // Reload data
                } else {
                    alert('Lỗi khi hủy lịch hẹn: ' + data.message);
                }
            })
            .catch(err => {
                console.error('Error cancelling appointment:', err);
                alert('Lỗi khi hủy lịch hẹn');
            });
        }
    };

    // Update appointment
    window.updateAppointment = function() {
        const form = document.getElementById('editAppointmentForm');
        const formData = new FormData(form);
        
        const appointmentData = {
            id: formData.get('appointment-id'),
            patientName: formData.get('patient-name'),
            patientEmail: formData.get('patient-email'),
            doctorName: formData.get('doctor-name'),
            clinic: formData.get('clinic'),
            date: formData.get('date'),
            time: formData.get('time'),
            reason: formData.get('reason'),
            patientType: formData.get('type')
        };

        fetch(`${API_BASE_URL}/api/appointment/update`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(appointmentData)
        })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                alert('Cập nhật lịch hẹn thành công');
                // Close offcanvas
                const offcanvas = bootstrap.Offcanvas.getInstance(document.getElementById('offcanvasAppointmentEdit'));
                offcanvas.hide();
                loadAppointments(); // Reload data
            } else {
                alert('Lỗi khi cập nhật lịch hẹn: ' + data.message);
            }
        })
        .catch(err => {
            console.error('Error updating appointment:', err);
            alert('Lỗi khi cập nhật lịch hẹn');
        });
    };

    // Complete appointment (chuyển từ đang khám sang hoàn thành)
    window.completeAppointment = function(appointmentId) {
        if (confirm('Bạn có chắc chắn muốn hoàn thành lịch hẹn này?')) {
            fetch(`${API_BASE_URL}/api/appointment/complete/${appointmentId}`, {
                method: 'PUT'
            })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    alert('Hoàn thành lịch hẹn thành công');
                    loadAppointments(); // Reload data
                } else {
                    alert('Lỗi khi hoàn thành lịch hẹn: ' + data.message);
                }
            })
            .catch(err => {
                console.error('Error completing appointment:', err);
                alert('Lỗi khi hoàn thành lịch hẹn');
            });
        }
    };

    // Pause appointment (tạm dừng khám)
    window.pauseAppointment = function(appointmentId) {
        const reason = prompt('Nhập lý do tạm dừng khám:');
        if (reason !== null) {
            fetch(`${API_BASE_URL}/api/appointment/pause/${appointmentId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ reason: reason })
            })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    alert('Tạm dừng lịch hẹn thành công');
                    loadAppointments(); // Reload data
                } else {
                    alert('Lỗi khi tạm dừng lịch hẹn: ' + data.message);
                }
            })
            .catch(err => {
                console.error('Error pausing appointment:', err);
                alert('Lỗi khi tạm dừng lịch hẹn');
            });
        }
    };

    // View appointment detail
    window.viewAppointmentDetail = function(appointmentId) {
        fetch(`${API_BASE_URL}/api/appointment/detail/${appointmentId}`)
            .then(res => res.json())
            .then(data => {
                if (data && data.id) {
                    // Tạo modal hiển thị chi tiết lịch hẹn
                    const detailHtml = `
                        <div class="modal fade" id="appointmentDetailModal" tabindex="-1" aria-labelledby="appointmentDetailModalLabel" aria-hidden="true">
                            <div class="modal-dialog modal-lg">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <h5 class="modal-title" id="appointmentDetailModalLabel">Chi tiết lịch hẹn</h5>
                                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                                    </div>
                                    <div class="modal-body">
                                        <div class="row">
                                            <div class="col-md-6">
                                                <h6 class="fw-bold">Thông tin bệnh nhân</h6>
                                                <p><strong>Tên:</strong> ${data.name || 'N/A'}</p>
                                                <p><strong>Email:</strong> ${data.email || 'N/A'}</p>
                                                <p><strong>Loại:</strong> ${data.type || 'N/A'}</p>
                                            </div>
                                            <div class="col-md-6">
                                                <h6 class="fw-bold">Thông tin lịch hẹn</h6>
                                                <p><strong>Ngày:</strong> ${data.date ? new Date(data.date).toLocaleDateString('vi-VN') : 'N/A'}</p>
                                                <p><strong>Giờ:</strong> ${data.time || 'N/A'}</p>
                                                <p><strong>Ca:</strong> ${getShiftText(data.shift)}</p>
                                                <p><strong>Trạng thái:</strong> ${getStatusText(data.status)}</p>
                                            </div>
                                        </div>
                                        <div class="row mt-3">
                                            <div class="col-md-6">
                                                <h6 class="fw-bold">Thông tin bác sĩ</h6>
                                                <p><strong>Tên bác sĩ:</strong> ${data.doctorName || 'N/A'}</p>
                                            </div>
                                            <div class="col-md-6">
                                                <h6 class="fw-bold">Thông tin phòng khám</h6>
                                                <p><strong>Phòng khám:</strong> ${data.clinic || 'N/A'}</p>
                                            </div>
                                        </div>
                                        ${data.note ? `
                                        <div class="row mt-3">
                                            <div class="col-12">
                                                <h6 class="fw-bold">Ghi chú</h6>
                                                <p>${data.note}</p>
                                            </div>
                                        </div>
                                        ` : ''}
                                    </div>
                                    <div class="modal-footer">
                                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
                                    </div>
                                </div>
                            </div>
                        </div>
                    `;
                    
                    // Xóa modal cũ nếu có
                    const oldModal = document.getElementById('appointmentDetailModal');
                    if (oldModal) {
                        oldModal.remove();
                    }
                    
                    // Thêm modal mới vào body
                    document.body.insertAdjacentHTML('beforeend', detailHtml);
                    
                    // Hiển thị modal
                    const modal = new bootstrap.Modal(document.getElementById('appointmentDetailModal'));
                    modal.show();
                    
                    // Xóa modal khi đóng
                    document.getElementById('appointmentDetailModal').addEventListener('hidden.bs.modal', function() {
                        this.remove();
                    });
                } else {
                    alert('Không tìm thấy thông tin lịch hẹn');
                }
            })
            .catch(err => {
                console.error('Error loading appointment detail:', err);
                alert('Lỗi khi tải thông tin chi tiết lịch hẹn');
            });
    };

    // Load all appointments
    function loadAppointments() {
        fetch(`${API_BASE_URL}/api/appointment/list`)
            .then(res => {
                if (!res.ok) throw new Error('API error: ' + res.status);
                return res.json();
            })
            .then(data => {
                console.log('Appointments data:', data); // Debug log
                allAppointments = data;
                // Mặc định hiển thị tab "Sắp tới"
                renderTable('scheduled');
            })
            .catch(err => {
                console.error('Error loading appointments:', err);
                var activeTab = document.querySelector('.tab-pane.active');
                var tbody = activeTab ? activeTab.querySelector('tbody') : null;
                if (tbody) {
                    tbody.innerHTML = `<tr><td colspan="7" style="text-align:center;color:red;">Lỗi tải dữ liệu lịch hẹn: ${err.message}</td></tr>`;
                }
            });
    }

    document.addEventListener('DOMContentLoaded', function() {
        loadAppointments();

        // Gắn sự kiện cho các tab
        document.querySelectorAll('[data-bs-toggle="tab"]').forEach(btn => {
            btn.addEventListener('shown.bs.tab', function(e) {
                // Xác định trạng thái cần lọc dựa vào tab
                let status = 'scheduled';
                const target = e.target.getAttribute('data-bs-target');
                if (target === '#upcoming') status = 'scheduled';
                else if (target === '#inprogress') status = 'inprogress';
                else if (target === '#request') status = 'completed';
                else if (target === '#cancelled') status = 'cancelled';
                renderTable(status);
            });
        });

        // Gắn sự kiện cho form cập nhật
        const updateForm = document.getElementById('editAppointmentForm');
        if (updateForm) {
            updateForm.addEventListener('submit', function(e) {
                e.preventDefault();
                updateAppointment();
            });
        }
    });
})(); 