// Week Calendar Manager with API Integration
class WeekCalendarManager {
    constructor() {
        this.currentDate = new Date(); // Start with today's date
        this.appointments = [];
        this.weekStats = null;
        this.apiService = new AppointmentAPIService();
        this.currentUser = null;
        this.isLoading = false;
        
        this.init();
    }

    async init() {
        console.log('🚀 init() started');
        
        // Render calendar immediately
        this.emergencyRender();
        console.log('✅ emergencyRender completed');
        
        // Bind events immediately
        this.bindEvents();
        console.log('✅ bindEvents completed');
        
        // User will be loaded in the main flow below
        
        // Load user first, then load appointments and statistics
        try {
            await this.loadCurrentUser();
            console.log('✅ loadCurrentUser completed');
            
            // Now load appointments and statistics in parallel
            const [appointmentsResult, statisticsResult] = await Promise.all([
                this.loadAppointments(),
                this.loadWeekStatistics()
            ]);
            console.log('✅ All data loaded in parallel');
        } catch (error) {
            console.error('❌ Error loading data:', error);
        }
        console.log('🚀 init() completed');
    }

    async loadCurrentUser() {
        try {
            const response = await this.apiService.getCurrentUserInfo();
            if (response && response.success) {
                this.currentUser = response.data;
                console.log('✅ Current user loaded:', this.currentUser);
            } else {
                console.error('❌ Failed to load user info');
                this.showError('Không thể tải thông tin người dùng');
            }
        } catch (error) {
            console.error('❌ Error loading user info:', error);
            this.showError('Lỗi kết nối API');
        }
    }

    bindEvents() {
        // Today button
        document.getElementById('todayBtn').addEventListener('click', () => {
            this.goToToday();
        });

        // Navigation buttons
        document.getElementById('prevWeekBtn').addEventListener('click', () => {
            this.navigateWeek(-1);
        });

        document.getElementById('nextWeekBtn').addEventListener('click', () => {
            this.navigateWeek(1);
        });
    }

    async goToToday() {
        this.currentDate = new Date();
        this.renderCalendar();
        await this.loadAppointments();
        await this.loadWeekStatistics();
    }

    async navigateWeek(direction) {
        const newDate = new Date(this.currentDate);
        newDate.setDate(newDate.getDate() + (direction * 7));
        this.currentDate = newDate;
        this.renderCalendar();
        await this.loadAppointments();
        await this.loadWeekStatistics();
    }

    getWeekDates() {
        const dates = [];
        const startOfWeek = this.getStartOfWeek(this.currentDate);
        
        for (let i = 0; i < 7; i++) {
            const date = new Date(startOfWeek);
            date.setDate(date.getDate() + i);
            dates.push(date);
        }
        
        return dates;
    }

    getStartOfWeek(date) {
        const d = new Date(date);
        const day = d.getDay();
        const diff = d.getDate() - day + (day === 0 ? -6 : 1); // Adjust when day is Sunday
        return new Date(d.setDate(diff));
    }

    formatDate(date) {
        const day = date.getDate();
        const dayNames = ['Chủ nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'];
        const dayName = dayNames[date.getDay()];
        return `${day} ${dayName}`;
    }

    formatDateRange() {
        const dates = this.getWeekDates();
        const start = dates[0];
        const end = dates[6];
        
        const formatDate = (date) => {
            return `${String(date.getDate()).padStart(2, '0')}/${String(date.getMonth() + 1).padStart(2, '0')}/${date.getFullYear()}`;
        };
        
        return `${formatDate(start)} - ${formatDate(end)}`;
    }

    renderCalendar() {
        console.log('🔄 renderCalendar called');
        
        // Update week range
        const weekRange = document.getElementById('weekRange');
        if (weekRange) {
            weekRange.textContent = this.formatDateRange();
        }
        
        // Get grid element
        const grid = document.getElementById('weekCalendarGrid');
        if (!grid) {
            console.error('❌ weekCalendarGrid element not found');
            return;
        }
        
        // Set grid styles to ensure visibility
        grid.style.display = 'flex';
        grid.style.flexDirection = 'row';
        grid.style.gap = '10px';
        grid.style.padding = '15px';
        grid.style.backgroundColor = '#f8f9fa';
        grid.style.border = '1px solid #dee2e6';
        grid.style.borderRadius = '8px';
        grid.style.minHeight = '300px';
        
        console.log('📅 Grid found and styled');
        
        // Clear grid
        grid.innerHTML = '';
        console.log('✅ Grid cleared');
        
        // Get week dates
        const dates = this.getWeekDates();
        console.log('📅 renderCalendar - dates:', dates.map(d => d.toISOString().split('T')[0]));
        
        // Create columns for each date
        dates.forEach((date, index) => {
            try {
                const dateStr = date.toISOString().split('T')[0];
                
                // Create column container
                const column = document.createElement('div');
                column.className = 'calendar-column';
                column.style.flex = '1';
                column.style.minWidth = '150px';
                column.style.backgroundColor = 'white';
                column.style.border = '1px solid #ced4da';
                column.style.borderRadius = '6px';
                column.style.padding = '10px';
                column.style.margin = '5px';
                
                // Create header
                const header = document.createElement('div');
                header.className = 'calendar-column-header';
                header.textContent = this.formatDate(date);
                header.style.backgroundColor = '#e9ecef';
                header.style.padding = '8px';
                header.style.marginBottom = '10px';
                header.style.borderRadius = '4px';
                header.style.fontWeight = 'bold';
                header.style.textAlign = 'center';
                
                // Create content area
                const content = document.createElement('div');
                content.className = 'day-content';
                content.dataset.date = dateStr;
                content.style.minHeight = '200px';
                content.style.padding = '10px';
                content.style.backgroundColor = '#f8f9fa';
                content.style.borderRadius = '4px';
                
                // Add placeholder text
                content.innerHTML = '<div class="text-muted text-center">Không có lịch hẹn</div>';
                
                // Assemble column
                column.appendChild(header);
                column.appendChild(content);
                grid.appendChild(column);
                
                console.log(`✅ Tạo calendar column ${index + 1} cho ${dateStr}`);
            } catch (error) {
                console.error(`❌ Lỗi khi tạo column cho date ${date}:`, error);
            }
        });
        
        // Verify creation
        const createdElements = document.querySelectorAll('[data-date]');
        console.log(`✅ Tạo thành công ${createdElements.length} calendar columns`);
        
        createdElements.forEach((el, index) => {
            console.log(`📅 Column ${index + 1}: data-date="${el.dataset.date}"`);
        });
    }

