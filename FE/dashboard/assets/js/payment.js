// Payment Management JavaScript
class PaymentManager {
    constructor() {
        this.currentPage = 1;
        this.pageSize = 10;
        this.currentFilter = {
            searchTerm: '',
            paymentMethod: '',
            fromDate: '',
            toDate: '',
            sortBy: 'PaymentDate',
            sortOrder: 'desc'
        };
        this.currentEditPaymentId = null; // Store current editing payment ID
        this.init();
    }

    init() {
        this.loadPayments();
        this.bindEvents();
        this.checkPaymentStatus();
    }

    bindEvents() {
        // Search functionality
        const searchInput = document.getElementById('searchPayment');
        if (searchInput) {
            searchInput.addEventListener('input', (e) => {
                this.currentFilter.searchTerm = e.target.value;
                this.currentPage = 1;
                this.loadPayments();
            });
        }

        // Filter by payment method
        const methodFilter = document.getElementById('paymentMethodFilter');
        if (methodFilter) {
            methodFilter.addEventListener('change', (e) => {
                this.currentFilter.paymentMethod = e.target.value;
                this.currentPage = 1;
                this.loadPayments();
            });
        }

        // Date range filters
        const fromDateFilter = document.getElementById('fromDateFilter');
        if (fromDateFilter) {
            fromDateFilter.addEventListener('change', (e) => {
                this.currentFilter.fromDate = e.target.value;
                this.currentPage = 1;
                this.loadPayments();
            });
        }

        const toDateFilter = document.getElementById('toDateFilter');
        if (toDateFilter) {
            toDateFilter.addEventListener('change', (e) => {
                this.currentFilter.toDate = e.target.value;
                this.currentPage = 1;
                this.loadPayments();
            });
        }

        // Sort functionality
        const sortSelect = document.getElementById('sortBy');
        if (sortSelect) {
            sortSelect.addEventListener('change', (e) => {
                this.currentFilter.sortBy = e.target.value;
                this.currentPage = 1;
                this.loadPayments();
            });
        }

        const sortOrderSelect = document.getElementById('sortOrder');
        if (sortOrderSelect) {
            sortOrderSelect.addEventListener('change', (e) => {
                this.currentFilter.sortOrder = e.target.value;
                this.currentPage = 1;
                this.loadPayments();
            });
        }

        // Pagination
        document.addEventListener('click', (e) => {
            if (e.target.classList.contains('page-link')) {
                e.preventDefault();
                const page = parseInt(e.target.dataset.page);
                if (page && page !== this.currentPage) {
                    this.currentPage = page;
                    this.loadPayments();
                }
            }
        });

        // Edit payment
        document.addEventListener('click', (e) => {
            console.log('=== CLICK EVENT ===');
            console.log('Target:', e.target);
            console.log('Target classes:', e.target.classList);
            
            const editButton = e.target.closest('.edit-payment');
            console.log('Found edit button:', editButton);
            
            if (editButton && !editButton.disabled) {
                e.preventDefault();
                const paymentId = editButton.dataset.id;
                console.log('Edit button clicked for payment ID:', paymentId);
                console.log('Button dataset:', editButton.dataset);
                this.editPayment(paymentId);
            } else if (editButton && editButton.disabled) {
                e.preventDefault();
                this.showError('Không thể chỉnh sửa payment đã hoàn thành');
            }
        });

        // Delete payment
        document.addEventListener('click', (e) => {
            const deleteButton = e.target.closest('.delete-payment');
            if (deleteButton && !deleteButton.disabled) {
                e.preventDefault();
                const paymentId = deleteButton.dataset.id;
                this.deletePayment(paymentId);
            } else if (deleteButton && deleteButton.disabled) {
                e.preventDefault();
                this.showError('Không thể xóa payment đã hoàn thành');
            }
        });

        // Generate QR VNPay
        document.addEventListener('click', (e) => {
            const qrButton = e.target.closest('.generate-qr-vnpay');
            if (qrButton) {
                e.preventDefault();
                const paymentId = qrButton.dataset.id;
                const amount = qrButton.dataset.amount;
                const payer = qrButton.dataset.payer;
                const paymentCode = qrButton.dataset.code;
                this.generateVNPayQR(paymentId, amount, payer, paymentCode);
            }
        });

        // Add payment form submission
        const submitAddPaymentBtn = document.getElementById('submitAddPayment');
        if (submitAddPaymentBtn) {
            submitAddPaymentBtn.addEventListener('click', (e) => {
                e.preventDefault();
                this.submitAddPayment();
            });
        }

        // Edit payment form submission
        const submitEditPaymentBtn = document.getElementById('submitEditPayment');
        if (submitEditPaymentBtn) {
            submitEditPaymentBtn.addEventListener('click', (e) => {
                e.preventDefault();
                this.submitEditPayment();
            });
        }

        // Initialize date picker for add payment form
        this.initializeAddPaymentForm();
    }

