// Time Slot Integration for Appointment Page
class TimeSlotIntegration {
    constructor() {
        this.apiBaseUrl = 'https://localhost:7097';
        this.bookedTimeSlots = [];
        this.selectedDoctor = null;
        this.selectedDate = null;
        this.autoRefreshInterval = null;
        this.init();
    }

    init() {
        this.setupEventListeners();
        this.loadSelectedData();
        this.startAutoRefresh();
        
        // Test: Kiểm tra ngay khi init
        console.log('[TimeSlotIntegration] Initialized');
        console.log('[TimeSlotIntegration] Current time:', new Date().toLocaleString());
        
        // Test: Kiểm tra xem có phải ngày hôm nay không
        const today = new Date();
        const currentHour = today.getHours();
        console.log('[TimeSlotIntegration] Current hour:', currentHour);
        console.log('[TimeSlotIntegration] Morning shift should be locked after 12:00:', currentHour >= 12);
        console.log('[TimeSlotIntegration] Afternoon shift should be locked after 17:00:', currentHour >= 17);
    }

    setupEventListeners() {
        // Debounce function để tránh gọi API quá nhiều
        let updateTimeout = null;
        const debouncedUpdate = () => {
            if (updateTimeout) clearTimeout(updateTimeout);
            updateTimeout = setTimeout(() => {
                if (this.selectedDoctor && this.selectedDate) {
                    console.log('[TimeSlotIntegration] Debounced update triggered');
                    this.updateTimeSlots();
                }
            }, 300); // Delay 300ms
        };

        // Listen for date selection changes
        window.addEventListener('customCalendarDateSelected', (event) => {
            if (event.detail && event.detail.date) {
                this.selectedDate = event.detail.date;
                
                // Lưu lại ngày vào sessionStorage
                if (this.selectedDate instanceof Date && !isNaN(this.selectedDate)) {
                    const d = this.selectedDate;
                    const dateStr = `${d.getDate().toString().padStart(2,0)}/${(d.getMonth()+1).toString().padStart(2,0)}/${d.getFullYear()}`;
                    sessionStorage.setItem('selectedDate', dateStr);
                }
                
                // Sử dụng debounced update
                if (this.selectedDoctor) {
                    debouncedUpdate();
                } else {
                    this.resetTimeSlots();
                }
            }
        });

        // Direct listener for doctor radio button changes (ưu tiên hơn storage event)
        document.addEventListener('change', (event) => {
            if (event.target.name === 'doctorRadios' && event.target.checked) {
                const doctorId = parseInt(event.target.value);
                const doctorName = event.target.dataset.name || 'Unknown Doctor';
                const doctor = { id: doctorId, name: doctorName };
                this.selectedDoctor = doctor;
                
                // Lưu vào sessionStorage
                sessionStorage.setItem('selectedDoctor', JSON.stringify(doctor));
                
                // Sử dụng debounced update
                if (this.selectedDate) {
                    debouncedUpdate();
                } else {
                    this.resetTimeSlots();
                }
            }
        });

        // Listen for time slot clicks (tối ưu hóa)
        document.addEventListener('click', (event) => {
            if (event.target.closest('.time-btn.shift-btn')) {
                const button = event.target.closest('.time-btn.shift-btn');
                this.handleTimeSlotClick(button);
            }
        });
    }