    async loadAppointments() {
        if (!this.currentUser) {
            console.log('⚠️ No user info available, skipping appointment load');
            return;
        }

        // Ensure calendar is rendered before loading appointments
        if (!document.querySelector('[data-date]')) {
            console.log('🔄 Calendar not ready, rendering first...');
            this.emergencyRender();
        }

        this.setLoading(true);
        
        try {
            const dates = this.getWeekDates();
            const startDate = dates[0];
            const endDate = dates[6];
            
            console.log('🔄 Loading appointments for week:', startDate.toISOString().split('T')[0], 'to', endDate.toISOString().split('T')[0]);
            
            const response = await this.apiService.getWeekCalendarAppointments(
                this.currentUser.id,
                startDate,
                endDate
            );
            
            if (response && response.success) {
                this.appointments = response.data.appointments || [];
                console.log('✅ Appointments loaded:', this.appointments.length);
                
                // Render immediately without setTimeout
                this.renderAppointments();
                this.renderWeekStats(response.data.statistics);
            } else {
                console.error('❌ Failed to load appointments:', response?.message);
                this.showError('Không thể tải lịch hẹn: ' + (response?.message || 'Lỗi không xác định'));
            }
        } catch (error) {
            console.error('❌ Error loading appointments:', error);
            this.showError('Lỗi kết nối khi tải lịch hẹn');
        } finally {
            this.setLoading(false);
        }
    }

    async loadWeekStatistics() {
        if (!this.currentUser) return;

        try {
            const dates = this.getWeekDates();
            const startDate = dates[0];
            const endDate = dates[6];
            
            const response = await this.apiService.getWeekStatistics(
                this.currentUser.id,
                startDate,
                endDate
            );
            
            if (response && response.success) {
                this.weekStats = response.data;
                console.log('✅ Week statistics loaded');
                
                // Render immediately without setTimeout
                this.renderDayStats();
            }
        } catch (error) {
            console.error('❌ Error loading week statistics:', error);
        }
    }

    renderAppointments() {
        console.log('🔄 renderAppointments() started');
        
        if (!this.appointments || this.appointments.length === 0) {
            console.log('📅 No appointments to render');
            return;
        }
        
        const dates = this.getWeekDates();
        console.log('🔄 Rendering', this.appointments.length, 'appointments for', dates.length, 'days');
        console.log('📅 Appointments data:', this.appointments);
        
        // Debug: Check calendar elements
        const calendarElements = document.querySelectorAll('[data-date]');
        console.log('📅 Calendar elements found:', calendarElements.length);
        
        dates.forEach(date => {
            const dateStr = date.toISOString().split('T')[0];
            console.log(`🔍 Looking for element with data-date="${dateStr}"`);
            const content = document.querySelector(`[data-date="${dateStr}"]`);
            
            console.log(`🔍 Element found:`, !!content);
            if (content) {
                // Clear existing content
                content.innerHTML = '';
                
                const dayAppointments = this.appointments.filter(apt => {
                    if (!apt.appointmentDate) return false;
                    
                    // Convert to string if it's a Date object
                    const dateValue = apt.appointmentDate instanceof Date ? apt.appointmentDate.toISOString().split('T')[0] : apt.appointmentDate;
                    
                    if (typeof dateValue === 'string') {
                        let aptDateStr = '';
                        
                        if (dateValue.includes('/')) {
                            // Format: dd/MM/yyyy
                            const aptDateParts = dateValue.split('/');
                            if (aptDateParts.length === 3) {
                                aptDateStr = `${aptDateParts[2]}-${aptDateParts[1].padStart(2, '0')}-${aptDateParts[0].padStart(2, '0')}`;
                            }
                        } else if (dateValue.includes('-')) {
                            // Format: yyyy-MM-dd or dd-MM-yyyy
                            const aptDateParts = dateValue.split('-');
                            if (aptDateParts.length === 3) {
                                if (aptDateParts[0].length === 4) {
                                    // Format: yyyy-MM-dd
                                    aptDateStr = dateValue;
                                } else {
                                    // Format: dd-MM-yyyy
                                    aptDateStr = `${aptDateParts[2]}-${aptDateParts[1].padStart(2, '0')}-${aptDateParts[0].padStart(2, '0')}`;
                                }
                            }
                        } else {
                            // Try to parse as Date object
                            try {
                                const aptDate = new Date(dateValue);
                                if (!isNaN(aptDate.getTime())) {
                                    aptDateStr = aptDate.toISOString().split('T')[0];
                                }
                            } catch (e) {
                                return false;
                            }
                        }
                        
                        return aptDateStr === dateStr;
                    }
                    
                    return false;
                });
                
                if (dayAppointments.length === 0) {
                    content.innerHTML = '<div class="text-muted text-center">Không có lịch hẹn</div>';
                } else {
                    console.log(`📅 Found ${dayAppointments.length} appointments for ${dateStr}`);
                    dayAppointments.forEach(appointment => {
                        console.log(`🎴 Creating card for appointment:`, appointment.id, appointment.patient?.name);
                        const card = this.createAppointmentCard(appointment);
                        content.appendChild(card);
                        console.log(`✅ Card added for ${appointment.patient?.name}`);
                    });
                }
            }
        });
        
        console.log('✅ Appointments rendering completed');
        
        // Force a re-render if no cards were created
        const allCards = document.querySelectorAll('.appointment-card');
        console.log('📊 Total appointment cards rendered:', allCards.length);
        
        if (allCards.length === 0 && this.appointments.length > 0) {
            console.log('⚠️ No cards rendered despite having appointments, forcing re-render...');
            setTimeout(() => {
                this.renderAppointments();
            }, 100);
        }
    }

