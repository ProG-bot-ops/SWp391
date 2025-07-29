// api-endpoints.js
// Định nghĩa tất cả các API endpoints cần thiết cho hệ thống appointment
(function() {
    const API_BASE_URL = 'https://localhost:7097';
    
    // API Endpoints Configuration
    window.AppointmentAPI = {
        // Base URL
        baseUrl: API_BASE_URL,
        
        // Appointment endpoints
        appointments: {
            list: `${API_BASE_URL}/api/appointment/list`,
            detail: (id) => `${API_BASE_URL}/api/appointment/detail/${id}`,
            create: `${API_BASE_URL}/api/appointment/create`,
            update: `${API_BASE_URL}/api/appointment/update`,
            delete: (id) => `${API_BASE_URL}/api/appointment/delete/${id}`,
            accept: (id) => `${API_BASE_URL}/api/appointment/accept/${id}`,
            cancel: (id) => `${API_BASE_URL}/api/appointment/cancel/${id}`,
            complete: (id) => `${API_BASE_URL}/api/appointment/complete/${id}`,
            pause: (id) => `${API_BASE_URL}/api/appointment/pause/${id}`,
            test: `${API_BASE_URL}/api/appointment/test`,
            statistics: `${API_BASE_URL}/api/appointment/statistics`
        },
        
        // Doctor endpoints
        doctors: {
            list: `${API_BASE_URL}/api/appointment/doctor/list`,
            byClinic: (clinicId, date) => `${API_BASE_URL}/api/appointment/doctors/${clinicId}?date=${date}`,
            workingDays: (id, year, month) => `${API_BASE_URL}/api/appointment/doctor-working-days/${id}?year=${year}&month=${month}`,
            availableShifts: (id, date) => `${API_BASE_URL}/api/appointment/available-shifts?doctorId=${id}&date=${date}`,
            availableTimeSlots: (id, date) => `${API_BASE_URL}/api/appointment/available-time-slots?doctorId=${id}&date=${date}`,
            bookedTimeSlots: (id, date) => `${API_BASE_URL}/api/appointment/booked-time-slots?doctorId=${id}&date=${date}`,
            services: (id) => `${API_BASE_URL}/api/appointment/services-by-doctor/${id}`
        },
        
        // Clinic endpoints
        clinics: {
            list: `${API_BASE_URL}/api/appointment/clinic/list`,
            active: (date) => `${API_BASE_URL}/api/appointment/clinics?date=${date}`,
            search: (name) => `${API_BASE_URL}/api/appointment/clinics/search?name=${encodeURIComponent(name)}`,
            detail: (id) => `${API_BASE_URL}/api/appointment/clinics/${id}`
        },
        
        // Service endpoints
        services: {
            list: `${API_BASE_URL}/api/appointment/service/list`
        },
        
        // Patient endpoints
        patients: {
            search: (term) => `${API_BASE_URL}/api/appointment/patient/search?term=${encodeURIComponent(term)}`,
            searchByPhone: (phone) => `${API_BASE_URL}/api/patient/search?phone=${encodeURIComponent(phone)}`,
            searchByCCCD: (cccd) => `${API_BASE_URL}/api/patient/search?cccd=${encodeURIComponent(cccd)}`,
            create: `${API_BASE_URL}/api/patient/create`,
            update: (id) => `${API_BASE_URL}/api/patient/update/${id}`,
            detail: (id) => `${API_BASE_URL}/api/patient/detail/${id}`,
            list: `${API_BASE_URL}/api/patient/list`
        },
        
        // Patient search endpoint for new form
        patient: {
            search: `${API_BASE_URL}/api/patient/search`
        },
        
        // Auth endpoints
        auth: {
            register: `${API_BASE_URL}/api/auth/register/patient`,
            login: `${API_BASE_URL}/api/auth/login`,
            sendPasswordEmail: `${API_BASE_URL}/api/auth/send-password-email`,
            resetPassword: `${API_BASE_URL}/api/auth/reset-password`,
            changePassword: `${API_BASE_URL}/api/auth/change-password`
        },
        
        // Time slot endpoints
        timeSlots: {
            bookedTimes: (clinicId, doctorId, serviceId, date) => `${API_BASE_URL}/api/appointment/booked-times?clinicId=${clinicId}&doctorId=${doctorId}&serviceId=${serviceId}&date=${date}`
        },
        
        // Utility functions
        utils: {
            // Make API request with error handling
            request: async function(url, options = {}) {
                try {
                    const response = await fetch(url, {
                        headers: {
                            'Content-Type': 'application/json',
                            ...options.headers
                        },
                        ...options
                    });
                    
                    if (!response.ok) {
                        throw new Error(`HTTP error! status: ${response.status}`);
                    }
                    
                    return await response.json();
                } catch (error) {
                    console.error('API request failed:', error);
                    throw error;
                }
            },
            
            // GET request
            get: function(url) {
                return this.request(url, { method: 'GET' });
            },
            
            // POST request
            post: function(url, data) {
                return this.request(url, {
                    method: 'POST',
                    body: JSON.stringify(data)
                });
            },
            
            // PUT request
            put: function(url, data) {
                return this.request(url, {
                    method: 'PUT',
                    body: JSON.stringify(data)
                });
            },
            
            // DELETE request
            delete: function(url) {
                return this.request(url, { method: 'DELETE' });
            }
        }
    };
    
    // Example usage:
    // const appointments = await AppointmentAPI.utils.get(AppointmentAPI.appointments.list);
    // const newAppointment = await AppointmentAPI.utils.post(AppointmentAPI.appointments.create, appointmentData);
    // const updatedAppointment = await AppointmentAPI.utils.put(AppointmentAPI.appointments.update, appointmentData);
    // await AppointmentAPI.utils.delete(AppointmentAPI.appointments.delete(123));
    
    console.log('✅ API endpoints đã được cấu hình');
})(); 