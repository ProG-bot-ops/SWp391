// appointment-filter.js
// Xử lý chức năng filter appointments
(function() {
    'use strict';

    // Các filter options
    const FILTER_OPTIONS = {
        ALL: 'all',
        TODAY: 'today',
        THIS_WEEK: 'this_week',
        THIS_MONTH: 'this_month',
        PENDING: 'pending',
        IN_PROGRESS: 'in_progress',
        COMPLETED: 'completed',
        CANCELLED: 'cancelled'
    };

    // Filter labels
    const FILTER_LABELS = {
        [FILTER_OPTIONS.ALL]: 'Tất cả cuộc hẹn',
        [FILTER_OPTIONS.TODAY]: 'Hôm nay',
        [FILTER_OPTIONS.THIS_WEEK]: 'Tuần này',
        [FILTER_OPTIONS.THIS_MONTH]: 'Tháng này',
        [FILTER_OPTIONS.PENDING]: 'Chờ khám',
        [FILTER_OPTIONS.IN_PROGRESS]: 'Đang khám',
        [FILTER_OPTIONS.COMPLETED]: 'Đã hoàn thành',
        [FILTER_OPTIONS.CANCELLED]: 'Đã hủy'
    };

    // Class AppointmentFilter
    class AppointmentFilter {
        constructor() {
            this.currentFilter = FILTER_OPTIONS.ALL;
            this.init();
        }

        init() {
            this.createFilterDropdown();
            this.setupEventListeners();
        }

        // Tạo dropdown filter
        createFilterDropdown() {
            const filterContainer = document.querySelector('.appointment-filter-container');
            if (!filterContainer) {
                console.warn('❌ Không tìm thấy filter container');
                return;
            }

            // Xóa nội dung cũ
            filterContainer.innerHTML = '';

            // Tạo dropdown
            const dropdownHTML = `
                <div class="dropdown">
                    <button class="btn btn-outline-danger dropdown-toggle" type="button" id="appointmentFilterDropdown" data-bs-toggle="dropdown" aria-expanded="false">
                        <i class="fas fa-filter me-2"></i>
                        Lọc cuộc hẹn
                        <i class="fas fa-chevron-down ms-2"></i>
                    </button>
                    <ul class="dropdown-menu" aria-labelledby="appointmentFilterDropdown">
                        ${Object.entries(FILTER_LABELS).map(([value, label]) => `
                            <li>
                                <a class="dropdown-item ${value === this.currentFilter ? 'active' : ''}" 
                                   href="#" 
                                   data-value="${value}">
                                    ${label}
                                </a>
                            </li>
                        `).join('')}
                    </ul>
                </div>
            `;

            filterContainer.innerHTML = dropdownHTML;
        }

        // Thiết lập event listeners
        setupEventListeners() {
            document.addEventListener('click', (e) => {
                if (e.target.closest('.dropdown-item')) {
                    e.preventDefault();
                    const filterValue = e.target.getAttribute('data-value');
                    if (filterValue) {
                        this.applyFilter(filterValue);
                    }
                }
            });
        }

        // Áp dụng filter
        applyFilter(filterValue) {
            console.log('🔍 Applying filter:', filterValue);
            
            this.currentFilter = filterValue;
            
            // Cập nhật UI
            this.updateFilterUI();
            
            // Thông báo cho AppointmentLoader
            if (window.AppointmentLoader) {
                window.AppointmentLoader.setFilter(filterValue);
            }
            
            // Cập nhật counter
            this.updateCounter();
        }

        // Cập nhật UI filter
        updateFilterUI() {
            const dropdownButton = document.getElementById('appointmentFilterDropdown');
            const dropdownItems = document.querySelectorAll('.dropdown-item');
            
            if (dropdownButton) {
                const currentLabel = FILTER_LABELS[this.currentFilter];
                dropdownButton.innerHTML = `
                    <i class="fas fa-filter me-2"></i>
                    ${currentLabel}
                    <i class="fas fa-chevron-down ms-2"></i>
                `;
            }

            // Cập nhật active state
            dropdownItems.forEach(item => {
                const value = item.getAttribute('data-value');
                if (value === this.currentFilter) {
                    item.classList.add('active');
                } else {
                    item.classList.remove('active');
                }
            });
        }

        // Cập nhật counter
        updateCounter() {
            if (!window.appState || !window.appState.appointments) {
                return;
            }

            const appointments = window.appState.appointments;
            let filteredCount = 0;
            const today = new Date().toISOString().split('T')[0];

            switch (this.currentFilter) {
                case FILTER_OPTIONS.TODAY:
                    filteredCount = appointments.filter(apt => {
                        const aptDate = new Date(apt.date).toISOString().split('T')[0];
                        return aptDate === today;
                    }).length;
                    break;
                    
                case FILTER_OPTIONS.THIS_WEEK:
                    const weekStart = this.getWeekStart();
                    const weekEnd = this.getWeekEnd();
                    filteredCount = appointments.filter(apt => {
                        const aptDate = new Date(apt.date);
                        return aptDate >= weekStart && aptDate <= weekEnd;
                    }).length;
                    break;
                    
                case FILTER_OPTIONS.THIS_MONTH:
                    const monthStart = this.getMonthStart();
                    const monthEnd = this.getMonthEnd();
                    filteredCount = appointments.filter(apt => {
                        const aptDate = new Date(apt.date);
                        return aptDate >= monthStart && aptDate <= monthEnd;
                    }).length;
                    break;
                    
                case FILTER_OPTIONS.PENDING:
                    filteredCount = appointments.filter(apt => 
                        apt.status === 'đã lên lịch' || apt.status === 'Đã lên lịch'
                    ).length;
                    break;
                    
                case FILTER_OPTIONS.IN_PROGRESS:
                    filteredCount = appointments.filter(apt => 
                        apt.status === 'đang khám' || apt.status === 'Đang khám'
                    ).length;
                    break;
                    
                case FILTER_OPTIONS.COMPLETED:
                    filteredCount = appointments.filter(apt => 
                        apt.status === 'đã hoàn thành' || apt.status === 'Đã hoàn thành'
                    ).length;
                    break;
                    
                case FILTER_OPTIONS.CANCELLED:
                    filteredCount = appointments.filter(apt => 
                        apt.status === 'đã hủy' || apt.status === 'Đã hủy'
                    ).length;
                    break;
                    
                default: // ALL
                    filteredCount = appointments.length;
                    break;
            }

            // Cập nhật counter display
            this.updateCounterDisplay(filteredCount);
        }

        // Cập nhật hiển thị counter
        updateCounterDisplay(count) {
            const counterElement = document.querySelector('.appointment-counter');
            if (counterElement) {
                const filterLabel = FILTER_LABELS[this.currentFilter];
                counterElement.textContent = `${count} cuộc hẹn ${filterLabel.toLowerCase()}`;
            }
        }

        // Utility functions cho date
        getWeekStart() {
            const now = new Date();
            const dayOfWeek = now.getDay();
            const diff = now.getDate() - dayOfWeek + (dayOfWeek === 0 ? -6 : 1);
            return new Date(now.setDate(diff));
        }

        getWeekEnd() {
            const weekStart = this.getWeekStart();
            return new Date(weekStart.getTime() + 6 * 24 * 60 * 60 * 1000);
        }

        getMonthStart() {
            const now = new Date();
            return new Date(now.getFullYear(), now.getMonth(), 1);
        }

        getMonthEnd() {
            const now = new Date();
            return new Date(now.getFullYear(), now.getMonth() + 1, 0);
        }

        // Reset filter về mặc định
        resetFilter() {
            this.applyFilter(FILTER_OPTIONS.ALL);
        }

        // Lấy filter hiện tại
        getCurrentFilter() {
            return this.currentFilter;
        }
    }

    // Khởi tạo khi DOM sẵn sàng
    function init() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                console.log('🔍 Initializing AppointmentFilter...');
                window.AppointmentFilter = new AppointmentFilter();
            });
        } else {
            console.log('🔍 DOM ready, initializing AppointmentFilter...');
            window.AppointmentFilter = new AppointmentFilter();
        }
    }

    // Khởi chạy
    init();

    // Export
    window.FILTER_OPTIONS = FILTER_OPTIONS;
    window.FILTER_LABELS = FILTER_LABELS;

    console.log('✅ Appointment Filter đã được khởi tạo');

})(); 