    loadSelectedData() {
        // Load selected doctor from session storage
        const doctorData = sessionStorage.getItem('selectedDoctor');
        if (doctorData) {
            try {
                this.selectedDoctor = JSON.parse(doctorData);
            } catch (e) {
                console.error('Error parsing doctor data:', e);
            }
        }

        // Kiểm tra xem có ca nào đã được chọn nhưng quá giờ không
        const selectedShift = sessionStorage.getItem('selectedShift');
        if (selectedShift && this.selectedDoctor) {
            // Kiểm tra xem có phải ngày hôm nay không
            const today = new Date();
            const currentTime = today.getTime();
            const currentHour = today.getHours();
            
            let shouldRemoveShift = false;
            
            if (selectedShift === 'morning' && currentHour >= 12) {
                shouldRemoveShift = true;
                console.log('[loadSelectedData] Morning shift expired, removing selection');
            } else if (selectedShift === 'afternoon' && currentHour >= 17) {
                shouldRemoveShift = true;
                console.log('[loadSelectedData] Afternoon shift expired, removing selection');
            }
            
            if (shouldRemoveShift) {
                sessionStorage.removeItem('selectedShift');
                sessionStorage.removeItem('selectedTime');
                console.log('[loadSelectedData] Removed expired shift from sessionStorage');
            }
        }

        // Nếu có doctor và date, gọi API ngay để kiểm tra availability
        const selectedDate = sessionStorage.getItem('selectedDate');
        if (this.selectedDoctor && selectedDate) {
            // Parse date từ DD/MM/YYYY
            const [day, month, year] = selectedDate.split('/');
            this.selectedDate = new Date(parseInt(year), parseInt(month) - 1, parseInt(day));
            
            // Kiểm tra xem có phải ngày hôm nay không
            const today = new Date();
            const isToday = today.toDateString() === this.selectedDate.toDateString();
            console.log('[loadSelectedData] Is today:', isToday);
            
                    // Gọi API ngay để kiểm tra
        setTimeout(() => {
            this.updateTimeSlots();
        }, 100);
        
        // Cập nhật UI ngay lập tức nếu có ca đã quá giờ
        setTimeout(() => {
            this.updateTimeSlotDisplay();
        }, 50);
        }
        
        // Force check nếu đang xem ngày hôm nay
        const today = new Date();
        const currentHour = today.getHours();
        if (currentHour >= 12) {
            console.log('[loadSelectedData] Current time is after 12:00, morning shift should be locked');
            // Bỏ chọn ca sáng nếu đã quá giờ
            if (sessionStorage.getItem('selectedShift') === 'morning') {
                sessionStorage.removeItem('selectedShift');
                sessionStorage.removeItem('selectedTime');
                console.log('[loadSelectedData] Removed morning shift selection (expired)');
            }
        }
        if (currentHour >= 17) {
            console.log('[loadSelectedData] Current time is after 17:00, afternoon shift should be locked');
            // Bỏ chọn ca chiều nếu đã quá giờ
            if (sessionStorage.getItem('selectedShift') === 'afternoon') {
                sessionStorage.removeItem('selectedShift');
                sessionStorage.removeItem('selectedTime');
                console.log('[loadSelectedData] Removed afternoon shift selection (expired)');
            }
        }

        // KHÔNG tự động load ngày từ sessionStorage khi khởi tạo
        // Chỉ load ngày khi người dùng chọn ngày trên calendar
        this.selectedDate = null;
        console.log('No date auto-loaded - waiting for user selection');

        // Reset time slots to disabled state when no date selected
        this.resetTimeSlots();
    }

    async updateTimeSlots() {
        console.log('[updateTimeSlots] Called', this.selectedDoctor, this.selectedDate);
        if (!this.selectedDoctor || !this.selectedDate) {
            console.log('[updateTimeSlots] Missing doctor or date selection');
            this.showErrorState();
            return;
        }

        try {
            this.showLoadingState();

            // Format date for API - ensure yyyy-MM-dd format
            const year = this.selectedDate.getFullYear();
            const month = String(this.selectedDate.getMonth() + 1).padStart(2, '0');
            const day = String(this.selectedDate.getDate()).padStart(2, '0');
            const dateStr = `${year}-${month}-${day}`;

            // Tạo URL với timeout
            const url = `${this.apiBaseUrl}/api/Appointment/available-time-slots?doctorId=${this.selectedDoctor.id}&date=${dateStr}`;
            console.log('[updateTimeSlots] API URL:', url);
            console.log('[updateTimeSlots] Current time:', new Date().toLocaleString());

            // Sử dụng AbortController để có thể cancel request
            const controller = new AbortController();
            const timeoutId = setTimeout(() => controller.abort(), 10000); // 10 giây timeout

            const response = await fetch(url, {
                method: 'GET',
                headers: { 'Content-Type': 'application/json' },
                signal: controller.signal
            });

            clearTimeout(timeoutId);

            if (!response.ok) {
                console.error('[updateTimeSlots] HTTP error! status:', response.status);
                this.showErrorState();
                return;
            }

            const result = await response.json();
            console.log('[updateTimeSlots] API Response:', result);

            if (result.success) {
                this.availableTimeSlots = result.data || {};
                console.log('[updateTimeSlots] Available slots data:', this.availableTimeSlots);
                this.updateTimeSlotDisplay();
            } else {
                console.error('[updateTimeSlots] API returned success=false:', result.message);
                this.showErrorState();
            }
        } catch (error) {
            if (error.name === 'AbortError') {
                console.error('[updateTimeSlots] Request timeout');
                this.showErrorState();
            } else {
            console.error('[updateTimeSlots] Exception:', error);
            this.showErrorState();
            }
        }
    }

