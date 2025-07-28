// Edit Appointment Functionality with Cascade Logic
class AppointmentEditor {
    constructor() {
        this.currentAppointment = null;
        this.selectedClinic = null;
        this.selectedDoctor = null;
        this.selectedService = null;
        this.selectedDate = null;
        this.selectedShift = null;
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.loadDropdownData();
        this.setupDateValidation();
    }

    setupEventListeners() {
        // Save button click
        const saveButton = document.getElementById('saveEditAppointment');
        if (saveButton) {
            saveButton.addEventListener('click', () => {
                this.saveAppointment();
            });
        }

        // Modal hidden event to reset form
        const modal = document.getElementById('editAppointmentModal');
        if (modal) {
            modal.addEventListener('hidden.bs.modal', () => {
                this.resetForm();
            });
        }

        // Clinic change event to load doctors
        const clinicSelect = document.getElementById('editClinic');
        if (clinicSelect) {
            clinicSelect.addEventListener('change', (e) => {
                this.handleClinicChange(e.target.value);
            });
        }

        // Doctor change event to load services
        const doctorSelect = document.getElementById('editDoctor');
        if (doctorSelect) {
            doctorSelect.addEventListener('change', (e) => {
                this.handleDoctorChange(e.target.value);
            });
        }

        // Service change event to enable date selection
        const serviceSelect = document.getElementById('editService');
        if (serviceSelect) {
            serviceSelect.addEventListener('change', (e) => {
                this.handleServiceChange(e.target.value);
            });
        }

        // Date change event to enable shift selection
        const dateInput = document.getElementById('editAppointmentDate');
        if (dateInput) {
            dateInput.addEventListener('change', (e) => {
                this.handleDateChange(e.target.value);
            });
        }

        // Shift change event
        const shiftSelect = document.getElementById('editShift');
        if (shiftSelect) {
            shiftSelect.addEventListener('change', (e) => {
                this.handleShiftChange(e.target.value);
            });
        }
    }

    setupDateValidation() {
        const dateInput = document.getElementById('editAppointmentDate');
        if (dateInput) {
            // Set min date to today
            const today = new Date().toISOString().split('T')[0];
            dateInput.min = today;
            
            // Prevent selecting past dates
            dateInput.addEventListener('input', (e) => {
                const selectedDate = new Date(e.target.value);
                const today = new Date();
                today.setHours(0, 0, 0, 0);
                
                if (selectedDate < today) {
                    alert('Không thể chọn ngày trong quá khứ!');
                    e.target.value = today.toISOString().split('T')[0];
                }
            });
        }
    }

    // Handle clinic selection
    async handleClinicChange(clinicId) {
        console.log('🏥 Clinic changed to:', clinicId);
        
        if (!clinicId) {
            this.resetDoctorAndService();
            return;
        }

        this.selectedClinic = clinicId;
        
        // Reset doctor and service
        this.resetDoctorAndService();
        
        // Load doctors for this clinic
        await this.loadDoctorsByClinic(clinicId);
        
        // Enable doctor selection
        const doctorSelect = document.getElementById('editDoctor');
        if (doctorSelect) {
            doctorSelect.disabled = false;
        }
    }

    // Handle doctor selection
    async handleDoctorChange(doctorId) {
        console.log('👨‍⚕️ Doctor changed to:', doctorId);
        
        if (!doctorId) {
            this.resetService();
            this.resetDateAndShift();
            return;
        }

        this.selectedDoctor = doctorId;
        
        // Reset service
        this.resetService();
        
        // Load services for this doctor
        await this.loadServicesByDoctor(doctorId);
        
        // Enable service selection
        const serviceSelect = document.getElementById('editService');
        if (serviceSelect) {
            serviceSelect.disabled = false;
        }
        
        // Enable date selection when doctor is selected
        const dateInput = document.getElementById('editAppointmentDate');
        if (dateInput) {
            dateInput.disabled = false;
        }
    }

    // Handle service selection
    handleServiceChange(serviceId) {
        console.log('🩺 Service changed to:', serviceId);
        
        if (!serviceId) {
            this.resetShift();
            return;
        }

        this.selectedService = serviceId;
        
        // Note: Date is already enabled when doctor is selected
        // No need to enable date here
    }