    async loadPayments() {
        try {
            const params = new URLSearchParams({
                page: this.currentPage.toString(),
                pageSize: this.pageSize.toString(),
                ...this.currentFilter
            });

            const response = await fetch(`https://localhost:7097/api/Payment/filter?${params}`);
            if (!response.ok) {
                throw new Error('Failed to fetch payments');
            }

            const data = await response.json();
            this.renderPayments(data);
            this.renderPagination(data);
            this.updateSummary(data);
        } catch (error) {
            console.error('Error loading payments:', error);
            this.showError('Không thể tải dữ liệu thanh toán');
        }
    }

    renderPayments(data) {
        const tbody = document.querySelector('#paymentTableBody');
        if (!tbody) return;

        tbody.innerHTML = '';

        if (!data.payments || data.payments.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="7" class="text-center py-4">
                        <div class="text-muted">Không có dữ liệu thanh toán</div>
                    </td>
                </tr>
            `;
            return;
        }

        data.payments.forEach((payment, index) => {
            // Calculate sequence number based on current page
            const sequenceNumber = (data.page - 1) * data.pageSize + index + 1;
            
            console.log(`=== PAYMENT ${index + 1} ===`);
            console.log('Payment object:', payment);
            console.log('Payment ID:', payment.id);
            console.log('Payment ID type:', typeof payment.id);
            console.log('Payment payer:', payment.payer);
            console.log('Payment status:', payment.status);
            
            // Check if payment is completed (status = 1)
            const isCompleted = payment.status === 1;
            
            const row = document.createElement('tr');
            row.innerHTML = `
                <td class="text-center">
                    <span class="fw-bold">${sequenceNumber}</span>
                </td>
                <td>
                    <h6 class="mb-0">${payment.payer}</h6>
                </td>
                <td>${this.formatDate(payment.paymentDate)}</td>
                <td>${payment.paymentMethod}</td>
                <td>
                    <span class="badge bg-success-subtle text-success">
                        ${this.formatCurrency(payment.amount)}
                    </span>
                </td>
                <td>
                    <span class="badge ${this.getStatusBadgeClass(payment.status)}">
                        ${this.getStatusText(payment.status)}
                    </span>
                </td>
                <td>
                    <div class="d-flex gap-2">
                        ${!isCompleted ? `
                            <button class="btn btn-sm btn-outline-primary edit-payment" 
                                    data-id="${payment.id}"
                                    title="Chỉnh sửa">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button class="btn btn-sm btn-outline-danger delete-payment" 
                                    data-id="${payment.id}"
                                    title="Xóa">
                                <i class="fas fa-trash"></i>
                            </button>
                            ${payment.paymentMethod === 'Chuyển khoản' ? `
                                <button class="btn btn-sm btn-outline-success generate-qr-vnpay" 
                                        data-id="${payment.id}"
                                        data-amount="${payment.amount}"
                                        data-payer="${payment.payer}"
                                        data-code="${payment.code}"
                                        title="Tạo mã QR VNPay">
                                    <i class="fas fa-qrcode"></i>
                                </button>
                            ` : ''}
                        ` : `
                            <span class="text-muted small">Đã hoàn thành</span>
                        `}
                    </div>
                </td>
            `;
            tbody.appendChild(row);
        });
    }

    renderPagination(data) {
        const paginationContainer = document.querySelector('.pagination-container');
        if (!paginationContainer) return;

        const totalPages = data.totalPages;
        const currentPage = data.page;

        let paginationHTML = '<ul class="page-number m-0 p-0 list-unstyled d-flex gap-2">';

        // Previous button
        if (data.hasPreviousPage) {
            paginationHTML += `
                <li>
                    <a href="#" class="page-link text-center bg-primary-subtle text-primary rounded" 
                       data-page="${currentPage - 1}">Trước</a>
                </li>
            `;
        }

        // Page numbers
        const startPage = Math.max(1, currentPage - 2);
        const endPage = Math.min(totalPages, currentPage + 2);

        for (let i = startPage; i <= endPage; i++) {
            const isActive = i === currentPage;
            paginationHTML += `
                <li>
                    <a href="#" class="page-link text-center ${isActive ? 'bg-primary text-white' : 'bg-primary-subtle text-primary'} rounded" 
                       data-page="${i}">${i}</a>
                </li>
            `;
        }

        // Next button
        if (data.hasNextPage) {
            paginationHTML += `
                <li>
                    <a href="#" class="page-link text-center bg-primary-subtle text-primary rounded" 
                       data-page="${currentPage + 1}">Tiếp</a>
                </li>
            `;
        }

        paginationHTML += '</ul>';
        paginationContainer.innerHTML = paginationHTML;
    }

    updateSummary(data) {
        const totalPayments = data.totalCount;
        const totalAmount = data.payments.reduce((sum, payment) => sum + payment.amount, 0);

        // Update summary cards if they exist
        const totalPaymentsElement = document.getElementById('totalPayments');
        if (totalPaymentsElement) {
            totalPaymentsElement.textContent = totalPayments.toLocaleString();
        }

        const totalAmountElement = document.getElementById('totalAmount');
        if (totalAmountElement) {
            totalAmountElement.textContent = this.formatCurrency(totalAmount);
        }
    }

    // ===== EDIT PAYMENT FUNCTIONS =====
    
    async editPayment(paymentId) {
        try {
            console.log('=== EDIT PAYMENT START ===');
            console.log('Payment ID to edit:', paymentId);
            console.log('Payment ID type:', typeof paymentId);
            
            // Validate payment ID
            if (!paymentId || paymentId === 'undefined' || paymentId === 'null') {
                throw new Error('Payment ID không hợp lệ');
            }
            
            // Fetch payment data
            const response = await fetch(`https://localhost:7097/api/Payment/${paymentId}`);
            console.log('Fetch response status:', response.status);
            
            if (!response.ok) {
                const errorText = await response.text();
                console.error('Fetch error:', errorText);
                throw new Error(`HTTP ${response.status}: ${response.statusText}`);
            }
            
            const payment = await response.json();
            console.log('Payment data received:', payment);
            
            // Store payment ID for update
            console.log('Before storing - currentEditPaymentId:', this.currentEditPaymentId);
            this.currentEditPaymentId = payment.id;
            // Also store in localStorage as backup
            localStorage.setItem('currentEditPaymentId', payment.id.toString());
            console.log('After storing - currentEditPaymentId:', this.currentEditPaymentId);
            console.log('Payment ID from API:', payment.id);
            console.log('Payment ID in localStorage:', localStorage.getItem('currentEditPaymentId'));
            
            // Populate form with payment data
            this.populateEditForm(payment);
            
            // Show edit modal
            this.showEditModal();
            
            console.log('=== EDIT PAYMENT READY ===');
        } catch (error) {
            console.error('Edit payment error:', error);
            this.showError(`Không thể tải thông tin thanh toán: ${error.message}`);
        }
    }
    
