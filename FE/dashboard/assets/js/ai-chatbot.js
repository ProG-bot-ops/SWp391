// AI Chatbot for Hospital Appointment System
class AIChatbot {
    constructor() {
        this.conversationHistory = [];
        this.currentContext = null;
        this.userInfo = {};
        this.appointmentData = {};
        this.isProcessing = false;
        
        // Initialize chatbot
        this.init();
    }

    init() {
        this.displayWelcomeMessage();
        this.setupEventListeners();
        this.loadConversationHistory();
    }

    displayWelcomeMessage() {
        const welcomeMessage = {
            type: 'bot',
            content: `Xin chào! Tôi là AI Assistant của bệnh viện. Tôi có thể giúp bạn:

🔹 Đặt lịch khám bệnh
🔹 Kiểm tra lịch hẹn hiện tại
🔹 Hủy hoặc thay đổi lịch hẹn
🔹 Cung cấp thông tin bệnh viện
🔹 Hướng dẫn quy trình khám bệnh

Bạn có thể nhập tin nhắn hoặc chọn thao tác nhanh bên trái. Tôi sẽ hỗ trợ bạn 24/7! 😊`,
            quickReplies: [
                'Đặt lịch khám',
                'Kiểm tra lịch hẹn',
                'Thông tin bệnh viện',
                'Hướng dẫn quy trình'
            ],
            timestamp: new Date()
        };
        
        this.addMessage(welcomeMessage);
    }

    setupEventListeners() {
        // Handle Enter key in input
        document.getElementById('messageInput').addEventListener('keypress', (e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                this.sendMessage();
            }
        });

