// add-appointment.js
// Xử lý chức năng thêm lịch hẹn mới
(function() {
    const API_BASE_URL = 'https://localhost:7097';

    // Load doctors for dropdown
    function loadDoctors() {
        fetch(`${API_BASE_URL}/api/appointment/doctor/list`)
            .then(res => res.json())
            .then(data => {
                const doctorSelect = document.getElementById('add_doctor');
                if (doctorSelect && data) {
                    doctorSelect.innerHTML = '<option value="">Chọn bác sĩ</option>';
                    data.forEach(doctor => {
                        doctorSelect.innerHTML += `<option value="${doctor.id}">${doctor.name}</option>`;
                    });
                }
            })
            .catch(err => {
                console.error('Error loading doctors:', err);
            });
    }

    // Load clinics for dropdown
    function loadClinics() {
        fetch(`${API_BASE_URL}/api/appointment/clinic/list`)
            .then(res => res.json())
            .then(data => {
                const clinicSelect = document.getElementById('add_clinic');
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

    // Load services for dropdown
    function loadServices() {
        fetch(`${API_BASE_URL}/api/appointment/service/list`)
            .then(res => res.json())
            .then(data => {
                const serviceSelect = document.getElementById('add_service');
                if (serviceSelect && data) {
                    serviceSelect.innerHTML = '<option value="">Chọn dịch vụ</option>';
                    data.forEach(service => {
                        serviceSelect.innerHTML += `<option value="${service.id}">${service.name}</option>`;
                    });
                }
            })
            .catch(err => {
                console.error('Error loading services:', err);
            });
    }

    // Add new appointment
    window.addNewAppointment = function() {
        const form = document.getElementById('addAppointmentForm');
        const formData = new FormData(form);
        
        // Hỗ trợ cả field cũ và mới
        const appointmentData = {
            // Field mới
            name: formData.get('patient_name'),
            email: formData.get('patient_email'),
            phone: formData.get('patient_phone'),
            doctorId: formData.get('doctor'),
            clinicId: formData.get('clinic'),
            serviceId: formData.get('service'),
            date: formData.get('appointment_date'),
            time: formData.get('start_time'),
            shift: formData.get('reason'), // Lý do/ca
            type: formData.get('patient_type'),
            note: formData.get('note'),
            
            // Field cũ (backward compatibility)
            patientName: formData.get('patient_name'),
            patientEmail: formData.get('patient_email'),
            patientPhone: formData.get('patient_phone'),
            appointmentDate: formData.get('appointment_date'),
            startTime: formData.get('start_time'),
            reason: formData.get('reason'),
            patientType: formData.get('patient_type')
        };

        // Validate required fields
        if (!appointmentData.name || !appointmentData.doctorId || !appointmentData.date) {
            alert('Vui lòng điền đầy đủ thông tin bắt buộc');
            return;
        }

        console.log('Sending appointment data:', appointmentData); // Debug log

        fetch(`${API_BASE_URL}/api/appointment/create`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(appointmentData)
        })
        .then(res => {
            if (!res.ok) {
                throw new Error(`HTTP error! status: ${res.status}`);
            }
            return res.json();
        })
        .then(data => {
            console.log('API response:', data); // Debug log
            if (data.success || data.id) {
                alert('Thêm lịch hẹn thành công');
                // Reset form
                form.reset();
                // Close modal
                const modal = bootstrap.Modal.getInstance(document.getElementById('addAppointmentModal'));
                if (modal) modal.hide();
                // Reload appointments list
                if (window.loadAppointments) {
                    window.loadAppointments();
                }
            } else {
                alert('Lỗi khi thêm lịch hẹn: ' + (data.message || 'Unknown error'));
            }
        })
        .catch(err => {
            console.error('Error adding appointment:', err);
            alert('Lỗi khi thêm lịch hẹn: ' + err.message);
        });
    };

    // Initialize date picker for add appointment form
    function initializeDatePickers() {
        // Initialize appointment date picker
        const appointmentDateInput = document.getElementById('appointment_date');
        if (appointmentDateInput) {
            // Check if flatpickr is available
            if (typeof flatpickr !== 'undefined') {
                flatpickr(appointmentDateInput, {
                    dateFormat: "Y-m-d",
                    minDate: "today",
                    locale: "vi"
                });
            } else {
                // Fallback to HTML5 date input
                appointmentDateInput.type = 'date';
                appointmentDateInput.min = new Date().toISOString().split('T')[0];
            }
        }

        // Initialize time picker
        const startTimeInput = document.getElementById('start_time');
        if (startTimeInput) {
            // Check if flatpickr is available
            if (typeof flatpickr !== 'undefined') {
                flatpickr(startTimeInput, {
                    enableTime: true,
                    noCalendar: true,
                    dateFormat: "H:i",
                    time_24hr: true,
                    minuteIncrement: 15
                });
            } else {
                // Fallback to HTML5 time input
                startTimeInput.type = 'time';
            }
        }
    }

    // Show add appointment modal
    window.showAddAppointmentModal = function() {
        // Load data for dropdowns
        loadDoctors();
        loadClinics();
        loadServices();
        
        // Initialize date pickers
        initializeDatePickers();
        
        // Show modal
        const modal = new bootstrap.Modal(document.getElementById('addAppointmentModal'));
        modal.show();
    };

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        // Bind form submit event
        const addForm = document.getElementById('addAppointmentForm');
        if (addForm) {
            addForm.addEventListener('submit', function(e) {
                e.preventDefault();
                addNewAppointment();
            });
        }

        // Bind add button click event
        const addButton = document.querySelector('[data-bs-target="#addAppointmentModal"]');
        if (addButton) {
            addButton.addEventListener('click', function() {
                showAddAppointmentModal();
            });
        }
    });
})(); 