    showLoadingState() {
        const timeButtons = document.querySelectorAll('.time-btn.shift-btn');
        timeButtons.forEach(btn => {
            btn.disabled = true;
            btn.style.opacity = '0.7';
            btn.style.cursor = 'wait';
            btn.title = 'Đang kiểm tra khả dụng...';
            
            // Tìm và cập nhật nội dung bên trong button mà không xóa cấu trúc HTML
            const textDiv = btn.querySelector('.fw-bold.fs-6');
            if (textDiv) {
                const shift = btn.getAttribute('data-shift');
                if (shift === 'morning') {
                    textDiv.textContent = 'CA SÁNG (07:00 - 12:00)';
                } else if (shift === 'afternoon') {
                    textDiv.textContent = 'CA CHIỀU (13:00 - 17:00)';
                }
            }
            
            // Cập nhật slot count thành "Đang kiểm tra..."
            const slotCountDiv = btn.querySelector('.slot-count');
            if (slotCountDiv) {
                slotCountDiv.textContent = 'Đang kiểm tra...';
                slotCountDiv.style.color = '#6c757d';
            }
        });
    }

    showErrorState() {
        const timeButtons = document.querySelectorAll('.time-btn.shift-btn');
        timeButtons.forEach(btn => {
            btn.disabled = true;
            btn.style.opacity = '0.5';
            btn.style.cursor = 'not-allowed';
            btn.style.backgroundColor = '#fef2f2';
            btn.style.color = '#ef4444';
            btn.title = 'Có lỗi xảy ra, vui lòng thử lại';
            
            // Tìm và cập nhật nội dung bên trong button mà không xóa cấu trúc HTML
            const textDiv = btn.querySelector('.fw-bold.fs-6');
            if (textDiv) {
                const shift = btn.getAttribute('data-shift');
                if (shift === 'morning') {
                    textDiv.textContent = 'CA SÁNG (07:00 - 12:00)';
                } else if (shift === 'afternoon') {
                    textDiv.textContent = 'CA CHIỀU (13:00 - 17:00)';
                }
            }
            
            // Cập nhật slot count thành "Lỗi"
            const slotCountDiv = btn.querySelector('.slot-count');
            if (slotCountDiv) {
                slotCountDiv.textContent = 'Lỗi';
                slotCountDiv.style.color = '#dc3545';
            }
        });
    }