    // Handle date selection
    async handleDateChange(dateValue) {
        console.log('📅 Date changed to:', dateValue);
        
        if (!dateValue) {
            this.resetShift();
            return;
        }

        this.selectedDate = dateValue;
        
        // Check doctor availability for this date
        const isAvailable = await this.checkDoctorAvailability(dateValue);
        
        if (isAvailable) {
            // Enable shift selection
            const shiftSelect = document.getElementById('editShift');
            if (shiftSelect) {
                shiftSelect.disabled = false;
            }
        } else {
            alert('Bác sĩ không làm việc vào ngày này hoặc đang nghỉ phép!');
            this.resetShift();
        }
    }

    // Handle shift selection
    handleShiftChange(shiftValue) {
        console.log('⏰ Shift changed to:', shiftValue);
        this.selectedShift = shiftValue;
    }

    // Reset doctor and service dropdowns
    resetDoctorAndService() {
        const doctorSelect = document.getElementById('editDoctor');
        const serviceSelect = document.getElementById('editService');
        
        if (doctorSelect) {
            doctorSelect.innerHTML = '<option value="">Chọn bác sĩ</option>';
            doctorSelect.disabled = true;
            doctorSelect.value = '';
        }
        
        if (serviceSelect) {
            serviceSelect.innerHTML = '<option value="">Chọn dịch vụ</option>';
            serviceSelect.disabled = true;
            serviceSelect.value = '';
        }
        
        this.selectedDoctor = null;
        this.selectedService = null;
    }

    // Reset service dropdown
    resetService() {
        const serviceSelect = document.getElementById('editService');
        if (serviceSelect) {
            serviceSelect.innerHTML = '<option value="">Chọn dịch vụ</option>';
            serviceSelect.disabled = true;
            serviceSelect.value = '';
        }
        this.selectedService = null;
        
        // Also reset date and shift when service is reset
        this.resetDateAndShift();
    }

    // Reset date and shift
    resetDateAndShift() {
        const dateInput = document.getElementById('editAppointmentDate');
        const shiftSelect = document.getElementById('editShift');
        
        if (dateInput) {
            dateInput.disabled = true;
            dateInput.value = '';
        }
        
        if (shiftSelect) {
            shiftSelect.disabled = true;
            shiftSelect.value = '';
        }
        
        this.selectedDate = null;
        this.selectedShift = null;
    }

    // Reset shift
    resetShift() {
        const shiftSelect = document.getElementById('editShift');
        if (shiftSelect) {
            shiftSelect.disabled = true;
            shiftSelect.value = '';
        }
        this.selectedShift = null;
    }

    // Check doctor availability
    async checkDoctorAvailability(date) {
        try {
            if (!this.selectedDoctor) return false;
            
            const response = await fetch(`https://localhost:7097/api/appointment/check-availability?doctorId=${this.selectedDoctor}&date=${date}`);
            
            if (response.ok) {
                const result = await response.json();
                return result.available || false;
            }
            
            return false;
        } catch (error) {
            console.error('❌ Error checking doctor availability:', error);
            return false;
        }
    }

    // Load dropdown data
    async loadDropdownData() {
        try {
            console.log('🔄 Loading dropdown data...');
            await this.loadClinics();
            console.log('✅ Dropdown data loaded successfully');
        } catch (error) {
            console.error('❌ Error loading dropdown data:', error);
        }
    }

    // Load clinics
    async loadClinics() {
        try {
            console.log('🏥 Loading clinics...');
            const response = await fetch('https://localhost:7097/api/appointment/clinic/list');
            
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
            
            const result = await response.json();
            const data = result.data || result;
            
            const clinicSelect = document.getElementById('editClinic');
            if (clinicSelect) {
                clinicSelect.innerHTML = '<option value="">Chọn phòng khám</option>';
                
                if (Array.isArray(data)) {
                    data.forEach(clinic => {
                        const option = document.createElement('option');
                        option.value = clinic.id;
                        option.textContent = clinic.name;
                        clinicSelect.appendChild(option);
                    });
                }
            }
            console.log('✅ Clinics loaded:', data);
        } catch (error) {
            console.error('❌ Error loading clinics:', error);
        }
    }

