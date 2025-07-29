// Appointment Management JavaScript
// Tìm kiếm và quản lý bệnh nhân cho cuộc hẹn

// Biến global để lưu thông tin bệnh nhân hiện tại
window.currentPatientId = null;
window.currentPatientData = null;

// Tìm kiếm bệnh nhân theo số điện thoại hoặc CCCD
window.searchPatient = function() {
    const searchInput = document.getElementById('searchPatientInput');
    const searchValue = searchInput ? searchInput.value.trim() : '';
    
    if (!searchValue) {
        showNotification('Vui lòng nhập số điện thoại hoặc CCCD để tìm kiếm', 'error');
        return;
    }

    // Hiển thị loading
    const searchButton = document.getElementById('searchPatientBtn');
    const originalText = searchButton ? searchButton.innerHTML : 'Tìm kiếm';
    if (searchButton) {
        searchButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Tìm kiếm...';
        searchButton.disabled = true;
    }

    // Xác định loại tìm kiếm (phone hoặc CCCD)
    const isPhone = /^0\d{9}$/.test(searchValue);
    const isCCCD = /^[0-9]{12}$/.test(searchValue);
    
    if (!isPhone && !isCCCD) {
        showNotification('Vui lòng nhập số điện thoại (10 chữ số) hoặc CCCD (12 chữ số) hợp lệ', 'error');
        if (searchButton) {
            searchButton.innerHTML = originalText;
            searchButton.disabled = false;
        }
        return;
    }

    // Tạo URL API
    const apiUrl = `https://localhost:7097/api/patient/search?${isPhone ? 'phone' : 'cccd'}=${encodeURIComponent(searchValue)}`;
    
    console.log('Searching patient with URL:', apiUrl);

    // Gọi API tìm kiếm bệnh nhân
    fetch(apiUrl)
        .then(response => {
            console.log('Response status:', response.status);
            console.log('Response headers:', response.headers);
            console.log('Response type:', response.type);
            
            const contentType = response.headers.get('Content-Type');
            console.log('Content-Type:', contentType);
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            
            if (contentType && contentType.includes('text/html')) {
                throw new Error('Server returned HTML instead of JSON. Please check API endpoint.');
            }
            
            return response.json();
        })
        .then(data => {
            console.log('Search response data:', data);
            
            // Kiểm tra response format mới: { success: true, data: {...}, message: "..." }
            if (data && data.success && data.data && data.data.id) {
                // Tìm thấy bệnh nhân
                const patient = {
                    id: data.data.id,
                    name: data.data.name,
                    cccd: data.data.cccd,
                    phone: data.data.phone,
                    email: data.data.email,
                    birthDate: data.data.birthDate,
                    gender: data.data.gender,
                    emergencyContact: data.data.emergencyContact || '',
                    address: data.data.address
                };
                
                window.currentPatientId = patient.id;
                window.currentPatientData = patient;
                fillPatientInfo(patient);
                showPatientInfoCard();
                showNotification('Đã tìm thấy thông tin bệnh nhân', 'success');
            } else if (data && data.success && (!data.data || Object.keys(data.data).length === 0)) {
                // API trả về success nhưng không có data (không tìm thấy)
                showPatientInfoCard();
                resetPatientForm();
                showNotification('Không tìm thấy bệnh nhân. Vui lòng nhập thông tin để tạo mới.', 'info');
            } else {
                // Fallback cho format cũ hoặc lỗi
                if (data && data.id) {
                    // Format cũ: { id: 4, name: "...", ... }
                    const patient = {
                        id: data.id,
                        name: data.name,
                        cccd: data.cccd,
                        phone: data.phone,
                        email: data.email,
                        birthDate: data.birthDate,
                        gender: data.gender,
                        emergencyContact: data.emergencyContact || '',
                        address: data.address
                    };
                    
                    window.currentPatientId = patient.id;
                    window.currentPatientData = patient;
                    fillPatientInfo(patient);
                    showPatientInfoCard();
                    showNotification('Đã tìm thấy thông tin bệnh nhân', 'success');
                } else {
                    // Không tìm thấy, hiển thị form tạo mới
                    showPatientInfoCard();
                    resetPatientForm();
                    showNotification('Không tìm thấy bệnh nhân. Vui lòng nhập thông tin để tạo mới.', 'info');
                }
            }
        })
        .catch(err => {
            console.error('Error searching patient:', err);
            showNotification('Lỗi khi tìm kiếm bệnh nhân: ' + err.message, 'error');
            showPatientInfoCard();
            resetPatientForm();
        })
        .finally(() => {
            // Reset button state
            if (searchButton) {
                searchButton.innerHTML = originalText;
                searchButton.disabled = false;
            }
        });
};