    updateTimeSlotDisplay() {
        // Cache DOM elements để tăng performance
        const timeButtons = document.querySelectorAll('.time-btn.shift-btn');
        if (!timeButtons || timeButtons.length < 2) return;

        console.log('[updateTimeSlotDisplay] Available slots:', this.availableTimeSlots);

        // Batch DOM updates để tránh reflow
        const updates = [];

        timeButtons.forEach(btn => {
            const shift = btn.getAttribute('data-shift');
            const textDiv = btn.querySelector('.fw-bold.fs-6');
            
            // Khôi phục nội dung gốc
            if (textDiv) {
                if (shift === 'morning') {
                    textDiv.textContent = 'CA SÁNG (07:00 - 12:00)';
                } else if (shift === 'afternoon') {
                    textDiv.textContent = 'CA CHIỀU (13:00 - 17:00)';
                }
            }

            // Cập nhật số lượng slot đã đặt
            const slotCountDiv = btn.querySelector('.slot-count');
            if (slotCountDiv) {
                let count = 0;
                if (this.availableTimeSlots) {
                    if (shift === 'morning' && this.availableTimeSlots.morning) {
                        count = this.availableTimeSlots.morning.count || 0;
                    } else if (shift === 'afternoon' && this.availableTimeSlots.afternoon) {
                        count = this.availableTimeSlots.afternoon.count || 0;
                    }
                }
                // Lấy số slot tối đa từ API một cách linh hoạt
                let maxSlots = 10; // Giá trị mặc định
                if (this.availableTimeSlots) {
                    if (shift === 'morning' && this.availableTimeSlots.morning) {
                        // Nếu API trả về maxSlots, sử dụng nó
                        maxSlots = this.availableTimeSlots.morning.maxSlots || 10;
                    } else if (shift === 'afternoon' && this.availableTimeSlots.afternoon) {
                        // Nếu API trả về maxSlots, sử dụng nó
                        maxSlots = this.availableTimeSlots.afternoon.maxSlots || 10;
                    }
                }
                slotCountDiv.textContent = `Đã đặt: ${count}/${maxSlots}`;
                
                // Thay đổi màu sắc dựa trên số lượng (linh hoạt theo maxSlots)
                const threshold80 = Math.ceil(maxSlots * 0.8); // 80% của maxSlots
                const threshold60 = Math.ceil(maxSlots * 0.6); // 60% của maxSlots
                
                if (count >= threshold80) {
                    slotCountDiv.style.color = '#dc3545'; // Đỏ khi gần đầy (>=80%)
                } else if (count >= threshold60) {
                    slotCountDiv.style.color = '#ffc107'; // Vàng khi trung bình (60-79%)
                } else {
                    slotCountDiv.style.color = '#6c757d'; // Xám khi còn nhiều (<60%)
                }
            }

            // Xác định trạng thái available và lý do
            let isAvailable = true;
            let reason = '';
            if (this.availableTimeSlots) {
                if (shift === 'morning' && this.availableTimeSlots.morning) {
                    isAvailable = this.availableTimeSlots.morning.available === true;
                    console.log('[updateTimeSlotDisplay] Morning shift:', {
                        available: isAvailable,
                        isPastTime: this.availableTimeSlots.morning.isPastTime,
                        doctorWorks: this.availableTimeSlots.morning.doctorWorks
                    });
                    if (!isAvailable) {
                        if (this.availableTimeSlots.morning.isPastTime) {
                            reason = 'Ca sáng đã kết thúc (sau 12:00)';
                        } else if (!this.availableTimeSlots.morning.doctorWorks) {
                            reason = 'Bác sĩ không làm việc ca sáng';
                        } else {
                            reason = 'Ca sáng đã đầy';
                        }
                    }
                } else if (shift === 'afternoon' && this.availableTimeSlots.afternoon) {
                    isAvailable = this.availableTimeSlots.afternoon.available === true;
                    console.log('[updateTimeSlotDisplay] Afternoon shift:', {
                        available: isAvailable,
                        isPastTime: this.availableTimeSlots.afternoon.isPastTime,
                        doctorWorks: this.availableTimeSlots.afternoon.doctorWorks
                    });
                    if (!isAvailable) {
                        if (this.availableTimeSlots.afternoon.isPastTime) {
                            reason = 'Ca chiều đã kết thúc (sau 17:00)';
                        } else if (!this.availableTimeSlots.afternoon.doctorWorks) {
                            reason = 'Bác sĩ không làm việc ca chiều';
                } else {
                            reason = 'Ca chiều đã đầy';
                        }
                    }
                }
            }

            // Chuẩn bị updates
            updates.push({
                button: btn,
                available: isAvailable,
                reason: reason
            });
        });

        // Apply tất cả updates cùng lúc
        requestAnimationFrame(() => {
            updates.forEach(({ button, available, reason }) => {
                button.disabled = !available;
                button.classList.toggle('disabled', !available);
                button.style.opacity = available ? '1' : '0.5';
                button.style.backgroundColor = available ? '#f8f9fa' : '#f3f4f6';
                button.style.color = available ? '#212529' : '#6b7280';
                button.style.cursor = available ? 'pointer' : 'not-allowed';
                button.title = available ? 'Click để chọn ca này' : reason || 'Ca này không khả dụng';
                
                // Nếu ca không available và đang được chọn, bỏ chọn
                if (!available && button.classList.contains('selected')) {
                    button.classList.remove('selected', 'active');
                    button.style.backgroundColor = '#f3f4f6';
                    button.style.color = '#6b7280';
                    console.log('[updateTimeSlotDisplay] Auto-deselected unavailable shift:', button.getAttribute('data-shift'));
                    
                    // Xóa khỏi sessionStorage
                    const shift = button.getAttribute('data-shift');
                    if (shift === sessionStorage.getItem('selectedShift')) {
                        sessionStorage.removeItem('selectedShift');
                        sessionStorage.removeItem('selectedTime');
                        console.log('[updateTimeSlotDisplay] Removed selected shift from sessionStorage');
                    }
                }
                
                console.log('[updateTimeSlotDisplay] Button update:', {
                    shift: button.getAttribute('data-shift'),
                    available: available,
                    reason: reason,
                    disabled: button.disabled,
                    selected: button.classList.contains('selected')
                });
            });
        });
    }