    // Load doctors by clinic
    async loadDoctorsByClinic(clinicId) {
        try {
            console.log('👨‍⚕️ Loading doctors for clinic:', clinicId);
            const response = await fetch(`https://localhost:7097/api/appointment/doctor/list?clinicId=${clinicId}`);
            
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
            
            const result = await response.json();
            const data = result.data || result;
            
            const doctorSelect = document.getElementById('editDoctor');
            if (doctorSelect) {
                doctorSelect.innerHTML = '<option value="">Chọn bác sĩ</option>';
                
                if (Array.isArray(data)) {
                    data.forEach(doctor => {
                        const option = document.createElement('option');
                        option.value = doctor.id;
                        option.textContent = doctor.name;
                        doctorSelect.appendChild(option);
                    });
                }
            }
            console.log('✅ Doctors loaded:', data);
        } catch (error) {
            console.error('❌ Error loading doctors:', error);
        }
    }

    // Load services by doctor
    async loadServicesByDoctor(doctorId) {
        try {
            console.log('🩺 Loading services for doctor:', doctorId);
            const response = await fetch(`https://localhost:7097/api/appointment/service/list?doctorId=${doctorId}`);
            
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
            
            const result = await response.json();
            const data = result.data || result;
            
            const serviceSelect = document.getElementById('editService');
            if (serviceSelect) {
                serviceSelect.innerHTML = '<option value="">Chọn dịch vụ</option>';
                
                if (Array.isArray(data)) {
                    data.forEach(service => {
                        const option = document.createElement('option');
                        option.value = service.id;
                        option.textContent = service.name;
                        serviceSelect.appendChild(option);
                    });
                }
            }
            console.log('✅ Services loaded:', data);
        } catch (error) {
            console.error('❌ Error loading services:', error);
        }
    }