// Điền thông tin bệnh nhân vào form
function fillPatientInfo(patient) {
    const nameInput = document.getElementById('patientName');
    const cccdInput = document.getElementById('patientCCCD');
    const phoneInput = document.getElementById('patientPhone');
    const emailInput = document.getElementById('patientEmail');
    const birthDateInput = document.getElementById('patientBirthDate');
    const genderInput = document.getElementById('patientGender');
    const emergencyContactInput = document.getElementById('patientEmergencyContact');
    const addressInput = document.getElementById('patientAddress');

    if (nameInput) nameInput.value = patient.name || '';
    if (cccdInput) cccdInput.value = patient.cccd || '';
    if (phoneInput) phoneInput.value = patient.phone || '';
    if (emailInput) emailInput.value = patient.email || '';
    if (birthDateInput) birthDateInput.value = patient.birthDate || '';
    if (genderInput) genderInput.value = patient.gender || '';
    if (emergencyContactInput) emergencyContactInput.value = patient.emergencyContact || '';
    if (addressInput) addressInput.value = patient.address || '';

    // Disable các field đã có thông tin
    if (nameInput) nameInput.disabled = true;
    if (cccdInput) cccdInput.disabled = true;
    if (phoneInput) phoneInput.disabled = true;
    if (emailInput) emailInput.disabled = true;
    if (birthDateInput) birthDateInput.disabled = true;
    if (genderInput) genderInput.disabled = true;
    if (emergencyContactInput) emergencyContactInput.disabled = true;
    if (addressInput) addressInput.disabled = true;
}

// Reset form về trạng thái ban đầu
function resetPatientForm() {
    const nameInput = document.getElementById('patientName');
    const cccdInput = document.getElementById('patientCCCD');
    const phoneInput = document.getElementById('patientPhone');
    const emailInput = document.getElementById('patientEmail');
    const birthDateInput = document.getElementById('patientBirthDate');
    const genderInput = document.getElementById('patientGender');
    const emergencyContactInput = document.getElementById('patientEmergencyContact');
    const addressInput = document.getElementById('patientAddress');

    if (nameInput) {
        nameInput.value = '';
        nameInput.disabled = false;
    }
    if (cccdInput) {
        cccdInput.value = '';
        cccdInput.disabled = false;
    }
    if (phoneInput) {
        phoneInput.value = '';
        phoneInput.disabled = false;
    }
    if (emailInput) {
        emailInput.value = '';
        emailInput.disabled = false;
    }
    if (birthDateInput) {
        birthDateInput.value = '';
        birthDateInput.disabled = false;
    }
    if (genderInput) {
        genderInput.value = '';
        genderInput.disabled = false;
    }
    if (emergencyContactInput) {
        emergencyContactInput.value = '';
        emergencyContactInput.disabled = false;
    }
    if (addressInput) {
        addressInput.value = '';
        addressInput.disabled = false;
    }
}

// Hiển thị card thông tin bệnh nhân
function showPatientInfoCard() {
    const patientInfoCard = document.getElementById('patientInfoCard');
    if (patientInfoCard) {
        patientInfoCard.style.display = 'block';
    }
}

// Hiển thị card thông tin cuộc hẹn
function showAppointmentInfoCard() {
    const appointmentInfoCard = document.getElementById('appointmentInfoCard');
    if (appointmentInfoCard) {
        appointmentInfoCard.style.display = 'block';
    }
}