    handleTimeSlotClick(button) {
        if (button.disabled || button.classList.contains('booked')) {
            console.log('[handleTimeSlotClick] Button is disabled or booked, ignoring click');
            return; // Don't allow selection of booked slots
        }

        // Kiểm tra thời gian hiện tại
        const shift = button.getAttribute('data-shift');
        const currentHour = new Date().getHours();
        
        if (shift === 'morning' && currentHour >= 12) {
            console.log('[handleTimeSlotClick] Morning shift expired (current hour:', currentHour, ')');
            alert('Ca sáng đã kết thúc (sau 12:00). Vui lòng chọn ca chiều.');
            return;
        }
        
        if (shift === 'afternoon' && currentHour >= 17) {
            console.log('[handleTimeSlotClick] Afternoon shift expired (current hour:', currentHour, ')');
            alert('Ca chiều đã kết thúc (sau 17:00). Vui lòng chọn ngày khác.');
            return;
        }

        // Kiểm tra xem ca này có available không
        if (this.availableTimeSlots) {
            const shiftData = shift === 'morning' ? this.availableTimeSlots.morning : this.availableTimeSlots.afternoon;
            if (shiftData && !shiftData.available) {
                console.log('[handleTimeSlotClick] Shift is not available:', shiftData);
                return; // Don't allow selection of unavailable slots
            }
        }

        // Cache DOM elements để tăng performance
        const allButtons = document.querySelectorAll('.time-btn.shift-btn');
        
        // Batch DOM updates
        requestAnimationFrame(() => {
        // Remove selection from all buttons
            allButtons.forEach(btn => {
            btn.classList.remove('selected', 'active');
                btn.style.backgroundColor = '#f8f9fa';
                btn.style.color = '#212529';
        });

        // Add selection to clicked button
        button.classList.add('selected', 'active');
            button.style.backgroundColor = '#2563eb';
            button.style.color = '#ffffff';

        // Store selected shift
            sessionStorage.setItem('selectedShift', shift);

            // Vẫn lưu selectedTime để tương thích với code cũ
            const textDiv = button.querySelector('.fw-bold.fs-6');
            const selectedTime = textDiv ? textDiv.textContent.trim() : '';
        sessionStorage.setItem('selectedTime', selectedTime);

            // Update confirmation display (async để tránh block UI)
            setTimeout(() => {
        if (window.appointmentConfirmation) {
            window.appointmentConfirmation.updateConfirmationDisplay();
        }
            }, 0);

        console.log('Selected time slot:', selectedTime);
        });
    }

