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
                console.log('[customCalendarDateSelected] Event detail:', event.detail);
                console.log('[customCalendarDateSelected] Date type:', typeof event.detail.date);
                console.log('[customCalendarDateSelected] Date value:', event.detail.date);
                
                this.selectedDate = event.detail.date;
                
                console.log('[customCalendarDateSelected] After setting selectedDate:', {
                    selectedDate: this.selectedDate,
                    isDate: this.selectedDate instanceof Date,
                    isValid: this.selectedDate instanceof Date && !isNaN(this.selectedDate)
                });
                
                // Lưu lại ngày vào sessionStorage
                if (this.selectedDate instanceof Date && !isNaN(this.selectedDate)) {
                    const d = this.selectedDate;
                    const dateStr = `${d.getDate().toString().padStart(2,0)}/${(d.getMonth()+1).toString().padStart(2,0)}/${d.getFullYear()}`;
                    sessionStorage.setItem('selectedDate', dateStr);
                    console.log('[customCalendarDateSelected] Saved to sessionStorage:', dateStr);
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
            
            // Chỉ kiểm tra thời gian nếu đang xem ngày hôm nay
            const selectedDate = sessionStorage.getItem('selectedDate');
            let isToday = false;
            
            if (selectedDate) {
                // Parse date từ DD/MM/YYYY
                const [day, month, year] = selectedDate.split('/');
                const parsedDate = new Date(parseInt(year), parseInt(month) - 1, parseInt(day));
                isToday = today.toDateString() === parsedDate.toDateString();
            }
            
            let shouldRemoveShift = false;
            
            // Chỉ kiểm tra thời gian nếu đang xem ngày hôm nay
            if (isToday) {
                if (selectedShift === 'morning' && currentHour >= 12) {
                    shouldRemoveShift = true;
                    console.log('[loadSelectedData] Morning shift expired, removing selection');
                } else if (selectedShift === 'afternoon' && currentHour >= 17) {
                    shouldRemoveShift = true;
                    console.log('[loadSelectedData] Afternoon shift expired, removing selection');
                }
            }
            
            if (shouldRemoveShift) {
                sessionStorage.removeItem('selectedShift');
                sessionStorage.removeItem('selectedTime');
                console.log('[loadSelectedData] Removed expired shift from sessionStorage');
            }
        }

        // Nếu có doctor và date, gọi API ngay để kiểm tra availability
        const storedDate = sessionStorage.getItem('selectedDate');
        if (this.selectedDoctor && storedDate) {
            // Parse date từ DD/MM/YYYY
            const [day, month, year] = storedDate.split('/');
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
        
        // Chỉ kiểm tra nếu đang xem ngày hôm nay
        const storedDateForCheck = sessionStorage.getItem('selectedDate');
        let isToday = false;
        
        if (storedDateForCheck) {
            // Parse date từ DD/MM/YYYY
            const [day, month, year] = storedDateForCheck.split('/');
            const parsedDate = new Date(parseInt(year), parseInt(month) - 1, parseInt(day));
            isToday = today.toDateString() === parsedDate.toDateString();
        }
        
        if (isToday) {
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

            // Debug: Kiểm tra ngày được gửi
            const today = new Date();
            const isToday = today.toDateString() === this.selectedDate.toDateString();
            console.log('[updateTimeSlots] Debug info:', {
                selectedDate: this.selectedDate,
                dateStr: dateStr,
                today: today,
                isToday: isToday,
                currentHour: today.getHours()
            });

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
            console.log('[updateTimeSlots] Full API Response structure:', JSON.stringify(result, null, 2));

            if (result.success) {
                this.availableTimeSlots = result.data || {};
                console.log('[updateTimeSlots] Available slots data:', this.availableTimeSlots);
                
                // Debug: Kiểm tra dữ liệu từ API
                if (this.availableTimeSlots.morning) {
                    console.log('[updateTimeSlots] Morning shift data:', {
                        available: this.availableTimeSlots.morning.available,
                        isPastTime: this.availableTimeSlots.morning.isPastTime,
                        doctorWorks: this.availableTimeSlots.morning.doctorWorks,
                        count: this.availableTimeSlots.morning.count
                    });
                }
                if (this.availableTimeSlots.afternoon) {
                    console.log('[updateTimeSlots] Afternoon shift data:', {
                        available: this.availableTimeSlots.afternoon.available,
                        isPastTime: this.availableTimeSlots.afternoon.isPastTime,
                        doctorWorks: this.availableTimeSlots.afternoon.doctorWorks,
                        count: this.availableTimeSlots.afternoon.count
                    });
                }
                
                // Cập nhật UI ngay lập tức
                this.updateTimeSlotDisplay();
                
                // Kiểm tra xem bác sĩ có lịch làm việc không
                const morningWorks = this.availableTimeSlots.morning?.doctorWorks;
                const afternoonWorks = this.availableTimeSlots.afternoon?.doctorWorks;
                
                console.log('[updateTimeSlots] Checking doctor schedule:', {
                    morningWorks: morningWorks,
                    afternoonWorks: afternoonWorks,
                    isToday: isToday,
                    shouldCreateSchedule: (!morningWorks || !afternoonWorks) && !isToday,
                    hasMorningData: !!this.availableTimeSlots.morning,
                    hasAfternoonData: !!this.availableTimeSlots.afternoon
                });
                
                // Nếu bác sĩ chưa có lịch làm việc hoặc không có dữ liệu, tự động tạo
                if (((!morningWorks || !afternoonWorks) || (!this.availableTimeSlots.morning || !this.availableTimeSlots.afternoon)) && !isToday) {
                    console.log('[updateTimeSlots] Doctor has no working schedule or missing data, creating sample shifts...');
                    
                    // Hiển thị thông báo cho người dùng
                    this.showCreatingScheduleMessage();
                    
                    await this.createSampleShifts(year, parseInt(month));
                    
                    console.log('[updateTimeSlots] Re-fetching time slots after creating shifts...');
                    await this.updateTimeSlots(); // Re-fetch after creating
                    return;
                }
                
                // Fallback: Nếu không có dữ liệu gì cả, thử tạo lịch
                if (!this.availableTimeSlots || (!this.availableTimeSlots.morning && !this.availableTimeSlots.afternoon)) {
                    console.log('[updateTimeSlots] No data available, creating sample shifts as fallback...');
                    this.showCreatingScheduleMessage();
                    await this.createSampleShifts(year, parseInt(month));
                    console.log('[updateTimeSlots] Re-fetching time slots after creating shifts...');
                    await this.updateTimeSlots();
                    return;
                }
            } else {
                console.error('[updateTimeSlots] API returned error:', result.message);
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

    // Thêm method để tạo sample shifts
    async createSampleShifts(year, month) {
        try {
            console.log('[createSampleShifts] Creating shifts for doctor', this.selectedDoctor.id, 'in', month, year);
            
            const url = `${this.apiBaseUrl}/api/Appointment/create-sample-shifts/${this.selectedDoctor.id}?year=${year}&month=${month}`;
            const response = await fetch(url, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' }
            });

            if (!response.ok) {
                console.error('[createSampleShifts] HTTP error! status:', response.status);
                return false;
            }

            const result = await response.json();
            console.log('[createSampleShifts] Response:', result);

            if (result.success) {
                console.log('[createSampleShifts] Successfully created shifts:', result.message);
                return true;
            } else {
                console.error('[createSampleShifts] Failed to create shifts:', result.message);
                return false;
            }
        } catch (error) {
            console.error('[createSampleShifts] Exception:', error);
            return false;
        }
    }

    showCreatingScheduleMessage() {
        const timeButtons = document.querySelectorAll('.time-btn.shift-btn');
        timeButtons.forEach(btn => {
            btn.disabled = true;
            btn.style.opacity = '0.7';
            btn.style.cursor = 'wait';
            btn.title = 'Đang tạo lịch làm việc cho bác sĩ...';
            
            // Tìm và cập nhật nội dung bên trong button
            const textDiv = btn.querySelector('.fw-bold.fs-6');
            if (textDiv) {
                const shift = btn.getAttribute('data-shift');
                if (shift === 'morning') {
                    textDiv.textContent = 'CA SÁNG (07:00 - 12:00)';
                } else if (shift === 'afternoon') {
                    textDiv.textContent = 'CA CHIỀU (13:00 - 17:00)';
                }
            }
            
            // Cập nhật slot count thành "Đang tạo lịch..."
            const slotCountDiv = btn.querySelector('.slot-count');
            if (slotCountDiv) {
                slotCountDiv.textContent = 'Đang tạo lịch làm việc...';
                slotCountDiv.style.color = '#3b82f6';
            }
        });
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
        console.log('[updateTimeSlotDisplay] Starting update...');
        console.log('[updateTimeSlotDisplay] Available time slots:', this.availableTimeSlots);
        
        const morningBtn = document.querySelector('.time-btn[data-shift="morning"]');
        const afternoonBtn = document.querySelector('.time-btn[data-shift="afternoon"]');

        if (!morningBtn || !afternoonBtn) {
            console.error('[updateTimeSlotDisplay] Could not find shift buttons');
            return;
        }

        // Update morning shift
        this.updateShiftButton(morningBtn, 'morning');
        
        // Update afternoon shift
        this.updateShiftButton(afternoonBtn, 'afternoon');
        
        console.log('[updateTimeSlotDisplay] Update completed');
    }

    updateShiftButton(button, shift) {
        console.log(`[updateShiftButton] Updating ${shift} button...`);
        
        // Xác định trạng thái available và lý do
        let isAvailable = true;
        let reason = '';
        
        // Kiểm tra xem có phải ngày trong tương lai không
        const today = new Date();
        const isFutureDate = this.selectedDate && this.selectedDate > today;
        
        console.log(`[updateShiftButton] Debug info for ${shift}:`, {
            today: today.toISOString(),
            currentSelectedDate: this.selectedDate ? this.selectedDate.toISOString() : 'null',
            isFutureDate: isFutureDate,
            availableTimeSlots: this.availableTimeSlots
        });
        
        if (this.availableTimeSlots) {
            const shiftData = this.availableTimeSlots[shift];
            console.log(`[updateShiftButton] ${shift} data:`, shiftData);
            
            if (shiftData) {
                // Sử dụng available từ API
                isAvailable = shiftData.available === true;
                
                console.log(`[updateShiftButton] ${shift} - isAvailable:`, isAvailable);
                
                if (!isAvailable) {
                    if (shiftData.isPastTime) {
                        reason = shift === 'morning' ? 'Ca sáng đã kết thúc (sau 12:00)' : 'Ca chiều đã kết thúc (sau 17:00)';
                        console.log(`[updateShiftButton] ${shift} disabled because isPastTime = true`);
                    } else if (!shiftData.doctorWorks) {
                        reason = `Bác sĩ chưa có lịch làm việc ca ${shift === 'morning' ? 'sáng' : 'chiều'} cho ngày này`;
                        console.log(`[updateShiftButton] ${shift} disabled because doctorWorks = false`);
                    } else {
                        reason = `Ca ${shift === 'morning' ? 'sáng' : 'chiều'} đã đầy`;
                        console.log(`[updateShiftButton] ${shift} disabled because full`);
                    }
                } else {
                    console.log(`[updateShiftButton] ${shift} enabled`);
                }
            } else {
                console.log(`[updateShiftButton] No data for ${shift}`);
                isAvailable = false;
                reason = `Không có dữ liệu cho ca ${shift === 'morning' ? 'sáng' : 'chiều'}`;
            }
        } else {
            console.log('[updateShiftButton] No availableTimeSlots data');
            isAvailable = false;
            reason = 'Không có dữ liệu từ server';
        }
        
        // Update button state
        if (isAvailable) {
            button.classList.remove('disabled');
            button.removeAttribute('disabled');
            button.style.opacity = '1';
            button.style.cursor = 'pointer';
            console.log(`[updateShiftButton] ${shift} button ENABLED`);
        } else {
            button.classList.add('disabled');
            button.setAttribute('disabled', 'disabled');
            button.style.opacity = '0.5';
            button.style.cursor = 'not-allowed';
            console.log(`[updateShiftButton] ${shift} button DISABLED - Reason: ${reason}`);
        }
        
        // Update tooltip
        if (reason) {
            button.title = reason;
        } else {
            button.title = '';
        }
        
        // Update count display
        const countElement = button.querySelector('.slot-count');
        if (countElement && this.availableTimeSlots && this.availableTimeSlots[shift]) {
            const count = this.availableTimeSlots[shift].count || 0;
            const maxSlots = this.availableTimeSlots[shift].maxSlots || 10;
            countElement.textContent = `Đã đặt: ${count}/${maxSlots}`;
            
            // Thay đổi màu sắc dựa trên số lượng
            const threshold80 = Math.ceil(maxSlots * 0.8);
            const threshold60 = Math.ceil(maxSlots * 0.6);
            
            if (count >= threshold80) {
                countElement.style.color = '#dc3545'; // Đỏ khi gần đầy
            } else if (count >= threshold60) {
                countElement.style.color = '#ffc107'; // Vàng khi trung bình
            } else {
                countElement.style.color = '#6c757d'; // Xám khi còn nhiều
            }
        }
        
        // Update button text
        const textDiv = button.querySelector('.fw-bold.fs-6');
        if (textDiv) {
            if (shift === 'morning') {
                textDiv.textContent = 'CA SÁNG (07:00 - 12:00)';
            } else if (shift === 'afternoon') {
                textDiv.textContent = 'CA CHIỀU (13:00 - 17:00)';
            }
        }
    }

    handleTimeSlotClick(button) {
        if (button.disabled || button.classList.contains('booked')) {
            console.log('[handleTimeSlotClick] Button is disabled or booked, ignoring click');
            return; // Don't allow selection of booked slots
        }

        // Kiểm tra thời gian hiện tại CHỈ KHI đang chọn ngày hôm nay
        const shift = button.getAttribute('data-shift');
        const today = new Date();
        const selectedDate = this.selectedDate;
        
        // Chỉ kiểm tra thời gian nếu đang chọn ngày hôm nay
        if (selectedDate && today.toDateString() === selectedDate.toDateString()) {
            const currentHour = today.getHours();
            
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