    renderWeekStats(stats) {
        if (!stats) return;

        // Tạo hoặc cập nhật stats container
        let statsContainer = document.getElementById('weekStatsContainer');
        if (!statsContainer) {
            statsContainer = document.createElement('div');
            statsContainer.id = 'weekStatsContainer';
            statsContainer.className = 'week-stats-container';
            
            const calendarHeader = document.querySelector('.calendar-header');
            if (calendarHeader) {
                calendarHeader.appendChild(statsContainer);
            } else {
                console.warn('Không tìm thấy .calendar-header element');
                return;
            }
        }

        statsContainer.innerHTML = `
            <div class="week-stats">
                <div class="stat-item">
                    <span class="stat-label">Tổng cộng:</span>
                    <span class="stat-value total">${stats.total}</span>
                </div>
                <div class="stat-item">
                    <span class="stat-label">Đã lên lịch:</span>
                    <span class="stat-value scheduled">${stats.scheduled}</span>
                </div>
                <div class="stat-item">
                    <span class="stat-label">Đang khám:</span>
                    <span class="stat-value in-progress">${stats.inProgress || 0}</span>
                </div>
                <div class="stat-item">
                    <span class="stat-label">Đã hoàn thành:</span>
                    <span class="stat-value completed">${stats.completed}</span>
                </div>
                <div class="stat-item">
                    <span class="stat-label">Đã hủy:</span>
                    <span class="stat-value cancelled">${stats.cancelled}</span>
                </div>

            </div>
        `;
    }

    renderDayStats() {
        if (!this.weekStats) return;

        // Kiểm tra xem calendar đã được render chưa
        const existingElements = document.querySelectorAll('[data-date]');
        if (existingElements.length === 0) {
            console.warn('⚠️ Calendar chưa được render, đang render lại...');
            this.renderCalendar();
        }

        const dates = this.getWeekDates();
        console.log('🔍 renderDayStats - dates:', dates.map(d => d.toISOString().split('T')[0]));
        
        dates.forEach((date, index) => {
            const dateStr = date.toISOString().split('T')[0];
            console.log(`🔍 Tìm kiếm element với data-date="${dateStr}"`);
            
            const dateElement = document.querySelector(`[data-date="${dateStr}"]`);
            
            // Kiểm tra xem element có tồn tại không
            if (!dateElement) {
                console.warn(`❌ Không tìm thấy element với data-date="${dateStr}"`);
                console.log('🔍 Tất cả elements với data-date:', document.querySelectorAll('[data-date]'));
                return;
            }
            
            console.log(`✅ Tìm thấy element với data-date="${dateStr}"`);
            
            const column = dateElement.parentElement;
            if (!column) {
                console.warn(`❌ Không tìm thấy parent element cho data-date="${dateStr}"`);
                return;
            }
            
            // Tìm hoặc tạo day stats element
            let dayStats = column.querySelector('.day-stats');
            if (!dayStats) {
                dayStats = document.createElement('div');
                dayStats.className = 'day-stats';
                column.appendChild(dayStats);
                console.log(`✅ Tạo mới day-stats cho ${dateStr}`);
            }

            const dayData = this.weekStats[index];
            if (dayData) {
                dayStats.innerHTML = `
                    <div class="day-stat-item">
                        <small class="text-muted">${dayData.total} lịch hẹn</small>
                    </div>
                `;
                console.log(`✅ Cập nhật stats cho ${dateStr}: ${dayData.total} lịch hẹn`);
            }
        });
    }

