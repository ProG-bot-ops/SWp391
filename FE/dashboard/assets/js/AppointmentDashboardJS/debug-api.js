// debug-api.js
// File để test và debug API
(function() {
    const API_BASE_URL = 'https://localhost:7097';

    // Test API connection
    window.testAPI = function() {
        console.log('🔍 Testing API connection...');
        
        // Test appointment list
        fetch(`${API_BASE_URL}/api/appointment/list`)
            .then(res => {
                console.log('📡 API Response Status:', res.status);
                console.log('📡 API Response Headers:', res.headers);
                return res.json();
            })
            .then(data => {
                console.log('✅ API Response Data:', data);
                console.log('📊 Data Type:', typeof data);
                console.log('📊 Is Array:', Array.isArray(data));
                if (Array.isArray(data)) {
                    console.log('📊 Array Length:', data.length);
                    if (data.length > 0) {
                        console.log('📊 First Item Structure:', data[0]);
                        console.log('📊 Available Fields:', Object.keys(data[0]));
                    }
                }
            })
            .catch(err => {
                console.error('❌ API Error:', err);
                console.error('❌ Error Message:', err.message);
                console.error('❌ Error Stack:', err.stack);
            });
    };

    // Test specific endpoints
    window.testEndpoints = function() {
        const endpoints = [
            '/api/appointment/list',
            '/api/appointment/doctor/list',
            '/api/appointment/clinic/list',
            '/api/appointment/service/list',
            '/api/appointment/patient/search?term=test',
            '/api/appointment/statistics',
            '/api/appointment/test'
        ];

        endpoints.forEach(endpoint => {
            fetch(`${API_BASE_URL}${endpoint}`)
                .then(res => {
                    console.log(`📡 ${endpoint} - Status:`, res.status);
                    return res.json();
                })
                .then(data => {
                    console.log(`✅ ${endpoint} - Data:`, data);
                })
                .catch(err => {
                    console.error(`❌ ${endpoint} - Error:`, err.message);
                });
        });
    };

    // Create test appointment
    window.createTestAppointment = function() {
        const testData = {
            name: 'Test Patient',
            email: 'test@example.com',
            phone: '0123456789',
            doctorId: 1,
            clinicId: 1,
            serviceId: 1,
            date: '2024-12-25',
            time: '10:00',
            shift: 'Test Appointment',
            type: 'New Patient',
            note: 'Test appointment for debugging'
        };

        console.log('🧪 Creating test appointment:', testData);

        fetch(`${API_BASE_URL}/api/appointment/create`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(testData)
        })
        .then(res => {
            console.log('📡 Create Response Status:', res.status);
            return res.json();
        })
        .then(data => {
            console.log('✅ Create Response Data:', data);
        })
        .catch(err => {
            console.error('❌ Create Error:', err);
        });
    };

    // Test update appointment
    window.testUpdateAppointment = function() {
        const updateData = {
            id: 1,
            patientName: "Updated Patient",
            patientEmail: "updated@example.com",
            date: "2024-12-26",
            time: "11:00",
            reason: "Updated reason",
            patientType: "Old Patient",
            note: "Updated note"
        };

        console.log('🧪 Updating appointment:', updateData);

        fetch(`${API_BASE_URL}/api/appointment/update`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(updateData)
        })
        .then(res => {
            console.log('📡 Update Response Status:', res.status);
            return res.json();
        })
        .then(data => {
            console.log('✅ Update Response Data:', data);
        })
        .catch(err => {
            console.error('❌ Update Error:', err);
        });
    };

    // Test delete appointment
    window.testDeleteAppointment = function() {
        const appointmentId = 1;
        console.log('🧪 Deleting appointment:', appointmentId);

        fetch(`${API_BASE_URL}/api/appointment/delete/${appointmentId}`, {
            method: 'DELETE'
        })
        .then(res => {
            console.log('📡 Delete Response Status:', res.status);
            return res.json();
        })
        .then(data => {
            console.log('✅ Delete Response Data:', data);
        })
        .catch(err => {
            console.error('❌ Delete Error:', err);
        });
    };

    // Add debug buttons to page
    function addDebugButtons() {
        const debugContainer = document.createElement('div');
        debugContainer.className = 'debug-container';
        debugContainer.style.cssText = `
            position: fixed;
            top: 10px;
            right: 10px;
            z-index: 9999;
            background: rgba(0,0,0,0.8);
            color: white;
            padding: 10px;
            border-radius: 5px;
            font-size: 12px;
        `;
        
        debugContainer.innerHTML = `
            <div style="margin-bottom: 10px;">
                <strong>🔧 Debug Tools</strong>
            </div>
            <button onclick="testAPI()" style="margin: 2px; padding: 5px 10px; font-size: 11px;">Test API</button>
            <button onclick="testEndpoints()" style="margin: 2px; padding: 5px 10px; font-size: 11px;">Test All</button>
            <button onclick="createTestAppointment()" style="margin: 2px; padding: 5px 10px; font-size: 11px;">Create Test</button>
            <button onclick="testUpdateAppointment()" style="margin: 2px; padding: 5px 10px; font-size: 11px;">Update Test</button>
            <button onclick="testDeleteAppointment()" style="margin: 2px; padding: 5px 10px; font-size: 11px;">Delete Test</button>
            <button onclick="window.location.reload()" style="margin: 2px; padding: 5px 10px; font-size: 11px;">Reload</button>
        `;
        
        document.body.appendChild(debugContainer);
    }

    // Auto-test on page load
    document.addEventListener('DOMContentLoaded', function() {
        // Add debug buttons
        addDebugButtons();
        
        // Auto-test API after 2 seconds
        setTimeout(() => {
            console.log('🚀 Auto-testing API...');
            testAPI();
        }, 2000);
    });

    // Export functions globally
    window.debugAPI = {
        test: testAPI,
        testEndpoints: testEndpoints,
        createTest: createTestAppointment,
        updateTest: testUpdateAppointment,
        deleteTest: testDeleteAppointment
    };

    console.log('🔧 Debug API tools loaded');
})(); 