    populateEditForm(payment) {
        console.log('Populating form with payment:', payment);
        
        // Get form elements
        const elements = {
            name: document.getElementById('Name'),
            method: document.getElementById('payment-method'),
            amount: document.getElementById('Amount'),
            status: document.getElementById('payment_id'),
            date: document.getElementById('datetime'),
            notes: document.getElementById('editNotes')
        };
        
        // Check if all elements exist
        const missingElements = Object.entries(elements)
            .filter(([key, element]) => !element)
            .map(([key]) => key);
            
        if (missingElements.length > 0) {
            console.error('Missing form elements:', missingElements);
            return;
        }
        
        // Populate form fields
        elements.name.value = payment.payer || '';
        elements.method.value = payment.paymentMethod || '';
        elements.status.value = payment.status?.toString() || '1';
        elements.notes.value = payment.notes || '';
        
        // Handle amount field (readonly)
        elements.amount.value = this.formatCurrency(payment.amount || 0);
        elements.amount.readOnly = true;
        elements.amount.style.backgroundColor = '#f8f9fa';
        elements.amount.style.cursor = 'not-allowed';
        elements.amount.dataset.originalAmount = (payment.amount || 0).toString();
        
                       // Handle date field (readonly, always current time)
               const now = new Date();
               const formattedDate = this.formatDate(now.toISOString());
               elements.date.value = formattedDate;
               elements.date.readOnly = true;
               elements.date.style.backgroundColor = '#f8f9fa';
               elements.date.style.cursor = 'not-allowed';
               console.log('Set date to:', formattedDate);
        
        console.log('Form populated successfully');
    }
    