    // Open edit modal
    async openEditModal(appointmentId) {
        try {
            console.log('🔄 Opening edit modal for appointment:', appointmentId);
            
            // Load appointment details
            console.log('🔍 Fetching appointment details for ID:', appointmentId);
            const response = await fetch(`https://localhost:7097/api/appointment/detail/${appointmentId}`);
            
            if (!response.ok) {
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
            
            const result = await response.json();
            console.log('📋 Appointment response received:', result);
            
            // Kiểm tra format response
            if (!result || result.success === false) {
                const errorMsg = result?.message || 'Không tìm thấy thông tin lịch hẹn!';
                alert(errorMsg);
                return;
            }
            
            // Lấy dữ liệu appointment
            const appointment = result.data || result;
            
            if (!appointment || !appointment.id) {
                alert('Dữ liệu lịch hẹn không hợp lệ!');
                return;
            }

            this.currentAppointment = appointment;
            this.populateForm(appointment);
            
            // Show modal
            const modalElement = document.getElementById('editAppointmentModal');
            if (modalElement) {
                const modal = new bootstrap.Modal(modalElement);
                modal.show();
            } else {
                console.error('❌ Modal element not found');
                alert('Không tìm thấy modal edit!');
            }
            
        } catch (error) {
            console.error('❌ Error opening edit modal:', error);
            
            // Hiển thị thông tin lỗi chi tiết hơn
            let errorMessage = 'Lỗi khi tải thông tin lịch hẹn!';
            
            if (error.message.includes('404')) {
                errorMessage = 'Không tìm thấy lịch hẹn với ID này!';
            } else if (error.message.includes('500')) {
                errorMessage = 'Lỗi server! Vui lòng thử lại sau.';
            } else if (error.message.includes('fetch')) {
                errorMessage = 'Không thể kết nối đến server! Vui lòng kiểm tra kết nối.';
            }
            
            alert(errorMessage);
        }
    }

    // Populate form with appointment data
    populateForm(appointment) {
        console.log('📝 Populating form with appointment data:', appointment);
        
        try {
            // Set appointment ID
            const idField = document.getElementById('editAppointmentId');
            if (idField) {
                idField.value = appointment.id || '';
            }
            
            // Set patient name
            const patientNameField = document.getElementById('editPatientName');
            if (patientNameField) {
                const patientName = appointment.patientName || 
                                   appointment.name || 
                                   appointment.patient?.name || 
                                   '';
                patientNameField.value = patientName;
            }
            
            // Set patient phone
            const patientPhoneField = document.getElementById('editPatientPhone');
            if (patientPhoneField) {
                const patientPhone = appointment.patientPhone || 
                                    appointment.phone || 
                                    appointment.patient?.phone || 
                                    '';
                patientPhoneField.value = patientPhone;
            }
            
            // Set appointment date
            const dateField = document.getElementById('editAppointmentDate');
            if (dateField && appointment.appointmentDate) {
                try {
                    const date = new Date(appointment.appointmentDate);
                    if (!isNaN(date.getTime())) {
                        dateField.value = date.toISOString().split('T')[0];
                        dateField.disabled = false;
                    }
                } catch (e) {
                    console.warn('⚠️ Invalid appointment date:', appointment.appointmentDate);
                }
            }
            
            // Set shift
            const shiftField = document.getElementById('editShift');
            if (shiftField && appointment.shift) {
                shiftField.value = appointment.shift.toLowerCase();
                shiftField.disabled = false;
            }
            
            // Set status
            const statusField = document.getElementById('editStatus');
            if (statusField && appointment.status !== undefined && appointment.status !== null) {
                const statusValue = typeof appointment.status === 'number' ? 
                    this.getStatusString(appointment.status) : appointment.status;
                statusField.value = statusValue;
            }
            
            // Set clinic and trigger cascade
            const clinicField = document.getElementById('editClinic');
            if (clinicField && appointment.clinicId) {
                clinicField.value = appointment.clinicId;
                this.handleClinicChange(appointment.clinicId);
            }
            
            // Set doctor (after doctors are loaded)
            setTimeout(async () => {
                const doctorField = document.getElementById('editDoctor');
                if (doctorField && appointment.doctorId) {
                    doctorField.value = appointment.doctorId;
                    this.handleDoctorChange(appointment.doctorId);
                }
            }, 500);
            
            // Set service (after services are loaded)
            setTimeout(() => {
                const serviceField = document.getElementById('editService');
                if (serviceField && appointment.serviceId) {
                    serviceField.value = appointment.serviceId;
                    this.handleServiceChange(appointment.serviceId);
                }
            }, 1000);
            
            // Set note
            const noteField = document.getElementById('editNote');
            if (noteField) {
                noteField.value = appointment.note || '';
            }
            
            console.log('✅ Form populated successfully');
            
        } catch (error) {
            console.error('❌ Error populating form:', error);
            alert('Lỗi khi điền dữ liệu vào form!');
        }
    }

    // Save appointment
    async saveAppointment() {
        try {
            const form = document.getElementById('editAppointmentForm');
            if (!form) {
                alert('Không tìm thấy form!');
                return;
            }
            
            const formData = new FormData(form);
            
            // Validate form
            if (!form.checkValidity()) {
                form.reportValidity();
                return;
            }
            
            // Validate cascade logic
            if (!this.validateCascadeLogic()) {
                return;
            }
            
            // Prepare data
            const appointmentData = {
                id: parseInt(formData.get('appointmentId')),
                patientName: formData.get('patientName'),
                patientPhone: formData.get('patientPhone'),
                appointmentDate: formData.get('appointmentDate'),
                shift: formData.get('shift'),
                status: formData.get('status'),
                clinicId: parseInt(formData.get('clinicId')),
                doctorId: parseInt(formData.get('doctorId')),
                serviceId: parseInt(formData.get('serviceId')),
                note: formData.get('note')
            };
            
            console.log('💾 Saving appointment data:', appointmentData);
            
            // Show loading
            const saveButton = document.getElementById('saveEditAppointment');
            if (saveButton) {
                const originalText = saveButton.textContent;
                saveButton.textContent = 'Đang lưu...';
                saveButton.disabled = true;
            }
            
            // Send update request
            const response = await fetch('https://localhost:7097/api/appointment/update', {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(appointmentData)
            });
            
            const result = await response.json();
            console.log('📋 Update response:', result);
            
            if (response.ok && result.success) {
                // Show success message
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        icon: 'success',
                        title: 'Thành công!',
                        text: 'Đã cập nhật lịch hẹn thành công.',
                        timer: 2000,
                        showConfirmButton: false
                    });
                } else {
                    alert('Đã cập nhật lịch hẹn thành công!');
                }
                
                // Close modal
                const modalElement = document.getElementById('editAppointmentModal');
                if (modalElement) {
                    const modal = bootstrap.Modal.getInstance(modalElement);
                    if (modal) {
                        modal.hide();
                    }
                }
                
                // Reload appointments
                if (window.AppointmentLoader) {
                    window.AppointmentLoader.loadAppointments();
                }
                
            } else {
                throw new Error(result.message || 'Lỗi khi cập nhật lịch hẹn');
            }
            
        } catch (error) {
            console.error('❌ Error saving appointment:', error);
            
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: error.message || 'Có lỗi xảy ra khi cập nhật lịch hẹn.'
                });
            } else {
                alert('Lỗi: ' + (error.message || 'Có lỗi xảy ra khi cập nhật lịch hẹn.'));
            }
            
        } finally {
            // Reset button
            const saveButton = document.getElementById('saveEditAppointment');
            if (saveButton) {
                saveButton.textContent = 'Lưu thay đổi';
                saveButton.disabled = false;
            }
        }
    }

    // Validate cascade logic
    validateCascadeLogic() {
        const clinicId = document.getElementById('editClinic')?.value;
        const doctorId = document.getElementById('editDoctor')?.value;
        const serviceId = document.getElementById('editService')?.value;
        const date = document.getElementById('editAppointmentDate')?.value;
        const shift = document.getElementById('editShift')?.value;
        
        if (!clinicId) {
            alert('Vui lòng chọn phòng khám!');
            return false;
        }
        
        if (!doctorId) {
            alert('Vui lòng chọn bác sĩ!');
            return false;
        }
        
        if (!serviceId) {
            alert('Vui lòng chọn dịch vụ!');
            return false;
        }
        
        if (!date) {
            alert('Vui lòng chọn ngày hẹn!');
            return false;
        }
        
        if (!shift) {
            alert('Vui lòng chọn ca khám!');
            return false;
        }
        
        return true;
    }

    // Convert status enum to string
    getStatusString(statusEnum) {
        const statusMap = {
            0: 'Scheduled',    // Scheduled
            1: 'InProgress',   // InProgress
            2: 'Completed',    // Completed
            3: 'Cancelled'     // Cancelled
        };
        return statusMap[statusEnum] || 'Scheduled';
    }

    // Reset form
    resetForm() {
        const form = document.getElementById('editAppointmentForm');
        if (form) {
            form.reset();
        }
        
        // Reset cascade state
        this.selectedClinic = null;
        this.selectedDoctor = null;
        this.selectedService = null;
        this.selectedDate = null;
        this.selectedShift = null;
        
        // Reset dropdowns
        this.resetDoctorAndService();
        this.resetDateAndShift();
        
        // Disable all dependent fields
        const doctorSelect = document.getElementById('editDoctor');
        const serviceSelect = document.getElementById('editService');
        const dateInput = document.getElementById('editAppointmentDate');
        const shiftSelect = document.getElementById('editShift');
        
        if (doctorSelect) doctorSelect.disabled = true;
        if (serviceSelect) serviceSelect.disabled = true;
        if (dateInput) dateInput.disabled = true;
        if (shiftSelect) shiftSelect.disabled = true;
        
        this.currentAppointment = null;
        console.log('🔄 Form reset');
    }
}

// Initialize appointment editor
let appointmentEditor;

// Wait for DOM to load
document.addEventListener('DOMContentLoaded', function() {
    console.log('🚀 Initializing AppointmentEditor...');
    appointmentEditor = new AppointmentEditor();
    console.log('✅ AppointmentEditor initialized');
});

// Global function to open edit modal
window.editAppointment = function(appointmentId) {
    console.log('🎯 editAppointment called with ID:', appointmentId);
    if (appointmentEditor) {
        appointmentEditor.openEditModal(appointmentId);
    } else {
        console.error('❌ AppointmentEditor not initialized');
        alert('Hệ thống chưa sẵn sàng! Vui lòng thử lại.');
    }
}; 