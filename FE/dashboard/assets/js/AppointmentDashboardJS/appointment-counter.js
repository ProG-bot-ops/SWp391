// appointment-counter.js
// Tính toán và hiển thị số cuộc hẹn theo thời gian thực
(function() {
    const API_BASE_URL = 'https://localhost:7097';
    let appointmentCounts = {
        today: 0,
        week: 0,
        month: 0,
        total: 0
    };

    // Hàm tính toán số cuộc hẹn
    function calculateAppointmentCounts(appointments) {
        const now = new Date();
        const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
        const weekStart = new Date(today);
        weekStart.setDate(today.getDate() - today.getDay());
        const monthStart = new Date(now.getFullYear(), now.getMonth(), 1);

        let todayCount = 0;
        let weekCount = 0;
        let monthCount = 0;
        let totalCount = appointments.length;
        
        // Đếm theo status
        let byStatus = {
            scheduled: 0,
            inprogress: 0,
            completed: 0,
            cancelled: 0,
            late: 0,
            pending: 0
        };

        appointments.forEach(appointment => {
            try {
                let appointmentDate = null;
                
                // Đếm theo status
                if (appointment.status) {
                    const status = appointment.status.toLowerCase();
                    if (byStatus.hasOwnProperty(status)) {
                        byStatus[status]++;
                    }
                }
                
                // Xử lý các format ngày tháng khác nhau
                if (appointment.date) {
                    // Nếu là string, chuyển thành Date object
                    if (typeof appointment.date === 'string') {
                        // Xử lý format YYYY-MM-DD
                        if (appointment.date.match(/^\d{4}-\d{2}-\d{2}$/)) {
                            appointmentDate = new Date(appointment.date + 'T00:00:00');
                        }
                        // Xử lý format YYYY-MM-DDTHH:mm:ss
                        else if (appointment.date.includes('T')) {
                            appointmentDate = new Date(appointment.date);
                        }
                        // Xử lý format DD/MM/YYYY
                        else if (appointment.date.match(/^\d{2}\/\d{2}\/\d{4}$/)) {
                            const parts = appointment.date.split('/');
                            appointmentDate = new Date(parts[2], parts[1] - 1, parts[0]);
                        }
                        // Xử lý format MM/DD/YYYY
                        else if (appointment.date.match(/^\d{1,2}\/\d{1,2}\/\d{4}$/)) {
                            appointmentDate = new Date(appointment.date);
                        }
                        // Fallback: thử parse trực tiếp
                        else {
                            appointmentDate = new Date(appointment.date);
                        }
                    }
                    // Nếu đã là Date object
                    else if (appointment.date instanceof Date) {
                        appointmentDate = appointment.date;
                    }
                    
                    // Kiểm tra xem date có hợp lệ không
                    if (appointmentDate && !isNaN(appointmentDate.getTime())) {
                        // Đếm cuộc hẹn hôm nay
                        if (appointmentDate >= today && appointmentDate < new Date(today.getTime() + 24 * 60 * 60 * 1000)) {
                            todayCount++;
                        }
                        
                        // Đếm cuộc hẹn tuần này
                        if (appointmentDate >= weekStart && appointmentDate < new Date(weekStart.getTime() + 7 * 24 * 60 * 60 * 1000)) {
                            weekCount++;
                        }
                        
                        // Đếm cuộc hẹn tháng này
                        if (appointmentDate >= monthStart && appointmentDate < new Date(monthStart.getFullYear(), monthStart.getMonth() + 1, 1)) {
                            monthCount++;
                        }
                    } else {
                        console.warn('Invalid date format:', appointment.date);
                    }
                }
            } catch (error) {
                console.error('Error processing appointment date:', appointment.date, error);
            }
        });

        return {
            today: todayCount,
            week: weekCount,
            month: monthCount,
            total: totalCount,
            byStatus: byStatus
        };
    }

    // Hàm cập nhật hiển thị số cuộc hẹn
    function updateAppointmentDisplay(counts, filterType = 'today', statusFilter = null) {
        const countElement = document.getElementById('appointment-count');
        if (!countElement) return;

        let displayText = '';
        let count = 0;

        // Xác định status filter từ tab hiện tại
        if (!statusFilter) {
            const activeTab = document.querySelector('.tab-pane.active');
            if (activeTab) {
                const tabId = activeTab.id;
                if (tabId === 'upcoming') statusFilter = 'scheduled';
                else if (tabId === 'inprogress') statusFilter = 'inprogress';
                else if (tabId === 'request') statusFilter = 'completed';
                else if (tabId === 'cancelled') statusFilter = 'cancelled';
            }
        }

        // Tính toán số cuộc hẹn theo filter và status
        let filteredCount = 0;
        
        // Lấy dữ liệu appointments từ localStorage để tính toán chính xác
        const appointments = JSON.parse(localStorage.getItem('allAppointments')) || [];
        
        if (statusFilter) {
            // Có status filter - lọc theo cả status và time
            const filteredAppointments = appointments.filter(appointment => {
                // Lọc theo status
                const statusMatch = appointment.status && appointment.status.toLowerCase() === statusFilter.toLowerCase();
                if (!statusMatch) return false;
                
                // Lọc theo thời gian
                if (filterType === 'all') return true;
                
                const appointmentDate = parseAppointmentDate(appointment.date);
                if (!appointmentDate) return false;
                
                const now = new Date();
                const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                const weekStart = new Date(today);
                weekStart.setDate(today.getDate() - today.getDay());
                const monthStart = new Date(now.getFullYear(), now.getMonth(), 1);
                
                switch (filterType) {
                    case 'today':
                        return appointmentDate >= today && appointmentDate < new Date(today.getTime() + 24 * 60 * 60 * 1000);
                    case 'week':
                        return appointmentDate >= weekStart && appointmentDate < new Date(weekStart.getTime() + 7 * 24 * 60 * 60 * 1000);
                    case 'month':
                        return appointmentDate >= monthStart && appointmentDate < new Date(monthStart.getFullYear(), monthStart.getMonth() + 1, 1);
                    default:
                        return true;
                }
            });
            
            filteredCount = filteredAppointments.length;
        } else {
            // Không có status filter, sử dụng filter thời gian
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

        count = filteredCount;

        // Tạo text hiển thị
        if (statusFilter) {
            const statusText = getStatusText(statusFilter);
            // Khi có status filter, hiển thị kết hợp status và time filter
            switch (filterType) {
                case 'today':
                    displayText = `${count} cuộc hẹn ${statusText} hôm nay`;
                    break;
                case 'week':
                    displayText = `${count} cuộc hẹn ${statusText} tuần này`;
                    break;
                case 'month':
                    displayText = `${count} cuộc hẹn ${statusText} tháng này`;
                    break;
                case 'all':
                    displayText = `${count} cuộc hẹn ${statusText}`;
                    break;
                default:
                    displayText = `${count} cuộc hẹn ${statusText}`;
            }
        } else {
            // Khi không có status filter, hiển thị theo time filter
            switch (filterType) {
                case 'today':
                    displayText = `${count} cuộc hẹn hôm nay`;
                    break;
                case 'week':
                    displayText = `${count} cuộc hẹn tuần này`;
                    break;
                case 'month':
                    displayText = `${count} cuộc hẹn tháng này`;
                    break;
                case 'all':
                    displayText = `${count} cuộc hẹn tổng cộng`;
                    break;
                default:
                    displayText = `${count} cuộc hẹn hôm nay`;
            }
        }

        // Thêm màu sắc dựa trên số lượng và status
        if (count === 0) {
            countElement.className = 'mb-0 text-muted';
        } else if (statusFilter === 'inprogress') {
            countElement.className = 'mb-0 text-warning';
        } else if (statusFilter === 'completed') {
            countElement.className = 'mb-0 text-success';
        } else if (statusFilter === 'cancelled') {
            countElement.className = 'mb-0 text-secondary';
        } else if (count <= 5) {
            countElement.className = 'mb-0 text-success';
        } else if (count <= 10) {
            countElement.className = 'mb-0 text-warning';
        } else {
            countElement.className = 'mb-0 text-danger';
        }

        countElement.textContent = displayText;
    }

    // Hàm chuyển đổi status sang tiếng Việt
    function getStatusText(status) {
        switch (status.toLowerCase()) {
            case 'scheduled':
                return 'đã lên lịch';
            case 'inprogress':
                return 'đang khám';
            case 'completed':
                return 'đã hoàn thành';
            case 'cancelled':
                return 'đã hủy';
            default:
                return status;
        }
    }
    
    // Hàm parse appointment date
    function parseAppointmentDate(dateString) {
        if (!dateString) return null;
        
        try {
            let appointmentDate = null;
            
            // Nếu là string, chuyển thành Date object
            if (typeof dateString === 'string') {
                // Xử lý format YYYY-MM-DD
                if (dateString.match(/^\d{4}-\d{2}-\d{2}$/)) {
                    appointmentDate = new Date(dateString + 'T00:00:00');
                }
                // Xử lý format YYYY-MM-DDTHH:mm:ss
                else if (dateString.includes('T')) {
                    appointmentDate = new Date(dateString);
                }
                // Xử lý format DD/MM/YYYY
                else if (dateString.match(/^\d{2}\/\d{2}\/\d{4}$/)) {
                    const parts = dateString.split('/');
                    appointmentDate = new Date(parts[2], parts[1] - 1, parts[0]);
                }
                // Xử lý format MM/DD/YYYY
                else if (dateString.match(/^\d{1,2}\/\d{1,2}\/\d{4}$/)) {
                    appointmentDate = new Date(dateString);
                }
                // Fallback: thử parse trực tiếp
                else {
                    appointmentDate = new Date(dateString);
                }
            }
            // Nếu đã là Date object
            else if (dateString instanceof Date) {
                appointmentDate = dateString;
            }
            
            // Kiểm tra xem date có hợp lệ không
            if (appointmentDate && !isNaN(appointmentDate.getTime())) {
                return appointmentDate;
            }
            
            return null;
        } catch (error) {
            console.error('Error parsing appointment date:', dateString, error);
            return null;
        }
    }

    // Hàm load dữ liệu cuộc hẹn và tính toán
    function loadAndCalculateAppointments() {
        // Kiểm tra xem có dữ liệu từ load-appointments.js không
        if (window.appState && window.appState.appointments && window.appState.appointments.length > 0) {
            console.log('✅ Sử dụng dữ liệu từ load-appointments.js');
            const data = window.appState.appointments;
            
            // Tính toán số cuộc hẹn
            appointmentCounts = calculateAppointmentCounts(data);
            
            // Cập nhật hiển thị mặc định (hôm nay)
            updateAppointmentDisplay(appointmentCounts, 'today');
            
            // Lưu vào localStorage để sử dụng ở các file khác
            localStorage.setItem('appointmentCounts', JSON.stringify(appointmentCounts));
            
            console.log('Appointment counts updated:', appointmentCounts);
            return;
        }
        
        // Nếu không có dữ liệu từ load-appointments.js, gọi API riêng
        console.log('⚠️ Không có dữ liệu từ load-appointments.js, gọi API riêng');
        fetch(`${API_BASE_URL}/api/appointment/list`)
            .then(res => {
                if (!res.ok) throw new Error('API error: ' + res.status);
                return res.json();
            })
            .then(response => {
                console.log('Loading appointments for counting:', response);
                
                // Xử lý response format mới
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
                
                // Tính toán số cuộc hẹn
                appointmentCounts = calculateAppointmentCounts(data);
                
                // Cập nhật hiển thị mặc định (hôm nay)
                updateAppointmentDisplay(appointmentCounts, 'today');
                
                // Lưu vào localStorage để sử dụng ở các file khác
                localStorage.setItem('appointmentCounts', JSON.stringify(appointmentCounts));
                
                console.log('Appointment counts updated:', appointmentCounts);
            })
            .catch(err => {
                console.error('Error loading appointments for counting:', err);
                const countElement = document.getElementById('appointment-count');
                if (countElement) {
                    countElement.textContent = 'Lỗi tải dữ liệu';
                    countElement.className = 'mb-0 text-danger';
                }
            });
    }

    // Hàm filter appointments - tích hợp với hệ thống đếm
    window.filterAppointments = function(filterType) {
        console.log('Lọc cuộc hẹn theo:', filterType);
        
        // Lưu filter hiện tại
        localStorage.setItem('currentFilter', filterType);
        
        // Cập nhật số cuộc hẹn hiển thị
        updateAppointmentCount(filterType);
        
        // Cập nhật text của dropdown button
        const dropdownButton = document.getElementById('appointmentFilterDropdown');
        if (dropdownButton) {
            let filterText = '';
            switch (filterType) {
                case 'all':
                    filterText = 'Tất cả cuộc hẹn';
                    break;
                case 'today':
                    filterText = 'Cuộc hẹn hôm nay';
                    break;
                case 'week':
                    filterText = 'Cuộc hẹn tuần này';
                    break;
                case 'month':
                    filterText = 'Cuộc hẹn tháng này';
                    break;
                default:
                    filterText = 'Lọc cuộc hẹn';
            }
            dropdownButton.innerHTML = `<i class="fas fa-filter me-1"></i>${filterText}`;
        }
        
        // Gọi hàm load appointments từ file load-appointments.js nếu có
        if (typeof window.loadAppointments === 'function') {
            console.log('Gọi loadAppointments với filterType:', filterType);
            window.loadAppointments(filterType);
        } else {
            console.warn('Hàm loadAppointments không tồn tại, thử gọi trực tiếp');
            // Thử gọi trực tiếp nếu hàm không tồn tại
            setTimeout(() => {
                if (typeof window.loadAppointments === 'function') {
                    window.loadAppointments(filterType);
                } else {
                    console.error('Không thể tìm thấy hàm loadAppointments');
                }
            }, 100);
        }
        
        // Phát sự kiện để các file khác có thể lắng nghe
        window.dispatchEvent(new CustomEvent('appointmentFilterChanged', {
            detail: { filterType: filterType }
        }));
    };

    // Hàm cập nhật số cuộc hẹn khi filter thay đổi
    window.updateAppointmentCount = function(filterType) {
        const counts = JSON.parse(localStorage.getItem('appointmentCounts')) || appointmentCounts;
        updateAppointmentDisplay(counts, filterType);
    };

    // Hàm refresh số cuộc hẹn
    window.refreshAppointmentCount = function() {
        loadAndCalculateAppointments();
    };

    // Hàm lấy số cuộc hẹn hiện tại
    window.getAppointmentCounts = function() {
        return JSON.parse(localStorage.getItem('appointmentCounts')) || appointmentCounts;
    };

    // Hàm test format dữ liệu
    window.testAppointmentDataFormat = function() {
        fetch(`${API_BASE_URL}/api/appointment/list`)
            .then(res => res.json())
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
                        const testDate = new Date(sample.date);
                        console.log('Parsed date:', testDate);
                        console.log('Is valid date:', !isNaN(testDate.getTime()));
                        console.log('Date string:', testDate.toISOString());
                    } catch (error) {
                        console.error('Date parsing error:', error);
                    }
                }
                
                console.log('=== END TEST ===');
            })
            .catch(err => {
                console.error('Test failed:', err);
            });
    };

    // Hàm di chuyển filter vào cạnh tiêu đề "Quản lí cuộc hẹn"
    function moveFilterToTitle() {
        // Tìm phần tử filter và tiêu đề "Quản lí cuộc hẹn"
        var filterDropdown = document.getElementById('appointmentFilterDropdown');
        var titleElement = document.querySelector('h4');
        
        if (filterDropdown && titleElement) {
            // Tìm phần tử cha của filter (nếu là .dropdown thì lấy .dropdown)
            var filterContainer = filterDropdown.closest('.dropdown') || filterDropdown;
            
            // Tìm container chứa tiêu đề
            var titleContainer = titleElement.closest('.col-md-4.col-lg-6.text-md-start');
            
            // Di chuyển filter vào cạnh tiêu đề
            if (titleContainer && filterContainer) {
                // Thêm CSS vào head để đảm bảo layout đẹp
                var style = document.createElement('style');
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
                
                // Thêm class cho container
                titleContainer.classList.add('title-filter-container');
                
                // Thêm style cho filter
                filterContainer.style.display = 'inline-block';
                filterContainer.style.verticalAlign = 'middle';
                filterContainer.style.marginTop = '0px';
                filterContainer.style.position = 'relative';
                
                // Di chuyển filter vào cạnh tiêu đề
                titleContainer.appendChild(filterContainer);
                
                // Đảm bảo dropdown menu vẫn hoạt động
                setTimeout(function() {
                    // Xóa data-bs-toggle để tránh xung đột
                    filterDropdown.removeAttribute('data-bs-toggle');
                    
                    // Thêm event listener để đảm bảo dropdown hoạt động
                    filterDropdown.addEventListener('click', function(e) {
                        e.preventDefault();
                        e.stopPropagation();
                        var dropdownMenu = this.nextElementSibling;
                        if (dropdownMenu && dropdownMenu.classList.contains('dropdown-menu')) {
                            dropdownMenu.classList.toggle('show');
                            console.log('Dropdown clicked, menu visible:', dropdownMenu.classList.contains('show'));
                        }
                    });
                    
                    // Đóng dropdown khi click ra ngoài
                    document.addEventListener('click', function(e) {
                        if (!filterContainer.contains(e.target)) {
                            var dropdownMenu = filterDropdown.nextElementSibling;
                            if (dropdownMenu && dropdownMenu.classList.contains('dropdown-menu')) {
                                dropdownMenu.classList.remove('show');
                            }
                        }
                    });
                    
                    // Đảm bảo dropdown menu items hoạt động
                    var dropdownItems = filterContainer.querySelectorAll('.dropdown-item');
                    dropdownItems.forEach(function(item) {
                        item.addEventListener('click', function(e) {
                            e.preventDefault();
                            var filterType = this.getAttribute('onclick').match(/'([^']+)'/)[1];
                            window.filterAppointments(filterType);
                            
                            // Đóng dropdown sau khi chọn
                            var dropdownMenu = filterDropdown.nextElementSibling;
                            if (dropdownMenu && dropdownMenu.classList.contains('dropdown-menu')) {
                                dropdownMenu.classList.remove('show');
                            }
                        });
                    });
                    
                    console.log('✅ Dropdown menu đã được khởi tạo');
                }, 100);
                
                console.log('✅ Đã di chuyển filter vào cạnh tiêu đề "Quản lí cuộc hẹn"');
            }
        } else {
            console.log('⚠️ Không tìm thấy filter dropdown hoặc tiêu đề');
        }
    }

    // Khởi tạo khi DOM load xong
    document.addEventListener('DOMContentLoaded', function() {
        console.log('🚀 Khởi tạo Appointment Counter...');
        
        // Load dữ liệu ban đầu
        loadAndCalculateAppointments();
        
        // Cập nhật mỗi 5 phút
        setInterval(loadAndCalculateAppointments, 5 * 60 * 1000);
        
        // Lắng nghe sự kiện filter từ các file khác
        window.addEventListener('appointmentFilterChanged', function(e) {
            updateAppointmentCount(e.detail.filterType);
        });
        
        // Di chuyển phần lọc cuộc hẹn vào cạnh tiêu đề
        moveFilterToTitle();
    });

    // Export cho các file khác sử dụng
    window.AppointmentCounter = {
        updateDisplay: updateAppointmentDisplay,
        calculate: calculateAppointmentCounts,
        load: loadAndCalculateAppointments,
        getCounts: window.getAppointmentCounts
    };

})(); 