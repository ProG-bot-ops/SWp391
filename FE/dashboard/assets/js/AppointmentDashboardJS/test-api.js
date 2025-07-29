// test-api.js
// File test để kiểm tra các API endpoints
(function() {
    // Test function to check if API endpoints are working
    window.testAPIEndpoints = function() {
        console.log('🧪 Bắt đầu test API endpoints...');
        
        const tests = [
            {
                name: 'Test Patient Search',
                test: () => AppointmentAPI.utils.get(AppointmentAPI.patients.searchByPhone('0123456789'))
            },
            {
                name: 'Test Clinics List',
                test: () => AppointmentAPI.utils.get(AppointmentAPI.clinics.list)
            },
            {
                name: 'Test Doctors List',
                test: () => AppointmentAPI.utils.get(AppointmentAPI.doctors.list)
            },
            {
                name: 'Test Services List',
                test: () => AppointmentAPI.utils.get(AppointmentAPI.services.list)
            }
        ];

        let passedTests = 0;
        let totalTests = tests.length;

        tests.forEach((testCase, index) => {
            console.log(`\n📋 Test ${index + 1}: ${testCase.name}`);
            
            testCase.test()
                .then(data => {
                    console.log(`✅ ${testCase.name} - PASSED`);
                    console.log('Response:', data);
                    passedTests++;
                })
                .catch(error => {
                    console.log(`❌ ${testCase.name} - FAILED`);
                    console.log('Error:', error.message);
                })
                .finally(() => {
                    if (index === tests.length - 1) {
                        console.log(`\n📊 Test Results: ${passedTests}/${totalTests} tests passed`);
                        if (passedTests === totalTests) {
                            console.log('🎉 Tất cả tests đều thành công!');
                        } else {
                            console.log('⚠️ Một số tests thất bại. Vui lòng kiểm tra API endpoints.');
                        }
                    }
                });
        });
    };

    // Test patient creation flow
    window.testPatientCreation = function() {
        console.log('🧪 Test patient creation flow...');
        
        const testPatient = {
            name: 'Test Patient',
            phone: '0123456789',
            email: 'test@example.com',
            address: 'Test Address',
            birthDate: '1990-01-01',
            gender: 'Male'
        };

        const testUser = {
            email: testPatient.email,
            password: 'TestPassword123!',
            role: 'Patient'
        };

        // Test user creation
        console.log('📋 Test 1: Create User');
        AppointmentAPI.utils.post(AppointmentAPI.auth.register, testUser)
            .then(userResult => {
                console.log('✅ User created:', userResult);
                
                // Test patient creation
                console.log('📋 Test 2: Create Patient');
                testPatient.userId = userResult.id;
                return AppointmentAPI.utils.post(AppointmentAPI.patients.create, testPatient);
            })
            .then(patientResult => {
                console.log('✅ Patient created:', patientResult);
                
                // Test email sending
                console.log('📋 Test 3: Send Password Email');
                return AppointmentAPI.utils.post(AppointmentAPI.auth.sendPasswordEmail, {
                    email: testPatient.email,
                    password: testUser.password
                });
            })
            .then(emailResult => {
                console.log('✅ Email sent:', emailResult);
                console.log('🎉 Patient creation flow test completed successfully!');
            })
            .catch(error => {
                console.log('❌ Patient creation flow test failed:', error.message);
            });
    };

    // Test appointment creation flow
    window.testAppointmentCreation = function() {
        console.log('🧪 Test appointment creation flow...');
        
        // First get clinics
        AppointmentAPI.utils.get(AppointmentAPI.clinics.list)
            .then(clinics => {
                console.log('✅ Clinics loaded:', clinics);
                if (clinics && clinics.length > 0) {
                    const clinicId = clinics[0].id;
                    
                    // Get doctors for this clinic
                    return AppointmentAPI.utils.get(AppointmentAPI.doctors.list + `?clinicId=${clinicId}`);
                } else {
                    throw new Error('No clinics available');
                }
            })
            .then(doctors => {
                console.log('✅ Doctors loaded:', doctors);
                if (doctors && doctors.length > 0) {
                    const doctorId = doctors[0].id;
                    
                    // Get services for this doctor
                    return AppointmentAPI.utils.get(AppointmentAPI.services.list + `?doctorId=${doctorId}`);
                } else {
                    throw new Error('No doctors available');
                }
            })
            .then(services => {
                console.log('✅ Services loaded:', services);
                if (services && services.length > 0) {
                    const serviceId = services[0].id;
                    
                    // Test appointment creation
                    const testAppointment = {
                        patientId: 1, // Assuming patient ID 1 exists
                        clinicId: 1,
                        doctorId: 1,
                        serviceId: serviceId,
                        date: new Date().toISOString().split('T')[0],
                        shiftId: 1,
                        note: 'Test appointment'
                    };
                    
                    return AppointmentAPI.utils.post(AppointmentAPI.appointments.create, testAppointment);
                } else {
                    throw new Error('No services available');
                }
            })
            .then(appointmentResult => {
                console.log('✅ Appointment created:', appointmentResult);
                console.log('🎉 Appointment creation flow test completed successfully!');
            })
            .catch(error => {
                console.log('❌ Appointment creation flow test failed:', error.message);
            });
    };

    // Add test buttons to the page
    document.addEventListener('DOMContentLoaded', function() {
        // Create test panel
        const testPanel = document.createElement('div');
        testPanel.id = 'testPanel';
        testPanel.className = 'position-fixed bottom-0 end-0 p-3';
        testPanel.style.zIndex = '9999';
        testPanel.style.display = 'none';
        
        testPanel.innerHTML = `
            <div class="card">
                <div class="card-header">
                    <h6 class="mb-0">🧪 API Test Panel</h6>
                </div>
                <div class="card-body">
                    <button class="btn btn-primary btn-sm me-2" onclick="testAPIEndpoints()">
                        Test API Endpoints
                    </button>
                    <button class="btn btn-success btn-sm me-2" onclick="testPatientCreation()">
                        Test Patient Creation
                    </button>
                    <button class="btn btn-info btn-sm" onclick="testAppointmentCreation()">
                        Test Appointment Creation
                    </button>
                </div>
            </div>
        `;
        
        document.body.appendChild(testPanel);
        
        // Add toggle button
        const toggleButton = document.createElement('button');
        toggleButton.className = 'btn btn-warning position-fixed bottom-0 end-0 m-3';
        toggleButton.style.zIndex = '10000';
        toggleButton.innerHTML = '🧪';
        toggleButton.onclick = function() {
            const panel = document.getElementById('testPanel');
            panel.style.display = panel.style.display === 'none' ? 'block' : 'none';
        };
        
        document.body.appendChild(toggleButton);
        
        console.log('🧪 Test panel đã được thêm vào trang. Nhấn nút 🧪 để mở/đóng.');
    });

    console.log('✅ Test API file đã được load');
})();