    resetTimeSlots() {
        this.availableTimeSlots = [];
        const timeButtons = document.querySelectorAll('.time-btn.shift-btn');
        timeButtons.forEach(btn => {
            // Chỉ disable khi chưa có đủ thông tin cần thiết
            const hasDoctor = this.selectedDoctor !== null;
            const hasDate = this.selectedDate !== null;
            
            if (!hasDoctor || !hasDate) {
            btn.disabled = true;
            btn.style.opacity = 0.5;
            btn.style.cursor = 'not-allowed';
            btn.style.backgroundColor = '#f3f4f6';
            btn.style.color = '#6b7280';
                btn.title = !hasDoctor ? 'Vui lòng chọn bác sĩ trước' : 'Vui lòng chọn ngày để xem các khung giờ';
            } else {
                // Nếu đã có đủ thông tin, enable các nút và chờ API response
                btn.disabled = false;
                btn.style.opacity = 1;
                btn.style.cursor = 'pointer';
                btn.style.backgroundColor = '#f8f9fa';
                btn.style.color = '#212529';
                btn.title = 'Đang kiểm tra khả dụng...';
            }
            
            btn.classList.remove('booked', 'available', 'selected', 'active', 'outside-hours', 'disabled');
            
            // Khôi phục nội dung gốc
            const shift = btn.getAttribute('data-shift');
            const textDiv = btn.querySelector('.fw-bold.fs-6');
            if (textDiv) {
                if (shift === 'morning') {
                    textDiv.textContent = 'CA SÁNG (07:00 - 12:00)';
                } else if (shift === 'afternoon') {
                    textDiv.textContent = 'CA CHIỀU (13:00 - 17:00)';
                }
            }
            
            // Reset slot count
            const slotCountDiv = btn.querySelector('.slot-count');
            if (slotCountDiv) {
                // Lấy maxSlots từ availableTimeSlots nếu có,否则 sử dụng giá trị mặc định
                let maxSlots = 10; // Giá trị mặc định
                if (this.availableTimeSlots) {
                    const shift = btn.getAttribute('data-shift');
                    if (shift === 'morning' && this.availableTimeSlots.morning) {
                        maxSlots = this.availableTimeSlots.morning.maxSlots || 10;
                    } else if (shift === 'afternoon' && this.availableTimeSlots.afternoon) {
                        maxSlots = this.availableTimeSlots.afternoon.maxSlots || 10;
                    }
                }
                slotCountDiv.textContent = `Đã đặt: 0/${maxSlots}`;
                slotCountDiv.style.color = '#6c757d';
            }
        });
    }

    // Public method to refresh time slots
    refreshTimeSlots() {
        if (this.selectedDoctor && this.selectedDate) {
            this.updateTimeSlots();
        }
    }

    // Auto refresh để kiểm tra thời gian thực
    startAutoRefresh() {
        // Dừng interval cũ nếu có
        this.stopAutoRefresh();
        
        // Refresh mỗi 30 giây để kiểm tra thời gian (nhanh hơn)
        this.autoRefreshInterval = setInterval(() => {
            if (this.selectedDoctor && this.selectedDate) {
                // Chỉ refresh nếu đang xem ngày hôm nay
                const today = new Date();
                const selectedDate = new Date(this.selectedDate);
                if (today.toDateString() === selectedDate.toDateString()) {
                    console.log('[TimeSlotIntegration] Auto refreshing time slots...');
                    this.updateTimeSlots();
                }
            }
        }, 30000); // 30 giây
        
        // Kiểm tra ngay lập tức nếu đang xem ngày hôm nay
        if (this.selectedDoctor && this.selectedDate) {
            const today = new Date();
            const selectedDate = new Date(this.selectedDate);
            if (today.toDateString() === selectedDate.toDateString()) {
                console.log('[TimeSlotIntegration] Initial check for today...');
                setTimeout(() => {
                    this.updateTimeSlots();
                }, 1000); // 1 giây sau khi load
            }
        }
    }

    stopAutoRefresh() {
        if (this.autoRefreshInterval) {
            clearInterval(this.autoRefreshInterval);
            this.autoRefreshInterval = null;
        }
    }
}

// Initialize time slot integration when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    // Only initialize if we're on the appointment page
    const appointmentPage = document.querySelector('.appointment-tab-content');
    if (appointmentPage) {
        console.log('Initializing TimeSlotIntegration...');
        window.timeSlotIntegration = new TimeSlotIntegration();
    } else {
        console.log('Appointment page not found, skipping TimeSlotIntegration');
    }
});

// Cleanup khi page unload
window.addEventListener('beforeunload', function() {
    if (window.timeSlotIntegration) {
        window.timeSlotIntegration.stopAutoRefresh();
    }
}); 