    showEditModal() {
        const offcanvas = document.getElementById('offcanvasPaymentEdit');
        if (!offcanvas) {
            console.error('Edit offcanvas not found');
            return;
        }
        
        const bsOffcanvas = new bootstrap.Offcanvas(offcanvas);
        bsOffcanvas.show();
        console.log('Edit modal shown');
    }

    async deletePayment(paymentId) {
        if (!confirm('Bạn có chắc chắn muốn xóa thanh toán này?')) {
            return;
        }

        try {
            const response = await fetch(`https://localhost:7097/api/Payment/${paymentId}`, {
                method: 'DELETE'
            });

            if (!response.ok) {
                throw new Error('Failed to delete payment');
            }

            this.showSuccess('Xóa thanh toán thành công');
            this.loadPayments();
        } catch (error) {
            console.error('Error deleting payment:', error);
            this.showError('Không thể xóa thanh toán');
        }
    }

    initializeAddPaymentForm() {
        // Set current date
        const addPaymentDateInput = document.getElementById('addPaymentDate');
        if (addPaymentDateInput) {
            const today = new Date().toISOString().split('T')[0];
            addPaymentDateInput.value = today;
        }

        // Load appointments in progress
        this.loadAppointmentsInProgress();

        // Initialize select2 for appointment select
        const addAppointmentSelect = document.getElementById('addAppointmentSelect');
        if (addAppointmentSelect && typeof $ !== 'undefined' && $.fn.select2) {
            $(addAppointmentSelect).select2({
                placeholder: "Chọn cuộc hẹn đang khám",
                allowClear: true
            });

            // Handle appointment selection change
            $(addAppointmentSelect).on('change', (e) => {
                this.handleAppointmentSelection(e.target.value);
            });
        }

        // Initialize select2 for payment method
        const addPaymentMethodSelect = document.getElementById('addPaymentMethod');
        if (addPaymentMethodSelect && typeof $ !== 'undefined' && $.fn.select2) {
            $(addPaymentMethodSelect).select2({
                placeholder: "Chọn phương thức thanh toán",
                allowClear: true
            });
        }
    }

    async loadAppointmentsInProgress() {
        try {
            const response = await fetch('https://localhost:7097/api/Appointment/in-progress');
            if (!response.ok) {
                throw new Error('Failed to fetch appointments in progress');
            }

            const result = await response.json();
            
            if (result.success && result.data) {
                this.populateAppointmentSelect(result.data);
            } else {
                throw new Error(result.message || 'Không thể tải danh sách cuộc hẹn đang khám');
            }
        } catch (error) {
            console.error('Error loading appointments in progress:', error);
            this.showError('Không thể tải danh sách cuộc hẹn đang khám: ' + error.message);
        }
    }

