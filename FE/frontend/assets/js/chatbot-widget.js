// AI Chatbot Widget for G-Care Clinic
class ChatbotWidget {
    constructor() {
        this.isOpen = false;
        this.conversationHistory = [];
        this.appointmentData = {};
        this.isProcessing = false;
        
        this.init();
    }

    init() {
        this.createWidget();
        this.setupEventListeners();
        this.displayWelcomeMessage();
        this.showNotificationBadge();
    }

    createWidget() {
        // Create widget HTML
        const widgetHTML = `
            <div class="chatbot-widget">
                <button class="chatbot-toggle" id="chatbotToggle">
                    <i class="fas fa-comments"></i>
                    <div class="notification-badge" id="notificationBadge">1</div>
                </button>
                
                <div class="chatbot-container" id="chatbotContainer">
                    <div class="chatbot-header">
                        <h5>
                            <i class="fas fa-robot"></i>
                            AI Assistant
                            <span class="status">Online</span>
                        </h5>
                        <button class="close-btn" id="chatbotClose">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                    
                    <div class="chatbot-messages" id="chatbotMessages">
                        <!-- Messages will be added here -->
                    </div>
                    
                    <div class="chatbot-typing" id="chatbotTyping">
                        <span>AI đang nhập...</span>
                        <div class="chatbot-typing-dots">
                            <span></span>
                            <span></span>
                            <span></span>
                        </div>
                    </div>
                    
                    <div class="chatbot-input">
                        <div class="chatbot-input-group">
                            <input type="text" id="chatbotInput" placeholder="Nhập tin nhắn của bạn...">
                            <button id="chatbotSend">
                                <i class="fas fa-paper-plane"></i>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        // Append to body
        document.body.insertAdjacentHTML('beforeend', widgetHTML);
    }

    setupEventListeners() {
        // Toggle chatbot
        document.getElementById('chatbotToggle').addEventListener('click', () => {
            this.toggleChatbot();
        });
        
        // Close chatbot
        document.getElementById('chatbotClose').addEventListener('click', () => {
            this.closeChatbot();
        });
        
        // Send message
        document.getElementById('chatbotSend').addEventListener('click', () => {
            this.sendMessage();
        });
        
        // Enter key
        document.getElementById('chatbotInput').addEventListener('keypress', (e) => {
            if (e.key === 'Enter' && !e.shiftKey) {
                e.preventDefault();
                this.sendMessage();
            }
        });
        
        // Auto-scroll to bottom
        const messagesContainer = document.getElementById('chatbotMessages');
        const observer = new MutationObserver(() => {
            messagesContainer.scrollTop = messagesContainer.scrollHeight;
        });
        observer.observe(messagesContainer, { childList: true });
    }

    toggleChatbot() {
        const container = document.getElementById('chatbotContainer');
        const badge = document.getElementById('notificationBadge');
        
        if (this.isOpen) {
            this.closeChatbot();
        } else {
            this.openChatbot();
            // Hide notification badge when opened
            badge.style.display = 'none';
        }
    }

    openChatbot() {
        const container = document.getElementById('chatbotContainer');
        container.classList.add('active');
        this.isOpen = true;
        
        // Focus on input
        setTimeout(() => {
            document.getElementById('chatbotInput').focus();
        }, 300);
    }

    closeChatbot() {
        const container = document.getElementById('chatbotContainer');
        container.classList.remove('active');
        this.isOpen = false;
    }

    showNotificationBadge() {
        // Show notification badge after 3 seconds
        setTimeout(() => {
            const badge = document.getElementById('notificationBadge');
            badge.style.display = 'flex';
        }, 3000);
    }

    displayWelcomeMessage() {
        const welcomeMessage = {
            type: 'bot',
            content: `Xin chào! Tôi là AI Assistant của G-Care Clinic. Tôi có thể giúp bạn:

🔹 Đặt lịch khám bệnh
🔹 Tìm hiểu về dịch vụ
🔹 Thông tin bác sĩ
🔹 Hướng dẫn quy trình khám

Bạn cần hỗ trợ gì? 😊`,
            quickReplies: [
                'Đặt lịch khám',
                'Thông tin dịch vụ',
                'Danh sách bác sĩ',
                'Liên hệ'
            ],
            timestamp: new Date()
        };
        
        this.addMessage(welcomeMessage);
    }

    async sendMessage() {
        const input = document.getElementById('chatbotInput');
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
            case 'service_info':
                return this.handleServiceInfo();
            case 'doctor_info':
                return this.handleDoctorInfo();
            case 'contact_info':
                return this.handleContactInfo();
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
            service_info: [
                'dịch vụ', 'thông tin dịch vụ', 'services', 'treatment'
            ],
            doctor_info: [
                'bác sĩ', 'doctor', 'danh sách bác sĩ', 'doctor list'
            ],
            contact_info: [
                'liên hệ', 'contact', 'địa chỉ', 'số điện thoại', 'address'
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
                content: 'Tôi sẽ giúp bạn đặt lịch khám tại G-Care Clinic. Để bắt đầu, vui lòng cho tôi biết họ tên của bạn:',
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
                content: 'Bạn muốn khám khoa nào? Dưới đây là các khoa hiện có tại G-Care Clinic:',
                quickReplies: [
                    'Khoa Răng Hàm Mặt',
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
            // Prepare appointment data
            const appointmentData = {
                patientName: this.appointmentData.name,
                phoneNumber: this.appointmentData.phone,
                department: this.appointmentData.department,
                appointmentDate: this.appointmentData.date,
                appointmentTime: this.appointmentData.time,
                status: 'Pending'
            };
            
            // Call API to create appointment (mock for now)
            const response = await this.createAppointmentAPI(appointmentData);
            
            if (response.success) {
                this.appointmentData = {};
                return {
                    type: 'bot',
                    content: `✅ **Đặt lịch thành công!**

Mã lịch hẹn: **${response.appointmentCode}**
Ngày khám: ${appointmentData.appointmentDate}
Giờ khám: ${appointmentData.appointmentTime}

Vui lòng đến G-Care Clinic trước 15 phút so với giờ hẹn. Mang theo CMND/CCCD và bảo hiểm y tế (nếu có).

Cảm ơn bạn đã chọn G-Care Clinic! 🏥`,
                    timestamp: new Date()
                };
            } else {
                throw new Error(response.message);
            }
            
        } catch (error) {
            return {
                type: 'bot',
                content: `❌ **Đặt lịch thất bại!**

Lỗi: ${error.message}

Vui lòng thử lại hoặc liên hệ hotline 0862502458 để được hỗ trợ.`,
                timestamp: new Date()
            };
        }
    }

    handleServiceInfo() {
        return {
            type: 'bot',
            content: `🏥 **Dịch vụ tại G-Care Clinic:**

🦷 **Khoa Răng Hàm Mặt:**
- Khám và điều trị răng miệng
- Cấy ghép Implant
- Chỉnh nha thẩm mỹ
- Tẩy trắng răng

👶 **Khoa Nhi:**
- Khám sức khỏe trẻ em
- Tiêm chủng
- Tư vấn dinh dưỡng

👁️ **Khoa Mắt:**
- Khám mắt tổng quát
- Đo thị lực
- Điều trị các bệnh về mắt

👂 **Khoa Tai Mũi Họng:**
- Khám và điều trị các bệnh tai mũi họng
- Nội soi tai mũi họng

🩺 **Khoa Nội tổng quát:**
- Khám sức khỏe tổng quát
- Tư vấn sức khỏe

Bạn quan tâm đến dịch vụ nào?`,
            quickReplies: ['Đặt lịch khám', 'Thông tin bác sĩ', 'Liên hệ'],
            timestamp: new Date()
        };
    }

    handleDoctorInfo() {
        return {
            type: 'bot',
            content: `👨‍⚕️ **Đội ngũ bác sĩ G-Care Clinic:**

**Dr. Vũ Văn Long**
- Chuyên khoa: Răng Hàm Mặt
- Kinh nghiệm: 12+ năm
- Chuyên môn: Điều trị răng miệng cho trẻ em và người lớn

**Dr. Nguyễn Minh Đức**
- Chuyên khoa: Răng Hàm Mặt
- Tốt nghiệp loại Giỏi Đại học Y Hà Nội
- Chứng chỉ cấy ghép Implant quốc tế

**Dr. Dương Minh Toản**
- Chuyên khoa: Răng Hàm Mặt
- Đã phục hình thành công 500+ ca răng thẩm mỹ

**Dr. Nguyễn Khác Tráng**
- Chuyên khoa: Răng Hàm Mặt
- 15+ năm kinh nghiệm tại các bệnh viện lớn

**Dr. Nguyễn Minh Anh**
- Chuyên khoa: Răng Hàm Mặt
- Giảng viên Đại học Y Hà Nội

Bạn muốn đặt lịch với bác sĩ nào?`,
            quickReplies: ['Đặt lịch khám', 'Thông tin dịch vụ', 'Liên hệ'],
            timestamp: new Date()
        };
    }

    handleContactInfo() {
        return {
            type: 'bot',
            content: `📞 **Thông tin liên hệ G-Care Clinic:**

📍 **Địa chỉ:** FPT University Hòa Lạc
📧 **Email:** nguyenducgiangqp@gmail.com
📱 **Hotline:** 0862502458

⏰ **Giờ làm việc:**
- Thứ 2 - Thứ 6: 8:00 - 18:00
- Thứ 7: 8:00 - 12:00
- Chủ nhật: Nghỉ

🚗 **Phương tiện:**
- Xe buýt: Tuyến đến FPT University
- Taxi: Có bãi đỗ xe
- Xe máy: Có bãi giữ xe

🅿️ **Bãi đỗ xe:** Miễn phí cho bệnh nhân

Bạn cần hỗ trợ thêm gì không?`,
            quickReplies: ['Đặt lịch khám', 'Thông tin dịch vụ', 'Danh sách bác sĩ'],
            timestamp: new Date()
        };
    }

    handleGreeting() {
        const greetings = [
            'Xin chào! Tôi có thể giúp gì cho bạn tại G-Care Clinic? 😊',
            'Chào bạn! Bạn cần hỗ trợ gì về dịch vụ khám chữa bệnh không?',
            'Xin chào! Tôi sẵn sàng hỗ trợ bạn đặt lịch khám tại G-Care Clinic!'
        ];
        
        return {
            type: 'bot',
            content: greetings[Math.floor(Math.random() * greetings.length)],
            quickReplies: ['Đặt lịch khám', 'Thông tin dịch vụ', 'Danh sách bác sĩ'],
            timestamp: new Date()
        };
    }

    handleHelp() {
        return {
            type: 'bot',
            content: `🔧 **Tôi có thể giúp bạn:**

📅 **Đặt lịch khám:** Hướng dẫn từng bước đặt lịch
🏥 **Thông tin dịch vụ:** Giới thiệu các khoa và dịch vụ
👨‍⚕️ **Danh sách bác sĩ:** Thông tin đội ngũ bác sĩ
📞 **Liên hệ:** Địa chỉ, số điện thoại, giờ làm việc

Bạn muốn làm gì? Chọn từ menu hoặc nhập tin nhắn!`,
            quickReplies: ['Đặt lịch khám', 'Thông tin dịch vụ', 'Danh sách bác sĩ'],
            timestamp: new Date()
        };
    }

    handleUnknownIntent(message) {
        return {
            type: 'bot',
            content: `Xin lỗi, tôi chưa hiểu rõ yêu cầu của bạn. 

Tại G-Care Clinic, bạn có thể:
- Đặt lịch khám bệnh
- Tìm hiểu thông tin dịch vụ
- Xem danh sách bác sĩ
- Liên hệ với chúng tôi

Hoặc gõ "giúp đỡ" để xem tất cả tính năng!`,
            quickReplies: ['Đặt lịch khám', 'Thông tin dịch vụ', 'Giúp đỡ'],
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

    // API functions (mock for now)
    async createAppointmentAPI(data) {
        // Simulate API call
        await new Promise(resolve => setTimeout(resolve, 2000));
        
        // Mock response
        return {
            success: true,
            appointmentCode: 'GC-' + Math.random().toString(36).substr(2, 4).toUpperCase(),
            message: 'Appointment created successfully'
        };
    }

    // UI functions
    addMessage(message) {
        const messagesContainer = document.getElementById('chatbotMessages');
        const messageElement = this.createMessageElement(message);
        messagesContainer.appendChild(messageElement);
        
        // Save to conversation history
        this.conversationHistory.push(message);
    }

    createMessageElement(message) {
        const messageDiv = document.createElement('div');
        messageDiv.className = `chatbot-message ${message.type}`;
        
        const content = document.createElement('div');
        content.className = 'chatbot-message-content';
        content.innerHTML = message.content;
        
        const time = document.createElement('div');
        time.className = 'chatbot-message-time';
        time.textContent = this.formatTime(message.timestamp);
        
        messageDiv.appendChild(content);
        messageDiv.appendChild(time);
        
        // Add quick replies if available
        if (message.quickReplies && message.quickReplies.length > 0) {
            const quickReplies = document.createElement('div');
            quickReplies.className = 'chatbot-quick-replies';
            
            message.quickReplies.forEach(reply => {
                const button = document.createElement('button');
                button.className = 'chatbot-quick-reply';
                button.textContent = reply;
                button.onclick = () => this.handleQuickReply(reply);
                quickReplies.appendChild(button);
            });
            
            content.appendChild(quickReplies);
        }
        
        return messageDiv;
    }

    handleQuickReply(reply) {
        document.getElementById('chatbotInput').value = reply;
        this.sendMessage();
    }

    formatTime(timestamp) {
        return timestamp.toLocaleTimeString('vi-VN', { 
            hour: '2-digit', 
            minute: '2-digit' 
        });
    }

    showTypingIndicator() {
        document.getElementById('chatbotTyping').classList.add('active');
    }

    hideTypingIndicator() {
        document.getElementById('chatbotTyping').classList.remove('active');
    }
}

// Initialize chatbot when page loads
document.addEventListener('DOMContentLoaded', () => {
    new ChatbotWidget();
}); 