    createAppointmentCard(appointment) {
        console.log('🎴 Creating appointment card for:', appointment);
        
        const card = document.createElement('div');
        card.className = `appointment-card ${this.getStatusClass(appointment.status)}`;
        
        const statusText = appointment.statusText || this.getStatusText(appointment.status);
        const patientName = appointment.patient?.name || 'N/A';
        const patientPhone = appointment.patient?.phone || 'N/A';
        const timeRange = `${appointment.startTime || 'N/A'} - ${appointment.endTime || 'N/A'}`;
        const note = appointment.note ? `<div class="appointment-note">${appointment.note}</div>` : '';
        const clinicName = appointment.clinic?.name ? `<div class="appointment-clinic">${appointment.clinic.name}</div>` : '';
        const serviceName = appointment.service?.name ? `<div class="appointment-service">${appointment.service.name}</div>` : '';
        
        card.innerHTML = `
            <div class="appointment-card-header">
                <div class="appointment-info">
                    <div class="appointment-patient-name">${patientName}</div>
                    <div class="appointment-phone">${patientPhone}</div>
                    <div class="appointment-time">${timeRange}</div>
                    ${note}
                    ${clinicName}
                    ${serviceName}
                </div>
                <div class="appointment-card-actions">
                    <button title="Chỉnh sửa" onclick="editAppointment('${appointment.id}')">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button title="Xem chi tiết" onclick="viewAppointment('${appointment.id}')">
                        <i class="fas fa-external-link-alt"></i>
                    </button>
                </div>
            </div>
            <div class="appointment-status ${this.getStatusClass(appointment.status)}">${statusText}</div>
        `;
        
        console.log('🎴 Appointment card created:', card.outerHTML);
        return card;
    }

    getStatusClass(status) {
        const statusMap = {
            'Scheduled': 'scheduled',
            'InProgress': 'in-progress',
            'Completed': 'arrived',
            'Cancelled': 'cancelled',

        };
        return statusMap[status] || 'scheduled';
    }

    getStatusText(status) {
        const statusMap = {
            'Scheduled': 'Đã lên lịch',
            'InProgress': 'Đang khám',
            'Completed': 'Đã hoàn thành',
            'Cancelled': 'Đã hủy',

        };
        return statusMap[status] || status;
    }

    setLoading(loading) {
        this.isLoading = loading;
        const grid = document.getElementById('weekCalendarGrid');
        
        if (loading) {
            grid.innerHTML = `
                <div class="calendar-loading">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                    <div class="mt-2">Đang tải lịch hẹn...</div>
                </div>
            `;
        }
        // Không xóa grid khi loading = false để giữ lại appointments đã render
    }

    showError(message) {
        const grid = document.getElementById('weekCalendarGrid');
        grid.innerHTML = `
            <div class="calendar-loading">
                <div class="text-danger">
                    <i class="fas fa-exclamation-triangle"></i>
                    <div class="mt-2">${message}</div>
                    <button class="btn btn-outline-primary mt-3" onclick="window.weekCalendarManager.loadAppointments()">
                        <i class="fas fa-redo"></i> Thử lại
                    </button>
                </div>
            </div>
        `;
    }

    // Debug method to check appointments
    debugAppointments() {
        console.log('🔍 Debug Appointments:');
        console.log('Current user:', this.currentUser);
        console.log('Appointments count:', this.appointments.length);
        console.log('Appointments data:', this.appointments);
        console.log('Week dates:', this.getWeekDates().map(d => d.toISOString().split('T')[0]));
        console.log('Calendar elements:', document.querySelectorAll('[data-date]').length);
        
        // Check each appointment date format
        this.appointments.forEach((apt, index) => {
            console.log(`Appointment ${index + 1}:`, {
                id: apt.id,
                date: apt.appointmentDate,
                patient: apt.patient?.name,
                status: apt.status
            });
        });
    }

    // Force render appointments method
    forceRenderAppointments() {
        console.log('🔄 Force rendering appointments...');
        this.renderCalendar();
        this.renderAppointments();
    }

    // Test with mock data method
    testWithMockData() {
        console.log('🧪 Testing with mock data...');
        
        // Mock appointments data
        const mockAppointments = [
            {
                id: 1,
                appointmentDate: "02/08/2025", // dd/MM/yyyy format
                startTime: "08:00",
                endTime: "09:00",
                status: "Scheduled",
                statusText: "Đã lên lịch",
                patient: {
                    name: "Nguyễn Văn Test",
                    phone: "0123456789"
                },
                clinic: { name: "Phòng khám Test" },
                service: { name: "Khám tổng quát" }
            }
        ];
        
        // Temporarily set mock data
        this.appointments = mockAppointments;
        console.log('✅ Mock data set:', this.appointments);
        
        // Force render
        this.renderCalendar();
        this.renderAppointments();
    }

    // Compare real data with mock data
    compareDataFormats() {
        console.log('🔍 Comparing data formats...');
        
        if (this.appointments.length === 0) {
            console.log('❌ No real appointments data to compare');
            return;
        }
        
        const realAppointment = this.appointments[0];
        const mockAppointment = {
            id: 1,
            appointmentDate: "02/08/2025",
            startTime: "08:00",
            endTime: "09:00",
            status: "Scheduled",
            statusText: "Đã lên lịch",
            patient: { name: "Test" },
            clinic: { name: "Test" },
            service: { name: "Test" }
        };
        
        console.log('📊 Real appointment structure:', {
            id: realAppointment.id,
            appointmentDate: realAppointment.appointmentDate,
            dateType: typeof realAppointment.appointmentDate,
            dateValue: realAppointment.appointmentDate,
            patient: realAppointment.patient,
            status: realAppointment.status
        });
        
        console.log('📊 Mock appointment structure:', {
            id: mockAppointment.id,
            appointmentDate: mockAppointment.appointmentDate,
            dateType: typeof mockAppointment.appointmentDate,
            dateValue: mockAppointment.appointmentDate,
            patient: mockAppointment.patient,
            status: mockAppointment.status
        });
        
        // Test date conversion for real data
        const realDate = realAppointment.appointmentDate;
        const mockDate = mockAppointment.appointmentDate;
        
        console.log('🔍 Date conversion test:');
        console.log('- Real date:', realDate, 'Type:', typeof realDate);
        console.log('- Mock date:', mockDate, 'Type:', typeof mockDate);
        
        // Test conversion for real date
        if (typeof realDate === 'string') {
            if (realDate.includes('/')) {
                const parts = realDate.split('/');
                const converted = `${parts[2]}-${parts[1].padStart(2, '0')}-${parts[0].padStart(2, '0')}`;
                console.log('- Real date converted:', realDate, '->', converted);
            }
        }
    }