    populateAppointmentSelect(appointments) {
        const select = document.getElementById('addAppointmentSelect');
        if (!select) return;

        // Clear existing options except the first one
        select.innerHTML = '<option value="">Chọn cuộc hẹn đang khám</option>';

        appointments.forEach(appointment => {
            const option = document.createElement('option');
            option.value = appointment.id;
            option.textContent = `${appointment.patientName} - ${appointment.serviceName} (${this.formatCurrency(appointment.servicePrice)})`;
            option.dataset.appointment = JSON.stringify(appointment);
            select.appendChild(option);
        });
    }

    handleAppointmentSelection(appointmentId) {
        if (!appointmentId) {
            // Clear all fields if no appointment selected
            document.getElementById('addPatientName').value = '';
            document.getElementById('addServiceName').value = '';
            document.getElementById('addAmount').value = '';
            return;
        }

        const select = document.getElementById('addAppointmentSelect');
        const selectedOption = select.querySelector(`option[value="${appointmentId}"]`);
        
        if (selectedOption && selectedOption.dataset.appointment) {
            const appointment = JSON.parse(selectedOption.dataset.appointment);
            
            // Populate fields
            document.getElementById('addPatientName').value = appointment.patientName;
            document.getElementById('addServiceName').value = appointment.serviceName;
            document.getElementById('addAmount').value = appointment.servicePrice;
        }
    }

    async submitAddPayment() {
        const form = document.getElementById('addPaymentForm');
        if (!form) return;

        // Get form data
        const formData = new FormData(form);
        const appointmentId = formData.get('appointmentId');
        const paymentMethod = formData.get('paymentMethod');
        const notes = formData.get('notes') || '';

        // Validate required fields
        if (!appointmentId || !paymentMethod) {
            this.showError('Vui lòng chọn cuộc hẹn và phương thức thanh toán');
            return;
        }

        try {
            const paymentData = {
                appointmentId: parseInt(appointmentId),
                paymentMethod: paymentMethod,
                notes: notes
            };

            const response = await fetch('https://localhost:7097/api/Payment/from-appointment', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(paymentData)
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Failed to add payment');
            }

            this.showSuccess('Thêm hóa đơn thành công');
            
            // Close the offcanvas
            const offcanvas = bootstrap.Offcanvas.getInstance(document.getElementById('offcanvasPaymentAdd'));
            if (offcanvas) {
                offcanvas.hide();
            }

            // Reset form
            form.reset();
            
            // Reload payments to show the new one
            this.loadPayments();
        } catch (error) {
            console.error('Error adding payment:', error);
            this.showError('Không thể thêm hóa đơn: ' + error.message);
        }
    }

