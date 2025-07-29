// load-appointments.js
// Script chính để load và hiển thị dữ liệu cuộc hẹn từ database
(function() {
    'use strict';

    // Cấu hình
    const CONFIG = {
        refreshInterval: 30000, // 30 giây
        maxRetries: 3
    };

    // Trạng thái ứng dụng
    let appState = {
        appointments: [],
        isLoading: false,
        lastUpdate: null,
        retryCount: 0
    };

    // Utility functions
    const Utils = {
        // Format ngày tháng
        formatDate: function(dateString) {
            if (!dateString) return 'N/A';
            try {
                const date = new Date(dateString);
                return date.toLocaleDateString('vi-VN', {
                    year: 'numeric',
                    month: '2-digit',
                    day: '2-digit'
                });
            } catch (e) {
                return 'N/A';
            }
        },

        // Lấy text trạng thái
        getStatusText: function(status) {
            const statusMap = {
                'PENDING': 'Chờ khám',
                'COMPLETED': 'Đã hoàn thành',
                'CANCELLED': 'Đã hủy',
                'IN_PROGRESS': 'Đang khám'
            };
            return statusMap[status] || status || 'N/A';
        },

        // Lấy text ca khám
        getShiftText: function(shift) {
            const shiftMap = {
                'MORNING': 'Ca sáng',
                'AFTERNOON': 'Ca chiều',
                'EVENING': 'Ca tối',
                'morning': 'Ca sáng',
                'afternoon': 'Ca chiều',
                'evening': 'Ca tối',
                'Morning': 'Ca sáng',
                'Afternoon': 'Ca chiều',
                'Evening': 'Ca tối'
            };
            return shiftMap[shift] || shift || 'N/A';
        },

        // Tạo badge trạng thái
        createStatusBadge: function(status) {
            const text = this.getStatusText(status);
            let className = 'badge bg-secondary'; // Mặc định
            
            // Xác định màu sắc dựa trên status (không phân biệt hoa thường)
            const statusLower = (status || '').toLowerCase();
            
            if (statusLower.includes('đã lên lịch') || statusLower.includes('scheduled') || statusLower.includes('pending')) {
                className = 'badge bg-primary'; // Xanh dương
            } else if (statusLower.includes('đang khám') || statusLower.includes('in progress') || statusLower.includes('inprogress')) {
                className = 'badge bg-info'; // Xanh nhạt
            } else if (statusLower.includes('đã hoàn thành') || statusLower.includes('completed')) {
                className = 'badge bg-success'; // Xanh lá
            } else if (statusLower.includes('đã hủy') || statusLower.includes('cancelled')) {
                className = 'badge bg-danger'; // Đỏ
            } else if (statusLower.includes('đến muộn') || statusLower.includes('late')) {
                className = 'badge bg-warning'; // Vàng
            }
            
            return `<span class="${className}">${text}</span>`;
        },

        // Tạo nút hành động
                    createActionButtons: function(appointmentId, status, appointment) {
            let buttons = '';
            
                // Nút xem chi tiết (luôn hiển thị)
                buttons += `<button class="btn btn-sm btn-outline-primary me-1" onclick="viewAppointmentDetail('${appointmentId}')" title="Xem chi tiết">
                    <i class="fas fa-eye"></i>
                </button>`;

                // Nút chỉnh sửa (không hiển thị cho appointments đã hoàn thành, đã hủy và đang khám)
                if (status !== 'đã hoàn thành' && status !== 'Đã hoàn thành' && status !== 'DA_HOAN_THANH' && 
                    status !== 'đã hủy' && status !== 'Đã hủy' && status !== 'DA_HUY' &&
                    status !== 'đang khám' && status !== 'Đang khám' && status !== 'DANG_KHAM') {
                    buttons += `<button class="btn btn-sm btn-outline-warning me-1" onclick="editAppointment('${appointmentId}')" title="Chỉnh sửa">
                        <i class="fas fa-edit"></i>
                    </button>`;
                }

                // Nút chuyển vào đang khám (chỉ hiển thị cho appointments "đã lên lịch")
                if (status === 'đã lên lịch' || status === 'Đã lên lịch' || status === 'DA_LEN_LICH') {
                    buttons += `<button class="btn btn-sm btn-outline-info me-1" onclick="startAppointment('${appointmentId}')" title="Bắt đầu khám">
                        <i class="fas fa-play"></i>
                    </button>`;
                }

                // Nút hoàn thành (chỉ hiển thị cho appointments "đang khám")
                if (status === 'đang khám' || status === 'Đang khám' || status === 'DANG_KHAM') {
                    buttons += `<button class="btn btn-sm btn-outline-success me-1" onclick="completeAppointment('${appointmentId}')" title="Hoàn thành khám">
                        <i class="fas fa-check"></i>
                    </button>`;
                }
      
                // Nút hủy (chỉ hiển thị cho appointments chưa hoàn thành và chưa hủy)
                if (status !== 'đã hoàn thành' && status !== 'Đã hoàn thành' && status !== 'DA_HOAN_THANH' && 
                    status !== 'đã hủy' && status !== 'Đã hủy' && status !== 'DA_HUY') {
                    buttons += `<button class="btn btn-sm btn-outline-danger" onclick="cancelAppointment('${appointmentId}')" title="Hủy lịch hẹn">
                        <i class="fas fa-times"></i>
                    </button>`;
                }

                // Nút khôi phục (chỉ hiển thị cho appointments đã hủy và trong ca làm việc)
                if (status === 'đã hủy' || status === 'Đã hủy' || status === 'DA_HUY') {
                    const now = new Date();
                    const today = new Date(now.getFullYear(), now.getMonth(), now.getDate());
                    const appointmentDate = new Date(appointment.appointmentDate);
                    
                    // Chỉ hiển thị nút restore nếu lịch hẹn trong ngày hôm nay
                    if (appointmentDate.toDateString() === today.toDateString()) {
                        // Xác định ca làm việc của lịch hẹn
                        let shiftStart, shiftEnd, shiftTypeVN;
                        if (appointment.startTime) {
                            const hour = new Date(`2000-01-01T${appointment.startTime}`).getHours();
                            if (hour < 12) {
                                // Ca sáng: 07:00 - 12:00
                                shiftStart = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 7, 0, 0);
                                shiftEnd = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 12, 0, 0);
                                shiftTypeVN = "Ca sáng";
                            } else {
                                // Ca chiều: 13:00 - 17:00
                                shiftStart = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 13, 0, 0);
                                shiftEnd = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 17, 0, 0);
                                shiftTypeVN = "Ca chiều";
                            }
                        } else {
                            // Mặc định ca sáng nếu không có thời gian
                            shiftStart = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 7, 0, 0);
                            shiftEnd = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 12, 0, 0);
                            shiftTypeVN = "Ca sáng";
                        }
                        
                        // Chỉ hiển thị nút restore nếu trong ca làm việc
                        if (now >= shiftStart && now <= shiftEnd) {
                            buttons += `<button class="btn btn-sm btn-outline-secondary" onclick="restoreAppointment('${appointmentId}')" title="Khôi phục cuộc hẹn">
                                <i class="fas fa-undo"></i>
                            </button>`;
                        }
                    }
                }

                return `<div class="d-flex justify-content-center align-items-center">${buttons}</div>`;
        },

        // Hiển thị loading
        showLoading: function(container) {
            if (!container) return;
            
            container.innerHTML = `
                <tr>
                    <td colspan="8" class="text-center py-5">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Loading...</span>
                        </div>
                        <p class="mt-3 text-muted">Đang tải dữ liệu...</p>
                    </td>
                </tr>
            `;
        },

        // Hiển thị lỗi
        showError: function(container, message) {
            if (!container) return;
            
            container.innerHTML = `
                <tr>
                    <td colspan="8" class="text-center py-5">
                        <div class="text-danger mb-3">
                            <i class="fas fa-exclamation-triangle fa-3x"></i>
                        </div>
                        <h5 class="text-danger">Lỗi tải dữ liệu</h5>
                        <p class="text-muted">${message}</p>
                        <button class="btn btn-primary" onclick="AppointmentLoader.retryLoad()">
                            <i class="fas fa-redo"></i> Thử lại
                        </button>
                    </td>
                </tr>
            `;
        },

        // Hiển thị thông báo không có dữ liệu
        showNoData: function(container, message = 'Không có cuộc hẹn nào') {
            if (!container) return;
            
            container.innerHTML = `
                <tr>
                    <td colspan="8" class="text-center py-5">
                        <div class="text-muted mb-3">
                            <i class="fas fa-calendar-times fa-3x"></i>
                        </div>
                        <h5 class="text-muted">${message}</h5>
                    </td>
                </tr>
            `;
        }
    };

    // Class chính để load và hiển thị cuộc hẹn
    class AppointmentLoader {
        constructor() {
            this.init();
        }

        init() {
            this.setupEventListeners();
            this.loadAppointments();
            this.startAutoRefresh();
        }

        // Thiết lập event listeners
        setupEventListeners() {
            // Lắng nghe sự kiện filter
            document.addEventListener('click', (e) => {
                if (e.target.matches('[data-filter]')) {
                    const filter = e.target.getAttribute('data-filter');
                    console.log('🔍 Filter clicked:', filter);
                    this.setFilter(filter);
                }
            });

            // Lắng nghe sự kiện dropdown filter
            document.addEventListener('click', (e) => {
                if (e.target.matches('[data-value]')) {
                    const filter = e.target.getAttribute('data-value');
                    console.log('🔍 Dropdown filter clicked:', filter);
                    this.setFilter(filter);
                }
            });

            // Lắng nghe sự kiện refresh
            document.addEventListener('click', (e) => {
                if (e.target.matches('[data-refresh]')) {
                    console.log('🔄 Refresh clicked');
                    this.loadAppointments();
                }
            });

            // Lắng nghe sự kiện từ appointment-counter.js
            window.addEventListener('appointmentFilterChanged', (e) => {
                console.log('🔍 Filter event received:', e.detail);
                if (e.detail && e.detail.filterType) {
                    this.setFilter(e.detail.filterType);
                }
            });
        }

        // Load dữ liệu cuộc hẹn từ API
        async loadAppointments() {
            if (appState.isLoading) return;

            appState.isLoading = true;
            this.showLoadingInAllTabs();

            try {
                console.log('🔄 Đang tải dữ liệu cuộc hẹn...');
                console.log('🔗 API URL:', window.AppointmentAPI.appointments.list);
                
                const response = await fetch(window.AppointmentAPI.appointments.list, {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json'
                    }
                });

                console.log('📡 API Response Status:', response.status);
                console.log('📡 API Response OK:', response.ok);

                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }

                const data = await response.json();
                console.log('✅ Dữ liệu cuộc hẹn:', data);
                console.log('📊 Data Type:', typeof data);
                console.log('📊 Is Array:', Array.isArray(data));
                console.log('📊 Data Keys:', Object.keys(data));

                // Xử lý response format
                let appointments = [];
                if (data && data.success && data.data) {
                    appointments = data.data;
                    console.log('✅ Sử dụng format success.data');
                } else if (Array.isArray(data)) {
                    appointments = data;
                    console.log('✅ Sử dụng format array trực tiếp');
                } else if (data && Array.isArray(data.data)) {
                    appointments = data.data;
                    console.log('✅ Sử dụng format data.data array');
                } else {
                    console.log('⚠️ Format dữ liệu không nhận diện được, thử parse lại');
                    appointments = Array.isArray(data) ? data : [];
                }

                console.log('📊 Appointments count:', appointments.length);
                if (appointments.length > 0) {
                    console.log('📊 First appointment:', appointments[0]);
                    console.log('📊 First appointment keys:', Object.keys(appointments[0]));
                }

                appState.appointments = appointments;
                appState.lastUpdate = new Date();
                appState.retryCount = 0;

                console.log('📊 AppState after update:', appState);
                console.log('📊 Appointments in appState:', appState.appointments.length);

                // Thông báo cho appointment-counter.js về dữ liệu mới
                if (typeof window.loadAndCalculateAppointments === 'function') {
                    console.log('🔄 Thông báo cho appointment-counter.js về dữ liệu mới');
                    window.loadAndCalculateAppointments();
                }

                // Kiểm tra DOM trước khi update
                console.log('🔍 Checking DOM before update...');
                const tabPanes = document.querySelectorAll('.tab-pane');
                console.log('📊 Found tab panes:', tabPanes.length);
                tabPanes.forEach((pane, index) => {
                    console.log(`📊 Tab ${index} - pane id:`, pane.id);
                    const table = pane.querySelector('table');
                    console.log(`📊 Tab ${index} - table found:`, !!table);
                    if (table) {
                        const tbody = table.querySelector('tbody');
                        console.log(`📊 Tab ${index} - tbody found:`, !!tbody);
                        if (tbody) {
                            console.log(`📊 Tab ${index} - tbody children:`, tbody.children.length);
                        }
                    }
                });

                this.updateAllTables();
                this.updateCounter();
                this.hideLoading();

                console.log(`✅ Đã tải ${appointments.length} cuộc hẹn thành công`);

                // Phát sự kiện để appointment-counter.js có thể đồng bộ
                window.dispatchEvent(new CustomEvent('appointmentsLoaded', {
                    detail: { 
                        appointments: appState.appointments,
                        timestamp: new Date()
                    }
                }));

            } catch (error) {
                console.error('❌ Lỗi tải dữ liệu:', error);
                appState.retryCount++;
                
                if (appState.retryCount < CONFIG.maxRetries) {
                    console.log(`🔄 Thử lại lần ${appState.retryCount}...`);
                    setTimeout(() => this.loadAppointments(), 2000);
                } else {
                    this.showErrorInAllTabs(error.message);
                }
            } finally {
                appState.isLoading = false;
            }
        }

        // Hiển thị loading trong tất cả tabs
        showLoadingInAllTabs() {
            const tabPanes = document.querySelectorAll('.tab-pane');
            tabPanes.forEach(pane => {
                const table = pane.querySelector('table');
                if (table) {
                    const tbody = table.querySelector('tbody');
                    if (tbody) {
                        Utils.showLoading(tbody);
                    }
                }
            });
        }

        // Hiển thị lỗi trong tất cả tabs
        showErrorInAllTabs(message) {
            const tabPanes = document.querySelectorAll('.tab-pane');
            tabPanes.forEach(pane => {
                const table = pane.querySelector('table');
                if (table) {
                    const tbody = table.querySelector('tbody');
                    if (tbody) {
                        Utils.showError(tbody, message);
                    }
                }
            });
        }

        // Ẩn loading
        hideLoading() {
            // Loading sẽ được thay thế bởi dữ liệu thực
        }

        // Cập nhật tất cả bảng
        updateAllTables() {
            console.log('🔄 Updating all tables...');
            const tabPanes = document.querySelectorAll('.tab-pane');
            console.log('📊 Found tab panes:', tabPanes.length);
            
            tabPanes.forEach((pane, index) => {
                console.log(`📊 Tab ${index} - pane:`, pane);
                console.log(`📊 Tab ${index} - pane id:`, pane.id);
                const table = pane.querySelector('table');
                console.log(`📊 Tab ${index} - table:`, table);
                if (table) {
                    const tbody = table.querySelector('tbody');
                    console.log(`📊 Tab ${index} - tbody:`, tbody);
                    this.updateTable(table, index);
                } else {
                    console.warn(`⚠️ No table found in tab ${index}`);
                }
            });
        }

        // Cập nhật bảng cụ thể
        updateTable(table, tabIndex) {
            console.log(`🔄 Updating table for tab ${tabIndex}...`);
            const tbody = table.querySelector('tbody');
            if (!tbody) {
                console.warn(`⚠️ No tbody found in table for tab ${tabIndex}`);
                return;
            }

            // Lọc dữ liệu theo tab
            let filteredAppointments = this.filterAppointmentsByTab(appState.appointments, tabIndex);
            console.log(`📊 Filtered appointments for tab ${tabIndex}:`, filteredAppointments.length);
            
                               if (filteredAppointments.length === 0) {
                       const messages = [
                           'Không có cuộc hẹn sắp tới',
                            'Không có cuộc hẹn đang khám',
                           'Không có cuộc hẹn đã hoàn thành',
                           'Không có cuộc hẹn đã hủy'
                       ];
                       Utils.showNoData(tbody, messages[tabIndex] || 'Không có dữ liệu');
                       console.log(`📊 Showing no data message for tab ${tabIndex}`);
                       return;
                   }

            // Tạo HTML cho bảng
            const tableHTML = this.generateTableRows(filteredAppointments);
            console.log(`📊 Generated HTML for tab ${tabIndex}:`, tableHTML);
            console.log(`📊 HTML length:`, tableHTML.length);
            console.log(`📊 Tbody before update:`, tbody.innerHTML.length, 'characters');
            tbody.innerHTML = tableHTML;
            console.log(`📊 Tbody after update:`, tbody.innerHTML.length, 'characters');
            console.log(`✅ Updated table for tab ${tabIndex} with ${filteredAppointments.length} appointments`);
        }

                           // Lọc cuộc hẹn theo tab
                   filterAppointmentsByTab(appointments, tabIndex) {
                       console.log(`🔍 Filtering appointments for tab ${tabIndex}...`);
                       console.log('📊 Total appointments:', appointments.length);
                       
                       const today = new Date();
                       today.setHours(0, 0, 0, 0);
                       console.log('📅 Today:', today);

                       // Áp dụng filter toàn cục trước
                       let globalFiltered = appointments;
                       if (appState.currentFilter && appState.currentFilter !== 'all') {
                           globalFiltered = this.applyGlobalFilter(appointments, appState.currentFilter);
                           console.log(`🔍 After global filter (${appState.currentFilter}):`, globalFiltered.length);
                       }

                       const filtered = globalFiltered.filter(appointment => {
                           console.log('📊 Processing appointment:', appointment);
                console.log('📊 Appointment status:', appointment.status);
                console.log('📊 Appointment date:', appointment.date);
                           
                           let appointmentDate;
                           try {
                               appointmentDate = new Date(appointment.date);
                               appointmentDate.setHours(0, 0, 0, 0);
                           } catch (e) {
                               console.warn('⚠️ Invalid date:', appointment.date);
                               return false;
                           }

                           let shouldInclude = false;
                           
                // Áp dụng filter theo tab
                           switch (tabIndex) {
                               case 0: // Sắp tới (Upcoming)
                        // Hiển thị appointments với status "đã lên lịch"
                        shouldInclude = appointment.status === 'đã lên lịch' ||
                                       appointment.status === 'Đã lên lịch' ||
                                       appointment.status === 'DA_LEN_LICH';
                                   break;
                    case 1: // Đang khám (In Progress)
                        // Hiển thị appointments với status "đang khám"
                        shouldInclude = appointment.status === 'đang khám' ||
                                       appointment.status === 'Đang khám' ||
                                       appointment.status === 'DANG_KHAM';
                                   break;
                               case 2: // Đã hoàn thành (Completed)
                        // Hiển thị appointments với status "đã hoàn thành"
                        shouldInclude = appointment.status === 'đã hoàn thành' ||
                                       appointment.status === 'Đã hoàn thành' ||
                                       appointment.status === 'DA_HOAN_THANH';
                                   break;
                               case 3: // Đã hủy (Cancelled)
                        // Hiển thị appointments với status "đã hủy"
                        shouldInclude = appointment.status === 'đã hủy' ||
                                       appointment.status === 'Đã hủy' ||
                                       appointment.status === 'DA_HUY';
                                   break;
                               default:
                        shouldInclude = false;
                }
                
                console.log(`📊 Appointment ${appointment.id}: status="${appointment.status}", tabIndex=${tabIndex}, shouldInclude=${shouldInclude}`);
                if (!shouldInclude) {
                    console.log(`❌ Appointment ${appointment.id} không match filter cho tab ${tabIndex}`);
                } else {
                    console.log(`✅ Appointment ${appointment.id} match filter cho tab ${tabIndex}`);
                }
                return shouldInclude;
            });
            
            console.log(`📊 Filtered result for tab ${tabIndex}:`, filtered.length);
            return filtered;
        }

        // Tạo HTML cho các dòng bảng
        generateTableRows(appointments) {
            console.log('🔄 Generating table rows for', appointments.length, 'appointments');
            
            const rows = appointments.map((appointment, index) => {
                console.log('📊 Generating row for appointment:', appointment);
                console.log('📊 Appointment ID:', appointment.id);
                
                // Mapping dữ liệu từ API response
                const patientName = appointment.name || appointment.patientName || appointment.patient?.name || 'N/A';
                const doctorName = appointment.doctorName || appointment.doctor?.name || 'N/A';
                const clinicName = appointment.clinic || appointment.clinicName || appointment.clinic?.name || 'N/A';
                const date = appointment.date || appointment.appointmentDate || 'N/A';
                const shift = appointment.shift || appointment.shiftName || 'N/A';
                const status = appointment.status || 'N/A';
                const patientImage = appointment.patientImage || appointment.patient?.image || './assets/images/table/10.png';
                
                console.log('📊 Mapped Data:', {
                    patientName,
                    doctorName,
                    clinicName,
                    date,
                    shift,
                    status,
                    patientImage
                });
                
                const row = `
                    <tr data-appointment-id="${appointment.id}">
                        <th scope="row">${index + 1}</th>
                        <td>
                            <h5 class="mb-0">${patientName}</h5>
                        </td>
                        <td>${doctorName}</td>
                        <td>${clinicName}</td>
                        <td>${Utils.formatDate(date)}</td>
                        <td>${Utils.getShiftText(shift)}</td>
                        <td>${Utils.createStatusBadge(status)}</td>
                        <td>
                            ${Utils.createActionButtons(appointment.id, status, appointment)}
                        </td>
                    </tr>
                `;
                
                console.log('📊 Generated row HTML length:', row.length);
                console.log('📊 Generated row HTML preview:', row.substring(0, 200) + '...');
                return row;
            });
            
            const result = rows.join('');
            console.log('✅ Generated total HTML length:', result.length);
            console.log('✅ Generated HTML preview:', result.substring(0, 500) + '...');
            return result;
        }

        // Cập nhật counter
        updateCounter() {
            console.log('🔄 Updating counter...');
            
            // Kiểm tra xem có AppointmentCounter từ appointment-counter.js không
            if (window.AppointmentCounter && typeof window.AppointmentCounter.updateDisplay === 'function') {
                console.log('✅ Sử dụng AppointmentCounter từ appointment-counter.js');
                const counts = window.AppointmentCounter.getCounts();
                window.AppointmentCounter.updateDisplay(counts, 'today');
                return;
            }
            
            // Fallback: tự tính toán nếu không có AppointmentCounter
            console.log('⚠️ Không có AppointmentCounter, tự tính toán...');
            
            // Đếm tất cả appointments
            const totalCount = appState.appointments.length;
            console.log('📊 Total appointments count:', totalCount);
            
            // Đếm appointments hôm nay
            const today = new Date();
            today.setHours(0, 0, 0, 0);

            const todayCount = appState.appointments.filter(appointment => {
                try {
                    const appointmentDate = new Date(appointment.date);
                    appointmentDate.setHours(0, 0, 0, 0);
                    return appointmentDate.getTime() === today.getTime();
                } catch (e) {
                    console.warn('⚠️ Invalid date for counter:', appointment.date);
                    return false;
                }
            }).length;

            console.log('📊 Today appointments count:', todayCount);

            // Tìm element hiển thị số cuộc hẹn - ưu tiên id="appointment-count"
            let counterElement = document.getElementById('appointment-count');
            
            // Nếu không tìm thấy, thử tìm bằng data-counter
            if (!counterElement) {
                counterElement = document.querySelector('[data-counter="today"]');
            }
            
            // Nếu vẫn không tìm thấy, thử tìm bằng class hoặc text content
            if (!counterElement) {
                const possibleElements = document.querySelectorAll('p, span, div');
                counterElement = Array.from(possibleElements).find(el => 
                    el.textContent && el.textContent.includes('cuộc hẹn') && 
                    (el.id === 'appointment-count' || el.classList.contains('counter'))
                );
            }
            
            if (counterElement) {
                console.log('🔍 Counter element found:', counterElement);
                console.log('🔍 Counter element text before update:', counterElement.textContent);
                
                // Hiển thị số cuộc hẹn hôm nay
                counterElement.textContent = `${todayCount} cuộc hẹn đã lên lịch hôm nay`;
                
                // Cập nhật CSS class
                if (todayCount === 0) {
                    counterElement.className = 'mb-0 text-muted';
                } else if (todayCount <= 5) {
                    counterElement.className = 'mb-0 text-success';
                } else if (todayCount <= 10) {
                    counterElement.className = 'mb-0 text-warning';
                } else {
                    counterElement.className = 'mb-0 text-danger';
                }
                
                console.log('🔍 Counter element text after update:', counterElement.textContent);
                console.log('✅ Counter updated successfully');
            } else {
                console.warn('⚠️ Counter element not found');
                // Tìm tất cả elements có thể là counter
                const allElements = document.querySelectorAll('*');
                const possibleCounters = Array.from(allElements).filter(el => 
                    el.textContent && el.textContent.includes('cuộc hẹn')
                );
                console.log('🔍 Possible counter elements found:', possibleCounters.length);
                possibleCounters.forEach((el, index) => {
                    console.log(`🔍 Possible counter ${index}:`, el.textContent);
                });
            }
        }

        // Thiết lập filter
        setFilter(filter) {
            console.log('🔍 Setting filter:', filter);
            appState.currentFilter = filter;
            
            // Cập nhật UI để hiển thị filter đang active
            this.updateFilterUI(filter);
            
            // Cập nhật bảng theo filter
            this.updateAllTables();
            
            // Cập nhật counter theo filter
            if (window.AppointmentFilter) {
                window.AppointmentFilter.updateCounter();
            }
        }

        // Cập nhật UI filter
        updateFilterUI(activeFilter) {
            // Tìm tất cả các nút filter
            const filterButtons = document.querySelectorAll('[data-filter]');
            filterButtons.forEach(button => {
                const filterType = button.getAttribute('data-filter');
                if (filterType === activeFilter) {
                    button.classList.add('active', 'btn-primary');
                    button.classList.remove('btn-outline-primary');
                } else {
                    button.classList.remove('active', 'btn-primary');
                    button.classList.add('btn-outline-primary');
                }
            });

            // Cập nhật dropdown filter nếu có
            const filterDropdown = document.getElementById('appointmentFilterDropdown');
            if (filterDropdown) {
                const activeItem = filterDropdown.querySelector(`[data-value="${activeFilter}"]`);
                if (activeItem) {
                    const buttonText = filterDropdown.querySelector('.dropdown-toggle');
                    if (buttonText) {
                        buttonText.textContent = activeItem.textContent;
                    }
                }
            }
        }

        // Áp dụng filter toàn cục
        applyGlobalFilter(appointments, filterType) {
            console.log('🔍 Applying global filter:', filterType);
            
            const today = new Date().toISOString().split('T')[0];
            
            switch (filterType) {
                case 'today':
                    return appointments.filter(apt => {
                        const aptDate = new Date(apt.date).toISOString().split('T')[0];
                        return aptDate === today;
                    });
                    
                case 'this_week':
                    const weekStart = this.getWeekStart();
                    const weekEnd = this.getWeekEnd();
                    return appointments.filter(apt => {
                        const aptDate = new Date(apt.date);
                        return aptDate >= weekStart && aptDate <= weekEnd;
                    });
                    
                case 'this_month':
                    const monthStart = this.getMonthStart();
                    const monthEnd = this.getMonthEnd();
                    return appointments.filter(apt => {
                        const aptDate = new Date(apt.date);
                        return aptDate >= monthStart && aptDate <= monthEnd;
                    });
                    
                case 'pending':
                    return appointments.filter(apt => 
                        apt.status === 'đã lên lịch' || apt.status === 'Đã lên lịch'
                    );
                    
                case 'in_progress':
                    return appointments.filter(apt => 
                        apt.status === 'đang khám' || apt.status === 'Đang khám'
                    );
                    
                case 'completed':
                    return appointments.filter(apt => 
                        apt.status === 'đã hoàn thành' || apt.status === 'Đã hoàn thành'
                    );
                    
                case 'cancelled':
                    return appointments.filter(apt => 
                        apt.status === 'đã hủy' || apt.status === 'Đã hủy'
                    );
                    
                default:
                    return appointments;
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

        // Thử lại load
        retryLoad() {
            appState.retryCount = 0;
            this.loadAppointments();
        }

        // Bắt đầu auto refresh
        startAutoRefresh() {
            setInterval(() => {
                if (!appState.isLoading) {
                    this.loadAppointments();
                }
            }, CONFIG.refreshInterval);
        }
    }

    // Khởi tạo khi DOM sẵn sàng
    function init() {
        // Đợi DOM content loaded trước
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                console.log('🔄 DOM Content Loaded - Waiting for tabs to be ready...');
                waitForTabs();
            });
        } else {
            console.log('🔄 DOM already ready - Waiting for tabs to be ready...');
            waitForTabs();
        }
    }

    function waitForTabs() {
        // Kiểm tra xem tabs đã được tạo chưa
        const tabPanes = document.querySelectorAll('.tab-pane');
        if (tabPanes.length >= 4) {
            console.log('✅ Tabs already exist, initializing AppointmentLoader');
            window.AppointmentLoader = new AppointmentLoader();
        } else {
            console.log('⏳ Waiting for tabs to be created...');
            // Đợi event tabsReady từ add-tabs-vn.js
            document.addEventListener('tabsReady', () => {
                console.log('✅ Tabs ready event received, initializing AppointmentLoader');
                window.AppointmentLoader = new AppointmentLoader();
            });
            
            // Fallback: nếu sau 2 giây vẫn chưa có tabs, khởi tạo anyway
            setTimeout(() => {
                if (!window.AppointmentLoader) {
                    console.log('⚠️ Timeout waiting for tabs, initializing AppointmentLoader anyway');
            window.AppointmentLoader = new AppointmentLoader();
                }
            }, 2000);
        }
    }

    // Khởi chạy
    init();

    // Export các hàm cần thiết ra global scope
    window.loadAppointments = function(filterType = null) {
        if (window.AppointmentLoader) {
            return window.AppointmentLoader.loadAppointments();
        } else {
            console.warn('AppointmentLoader chưa được khởi tạo');
        }
    };

    window.filterAppointments = function(filterType) {
        if (window.AppointmentLoader) {
            window.AppointmentLoader.setFilter(filterType);
        } else {
            console.warn('AppointmentLoader chưa được khởi tạo');
        }
    };

    window.refreshAppointments = function() {
        if (window.AppointmentLoader) {
            window.AppointmentLoader.loadAppointments();
        } else {
            console.warn('AppointmentLoader chưa được khởi tạo');
        }
    };

    // Test function để kiểm tra script
    window.testAppointmentLoader = function() {
        console.log('🧪 Testing AppointmentLoader...');
        console.log('📊 Window.AppointmentLoader:', window.AppointmentLoader);
        console.log('📊 AppState:', window.appState);
        
        if (window.AppointmentLoader) {
            console.log('✅ AppointmentLoader is initialized');
            console.log('📊 Current appointments:', window.appState.appointments.length);
            
            // Test DOM elements
            const tabPanes = document.querySelectorAll('.tab-pane');
            console.log('📊 Tab panes found:', tabPanes.length);
            
            const expectedTabs = ['upcoming', 'inprogress', 'completed', 'cancelled'];
            tabPanes.forEach((pane, index) => {
                console.log(`📊 Tab ${index}:`, pane.id);
                console.log(`📊 Tab ${index} expected:`, expectedTabs[index]);
                const table = pane.querySelector('table');
                if (table) {
                    const tbody = table.querySelector('tbody');
                    console.log(`📊 Tab ${index} tbody:`, tbody ? 'Found' : 'Not found');
                }
            });
            
            // Test with mock data
            const mockAppointment = {
                id: 'test-1',
                patientName: 'Test Patient',
                doctorName: 'Test Doctor',
                clinicName: 'Test Clinic',
                date: '2024-01-15',
                shift: 'MORNING',
                status: 'PENDING'
            };
            
            console.log('🧪 Testing with mock data:', mockAppointment);
            const testHTML = Utils.createStatusBadge(mockAppointment.status);
            console.log('🧪 Status badge test:', testHTML);
            
        } else {
            console.error('❌ AppointmentLoader is not initialized');
        }
    };

    // Test function để hiển thị dữ liệu test
    window.testDisplayWithMockData = function() {
        console.log('🧪 Testing display with mock data...');
        
        if (!window.AppointmentLoader) {
            console.error('❌ AppointmentLoader not initialized');
            return;
        }
        
        // Tạo dữ liệu test cho 4 tab
        const mockAppointments = [
            {
                id: 'test-1',
                patientName: 'Nguyễn Văn A',
                doctorName: 'Bác sĩ Trần Thị B',
                clinicName: 'Phòng khám Tim mạch',
                date: '2024-01-15',
                shift: 'MORNING',
                status: 'PENDING'
            },
            {
                id: 'test-2',
                patientName: 'Lê Văn C',
                doctorName: 'Bác sĩ Phạm Văn D',
                clinicName: 'Phòng khám Nhi khoa',
                date: '2024-01-16',
                shift: 'AFTERNOON',
                status: 'IN_PROGRESS'
            },
            {
                id: 'test-3',
                patientName: 'Trần Thị E',
                doctorName: 'Bác sĩ Nguyễn Văn F',
                clinicName: 'Phòng khám Da liễu',
                date: '2024-01-14',
                shift: 'EVENING',
                status: 'COMPLETED'
            },
            {
                id: 'test-4',
                patientName: 'Phạm Văn G',
                doctorName: 'Bác sĩ Lê Thị H',
                clinicName: 'Phòng khám Tai mũi họng',
                date: '2024-01-17',
                shift: 'MORNING',
                status: 'CANCELLED'
            }
        ];
        
        console.log('🧪 Mock appointments:', mockAppointments);
        
        // Cập nhật appState với dữ liệu test
        window.appState.appointments = mockAppointments;
        window.appState.lastUpdate = new Date();
        
        console.log('🧪 Updated appState:', window.appState);
        
        // Cập nhật bảng
        window.AppointmentLoader.updateAllTables();
        
        console.log('🧪 Display test completed');
    };

    // Export appState để các file khác có thể truy cập
    window.appState = appState;
    window.AppointmentUtils = Utils;

    // Helper functions
    function getStatusColor(status) {
        const statusMap = {
            'đã lên lịch': 'warning',
            'Đã lên lịch': 'warning',
            'DA_LEN_LICH': 'warning',
            'đang khám': 'info',
            'Đang khám': 'info',
            'DANG_KHAM': 'info',
            'đã hoàn thành': 'success',
            'Đã hoàn thành': 'success',
            'DA_HOAN_THANH': 'success',
            'đã hủy': 'danger',
            'Đã hủy': 'danger',
            'DA_HUY': 'danger'
        };
        return statusMap[status] || 'secondary';
    }

    function formatCurrency(amount) {
        if (!amount || amount === 0) return 'Miễn phí';
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    }

    // Global functions cho các nút hành động
    window.viewAppointmentDetail = function(appointmentId) {
        console.log('🔍 Viewing appointment detail:', appointmentId);
        
        // Hiển thị loading
        const loadingHTML = `
            <div class="text-center">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <p class="mt-2">Đang tải thông tin chi tiết...</p>
            </div>
        `;
        
        // Tạo modal nếu chưa có
        if (!document.getElementById('appointmentDetailModal')) {
            const modalHTML = `
                <div class="modal fade" id="appointmentDetailModal" tabindex="-1">
                    <div class="modal-dialog modal-lg">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Chi tiết cuộc hẹn</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                            </div>
                            <div class="modal-body" id="appointmentDetailContent">
                                ${loadingHTML}
                            </div>
                        </div>
                    </div>
                </div>
            `;
            document.body.insertAdjacentHTML('beforeend', modalHTML);
        } else {
            // Hiển thị loading trong modal hiện có
            const content = document.getElementById('appointmentDetailContent');
            content.innerHTML = loadingHTML;
        }
        
        // Hiển thị modal
        const modal = new bootstrap.Modal(document.getElementById('appointmentDetailModal'));
        modal.show();
        
        // Gọi API để lấy thông tin chi tiết
        fetch(window.AppointmentAPI.appointments.detail(appointmentId))
            .then(response => {
                console.log('📡 Response status:', response.status);
                console.log('📡 Response headers:', response.headers);
                
                if (!response.ok) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }
                
                // Kiểm tra content-type
                const contentType = response.headers.get('content-type');
                console.log('📡 Content-Type:', contentType);
                
                if (!contentType || !contentType.includes('application/json')) {
                    // Nếu không phải JSON, đọc text để debug
                    return response.text().then(text => {
                        console.error('❌ Server trả về không phải JSON:', text);
                        throw new Error('Server trả về không phải JSON. Vui lòng kiểm tra API endpoint.');
                    });
                }
                
                return response.json();
            })
            .then(data => {
                console.log('📊 API Detail Response:', data);
                
                if (!data.success || !data.data) {
                    throw new Error(data.message || 'Không thể tải thông tin chi tiết');
                }
                
                const appointment = data.data;
                console.log('📊 Appointment detail data:', appointment);
                console.log('📊 Doctor phone:', appointment.doctorPhone);
                
                // Fill content với dữ liệu từ API chi tiết
                const content = document.getElementById('appointmentDetailContent');
                content.innerHTML = `
                    <div class="row">
                        <div class="col-md-6">
                            <h6 class="mb-3">
                                <i class="fas fa-user text-primary me-2"></i>
                                Thông tin bệnh nhân
                            </h6>
                            <div class="d-flex align-items-center mb-3">
                                <img src="${appointment.patientImage || appointment.patient?.image || './assets/images/table/10.png'}" 
                                     class="rounded-circle me-3" 
                                     width="60" 
                                     height="60"
                                     alt="Ảnh bệnh nhân"
                                     onerror="this.src='./assets/images/table/10.png'">
                                <div>
                                    <h6 class="mb-1">${appointment.name || appointment.patientName || 'N/A'}</h6>
                                    <small class="text-muted">Mã: ${appointment.patientId || 'N/A'}</small>
                                </div>
                            </div>
                            <p><strong>Số điện thoại:</strong> ${appointment.patientPhone || appointment.patient?.phone || appointment.phone || appointment.patientPhone || 'N/A'}</p>
                            <p><strong>Email:</strong> ${appointment.patientEmail || appointment.patient?.email || appointment.email || appointment.patientEmail || 'N/A'}</p>
                        </div>
                        <div class="col-md-6">
                            <h6 class="mb-3">
                                <i class="fas fa-user-md text-success me-2"></i>
                                Thông tin bác sĩ
                            </h6>
                            <div class="d-flex align-items-center mb-3">
                                <img src="${appointment.doctorImage || appointment.doctor?.image || './assets/images/table/10.png'}" 
                                     class="rounded-circle me-3" 
                                     width="60" 
                                     height="60"
                                     alt="Ảnh bác sĩ"
                                     onerror="this.src='./assets/images/table/10.png'">
                                <div>
                                    <h6 class="mb-1">${appointment.doctorName || 'N/A'}</h6>
                                    <small class="text-muted">${appointment.doctorSpecialty || appointment.doctor?.specialty || 'Chuyên khoa'}</small>
                                </div>
                            </div>
                            <p><strong>Phòng khám:</strong> ${appointment.clinic || appointment.clinicName || 'N/A'}</p>
                            <p><strong>Số điện thoại:</strong> ${appointment.doctorPhone || 'N/A'}</p>
                        </div>
                    </div>
                    <hr>
                    <div class="row">
                        <div class="col-md-6">
                            <h6 class="mb-3">
                                <i class="fas fa-calendar-alt text-info me-2"></i>
                                Thông tin lịch hẹn
                            </h6>
                            <p><strong>Ngày:</strong> ${new Date(appointment.date).toLocaleDateString('vi-VN')}</p>
                            <p><strong>Ca:</strong> ${appointment.shift || 'N/A'}</p>
                            <p><strong>Thời gian bắt đầu:</strong> ${appointment.startTime || appointment.timeStart || 'N/A'}</p>
                            <p><strong>Thời gian kết thúc:</strong> ${appointment.endTime || appointment.timeEnd || 'N/A'}</p>
                            <p><strong>Trạng thái:</strong> <span class="badge bg-${getStatusColor(appointment.status)}">${appointment.status}</span></p>
                        </div>
                        <div class="col-md-6">
                            <h6 class="mb-3">
                                <i class="fas fa-stethoscope text-warning me-2"></i>
                                Dịch vụ khám
                            </h6>
                            <p><strong>Tên dịch vụ:</strong> ${appointment.serviceName || appointment.service?.name || 'N/A'}</p>
                            <p><strong>Mô tả:</strong> ${appointment.serviceDescription || appointment.service?.description || 'N/A'}</p>
                            <hr>
                            <h6 class="mb-2">
                                <i class="fas fa-comment text-secondary me-2"></i>
                                Ghi chú
                            </h6>
                            <p class="text-muted">${appointment.note || 'Không có ghi chú'}</p>
                        </div>
                    </div>
                `;
            })
            .catch(error => {
                console.error('❌ Lỗi khi tải thông tin chi tiết:', error);
                const content = document.getElementById('appointmentDetailContent');
                content.innerHTML = `
                    <div class="alert alert-danger">
                        <i class="fas fa-exclamation-triangle me-2"></i>
                        Lỗi khi tải thông tin chi tiết: ${error.message}
                    </div>
                `;
            });
    };

    window.editAppointment = function(appointmentId) {
        console.log('✏️ Editing appointment:', appointmentId);
        alert('Chức năng chỉnh sửa cuộc hẹn sẽ được phát triển sau!');
    };

    window.startAppointment = function(appointmentId) {
        console.log('▶️ Starting appointment:', appointmentId);
        
        if (confirm('Bạn có chắc chắn muốn bắt đầu khám cho cuộc hẹn này?')) {
            // Gọi API để chuyển trạng thái sang "đang khám"
            fetch(`https://localhost:7097/api/appointment/start/${appointmentId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert('Bắt đầu khám thành công!');
                    // Reload appointments
                    if (window.AppointmentLoader) {
                        window.AppointmentLoader.loadAppointments();
                    }
                } else {
                    alert('Lỗi khi bắt đầu khám: ' + (data.message || 'Unknown error'));
                }
            })
            .catch(error => {
                console.error('Error starting appointment:', error);
                alert('Lỗi khi bắt đầu khám!');
            });
        }
    };

    window.cancelAppointment = function(appointmentId) {
        console.log('❌ Canceling appointment:', appointmentId);
        
        const reason = prompt('Lý do hủy cuộc hẹn (không bắt buộc):');
        if (reason === null) return; // User clicked Cancel
        
        if (confirm('Bạn có chắc chắn muốn hủy cuộc hẹn này?')) {
            // Gọi API để hủy appointment
            fetch(`https://localhost:7097/api/appointment/cancel/${appointmentId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    reason: reason || 'Không có lý do'
                })
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert('Hủy cuộc hẹn thành công!');
                    // Reload appointments
                    if (window.AppointmentLoader) {
                        window.AppointmentLoader.loadAppointments();
                    }
                } else {
                    alert('Lỗi khi hủy cuộc hẹn: ' + (data.message || 'Unknown error'));
                }
            })
            .catch(error => {
                console.error('Error canceling appointment:', error);
                alert('Lỗi khi hủy cuộc hẹn!');
            });
        }
    };

    window.completeAppointment = function(appointmentId) {
        console.log('✅ Completing appointment:', appointmentId);
        
        if (confirm('Bạn có chắc chắn muốn hoàn thành cuộc hẹn này?')) {
            // Gọi API để chuyển trạng thái sang "đã hoàn thành"
            fetch(`https://localhost:7097/api/appointment/complete/${appointmentId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    alert('Hoàn thành cuộc hẹn thành công!');
                    // Reload appointments
                    if (window.AppointmentLoader) {
                        window.AppointmentLoader.loadAppointments();
                    }
                } else {
                    alert('Lỗi khi hoàn thành cuộc hẹn: ' + (data.message || 'Unknown error'));
                }
            })
            .catch(error => {
                console.error('Error completing appointment:', error);
                alert('Lỗi khi hoàn thành cuộc hẹn!');
            });
        }
    };

    window.restoreAppointment = function(appointmentId) {
        console.log('🔄 Restoring appointment:', appointmentId);
        
        if (confirm('Bạn có chắc chắn muốn khôi phục cuộc hẹn này về trạng thái "Sắp tới"?')) {
            // Gọi API để khôi phục appointment
            fetch(`https://localhost:7097/api/appointment/restore/${appointmentId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                }
            })
            .then(response => response.json())
                            .then(data => {
                    if (data.success) {
                        let successMessage = `✅ ${data.message}\n\n`;
                        successMessage += `⏰ Thời gian khôi phục: ${data.restoredTime}\n`;
                        if (data.shiftType && data.shiftTime) {
                            successMessage += `📅 Ca làm việc: ${data.shiftType} (${data.shiftTime})`;
                        }
                        alert(successMessage);
                        // Reload appointments
                        if (window.AppointmentLoader) {
                            window.AppointmentLoader.loadAppointments();
                        }
                    } else {
                        let errorMessage = `❌ ${data.message}`;
                        
                        // Thêm thông tin chi tiết nếu có
                        if (data.currentTime && data.shiftStart && data.shiftEnd) {
                            errorMessage += `\n\n⏰ Thời gian hiện tại: ${data.currentTime}`;
                            errorMessage += `\n🕐 Ca làm việc: ${data.shiftStart} - ${data.shiftEnd}`;
                            errorMessage += `\n📅 Loại ca: ${data.shiftType}`;
                        }
                        
                        if (data.leaveReason) {
                            errorMessage += `\n📝 Lý do nghỉ phép: ${data.leaveReason}`;
                        }
                        
                        alert(errorMessage);
                    }
                })
            .catch(error => {
                console.error('Error restoring appointment:', error);
                alert('❌ Lỗi khi khôi phục cuộc hẹn!');
            });
        }
    };

    console.log('✅ Appointment Loader đã được khởi tạo');
    console.log('🔧 Mở browser console để xem debug logs');
    console.log('📤 Đã export các hàm: loadAppointments, filterAppointments, refreshAppointments');
    console.log('🧪 Test functions: testAppointmentLoader(), testDisplayWithMockData()');

})(); 