    // Force render with real data
    forceRenderWithRealData() {
        console.log('🔄 Force rendering with real data...');
        
        if (this.appointments.length === 0) {
            console.log('❌ No appointments data available');
            return;
        }
        
        // Log current week dates
        const dates = this.getWeekDates();
        console.log('📅 Current week dates:', dates.map(d => d.toISOString().split('T')[0]));
        
        // Log all appointments with their dates
        this.appointments.forEach((apt, index) => {
            console.log(`📅 Appointment ${index + 1}:`, {
                id: apt.id,
                date: apt.appointmentDate,
                patient: apt.patient?.name,
                status: apt.status
            });
        });
        
        // Force render calendar first
        this.renderCalendar();
        
        // Then force render appointments
        setTimeout(() => {
            this.renderAppointments();
        }, 50);
    }

    // Force render everything
    forceRenderEverything() {
        console.log('🔄 Force rendering everything...');
        
        // Force render calendar first
        this.renderCalendar();
        
        // Wait a bit then render appointments
        setTimeout(() => {
            console.log('🔄 Rendering appointments after calendar...');
            this.renderAppointments();
        }, 100);
    }

    // Monitor DOM changes
    startDOMMonitoring() {
        console.log('🔍 Starting DOM monitoring...');
        
        const grid = document.getElementById('weekCalendarGrid');
        if (!grid) {
            console.log('❌ Grid not found for monitoring');
            return;
        }
        
        // Monitor innerHTML changes
        let lastInnerHTML = grid.innerHTML;
        setInterval(() => {
            if (grid.innerHTML !== lastInnerHTML) {
                console.log('⚠️ Grid innerHTML changed!');
                console.log('   Previous length:', lastInnerHTML.length);
                console.log('   Current length:', grid.innerHTML.length);
                console.log('   Current children:', grid.children.length);
                lastInnerHTML = grid.innerHTML;
            }
        }, 100);
        
        // Monitor data-date elements
        setInterval(() => {
            const dateElements = document.querySelectorAll('[data-date]');
            if (dateElements.length === 0) {
                console.log('⚠️ No data-date elements found!');
            } else {
                console.log(`📅 Found ${dateElements.length} data-date elements`);
            }
        }, 500);
    }

    // Force render and keep elements
    forceRenderAndKeep() {
        console.log('🔄 Force rendering and keeping elements...');
        
        // Render calendar
        this.renderCalendar();
        
        // Keep monitoring and re-render if elements disappear
        const checkAndRender = () => {
            const dateElements = document.querySelectorAll('[data-date]');
            if (dateElements.length === 0) {
                console.log('⚠️ Elements disappeared, re-rendering...');
                this.renderCalendar();
            }
        };
        
        // Check every 200ms
        setInterval(checkAndRender, 200);
        
        // Render appointments after a delay
        setTimeout(() => {
            this.renderAppointments();
        }, 150);
    }

    // Simple test render
    testSimpleRender() {
        console.log('🧪 Testing simple render...');
        
        const grid = document.getElementById('weekCalendarGrid');
        if (!grid) {
            console.error('❌ Grid not found');
            return;
        }
        
        // Check grid visibility
        const gridStyle = window.getComputedStyle(grid);
        console.log('📅 Grid visibility:', {
            display: gridStyle.display,
            visibility: gridStyle.visibility,
            opacity: gridStyle.opacity,
            height: gridStyle.height,
            width: gridStyle.width
        });
        
        // Force grid to be visible
        grid.style.display = 'flex';
        grid.style.flexDirection = 'row';
        grid.style.gap = '10px';
        grid.style.padding = '20px';
        grid.style.backgroundColor = '#f0f0f0';
        grid.style.border = '2px solid blue';
        grid.style.minHeight = '200px';
        
        // Clear grid
        grid.innerHTML = '';
        
        // Create one simple column
        const column = document.createElement('div');
        column.className = 'calendar-column';
        column.style.border = '3px solid red';
        column.style.padding = '15px';
        column.style.margin = '5px';
        column.style.backgroundColor = 'white';
        column.style.minWidth = '150px';
        column.style.minHeight = '100px';
        
        const header = document.createElement('div');
        header.textContent = 'Test Header';
        header.style.backgroundColor = 'yellow';
        header.style.padding = '5px';
        header.style.marginBottom = '10px';
        header.style.fontWeight = 'bold';
        
        const content = document.createElement('div');
        content.className = 'day-content';
        content.dataset.date = '2025-08-02';
        content.textContent = 'Test Content - Should be visible!';
        content.style.backgroundColor = 'lightblue';
        content.style.padding = '10px';
        content.style.border = '1px solid green';
        
        column.appendChild(header);
        column.appendChild(content);
        grid.appendChild(column);
        
        console.log('✅ Test column created');
        console.log('📅 Grid children:', grid.children.length);
        console.log('📅 Data-date elements:', document.querySelectorAll('[data-date]').length);
        
        // Check if elements are actually in DOM
        const testContent = document.querySelector('[data-date="2025-08-02"]');
        console.log('📅 Test content found:', !!testContent);
        if (testContent) {
            console.log('📅 Test content text:', testContent.textContent);
        }
    }

