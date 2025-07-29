// search-patient.js
// Xử lý tìm kiếm bệnh nhân theo số điện thoại và tự động điền thông tin
(function() {
    // Tìm kiếm bệnh nhân theo số điện thoại
    function searchPatientByPhone(phone) {
        if (!phone || phone.length < 10) {
            return Promise.reject(new Error('Số điện thoại không hợp lệ'));
        }

        return AppointmentAPI.utils.get(AppointmentAPI.patients.searchByPhone(phone))
            .then(data => {
                console.log('Patient search result:', data);
                return data;
            });
    }

    // Tạo bệnh nhân mới
    function createNewPatient(patientData) {
        return AppointmentAPI.utils.post(AppointmentAPI.patients.create, patientData)
            .then(data => {
                console.log('New patient created:', data);
                return data;
            });
    }

    // Tạo user mới với mật khẩu ngẫu nhiên
    function createNewUser(userData) {
        return AppointmentAPI.utils.post(AppointmentAPI.auth.register, userData)
            .then(data => {
                console.log('New user created:', data);
                return data;
            });
    }

    // Gửi email chứa mật khẩu
    function sendPasswordEmail(email, password) {
        return AppointmentAPI.utils.post(AppointmentAPI.auth.sendPasswordEmail, {
            email: email,
            password: password
        });
    }

    // Tạo mật khẩu ngẫu nhiên
    function generateRandomPassword() {
        const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*';
        let password = '';
        for (let i = 0; i < 12; i++) {
            password += chars.charAt(Math.floor(Math.random() * chars.length));
        }
        return password;
    }

    // Điền thông tin bệnh nhân vào form
    function fillPatientInfo(patient) {
        const nameInput = document.getElementById('addName');
        const phoneInput = document.getElementById('addPhone');
        const emailInput = document.getElementById('addEmail');
        const addressInput = document.getElementById('addAddress');
        const birthDateInput = document.getElementById('addBirthDate');
        const genderInput = document.getElementById('addGender');

        if (nameInput) nameInput.value = patient.name || '';
        if (phoneInput) phoneInput.value = patient.phone || '';
        if (emailInput) emailInput.value = patient.email || '';
        if (addressInput) addressInput.value = patient.address || '';
        if (birthDateInput) birthDateInput.value = patient.birthDate || '';
        if (genderInput) genderInput.value = patient.gender || '';

        // Disable các field đã có thông tin
        if (nameInput) nameInput.disabled = true;
        if (phoneInput) phoneInput.disabled = true;
        if (emailInput) emailInput.disabled = true;
        if (addressInput) addressInput.disabled = true;
        if (birthDateInput) birthDateInput.disabled = true;
        if (genderInput) genderInput.disabled = true;

        // Hiển thị thông báo
        showNotification('Đã tìm thấy thông tin bệnh nhân và tự động điền', 'success');
    }

    // Reset form về trạng thái ban đầu
    function resetPatientForm() {
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
    }

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

    // Xử lý tìm kiếm bệnh nhân
    window.searchPatient = function() {
        const phoneInput = document.getElementById('addPhone');
        const phone = phoneInput ? phoneInput.value.trim() : '';

        if (!phone) {
            showNotification('Vui lòng nhập số điện thoại', 'error');
            return;
        }

        // Validate phone number format (Vietnamese phone number)
        const phoneRegex = /^(0|\+84)(3[2-9]|5[689]|7[06-9]|8[1-689]|9[0-46-9])[0-9]{7}$/;
        if (!phoneRegex.test(phone)) {
            showNotification('Vui lòng nhập số điện thoại hợp lệ', 'error');
            return;
        }

        // Hiển thị loading
        const searchButton = document.getElementById('searchPatientBtn');
        const originalText = searchButton ? searchButton.innerHTML : 'Tìm kiếm';
        if (searchButton) {
            searchButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Đang tìm...';
            searchButton.disabled = true;
        }

        searchPatientByPhone(phone)
            .then(patient => {
                if (patient && patient.id) {
                    // Tìm thấy bệnh nhân
                    fillPatientInfo(patient);
                    // Lưu patient ID để sử dụng khi tạo appointment
                    window.currentPatientId = patient.id;
                } else {
                    // Không tìm thấy bệnh nhân
                    showNotification('Không tìm thấy bệnh nhân với số điện thoại này. Vui lòng nhập thông tin mới.', 'info');
                    resetPatientForm();
                    window.currentPatientId = null;
                }
            })
            .catch(err => {
                console.error('Error searching patient:', err);
                showNotification('Lỗi khi tìm kiếm bệnh nhân: ' + err.message, 'error');
                resetPatientForm();
                window.currentPatientId = null;
            })
            .finally(() => {
                // Reset button state
                if (searchButton) {
                    searchButton.innerHTML = originalText;
                    searchButton.disabled = false;
                }
            });
    };

    // Xử lý tạo bệnh nhân mới
    window.createNewPatientAndUser = function() {
        const nameInput = document.getElementById('addName');
        const phoneInput = document.getElementById('addPhone');
        const emailInput = document.getElementById('addEmail');
        const addressInput = document.getElementById('addAddress');
        const birthDateInput = document.getElementById('addBirthDate');
        const genderInput = document.getElementById('addGender');

        const patientData = {
            name: nameInput ? nameInput.value.trim() : '',
            phone: phoneInput ? phoneInput.value.trim() : '',
            email: emailInput ? emailInput.value.trim() : '',
            address: addressInput ? addressInput.value.trim() : '',
            birthDate: birthDateInput ? birthDateInput.value : '',
            gender: genderInput ? genderInput.value : ''
        };

        // Validate required fields
        if (!patientData.name) {
            showNotification('Vui lòng nhập họ và tên', 'error');
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

        // Validate email format
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(patientData.email)) {
            showNotification('Vui lòng nhập email hợp lệ', 'error');
            return;
        }

        // Hiển thị loading
        const createButton = document.getElementById('createPatientBtn');
        const originalText = createButton ? createButton.innerHTML : 'Tạo bệnh nhân mới';
        if (createButton) {
            createButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Đang tạo...';
            createButton.disabled = true;
        }

        // Tạo mật khẩu ngẫu nhiên
        const randomPassword = generateRandomPassword();

        // Tạo user mới
        const userData = {
            email: patientData.email,
            password: randomPassword,
            role: 'Patient'
        };

        createNewUser(userData)
            .then(userResult => {
                if (userResult && userResult.id) {
                    // Tạo bệnh nhân với user ID
                    patientData.userId = userResult.id;
                    return createNewPatient(patientData);
                } else {
                    throw new Error('Không thể tạo user');
                }
            })
            .then(patientResult => {
                if (patientResult && patientResult.id) {
                    // Gửi email chứa mật khẩu
                    return sendPasswordEmail(patientData.email, randomPassword)
                        .then(() => {
                            showNotification('Đã tạo bệnh nhân mới thành công. Mật khẩu đã được gửi đến email.', 'success');
                            window.currentPatientId = patientResult.id;
                            
                            // Disable các field sau khi tạo thành công
                            if (nameInput) nameInput.disabled = true;
                            if (phoneInput) phoneInput.disabled = true;
                            if (emailInput) emailInput.disabled = true;
                            if (addressInput) addressInput.disabled = true;
                            if (birthDateInput) birthDateInput.disabled = true;
                            if (genderInput) genderInput.disabled = true;
                        });
                } else {
                    throw new Error('Không thể tạo bệnh nhân');
                }
            })
            .catch(err => {
                console.error('Error creating patient:', err);
                showNotification('Lỗi khi tạo bệnh nhân mới: ' + err.message, 'error');
            })
            .finally(() => {
                // Reset button state
                if (createButton) {
                    createButton.innerHTML = originalText;
                    createButton.disabled = false;
                }
            });
    };

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        // Add event listener for phone input
        const phoneInput = document.getElementById('addPhone');
        if (phoneInput) {
            // Add search button next to phone input
            const phoneContainer = phoneInput.parentElement;
            if (phoneContainer && !document.getElementById('searchPatientBtn')) {
                const searchButton = document.createElement('button');
                searchButton.type = 'button';
                searchButton.id = 'searchPatientBtn';
                searchButton.className = 'btn btn-outline-primary btn-sm ms-2';
                searchButton.innerHTML = 'Tìm kiếm';
                searchButton.onclick = window.searchPatient;
                
                phoneContainer.appendChild(searchButton);
            }

            // Auto search when phone number is entered (after 1 second delay)
            let searchTimeout;
            phoneInput.addEventListener('input', function() {
                clearTimeout(searchTimeout);
                const phone = this.value.trim();
                
                if (phone.length >= 10) {
                    searchTimeout = setTimeout(() => {
                        window.searchPatient();
                    }, 1000);
                }
            });
        }

        // Add create patient button if not exists
        if (!document.getElementById('createPatientBtn')) {
            const formContainer = document.querySelector('#offcanvasAppointmentAdd .offcanvas-body');
            if (formContainer) {
                const createButton = document.createElement('button');
                createButton.type = 'button';
                createButton.id = 'createPatientBtn';
                createButton.className = 'btn btn-success btn-sm mt-3 mb-3';
                createButton.innerHTML = '<i class="fas fa-user-plus me-2"></i>Tạo bệnh nhân mới';
                createButton.onclick = window.createNewPatientAndUser;
                
                // Insert button after the patient info section
                const patientInfoEnd = formContainer.querySelector('.form-group:last-of-type');
                if (patientInfoEnd) {
                    patientInfoEnd.parentNode.insertBefore(createButton, patientInfoEnd.nextSibling);
                } else {
                    formContainer.appendChild(createButton);
                }
            }
        }

        // Add reset button for patient info
        if (!document.getElementById('resetPatientBtn')) {
            const formContainer = document.querySelector('#offcanvasAppointmentAdd .offcanvas-body');
            if (formContainer) {
                const resetButton = document.createElement('button');
                resetButton.type = 'button';
                resetButton.id = 'resetPatientBtn';
                resetButton.className = 'btn btn-outline-secondary btn-sm ms-2';
                resetButton.innerHTML = '<i class="fas fa-undo me-2"></i>Reset';
                resetButton.onclick = function() {
                    resetPatientForm();
                    window.currentPatientId = null;
                    showNotification('Đã reset thông tin bệnh nhân', 'info');
                };
                
                // Add next to create button
                const createButton = document.getElementById('createPatientBtn');
                if (createButton) {
                    createButton.parentNode.insertBefore(resetButton, createButton.nextSibling);
                }
            }
        }
    });
})(); 