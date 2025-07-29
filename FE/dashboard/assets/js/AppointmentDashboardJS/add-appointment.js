// add-appointment.js
// Xử lý chức năng thêm lịch hẹn mới
(function() {
    // Load clinics for dropdown
    function loadClinics() {
        AppointmentAPI.utils.get(AppointmentAPI.clinics.list)
            .then(data => {
                const clinicSelect = document.getElementById('addClinic');
                if (clinicSelect && data) {
                    clinicSelect.innerHTML = '<option value="">Chọn phòng khám</option>';
                    data.forEach(clinic => {
                        clinicSelect.innerHTML += `<option value="${clinic.id}">${clinic.name}</option>`;
                    });
                }
            })
            .catch(err => {
                console.error('Error loading clinics:', err);
            });
    }

    // Load doctors for dropdown based on selected clinic
    function loadDoctors(clinicId) {
        if (!clinicId) {
            const doctorSelect = document.getElementById('addDoctor');
            if (doctorSelect) {
                doctorSelect.innerHTML = '<option value="">Chọn bác sĩ</option>';
                doctorSelect.disabled = true;
            }
            return;
        }

        AppointmentAPI.utils.get(AppointmentAPI.doctors.list + `?clinicId=${clinicId}`)
            .then(data => {
                const doctorSelect = document.getElementById('addDoctor');
                if (doctorSelect && data) {
                    doctorSelect.innerHTML = '<option value="">Chọn bác sĩ</option>';
                    data.forEach(doctor => {
                        doctorSelect.innerHTML += `<option value="${doctor.id}">${doctor.name}</option>`;
                    });
                    doctorSelect.disabled = false;
                }
            })
            .catch(err => {
                console.error('Error loading doctors:', err);
            });
    }

    // Load services for dropdown based on selected doctor
    function loadServices(doctorId) {
        if (!doctorId) {
            const serviceSelect = document.getElementById('addService');
            if (serviceSelect) {
                serviceSelect.innerHTML = '<option value="">Chọn dịch vụ</option>';
                serviceSelect.disabled = true;
            }
            return;
        }

        AppointmentAPI.utils.get(AppointmentAPI.services.list + `?doctorId=${doctorId}`)
            .then(data => {
                const serviceSelect = document.getElementById('addService');
                if (serviceSelect && data) {
                    serviceSelect.innerHTML = '<option value="">Chọn dịch vụ</option>';
                    data.forEach(service => {
                        serviceSelect.innerHTML += `<option value="${service.id}">${service.name}</option>`;
                    });
                    serviceSelect.disabled = false;
                }
            })
            .catch(err => {
                console.error('Error loading services:', err);
            });
    }

    // Load available shifts for selected doctor and date
    function loadShifts(doctorId, date) {
        if (!doctorId || !date) {
            const shiftSelect = document.getElementById('addShift');
            if (shiftSelect) {
                shiftSelect.innerHTML = '<option value="">Chọn ca khám</option>';
                shiftSelect.disabled = true;
            }
            return;
        }

        // Check if selected date is in the past
        const selectedDate = new Date(date);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        
        if (selectedDate < today) {
            showNotification('Không thể chọn ngày trong quá khứ', 'error');
            document.getElementById('addDate').value = '';
            const shiftSelect = document.getElementById('addShift');
            if (shiftSelect) {
                shiftSelect.innerHTML = '<option value="">Chọn ca khám</option>';
                shiftSelect.disabled = true;
            }
            return;
        }

        // Check if doctor works on this day
        const dayOfWeek = selectedDate.getDay(); // 0 = Sunday, 1 = Monday, etc.
        const dayNames = ['Chủ nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'];
        
        // Show loading state for shift dropdown
        const shiftSelect = document.getElementById('addShift');
        if (shiftSelect) {
            shiftSelect.innerHTML = '<option value="">Đang tải...</option>';
            shiftSelect.disabled = true;
        }

        // First check if doctor works on this day
        AppointmentAPI.utils.get(AppointmentAPI.doctors.workingDays(doctorId, selectedDate.getFullYear(), selectedDate.getMonth() + 1))
            .then(workingDays => {
                const isWorkingDay = workingDays.some(day => {
                    const workingDate = new Date(day.date);
                    return workingDate.toDateString() === selectedDate.toDateString();
                });

                if (!isWorkingDay) {
                    showNotification(`Bác sĩ không làm việc vào ${dayNames[dayOfWeek]} (${date})`, 'error');
                    document.getElementById('addDate').value = '';
                    if (shiftSelect) {
                        shiftSelect.innerHTML = '<option value="">Chọn ca khám</option>';
                        shiftSelect.disabled = true;
                    }
                    return;
                }

                // If doctor works on this day, load available shifts
                return AppointmentAPI.utils.get(AppointmentAPI.doctors.availableShifts(doctorId, date));
            })
            .then(data => {
                if (!data) return; // Doctor doesn't work on this day
                
                const shiftSelect = document.getElementById('addShift');
                if (shiftSelect && data) {
                    shiftSelect.innerHTML = '<option value="">Chọn ca khám</option>';
                    
                    if (data.length === 0) {
                        shiftSelect.innerHTML += '<option value="" disabled>Không có ca khám nào trong ngày này</option>';
                    } else {
                        data.forEach(shift => {
                            const isAvailable = shift.isAvailable !== false; // Default to available if not specified
                            const optionText = `${shift.name} (${shift.startTime} - ${shift.endTime})`;
                            const optionValue = isAvailable ? shift.id : '';
                            const disabledAttr = isAvailable ? '' : 'disabled';
                            
                            shiftSelect.innerHTML += `<option value="${optionValue}" ${disabledAttr}>${optionText}${!isAvailable ? ' - Đã đầy' : ''}</option>`;
                        });
                    }
                    shiftSelect.disabled = false;
                }
            })
            .catch(err => {
                console.error('Error loading shifts:', err);
                const shiftSelect = document.getElementById('addShift');
                if (shiftSelect) {
                    shiftSelect.innerHTML = '<option value="">Lỗi khi tải ca khám</option>';
                    shiftSelect.disabled = true;
                }
            });
    }

    // Show notification
    function showNotification(message, type = 'info') {
        // Check if SweetAlert2 is available
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: type === 'success' ? 'Thành công!' : type === 'error' ? 'Lỗi!' : 'Thông báo',
                text: message,
                icon: type,
                confirmButtonText: 'OK',
                confirmButtonColor: type === 'success' ? '#28a745' : type === 'error' ? '#dc3545' : '#007bff'
            });
        } else {
            // Fallback to alert
            alert(message);
        }
    }

    // Add new appointment
    window.addNewAppointment = function() {
        // Kiểm tra xem đã có patient ID chưa
        if (!window.currentPatientId) {
            showNotification('Vui lòng tìm kiếm hoặc tạo bệnh nhân trước', 'error');
            return;
        }

        // Show loading state
        const submitButton = document.querySelector('#offcanvasAppointmentAdd .btn-primary');
        const originalText = submitButton.innerHTML;
        submitButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Đang xử lý...';
        submitButton.disabled = true;

        const appointmentData = {
            patientId: window.currentPatientId,
            clinicId: document.getElementById('addClinic').value,
            doctorId: document.getElementById('addDoctor').value,
            serviceId: document.getElementById('addService').value,
            date: document.getElementById('addDate').value,
            shiftId: document.getElementById('addShift').value,
            note: document.getElementById('addNote').value.trim()
        };

        // Validate required fields
        if (!appointmentData.clinicId) {
            showNotification('Vui lòng chọn phòng khám', 'error');
            document.getElementById('addClinic').focus();
            resetButtonState();
            return;
        }

        if (!appointmentData.doctorId) {
            showNotification('Vui lòng chọn bác sĩ', 'error');
            document.getElementById('addDoctor').focus();
            resetButtonState();
            return;
        }

        if (!appointmentData.serviceId) {
            showNotification('Vui lòng chọn dịch vụ', 'error');
            document.getElementById('addService').focus();
            resetButtonState();
            return;
        }

        if (!appointmentData.date) {
            showNotification('Vui lòng chọn ngày khám', 'error');
            document.getElementById('addDate').focus();
            resetButtonState();
            return;
        }

        if (!appointmentData.shiftId) {
            showNotification('Vui lòng chọn ca khám', 'error');
            document.getElementById('addShift').focus();
            resetButtonState();
            return;
        }

        console.log('Sending appointment data:', appointmentData);

        AppointmentAPI.utils.post(AppointmentAPI.appointments.create, appointmentData)
        .then(data => {
            console.log('API response:', data);
            if (data.success || data.id) {
                showNotification('Thêm lịch hẹn thành công', 'success');
                // Reset form
                resetForm();
                // Close offcanvas
                const offcanvas = bootstrap.Offcanvas.getInstance(document.getElementById('offcanvasAppointmentAdd'));
                if (offcanvas) offcanvas.hide();
                // Reload appointments list
                if (window.loadAppointments) {
                    window.loadAppointments();
                }
                // Refresh appointment count
                if (window.refreshAppointmentCount) {
                    window.refreshAppointmentCount();
                }
            } else {
                showNotification('Lỗi khi thêm lịch hẹn: ' + (data.message || 'Unknown error'), 'error');
            }
        })
        .catch(err => {
            console.error('Error adding appointment:', err);
            showNotification('Lỗi khi thêm lịch hẹn: ' + err.message, 'error');
        })
        .finally(() => {
            resetButtonState();
        });

        function resetButtonState() {
            submitButton.innerHTML = originalText;
            submitButton.disabled = false;
        }
    };

    // Reset form
    function resetForm() {
        // Reset patient info
        const nameInput = document.getElementById('addName');
        const phoneInput = document.getElementById('addPhone');
        const emailInput = document.getElementById('addEmail');
        const addressInput = document.getElementById('addAddress');
        const birthDateInput = document.getElementById('addBirthDate');
        const genderInput = document.getElementById('addGender');

        if (nameInput) {
            nameInput.value = '';
            nameInput.disabled = false;
        }
        if (phoneInput) {
            phoneInput.value = '';
            phoneInput.disabled = false;
        }
        if (emailInput) {
            emailInput.value = '';
            emailInput.disabled = false;
        }
        if (addressInput) {
            addressInput.value = '';
            addressInput.disabled = false;
        }
        if (birthDateInput) {
            birthDateInput.value = '';
            birthDateInput.disabled = false;
        }
        if (genderInput) {
            genderInput.value = '';
            genderInput.disabled = false;
        }

        // Reset appointment fields
        document.getElementById('addClinic').value = '';
        document.getElementById('addDoctor').innerHTML = '<option value="">Chọn bác sĩ</option>';
        document.getElementById('addDoctor').disabled = true;
        document.getElementById('addService').innerHTML = '<option value="">Chọn dịch vụ</option>';
        document.getElementById('addService').disabled = true;
        document.getElementById('addDate').value = '';
        document.getElementById('addShift').innerHTML = '<option value="">Chọn ca khám</option>';
        document.getElementById('addShift').disabled = true;
        document.getElementById('addNote').value = '';

        // Reset patient ID
        window.currentPatientId = null;
    }

    // Initialize date picker
    function initializeDatePicker() {
        const dateInput = document.getElementById('addDate');
        if (dateInput) {
            // Set minimum date to today
            const today = new Date().toISOString().split('T')[0];
            dateInput.min = today;
            
            // Add change event listener
            dateInput.addEventListener('change', function() {
                const doctorId = document.getElementById('addDoctor').value;
                const date = this.value;
                if (doctorId && date) {
                    loadShifts(doctorId, date);
                }
            });
        }
    }

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        // Load clinics on page load
        loadClinics();
        
        // Initialize date picker
        initializeDatePicker();

        // Add event listeners for cascading dropdowns
        const clinicSelect = document.getElementById('addClinic');
        if (clinicSelect) {
            clinicSelect.addEventListener('change', function() {
                const clinicId = this.value;
                loadDoctors(clinicId);
                // Reset dependent dropdowns
                document.getElementById('addService').innerHTML = '<option value="">Chọn dịch vụ</option>';
                document.getElementById('addService').disabled = true;
                document.getElementById('addShift').innerHTML = '<option value="">Chọn ca khám</option>';
                document.getElementById('addShift').disabled = true;
            });
        }

        const doctorSelect = document.getElementById('addDoctor');
        if (doctorSelect) {
            doctorSelect.addEventListener('change', function() {
                const doctorId = this.value;
                loadServices(doctorId);
                // Reset shift dropdown
                document.getElementById('addShift').innerHTML = '<option value="">Chọn ca khám</option>';
                document.getElementById('addShift').disabled = true;
            });
        }

        const serviceSelect = document.getElementById('addService');
        if (serviceSelect) {
            serviceSelect.addEventListener('change', function() {
                const doctorId = document.getElementById('addDoctor').value;
                const date = document.getElementById('addDate').value;
                if (doctorId && date) {
                    loadShifts(doctorId, date);
                }
            });
        }

        // Bind form submit event
        const submitButton = document.querySelector('#offcanvasAppointmentAdd .btn-primary');
        if (submitButton) {
            submitButton.addEventListener('click', function(e) {
                e.preventDefault();
                addNewAppointment();
            });
        }

        // Auto focus on phone field when offcanvas opens
        const offcanvas = document.getElementById('offcanvasAppointmentAdd');
        if (offcanvas) {
            offcanvas.addEventListener('shown.bs.offcanvas', function() {
                const phoneInput = document.getElementById('addPhone');
                if (phoneInput) {
                    phoneInput.focus();
                }
            });
        }
    });
})(); 