    // Check page structure
    checkPageStructure() {
        console.log('🔍 Checking page structure...');
        
        // Check main elements
        const mainElements = [
            'weekCalendarGrid',
            'weekRange',
            'todayBtn',
            'prevWeekBtn',
            'nextWeekBtn'
        ];
        
        mainElements.forEach(id => {
            const element = document.getElementById(id);
            console.log(`📅 ${id}:`, {
                exists: !!element,
                visible: element ? window.getComputedStyle(element).display !== 'none' : false,
                children: element ? element.children.length : 0
            });
        });
        
        // Check calendar container
        const container = document.querySelector('.week-calendar-container');
        console.log('📅 week-calendar-container:', {
            exists: !!container,
            visible: container ? window.getComputedStyle(container).display !== 'none' : false,
            children: container ? container.children.length : 0
        });
        
        // Check if page is loaded
        console.log('📅 Document ready state:', document.readyState);
        console.log('📅 Body children:', document.body.children.length);
    }

    // Force render and test immediately
    forceRenderAndTest() {
        console.log('🚀 Force rendering and testing immediately...');
        
        // Check if grid exists
        const grid = document.getElementById('weekCalendarGrid');
        if (!grid) {
            console.error('❌ weekCalendarGrid not found!');
            return;
        }
        
        console.log('✅ Grid found, forcing render...');
        
        // Force render calendar
        this.renderCalendar();
        
        // Wait a moment then render appointments
        setTimeout(() => {
            console.log('🔄 Rendering appointments...');
            this.renderAppointments();
            
            // Test with mock data if no real appointments
            setTimeout(() => {
                const dateElements = document.querySelectorAll('[data-date]');
                if (dateElements.length > 0) {
                    console.log('✅ Calendar elements found, testing with mock data...');
                    this.testWithMockData();
                } else {
                    console.log('❌ No calendar elements found');
                    console.log('🔍 Grid children:', grid.children.length);
                    console.log('🔍 Grid innerHTML length:', grid.innerHTML.length);
                }
            }, 500);
        }, 200);
    }

    // Emergency render - create everything from scratch
    emergencyRender() {
        console.log('🚨 Emergency render - creating everything from scratch...');
        
        // Find or create grid
        let grid = document.getElementById('weekCalendarGrid');
        if (!grid) {
            console.log('❌ Grid not found, creating container...');
            const container = document.querySelector('.week-calendar-container');
            if (container) {
                grid = document.createElement('div');
                grid.id = 'weekCalendarGrid';
                grid.className = 'week-calendar-grid';
                container.appendChild(grid);
                console.log('✅ Created new grid');
            } else {
                console.error('❌ Container not found either!');
                // Try to find any container
                const anyContainer = document.querySelector('.container, .main-content, body');
                if (anyContainer) {
                    grid = document.createElement('div');
                    grid.id = 'weekCalendarGrid';
                    grid.className = 'week-calendar-grid';
                    anyContainer.appendChild(grid);
                    console.log('✅ Created grid in fallback container');
                } else {
                    console.error('❌ No container found anywhere!');
                    return;
                }
            }
        }
        
        // Force styles
        grid.style.display = 'flex';
        grid.style.flexDirection = 'row';
        grid.style.gap = '10px';
        grid.style.padding = '15px';
        grid.style.backgroundColor = '#f8f9fa';
        grid.style.border = '2px solid #007bff';
        grid.style.borderRadius = '8px';
        grid.style.minHeight = '300px';
        
        // Clear and create
        grid.innerHTML = '';
        
        const dates = this.getWeekDates();
        console.log('📅 Creating columns for dates:', dates.map(d => d.toISOString().split('T')[0]));
        
        dates.forEach((date, index) => {
            const dateStr = date.toISOString().split('T')[0];
            
            const column = document.createElement('div');
            column.className = 'calendar-column';
            column.style.cssText = `
                flex: 1;
                min-width: 150px;
                background-color: white;
                border: 1px solid #ced4da;
                border-radius: 6px;
                padding: 10px;
                margin: 5px;
            `;
            
            const header = document.createElement('div');
            header.className = 'calendar-column-header';
            header.textContent = this.formatDate(date);
            header.style.cssText = `
                background-color: #e9ecef;
                padding: 8px;
                margin-bottom: 10px;
                border-radius: 4px;
                font-weight: bold;
                text-align: center;
            `;
            
            const content = document.createElement('div');
            content.className = 'day-content';
            content.dataset.date = dateStr;
            content.style.cssText = `
                min-height: 200px;
                padding: 10px;
                background-color: #f8f9fa;
                border-radius: 4px;
            `;
            content.innerHTML = '<div class="text-muted text-center">Không có lịch hẹn</div>';
            
            column.appendChild(header);
            column.appendChild(content);
            grid.appendChild(column);
            
            console.log(`✅ Created column ${index + 1} for ${dateStr}`);
        });
        
        console.log(`✅ Emergency render completed. Grid children: ${grid.children.length}`);
        console.log(`✅ Data-date elements: ${document.querySelectorAll('[data-date]').length}`);
    }