        // Auto-scroll to bottom when new messages are added
        const chatMessages = document.getElementById('chatMessages');
        const observer = new MutationObserver(() => {
            chatMessages.scrollTop = chatMessages.scrollHeight;
        });
        observer.observe(chatMessages, { childList: true });
    }

    async sendMessage() {
        const input = document.getElementById('messageInput');
        const message = input.value.trim();
        
        if (!message || this.isProcessing) return;
        
        // Add user message
        this.addMessage({
            type: 'user',
            content: message,
            timestamp: new Date()
        });
        
        input.value = '';
        this.isProcessing = true;
        
        // Show typing indicator
        this.showTypingIndicator();
        
        try {
            // Process message with AI
            const response = await this.processMessage(message);
            
            // Hide typing indicator
            this.hideTypingIndicator();
            
            // Add bot response
            this.addMessage(response);
            
        } catch (error) {
            console.error('Error processing message:', error);
            this.hideTypingIndicator();
            
            this.addMessage({
                type: 'bot',
                content: 'Xin lỗi, có lỗi xảy ra. Vui lòng thử lại sau.',
                timestamp: new Date()
            });
        }
        
        this.isProcessing = false;
    }

    async processMessage(message) {
        const lowerMessage = message.toLowerCase();
        
        // Intent recognition
        const intent = this.recognizeIntent(lowerMessage);
        
        // Process based on intent
        switch (intent) {
            case 'book_appointment':
                return await this.handleBookAppointment(message);
            case 'check_appointment':
                return await this.handleCheckAppointment(message);
            case 'cancel_appointment':
                return await this.handleCancelAppointment(message);
            case 'hospital_info':
                return this.handleHospitalInfo();
            case 'process_guide':
                return this.handleProcessGuide();
            case 'greeting':
                return this.handleGreeting();
            case 'help':
                return this.handleHelp();
            default:
                return this.handleUnknownIntent(message);
        }
    }

    recognizeIntent(message) {
        const intents = {
            book_appointment: [
                'đặt lịch', 'đặt hẹn', 'khám bệnh', 'lịch khám', 'đặt lịch khám',
                'book appointment', 'schedule', 'appointment'
            ],
            check_appointment: [
                'kiểm tra', 'xem lịch', 'lịch hẹn', 'appointment status',
                'check appointment', 'my appointment'
            ],
            cancel_appointment: [
                'hủy lịch', 'hủy hẹn', 'cancel appointment', 'cancel booking'
            ],
            hospital_info: [
                'thông tin', 'bệnh viện', 'địa chỉ', 'số điện thoại',
                'hospital info', 'contact', 'address'
            ],
            process_guide: [
                'quy trình', 'hướng dẫn', 'cách khám', 'process', 'guide'
            ],
            greeting: [
                'xin chào', 'hello', 'hi', 'chào', 'good morning', 'good afternoon'
            ],
            help: [
                'giúp đỡ', 'help', 'hỗ trợ', 'support'
            ]
        };

        for (const [intent, keywords] of Object.entries(intents)) {
            if (keywords.some(keyword => message.includes(keyword))) {
                return intent;
            }
        }
        
        return 'unknown';
    }

    async handleBookAppointment(message) {
        if (!this.appointmentData.step) {
            this.appointmentData = { step: 'name' };
            return {
                type: 'bot',
                content: 'Tôi sẽ giúp bạn đặt lịch khám. Để bắt đầu, vui lòng cho tôi biết họ tên của bạn:',
                timestamp: new Date()
            };
        }

        // Extract information from message
        const extractedInfo = this.extractAppointmentInfo(message);
        
        if (this.appointmentData.step === 'name') {
            this.appointmentData.name = extractedInfo.name || message;
            this.appointmentData.step = 'phone';
            return {
                type: 'bot',
                content: `Cảm ơn ${this.appointmentData.name}! Vui lòng cung cấp số điện thoại của bạn:`,
                timestamp: new Date()
            };
        }
        
        if (this.appointmentData.step === 'phone') {
            this.appointmentData.phone = extractedInfo.phone || message;
            this.appointmentData.step = 'department';
            return {
                type: 'bot',
                content: 'Bạn muốn khám khoa nào? Dưới đây là các khoa hiện có:',
                quickReplies: [
                    'Khoa Tim mạch',
                    'Khoa Nhi',
                    'Khoa Da liễu',
                    'Khoa Mắt',
                    'Khoa Tai mũi họng',
                    'Khoa Nội tổng quát'
                ],
                timestamp: new Date()
            };
        }
        
        if (this.appointmentData.step === 'department') {
            this.appointmentData.department = message;
            this.appointmentData.step = 'date';
            return {
                type: 'bot',
                content: 'Bạn muốn đặt lịch vào ngày nào? (Vui lòng nhập theo định dạng DD/MM/YYYY):',
                timestamp: new Date()
            };
        }
        
        if (this.appointmentData.step === 'date') {
            const date = this.parseDate(message);
            if (!date) {
                return {
                    type: 'bot',
                    content: 'Vui lòng nhập ngày theo định dạng DD/MM/YYYY (ví dụ: 25/12/2024):',
                    timestamp: new Date()
                };
            }
            this.appointmentData.date = date;
            this.appointmentData.step = 'time';
            return {
                type: 'bot',
                content: 'Bạn muốn khám vào giờ nào?',
                quickReplies: [
                    '8:00 - 9:00',
                    '9:00 - 10:00',
                    '10:00 - 11:00',
                    '14:00 - 15:00',
                    '15:00 - 16:00',
                    '16:00 - 17:00'
                ],
                timestamp: new Date()
            };
        }
        
        if (this.appointmentData.step === 'time') {
            this.appointmentData.time = message;
            this.appointmentData.step = 'confirm';
            
            return {
                type: 'bot',
                content: `Vui lòng xác nhận thông tin đặt lịch:

👤 **Họ tên:** ${this.appointmentData.name}
📞 **Số điện thoại:** ${this.appointmentData.phone}
🏥 **Khoa:** ${this.appointmentData.department}
📅 **Ngày:** ${this.appointmentData.date}
⏰ **Giờ:** ${this.appointmentData.time}

Thông tin trên có chính xác không?`,
                quickReplies: ['Xác nhận', 'Sửa lại'],
                timestamp: new Date()
            };
        }
        
        if (this.appointmentData.step === 'confirm') {
            if (message.toLowerCase().includes('xác nhận') || message.toLowerCase().includes('đúng')) {
                return await this.submitAppointment();
            } else {
                this.appointmentData = { step: 'name' };
                return {
                    type: 'bot',
                    content: 'Vui lòng bắt đầu lại. Cho tôi biết họ tên của bạn:',
                    timestamp: new Date()
                };
            }
        }
    }

    async submitAppointment() {
        try {
            // Show loading
            this.showLoading();
            
            // Prepare appointment data
            const appointmentData = {
                patientName: this.appointmentData.name,
                phoneNumber: this.appointmentData.phone,
                department: this.appointmentData.department,
                appointmentDate: this.appointmentData.date,
                appointmentTime: this.appointmentData.time,
                status: 'Pending'
            };
            
            // Call API to create appointment
            const response = await this.createAppointmentAPI(appointmentData);
            
            this.hideLoading();
            
            if (response.success) {
                this.appointmentData = {};
                return {
                    type: 'bot',
                    content: `✅ **Đặt lịch thành công!**

Mã lịch hẹn: **${response.appointmentCode}**
Ngày khám: ${appointmentData.appointmentDate}
Giờ khám: ${appointmentData.appointmentTime}

Vui lòng đến bệnh viện trước 15 phút so với giờ hẹn. Mang theo CMND/CCCD và bảo hiểm y tế (nếu có).

Cảm ơn bạn đã sử dụng dịch vụ của chúng tôi! 🏥`,
                    timestamp: new Date()
                };
            } else {
                throw new Error(response.message);
            }
            
        } catch (error) {
            this.hideLoading();
            return {
                type: 'bot',
                content: `❌ **Đặt lịch thất bại!**

Lỗi: ${error.message}

Vui lòng thử lại hoặc liên hệ hotline 1900-xxxx để được hỗ trợ.`,
                timestamp: new Date()
            };
        }
    }

    async handleCheckAppointment(message) {
        // Extract phone number or appointment code
        const phoneOrCode = this.extractPhoneOrCode(message);
        
        if (!phoneOrCode) {
            return {
                type: 'bot',
                content: 'Vui lòng cung cấp số điện thoại hoặc mã lịch hẹn để kiểm tra:',
                timestamp: new Date()
            };
        }
        
        try {
            this.showLoading();
            const appointments = await this.getAppointmentsAPI(phoneOrCode);
            this.hideLoading();
            
            if (appointments.length === 0) {
                return {
                    type: 'bot',
                    content: 'Không tìm thấy lịch hẹn nào với thông tin bạn cung cấp. Vui lòng kiểm tra lại.',
                    timestamp: new Date()
                };
            }
            
            let response = '📋 **Lịch hẹn của bạn:**\n\n';
            appointments.forEach((apt, index) => {
                response += `${index + 1}. **Mã:** ${apt.appointmentCode}\n`;
                response += `   **Ngày:** ${apt.appointmentDate}\n`;
                response += `   **Giờ:** ${apt.appointmentTime}\n`;
                response += `   **Khoa:** ${apt.department}\n`;
                response += `   **Trạng thái:** ${this.getStatusText(apt.status)}\n\n`;
            });
            
            return {
                type: 'bot',
                content: response,
                quickReplies: ['Hủy lịch hẹn', 'Đặt lịch mới'],
                timestamp: new Date()
            };
            
        } catch (error) {
            this.hideLoading();
            return {
                type: 'bot',
                content: 'Có lỗi xảy ra khi kiểm tra lịch hẹn. Vui lòng thử lại sau.',
                timestamp: new Date()
            };
        }
    }

    async handleCancelAppointment(message) {
        const appointmentCode = this.extractAppointmentCode(message);
        
        if (!appointmentCode) {
            return {
                type: 'bot',
                content: 'Vui lòng cung cấp mã lịch hẹn để hủy:',
                timestamp: new Date()
            };
        }
        
        try {
            this.showLoading();
            const result = await this.cancelAppointmentAPI(appointmentCode);
            this.hideLoading();
            
            if (result.success) {
                return {
                    type: 'bot',
                    content: `✅ **Hủy lịch hẹn thành công!**

Mã lịch hẹn: ${appointmentCode}
Đã được hủy thành công.

Bạn có thể đặt lịch mới bất cứ lúc nào.`,
                    timestamp: new Date()
                };
            } else {
                throw new Error(result.message);
            }
            
        } catch (error) {
            this.hideLoading();
            return {
                type: 'bot',
                content: `❌ **Hủy lịch hẹn thất bại!**

Lỗi: ${error.message}

Vui lòng kiểm tra lại mã lịch hẹn hoặc liên hệ hotline để được hỗ trợ.`,
                timestamp: new Date()
            };
        }
    }

    handleHospitalInfo() {
        return {
            type: 'bot',
            content: `🏥 **Thông tin Bệnh viện**

📍 **Địa chỉ:** 123 Đường ABC, Quận 1, TP.HCM
📞 **Hotline:** 1900-xxxx
📧 **Email:** info@hospital.com
🌐 **Website:** www.hospital.com

⏰ **Giờ làm việc:**
- Thứ 2 - Thứ 6: 7:00 - 18:00
- Thứ 7: 7:00 - 12:00
- Chủ nhật: Nghỉ

🚗 **Phương tiện:**
- Xe buýt: Tuyến 01, 02, 03
- Taxi: Có bãi đỗ xe
- Xe máy: Có bãi giữ xe

🅿️ **Bãi đỗ xe:** Miễn phí cho bệnh nhân`,
            timestamp: new Date()
        };
    }

    handleProcessGuide() {
        return {
            type: 'bot',
            content: `📋 **Quy trình khám bệnh:**

1️⃣ **Đặt lịch hẹn**
   - Qua chatbot này
   - Gọi hotline 1900-xxxx
   - Đến trực tiếp bệnh viện

2️⃣ **Chuẩn bị**
   - Mang CMND/CCCD
   - Bảo hiểm y tế (nếu có)
   - Sổ khám bệnh cũ (nếu có)

3️⃣ **Đến bệnh viện**
   - Đến trước 15 phút
   - Làm thủ tục đăng ký
   - Thanh toán phí khám

4️⃣ **Khám bệnh**
   - Được gọi vào phòng khám
   - Bác sĩ khám và tư vấn
   - Nhận đơn thuốc (nếu cần)

5️⃣ **Kết thúc**
   - Thanh toán thuốc (nếu có)
   - Nhận hẹn tái khám (nếu cần)

❓ **Cần hỗ trợ thêm?** Gọi hotline hoặc chat với tôi!`,
            timestamp: new Date()
        };
    }

    handleGreeting() {
        const greetings = [
            'Xin chào! Tôi có thể giúp gì cho bạn? 😊',
            'Chào bạn! Bạn cần hỗ trợ gì về đặt lịch khám không?',
            'Xin chào! Tôi sẵn sàng hỗ trợ bạn đặt lịch khám bệnh!'
        ];
        
        return {
            type: 'bot',
            content: greetings[Math.floor(Math.random() * greetings.length)],
            quickReplies: ['Đặt lịch khám', 'Kiểm tra lịch hẹn', 'Thông tin bệnh viện'],
            timestamp: new Date()
        };
    }

    handleHelp() {
        return {
            type: 'bot',
            content: `🔧 **Tôi có thể giúp bạn:**

📅 **Đặt lịch khám:** Tôi sẽ hướng dẫn từng bước
🔍 **Kiểm tra lịch hẹn:** Xem lịch hẹn hiện tại
❌ **Hủy lịch hẹn:** Hủy lịch hẹn không cần thiết
🏥 **Thông tin bệnh viện:** Địa chỉ, giờ làm việc
📋 **Quy trình khám:** Hướng dẫn chi tiết

Bạn muốn làm gì? Chọn từ menu hoặc nhập tin nhắn!`,
            quickReplies: ['Đặt lịch khám', 'Kiểm tra lịch hẹn', 'Thông tin bệnh viện'],
            timestamp: new Date()
        };
    }

    handleUnknownIntent(message) {
        return {
            type: 'bot',
            content: `Xin lỗi, tôi chưa hiểu rõ yêu cầu của bạn. 

Bạn có thể:
- Đặt lịch khám
- Kiểm tra lịch hẹn
- Hủy lịch hẹn
- Xem thông tin bệnh viện
- Tìm hiểu quy trình khám

Hoặc gõ "giúp đỡ" để xem tất cả tính năng!`,
            quickReplies: ['Đặt lịch khám', 'Kiểm tra lịch hẹn', 'Giúp đỡ'],
            timestamp: new Date()
        };
    }

    // Utility functions
    extractAppointmentInfo(message) {
        const info = {};
        
        // Extract name (simple heuristic)
        if (message.length > 2 && message.length < 50) {
            info.name = message.trim();
        }
        
        // Extract phone number
        const phoneRegex = /(\d{10,11})/;
        const phoneMatch = message.match(phoneRegex);
        if (phoneMatch) {
            info.phone = phoneMatch[1];
        }
        
        return info;
    }

    extractPhoneOrCode(message) {
        // Extract phone number
        const phoneRegex = /(\d{10,11})/;
        const phoneMatch = message.match(phoneRegex);
        if (phoneMatch) {
            return phoneMatch[1];
        }
        
        // Extract appointment code (assuming format like APT-XXXX)
        const codeRegex = /(APT-\d{4})/i;
        const codeMatch = message.match(codeRegex);
        if (codeMatch) {
            return codeMatch[1];
        }
        
        return null;
    }

    extractAppointmentCode(message) {
        const codeRegex = /(APT-\d{4})/i;
        const codeMatch = message.match(codeRegex);
        return codeMatch ? codeMatch[1] : null;
    }

    parseDate(dateString) {
        const dateRegex = /(\d{1,2})\/(\d{1,2})\/(\d{4})/;
        const match = dateString.match(dateRegex);
        
        if (match) {
            const day = parseInt(match[1]);
            const month = parseInt(match[2]) - 1;
            const year = parseInt(match[3]);
            
            const date = new Date(year, month, day);
            const today = new Date();
            
            // Check if date is valid and in the future
            if (date > today && date.getDate() === day && date.getMonth() === month && date.getFullYear() === year) {
                return `${day.toString().padStart(2, '0')}/${(month + 1).toString().padStart(2, '0')}/${year}`;
            }
        }
        
        return null;
    }

    getStatusText(status) {
        const statusMap = {
            'Pending': 'Chờ xác nhận',
            'Confirmed': 'Đã xác nhận',
            'Completed': 'Đã hoàn thành',
            'Cancelled': 'Đã hủy'
        };
        return statusMap[status] || status;
    }

    // API functions (mock for now, can be replaced with real API calls)
    async createAppointmentAPI(data) {
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 2000));
        
        // Mock response
        return {
            success: true,
            appointmentCode: 'APT-' + Math.random().toString(36).substr(2, 4).toUpperCase(),
            message: 'Appointment created successfully'
        };
    }

    async getAppointmentsAPI(phoneOrCode) {
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 1500));
        
        // Mock response
        return [
            {
                appointmentCode: 'APT-1234',
                appointmentDate: '25/12/2024',
                appointmentTime: '9:00 - 10:00',
                department: 'Khoa Tim mạch',
                status: 'Confirmed'
            }
        ];
    }

    async cancelAppointmentAPI(appointmentCode) {
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        // Mock response
        return {
            success: true,
            message: 'Appointment cancelled successfully'
        };
    }

    // UI functions
    addMessage(message) {
        const chatMessages = document.getElementById('chatMessages');
        const messageElement = this.createMessageElement(message);
        chatMessages.appendChild(messageElement);
        
        // Save to conversation history
        this.conversationHistory.push(message);
        this.saveConversationHistory();
    }

    createMessageElement(message) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `message ${message.type}`;
        
        const avatar = document.createElement('div');
        avatar.className = 'message-avatar';
        avatar.innerHTML = message.type === 'user' ? '<i class="fas fa-user"></i>' : '<i class="fas fa-robot"></i>';
        
        const content = document.createElement('div');
        content.className = 'message-content';
        content.innerHTML = message.content;
        
        const time = document.createElement('div');
        time.className = 'message-time';
        time.textContent = this.formatTime(message.timestamp);
        
        messageDiv.appendChild(avatar);
        messageDiv.appendChild(content);
        messageDiv.appendChild(time);
        
        // Add quick replies if available
        if (message.quickReplies && message.quickReplies.length > 0) {
            const quickReplies = document.createElement('div');
            quickReplies.className = 'quick-replies';
            
            message.quickReplies.forEach(reply => {
                const button = document.createElement('button');
                button.className = 'quick-reply-btn';
                button.textContent = reply;
                button.onclick = () => this.handleQuickReply(reply);
                quickReplies.appendChild(button);
            });
            
            content.appendChild(quickReplies);
        }
        
        return messageDiv;
    }

    handleQuickReply(reply) {
        document.getElementById('messageInput').value = reply;
        this.sendMessage();
    }

    formatTime(timestamp) {
        return timestamp.toLocaleTimeString('vi-VN', { 
            hour: '2-digit', 
            minute: '2-digit' 
        });
    }

    showTypingIndicator() {
        document.getElementById('typingIndicator').style.display = 'flex';
    }

    hideTypingIndicator() {
        document.getElementById('typingIndicator').style.display = 'none';
    }

    showLoading() {
        const modal = new bootstrap.Modal(document.getElementById('loadingModal'));
        modal.show();
    }

    hideLoading() {
        const modal = bootstrap.Modal.getInstance(document.getElementById('loadingModal'));
        if (modal) {
            modal.hide();
        }
    }

    saveConversationHistory() {
        localStorage.setItem('chatHistory', JSON.stringify(this.conversationHistory));
    }

    loadConversationHistory() {
        const saved = localStorage.getItem('chatHistory');
        if (saved) {
            this.conversationHistory = JSON.parse(saved);
        }
    }

    clearChat() {
        document.getElementById('chatMessages').innerHTML = '';
        this.conversationHistory = [];
        localStorage.removeItem('chatHistory');
        this.displayWelcomeMessage();
    }

    exportChat() {
        const chatText = this.conversationHistory.map(msg => 
            `${msg.type === 'user' ? 'Bạn' : 'AI'}: ${msg.content}`
        ).join('\n\n');
        
        const blob = new Blob([chatText], { type: 'text/plain' });
        const url = URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `chat-export-${new Date().toISOString().split('T')[0]}.txt`;
        a.click();
        URL.revokeObjectURL(url);
    }
}

// Global functions for HTML onclick events
let chatbot;

function quickAction(action) {
    if (!chatbot) return;
    
    const actions = {
        'book': 'Tôi muốn đặt lịch khám',
        'check': 'Tôi muốn kiểm tra lịch hẹn',
        'cancel': 'Tôi muốn hủy lịch hẹn',
        'info': 'Thông tin bệnh viện'
    };
    
    if (actions[action]) {
        document.getElementById('messageInput').value = actions[action];
        chatbot.sendMessage();
    }
}

function sendMessage() {
    if (chatbot) {
        chatbot.sendMessage();
    }
}

function handleKeyPress(event) {
    if (event.key === 'Enter' && !event.shiftKey) {
        event.preventDefault();
        sendMessage();
    }
}

function clearChat() {
    if (chatbot) {
        chatbot.clearChat();
    }
}

function exportChat() {
    if (chatbot) {
        chatbot.exportChat();
    }
}

// Initialize chatbot when page loads
document.addEventListener('DOMContentLoaded', () => {
    chatbot = new AIChatbot();
}); 