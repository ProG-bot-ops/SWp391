// API Service for Patient Appointment
class AppointmentAPIService {
    constructor() {
        this.baseURL = 'https://localhost:7097/api';
        this.token = this.getToken();
    }

    // Get JWT token from localStorage
    getToken() {
        return localStorage.getItem('token') || sessionStorage.getItem('token');
    }

    // Set authorization header
    getHeaders() {
        const headers = {
            'Content-Type': 'application/json',
        };
        
        if (this.token) {
            headers['Authorization'] = `Bearer ${this.token}`;
        }
        
        return headers;
    }

    // Generic API call method
    async apiCall(url, options = {}) {
        try {
            const config = {
                headers: this.getHeaders(),
                ...options
            };

            const response = await fetch(url, config);
            
            if (!response.ok) {
                if (response.status === 401) {
                    // Token expired or invalid
                    this.handleUnauthorized();
                    return null;
                }
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            return data;
        } catch (error) {
            console.error('API call failed:', error);
            throw error;
        }
    }

    // Handle unauthorized access
    handleUnauthorized() {
        console.log('Token expired or invalid. Redirecting to login...');
        localStorage.removeItem('token');
        sessionStorage.removeItem('token');
        window.location.href = '../frontend/login.html';
    }

    // Get current user info for calendar
    async getCurrentUserInfo() {
        return await this.apiCall(`${this.baseURL}/appointment/calendar-user-info`);
    }

    // Get week calendar appointments (no authentication required)
    async getWeekCalendarAppointments(userId, startDate, endDate) {
        const startDateStr = startDate.toISOString().split('T')[0];
        const endDateStr = endDate.toISOString().split('T')[0];
        
        try {
            const response = await fetch(
                `${this.baseURL}/appointment/week-calendar?userId=${userId}&startDate=${startDateStr}&endDate=${endDateStr}`,
                {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                }
            );
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            return data;
        } catch (error) {
            console.error('Week calendar API call failed:', error);
            throw error;
        }
    }

    // Get week statistics (no authentication required)
    async getWeekStatistics(userId, startDate, endDate) {
        const startDateStr = startDate.toISOString().split('T')[0];
        const endDateStr = endDate.toISOString().split('T')[0];
        
        try {
            const response = await fetch(
                `${this.baseURL}/appointment/week-statistics?userId=${userId}&startDate=${startDateStr}&endDate=${endDateStr}`,
                {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                }
            );
            
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            return data;
        } catch (error) {
            console.error('Week statistics API call failed:', error);
            throw error;
        }
    }

    // Get patient appointments with filters (legacy)
    async getPatientAppointments(userId, status = 'all', searchTerm = null, page = 1, pageSize = 100) {
        let url = `${this.baseURL}/appointment/patient-appointments?userId=${userId}&status=${status}&page=${page}&pageSize=${pageSize}`;
        
        if (searchTerm) {
            url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
        }

        return await this.apiCall(url);
    }

    // Get appointment detail
    async getAppointmentDetail(appointmentId) {
        return await this.apiCall(`${this.baseURL}/appointment/patient-appointments/${appointmentId}`);
    }

    // Get available shifts for a doctor on a specific date
    async getAvailableShifts(doctorId, date) {
        const dateStr = date.toISOString().split('T')[0];
        return await this.apiCall(`${this.baseURL}/appointment/available-shifts?doctorId=${doctorId}&date=${dateStr}`);
    }

    // Update appointment
    async updateAppointment(appointmentId, updateData) {
        return await this.apiCall(`${this.baseURL}/appointment/patient-appointments/${appointmentId}`, {
            method: 'PUT',
            body: JSON.stringify(updateData)
        });
    }

    // Update appointment shift
    async updateAppointmentShift(appointmentId, shift) {
        return await this.apiCall(`${this.baseURL}/appointment/patient-appointments/${appointmentId}/shift`, {
            method: 'PUT',
            body: JSON.stringify(shift)
        });
    }

    // Cancel appointment
    async cancelAppointment(appointmentId, cancelReason) {
        return await this.apiCall(`${this.baseURL}/appointment/patient-appointments/${appointmentId}/cancel`, {
            method: 'PUT',
            body: JSON.stringify({
                cancelReason: cancelReason
            })
        });
    }

    // Search appointments (legacy)
    async searchAppointments(searchTerm = null, startDate = null, endDate = null, status = null, page = 1, pageSize = 100) {
        let url = `${this.baseURL}/appointment/patient-appointments/search?page=${page}&pageSize=${pageSize}`;
        
        if (searchTerm) url += `&searchTerm=${encodeURIComponent(searchTerm)}`;
        if (startDate) url += `&startDate=${startDate}`;
        if (endDate) url += `&endDate=${endDate}`;
        if (status) url += `&status=${status}`;

        return await this.apiCall(url);
    }

    // Get appointments for specific week (legacy - use getWeekCalendarAppointments instead)
    async getAppointmentsForWeek(userId, startDate, endDate) {
        return await this.getWeekCalendarAppointments(userId, startDate, endDate);
    }

    // Test API connection
    async testConnection() {
        try {
            const response = await this.apiCall(`${this.baseURL}/appointment/test`);
            return response;
        } catch (error) {
            console.error('API connection test failed:', error);
            return null;
        }
    }

    // Get debug appointments list
    async getDebugAppointments() {
        try {
            const response = await this.apiCall(`${this.baseURL}/appointment/debug-list`);
            return response;
        } catch (error) {
            console.error('Debug appointments failed:', error);
            return null;
        }
    }
}

// Export for global access
window.AppointmentAPIService = AppointmentAPIService; 