    async submitEditPayment() {
        console.log('=== SUBMIT EDIT PAYMENT START ===');
        
        // Validate payment ID
        console.log('=== SUBMIT EDIT VALIDATION ===');
        console.log('currentEditPaymentId:', this.currentEditPaymentId);
        console.log('currentEditPaymentId type:', typeof this.currentEditPaymentId);
        
        // Try to get payment ID from localStorage if global variable is lost
        if (!this.currentEditPaymentId) {
            const storedId = localStorage.getItem('currentEditPaymentId');
            console.log('Retrieved from localStorage:', storedId);
            if (storedId) {
                this.currentEditPaymentId = parseInt(storedId);
                console.log('Restored currentEditPaymentId:', this.currentEditPaymentId);
            }
        }
        
        if (!this.currentEditPaymentId) {
            this.showError('Không tìm thấy ID thanh toán cần cập nhật');
            return;
        }
        
        console.log('Final payment ID to update:', this.currentEditPaymentId);
        
        // Get form
        const form = document.getElementById('editPaymentForm');
        if (!form) {
            this.showError('Không tìm thấy form chỉnh sửa');
            return;
        }
        
        // Get form data
        const formData = new FormData(form);
        
        const paymentData = {
            id: this.currentEditPaymentId,
            payer: formData.get('payer') || '',
            paymentMethod: formData.get('paymentMethod') || '',
            status: parseInt(formData.get('status')) || 1,
            paymentDate: new Date().toISOString(), // Backend sẽ override thành DateTime.Now
            amount: this.getOriginalAmount(),
            notes: formData.get('notes') || ''
        };
        
        console.log('Payment data to update:', paymentData);
        
        // Validate data
        if (!paymentData.payer || !paymentData.paymentMethod) {
            this.showError('Vui lòng điền đầy đủ thông tin bắt buộc');
            return;
        }
        
        if (paymentData.amount <= 0) {
            this.showError('Số tiền phải lớn hơn 0');
            return;
        }
        
        try {
            // Send update request
            const response = await fetch(`https://localhost:7097/api/Payment/${this.currentEditPaymentId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(paymentData)
            });
            
            console.log('Update response status:', response.status);
            
            if (!response.ok) {
                const errorText = await response.text();
                console.error('Update error response:', errorText);
                throw new Error(`Cập nhật thất bại: ${response.status} ${response.statusText}`);
            }
            
            // Success
            this.showSuccess('Cập nhật hóa đơn thành công');
            this.closeEditModal();
            this.loadPayments();
            
            console.log('=== SUBMIT EDIT PAYMENT SUCCESS ===');
            
        } catch (error) {
            console.error('Submit edit payment error:', error);
            this.showError(`Không thể cập nhật hóa đơn: ${error.message}`);
        }
    }
    
    getOriginalAmount() {
        const amountInput = document.getElementById('Amount');
        if (!amountInput) return 0;
        
        const originalAmount = amountInput.dataset.originalAmount;
        return parseFloat(originalAmount) || 0;
    }
    
    closeEditModal() {
        const offcanvas = document.getElementById('offcanvasPaymentEdit');
        if (!offcanvas) return;
        
        const bsOffcanvas = bootstrap.Offcanvas.getInstance(offcanvas);
        if (bsOffcanvas) {
            bsOffcanvas.hide();
        }
        
        // Reset current edit payment ID
        console.log('Before reset - currentEditPaymentId:', this.currentEditPaymentId);
        this.currentEditPaymentId = null;
        localStorage.removeItem('currentEditPaymentId');
        console.log('After reset - currentEditPaymentId:', this.currentEditPaymentId);
        console.log('localStorage cleared');
    }

    getStatusText(status) {
        switch (status) {
            case 0: return 'Đang chờ';
            case 1: return 'Hoàn thành';
            case 2: return 'Thất bại';
            case 3: return 'Hoàn tiền';
            default: return 'Không xác định';
        }
    }

    getStatusBadgeClass(status) {
        switch (status) {
            case 0: return 'bg-warning-subtle text-warning'; // Pending - màu vàng
            case 1: return 'bg-success-subtle text-success'; // Completed - màu xanh
            case 2: return 'bg-danger-subtle text-danger';   // Failed - màu đỏ
            case 3: return 'bg-info-subtle text-info';       // Refunded - màu xanh dương
            default: return 'bg-secondary-subtle text-secondary';
        }
    }

    formatDate(dateString) {
        const date = new Date(dateString);
        return date.toLocaleString('vi-VN', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            second: '2-digit'
        });
    }

    formatCurrency(amount) {
        return new Intl.NumberFormat('vi-VN', {
            style: 'currency',
            currency: 'VND'
        }).format(amount);
    }

    showSuccess(message) {
        // Implement success notification
        console.log('Success:', message);
        alert('Thành công: ' + message);
    }

    showError(message) {
        // Implement error notification
        console.error('Error:', message);
        alert('Lỗi: ' + message);
    }

    checkPaymentStatus() {
        console.log('=== CHECK PAYMENT STATUS ===');
        console.log('Current URL:', window.location.href);
        
        const urlParams = new URLSearchParams(window.location.search);
        const status = urlParams.get('status');
        
        console.log('Status from URL:', status);
        console.log('All URL params:', Object.fromEntries(urlParams.entries()));
        
        if (status === 'success') {
            const orderId = urlParams.get('orderId');
            const amount = urlParams.get('amount');
            const transactionId = urlParams.get('transactionId');
            
            console.log('Success params:', { orderId, amount, transactionId });
            
            const message = `Thanh toán thành công!\n\nMã giao dịch: ${orderId}\nSố tiền: ${this.formatCurrency(amount)}\nMã thanh toán: ${transactionId}`;
            this.showSuccess(message);
            
            // Clear URL parameters
            window.history.replaceState({}, document.title, window.location.pathname);
            
            // Reload payments to update status
            this.loadPayments();
        } else if (status === 'error') {
            const message = urlParams.get('message') || 'Thanh toán thất bại';
            console.log('Error message:', message);
            this.showError(message);
            
            // Clear URL parameters
            window.history.replaceState({}, document.title, window.location.pathname);
        } else {
            console.log('No status parameter found');
        }
    }

    async generateVNPayQR(paymentId, amount, payer, paymentCode) {
        try {
            console.log('=== GENERATE VNPAY QR START ===');
            console.log('Payment ID:', paymentId, '(Type:', typeof paymentId, ')');
            console.log('Payment Code:', paymentCode, '(Type:', typeof paymentCode, ')');
            console.log('Amount:', amount, '(Type:', typeof amount, ')');
            console.log('Payer:', payer, '(Type:', typeof payer, ')');

            const requestData = {
                paymentId: parseInt(paymentId),
                paymentCode: paymentCode,
                amount: parseFloat(amount),
                payer: payer,
                orderInfo: "string",
                orderType: "string"
            };

            console.log('Request data before JSON.stringify:', requestData);
            console.log('Request data after JSON.stringify:', JSON.stringify(requestData));
            console.log('Request data types:', {
                paymentId: typeof requestData.paymentId,
                paymentCode: typeof requestData.paymentCode,
                amount: typeof requestData.amount,
                payer: typeof requestData.payer,
                orderInfo: typeof requestData.orderInfo,
                orderType: typeof requestData.orderType
            });

            const response = await fetch('https://localhost:7097/api/Payment/vnpay-qr-code', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(requestData)
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`Tạo QR thất bại: ${response.status} ${response.statusText}`);
            }

            const result = await response.json();
            console.log('VNPay QR result:', result);

            if (!result.success) {
                throw new Error(result.message || 'Tạo thanh toán VNPay thất bại');
            }

            // Hiển thị thông tin thanh toán trong modal
            this.showVNPayQRModal(result);

            console.log('=== GENERATE VNPAY QR SUCCESS ===');

        } catch (error) {
            console.error('Generate VNPay QR error:', error);
            this.showError(`Không thể tạo mã QR VNPay: ${error.message}`);
        }
    }

    showVNPayQRModal(qrData) {
        // Tạo modal hiển thị thanh toán VNPay
        const modalHtml = `
            <div class="modal fade" id="vnpayQRModal" tabindex="-1" aria-labelledby="vnpayQRModalLabel" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="vnpayQRModalLabel">Thanh toán VNPay</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body text-center">
                            <div class="mb-4">
                                <i class="fas fa-credit-card fa-3x text-primary mb-3"></i>
                                <h6>Thông tin thanh toán</h6>
                            </div>
                            <div class="mb-3">
                                <p class="mb-2"><strong>Số tiền:</strong> ${this.formatCurrency(qrData.amount)}</p>
                                <p class="mb-2"><strong>Mã giao dịch:</strong> ${qrData.orderId || qrData.transactionId}</p>
                                <p class="mb-0"><strong>Thời gian:</strong> ${new Date().toLocaleString('vi-VN')}</p>
                            </div>
                            <div class="alert alert-info">
                                <i class="fas fa-info-circle"></i>
                                Click "Thanh toán trực tuyến" để chuyển đến trang VNPay
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
                            <button type="button" class="btn btn-primary" onclick="window.open('${qrData.paymentUrl}', '_blank')">
                                <i class="fas fa-external-link-alt"></i> Thanh toán trực tuyến
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Xóa modal cũ nếu có
        const existingModal = document.getElementById('vnpayQRModal');
        if (existingModal) {
            existingModal.remove();
        }

        // Thêm modal mới vào body
        document.body.insertAdjacentHTML('beforeend', modalHtml);

        // Hiển thị modal
        const modal = new bootstrap.Modal(document.getElementById('vnpayQRModal'));
        modal.show();
    }
}

// Initialize payment manager when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    new PaymentManager();
}); 