// tab-appointment-system.js
// Hệ thống 2 tab cho thêm cuộc hẹn: Bệnh nhân cũ và Bệnh nhân mới

(function() {
    'use strict';

    // Global variables
    let currentPatientId = null;
    let currentPatientData = null;

    // Initialize tab system
    function initializeTabSystem() {
        const offcanvasBody = document.querySelector('#offcanvasAppointmentAdd .offcanvas-body');
        
        // Inject the tab structure
        offcanvasBody.innerHTML = `
            <ul class="nav nav-tabs" id="appointmentTabs" role="tablist">
                <li class="nav-item" role="presentation">
                    <button class="nav-link active" id="existing-patient-tab" data-bs-toggle="tab" data-bs-target="#existing-patient" type="button" role="tab" aria-controls="existing-patient" aria-selected="true">
                        Bệnh nhân cũ
                    </button>
                </li>
                <li class="nav-item" role="presentation">
                    <button class="nav-link" id="new-patient-tab" data-bs-toggle="tab" data-bs-target="#new-patient" type="button" role="tab" aria-controls="new-patient" aria-selected="false">
                        Bệnh nhân mới
                    </button>
                </li>
            </ul>
            
            <div class="tab-content mt-3" id="appointmentTabContent">
                <!-- Tab 1: Existing Patient -->
                <div class="tab-pane fade show active" id="existing-patient" role="tabpanel" aria-labelledby="existing-patient-tab">
                    <div class="mb-3">
                        <label for="searchPatientInput" class="form-label">Tìm kiếm bệnh nhân</label>
                        <div class="input-group">
                            <input type="text" class="form-control" id="searchPatientInput" placeholder="Nhập số điện thoại hoặc CCCD">
                            <button class="btn btn-primary" type="button" id="searchPatientBtn" onclick="searchPatient()">
                                <i class="fas fa-search"></i> Tìm kiếm
                            </button>
                        </div>
                    </div>
                    
                    <div id="patientSearchResults" style="display: none;" class="alert alert-success">
                        <h6>Thông tin bệnh nhân:</h6>
                        <div id="patientInfo"></div>
                    </div>
                    
                    <div id="noPatientFound" style="display: none;" class="alert alert-warning">
                        <i class="fas fa-exclamation-triangle"></i>
                        Không tìm thấy bệnh nhân. Vui lòng chuyển sang tab "Bệnh nhân mới" để đăng ký.
                    </div>
                </div>
                
                <!-- Tab 2: New Patient -->
                <div class="tab-pane fade" id="new-patient" role="tabpanel" aria-labelledby="new-patient-tab">
                    <div class="row">
                        <div class="col-md-6">
                            <div class="mb-3">
                                <label for="addCCCD" class="form-label">CCCD <span class="text-danger">*</span></label>
                                <input type="text" class="form-control" id="addCCCD" placeholder="Nhập CCCD">
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="mb-3">
                                <label for="addPhoneNumber" class="form-label">Số điện thoại <span class="text-danger">*</span></label>
                                <input type="text" class="form-control" id="addPhoneNumber" placeholder="Nhập số điện thoại">
                            </div>
                        </div>
                    </div>
                    
                    <div class="row">
                        <div class="col-md-6">
                            <div class="mb-3">
                                <label for="addEmergencyContact" class="form-label">Liên hệ khẩn cấp <span class="text-danger">*</span></label>
                                <input type="text" class="form-control" id="addEmergencyContact" placeholder="Nhập số điện thoại liên hệ khẩn cấp">
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="mb-3">
                                <label for="addName" class="form-label">Họ và tên <span class="text-danger">*</span></label>
                                <input type="text" class="form-control" id="addName" placeholder="Nhập họ và tên">
                            </div>
                        </div>
                    </div>
                    
                    <div class="row">
                        <div class="col-md-6">
                            <div class="mb-3">
                                <label for="addEmail" class="form-label">Email <span class="text-danger">*</span></label>
                                <input type="email" class="form-control" id="addEmail" placeholder="Nhập email">
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="mb-3">
                                <label for="addBirthDate" class="form-label">Ngày sinh</label>
                                <input type="date" class="form-control" id="addBirthDate">
                            </div>
                        </div>
                    </div>
                    
                    <div class="row">
                        <div class="col-md-6">
                            <div class="mb-3">
                                <label for="addGender" class="form-label">Giới tính</label>
                                <select class="form-select" id="addGender">
                                    <option value="0">Nam</option>
                                    <option value="1">Nữ</option>
                                </select>
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="mb-3">
                                <label for="addAddress" class="form-label">Địa chỉ</label>
                                <input type="text" class="form-control" id="addAddress" placeholder="Nhập địa chỉ">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Common Appointment Details Section -->
            <div class="mt-4">
                <h6>Thông tin cuộc hẹn</h6>
                <div class="row">
                    <div class="col-md-6">
                        <div class="mb-3">
                            <label for="addClinic" class="form-label">Phòng khám <span class="text-danger">*</span></label>
                            <select class="form-select" id="addClinic">
                                <option value="">Chọn phòng khám</option>
                            </select>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="mb-3">
                            <label for="addDoctor" class="form-label">Bác sĩ <span class="text-danger">*</span></label>
                            <select class="form-select" id="addDoctor" disabled>
                                <option value="">Chọn bác sĩ</option>
                            </select>
                        </div>
                    </div>
                </div>
                
                <div class="row">
                    <div class="col-md-6">
                        <div class="mb-3">
                            <label for="addService" class="form-label">Dịch vụ <span class="text-danger">*</span></label>
                            <select class="form-select" id="addService" disabled>
                                <option value="">Chọn dịch vụ</option>
                            </select>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="mb-3">
                            <label for="addDate" class="form-label">Ngày khám <span class="text-danger">*</span></label>
                            <input type="date" class="form-control" id="addDate">
                        </div>
                    </div>
                </div>
                
                <div class="row">
                    <div class="col-md-6">
                        <div class="mb-3">
                            <label for="addShift" class="form-label">Ca khám <span class="text-danger">*</span></label>
                            <select class="form-select" id="addShift" disabled>
                                <option value="">Chọn ca khám</option>
                            </select>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="mb-3">
                            <label for="addNote" class="form-label">Ghi chú</label>
                            <textarea class="form-control" id="addNote" rows="2" placeholder="Nhập ghi chú (nếu có)"></textarea>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        // Initialize dropdowns and event listeners
        initializeDropdowns();
        initializeEventListeners();
    }

    // Initialize dropdowns
    function initializeDropdowns() {
        // Load clinics
        loadClinics();
        
        // Initialize date picker
        initializeDatePicker();
    }

    // Load clinics
    function loadClinics() {
        fetch('https://localhost:7097/api/appointment/clinic/list')
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(result => {
                console.log('Clinics response:', result);
                
                if (result.success && result.data && Array.isArray(result.data)) {
                    const clinicSelect = document.getElementById('addClinic');
                    clinicSelect.innerHTML = '<option value="">Chọn phòng khám</option>';
                    result.data.forEach(clinic => {
                        clinicSelect.innerHTML += `<option value="${clinic.id}">${clinic.name}</option>`;
                    });
                } else {
                    console.error('Failed to load clinics:', result);
                    showNotification('Lỗi khi tải danh sách phòng khám: ' + (result.message || 'Unknown error'), 'error');
                }
            })
            .catch(error => {
                console.error('Error loading clinics:', error);
                showNotification('Lỗi khi tải danh sách phòng khám: ' + error.message, 'error');
            });
    }

    // Load doctors based on selected clinic
    function loadDoctors(clinicId) {
        if (!clinicId) {
            const doctorSelect = document.getElementById('addDoctor');
            doctorSelect.innerHTML = '<option value="">Chọn bác sĩ</option>';
            doctorSelect.disabled = true;
            return;
        }

        fetch(`https://localhost:7097/api/appointment/doctor/list?clinicId=${clinicId}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(result => {
                console.log('Doctors response:', result);
                
                if (result.success && result.data && Array.isArray(result.data)) {
                    const doctorSelect = document.getElementById('addDoctor');
                    doctorSelect.innerHTML = '<option value="">Chọn bác sĩ</option>';
                    result.data.forEach(doctor => {
                        doctorSelect.innerHTML += `<option value="${doctor.id}">${doctor.name}</option>`;
                    });
                    // Enable the doctor dropdown
                    doctorSelect.disabled = false;
                } else {
                    console.error('Failed to load doctors:', result);
                    showNotification('Lỗi khi tải danh sách bác sĩ: ' + (result.message || 'Unknown error'), 'error');
                    const doctorSelect = document.getElementById('addDoctor');
                    doctorSelect.disabled = true;
                }
            })
            .catch(error => {
                console.error('Error loading doctors:', error);
                showNotification('Lỗi khi tải danh sách bác sĩ: ' + error.message, 'error');
                const doctorSelect = document.getElementById('addDoctor');
                doctorSelect.disabled = true;
            });
    }

    // Load services based on selected doctor
    function loadServices(doctorId) {
        if (!doctorId) {
            const serviceSelect = document.getElementById('addService');
            serviceSelect.innerHTML = '<option value="">Chọn dịch vụ</option>';
            serviceSelect.disabled = true;
            return;
        }

        fetch(`https://localhost:7097/api/appointment/service/list?doctorId=${doctorId}`)
            .then(response => response.json())
            .then(result => {
                console.log('Services response:', result);
                
                if (result.success && result.data && Array.isArray(result.data)) {
                    const serviceSelect = document.getElementById('addService');
                    serviceSelect.innerHTML = '<option value="">Chọn dịch vụ</option>';
                    result.data.forEach(service => {
                        serviceSelect.innerHTML += `<option value="${service.id}">${service.name}</option>`;
                    });
                    // Enable the service dropdown
                    serviceSelect.disabled = false;
                } else {
                    console.error('Failed to load services:', result);
                    showNotification('Lỗi khi tải danh sách dịch vụ', 'error');
                    const serviceSelect = document.getElementById('addService');
                    serviceSelect.disabled = true;
                }
            })
            .catch(error => {
                console.error('Error loading services:', error);
                showNotification('Lỗi khi tải danh sách dịch vụ', 'error');
                const serviceSelect = document.getElementById('addService');
                serviceSelect.disabled = true;
            });
    }

            // Load available shifts for selected doctor and date
        function loadShifts(doctorId, date) {
            if (!doctorId || !date) {
                const shiftSelect = document.getElementById('addShift');
                shiftSelect.innerHTML = '<option value="">Chọn ca khám</option>';
                shiftSelect.disabled = true;
                return;
            }

            // Sử dụng endpoint đúng cho shifts
            fetch(`https://localhost:7097/api/appointment/available-time-slots?doctorId=${doctorId}&date=${date}`)
            .then(response => response.json())
            .then(result => {
                console.log('Shifts response:', result);
                
                const shiftSelect = document.getElementById('addShift');
                shiftSelect.innerHTML = '<option value="">Chọn ca khám</option>';
                
                if (result.success && result.data && Array.isArray(result.data)) {
                    if (result.data.length === 0) {
                        shiftSelect.innerHTML += '<option value="" disabled>Không có ca khám nào trong ngày này</option>';
                    } else {
                        // Xử lý format response mới từ available-time-slots
                        result.data.forEach(timeSlot => {
                            const optionText = `${timeSlot.shift} (${timeSlot.startTime} - ${timeSlot.endTime})`;
                            const optionValue = timeSlot.shift; // Sử dụng shift name thay vì id
                            shiftSelect.innerHTML += `<option value="${optionValue}">${optionText}</option>`;
                        });
                    }
                    // Enable the shift dropdown
                    shiftSelect.disabled = false;
                } else if (result.success && result.data && !Array.isArray(result.data)) {
                    // Nếu data không phải array, thử xử lý như object
                    console.log('Data is not array, treating as object:', result.data);
                    const shifts = ['morning', 'afternoon'];
                    shifts.forEach(shift => {
                        shiftSelect.innerHTML += `<option value="${shift}">${shift === 'morning' ? 'Sáng' : 'Chiều'}</option>`;
                    });
                    shiftSelect.disabled = false;
                } else {
                    console.error('Failed to load shifts:', result);
                    showNotification('Lỗi khi tải danh sách ca khám', 'error');
                    shiftSelect.disabled = true;
                }
            })
            .catch(error => {
                console.error('Error loading shifts:', error);
                showNotification('Lỗi khi tải danh sách ca khám', 'error');
                const shiftSelect = document.getElementById('addShift');
                shiftSelect.disabled = true;
            });
    }

    // Initialize date picker
    function initializeDatePicker() {
        const dateInput = document.getElementById('addDate');
        if (dateInput) {
            // Set minimum date to today
            const today = new Date().toISOString().split('T')[0];
            dateInput.min = today;
        }
    }

    // Initialize event listeners
    function initializeEventListeners() {
        // Clinic change event
        document.getElementById('addClinic').addEventListener('change', function() {
            const clinicId = this.value;
            if (clinicId) {
                loadDoctors(clinicId);
            } else {
                document.getElementById('addDoctor').innerHTML = '<option value="">Chọn bác sĩ</option>';
                document.getElementById('addDoctor').disabled = true;
                document.getElementById('addService').innerHTML = '<option value="">Chọn dịch vụ</option>';
                document.getElementById('addService').disabled = true;
                document.getElementById('addShift').innerHTML = '<option value="">Chọn ca khám</option>';
                document.getElementById('addShift').disabled = true;
            }
        });

        // Doctor change event
        document.getElementById('addDoctor').addEventListener('change', function() {
            const doctorId = this.value;
            if (doctorId) {
                loadServices(doctorId);
            } else {
                document.getElementById('addService').innerHTML = '<option value="">Chọn dịch vụ</option>';
                document.getElementById('addService').disabled = true;
                document.getElementById('addShift').innerHTML = '<option value="">Chọn ca khám</option>';
                document.getElementById('addShift').disabled = true;
            }
        });

        // Service change event
        document.getElementById('addService').addEventListener('change', function() {
            const serviceId = this.value;
            if (serviceId) {
                // Service selected, enable shift dropdown when date is selected
                const dateInput = document.getElementById('addDate');
                if (dateInput.value) {
                    const doctorId = document.getElementById('addDoctor').value;
                    loadShifts(doctorId, dateInput.value);
                }
            } else {
                document.getElementById('addShift').innerHTML = '<option value="">Chọn ca khám</option>';
                document.getElementById('addShift').disabled = true;
            }
        });

        // Date change event
        document.getElementById('addDate').addEventListener('change', function() {
            const date = this.value;
            const doctorId = document.getElementById('addDoctor').value;
            const serviceId = document.getElementById('addService').value;
            
            if (date && doctorId && serviceId) {
                loadShifts(doctorId, date);
            } else {
                document.getElementById('addShift').innerHTML = '<option value="">Chọn ca khám</option>';
                document.getElementById('addShift').disabled = true;
            }
        });

        // Add event listener for the Save button in footer
        const saveButton = document.querySelector('#offcanvasAppointmentAdd .offcanvas-footer .btn-primary');
        if (saveButton) {
            saveButton.addEventListener('click', function(e) {
                e.preventDefault();
                submitAppointment();
            });
        }
    }

    // Search patient function
    window.searchPatient = function() {
        const searchInput = document.getElementById('searchPatientInput');
        const searchTerm = searchInput.value.trim();
        
        if (!searchTerm) {
            showNotification('Vui lòng nhập số điện thoại hoặc CCCD để tìm kiếm', 'error');
            return;
        }

        // Chỉ disable button để tránh click nhiều lần - KHÔNG thay đổi text
        const searchBtn = document.getElementById('searchPatientBtn');
        searchBtn.disabled = true;

        // Hide previous results
        document.getElementById('patientSearchResults').style.display = 'none';
        document.getElementById('noPatientFound').style.display = 'none';

        // Determine if search term is phone or CCCD
        const isPhone = /^\d{10,11}$/.test(searchTerm); // Vietnamese phone number format
        const isCCCD = /^\d{12}$/.test(searchTerm); // CCCD format
        
        console.log('Search Debug:', {
            searchTerm,
            isPhone,
            isCCCD,
            searchTermLength: searchTerm.length
        });
        
        let apiUrl;
        if (isPhone) {
            apiUrl = `https://localhost:7097/api/patient/search?phone=${encodeURIComponent(searchTerm)}`;
        } else if (isCCCD) {
            apiUrl = `https://localhost:7097/api/patient/search?cccd=${encodeURIComponent(searchTerm)}`;
        } else {
            // Try both phone and CCCD
            apiUrl = `https://localhost:7097/api/patient/search?phone=${encodeURIComponent(searchTerm)}&cccd=${encodeURIComponent(searchTerm)}`;
        }

        console.log('API URL:', apiUrl);

        // Call API to search patient
        fetch(apiUrl)
            .then(response => {
                console.log('Response status:', response.status);
                console.log('Response headers:', response.headers);
                console.log('Response type:', response.type);
                
                // Kiểm tra content type
                const contentType = response.headers.get('content-type');
                console.log('Content-Type:', contentType);
                
                if (!response.ok) {
                    if (response.status === 404) {
                        return { success: false, message: "Không tìm thấy bệnh nhân" };
                    }
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                
                // Kiểm tra nếu response là HTML thay vì JSON
                if (contentType && contentType.includes('text/html')) {
                    throw new Error('Server returned HTML instead of JSON. Check if API endpoint is correct.');
                }
                
                return response.json();
            })
            .then(data => {
                console.log('Response data:', data);
                console.log('Data type:', typeof data);
                console.log('Data keys:', Object.keys(data));
                
                // Kiểm tra format response mới: { success: true, data: {...} }
                if (data && data.success && data.data && data.data.id) {
                    const patientData = {
                        id: data.data.id,
                        name: data.data.name,
                        phone: data.data.phone,
                        cccd: data.data.cccd,
                        email: data.data.email,
                        address: data.data.address,
                        birthDate: data.data.birthDate,
                        gender: data.data.gender
                    };
                    
                    currentPatientId = patientData.id;
                    currentPatientData = patientData;
                    displayPatientInfo(patientData);
                    document.getElementById('patientSearchResults').style.display = 'block';
                    document.getElementById('noPatientFound').style.display = 'none';
                    showNotification('Tìm thấy bệnh nhân!', 'success');
                } else if (data && data.success && (!data.data || Object.keys(data.data).length === 0)) {
                    // API trả về success nhưng không có data (không tìm thấy)
                    console.log('No patient found in response');
                    document.getElementById('noPatientFound').style.display = 'block';
                    document.getElementById('patientSearchResults').style.display = 'none';
                    showNotification('Không tìm thấy bệnh nhân với thông tin này', 'warning');
                } else if (data && (data.id || data.Id)) {
                    // Fallback cho format cũ: { id: 4, name: "...", ... }
                    const patientData = {
                        id: data.id || data.Id,
                        name: data.name || data.Name,
                        phone: data.phone || data.Phone,
                        cccd: data.cccd || data.CCCD,
                        email: data.email || data.Email,
                        address: data.address || data.Address,
                        birthDate: data.birthDate || data.BirthDate,
                        gender: data.gender || data.Gender
                    };
                    
                    currentPatientId = patientData.id;
                    currentPatientData = patientData;
                    displayPatientInfo(patientData);
                    document.getElementById('patientSearchResults').style.display = 'block';
                    document.getElementById('noPatientFound').style.display = 'none';
                    showNotification('Tìm thấy bệnh nhân! (Format cũ)', 'success');
                } else {
                    console.log('No patient found in response');
                    document.getElementById('noPatientFound').style.display = 'block';
                    document.getElementById('patientSearchResults').style.display = 'none';
                    showNotification('Không tìm thấy bệnh nhân với thông tin này', 'warning');
                }
            })
            .catch(error => {
                console.error('Error searching patient:', error);
                
                // Hiển thị thông báo lỗi chi tiết hơn
                let errorMessage = 'Lỗi khi tìm kiếm bệnh nhân';
                if (error.message.includes('HTML instead of JSON')) {
                    errorMessage = 'Lỗi kết nối API. Vui lòng kiểm tra server và endpoint.';
                } else if (error.message.includes('fetch')) {
                    errorMessage = 'Không thể kết nối đến server. Vui lòng kiểm tra kết nối mạng.';
                }
                
                showNotification(errorMessage, 'error');
            })
            .finally(() => {
                // Chỉ enable lại button - KHÔNG thay đổi text
                searchBtn.disabled = false;
            });
    };

    // Display patient information
    function displayPatientInfo(patient) {
        const patientInfoDiv = document.getElementById('patientInfo');
        patientInfoDiv.innerHTML = `
            <div class="row">
                <div class="col-md-6">
                    <p><strong>Họ và tên:</strong> ${patient.name}</p>
                    <p><strong>Số điện thoại:</strong> ${patient.phone}</p>
                    <p><strong>CCCD:</strong> ${patient.cccd}</p>
                    <p><strong>Email:</strong> ${patient.email || 'Chưa có'}</p>
                </div>
                <div class="col-md-6">
                    <p><strong>Ngày sinh:</strong> ${patient.birthDate || 'Chưa có'}</p>
                    <p><strong>Giới tính:</strong> ${patient.gender === '0' ? 'Nam' : patient.gender === '1' ? 'Nữ' : 'Khác'}</p>
                    <p><strong>Địa chỉ:</strong> ${patient.address || 'Chưa có'}</p>
                </div>
            </div>
        `;
    }

    // Create new patient and user
    window.createNewPatientAndUser = function() {
        // This function is no longer needed as we use submitAppointment for both cases
        console.log('createNewPatientAndUser is deprecated. Use submitAppointment instead.');
    };

    // Submit appointment for both existing and new patients
    window.submitAppointment = async function() {
        // Check which tab is active
        const existingPatientTab = document.getElementById('existing-patient-tab');
        const isExistingPatientTab = existingPatientTab.classList.contains('active');
        
        if (isExistingPatientTab) {
            // Existing patient tab - check if patient is selected
            if (!currentPatientId) {
                showNotification('Vui lòng tìm kiếm và chọn bệnh nhân trước', 'error');
                return;
            }
            await submitExistingPatientAppointment();
        } else {
            // New patient tab - validate and create patient with appointment
            await submitNewPatientWithAppointment();
        }
    };

    // Submit appointment for existing patient
    async function submitExistingPatientAppointment() {
        // Validate appointment fields
        const appointmentFields = {
            'addClinic': 'Phòng khám',
            'addDoctor': 'Bác sĩ',
            'addService': 'Dịch vụ',
            'addDate': 'Ngày khám',
            'addShift': 'Ca khám'
        };

        for (const [fieldId, fieldName] of Object.entries(appointmentFields)) {
            const field = document.getElementById(fieldId);
            if (!field.value) {
                showNotification(`Vui lòng chọn ${fieldName}`, 'error');
                field.focus();
                return;
            }
        }

        // Chỉ disable button để tránh click nhiều lần - KHÔNG thay đổi text
        const submitBtn = document.querySelector('#offcanvasAppointmentAdd .offcanvas-footer .btn-primary');
        submitBtn.disabled = true;

        // Prepare appointment data
        const appointmentData = {
            clinicId: parseInt(document.getElementById('addClinic').value),
            doctorId: parseInt(document.getElementById('addDoctor').value),
            serviceId: parseInt(document.getElementById('addService').value),
            appointmentDate: document.getElementById('addDate').value,
            shift: document.getElementById('addShift').value,
            note: document.getElementById('addNote').value.trim() || '',
            patientInfo: {
                name: currentPatientData.name,
                phone: currentPatientData.phone,
                gender: currentPatientData.gender,
                dob: currentPatientData.birthDate,
                cccd: currentPatientData.cccd,
                address: currentPatientData.address || '',
                insuranceNumber: '',
                allergies: '',
                bloodType: ''
            }
        };

        console.log('Sending appointment data:', appointmentData);

        // Gọi API tạo cuộc hẹn với timeout
        const controller = new AbortController();
        const timeoutId = setTimeout(() => {
            controller.abort();
            showNotification('Yêu cầu bị timeout sau 10 giây. Vui lòng thử lại!', 'error');
        }, 10000); // 10 seconds timeout
        
        fetch(`https://localhost:7097/api/appointment/create-test?patientId=${currentPatientId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(appointmentData),
            signal: controller.signal
        })
        .then(response => {
            clearTimeout(timeoutId);
            console.log('Response status:', response.status);
            console.log('Response headers:', response.headers);
            
            if (!response.ok) {
                return response.text().then(text => {
                    console.log('Error response body:', text);
                    throw new Error(`HTTP error! status: ${response.status}, body: ${text}`);
                });
            }
            return response.json();
        })
        .then(data => {
            console.log('Success response:', data);
            if (data.success || data.id) {
                showNotification('Tạo cuộc hẹn thành công!', 'success');
                handleSuccess();
            } else {
                showNotification('Lỗi khi tạo cuộc hẹn: ' + (data.message || 'Unknown error'), 'error');
            }
        })
        .catch(error => {
            clearTimeout(timeoutId);
            console.error('Error creating appointment:', error);
            
            let errorMessage = 'Lỗi khi tạo cuộc hẹn';
            if (error.name === 'AbortError') {
                errorMessage = 'Yêu cầu bị timeout. Vui lòng thử lại!';
            } else if (error.message.includes('fetch')) {
                errorMessage = 'Không thể kết nối đến server. Vui lòng kiểm tra kết nối!';
            } else {
                errorMessage = error.message;
            }
            
            showNotification(errorMessage, 'error');
        })
        .finally(() => {
            clearTimeout(timeoutId);
            // Chỉ enable lại button - KHÔNG thay đổi text
            submitBtn.disabled = false;
        });
    }

    // Validate data before sending to API
    function validatePatientData(data) {
        const errors = [];
        
        // Validate CCCD format (12 digits)
        if (!/^\d{12}$/.test(data.cccd)) {
            errors.push('CCCD phải có đúng 12 chữ số');
        }
        
        // Validate phone format (Vietnamese phone number - chỉ số 0)
        if (!/^0(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$/.test(data.phone)) {
            errors.push('Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số');
        }
        
        // Validate email format
        if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(data.email)) {
            errors.push('Email không đúng định dạng');
        }
        
        // Validate emergency contact (chỉ số 0)
        if (!/^0(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$/.test(data.emergencyContact)) {
            errors.push('Liên hệ khẩn cấp phải bắt đầu bằng 0 và có 10 chữ số');
        }
        
        // Validate appointment date (not in past)
        const appointmentDate = new Date(data.appointmentDate);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        if (appointmentDate < today) {
            errors.push('Ngày khám không thể là ngày trong quá khứ');
        }
        
        return errors;
    }

    // Submit new patient with appointment
    async function submitNewPatientWithAppointment() {
        // Validate required patient fields
        const requiredPatientFields = {
            'addCCCD': 'CCCD',
            'addPhoneNumber': 'Số điện thoại',
            'addEmergencyContact': 'Liên hệ khẩn cấp',
            'addName': 'Họ và tên',
            'addEmail': 'Email'
        };

        for (const [fieldId, fieldName] of Object.entries(requiredPatientFields)) {
            const field = document.getElementById(fieldId);
            if (!field.value.trim()) {
                showNotification(`Vui lòng nhập ${fieldName}`, 'error');
                field.focus();
                return;
            }
        }

        // Validate appointment fields
        const appointmentFields = {
            'addClinic': 'Phòng khám',
            'addDoctor': 'Bác sĩ',
            'addService': 'Dịch vụ',
            'addDate': 'Ngày khám',
            'addShift': 'Ca khám'
        };

        for (const [fieldId, fieldName] of Object.entries(appointmentFields)) {
            const field = document.getElementById(fieldId);
            if (!field.value) {
                showNotification(`Vui lòng chọn ${fieldName}`, 'error');
                field.focus();
                return;
            }
        }

        // Prepare combined data for patient and appointment
        const combinedData = {
            // Patient Information
            fullName: document.getElementById('addName').value.trim(),
            phone: document.getElementById('addPhoneNumber').value.trim(),
            emergencyContact: document.getElementById('addEmergencyContact').value.trim(),
            cccd: document.getElementById('addCCCD').value.trim(),
            email: document.getElementById('addEmail').value.trim(),
            dob: document.getElementById('addBirthDate').value || new Date().toISOString(),
            gender: parseInt(document.getElementById('addGender').value) || 0,
            address: document.getElementById('addAddress').value.trim() || '',
            
            // Appointment Information
            clinicId: parseInt(document.getElementById('addClinic').value),
            doctorId: parseInt(document.getElementById('addDoctor').value),
            serviceId: parseInt(document.getElementById('addService').value),
            appointmentDate: document.getElementById('addDate').value,
            shift: document.getElementById('addShift').value,
            note: document.getElementById('addNote').value.trim() || ''
        };

        // Validate data before sending
        const validationErrors = validatePatientData(combinedData);
        if (validationErrors.length > 0) {
            showNotification('Lỗi validation: ' + validationErrors.join(', '), 'error');
            return;
        }

        console.log('Sending combined data:', combinedData);
        
        // Chỉ disable button để tránh click nhiều lần - KHÔNG thay đổi text
        const submitBtn = document.querySelector('#offcanvasAppointmentAdd .offcanvas-footer .btn-primary');
        submitBtn.disabled = true;
        
        // Call API to create patient and appointment together with timeout
        const controller = new AbortController();
        const timeoutId = setTimeout(() => {
            controller.abort();
            showNotification('Yêu cầu bị timeout sau 10 giây. Vui lòng thử lại!', 'error');
        }, 10000); // 10 seconds timeout
        
        fetch('https://localhost:7097/api/patient/create-with-appointment', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(combinedData),
            signal: controller.signal
        })
        .then(response => {
            clearTimeout(timeoutId);
            console.log('Response status:', response.status);
            console.log('Response headers:', response.headers);
            
            if (!response.ok) {
                return response.text().then(text => {
                    console.log('Error response body:', text);
                    throw new Error(`HTTP error! status: ${response.status}, body: ${text}`);
                });
            }
            return response.json();
        })
        .then(data => {
            console.log('Success response:', data);
            if (data.success && data.data) {
                showNotification('Tạo bệnh nhân và cuộc hẹn thành công!', 'success');
                handleSuccess();
            } else {
                showNotification('Lỗi khi tạo bệnh nhân và cuộc hẹn: ' + (data.message || 'Unknown error'), 'error');
            }
        })
        .catch(error => {
            clearTimeout(timeoutId);
            console.error('Error creating patient and appointment:', error);
            
            let errorMessage = 'Lỗi khi tạo bệnh nhân và cuộc hẹn';
            if (error.name === 'AbortError') {
                errorMessage = 'Yêu cầu bị timeout. Vui lòng thử lại!';
            } else if (error.message.includes('fetch')) {
                errorMessage = 'Không thể kết nối đến server. Vui lòng kiểm tra kết nối!';
            } else {
                errorMessage = error.message;
            }
            
            showNotification(errorMessage, 'error');
        })
        .finally(() => {
            clearTimeout(timeoutId);
            // Chỉ enable lại button - KHÔNG thay đổi text
            submitBtn.disabled = false;
        });
    }

    // Handle success for both cases
    function handleSuccess() {
        // Reset form
        resetForm();
        
        // Close offcanvas
        const offcanvas = bootstrap.Offcanvas.getInstance(document.getElementById('offcanvasAppointmentAdd'));
        if (offcanvas) offcanvas.hide();
        
        // Reload appointments list if function exists
        if (window.loadAppointments) {
            window.loadAppointments();
        }
        
        // Refresh appointment count if function exists
        if (window.refreshAppointmentCount) {
            window.refreshAppointmentCount();
        }
    }

    // Reset form
    function resetForm() {
        // Clear all form fields
        const fieldsToClear = [
            'searchPatientInput', 'addCCCD', 'addPhoneNumber', 'addEmergencyContact',
            'addName', 'addEmail', 'addBirthDate', 'addAddress', 'addNote'
        ];
        
        fieldsToClear.forEach(fieldId => {
            const field = document.getElementById(fieldId);
            if (field) {
                field.value = '';
            }
        });
        
        // Reset dropdowns
        const dropdownsToReset = ['addClinic', 'addDoctor', 'addService', 'addShift'];
        dropdownsToReset.forEach(dropdownId => {
            const dropdown = document.getElementById(dropdownId);
            if (dropdown) {
                dropdown.innerHTML = `<option value="">Chọn ${dropdownId.replace('add', '').toLowerCase()}</option>`;
                dropdown.disabled = dropdownId !== 'addClinic';
            }
        });
        
        // Reset gender dropdown
        const genderDropdown = document.getElementById('addGender');
        if (genderDropdown) {
            genderDropdown.value = '0';
        }
        
        // Hide search results
        document.getElementById('patientSearchResults').style.display = 'none';
        document.getElementById('noPatientFound').style.display = 'none';
        
        // Reset global variables
        currentPatientId = null;
        currentPatientData = null;
        
        // Switch back to existing patient tab
        const existingPatientTab = document.getElementById('existing-patient-tab');
        const newPatientTab = document.getElementById('new-patient-tab');
        const existingPatientContent = document.getElementById('existing-patient');
        const newPatientContent = document.getElementById('new-patient');
        
        existingPatientTab.classList.add('active');
        newPatientTab.classList.remove('active');
        existingPatientContent.classList.add('show', 'active');
        newPatientContent.classList.remove('show', 'active');
        
        // Focus on search input
        document.getElementById('searchPatientInput').focus();
    }

    // Retry API call with exponential backoff
    async function retryAPICall(fetchFunction, maxRetries = 3) {
        for (let i = 0; i < maxRetries; i++) {
            try {
                return await fetchFunction();
            } catch (error) {
                if (i === maxRetries - 1) {
                    throw error;
                }
                // Wait before retrying (exponential backoff)
                await new Promise(resolve => setTimeout(resolve, Math.pow(2, i) * 1000));
            }
        }
    }

    // Show notification
    function showNotification(message, type = 'info') {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: type === 'success' ? 'Thành công!' : type === 'error' ? 'Lỗi!' : 'Thông báo',
                text: message,
                icon: type,
                confirmButtonText: 'OK',
                confirmButtonColor: type === 'success' ? '#28a745' : type === 'error' ? '#dc3545' : '#007bff'
            });
        } else {
            alert(message);
        }
    }

    // Check API connection
    function checkAPIConnection() {
        fetch('https://localhost:7097/api/appointment/test')
            .then(response => {
                if (!response.ok) {
                    throw new Error(`API không khả dụng (Status: ${response.status})`);
                }
                return response.json();
            })
            .then(data => {
                console.log('API connection successful:', data);
            })
            .catch(error => {
                console.error('API connection failed:', error);
                showNotification('Không thể kết nối đến API. Vui lòng kiểm tra server.', 'error');
            });
    }

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        // Check API connection on page load
        checkAPIConnection();
        
        // Initialize tab system when offcanvas is shown
        const offcanvas = document.getElementById('offcanvasAppointmentAdd');
        if (offcanvas) {
            offcanvas.addEventListener('shown.bs.offcanvas', function() {
                initializeTabSystem();
                
                // Focus on search input
                setTimeout(() => {
                    const searchInput = document.getElementById('searchPatientInput');
                    if (searchInput) {
                        searchInput.focus();
                    }
                }, 100);
            });
        }
    });

})();