    // Auto-detect and render if needed
    autoDetectAndRender() {
        console.log('🔍 Auto-detecting calendar state...');
        
        const grid = document.getElementById('weekCalendarGrid');
        const dateElements = document.querySelectorAll('[data-date]');
        
        console.log('📅 Current state:', {
            gridExists: !!grid,
            gridChildren: grid ? grid.children.length : 0,
            dateElements: dateElements.length
        });
        
        if (!grid || dateElements.length === 0) {
            console.log('🔄 Calendar needs rendering, doing emergency render...');
            this.emergencyRender();
        } else {
            console.log('✅ Calendar is already rendered');
        }
    }

    // Test with specific appointment
    testSpecificAppointment(appointmentIndex = 0) {
        console.log('🧪 Testing with specific appointment...');
        
        if (this.appointments.length === 0) {
            console.log('❌ No appointments available');
            return;
        }
        
        if (appointmentIndex >= this.appointments.length) {
            console.log(`❌ Appointment index ${appointmentIndex} out of range (0-${this.appointments.length - 1})`);
            return;
        }
        
        const appointment = this.appointments[appointmentIndex];
        console.log('🧪 Testing appointment:', appointment);
        
        // Get current week dates
        const dates = this.getWeekDates();
        console.log('📅 Week dates:', dates.map(d => d.toISOString().split('T')[0]));
        
        // Test date matching for each day
        dates.forEach((date, dayIndex) => {
            const dateStr = date.toISOString().split('T')[0];
            console.log(`\n🔍 Testing day ${dayIndex + 1} (${dateStr}):`);
            
            // Parse appointment date
            let aptDateStr = '';
            const dateValue = appointment.appointmentDate instanceof Date ? 
                appointment.appointmentDate.toISOString().split('T')[0] : 
                appointment.appointmentDate;
            
            if (typeof dateValue === 'string') {
                if (dateValue.includes('/')) {
                    const parts = dateValue.split('/');
                    if (parts.length === 3) {
                        aptDateStr = `${parts[2]}-${parts[1].padStart(2, '0')}-${parts[0].padStart(2, '0')}`;
                    }
                } else if (dateValue.includes('-')) {
                    const parts = dateValue.split('-');
                    if (parts.length === 3) {
                        if (parts[0].length === 4) {
                            aptDateStr = dateValue;
                        } else {
                            aptDateStr = `${parts[2]}-${parts[1].padStart(2, '0')}-${parts[0].padStart(2, '0')}`;
                        }
                    }
                }
            }
            
            console.log(`   Appointment date: ${aptDateStr}`);
            console.log(`   Calendar date: ${dateStr}`);
            console.log(`   Match: ${aptDateStr === dateStr}`);
            
            // Check if element exists
            const element = document.querySelector(`[data-date="${dateStr}"]`);
            console.log(`   Element exists: ${!!element}`);
        });
    }
}

// Global functions for appointment actions
async function editAppointment(id) {
    console.log('Edit appointment:', id);
    
    // Check if user is logged in
    const token = localStorage.getItem('token') || sessionStorage.getItem('token');
    if (!token) {
        alert('Vui lòng đăng nhập để chỉnh sửa lịch hẹn');
        return;
    }
    
    // Open edit modal
    if (typeof openEditModal === 'function') {
        await openEditModal(id);
    } else {
        console.error('openEditModal function not found');
        alert('Chức năng chỉnh sửa chưa sẵn sàng');
    }
}