// Tạo bệnh nhân mới
window.createNewPatient = function() {
    const nameInput = document.getElementById('patientName');
    const cccdInput = document.getElementById('patientCCCD');
    const phoneInput = document.getElementById('patientPhone');
    const emailInput = document.getElementById('patientEmail');
    const birthDateInput = document.getElementById('patientBirthDate');
    const genderInput = document.getElementById('patientGender');
    const emergencyContactInput = document.getElementById('patientEmergencyContact');
    const addressInput = document.getElementById('patientAddress');

    const patientData = {
        name: nameInput ? nameInput.value.trim() : '',
        cccd: cccdInput ? cccdInput.value.trim() : '',
        phone: phoneInput ? phoneInput.value.trim() : '',
        email: emailInput ? emailInput.value.trim() : '',
        birthDate: birthDateInput ? birthDateInput.value : '',
        gender: genderInput ? genderInput.value : '',
        emergencyContact: emergencyContactInput ? emergencyContactInput.value.trim() : '',
        address: addressInput ? addressInput.value.trim() : ''
    };

    // Validate required fields
    if (!patientData.name) {
        showNotification('Vui lòng nhập họ và tên', 'error');
        return;
    }
    if (!patientData.cccd) {
        showNotification('Vui lòng nhập số CCCD', 'error');
        return;
    }
    if (!patientData.phone) {
        showNotification('Vui lòng nhập số điện thoại', 'error');
        return;
    }
    if (!patientData.email) {
        showNotification('Vui lòng nhập email', 'error');
        return;
    }
    if (!patientData.birthDate) {
        showNotification('Vui lòng nhập ngày sinh', 'error');
        return;
    }
    if (!patientData.gender) {
        showNotification('Vui lòng chọn giới tính', 'error');
        return;
    }
    if (!patientData.emergencyContact) {
        showNotification('Vui lòng nhập số điện thoại liên hệ khẩn cấp', 'error');
        return;
    }
    if (!patientData.address) {
        showNotification('Vui lòng nhập địa chỉ', 'error');
        return;
    }

    // Validate formats
    const phoneRegex = /^0\d{9}$/;
    if (!phoneRegex.test(patientData.phone)) {
        showNotification('Vui lòng nhập số điện thoại hợp lệ (10 chữ số, bắt đầu bằng 0)', 'error');
        return;
    }
    if (!phoneRegex.test(patientData.emergencyContact)) {
        showNotification('Vui lòng nhập số điện thoại liên hệ khẩn cấp hợp lệ', 'error');
        return;
    }

    const cccdRegex = /^[0-9]{12}$/;
    if (!cccdRegex.test(patientData.cccd)) {
        showNotification('Vui lòng nhập CCCD hợp lệ (12 chữ số)', 'error');
        return;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(patientData.email)) {
        showNotification('Vui lòng nhập email hợp lệ', 'error');
        return;
    }

    // Validate birth date (not in the future)
    const birthDate = new Date(patientData.birthDate);
    const today = new Date();
    if (birthDate > today) {
        showNotification('Ngày sinh không thể là ngày trong tương lai', 'error');
        return;
    }

    // Hiển thị loading
    const createButton = document.getElementById('createPatientBtn');
    const originalText = createButton ? createButton.innerHTML : 'Tạo bệnh nhân mới';
    if (createButton) {
        createButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Đang tạo...';
        createButton.disabled = true;
    }

    // Tạo dữ liệu bệnh nhân (không có user)
    const patientRequestData = {
        fullName: patientData.name,
        gender: parseInt(patientData.gender) || 0,
        dob: new Date(patientData.birthDate).toISOString(),
        cccd: patientData.cccd,
        phone: patientData.phone,
        emergencyContact: patientData.emergencyContact,
        address: patientData.address,
        email: patientData.email
    };

    console.log('Sending patient data to API:', patientRequestData);

    // Gọi API tạo bệnh nhân (không tạo user)
    fetch(AppointmentAPI.patients.create, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(patientRequestData)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return response.json();
    })
    .then(result => {
        if (result && result.success) {
            // Hiển thị thông báo thành công
            showNotification('Đã tạo bệnh nhân mới thành công!', 'success');
            
            // Lưu thông tin bệnh nhân
            window.currentPatientId = result.data ? result.data.id : result.patientId;
            window.currentPatientData = {
                id: window.currentPatientId,
                name: patientData.name,
                cccd: patientData.cccd,
                phone: patientData.phone,
                email: patientData.email,
                birthDate: patientData.birthDate,
                gender: patientData.gender,
                emergencyContact: patientData.emergencyContact,
                address: patientData.address
            };
            
            // Disable các field sau khi tạo thành công
            if (nameInput) nameInput.disabled = true;
            if (cccdInput) cccdInput.disabled = true;
            if (phoneInput) phoneInput.disabled = true;
            if (emailInput) emailInput.disabled = true;
            if (birthDateInput) birthDateInput.disabled = true;
            if (genderInput) genderInput.disabled = true;
            if (emergencyContactInput) emergencyContactInput.disabled = true;
            if (addressInput) addressInput.disabled = true;
            
            // Hiển thị form cuộc hẹn
            showAppointmentInfoCard();
            
            // Reset button text
            if (createButton) {
                createButton.innerHTML = originalText;
                createButton.disabled = false;
            }
        } else {
            throw new Error('Không thể tạo bệnh nhân');
        }
    })
    .catch(error => {
        console.error('Error creating patient:', error);
        showNotification(`Lỗi tạo bệnh nhân: ${error.message}`, 'error');
        
        // Reset button text
        if (createButton) {
            createButton.innerHTML = originalText;
            createButton.disabled = false;
        }
    });
};

// Lưu cuộc hẹn
window.saveAppointment = function() {
    // TODO: Implement appointment saving logic
    showNotification('Chức năng lưu cuộc hẹn sẽ được implement sau', 'info');
};

// Hiển thị thông báo
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

// Initialize when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    // Add event listener for search input
    const searchInput = document.getElementById('searchPatientInput');
    if (searchInput) {
        // Auto search when input is entered (after 1 second delay)
        let searchTimeout;
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            const value = this.value.trim();
            
            if (value.length >= 10) {
                searchTimeout = setTimeout(() => {
                    window.searchPatient();
                }, 1000);
            }
        });
    }
}); 