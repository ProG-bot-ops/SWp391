// appointment-counter.js
// Tính toán và hiển thị số cuộc hẹn theo thời gian thực
(function() {
    'use strict';
    
    const API_BASE_URL = 'https://localhost:7097';
    const REFRESH_INTERVAL = 5 * 60 * 1000; // 5 phút
    
    let appointmentCounts = {
        today: 0,
        week: 0,
        month: 0,
        total: 0,
        byStatus: {
            scheduled: 0,
            inprogress: 0,
            completed: 0,
            cancelled: 0,
            late: 0,
            pending: 0
        }
    };

    // Cấu hình status mapping
    const STATUS_CONFIG = {
        scheduled: { text: 'đã lên lịch', class: 'text-primary' },
        inprogress: { text: 'đang khám', class: 'text-warning' },
        completed: { text: 'đã hoàn thành', class: 'text-success' },
        cancelled: { text: 'đã hủy', class: 'text-secondary' },
        late: { text: 'trễ hẹn', class: 'text-danger' },
        pending: { text: 'chờ xử lý', class: 'text-info' }
    };

    // Cấu hình filter mapping
    const FILTER_CONFIG = {
        all: { text: 'Tất cả cuộc hẹn', shortText: 'tổng cộng' },
        today: { text: 'Cuộc hẹn hôm nay', shortText: 'hôm nay' },
        week: { text: 'Cuộc hẹn tuần này', shortText: 'tuần này' },
        month: { text: 'Cuộc hẹn tháng này', shortText: 'tháng này' }
    };

    /**
     * Parse appointment date từ nhiều format khác nhau
     * @param {string|Date} dateInput - Input date
     * @returns {Date|null} - Date object hoặc null nếu không parse được
     */
    function parseAppointmentDate(dateInput) {
        if (!dateInput) return null;
        
        try {
            let appointmentDate = null;
            
            if (typeof dateInput === 'string') {
                // Format YYYY-MM-DD
                if (dateInput.match(/^\d{4}-\d{2}-\d{2}$/)) {
                    appointmentDate = new Date(dateInput + 'T00:00:00');
                }
                // Format YYYY-MM-DDTHH:mm:ss
                else if (dateInput.includes('T')) {
                    appointmentDate = new Date(dateInput);
                }
                // Format DD/MM/YYYY
                else if (dateInput.match(/^\d{2}\/\d{2}\/\d{4}$/)) {
                    const parts = dateInput.split('/');
                    appointmentDate = new Date(parts[2], parts[1] - 1, parts[0]);
                }
                // Format MM/DD/YYYY
                else if (dateInput.match(/^\d{1,2}\/\d{1,2}\/\d{4}$/)) {
                    appointmentDate = new Date(dateInput);
                }
                // Fallback
                else {
                    appointmentDate = new Date(dateInput);
                }
            } else if (dateInput instanceof Date) {
                appointmentDate = dateInput;
            }
            
            // Kiểm tra và sửa lỗi timezone
            if (appointmentDate && !isNaN(appointmentDate.getTime())) {
                // Đảm bảo date được set về 00:00:00 để so sánh chính xác
                appointmentDate.setHours(0, 0, 0, 0);
                return appointmentDate;
            }
            
            return null;
        } catch (error) {
            console.error('Error parsing appointment date:', dateInput, error);
            return null;
        }
    }

    /**
     * Kiểm tra xem appointment có phải hôm nay không
     * @param {Date} appointmentDate - Ngày appointment
     * @returns {boolean} - True nếu là hôm nay
     */
    function isToday(appointmentDate) {
        if (!appointmentDate) return false;
        
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        
        const compareDate = new Date(appointmentDate);
        compareDate.setHours(0, 0, 0, 0);
        
        const isToday = compareDate.getTime() === today.getTime();
        
        console.log('📅 Date comparison:', {
            appointmentDate: compareDate.toISOString().split('T')[0],
            today: today.toISOString().split('T')[0],
            isToday: isToday
        });
        
        return isToday;
    }

    /**
     * Tính toán các khoảng thời gian
     * @returns {Object} - Các khoảng thời gian
     */
    function getTimeRanges() {
        const now = new Date();
        // Đảm bảo today được set về 00:00:00
        const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        
        // Tính tuần bắt đầu từ thứ 2 (0 = Chủ nhật, 1 = Thứ 2)
        const weekStart = new Date(today);
        const dayOfWeek = today.getDay();
        const daysToSubtract = dayOfWeek === 0 ? 6 : dayOfWeek - 1; // Thứ 2 là đầu tuần
        weekStart.setDate(today.getDate() - daysToSubtract);
        
        // Tháng bắt đầu từ ngày 1
        const monthStart = new Date(now.getFullYear(), now.getMonth(), 1);

        console.log('📅 Time ranges calculated:');
        console.log('📅 Today:', today.toISOString().split('T')[0]);
        console.log('📅 Week start:', weekStart.toISOString().split('T')[0]);
        console.log('📅 Month start:', monthStart.toISOString().split('T')[0]);
        
        return {
            today: {
                start: today,
                end: new Date(today.getTime() + 24 * 60 * 60 * 1000)
            },
            week: {
                start: weekStart,
                end: new Date(weekStart.getTime() + 7 * 24 * 60 * 60 * 1000)
            },
            month: {
                start: monthStart,
                end: new Date(monthStart.getFullYear(), monthStart.getMonth() + 1, 1)
            }
        };
    }

    /**
     * Kiểm tra xem appointment có trong khoảng thời gian không
     * @param {Date} appointmentDate - Ngày appointment
     * @param {string} timeRange - Khoảng thời gian ('today', 'week', 'month')
     * @returns {boolean} - True nếu trong khoảng thời gian
     */
    function isInTimeRange(appointmentDate, timeRange) {
        if (!appointmentDate) return false;
        
        // Đặc biệt xử lý cho 'today'
        if (timeRange === 'today') {
            return isToday(appointmentDate);
        }
        
        const ranges = getTimeRanges();
        const range = ranges[timeRange];
        
        if (!range) return false;
        
        // Đảm bảo appointmentDate được set về 00:00:00
        const normalizedAppointmentDate = new Date(appointmentDate);
        normalizedAppointmentDate.setHours(0, 0, 0, 0);
        
        const isInRange = normalizedAppointmentDate >= range.start && normalizedAppointmentDate < range.end;
        
        console.log(`📅 Checking ${timeRange}:`, {
            appointmentDate: normalizedAppointmentDate.toISOString().split('T')[0],
            rangeStart: range.start.toISOString().split('T')[0],
            rangeEnd: range.end.toISOString().split('T')[0],
            isInRange: isInRange
        });
        
        return isInRange;
    }

    /**
     * Tính toán số cuộc hẹn từ danh sách appointments
     * @param {Array} appointments - Danh sách appointments
     * @returns {Object} - Kết quả tính toán
     */
    function calculateAppointmentCounts(appointments) {
        if (!Array.isArray(appointments)) {
            console.warn('Invalid appointments data:', appointments);
            return appointmentCounts;
        }

        console.log('🔄 Calculating appointment counts for', appointments.length, 'appointments');
        
        const ranges = getTimeRanges();
        let todayCount = 0;
        let weekCount = 0;
        let monthCount = 0;
        let totalCount = appointments.length;
        
        let byStatus = {
            scheduled: 0,
            inprogress: 0,
            completed: 0,
            cancelled: 0,
            late: 0,
            pending: 0
        };

        // Danh sách appointments hôm nay để debug
        const todayAppointments = [];

        appointments.forEach((appointment, index) => {
            try {
                // Đếm theo status
                if (appointment.status) {
                    const status = appointment.status.toLowerCase();
                    if (byStatus.hasOwnProperty(status)) {
                        byStatus[status]++;
                    }
                }
                
                // Xử lý ngày tháng
                const appointmentDate = parseAppointmentDate(appointment.date);
                if (appointmentDate) {
                    console.log(`📅 Appointment ${index + 1} (${appointment.patientName}):`, {
                        originalDate: appointment.date,
                        parsedDate: appointmentDate.toISOString().split('T')[0],
                        isToday: isToday(appointmentDate),
                        isWeek: isInTimeRange(appointmentDate, 'week'),
                        isMonth: isInTimeRange(appointmentDate, 'month')
                    });
                    
                    // Kiểm tra hôm nay
                    if (isToday(appointmentDate)) {
                        todayCount++;
                        todayAppointments.push({
                            patientName: appointment.patientName,
                            date: appointment.date,
                            status: appointment.status
                        });
                        console.log(`✅ Counted as today: ${appointment.patientName} (${appointment.date})`);
                    }
                    
                    // Kiểm tra tuần
                    if (isInTimeRange(appointmentDate, 'week')) {
                            weekCount++;
                        }
                        
                    // Kiểm tra tháng
                    if (isInTimeRange(appointmentDate, 'month')) {
                            monthCount++;
                        }
                    } else {
                    console.warn(`⚠️ Could not parse date for appointment ${index + 1}:`, appointment.date);
                }
            } catch (error) {
                console.error('Error processing appointment:', appointment, error);
            }
        });

        const result = {
            today: todayCount,
            week: weekCount,
            month: monthCount,
            total: totalCount,
            byStatus: byStatus
        };

        console.log('📊 Final counts:', result);
        console.log('📋 Today appointments:', todayAppointments);
        
        // Hiển thị thông tin chi tiết về ngày hôm nay
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        console.log('📅 Today\'s date:', today.toISOString().split('T')[0]);
        console.log('📊 Today count should be:', todayCount);
        
        return result;
    }

    /**
     * Lấy text hiển thị cho status
     * @param {string} status - Status code
     * @returns {string} - Text hiển thị
     */
    function getStatusText(status) {
        const config = STATUS_CONFIG[status.toLowerCase()];
        return config ? config.text : status;
    }

    /**
     * Lấy CSS class cho status
     * @param {string} status - Status code
     * @returns {string} - CSS class
     */
    function getStatusClass(status) {
        const config = STATUS_CONFIG[status.toLowerCase()];
        return config ? config.class : 'text-muted';
    }

    /**
     * Cập nhật hiển thị số cuộc hẹn
     * @param {Object} counts - Số liệu cuộc hẹn
     * @param {string} filterType - Loại filter
     * @param {string} statusFilter - Filter theo status
     */
    function updateAppointmentDisplay(counts, filterType = 'today', statusFilter = null) {
        // Tìm element hiển thị số cuộc hẹn - ưu tiên id="appointment-count"
        let countElement = document.getElementById('appointment-count');
        
        // Nếu không tìm thấy, thử tìm bằng data-counter
        if (!countElement) {
            countElement = document.querySelector('[data-counter="today"]');
        }
        
        // Nếu vẫn không tìm thấy, thử tìm bằng class hoặc text content
        if (!countElement) {
            const possibleElements = document.querySelectorAll('p, span, div');
            countElement = Array.from(possibleElements).find(el => 
                el.textContent && el.textContent.includes('cuộc hẹn') && 
                (el.id === 'appointment-count' || el.classList.contains('counter'))
            );
        }
        
        if (!countElement) {
            console.warn('⚠️ Không tìm thấy element hiển thị số cuộc hẹn');
            return;
        }

        // Xác định status filter từ tab hiện tại nếu không được truyền vào
        if (!statusFilter) {
            const activeTab = document.querySelector('.tab-pane.active');
            if (activeTab) {
                const tabId = activeTab.id;
                const tabStatusMap = {
                    'upcoming': 'scheduled',
                    'inprogress': 'inprogress',
                    'request': 'completed',
                    'cancelled': 'cancelled'
                };
                statusFilter = tabStatusMap[tabId];
            }
        }

        // Tính toán số cuộc hẹn theo filter
        let filteredCount = 0;
        const appointments = JSON.parse(localStorage.getItem('allAppointments')) || [];
        
        if (statusFilter) {
            // Lọc theo cả status và thời gian
            filteredCount = appointments.filter(appointment => {
                const statusMatch = appointment.status && 
                    appointment.status.toLowerCase() === statusFilter.toLowerCase();
                if (!statusMatch) return false;
                
                if (filterType === 'all') return true;
                
                const appointmentDate = parseAppointmentDate(appointment.date);
                return appointmentDate && isInTimeRange(appointmentDate, filterType);
            }).length;
        } else {
            // Chỉ lọc theo thời gian
            switch (filterType) {
                case 'today':
                    filteredCount = counts.today;
                    break;
                case 'week':
                    filteredCount = counts.week;
                    break;
                case 'month':
                    filteredCount = counts.month;
                    break;
                case 'all':
                    filteredCount = counts.total;
                    break;
                default:
                    filteredCount = counts.today;
            }
        }

        // Tạo text hiển thị
        let displayText = '';
        if (statusFilter) {
            const statusText = getStatusText(statusFilter);
            const filterConfig = FILTER_CONFIG[filterType] || FILTER_CONFIG.today;
            displayText = `${filteredCount} cuộc hẹn ${statusText} ${filterConfig.shortText}`;
        } else {
            const filterConfig = FILTER_CONFIG[filterType] || FILTER_CONFIG.today;
            displayText = `${filteredCount} cuộc hẹn ${filterConfig.shortText}`;
        }

        // Cập nhật CSS class
        let cssClass = 'mb-0';
        if (filteredCount === 0) {
            cssClass += ' text-muted';
        } else if (statusFilter) {
            cssClass += ' ' + getStatusClass(statusFilter);
        } else if (filteredCount <= 5) {
            cssClass += ' text-success';
        } else if (filteredCount <= 10) {
            cssClass += ' text-warning';
        } else {
            cssClass += ' text-danger';
        }

        // Cập nhật element
        countElement.className = cssClass;
        countElement.textContent = displayText;
        
        console.log('✅ Cập nhật hiển thị số cuộc hẹn:', displayText);
    }

    /**
     * Load dữ liệu cuộc hẹn và tính toán
     */
    function loadAndCalculateAppointments() {
        // Ưu tiên sử dụng dữ liệu từ load-appointments.js
        if (window.appState && window.appState.appointments && window.appState.appointments.length > 0) {
            console.log('✅ Sử dụng dữ liệu từ load-appointments.js');
            const data = window.appState.appointments;
            
            appointmentCounts = calculateAppointmentCounts(data);
            updateAppointmentDisplay(appointmentCounts, 'today');
            localStorage.setItem('appointmentCounts', JSON.stringify(appointmentCounts));
            
            console.log('Appointment counts updated:', appointmentCounts);
            return;
        }
        
        // Gọi API nếu không có dữ liệu từ load-appointments.js
        console.log('⚠️ Không có dữ liệu từ load-appointments.js, gọi API riêng');
        
        fetch(`${API_BASE_URL}/api/appointment/list`)
            .then(response => {
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                return response.json();
            })
            .then(response => {
                console.log('Loading appointments for counting:', response);
                
                let data = response;
                if (response && response.success && response.data) {
                    data = response.data;
                    console.log('✅ API response format mới - lấy data từ response.data');
                } else {
                    console.log('⚠️ API response format cũ - sử dụng response trực tiếp');
                }
                
                // Debug: Kiểm tra format dữ liệu
                if (data && data.length > 0) {
                    console.log('Sample appointment data:', data[0]);
                    console.log('Date field type:', typeof data[0].date);
                    console.log('Date field value:', data[0].date);
                }
                
                appointmentCounts = calculateAppointmentCounts(data);
                updateAppointmentDisplay(appointmentCounts, 'today');
                localStorage.setItem('appointmentCounts', JSON.stringify(appointmentCounts));
                
                console.log('Appointment counts updated:', appointmentCounts);
            })
            .catch(error => {
                console.error('Error loading appointments for counting:', error);
                const countElement = document.getElementById('appointment-count');
                if (countElement) {
                    countElement.textContent = 'Lỗi tải dữ liệu';
                    countElement.className = 'mb-0 text-danger';
                }
            });
    }

    /**
     * Đồng bộ với load-appointments.js
     */
    function syncWithLoadAppointments() {
        // Lắng nghe sự kiện từ load-appointments.js
        window.addEventListener('appointmentsLoaded', function(e) {
            console.log('🔄 Nhận sự kiện appointmentsLoaded từ load-appointments.js');
            if (e.detail && e.detail.appointments) {
                appointmentCounts = calculateAppointmentCounts(e.detail.appointments);
                updateAppointmentDisplay(appointmentCounts, 'today');
                localStorage.setItem('appointmentCounts', JSON.stringify(appointmentCounts));
                console.log('✅ Đã đồng bộ với load-appointments.js');
            }
        });

        // Lắng nghe sự kiện filter thay đổi
        window.addEventListener('appointmentFilterChanged', function(e) {
            console.log('🔄 Nhận sự kiện appointmentFilterChanged:', e.detail);
            updateAppointmentCount(e.detail.filterType);
        });
    }

    /**
     * Khởi tạo counter với delay để đảm bảo DOM đã sẵn sàng
     */
    function initializeCounter() {
        // Thử khởi tạo ngay lập tức
        if (document.readyState === 'loading') {
            // DOM chưa sẵn sàng, đợi
            document.addEventListener('DOMContentLoaded', function() {
                setTimeout(initializeCounter, 100);
            });
            return;
        }

        // Kiểm tra xem element có tồn tại không
        const countElement = document.getElementById('appointment-count');
        if (!countElement) {
            console.log('⚠️ Element appointment-count chưa sẵn sàng, thử lại sau 500ms');
            setTimeout(initializeCounter, 500);
            return;
        }

        console.log('🚀 Khởi tạo Appointment Counter...');
        
        // Đồng bộ với load-appointments.js
        syncWithLoadAppointments();
        
        // Load dữ liệu ban đầu
        loadAndCalculateAppointments();
        
        // Cập nhật định kỳ
        setInterval(loadAndCalculateAppointments, REFRESH_INTERVAL);
        
        // Di chuyển filter vào cạnh tiêu đề
        moveFilterToTitle();
        
        console.log('✅ Appointment Counter đã được khởi tạo thành công');
    }

    /**
     * Filter appointments
     * @param {string} filterType - Loại filter
     */
    window.filterAppointments = function(filterType) {
        console.log('Lọc cuộc hẹn theo:', filterType);
        
        // Lưu filter hiện tại
        localStorage.setItem('currentFilter', filterType);
        
        // Cập nhật số cuộc hẹn hiển thị
        updateAppointmentCount(filterType);
        
        // Cập nhật text của dropdown button
        updateFilterDropdownText(filterType);
        
        // Gọi hàm load appointments từ file load-appointments.js
        if (typeof window.loadAppointments === 'function') {
            console.log('Gọi loadAppointments với filterType:', filterType);
            window.loadAppointments(filterType);
        } else {
            console.warn('Hàm loadAppointments không tồn tại');
            setTimeout(() => {
                if (typeof window.loadAppointments === 'function') {
                    window.loadAppointments(filterType);
                } else {
                    console.error('Không thể tìm thấy hàm loadAppointments');
                }
            }, 100);
        }
        
        // Phát sự kiện
        window.dispatchEvent(new CustomEvent('appointmentFilterChanged', {
            detail: { filterType: filterType }
        }));
    };

    /**
     * Cập nhật text của dropdown filter
     * @param {string} filterType - Loại filter
     */
    function updateFilterDropdownText(filterType) {
        const dropdownButton = document.getElementById('appointmentFilterDropdown');
        if (dropdownButton) {
            const filterConfig = FILTER_CONFIG[filterType] || FILTER_CONFIG.today;
            dropdownButton.innerHTML = `<i class="fas fa-filter me-1"></i>${filterConfig.text}`;
        }
    }

    /**
     * Cập nhật số cuộc hẹn khi filter thay đổi
     * @param {string} filterType - Loại filter
     */
    window.updateAppointmentCount = function(filterType) {
        const counts = JSON.parse(localStorage.getItem('appointmentCounts')) || appointmentCounts;
        updateAppointmentDisplay(counts, filterType);
    };

    /**
     * Refresh số cuộc hẹn
     */
    window.refreshAppointmentCount = function() {
        loadAndCalculateAppointments();
    };

    /**
     * Lấy số cuộc hẹn hiện tại
     * @returns {Object} - Số liệu cuộc hẹn
     */
    window.getAppointmentCounts = function() {
        return JSON.parse(localStorage.getItem('appointmentCounts')) || appointmentCounts;
    };

    /**
     * Test format dữ liệu appointment
     */
    window.testAppointmentDataFormat = function() {
        fetch(`${API_BASE_URL}/api/appointment/list`)
            .then(response => response.json())
            .then(data => {
                console.log('=== TEST APPOINTMENT DATA FORMAT ===');
                console.log('Total appointments:', data.length);
                
                if (data && data.length > 0) {
                    const sample = data[0];
                    console.log('Sample appointment:', sample);
                    console.log('Date field:', sample.date);
                    console.log('Date type:', typeof sample.date);
                    
                    // Test date parsing
                    try {
                        const testDate = parseAppointmentDate(sample.date);
                        console.log('Parsed date:', testDate);
                        console.log('Is valid date:', testDate !== null);
                        if (testDate) {
                        console.log('Date string:', testDate.toISOString());
                        }
                    } catch (error) {
                        console.error('Date parsing error:', error);
                    }
                }
                
                console.log('=== END TEST ===');
            })
            .catch(error => {
                console.error('Test failed:', error);
            });
    };

    /**
     * Debug chi tiết dữ liệu appointments
     */
    window.debugAppointmentData = function() {
        console.log('🔍 === DEBUG APPOINTMENT DATA ===');
        
        // Lấy dữ liệu appointments
        const appointments = window.appState?.appointments || JSON.parse(localStorage.getItem('allAppointments')) || [];
        console.log('📊 Total appointments:', appointments.length);
        
        // Hiển thị thông tin từng appointment
        appointments.forEach((appointment, index) => {
            console.log(`📋 Appointment ${index + 1}:`, {
                id: appointment.id,
                patientName: appointment.patientName,
                date: appointment.date,
                dateType: typeof appointment.date,
                status: appointment.status,
                parsedDate: parseAppointmentDate(appointment.date)
            });
        });
        
        // Tính toán lại counts
        const counts = calculateAppointmentCounts(appointments);
        console.log('📊 Calculated counts:', counts);
        
        // Kiểm tra time ranges
        const ranges = getTimeRanges();
        console.log('📅 Time ranges:', {
            today: {
                start: ranges.today.start.toISOString().split('T')[0],
                end: ranges.today.end.toISOString().split('T')[0]
            },
            week: {
                start: ranges.week.start.toISOString().split('T')[0],
                end: ranges.week.end.toISOString().split('T')[0]
            },
            month: {
                start: ranges.month.start.toISOString().split('T')[0],
                end: ranges.month.end.toISOString().split('T')[0]
            }
        });
        
        // Kiểm tra từng appointment có trong today không
        console.log('🔍 Checking which appointments are today:');
        appointments.forEach((appointment, index) => {
            const parsedDate = parseAppointmentDate(appointment.date);
            const isToday = isInTimeRange(parsedDate, 'today');
            console.log(`📅 Appointment ${index + 1} (${appointment.date}): ${isToday ? '✅ TODAY' : '❌ NOT TODAY'}`);
        });
        
        console.log('🔍 === END DEBUG ===');
    };

    /**
     * Force update counter ngay lập tức
     */
    window.forceUpdateCounter = function() {
        console.log('🚀 Force updating appointment counter...');
        
        // Lấy dữ liệu appointments
        const appointments = window.appState?.appointments || JSON.parse(localStorage.getItem('allAppointments')) || [];
        console.log('📊 Found appointments:', appointments.length);
        
        // Tính toán lại counts
        const counts = calculateAppointmentCounts(appointments);
        
        // Cập nhật hiển thị ngay lập tức
        updateAppointmentDisplay(counts, 'today');
        
        // Lưu vào localStorage
        localStorage.setItem('appointmentCounts', JSON.stringify(counts));
        
        console.log('✅ Counter force updated:', counts);
        
        return counts;
    };

    /**
     * Test function để debug counter
     */
    window.testAppointmentCounter = function() {
        console.log('🧪 Testing Appointment Counter...');
        
        // Test 1: Kiểm tra element
        const countElement = document.getElementById('appointment-count');
        console.log('🔍 Test 1 - Element found:', !!countElement);
        if (countElement) {
            console.log('🔍 Element text:', countElement.textContent);
            console.log('🔍 Element class:', countElement.className);
        }
        
        // Test 2: Kiểm tra appState
        console.log('🔍 Test 2 - appState exists:', !!window.appState);
        if (window.appState) {
            console.log('🔍 appState.appointments:', window.appState.appointments?.length || 0);
        }
        
        // Test 3: Kiểm tra localStorage
        const storedCounts = localStorage.getItem('appointmentCounts');
        console.log('🔍 Test 3 - Stored counts:', storedCounts);
        
        // Test 4: Kiểm tra AppointmentCounter
        console.log('🔍 Test 4 - AppointmentCounter exists:', !!window.AppointmentCounter);
        if (window.AppointmentCounter) {
            console.log('🔍 AppointmentCounter methods:', Object.keys(window.AppointmentCounter));
        }
        
        // Test 5: Debug dữ liệu appointments
        console.log('🔍 Test 5 - Debug appointment data...');
        if (typeof window.debugAppointmentData === 'function') {
            window.debugAppointmentData();
        }
        
        // Test 6: Force update counter
        console.log('🔍 Test 6 - Force updating counter...');
        if (typeof window.forceUpdateCounter === 'function') {
            const result = window.forceUpdateCounter();
            console.log('✅ Force update result:', result);
        }
        
        console.log('🧪 Appointment Counter test completed');
    };

    /**
     * Force refresh counter
     */
    window.forceRefreshCounter = function() {
        console.log('🔄 Force refreshing appointment counter...');
        loadAndCalculateAppointments();
    };

    /**
     * Di chuyển filter vào cạnh tiêu đề
     */
    function moveFilterToTitle() {
        const filterDropdown = document.getElementById('appointmentFilterDropdown');
        const titleElement = document.querySelector('h4');
        
        if (!filterDropdown || !titleElement) {
            console.log('⚠️ Không tìm thấy filter dropdown hoặc tiêu đề');
            return;
        }
        
        const filterContainer = filterDropdown.closest('.dropdown') || filterDropdown;
        const titleContainer = titleElement.closest('.col-md-4.col-lg-6.text-md-start');
        
        if (!titleContainer || !filterContainer) {
            console.log('⚠️ Không tìm thấy container phù hợp');
            return;
        }
        
        // Thêm CSS styles
        addFilterStyles();
        
        // Cập nhật layout
        titleContainer.classList.add('title-filter-container');
        filterContainer.style.cssText = `
            display: inline-block !important;
            vertical-align: middle !important;
            margin-top: 0px !important;
            position: relative !important;
        `;
        
        // Di chuyển filter
        titleContainer.appendChild(filterContainer);
        
        // Khởi tạo dropdown functionality
        initializeDropdownFunctionality(filterContainer, filterDropdown);
        
        console.log('✅ Đã di chuyển filter vào cạnh tiêu đề "Quản lí cuộc hẹn"');
    }

    /**
     * Thêm CSS styles cho filter
     */
    function addFilterStyles() {
        const styleId = 'appointment-filter-styles';
        if (document.getElementById(styleId)) return;
        
        const style = document.createElement('style');
        style.id = styleId;
                style.textContent = `
                    .title-filter-container {
                        display: flex !important;
                        align-items: center !important;
                        gap: 16px !important;
                        flex-wrap: wrap !important;
                    }
                    .title-filter-container h4 {
                        margin: 0 !important;
                        flex-shrink: 0 !important;
                    }
                    .title-filter-container .dropdown {
                        flex-shrink: 0 !important;
                        position: relative !important;
                    }
                    .title-filter-container .dropdown-menu {
                        position: absolute !important;
                        z-index: 9999 !important;
                        display: none !important;
                        min-width: 12rem !important;
                        padding: 0.5rem 0 !important;
                        margin: 0 !important;
                        font-size: 1rem !important;
                        color: #212529 !important;
                        text-align: left !important;
                        list-style: none !important;
                        background-color: #fff !important;
                        background-clip: padding-box !important;
                        border: 1px solid rgba(0, 0, 0, 0.15) !important;
                        border-radius: 0.375rem !important;
                        box-shadow: 0 0.5rem 1rem rgba(0, 0, 0, 0.15) !important;
                        top: 100% !important;
                        left: 0 !important;
                        transform: none !important;
                        flex-direction: column !important;
                    }
                    .title-filter-container .dropdown-menu.show {
                        display: flex !important;
                        flex-direction: column !important;
                    }
                    .title-filter-container .dropdown-item {
                        display: block !important;
                        width: 100% !important;
                        padding: 0.5rem 1rem !important;
                        clear: both !important;
                        font-weight: 400 !important;
                        color: #212529 !important;
                        text-align: left !important;
                        text-decoration: none !important;
                        white-space: nowrap !important;
                        background-color: transparent !important;
                        border: 0 !important;
                        border-bottom: 1px solid #f8f9fa !important;
                    }
                    .title-filter-container .dropdown-item:last-child {
                        border-bottom: none !important;
                    }
                    .title-filter-container .dropdown-item:hover {
                        color: #1e2125 !important;
                        background-color: #e9ecef !important;
                    }
                `;
                document.head.appendChild(style);
    }

    /**
     * Khởi tạo dropdown functionality
     * @param {Element} filterContainer - Container của filter
     * @param {Element} filterDropdown - Dropdown button
     */
    function initializeDropdownFunctionality(filterContainer, filterDropdown) {
        setTimeout(() => {
                    // Xóa data-bs-toggle để tránh xung đột
                    filterDropdown.removeAttribute('data-bs-toggle');
                    
            // Toggle dropdown
                    filterDropdown.addEventListener('click', function(e) {
                        e.preventDefault();
                        e.stopPropagation();
                const dropdownMenu = this.nextElementSibling;
                        if (dropdownMenu && dropdownMenu.classList.contains('dropdown-menu')) {
                            dropdownMenu.classList.toggle('show');
                            console.log('Dropdown clicked, menu visible:', dropdownMenu.classList.contains('show'));
                        }
                    });
                    
                    // Đóng dropdown khi click ra ngoài
                    document.addEventListener('click', function(e) {
                        if (!filterContainer.contains(e.target)) {
                    const dropdownMenu = filterDropdown.nextElementSibling;
                            if (dropdownMenu && dropdownMenu.classList.contains('dropdown-menu')) {
                                dropdownMenu.classList.remove('show');
                            }
                        }
                    });
                    
            // Xử lý dropdown items
            const dropdownItems = filterContainer.querySelectorAll('.dropdown-item');
            dropdownItems.forEach(item => {
                        item.addEventListener('click', function(e) {
                            e.preventDefault();
                    const onclickAttr = this.getAttribute('onclick');
                    if (onclickAttr) {
                        const match = onclickAttr.match(/'([^']+)'/);
                        if (match) {
                            const filterType = match[1];
                            window.filterAppointments(filterType);
                            
                            // Đóng dropdown
                            const dropdownMenu = filterDropdown.nextElementSibling;
                            if (dropdownMenu && dropdownMenu.classList.contains('dropdown-menu')) {
                                dropdownMenu.classList.remove('show');
                            }
                        }
                            }
                        });
                    });
                    
                    console.log('✅ Dropdown menu đã được khởi tạo');
                }, 100);
    }

    // Khởi tạo khi DOM load xong
    initializeCounter();

    // Export cho các file khác sử dụng
    window.AppointmentCounter = {
        updateDisplay: updateAppointmentDisplay,
        calculate: calculateAppointmentCounts,
        load: loadAndCalculateAppointments,
        getCounts: window.getAppointmentCounts,
        parseDate: parseAppointmentDate,
        getStatusText: getStatusText,
        getStatusClass: getStatusClass
    };

})(); 