async function viewAppointment(id) {
    console.log('View appointment:', id);
    try {
        const response = await window.weekCalendarManager.apiService.getAppointmentDetail(id);
        if (response && response.success) {
            const apt = response.data;
            
            // Debug: Log toàn bộ dữ liệu appointment
            console.log('🔍 Appointment data:', apt);
            console.log('🔍 Shift value:', apt.shift);
            console.log('🔍 Doctors:', apt.doctors);
            
            // Xử lý thông tin bác sĩ
            let doctorInfo = 'Chưa phân công';
            if (apt.doctors && apt.doctors.length > 0) {
                const doctorNames = apt.doctors.map(d => d.name).join(', ');
                const doctorPhones = apt.doctors.map(d => d.phone).filter(p => p).join(', ');
                doctorInfo = `Bác sĩ: ${doctorNames}`;
                if (doctorPhones) {
                    doctorInfo += `\nSĐT: ${doctorPhones}`;
                }
            }
            
            // Xử lý ca khám
            let shiftInfo = 'Chưa phân ca';
            if (apt.shift && apt.shift.trim() !== '') {
                shiftInfo = apt.shift;
            }
            
            const details = `
Chi tiết lịch hẹn #${apt.id}

Bệnh nhân: ${apt.patient?.name || 'N/A'}
Số điện thoại: ${apt.patient?.phone || 'N/A'}
Ngày hẹn: ${apt.appointmentDate || 'N/A'}
Thời gian: ${apt.startTime || 'N/A'} - ${apt.endTime || 'N/A'}
Ca khám: ${shiftInfo}
${doctorInfo}
Phòng khám: ${apt.clinic?.name || 'N/A'}
Dịch vụ: ${apt.service?.name || 'N/A'}
Trạng thái: ${apt.statusText || 'N/A'}
Ghi chú: ${apt.note || 'Không có'}
            `;
            alert(details);
        } else {
            alert('Không thể tải thông tin lịch hẹn');
        }
    } catch (error) {
        console.error('Error getting appointment detail:', error);
        alert('Lỗi khi tải thông tin lịch hẹn');
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', async function() {
    console.log('🚀 Khởi tạo Week Calendar Manager với API...');
    
    // Kiểm tra xem các elements cần thiết có tồn tại không
    const weekCalendarGrid = document.getElementById('weekCalendarGrid');
    const weekRange = document.getElementById('weekRange');
    const todayBtn = document.getElementById('todayBtn');
    const prevWeekBtn = document.getElementById('prevWeekBtn');
    const nextWeekBtn = document.getElementById('nextWeekBtn');
    
    console.log('🔍 Kiểm tra elements:', {
        weekCalendarGrid: !!weekCalendarGrid,
        weekRange: !!weekRange,
        todayBtn: !!todayBtn,
        prevWeekBtn: !!prevWeekBtn,
        nextWeekBtn: !!nextWeekBtn
    });
    
    if (!weekCalendarGrid) {
        console.error('❌ Không tìm thấy weekCalendarGrid element');
        return;
    }
    
    // Test API connection first
    const apiService = new AppointmentAPIService();
    const testResponse = await apiService.testConnection();
    
    if (testResponse) {
        console.log('✅ API connection test successful');
        
        // Initialize the calendar manager
        window.weekCalendarManager = new WeekCalendarManager();
        console.log('✅ Week Calendar Manager đã được khởi tạo thành công');
        
        // Thêm debug info
        setTimeout(() => {
            console.log('🔍 Debug sau khi khởi tạo:');
            console.log('- WeekCalendarManager:', !!window.weekCalendarManager);
            console.log('- Appointments count:', window.weekCalendarManager?.appointments?.length || 0);
            console.log('- Current user:', window.weekCalendarManager?.currentUser);
            console.log('- Calendar elements:', document.querySelectorAll('[data-date]').length);
        }, 1000);
    } else {
        console.error('❌ API connection test failed');
        weekCalendarGrid.innerHTML = `
            <div class="calendar-loading">
                <div class="text-danger">
                    <i class="fas fa-exclamation-triangle"></i>
                    <div class="mt-2">Không thể kết nối đến server</div>
                    <div class="mt-1">Vui lòng kiểm tra kết nối mạng và thử lại</div>
                    <button class="btn btn-outline-primary mt-3" onclick="location.reload()">
                        <i class="fas fa-redo"></i> Tải lại trang
                    </button>
                </div>
            </div>
        `;
    }
});

// Export for global access
window.WeekCalendarManager = WeekCalendarManager;

// Global debug function
window.debugCalendar = function() {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.debugAppointments();
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global force render function
window.forceRenderAppointments = function() {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.forceRenderAppointments();
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global test with mock data function
window.testWithMockData = function() {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.testWithMockData();
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global compare data formats function
window.compareDataFormats = function() {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.compareDataFormats();
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global force render with real data function
window.forceRenderWithRealData = function() {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.forceRenderWithRealData();
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global test specific appointment function
window.testSpecificAppointment = function(index = 0) {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.testSpecificAppointment(index);
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global force render everything function
window.forceRenderEverything = function() {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.forceRenderEverything();
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global start DOM monitoring function
window.startDOMMonitoring = function() {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.startDOMMonitoring();
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global force render and keep function
window.forceRenderAndKeep = function() {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.forceRenderAndKeep();
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global test simple render function
window.testSimpleRender = function() {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.testSimpleRender();
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global check page structure function
window.checkPageStructure = function() {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.checkPageStructure();
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global function to check calendar state
window.checkCalendarState = function() {
    console.log('🔍 Checking calendar state...');
    console.log('Calendar elements:', document.querySelectorAll('[data-date]').length);
    console.log('Appointment cards:', document.querySelectorAll('.appointment-card').length);
    console.log('WeekCalendarManager:', !!window.weekCalendarManager);
    if (window.weekCalendarManager) {
        console.log('Appointments count:', window.weekCalendarManager.appointments.length);
        console.log('Current user:', window.weekCalendarManager.currentUser);
    }
};

// Global force render and test function
window.forceRenderAndTest = function() {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.forceRenderAndTest();
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global function to force render with delay
window.forceRenderWithDelay = function() {
    console.log('🔄 Force rendering with delay...');
    setTimeout(() => {
        if (window.weekCalendarManager) {
            window.weekCalendarManager.renderAppointments();
        }
    }, 500);
};

// Global emergency render function
window.emergencyRender = function() {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.emergencyRender();
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global auto-detect and render function
window.autoDetectAndRender = function() {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.autoDetectAndRender();
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
};

// Global update appointment shift function
window.updateAppointmentShift = function(appointmentId, shift) {
    if (window.weekCalendarManager) {
        window.weekCalendarManager.apiService.updateAppointmentShift(appointmentId, shift)
            .then(response => {
                if (response && response.success) {
                    console.log('✅ Ca khám đã được cập nhật:', shift);
                    alert(`Đã cập nhật ca khám thành: ${shift}`);
                } else {
                    console.error('❌ Lỗi cập nhật ca khám:', response?.message);
                    alert('Lỗi cập nhật ca khám: ' + (response?.message || 'Lỗi không xác định'));
                }
            })
            .catch(error => {
                console.error('❌ Lỗi API:', error);
                alert('Lỗi kết nối API');
            });
    } else {
        console.log('❌ WeekCalendarManager not initialized');
    }
}; 