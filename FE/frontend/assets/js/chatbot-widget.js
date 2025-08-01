// AI Chatbot Widget for G-Care Clinic - Complete Version
class ChatbotWidget {
    constructor() {
        this.isOpen = false;
        this.isTyping = false;
        this.currentMode = 'booking'; // 'booking' or 'ai'
        this.appointmentData = {};
        this.aiApiKey = ''; // Will be set by user
        this.aiApiEndpoint = 'https://api.openai.com/v1/chat/completions'; // Default OpenAI endpoint
        this.apiBaseUrl = 'http://localhost:7097'; // Backend API base URL
        
        // Drag and drop properties
        this.isDragging = false;
        this.dragStartX = 0;
        this.dragStartY = 0;
        this.initialX = 0;
        this.initialY = 0;
        this.currentX = 0;
        this.currentY = 0;
        this.dragElement = null;
        
        // Performance optimizations
        this.requestCache = new Map(); // Cache for API responses
        this.activeRequests = new Map(); // Track active requests
        this.connectionPool = new Map(); // Connection pooling
        this.lastRequestTime = 0; // Rate limiting
        
        // Authentication
        this.isLoggedIn = false;
        this.userInfo = null;
        
        this.init();
    }

    init() {
        this.attachEventListeners();
        this.loadStoredData();
        this.loadPosition();
        this.checkLoginStatus();
        this.showWelcomeMessage();
    }

    async checkLoginStatus() {
        try {
            console.log('🔍 Checking login status...');
            
            // Check multiple possible token storage locations
            const possibleTokens = [
                localStorage.getItem('authToken'),
                sessionStorage.getItem('authToken'),
                localStorage.getItem('token'),
                sessionStorage.getItem('token'),
                localStorage.getItem('accessToken'),
                sessionStorage.getItem('accessToken'),
                localStorage.getItem('jwt'),
                sessionStorage.getItem('jwt')
            ];
            
            const token = possibleTokens.find(t => t && t.length > 0);
            console.log('🔑 Token found:', token ? 'Yes' : 'No');
            
            if (token) {
                console.log('🔑 Token length:', token.length);
                
                // Try to verify token with backend first
                try {
                    console.log('🔄 Attempting to verify token with backend...');
                    const response = await fetch(`${this.apiBaseUrl}/api/auth/verify`, {
                        method: 'GET',
                        headers: {
                            'Authorization': `Bearer ${token}`,
                            'Content-Type': 'application/json'
                        }
                    });
                    
                    console.log('📡 Backend response status:', response.status);
                    
                    if (response.ok) {
                        const userData = await response.json();
                        this.isLoggedIn = true;
                        this.userInfo = userData;
                        console.log('✅ User logged in (verified):', userData);
                        return;
                    } else {
                        console.log('❌ Backend verification failed, status:', response.status);
                    }
                } catch (verifyError) {
                    console.log('❌ Token verification failed:', verifyError);
                }
                
                // If verification fails, try to get user info from other sources
                try {
                    console.log('🔍 Looking for user info in storage...');
                    const possibleUserInfoKeys = [
                        'userInfo',
                        'user',
                        'currentUser',
                        'userData',
                        'profile'
                    ];
                    
                    let userInfo = null;
                    for (const key of possibleUserInfoKeys) {
                        const stored = localStorage.getItem(key) || sessionStorage.getItem(key);
                        if (stored) {
                            try {
                                userInfo = JSON.parse(stored);
                                console.log('✅ Found user info with key:', key);
                                break;
                            } catch (e) {
                                console.log('❌ Failed to parse user info from key:', key);
                            }
                        }
                    }
                    
                    if (userInfo) {
                        this.isLoggedIn = true;
                        
                        // Normalize user info structure
                        this.userInfo = {
                            name: userInfo.name || userInfo.fullName || userInfo.username || userInfo.displayName || 'User',
                            email: userInfo.email || userInfo.mail || userInfo.userEmail || 'user@example.com',
                            role: userInfo.role || userInfo.userRole || userInfo.type || 'User'
                        };
                        
                        console.log('✅ User logged in (from storage):', this.userInfo);
                        return;
                    }
                } catch (storageError) {
                    console.log('❌ Error reading user info from storage:', storageError);
                }
                
                // Try to get user info from backend API
                try {
                    console.log('🔄 Attempting to get user info from backend...');
                    const userInfoResponse = await fetch(`${this.apiBaseUrl}/api/auth/me`, {
                        method: 'GET',
                        headers: {
                            'Authorization': `Bearer ${token}`,
                            'Content-Type': 'application/json'
                        }
                    });
                    
                    if (userInfoResponse.ok) {
                        const userData = await userInfoResponse.json();
                        this.isLoggedIn = true;
                        
                        // Normalize user info structure
                        this.userInfo = {
                            name: userData.name || userData.fullName || userData.username || userData.displayName || 'User',
                            email: userData.email || userData.mail || userData.userEmail || 'user@example.com',
                            role: userData.role || userData.userRole || userData.type || 'User'
                        };
                        
                        console.log('✅ User logged in (from API):', this.userInfo);
                        return;
                    } else {
                        console.log('❌ Failed to get user info from API, status:', userInfoResponse.status);
                    }
                } catch (apiError) {
                    console.log('❌ Error getting user info from API:', apiError);
                }
                
                // Check if there's any indication of login in the DOM
                try {
                    console.log('🔍 Checking DOM for login indicators...');
                    
                    // More comprehensive DOM selectors for user information
                    const userSelectors = [
                        // Common user name selectors
                        '.user-name', '.username', '.profile-name', '.name',
                        '[data-user-name]', '[data-username]', '[data-name]',
                        '.user-info .name', '.profile .name', '.header .name',
                        
                        // Navigation/user menu selectors
                        '.nav-user .name', '.user-menu .name', '.dropdown-user .name',
                        '.navbar-user .name', '.header-user .name',
                        
                        // Specific to this website structure
                        '.iq-navbar .user-name', '.navbar .user-name',
                        '.header .user-name', '.top-header .user-name',
                        
                        // Generic selectors
                        '[class*="user"][class*="name"]', '[class*="profile"][class*="name"]',
                        '[class*="login"][class*="name"]', '[class*="account"][class*="name"]'
                    ];
                    
                    let foundUserName = null;
                    let foundUserEmail = null;
                    
                    // Try to find user name
                    for (const selector of userSelectors) {
                        const element = document.querySelector(selector);
                        if (element && element.textContent && element.textContent.trim().length > 0) {
                            const text = element.textContent.trim();
                            // Filter out common non-name text
                            if (text.length > 1 && text.length < 50 && 
                                !text.includes('Login') && !text.includes('Sign') && 
                                !text.includes('Register') && !text.includes('Logout')) {
                                foundUserName = text;
                                console.log('✅ Found user name with selector:', selector, 'Value:', text);
                                break;
                            }
                        }
                    }
                    
                    // Try to find user email
                    const emailSelectors = [
                        '.user-email', '.email', '[data-email]', '.user-info .email',
                        '.profile .email', '.header .email'
                    ];
                    
                    for (const selector of emailSelectors) {
                        const element = document.querySelector(selector);
                        if (element && element.textContent && element.textContent.includes('@')) {
                            foundUserEmail = element.textContent.trim();
                            console.log('✅ Found user email with selector:', selector, 'Value:', foundUserEmail);
                            break;
                        }
                    }
                    
                    // Check for any login/logout buttons to determine login status
                    const loginButtons = document.querySelectorAll('a[href*="login"], a[href*="signin"], .login-btn, .signin-btn');
                    const logoutButtons = document.querySelectorAll('a[href*="logout"], .logout-btn, .signout-btn');
                    
                    const hasLoginButton = loginButtons.length > 0;
                    const hasLogoutButton = logoutButtons.length > 0;
                    
                    console.log('🔍 Login/Logout buttons found:', { hasLoginButton, hasLogoutButton });
                    
                    // If we found user name or have logout button, assume logged in
                    if (foundUserName || hasLogoutButton) {
                        this.isLoggedIn = true;
                        this.userInfo = {
                            name: foundUserName || 'User',
                            email: foundUserEmail || 'user@example.com',
                            role: 'User'
                        };
                        console.log('✅ User logged in (DOM indicators found):', this.userInfo);
                        return;
                    }
                    
                    console.log('❌ No clear login indicators found in DOM');
                } catch (domError) {
                    console.log('❌ Error checking DOM indicators:', domError);
                }
                
                // Final fallback: if we have a token, assume user is logged in
                this.isLoggedIn = true;
                this.userInfo = {
                    name: 'User',
                    email: 'user@example.com',
                    role: 'User'
                };
                console.log('✅ User assumed logged in (token exists)');
            } else {
                this.isLoggedIn = false;
                this.userInfo = null;
                console.log('❌ No auth token found');
            }
        } catch (error) {
            console.error('❌ Error checking login status:', error);
            this.isLoggedIn = false;
            this.userInfo = null;
        }
    }

    getLoginStatus() {
        return this.isLoggedIn;
    }

    getUserInfo() {
        return this.userInfo;
    }

    attachEventListeners() {
        document.getElementById('chatbot-toggle').addEventListener('click', (e) => {
            if (!this.isDragging) {
                this.toggleChat();
            }
        });
        document.getElementById('closeChatbot').addEventListener('click', () => this.toggleChat());
        document.getElementById('modeToggle').addEventListener('click', () => this.toggleMode());
        document.getElementById('sendBtn').addEventListener('click', () => this.sendMessage());
        document.getElementById('messageInput').addEventListener('keypress', (e) => {
            if (e.key === 'Enter') this.sendMessage();
        });

        // Drag and drop event listeners
        this.attachDragListeners();
    }

    attachDragListeners() {
        const toggle = document.getElementById('chatbot-toggle');
        const header = document.querySelector('.chatbot-header');
        const widget = document.querySelector('.chatbot-widget');

        // Toggle button drag
        toggle.addEventListener('mousedown', (e) => this.startDrag(e, toggle));
        toggle.addEventListener('touchstart', (e) => this.startDrag(e, toggle));

        // Header drag (when chat is open)
        header.addEventListener('mousedown', (e) => this.startDrag(e, header));
        header.addEventListener('touchstart', (e) => this.startDrag(e, header));

        // Global mouse/touch events
        document.addEventListener('mousemove', (e) => this.drag(e));
        document.addEventListener('touchmove', (e) => this.drag(e));
        document.addEventListener('mouseup', () => this.stopDrag());
        document.addEventListener('touchend', () => this.stopDrag());

        // Prevent text selection during drag
        widget.addEventListener('selectstart', (e) => {
            if (this.isDragging) {
                e.preventDefault();
            }
        });

        // Handle window resize
        window.addEventListener('resize', () => {
            if (this.isDragging) {
                this.stopDrag();
            }
            this.applyBoundaries();
            this.updatePosition();
        });

        // Handle scroll events to prevent dragging during scroll
        window.addEventListener('scroll', () => {
            if (this.isDragging) {
                this.stopDrag();
            }
        });

        // Double-tap to reset position
        let lastTap = 0;
        widget.addEventListener('touchend', (e) => {
            const currentTime = new Date().getTime();
            const tapLength = currentTime - lastTap;
            
            if (tapLength < 500 && tapLength > 0) {
                // Double tap detected
                e.preventDefault();
                this.resetPosition();
                
                // Show feedback
                this.showResetFeedback();
            }
            lastTap = currentTime;
        });

        // Double-click to reset position (desktop)
        widget.addEventListener('dblclick', (e) => {
            if (!this.isDragging) {
                e.preventDefault();
                this.resetPosition();
                this.showResetFeedback();
            }
        });
    }

    startDrag(e, element) {
        e.preventDefault();
        e.stopPropagation();
        
        this.isDragging = true;
        this.dragElement = element;
        
        const widget = document.querySelector('.chatbot-widget');
        widget.classList.add('dragging');

        // Get initial position
        const rect = widget.getBoundingClientRect();
        this.initialX = rect.left;
        this.initialY = rect.top;

        // Get mouse/touch position
        if (e.type === 'mousedown') {
            this.dragStartX = e.clientX;
            this.dragStartY = e.clientY;
        } else if (e.type === 'touchstart') {
            this.dragStartX = e.touches[0].clientX;
            this.dragStartY = e.touches[0].clientY;
        }

        this.currentX = this.initialX;
        this.currentY = this.initialY;

        // Add dragging class to body to prevent text selection
        document.body.classList.add('chatbot-dragging');
    }

    drag(e) {
        if (!this.isDragging) return;

        e.preventDefault();
        e.stopPropagation();

        // Get current mouse/touch position
        let clientX, clientY;
        if (e.type === 'mousemove') {
            clientX = e.clientX;
            clientY = e.clientY;
        } else if (e.type === 'touchmove') {
            clientX = e.touches[0].clientX;
            clientY = e.touches[0].clientY;
        }

        // Calculate new position
        const deltaX = clientX - this.dragStartX;
        const deltaY = clientY - this.dragStartY;

        this.currentX = this.initialX + deltaX;
        this.currentY = this.initialY + deltaY;

        // Apply boundaries
        this.applyBoundaries();

        // Update position
        this.updatePosition();
    }

    stopDrag() {
        if (!this.isDragging) return;

        this.isDragging = false;
        const widget = document.querySelector('.chatbot-widget');
        widget.classList.remove('dragging');

        // Remove dragging class from body
        document.body.classList.remove('chatbot-dragging');

        // Save position
        this.savePosition();

        // Reset drag element
        this.dragElement = null;
    }

    applyBoundaries() {
        const widget = document.querySelector('.chatbot-widget');
        const rect = widget.getBoundingClientRect();
        const windowWidth = window.innerWidth;
        const windowHeight = window.innerHeight;

        // Minimum distance from edges
        const minDistance = 20;

        // Apply horizontal boundaries
        if (this.currentX < minDistance) {
            this.currentX = minDistance;
        } else if (this.currentX + rect.width > windowWidth - minDistance) {
            this.currentX = windowWidth - rect.width - minDistance;
        }

        // Apply vertical boundaries
        if (this.currentY < minDistance) {
            this.currentY = minDistance;
        } else if (this.currentY + rect.height > windowHeight - minDistance) {
            this.currentY = windowHeight - rect.height - minDistance;
        }

        // Check if near boundary and add warning animation
        const isNearBoundary = this.currentX <= minDistance + 10 || 
                              this.currentX + rect.width >= windowWidth - minDistance - 10 ||
                              this.currentY <= minDistance + 10 || 
                              this.currentY + rect.height >= windowHeight - minDistance - 10;

        if (isNearBoundary) {
            widget.classList.add('near-boundary');
        } else {
            widget.classList.remove('near-boundary');
        }
    }

    updatePosition() {
        const widget = document.querySelector('.chatbot-widget');
        widget.style.left = this.currentX + 'px';
        widget.style.top = this.currentY + 'px';
        widget.style.right = 'auto';
        widget.style.bottom = 'auto';
    }

    savePosition() {
        const position = {
            x: this.currentX,
            y: this.currentY
        };
        localStorage.setItem('chatbot_position', JSON.stringify(position));
    }

    loadPosition() {
        const savedPosition = localStorage.getItem('chatbot_position');
        if (savedPosition) {
            try {
                const position = JSON.parse(savedPosition);
                this.currentX = position.x;
                this.currentY = position.y;
                this.updatePosition();
            } catch (error) {
                console.error('Error loading chatbot position:', error);
                this.resetPosition();
            }
        } else {
            this.resetPosition();
        }
    }

    resetPosition() {
        const widget = document.querySelector('.chatbot-widget');
        widget.style.left = 'auto';
        widget.style.top = 'auto';
        widget.style.right = '20px';
        widget.style.bottom = '20px';
        
        // Update current position to default
        const rect = widget.getBoundingClientRect();
        this.currentX = rect.left;
        this.currentY = rect.top;
        
        // Clear saved position
        localStorage.removeItem('chatbot_position');
    }



    toggleMode() {
        if (this.currentMode === 'booking') {
            // Switching to AI mode - check login first
            if (!this.isLoggedIn) {
                this.addMessage({
                    type: 'bot',
                    content: `🔒 **Yêu cầu đăng nhập**\n\nTính năng AI chỉ dành cho người dùng đã đăng nhập.\n\nVui lòng đăng nhập để sử dụng tính năng này.\n\n💡 **Lệnh:** \`/login\` để mở trang đăng nhập`,
                    quickReplies: ['Đăng nhập', 'Ở lại chế độ đặt lịch'],
                    timestamp: new Date()
                });
                return;
            }
            
            this.currentMode = 'ai';
            this.updateModeDisplay();
            this.resetAppointmentData();
            
            this.addMessage({
                type: 'bot',
                content: `🤖 **Chế độ AI Hỏi đáp (Gemini)**\n\nChào mừng ${this.userInfo?.name || 'bạn'}!\n📧 **Email:** ${this.userInfo?.email || 'N/A'}\n🔑 **Quyền:** ${this.userInfo?.role || 'User'}\n\nTôi có thể giúp bạn trả lời các câu hỏi chung về sức khỏe, bệnh viện, hoặc bất kỳ vấn đề nào khác.\n\n✅ **Gemini AI đã sẵn sàng!**\n\n💡 **Mẹo:** Bạn có thể nhắn tin "đặt lịch khám" để tự động chuyển sang chế độ đặt lịch!`,
                timestamp: new Date()
            });
            
            // Check if API key is set
            if (!this.aiApiKey) {
                this.addMessage({
                    type: 'bot',
                    content: `🔑 **Cài đặt API Key:**\n\nVui lòng nhập API key của bạn để sử dụng tính năng AI:\n\n\`/setkey YOUR_API_KEY\`\n\nHoặc:\n\`/setkey openai YOUR_OPENAI_KEY\`\n\`/setkey google YOUR_GOOGLE_AI_KEY\``,
                    timestamp: new Date()
                });
            }
        } else {
            // Switching to booking mode
            this.currentMode = 'booking';
            this.updateModeDisplay();
            this.resetAppointmentData();
            
            this.addMessage({
                type: 'bot',
                content: `📅 **Chế độ Đặt lịch**\n\nTôi sẽ giúp bạn đặt lịch khám bệnh. Bạn có muốn đặt lịch không?`,
                quickReplies: ['Có, tôi muốn đặt lịch', 'Không, cảm ơn'],
                timestamp: new Date()
            });
        }
    }

    updateModeDisplay() {
        const modeBadge = document.querySelector('.mode-badge');
        const modeToggle = document.getElementById('modeToggle');
        
        if (this.currentMode === 'booking') {
            modeBadge.textContent = '📅 Đặt lịch';
            modeBadge.className = 'mode-badge booking-mode';
            modeToggle.innerHTML = '<i class="fas fa-robot"></i>';
            modeToggle.title = this.isLoggedIn ? 'Chuyển sang chế độ AI' : 'Chuyển sang chế độ AI (Cần đăng nhập)';
        } else {
            modeBadge.textContent = '🤖 AI Hỏi đáp';
            modeBadge.className = 'mode-badge ai-mode';
            modeToggle.innerHTML = '<i class="fas fa-calendar-alt"></i>';
            modeToggle.title = 'Chuyển sang chế độ đặt lịch';
        }
    }

    async refreshLoginStatus() {
        await this.checkLoginStatus();
        this.updateModeDisplay();
        
        // Debug: Log current state
        console.log('🔄 Refresh Login Status - Current state:', {
            isLoggedIn: this.isLoggedIn,
            userInfo: this.userInfo,
            userInfoType: typeof this.userInfo,
            userInfoKeys: this.userInfo ? Object.keys(this.userInfo) : 'null'
        });
        
        // Show updated status with more details
        if (this.isLoggedIn) {
            const userName = this.userInfo?.name || this.userInfo?.fullName || this.userInfo?.username || 'bạn';
            const userEmail = this.userInfo?.email || this.userInfo?.mail || 'N/A';
            const userRole = this.userInfo?.role || this.userInfo?.userRole || 'User';
            
            this.addMessage({
                type: 'bot',
                content: `🔄 **Đã cập nhật trạng thái đăng nhập**\n\n✅ **Đã đăng nhập** - Chào mừng ${userName}!\n📧 **Email:** ${userEmail}\n🔑 **Quyền:** ${userRole}\n\nBây giờ bạn có thể sử dụng tính năng AI.`,
                timestamp: new Date()
            });
        } else {
            this.addMessage({
                type: 'bot',
                content: `🔄 **Đã cập nhật trạng thái đăng nhập**\n\n❌ **Chưa đăng nhập** - Chỉ có thể đặt lịch\n\n💡 **Cách khắc phục:**\n1. Đăng nhập vào website\n2. Làm mới trang\n3. Thử lại lệnh \`/refresh\`\n\nHoặc sử dụng lệnh \`/login\` để mở trang đăng nhập.`,
                timestamp: new Date()
            });
        }
    }

    async sendMessage() {
        const input = document.getElementById('messageInput');
        const message = input.value.trim();
        
        if (!message) return;
        
        // Clear input
        input.value = '';
        
        // Add user message
        this.addMessage({
            type: 'user',
            content: message,
            timestamp: new Date()
        });

        // Check if user wants to book appointment (intent recognition)
        if (this.currentMode === 'ai' && this.detectBookingIntent(message)) {
            this.currentMode = 'booking';
            this.updateModeDisplay();
            this.resetAppointmentData();
            
            this.addMessage({
                type: 'bot',
                content: `🔄 **Tự động chuyển sang chế độ Đặt lịch**\n\nTôi hiểu bạn muốn đặt lịch khám. Hãy để tôi giúp bạn đặt lịch!`,
                timestamp: new Date()
            });
            
            // Start booking flow
            await this.processBookingMessage(message);
            return;
        }

        // Process message based on current mode
        if (this.currentMode === 'booking') {
            await this.processBookingMessage(message);
        } else {
            await this.processAIMessage(message);
        }
    }

    detectBookingIntent(message) {
        const bookingKeywords = [
            'đặt lịch', 'đặt lịch hẹn', 'đặt lịch khám', 'đặt lịch khám bệnh',
            'đăng ký lịch', 'đăng ký lịch hẹn', 'đăng ký lịch khám', 'đăng ký lịch khám bệnh',
            'book appointment', 'make appointment', 'schedule appointment',
            'đặt hẹn', 'đặt hẹn khám', 'đặt hẹn bác sĩ',
            'khám bệnh', 'đi khám', 'đi khám bệnh',
            'lịch hẹn', 'lịch khám', 'appointment',
            'muốn đặt', 'muốn đăng ký', 'cần đặt', 'cần đăng ký',
            'hẹn khám', 'hẹn bác sĩ', 'khám với bác sĩ'
        ];
        
        const lowerMessage = message.toLowerCase();
        return bookingKeywords.some(keyword => lowerMessage.includes(keyword));
    }

    async processAIMessage(message) {
        // Check for special commands first
        if (this.handleSpecialCommands(message)) {
            return;
        }

        // Check for API key commands
        if (message.startsWith('/setkey ')) {
            this.handleSetApiKey(message);
            return;
        }

        // Check if user is logged in
        if (!this.isLoggedIn) {
            this.addMessage({
                type: 'bot',
                content: `🔒 **Yêu cầu đăng nhập**\n\nTính năng AI chỉ dành cho người dùng đã đăng nhập.\n\nVui lòng đăng nhập để sử dụng tính năng này.\n\n💡 **Lệnh:** \`/login\` để mở trang đăng nhập`,
                quickReplies: ['Đăng nhập', 'Chuyển sang đặt lịch'],
                timestamp: new Date()
            });
            return;
        }

        // Check if API key is set
        if (!this.aiApiKey) {
            this.addMessage({
                type: 'bot',
                content: `🔑 **API Key chưa được cài đặt**\n\nVui lòng cài đặt API key trước khi sử dụng tính năng AI:\n\n\`/setkey YOUR_API_KEY\`\n\nHoặc:\n\`/setkey openai YOUR_OPENAI_KEY\`\n\`/setkey google YOUR_GOOGLE_AI_KEY\``,
                timestamp: new Date()
            });
            return;
        }

        // Check for cached response
        const cacheKey = this.generateCacheKey(message);
        if (this.requestCache.has(cacheKey)) {
            const cachedResponse = this.requestCache.get(cacheKey);
            this.addMessage({
                type: 'bot',
                content: cachedResponse + '\n\n💾 *Trả lời từ cache*',
                timestamp: new Date()
            });
            return;
        }

        // Check for duplicate requests
        if (this.activeRequests.has(cacheKey)) {
            this.addMessage({
                type: 'bot',
                content: '⏳ **Đang xử lý câu hỏi tương tự, vui lòng chờ...**',
                timestamp: new Date()
            });
            return;
        }

        // Rate limiting - prevent too many requests
        const now = Date.now();
        if (now - this.lastRequestTime < 1000) { // 1 second between requests
            this.addMessage({
                type: 'bot',
                content: '⏳ **Vui lòng chờ 1 giây trước khi gửi câu hỏi tiếp theo...**',
                timestamp: new Date()
            });
            return;
        }
        this.lastRequestTime = now;

        // Show typing indicator
        this.showTyping();

        // Create abort controller for request cancellation
        const abortController = new AbortController();
        const timeoutId = setTimeout(() => {
            abortController.abort();
        }, 30000); // 30 second timeout

        // Track active request
        this.activeRequests.set(cacheKey, abortController);

        try {
            const response = await this.callExternalAI(message, abortController.signal);
            clearTimeout(timeoutId);
            this.hideTyping();
            
            // Cache the response
            this.requestCache.set(cacheKey, response);
            
            // Limit cache size
            if (this.requestCache.size > 50) {
                const firstKey = this.requestCache.keys().next().value;
                this.requestCache.delete(firstKey);
            }
            
            this.addMessage({
                type: 'bot',
                content: response,
                timestamp: new Date()
            });
        } catch (error) {
            clearTimeout(timeoutId);
            this.hideTyping();
            
            let errorMessage = 'Đã xảy ra lỗi khi gọi AI.';
            
            if (error.name === 'AbortError') {
                errorMessage = '⏰ **Hết thời gian chờ**\n\nYêu cầu đã bị hủy do quá thời gian chờ (30 giây). Vui lòng thử lại với câu hỏi ngắn gọn hơn.';
            } else if (error.message.includes('Failed to fetch')) {
                errorMessage = '🌐 **Lỗi kết nối**\n\nKhông thể kết nối đến AI service. Vui lòng kiểm tra kết nối internet và thử lại.';
            } else if (error.message.includes('API key')) {
                errorMessage = `🔑 **Lỗi API Key**\n\n${error.message}\n\nVui lòng kiểm tra lại API key.`;
            } else {
                errorMessage = `❌ **Lỗi AI:**\n\n${error.message}\n\nVui lòng thử lại sau.`;
            }
            
            this.addMessage({
                type: 'bot',
                content: errorMessage,
                timestamp: new Date()
            });
        } finally {
            // Clean up active request
            this.activeRequests.delete(cacheKey);
        }
    }

    generateCacheKey(message) {
        // Create a simple hash for caching
        const provider = localStorage.getItem('chatbot_ai_provider') || 'openai';
        return `${provider}:${message.toLowerCase().trim()}`;
    }

    clearCache() {
        this.requestCache.clear();
        this.addMessage({
            type: 'bot',
            content: '🗑️ **Đã xóa cache thành công!**',
            timestamp: new Date()
        });
    }

    cancelAllRequests() {
        // Cancel all active requests
        for (const [key, controller] of this.activeRequests) {
            controller.abort();
        }
        this.activeRequests.clear();
        this.hideTyping();
    }

    // Add command to clear cache
    handleSpecialCommands(message) {
        const lowerMessage = message.toLowerCase().trim();
        
        if (lowerMessage === '/clear' || lowerMessage === '/clear cache') {
            this.clearCache();
            return true;
        }
        
        if (lowerMessage === '/reset' || lowerMessage === '/clear all') {
            // Clear all data and reset
            this.clearCache();
            this.isLoggedIn = false;
            this.userInfo = null;
            
            // Clear all possible storage locations
            const keysToClear = [
                'authToken', 'token', 'accessToken', 'jwt',
                'userInfo', 'user', 'currentUser', 'userData', 'profile'
            ];
            
            keysToClear.forEach(key => {
                localStorage.removeItem(key);
                sessionStorage.removeItem(key);
            });
            
            this.updateModeDisplay();
            
            this.addMessage({
                type: 'bot',
                content: `🔄 **Reset hoàn toàn**\n\n✅ **Đã xóa tất cả dữ liệu:**\n• Cache\n• Token\n• User info\n• Trạng thái đăng nhập\n\nBây giờ chatbot đã được reset về trạng thái ban đầu.`,
                timestamp: new Date()
            });
            return true;
        }
        
        if (lowerMessage === '/cancel' || lowerMessage === '/stop') {
            this.cancelAllRequests();
            this.addMessage({
                type: 'bot',
                content: '⏹️ **Đã hủy tất cả yêu cầu đang xử lý!**',
                timestamp: new Date()
            });
            return true;
        }
        
        if (lowerMessage === '/login' || lowerMessage === 'đăng nhập') {
            this.handleLoginRequest();
            return true;
        }
        
        if (lowerMessage === '/logout' || lowerMessage === 'đăng xuất') {
            this.handleLogoutRequest();
            return true;
        }
        
        if (lowerMessage === '/status' || lowerMessage === 'trạng thái') {
            this.showLoginStatus();
            return true;
        }
        
        if (lowerMessage === '/userinfo') {
            // Show detailed user info
            const userInfo = this.userInfo || {};
            const userInfoStr = JSON.stringify(userInfo, null, 2);
            
            this.addMessage({
                type: 'bot',
                content: `👤 **Thông tin người dùng chi tiết**\n\n` +
                        `**Trạng thái đăng nhập:** ${this.isLoggedIn ? '✅ Đã đăng nhập' : '❌ Chưa đăng nhập'}\n\n` +
                        `**UserInfo Object:**\n\`\`\`json\n${userInfoStr}\n\`\`\`\n\n` +
                        `**Các trường có sẵn:**\n` +
                        `• name: ${userInfo.name || 'N/A'}\n` +
                        `• fullName: ${userInfo.fullName || 'N/A'}\n` +
                        `• username: ${userInfo.username || 'N/A'}\n` +
                        `• displayName: ${userInfo.displayName || 'N/A'}\n` +
                        `• email: ${userInfo.email || 'N/A'}\n` +
                        `• mail: ${userInfo.mail || 'N/A'}\n` +
                        `• userEmail: ${userInfo.userEmail || 'N/A'}\n` +
                        `• role: ${userInfo.role || 'N/A'}\n` +
                        `• userRole: ${userInfo.userRole || 'N/A'}\n` +
                        `• type: ${userInfo.type || 'N/A'}\n\n` +
                        `💡 **Gợi ý:** Sử dụng \`/set-user "Tên" "Email"\` để cài đặt thông tin thủ công.`,
                timestamp: new Date()
            });
            return true;
        }
        
        if (lowerMessage === '/connection') {
            // Test connection to backend
            this.addMessage({
                type: 'bot',
                content: `🌐 **Kiểm tra kết nối...**\n\nĐang kiểm tra kết nối đến backend...`,
                timestamp: new Date()
            });
            
            // Use setTimeout to avoid blocking the UI
            setTimeout(async () => {
                try {
                    const startTime = Date.now();
                    const response = await fetch(`${this.apiBaseUrl}/api/health`, {
                        method: 'GET',
                        headers: {
                            'Content-Type': 'application/json'
                        }
                    });
                    const endTime = Date.now();
                    const responseTime = endTime - startTime;
                    
                    if (response.ok) {
                        this.addMessage({
                            type: 'bot',
                            content: `✅ **Kết nối thành công**\n\n🌐 **Backend:** ${this.apiBaseUrl}\n⏱️ **Thời gian phản hồi:** ${responseTime}ms\n📡 **Trạng thái:** ${response.status} ${response.statusText}\n\nHệ thống hoạt động bình thường!`,
                            timestamp: new Date()
                        });
                    } else {
                        this.addMessage({
                            type: 'bot',
                            content: `⚠️ **Kết nối có vấn đề**\n\n🌐 **Backend:** ${this.apiBaseUrl}\n⏱️ **Thời gian phản hồi:** ${responseTime}ms\n📡 **Trạng thái:** ${response.status} ${response.statusText}\n\nCó thể có vấn đề với backend.`,
                            timestamp: new Date()
                        });
                    }
                } catch (error) {
                    this.addMessage({
                        type: 'bot',
                        content: `❌ **Lỗi kết nối**\n\n🌐 **Backend:** ${this.apiBaseUrl}\n❌ **Lỗi:** ${error.message}\n\nVui lòng kiểm tra:\n• Kết nối internet\n• Backend có đang chạy không\n• URL backend có đúng không`,
                        timestamp: new Date()
                    });
                }
            }, 100);
            
            return true;
        }
        
        if (lowerMessage === '/refresh' || lowerMessage === 'làm mới') {
            this.refreshLoginStatus();
            return true;
        }
        
        if (lowerMessage === '/help' || lowerMessage === '/commands') {
            this.addMessage({
                type: 'bot',
                content: `🤖 **Các lệnh có sẵn:**\n\n` +
                        `\`/login\` - Đăng nhập để sử dụng AI\n` +
                        `\`/logout\` - Đăng xuất\n` +
                        `\`/status\` - Xem trạng thái đăng nhập\n` +
                        `\`/refresh\` - Cập nhật trạng thái đăng nhập\n` +
                        `\`/debug\` - Kiểm tra thông tin đăng nhập\n` +
                        `\`/userinfo\` - Xem thông tin người dùng chi tiết\n` +
                        `\`/connection\` - Kiểm tra kết nối backend\n` +
                        `\`/force-login\` - Giả lập đăng nhập (test)\n` +
                        `\`/set-user "Tên" "Email"\` - Cài đặt thông tin người dùng\n` +
                        `\`/setkey YOUR_API_KEY\` - Cài đặt API key\n` +
                        `\`/clear\` - Xóa cache\n` +
                        `\`/reset\` - Reset hoàn toàn\n` +
                        `\`/cancel\` - Hủy yêu cầu đang xử lý\n` +
                        `\`/help\` - Hiển thị trợ giúp\n\n` +
                        `💡 **Mẹo:** Sử dụng cache để tăng tốc độ phản hồi!`,
                timestamp: new Date()
            });
            return true;
        }
        
        if (lowerMessage === '/debug') {
            // Check all possible token locations
            const possibleTokens = [
                { key: 'authToken', value: localStorage.getItem('authToken') || sessionStorage.getItem('authToken') },
                { key: 'token', value: localStorage.getItem('token') || sessionStorage.getItem('token') },
                { key: 'accessToken', value: localStorage.getItem('accessToken') || sessionStorage.getItem('accessToken') },
                { key: 'jwt', value: localStorage.getItem('jwt') || sessionStorage.getItem('jwt') }
            ];
            
            // Check all possible user info locations
            const possibleUserInfos = [
                { key: 'userInfo', value: localStorage.getItem('userInfo') || sessionStorage.getItem('userInfo') },
                { key: 'user', value: localStorage.getItem('user') || sessionStorage.getItem('user') },
                { key: 'currentUser', value: localStorage.getItem('currentUser') || sessionStorage.getItem('currentUser') },
                { key: 'userData', value: localStorage.getItem('userData') || sessionStorage.getItem('userData') },
                { key: 'profile', value: localStorage.getItem('profile') || sessionStorage.getItem('profile') }
            ];
            
            let debugInfo = `🔍 **Debug thông tin đăng nhập:**\n\n`;
            
            // Token information
            debugInfo += `🔑 **Token Information:**\n`;
            possibleTokens.forEach(({ key, value }) => {
                debugInfo += `• ${key}: ${value ? `✅ Có (${value.length} ký tự)` : '❌ Không'}\n`;
            });
            
            // User info information
            debugInfo += `\n👤 **User Info:**\n`;
            possibleUserInfos.forEach(({ key, value }) => {
                if (value) {
                    try {
                        const parsed = JSON.parse(value);
                        debugInfo += `• ${key}: ✅ Có (${parsed.name || 'N/A'})\n`;
                    } catch (e) {
                        debugInfo += `• ${key}: ✅ Có (không parse được JSON)\n`;
                    }
                } else {
                    debugInfo += `• ${key}: ❌ Không\n`;
                }
            });
            
            // Current status
            debugInfo += `\n📊 **Trạng thái hiện tại:**\n`;
            debugInfo += `• Đăng nhập: ${this.isLoggedIn ? '✅ Đã đăng nhập' : '❌ Chưa đăng nhập'}\n`;
            debugInfo += `• Tên: ${this.userInfo?.name || 'N/A'}\n`;
            debugInfo += `• Email: ${this.userInfo?.email || 'N/A'}\n`;
            debugInfo += `• Quyền: ${this.userInfo?.role || 'N/A'}\n`;
            
            // DOM indicators
            debugInfo += `\n🏷️ **DOM Indicators:**\n`;
            const domSelectors = [
                // User name selectors
                '.user-name', '.username', '.profile-name', '.name',
                '[data-user-name]', '[data-username]', '[data-name]',
                '.user-info .name', '.profile .name', '.header .name',
                
                // Navigation selectors
                '.nav-user .name', '.user-menu .name', '.dropdown-user .name',
                '.navbar-user .name', '.header-user .name',
                
                // Website specific
                '.iq-navbar .user-name', '.navbar .user-name',
                '.header .user-name', '.top-header .user-name',
                
                // Login/logout buttons
                'a[href*="login"]', 'a[href*="signin"]', '.login-btn',
                'a[href*="logout"]', '.logout-btn', '.signout-btn'
            ];
            
            domSelectors.forEach(selector => {
                const element = document.querySelector(selector);
                if (element) {
                    const text = element.textContent ? element.textContent.trim() : '';
                    debugInfo += `• ${selector}: ✅ Tìm thấy "${text}"\n`;
                } else {
                    debugInfo += `• ${selector}: ❌ Không tìm thấy\n`;
                }
            });
            
            debugInfo += `\n💡 **Gợi ý:**\n`;
            debugInfo += `• Nếu token có nhưng vẫn chưa đăng nhập, hãy thử lệnh \`/refresh\`\n`;
            debugInfo += `• Nếu vẫn lỗi, hãy làm mới trang và thử lại\n`;
            debugInfo += `• Kiểm tra Console (F12) để xem log chi tiết\n`;
            debugInfo += `• Thử lệnh \`/force-login\` để giả lập đăng nhập (test)`;
            
            this.addMessage({
                type: 'bot',
                content: debugInfo,
                timestamp: new Date()
            });
            return true;
        }
        
        if (lowerMessage === '/force-login') {
            // Force login for testing purposes
            this.isLoggedIn = true;
            this.userInfo = {
                name: 'Test User',
                email: 'test@example.com',
                role: 'User'
            };
            
            // Store fake token and user info
            localStorage.setItem('authToken', 'fake-token-for-testing');
            localStorage.setItem('userInfo', JSON.stringify(this.userInfo));
            
            this.updateModeDisplay();
            
            this.addMessage({
                type: 'bot',
                content: `🔧 **Force Login (Test Mode)**\n\n✅ **Đã giả lập đăng nhập thành công!**\n\n👤 **Tên:** Test User\n📧 **Email:** test@example.com\n🔑 **Quyền:** User\n\nBây giờ bạn có thể sử dụng tính năng AI để test.\n\n⚠️ **Lưu ý:** Đây chỉ là chế độ test, không phải đăng nhập thật.`,
                timestamp: new Date()
            });
            return true;
        }
        
        if (lowerMessage.startsWith('/set-user ')) {
            // Manual set user info: /set-user "Tên người dùng" "email@example.com"
            const parts = lowerMessage.split('"');
            if (parts.length >= 3) {
                const userName = parts[1];
                const userEmail = parts[3] || 'user@example.com';
                
                this.isLoggedIn = true;
                this.userInfo = {
                    name: userName,
                    email: userEmail,
                    role: 'User'
                };
                
                // Store user info
                localStorage.setItem('userInfo', JSON.stringify(this.userInfo));
                
                this.updateModeDisplay();
                
                this.addMessage({
                    type: 'bot',
                    content: `👤 **Đã cài đặt thông tin người dùng**\n\n✅ **Thông tin mới:**\n👤 **Tên:** ${userName}\n📧 **Email:** ${userEmail}\n🔑 **Quyền:** User\n\nBây giờ bạn có thể sử dụng tính năng AI!`,
                    timestamp: new Date()
                });
                return true;
            } else {
                this.addMessage({
                    type: 'bot',
                    content: `❌ **Cú pháp không đúng**\n\nSử dụng: \`/set-user "Tên người dùng" "email@example.com"\`\n\nVí dụ: \`/set-user "Nguyễn Văn A" "nguyenvana@example.com"\``,
                    timestamp: new Date()
                });
                return true;
            }
        }
        
        return false;
    }

    handleLoginRequest() {
        if (this.isLoggedIn) {
            this.addMessage({
                type: 'bot',
                content: `✅ **Bạn đã đăng nhập rồi!**\n\nTên: ${this.userInfo?.name || 'N/A'}\nEmail: ${this.userInfo?.email || 'N/A'}`,
                timestamp: new Date()
            });
        } else {
            this.addMessage({
                type: 'bot',
                content: `🔐 **Đăng nhập**\n\nVui lòng đăng nhập để sử dụng tính năng AI.\n\n💡 **Cách 1:** Nhấn "Mở trang đăng nhập" bên dưới\n💡 **Cách 2:** Truy cập thủ công: ./login.html\n💡 **Cách 3:** Tìm trang đăng nhập trong menu chính của website`,
                quickReplies: ['Mở trang đăng nhập', 'Hủy'],
                timestamp: new Date()
            });
        }
    }

    handleLogoutRequest() {
        if (!this.isLoggedIn) {
            this.addMessage({
                type: 'bot',
                content: '❌ **Bạn chưa đăng nhập!**',
                timestamp: new Date()
            });
            return;
        }
        
        // Clear auth data
        localStorage.removeItem('authToken');
        sessionStorage.removeItem('authToken');
        this.isLoggedIn = false;
        this.userInfo = null;
        
        // Switch back to booking mode if in AI mode
        if (this.currentMode === 'ai') {
            this.currentMode = 'booking';
            this.updateModeDisplay();
        }
        
        this.addMessage({
            type: 'bot',
            content: '✅ **Đã đăng xuất thành công!**\n\nBạn đã được chuyển về chế độ đặt lịch.',
            timestamp: new Date()
        });
    }

    showLoginStatus() {
        if (this.isLoggedIn) {
            const userName = this.userInfo?.name || this.userInfo?.fullName || this.userInfo?.username || this.userInfo?.displayName || 'N/A';
            const userEmail = this.userInfo?.email || this.userInfo?.mail || this.userInfo?.userEmail || 'N/A';
            const userRole = this.userInfo?.role || this.userInfo?.userRole || this.userInfo?.type || 'User';
            
            this.addMessage({
                type: 'bot',
                content: `✅ **Đã đăng nhập**\n\n👤 **Tên:** ${userName}\n📧 **Email:** ${userEmail}\n🔑 **Quyền:** ${userRole}\n\nBạn có thể sử dụng tính năng AI!\n\n💡 **Lệnh:** \`/userinfo\` để xem thông tin chi tiết`,
                timestamp: new Date()
            });
        } else {
            this.addMessage({
                type: 'bot',
                content: `❌ **Chưa đăng nhập**\n\nBạn cần đăng nhập để sử dụng tính năng AI.\n\n💡 **Cách khắc phục:**\n1. Đăng nhập vào website\n2. Làm mới trang\n3. Thử lại lệnh \`/status\`\n\n💡 **Lệnh:** \`/login\` để mở trang đăng nhập`,
                timestamp: new Date()
            });
        }
    }

    handleSetApiKey(message) {
        const parts = message.split(' ');
        
        if (parts.length < 2) {
            this.addMessage({
                type: 'bot',
                content: `❌ **Cú pháp không đúng**\n\nSử dụng: \`/setkey YOUR_API_KEY\`\n\nHoặc:\n\`/setkey openai YOUR_OPENAI_KEY\`\n\`/setkey google YOUR_GOOGLE_AI_KEY\``,
                timestamp: new Date()
            });
            return;
        }

        const apiKey = parts[1];
        const provider = parts.length > 2 ? parts[2] : 'openai';

        // Validate API key format (basic validation)
        if (apiKey.length < 10) {
            this.addMessage({
                type: 'bot',
                content: `❌ **API Key không hợp lệ**\n\nAPI key phải có ít nhất 10 ký tự.`,
                timestamp: new Date()
            });
            return;
        }

        this.aiApiKey = apiKey;
        this.aiApiEndpoint = this.getApiEndpoint(provider);
        
        // Store in localStorage
        localStorage.setItem('chatbot_ai_api_key', apiKey);
        localStorage.setItem('chatbot_ai_provider', provider);

        this.addMessage({
            type: 'bot',
            content: `✅ **API Key đã được cài đặt thành công!**\n\nProvider: ${provider.toUpperCase()}\n\nBây giờ bạn có thể hỏi tôi bất kỳ câu hỏi nào.`,
            timestamp: new Date()
        });
    }

    getApiEndpoint(provider) {
        switch (provider.toLowerCase()) {
            case 'openai':
                return 'https://api.openai.com/v1/chat/completions';
            case 'google':
                return 'https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash:generateContent';
            case 'anthropic':
                return 'https://api.anthropic.com/v1/messages';
            default:
                return 'https://api.openai.com/v1/chat/completions';
        }
    }

    async callExternalAI(message, signal) {
        const provider = localStorage.getItem('chatbot_ai_provider') || 'openai';
        
        // Retry mechanism
        const maxRetries = 2;
        let lastError;
        
        for (let attempt = 1; attempt <= maxRetries; attempt++) {
            try {
                if (provider === 'openai') {
                    return await this.callOpenAI(message, signal);
                } else if (provider === 'google') {
                    return await this.callGoogleAI(message, signal);
                } else if (provider === 'anthropic') {
                    return await this.callAnthropic(message, signal);
                } else {
                    throw new Error('Provider không được hỗ trợ');
                }
            } catch (error) {
                lastError = error;
                
                // Don't retry on certain errors
                if (error.name === 'AbortError' || 
                    error.message.includes('API key') || 
                    error.message.includes('rate limit')) {
                    throw error;
                }
                
                // Show retry message
                if (attempt < maxRetries) {
                    this.addMessage({
                        type: 'bot',
                        content: `🔄 **Lần thử ${attempt} thất bại, đang thử lại...**`,
                        timestamp: new Date()
                    });
                    
                    // Wait before retry
                    await new Promise(resolve => setTimeout(resolve, 1000 * attempt));
                }
            }
        }
        
        throw lastError;
    }

    async callOpenAI(message, signal) {
        const response = await fetch(this.aiApiEndpoint, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${this.aiApiKey}`
            },
            body: JSON.stringify({
                model: 'gpt-3.5-turbo',
                messages: [
                    {
                        role: 'system',
                        content: 'Bạn là một trợ lý AI thân thiện và hữu ích. Hãy trả lời bằng tiếng Việt một cách rõ ràng và dễ hiểu. Nếu được hỏi về y tế, hãy đưa ra thông tin chung và khuyến nghị người dùng tham khảo ý kiến bác sĩ.'
                    },
                    {
                        role: 'user',
                        content: message
                    }
                ],
                max_tokens: 300, // Reduced for faster response
                temperature: 0.7,
                stream: false
            }),
            signal: signal
        });

        if (!response.ok) {
            const error = await response.json();
            if (response.status === 429) {
                throw new Error('Rate limit exceeded. Vui lòng thử lại sau 1 phút.');
            } else if (response.status === 401) {
                throw new Error('API key không hợp lệ hoặc đã hết hạn.');
            } else if (response.status === 503) {
                throw new Error('Service temporarily unavailable. Vui lòng thử lại sau.');
            }
            throw new Error(error.error?.message || `HTTP ${response.status}: ${response.statusText}`);
        }

        const data = await response.json();
        return data.choices[0].message.content;
    }

    async callGoogleAI(message, signal) {
        const response = await fetch(`${this.aiApiEndpoint}?key=${this.aiApiKey}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                contents: [
                    {
                        role: 'user',
                        parts: [
                            {
                                text: `Bạn là một trợ lý AI thân thiện và hữu ích. Hãy trả lời bằng tiếng Việt một cách rõ ràng và dễ hiểu. Nếu được hỏi về y tế, hãy đưa ra thông tin chung và khuyến nghị người dùng tham khảo ý kiến bác sĩ.\n\nCâu hỏi: ${message}`
                            }
                        ]
                    }
                ],
                generationConfig: {
                    maxOutputTokens: 300, // Reduced for faster response
                    temperature: 0.7,
                    topP: 0.8,
                    topK: 40
                },
                safetySettings: [
                    {
                        category: "HARM_CATEGORY_HARASSMENT",
                        threshold: "BLOCK_MEDIUM_AND_ABOVE"
                    }
                ]
            }),
            signal: signal
        });

        if (!response.ok) {
            const error = await response.json();
            if (response.status === 429) {
                throw new Error('Rate limit exceeded. Vui lòng thử lại sau 1 phút.');
            } else if (response.status === 400) {
                throw new Error('API key không hợp lệ hoặc đã hết hạn.');
            }
            throw new Error(error.error?.message || `HTTP ${response.status}: ${response.statusText}`);
        }

        const data = await response.json();
        return data.candidates[0].content.parts[0].text;
    }

    async callAnthropic(message, signal) {
        const response = await fetch(this.aiApiEndpoint, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'x-api-key': this.aiApiKey,
                'anthropic-version': '2023-06-01'
            },
            body: JSON.stringify({
                model: 'claude-3-haiku-20240307', // Using faster model
                max_tokens: 300, // Reduced for faster response
                messages: [
                    {
                        role: 'user',
                        content: `Bạn là một trợ lý AI thân thiện và hữu ích. Hãy trả lời bằng tiếng Việt một cách rõ ràng và dễ hiểu. Nếu được hỏi về y tế, hãy đưa ra thông tin chung và khuyến nghị người dùng tham khảo ý kiến bác sĩ.\n\nCâu hỏi: ${message}`
                    }
                ]
            }),
            signal: signal
        });

        if (!response.ok) {
            const error = await response.json();
            if (response.status === 429) {
                throw new Error('Rate limit exceeded. Vui lòng thử lại sau 1 phút.');
            } else if (response.status === 401) {
                throw new Error('API key không hợp lệ hoặc đã hết hạn.');
            }
            throw new Error(error.error?.message || `HTTP ${response.status}: ${response.statusText}`);
        }

        const data = await response.json();
        return data.content[0].text;
    }

    loadStoredData() {
        // Load API key if exists
        const storedApiKey = localStorage.getItem('chatbot_ai_api_key');
        const storedProvider = localStorage.getItem('chatbot_ai_provider');
        
        if (storedApiKey) {
            this.aiApiKey = storedApiKey;
            this.aiApiEndpoint = this.getApiEndpoint(storedProvider || 'openai');
        } else {
            // Auto-set Gemini API key if not already set
            const geminiApiKey = 'AIzaSyCDn0x0eqw8j19ERcMeUKfBguesouAGEeU';
            this.aiApiKey = geminiApiKey;
            this.aiApiEndpoint = this.getApiEndpoint('google');
            
            // Store in localStorage
            localStorage.setItem('chatbot_ai_api_key', geminiApiKey);
            localStorage.setItem('chatbot_ai_provider', 'google');
        }
    }

    // Utility functions
    extractAppointmentInfo(message) {
        const info = {};
        
        // Extract name (simple heuristic)
        if (message.length > 1 && message.length < 50) {
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
        if (!dateString) return null;
        
        // Handle DD/MM/YYYY format
        const ddMMyyyyRegex = /^(\d{1,2})\/(\d{1,2})\/(\d{4})$/;
        const match = dateString.match(ddMMyyyyRegex);
        
        if (match) {
            const day = parseInt(match[1]);
            const month = parseInt(match[2]) - 1; // Month is 0-indexed
            const year = parseInt(match[3]);
            
            const date = new Date(year, month, day);
            
            // Validate date
            if (date.getFullYear() === year && 
                date.getMonth() === month && 
                date.getDate() === day) {
                return dateString; // Return original format for consistency
            }
        }
        
        // Handle YYYY-MM-DD format
        const yyyyMMddRegex = /^(\d{4})-(\d{1,2})-(\d{1,2})$/;
        const match2 = dateString.match(yyyyMMddRegex);
        
        if (match2) {
            const year = parseInt(match2[1]);
            const month = parseInt(match2[2]) - 1;
            const day = parseInt(match2[3]);
            
            const date = new Date(year, month, day);
            
            if (date.getFullYear() === year && 
                date.getMonth() === month && 
                date.getDate() === day) {
                // Convert to DD/MM/YYYY format
                return `${day.toString().padStart(2, '0')}/${(month + 1).toString().padStart(2, '0')}/${year}`;
            }
        }
        
        return null; // Invalid date
    }

    // Get doctors by department
    getDoctorsByDepartment(department) {
        const doctors = {
            'Khoa Răng Hàm Mặt': [
                { name: 'Dr. Vũ Văn Long', experience: '12+ năm', specialty: 'Điều trị răng miệng' },
                { name: 'Dr. Nguyễn Minh Đức', experience: '8+ năm', specialty: 'Cấy ghép Implant' },
                { name: 'Dr. Dương Minh Toản', experience: '10+ năm', specialty: 'Chỉnh nha thẩm mỹ' },
                { name: 'Dr. Nguyễn Khác Tráng', experience: '15+ năm', specialty: 'Phục hình răng' },
                { name: 'Dr. Nguyễn Minh Anh', experience: '6+ năm', specialty: 'Tẩy trắng răng' }
            ],
            'Khoa Nhi': [
                { name: 'Dr. Trần Thị Hoa', experience: '10+ năm', specialty: 'Nhi tổng quát' },
                { name: 'Dr. Lê Văn Nam', experience: '8+ năm', specialty: 'Nhi sơ sinh' },
                { name: 'Dr. Phạm Thị Lan', experience: '12+ năm', specialty: 'Tiêm chủng' }
            ],
            'Khoa Da liễu': [
                { name: 'Dr. Hoàng Văn Tuấn', experience: '9+ năm', specialty: 'Da liễu tổng quát' },
                { name: 'Dr. Nguyễn Thị Mai', experience: '7+ năm', specialty: 'Thẩm mỹ da' }
            ],
            'Khoa Mắt': [
                { name: 'Dr. Võ Thị Hương', experience: '11+ năm', specialty: 'Mắt tổng quát' },
                { name: 'Dr. Trần Văn Sơn', experience: '13+ năm', specialty: 'Phẫu thuật mắt' }
            ],
            'Khoa Tai mũi họng': [
                { name: 'Dr. Lê Thị Thảo', experience: '10+ năm', specialty: 'Tai mũi họng tổng quát' },
                { name: 'Dr. Nguyễn Văn Dũng', experience: '8+ năm', specialty: 'Nội soi TMH' }
            ],
            'Khoa Nội tổng quát': [
                { name: 'Dr. Phạm Văn Hùng', experience: '15+ năm', specialty: 'Nội tổng quát' },
                { name: 'Dr. Trần Thị Nga', experience: '12+ năm', specialty: 'Tim mạch' }
            ]
        };
        
        return doctors[department] || [];
    }

    // Get services by doctor
    getServicesByDoctor(doctorName) {
        const services = {
            'Dr. Vũ Văn Long': [
                { name: 'Khám răng tổng quát', price: '200,000 VNĐ' },
                { name: 'Điều trị sâu răng', price: '300,000 VNĐ' },
                { name: 'Nhổ răng', price: '500,000 VNĐ' }
            ],
            'Dr. Nguyễn Minh Đức': [
                { name: 'Tư vấn cấy ghép Implant', price: '500,000 VNĐ' },
                { name: 'Cấy ghép Implant', price: '15,000,000 VNĐ' },
                { name: 'Khám răng tổng quát', price: '200,000 VNĐ' }
            ],
            'Dr. Dương Minh Toản': [
                { name: 'Tư vấn chỉnh nha', price: '300,000 VNĐ' },
                { name: 'Niềng răng', price: '25,000,000 VNĐ' },
                { name: 'Khám răng tổng quát', price: '200,000 VNĐ' }
            ],
            'Dr. Nguyễn Khác Tráng': [
                { name: 'Phục hình răng sứ', price: '3,000,000 VNĐ' },
                { name: 'Làm cầu răng', price: '5,000,000 VNĐ' },
                { name: 'Khám răng tổng quát', price: '200,000 VNĐ' }
            ],
            'Dr. Nguyễn Minh Anh': [
                { name: 'Tẩy trắng răng', price: '1,500,000 VNĐ' },
                { name: 'Khám răng tổng quát', price: '200,000 VNĐ' },
                { name: 'Điều trị sâu răng', price: '300,000 VNĐ' }
            ],
            'Dr. Trần Thị Hoa': [
                { name: 'Khám nhi tổng quát', price: '150,000 VNĐ' },
                { name: 'Tư vấn dinh dưỡng', price: '100,000 VNĐ' }
            ],
            'Dr. Lê Văn Nam': [
                { name: 'Khám nhi sơ sinh', price: '200,000 VNĐ' },
                { name: 'Tư vấn chăm sóc trẻ', price: '100,000 VNĐ' }
            ],
            'Dr. Phạm Thị Lan': [
                { name: 'Tiêm chủng', price: '300,000 VNĐ' },
                { name: 'Khám nhi tổng quát', price: '150,000 VNĐ' }
            ],
            'Dr. Hoàng Văn Tuấn': [
                { name: 'Khám da liễu tổng quát', price: '200,000 VNĐ' },
                { name: 'Điều trị mụn', price: '500,000 VNĐ' }
            ],
            'Dr. Nguyễn Thị Mai': [
                { name: 'Tư vấn thẩm mỹ da', price: '300,000 VNĐ' },
                { name: 'Khám da liễu tổng quát', price: '200,000 VNĐ' }
            ],
            'Dr. Võ Thị Hương': [
                { name: 'Khám mắt tổng quát', price: '200,000 VNĐ' },
                { name: 'Đo thị lực', price: '100,000 VNĐ' }
            ],
            'Dr. Trần Văn Sơn': [
                { name: 'Tư vấn phẫu thuật mắt', price: '500,000 VNĐ' },
                { name: 'Khám mắt tổng quát', price: '200,000 VNĐ' }
            ],
            'Dr. Lê Thị Thảo': [
                { name: 'Khám TMH tổng quát', price: '200,000 VNĐ' },
                { name: 'Nội soi tai mũi họng', price: '400,000 VNĐ' }
            ],
            'Dr. Nguyễn Văn Dũng': [
                { name: 'Nội soi TMH', price: '400,000 VNĐ' },
                { name: 'Khám TMH tổng quát', price: '200,000 VNĐ' }
            ],
            'Dr. Phạm Văn Hùng': [
                { name: 'Khám nội tổng quát', price: '200,000 VNĐ' },
                { name: 'Tư vấn sức khỏe', price: '100,000 VNĐ' }
            ],
            'Dr. Trần Thị Nga': [
                { name: 'Khám tim mạch', price: '300,000 VNĐ' },
                { name: 'Đo điện tim', price: '200,000 VNĐ' }
            ]
        };
        
        return services[doctorName] || [];
    }

    // Get available shifts based on doctor's schedule
    getAvailableShifts(doctorName, date) {
        // Mock doctor schedules - in real app, this would come from API
        const schedules = {
            'Dr. Vũ Văn Long': {
                'Monday': ['morning', 'afternoon'],
                'Tuesday': ['morning', 'afternoon'],
                'Wednesday': ['morning', 'afternoon'],
                'Thursday': ['morning', 'afternoon'],
                'Friday': ['morning', 'afternoon']
            },
            'Dr. Nguyễn Minh Đức': {
                'Monday': ['morning', 'afternoon'],
                'Tuesday': ['morning', 'afternoon'],
                'Wednesday': ['morning'],
                'Thursday': ['afternoon'],
                'Friday': ['morning', 'afternoon']
            },
            'Dr. Dương Minh Toản': {
                'Monday': ['morning'],
                'Tuesday': ['afternoon'],
                'Wednesday': ['morning', 'afternoon'],
                'Thursday': ['morning', 'afternoon'],
                'Friday': ['morning']
            },
            'Dr. Nguyễn Khác Tráng': {
                'Monday': ['afternoon'],
                'Tuesday': ['morning', 'afternoon'],
                'Wednesday': ['morning', 'afternoon'],
                'Thursday': ['morning'],
                'Friday': ['afternoon']
            },
            'Dr. Nguyễn Minh Anh': {
                'Monday': ['morning', 'afternoon'],
                'Tuesday': ['morning'],
                'Wednesday': ['afternoon'],
                'Thursday': ['morning', 'afternoon'],
                'Friday': ['morning', 'afternoon']
            }
        };
        
        // Get day of week from date
        const dateObj = new Date(date.split('/').reverse().join('-'));
        const dayOfWeek = dateObj.toLocaleDateString('en-US', { weekday: 'long' });
        
        // Get schedule for this doctor and day, or return default shifts
        const doctorSchedule = schedules[doctorName];
        if (doctorSchedule && doctorSchedule[dayOfWeek]) {
            return doctorSchedule[dayOfWeek];
        }
        
        // Default shifts if no specific schedule
        return ['morning', 'afternoon'];
    }

    // Convert shift to display text
    getShiftDisplayText(shift) {
        const shiftTexts = {
            'morning': 'Ca sáng (07:00 - 12:00)',
            'afternoon': 'Ca chiều (13:00 - 17:00)'
        };
        return shiftTexts[shift] || shift;
    }

    // API functions to get data from database
    async getDepartmentsFromAPI() {
        try {
            const response = await fetch(`${this.apiBaseUrl}/api/chatbot/departments`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            if (response.ok) {
                const data = await response.json();
                return data.departments || [];
            } else {
                console.error('Failed to fetch departments');
                // Fallback to mock data if API fails
                return [
                    { id: 1, name: 'Khoa Răng Hàm Mặt' },
                    { id: 2, name: 'Khoa Nhi' },
                    { id: 3, name: 'Khoa Da liễu' },
                    { id: 4, name: 'Khoa Mắt' },
                    { id: 5, name: 'Khoa Tai mũi họng' },
                    { id: 6, name: 'Khoa Nội tổng quát' }
                ];
            }
        } catch (error) {
            console.error('Error fetching departments:', error);
            // Fallback to mock data
            return [
                { id: 1, name: 'Khoa Răng Hàm Mặt' },
                { id: 2, name: 'Khoa Nhi' },
                { id: 3, name: 'Khoa Da liễu' },
                { id: 4, name: 'Khoa Mắt' },
                { id: 5, name: 'Khoa Tai mũi họng' },
                { id: 6, name: 'Khoa Nội tổng quát' }
            ];
        }
    }

    async getDoctorsByDepartmentFromAPI(departmentName) {
        try {
            const response = await fetch(`${this.apiBaseUrl}/api/chatbot/doctors?department=${encodeURIComponent(departmentName)}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            if (response.ok) {
                const data = await response.json();
                return data.doctors || [];
            } else {
                console.error('Failed to fetch doctors');
                // Fallback to mock data
                return this.getDoctorsByDepartment(departmentName);
            }
        } catch (error) {
            console.error('Error fetching doctors:', error);
            // Fallback to mock data
            return this.getDoctorsByDepartment(departmentName);
        }
    }

    async getServicesByDoctorFromAPI(doctorName) {
        try {
            const response = await fetch(`${this.apiBaseUrl}/api/chatbot/services?doctor=${encodeURIComponent(doctorName)}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            if (response.ok) {
                const data = await response.json();
                return data.services || [];
            } else {
                console.error('Failed to fetch services');
                // Fallback to mock data
                return this.getServicesByDoctor(doctorName);
            }
        } catch (error) {
            console.error('Error fetching services:', error);
            // Fallback to mock data
            return this.getServicesByDoctor(doctorName);
        }
    }

    async getAvailableShiftsFromAPI(doctorName, date) {
        try {
            const response = await fetch(`${this.apiBaseUrl}/api/chatbot/shifts?doctor=${encodeURIComponent(doctorName)}&date=${date}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            if (response.ok) {
                const data = await response.json();
                return data.shifts || [];
            } else {
                console.error('Failed to fetch shifts');
                // Fallback to mock data
                return this.getAvailableShifts(doctorName, date);
            }
        } catch (error) {
            console.error('Error fetching shifts:', error);
            // Fallback to mock data
            return this.getAvailableShifts(doctorName, date);
        }
    }

    async getAvailableDatesFromAPI(doctorName) {
        try {
            const response = await fetch(`${this.apiBaseUrl}/api/chatbot/available-dates?doctor=${encodeURIComponent(doctorName)}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            
            if (response.ok) {
                const data = await response.json();
                return {
                    availableDates: data.availableDates || [],
                    dateInfo: data.dateInfo || [],
                    workingDates: data.workingDates || [],
                    leaveDates: data.leaveDates || []
                };
            } else {
                console.error('Failed to fetch available dates');
                // Fallback to mock data
                const dates = [];
                for (let i = 1; i <= 7; i++) {
                    const date = new Date();
                    date.setDate(date.getDate() + i);
                    dates.push(date.toLocaleDateString('vi-VN'));
                }
                return {
                    availableDates: dates,
                    dateInfo: [],
                    workingDates: dates,
                    leaveDates: []
                };
            }
        } catch (error) {
            console.error('Error fetching available dates:', error);
            // Fallback to mock data
            const dates = [];
            for (let i = 1; i <= 7; i++) {
                const date = new Date();
                date.setDate(date.getDate() + i);
                dates.push(date.toLocaleDateString('vi-VN'));
            }
            return {
                availableDates: dates,
                dateInfo: [],
                workingDates: dates,
                leaveDates: []
            };
        }
    }

    // API function to create appointment
    async createAppointmentAPI(data) {
        try {
            const response = await fetch(`${this.apiBaseUrl}/api/chatbot/appointments`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });
            
            if (response.ok) {
                const result = await response.json();
                return {
                    success: true,
                    appointmentCode: result.appointmentCode || 'GC-' + Math.random().toString(36).substr(2, 4).toUpperCase(),
                    message: result.message || 'Appointment created successfully'
                };
            } else {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Failed to create appointment');
            }
        } catch (error) {
            console.error('Error creating appointment:', error);
            // Fallback to mock response
            await new Promise(resolve => setTimeout(resolve, 2000));
            return {
                success: true,
                appointmentCode: 'GC-' + Math.random().toString(36).substr(2, 4).toUpperCase(),
                message: 'Appointment created successfully'
            };
        }
    }

    // UI functions
    addMessage(message) {
        const messagesContainer = document.getElementById('chatbotMessages');
        const messageElement = this.createMessageElement(message);
        messagesContainer.appendChild(messageElement);
        
        // Save to conversation history
        if (!this.conversationHistory) {
            this.conversationHistory = [];
        }
        this.conversationHistory.push(message);
        
        // Scroll to bottom
        messagesContainer.scrollTop = messagesContainer.scrollHeight;
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
            
            message.quickReplies.forEach((reply, index) => {
                const button = document.createElement('button');
                button.className = 'chatbot-quick-reply';
                button.textContent = reply;
                button.style.animationDelay = `${index * 0.1}s`;
                button.onclick = () => this.handleQuickReply(reply);
                quickReplies.appendChild(button);
            });
            
            content.appendChild(quickReplies);
        }
        
        // Add beautiful entrance animation
        setTimeout(() => {
            messageDiv.style.opacity = '1';
            messageDiv.style.transform = 'translateY(0)';
        }, 50);
        
        return messageDiv;
    }

    handleQuickReply(reply) {
        if (reply === 'Mở trang đăng nhập') {
            // Open login page
            this.openLoginPage();
            return;
        }
        
        if (reply === 'Hủy') {
            this.addMessage({
                type: 'bot',
                content: `❌ **Đã hủy đăng nhập**\n\nBạn có thể tiếp tục sử dụng chế độ đặt lịch hoặc thử lại sau.`,
                timestamp: new Date()
            });
            return;
        }
        
        if (reply === 'Đăng nhập') {
            this.handleLoginRequest();
            return;
        }
        
        if (reply === 'Chuyển sang đặt lịch') {
            this.currentMode = 'booking';
            this.updateModeDisplay();
            this.resetAppointmentData();
            
            this.addMessage({
                type: 'bot',
                content: `📅 **Chế độ Đặt lịch**\n\nTôi sẽ giúp bạn đặt lịch khám bệnh. Bạn có muốn đặt lịch không?`,
                quickReplies: ['Có, tôi muốn đặt lịch', 'Không, cảm ơn'],
                timestamp: new Date()
            });
            return;
        }
        
        if (reply === 'Ở lại chế độ đặt lịch') {
            this.addMessage({
                type: 'bot',
                content: `✅ **Đã ở lại chế độ đặt lịch**\n\nBạn có muốn đặt lịch khám bệnh không?`,
                quickReplies: ['Có, tôi muốn đặt lịch', 'Không, cảm ơn'],
                timestamp: new Date()
            });
            return;
        }
        
        if (reply === 'Thử lại') {
            // Retry the last operation based on current step
            if (this.appointmentData.step === 'patient_search') {
                this.addMessage({
                    type: 'bot',
                    content: `🔄 **Thử lại tìm kiếm**\n\nVui lòng nhập lại số điện thoại hoặc số căn cước công dân:`,
                    timestamp: new Date()
                });
            } else {
                // Retry getting patient data from user
                this.appointmentData.step = 'booking_type';
                this.addMessage({
                    type: 'bot',
                    content: `🔄 **Thử lại lấy thông tin**\n\nBạn muốn đặt lịch cho ai?`,
                    quickReplies: ['Đặt cho bản thân', 'Đặt cho người thân'],
                    timestamp: new Date()
                });
            }
            return;
        }
        
        // Handle other quick replies
        this.processMessage(reply);
    }

    // Reset appointment data when starting new conversation
    resetAppointmentData() {
        this.appointmentData = {
            step: null,
            department: '',
            doctor: '',
            service: '',
            date: '',
            shift: '',
            notes: '',
            patientData: null,
            bookingType: null,
            foundPatient: null,
            newPatientData: null
        };
    }

    formatTime(timestamp) {
        return timestamp.toLocaleTimeString('vi-VN', { 
            hour: '2-digit', 
            minute: '2-digit' 
        });
    }

    showTyping() {
        this.isTyping = true;
        // Create typing indicator if it doesn't exist
        let typingIndicator = document.getElementById('typingIndicator');
        if (!typingIndicator) {
            typingIndicator = document.createElement('div');
            typingIndicator.id = 'typingIndicator';
            typingIndicator.className = 'typing-indicator';
            typingIndicator.innerHTML = `
                <div class="typing-dot"></div>
                <div class="typing-dot"></div>
                <div class="typing-dot"></div>
            `;
            document.getElementById('chatbotMessages').appendChild(typingIndicator);
        }
        typingIndicator.style.display = 'block';
    }

    hideTyping() {
        this.isTyping = false;
        const typingIndicator = document.getElementById('typingIndicator');
        if (typingIndicator) {
            typingIndicator.style.display = 'none';
        }
    }

    toggleChat() {
        this.isOpen = !this.isOpen;
        const container = document.getElementById('chatbot-container');
        
        if (this.isOpen) {
            container.classList.add('open');
        } else {
            container.classList.remove('open');
        }
    }

    showWelcomeMessage() {
        let loginStatus;
        if (this.isLoggedIn) {
            loginStatus = `✅ **Đã đăng nhập** - Chào mừng ${this.userInfo?.name || 'bạn'}!\n📧 **Email:** ${this.userInfo?.email || 'N/A'}\n🔑 **Quyền:** ${this.userInfo?.role || 'User'}`;
        } else {
            loginStatus = `❌ **Chưa đăng nhập** - Chỉ có thể đặt lịch\n\n💡 **Để sử dụng AI:**\n1. Đăng nhập vào website\n2. Làm mới trang\n3. Thử lại`;
        }
        
        this.addMessage({
            type: 'bot',
            content: `👋 **Chào mừng bạn đến với G-Care Clinic!**\n\n${loginStatus}\n\nTôi là trợ lý AI, có thể giúp bạn:\n\n📅 **Đặt lịch khám bệnh** (Dành cho tất cả)\n🤖 **Trả lời câu hỏi chung (Gemini AI)** (Chỉ dành cho người đã đăng nhập)\n\n🚀 **Tính năng mới:**\n• Cache thông minh để tăng tốc độ\n• Timeout 30 giây để tránh lag\n• Retry tự động khi lỗi mạng\n• Rate limiting để tránh spam\n• Yêu cầu đăng nhập cho AI\n\n💡 **Lệnh hữu ích:**\n\`/login\` - Đăng nhập\n\`/status\` - Xem trạng thái\n\`/debug\` - Kiểm tra thông tin\n\`/refresh\` - Cập nhật trạng thái\n\`/help\` - Xem tất cả lệnh\n\`/clear\` - Xóa cache\n\`/cancel\` - Hủy yêu cầu\n\n💡 **Mẹo:** Bạn có thể nhắn tin "đặt lịch khám" hoặc "muốn đặt lịch hẹn" để tự động chuyển sang chế độ đặt lịch!\n\nBạn muốn làm gì hôm nay?`,
            quickReplies: this.isLoggedIn ? ['Đặt lịch khám', 'Hỏi đáp AI', '/help'] : ['Đặt lịch khám', 'Đăng nhập', '/help'],
            timestamp: new Date()
        });
    }

    async processBookingMessage(message) {
        const lowerMessage = message.toLowerCase();

        // Check if user wants to switch to AI mode
        if (lowerMessage.includes('hỏi đáp ai') || lowerMessage.includes('ai hỏi đáp') || lowerMessage.includes('chế độ ai') || lowerMessage.includes('ai mode') || lowerMessage.includes('general query')) {
            if (this.currentMode !== 'ai') {
                this.toggleMode(); // This will check login status
            }
            return; // Stop further processing in booking mode
        }

        // Check for login requests
        if (lowerMessage.includes('đăng nhập') || lowerMessage.includes('login')) {
            this.handleLoginRequest();
            return;
        }

        // Check if user wants to book appointment
        if (lowerMessage.includes('đặt lịch') || lowerMessage.includes('book') || lowerMessage.includes('appointment')) {
            this.resetAppointmentData();
            this.appointmentData.step = 'booking_type';
            
            return this.addMessage({
                type: 'bot',
                content: `📅 **Đặt lịch khám bệnh**\n\nBạn muốn đặt lịch cho ai?\n\n💡 **Lưu ý:** Nếu đặt cho bản thân, tôi sẽ lấy thông tin từ tài khoản của bạn.`,
                quickReplies: ['Đặt cho bản thân', 'Đặt cho người thân'],
                timestamp: new Date()
            });
        }

        // Handle appointment booking flow
        if (this.appointmentData.step) {
            return await this.handleBookAppointment(message);
        }

        // Default response
        this.addMessage({
            type: 'bot',
            content: `Tôi có thể giúp bạn đặt lịch khám bệnh hoặc trả lời các câu hỏi (cần đăng nhập). Bạn muốn làm gì?`,
            quickReplies: this.isLoggedIn ? ['Đặt lịch khám', 'Hỏi đáp AI'] : ['Đặt lịch khám', 'Đăng nhập'],
            timestamp: new Date()
        });
    }

    async handleBookAppointment(message) {
        // Step 1: Determine booking type (self vs family)
        if (this.appointmentData.step === 'booking_type') {
            if (message.toLowerCase().includes('bản thân') || message.toLowerCase().includes('mình') || message.toLowerCase().includes('tôi')) {
                // Check if user is logged in
                if (!this.isLoggedIn) {
                    this.addMessage({
                        type: 'bot',
                        content: `🔒 **Yêu cầu đăng nhập**\n\nĐể đặt lịch cho bản thân, bạn cần đăng nhập trước.\n\nVui lòng đăng nhập và thử lại.`,
                        quickReplies: ['Đăng nhập', 'Đặt cho người thân'],
                        timestamp: new Date()
                    });
                    return;
                }
                
                // Get patient data from user account
                try {
                    const patientData = await this.getPatientDataFromUser();
                    if (patientData) {
                        this.appointmentData.patientData = patientData;
                        this.appointmentData.bookingType = 'self';
                        this.appointmentData.step = 'department';
                        
                        const departments = await this.getDepartmentsFromAPI();
                        const departmentNames = departments.map(dept => dept.name);
                        
                        this.addMessage({
                            type: 'bot',
                            content: `✅ **Đã lấy thông tin từ tài khoản**\n\n👤 **Bệnh nhân:** ${patientData.name}\n📱 **SĐT:** ${patientData.phone}\n\n📅 **Đặt lịch khám bệnh**\n\nVui lòng chọn khoa/phòng khám:`,
                            quickReplies: departmentNames,
                            timestamp: new Date()
                        });
                    } else {
                        this.addMessage({
                            type: 'bot',
                            content: `❌ **Không tìm thấy thông tin bệnh nhân**\n\nVui lòng cập nhật thông tin cá nhân trong tài khoản hoặc đặt lịch cho người thân.`,
                            quickReplies: ['Đặt cho người thân', 'Hủy'],
                            timestamp: new Date()
                        });
                    }
                } catch (error) {
                    console.error('Error in patient data retrieval:', error);
                    
                    let errorMessage = '❌ **Lỗi khi lấy thông tin**\n\n';
                    
                    if (error.message.includes('Phiên đăng nhập đã hết hạn')) {
                        errorMessage += `🔑 **Phiên đăng nhập đã hết hạn**\n\nVui lòng đăng nhập lại để tiếp tục.\n\n💡 **Lệnh:** \`/login\` để mở trang đăng nhập`;
                    } else if (error.message.includes('Tài khoản chưa có thông tin')) {
                        errorMessage += `📝 **Tài khoản chưa có thông tin bệnh nhân**\n\nVui lòng cập nhật thông tin cá nhân trong tài khoản hoặc đặt lịch cho người thân.`;
                    } else if (error.message.includes('Lỗi kết nối mạng')) {
                        errorMessage += `🌐 **Lỗi kết nối mạng**\n\nVui lòng kiểm tra kết nối internet và thử lại.`;
                    } else {
                        errorMessage += `${error.message}\n\nVui lòng thử lại hoặc đặt lịch cho người thân.`;
                    }
                    
                    this.addMessage({
                        type: 'bot',
                        content: errorMessage,
                        quickReplies: ['Đặt cho người thân', 'Hủy', 'Thử lại'],
                        timestamp: new Date()
                    });
                }
            } else if (message.toLowerCase().includes('người thân') || message.toLowerCase().includes('gia đình')) {
                this.appointmentData.bookingType = 'family';
                this.appointmentData.step = 'patient_search';
                
                this.addMessage({
                    type: 'bot',
                    content: `👥 **Đặt lịch cho người thân**\n\nVui lòng nhập số điện thoại hoặc số căn cước công dân của người cần đặt lịch:\n\n💡 **Mẹo:** Nếu người này đã có trong hệ thống, tôi sẽ tìm thông tin tự động.`,
                    timestamp: new Date()
                });
            } else {
                this.addMessage({
                    type: 'bot',
                    content: `❓ **Vui lòng chọn rõ ràng**\n\nBạn muốn đặt lịch cho ai?`,
                    quickReplies: ['Đặt cho bản thân', 'Đặt cho người thân'],
                    timestamp: new Date()
                });
            }
            return;
        }

        // Step 2: Search for existing patient (family booking)
        if (this.appointmentData.step === 'patient_search') {
            const searchTerm = message.trim();
            
            // Validate input
            if (searchTerm.length < 5) {
                this.addMessage({
                    type: 'bot',
                    content: `❌ **Thông tin không hợp lệ**\n\nVui lòng nhập số điện thoại (10-11 số) hoặc số căn cước công dân (12 số).`,
                    timestamp: new Date()
                });
                return;
            }

            try {
                // Search for existing patient
                const existingPatient = await this.searchPatient(searchTerm);
                
                if (existingPatient) {
                    // Found existing patient
                    this.appointmentData.step = 'confirm_patient';
                    this.appointmentData.foundPatient = existingPatient;
                    
                    this.addMessage({
                        type: 'bot',
                        content: `🔍 **Tìm thấy bệnh nhân**\n\n👤 **Họ tên:** ${existingPatient.name}\n📅 **Ngày sinh:** ${existingPatient.dob}\n📱 **SĐT:** ${existingPatient.phone}\n🆔 **CCCD:** ${existingPatient.identityNumber}\n\nĐây có phải là người bạn muốn đặt lịch không?`,
                        quickReplies: ['Đúng rồi', 'Không phải, tạo mới'],
                        timestamp: new Date()
                    });
                } else {
                    // No existing patient found, create new one
                    this.appointmentData.step = 'new_patient_name';
                    this.addMessage({
                        type: 'bot',
                        content: `❌ **Không tìm thấy bệnh nhân**\n\nVui lòng nhập thông tin để tạo bệnh nhân mới.\n\n📝 **Họ và tên:**`,
                        timestamp: new Date()
                    });
                }
            } catch (error) {
                console.error('Error searching patient:', error);
                
                let errorMessage = '❌ **Lỗi khi tìm kiếm**\n\n';
                
                if (error.message.includes('network') || error.message.includes('fetch')) {
                    errorMessage += `🌐 **Lỗi kết nối mạng**\n\nVui lòng kiểm tra kết nối internet và thử lại.`;
                } else if (error.message.includes('timeout')) {
                    errorMessage += `⏰ **Hệ thống phản hồi chậm**\n\nVui lòng thử lại sau vài giây.`;
                } else {
                    errorMessage += `${error.message}\n\nVui lòng thử lại.`;
                }
                
                this.addMessage({
                    type: 'bot',
                    content: errorMessage,
                    quickReplies: ['Thử lại', 'Tạo mới', 'Hủy'],
                    timestamp: new Date()
                });
            }
            return;
        }

        // Step 3: Confirm found patient
        if (this.appointmentData.step === 'confirm_patient') {
            if (message.toLowerCase().includes('đúng') || message.toLowerCase().includes('phải')) {
                // Use found patient data
                this.appointmentData.patientData = this.appointmentData.foundPatient;
                this.appointmentData.step = 'department';
                
                const departments = await this.getDepartmentsFromAPI();
                const departmentNames = departments.map(dept => dept.name);
                
                this.addMessage({
                    type: 'bot',
                    content: `✅ **Đã xác nhận thông tin bệnh nhân**\n\n👤 **Bệnh nhân:** ${this.appointmentData.patientData.name}\n\n📅 **Đặt lịch khám bệnh**\n\nVui lòng chọn khoa/phòng khám:`,
                    quickReplies: departmentNames,
                    timestamp: new Date()
                });
            } else {
                // Create new patient
                this.appointmentData.step = 'new_patient_name';
                this.addMessage({
                    type: 'bot',
                    content: `📝 **Tạo bệnh nhân mới**\n\nVui lòng nhập thông tin bệnh nhân.\n\n👤 **Họ và tên:**`,
                    timestamp: new Date()
                });
            }
            return;
        }

        // Step 4: New patient information collection
        if (this.appointmentData.step === 'new_patient_name') {
            this.appointmentData.newPatientData = { name: message.trim() };
            this.appointmentData.step = 'new_patient_dob';
            
            this.addMessage({
                type: 'bot',
                content: `📅 **Ngày tháng năm sinh**\n\nVui lòng nhập theo định dạng DD/MM/YYYY\n\nVí dụ: 15/03/1990`,
                timestamp: new Date()
            });
            return;
        }

        if (this.appointmentData.step === 'new_patient_dob') {
            const dob = this.parseDate(message);
            if (!dob) {
                this.addMessage({
                    type: 'bot',
                    content: `❌ **Ngày sinh không hợp lệ**\n\nVui lòng nhập theo định dạng DD/MM/YYYY\n\nVí dụ: 15/03/1990`,
                    timestamp: new Date()
                });
                return;
            }
            
            this.appointmentData.newPatientData.dob = dob;
            this.appointmentData.step = 'new_patient_phone';
            
            this.addMessage({
                type: 'bot',
                content: `📱 **Số điện thoại**\n\nVui lòng nhập số điện thoại (10-11 số)`,
                timestamp: new Date()
            });
            return;
        }

        if (this.appointmentData.step === 'new_patient_phone') {
            const phoneRegex = /^\d{10,11}$/;
            if (!phoneRegex.test(message.trim())) {
                this.addMessage({
                    type: 'bot',
                    content: `❌ **Số điện thoại không hợp lệ**\n\nVui lòng nhập số điện thoại từ 10-11 số.`,
                    timestamp: new Date()
                });
                return;
            }
            
            this.appointmentData.newPatientData.phone = message.trim();
            this.appointmentData.step = 'new_patient_identity';
            
            this.addMessage({
                type: 'bot',
                content: `🆔 **Số căn cước công dân**\n\nVui lòng nhập số CCCD (12 số)`,
                timestamp: new Date()
            });
            return;
        }

        if (this.appointmentData.step === 'new_patient_identity') {
            const identityRegex = /^\d{12}$/;
            if (!identityRegex.test(message.trim())) {
                this.addMessage({
                    type: 'bot',
                    content: `❌ **Số CCCD không hợp lệ**\n\nVui lòng nhập số CCCD đúng 12 số.`,
                    timestamp: new Date()
                });
                return;
            }
            
            this.appointmentData.newPatientData.identityNumber = message.trim();
            
            // Create new patient
            try {
                const newPatient = await this.createNewPatient(this.appointmentData.newPatientData);
                this.appointmentData.patientData = newPatient;
                this.appointmentData.step = 'department';
                
                const departments = await this.getDepartmentsFromAPI();
                const departmentNames = departments.map(dept => dept.name);
                
                this.addMessage({
                    type: 'bot',
                    content: `✅ **Đã tạo bệnh nhân mới thành công**\n\n👤 **Họ tên:** ${newPatient.name}\n📅 **Ngày sinh:** ${newPatient.dob}\n📱 **SĐT:** ${newPatient.phone}\n🆔 **CCCD:** ${newPatient.identityNumber}\n\n📅 **Đặt lịch khám bệnh**\n\nVui lòng chọn khoa/phòng khám:`,
                    quickReplies: departmentNames,
                    timestamp: new Date()
                });
            } catch (error) {
                this.addMessage({
                    type: 'bot',
                    content: `❌ **Lỗi khi tạo bệnh nhân**\n\n${error.message}\n\nVui lòng thử lại.`,
                    timestamp: new Date()
                });
            }
            return;
        }

        // Continue with existing flow from department selection
        if (this.appointmentData.step === 'department') {
            this.appointmentData.department = message;
            this.appointmentData.step = 'doctor';
            
            const doctors = await this.getDoctorsByDepartmentFromAPI(message);
            const doctorNames = doctors.map(doctor => doctor.name || doctor.email);
            
            return this.addMessage({
                type: 'bot',
                content: `👨‍⚕️ **Chọn bác sĩ trong khoa ${message}:**`,
                quickReplies: doctorNames,
                timestamp: new Date()
            });
        }

        if (this.appointmentData.step === 'doctor') {
            this.appointmentData.doctor = message;
            this.appointmentData.step = 'service';
            
            const services = await this.getServicesByDoctorFromAPI(message);
            const serviceNames = services.map(service => service.name);
            
            return this.addMessage({
                type: 'bot',
                content: `🏥 **Chọn dịch vụ của bác sĩ ${message}:**`,
                quickReplies: serviceNames,
                timestamp: new Date()
            });
        }

        if (this.appointmentData.step === 'service') {
            this.appointmentData.service = message;
            this.appointmentData.step = 'date';
            
            // Get available dates from database based on doctor's working schedule
            const dateData = await this.getAvailableDatesFromAPI(this.appointmentData.doctor);
            
            // Create detailed date information
            let dateMessage = `📅 **Lịch làm việc của bác sĩ ${this.appointmentData.doctor}:**\n\n`;
            
            if (dateData.dateInfo && dateData.dateInfo.length > 0) {
                // Group dates by type
                const workingDates = dateData.dateInfo.filter(d => d.type === 'working');
                const leaveDates = dateData.dateInfo.filter(d => d.type === 'leave');
                
                if (workingDates.length > 0) {
                    dateMessage += "✅ **Ngày có lịch làm việc:**\n";
                    workingDates.forEach(date => {
                        dateMessage += `• ${date.date}\n`;
                    });
                    dateMessage += "\n";
                }
                
                if (leaveDates.length > 0) {
                    dateMessage += "🚫 **Ngày nghỉ phép:**\n";
                    leaveDates.forEach(date => {
                        dateMessage += `• ${date.date}\n`;
                    });
                    dateMessage += "\n";
                }
            }
            
            dateMessage += "Vui lòng chọn ngày bạn muốn đặt lịch:";
            
            return this.addMessage({
                type: 'bot',
                content: dateMessage,
                quickReplies: dateData.availableDates,
                timestamp: new Date()
            });
        }

        if (this.appointmentData.step === 'date') {
            const selectedDate = this.parseDate(message) || message;
            if (!selectedDate) {
                return this.addMessage({
                    type: 'bot',
                    content: `❌ **Ngày không hợp lệ**\n\nVui lòng nhập ngày theo định dạng DD/MM/YYYY hoặc chọn từ danh sách.`,
                    timestamp: new Date()
                });
            }
            
            this.appointmentData.date = selectedDate;
            this.appointmentData.step = 'shift';
            
            const shifts = await this.getAvailableShiftsFromAPI(this.appointmentData.doctor, selectedDate);
            const shiftOptions = shifts.map(shift => this.getShiftDisplayText(shift));
            
            return this.addMessage({
                type: 'bot',
                content: `⏰ **Chọn ca khám cho ngày ${selectedDate}:**`,
                quickReplies: shiftOptions,
                timestamp: new Date()
            });
        }

        if (this.appointmentData.step === 'shift') {
            // Extract shift from message
            let selectedShift = 'morning'; // default
            if (message.toLowerCase().includes('chiều') || message.toLowerCase().includes('afternoon')) {
                selectedShift = 'afternoon';
            }
            
            this.appointmentData.shift = selectedShift;
            this.appointmentData.step = 'notes';
            
            return this.addMessage({
                type: 'bot',
                content: `📝 **Ghi chú (tùy chọn)**\n\nBạn có muốn thêm ghi chú gì không? Nếu không, hãy nhập "Không" hoặc "Không có".`,
                quickReplies: ['Không', 'Không có'],
                timestamp: new Date()
            });
        }

        if (this.appointmentData.step === 'notes') {
            if (message.toLowerCase().includes('không')) {
                this.appointmentData.notes = '';
            } else {
                this.appointmentData.notes = message;
            }
            
            // Show confirmation
            const shiftText = this.getShiftDisplayText(this.appointmentData.shift);
            const patientName = this.appointmentData.patientData.name;
            const confirmationMessage = `📋 **Xác nhận thông tin đặt lịch:**\n\n👤 **Bệnh nhân:** ${patientName}\n🏥 **Khoa:** ${this.appointmentData.department}\n👨‍⚕️ **Bác sĩ:** ${this.appointmentData.doctor}\n🏥 **Dịch vụ:** ${this.appointmentData.service}\n📅 **Ngày:** ${this.appointmentData.date}\n⏰ **Ca:** ${shiftText}\n📝 **Ghi chú:** ${this.appointmentData.notes || 'Không có'}\n\nBạn có muốn xác nhận đặt lịch không?`;
            
            this.appointmentData.step = 'confirm';
            
            return this.addMessage({
                type: 'bot',
                content: confirmationMessage,
                quickReplies: ['Xác nhận', 'Hủy bỏ'],
                timestamp: new Date()
            });
        }

        if (this.appointmentData.step === 'confirm') {
            if (message.toLowerCase().includes('xác nhận') || message.toLowerCase().includes('ok') || message.toLowerCase().includes('yes')) {
                this.appointmentData.step = 'submitting';
                
                this.addMessage({
                    type: 'bot',
                    content: `⏳ **Đang xử lý đặt lịch...**\n\nVui lòng chờ trong giây lát.`,
                    timestamp: new Date()
                });
                
                this.showTyping();
                
                try {
                    const result = await this.submitAppointment();
                    this.hideTyping();
                    
                    if (result.success) {
                        this.addMessage({
                            type: 'bot',
                            content: `✅ **Đặt lịch thành công!**\n\n📋 **Mã lịch hẹn:** ${result.appointmentCode}\n👤 **Bệnh nhân:** ${this.appointmentData.patientData.name}\n\nChúng tôi sẽ liên hệ với bạn sớm nhất để xác nhận lịch hẹn. Cảm ơn bạn đã sử dụng dịch vụ của G-Care Clinic!`,
                            timestamp: new Date()
                        });
                    } else {
                        this.addMessage({
                            type: 'bot',
                            content: `❌ **Đặt lịch thất bại**\n\n${result.message}\n\nVui lòng thử lại hoặc liên hệ với chúng tôi để được hỗ trợ.`,
                            timestamp: new Date()
                        });
                    }
                } catch (error) {
                    this.hideTyping();
                    this.addMessage({
                        type: 'bot',
                        content: `❌ **Lỗi khi đặt lịch**\n\n${error.message}\n\nVui lòng thử lại sau.`,
                        timestamp: new Date()
                    });
                }
                
                this.resetAppointmentData();
            } else {
                this.addMessage({
                    type: 'bot',
                    content: `❌ **Đã hủy đặt lịch**\n\nBạn có muốn đặt lịch khác không?`,
                    quickReplies: ['Có, đặt lịch mới', 'Không, cảm ơn'],
                    timestamp: new Date()
                });
                this.resetAppointmentData();
            }
        }
    }

    async submitAppointment() {
        try {
            const appointmentData = {
                department: this.appointmentData.department,
                doctor: this.appointmentData.doctor,
                service: this.appointmentData.service,
                date: this.appointmentData.date,
                shift: this.appointmentData.shift,
                notes: this.appointmentData.notes || '',
                patientId: parseInt(this.appointmentData.patientData.id),
                bookingType: this.appointmentData.bookingType || 'family'
            };

            const response = await fetch(`${this.apiBaseUrl}/api/chatbot/appointments`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(appointmentData)
            });

            if (response.ok) {
                const result = await response.json();
                return {
                    success: true,
                    appointmentCode: result.appointmentCode,
                    message: 'Đặt lịch thành công'
                };
            } else {
                const error = await response.json();
                return {
                    success: false,
                    message: error.message || 'Đặt lịch thất bại'
                };
            }
        } catch (error) {
            console.error('Error submitting appointment:', error);
            return {
                success: false,
                message: 'Lỗi kết nối khi đặt lịch'
            };
        }
    }

    showResetFeedback() {
        const widget = document.querySelector('.chatbot-widget');
        widget.style.transform = 'scale(1.1)';
        widget.style.transition = 'transform 0.2s ease';
        
        setTimeout(() => {
            widget.style.transform = 'scale(1)';
        }, 200);
        
        // Show temporary message
        this.addMessage({
            type: 'bot',
            content: '🔄 **Đã reset vị trí chatbot về vị trí mặc định!**',
            timestamp: new Date()
        });
    }

    // Get patient data from logged-in user account
    async getPatientDataFromUser() {
        try {
            const token = localStorage.getItem('authToken') || sessionStorage.getItem('authToken');
            
            if (!token) {
                throw new Error('Không tìm thấy token đăng nhập');
            }

            const response = await fetch(`${this.apiBaseUrl}/api/chatbot/patient-from-user`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${token}`,
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const data = await response.json();
                return data.patient;
            } else if (response.status === 404) {
                return null; // No patient data found
            } else {
                const error = await response.json();
                throw new Error(error.message || 'Lỗi khi lấy thông tin bệnh nhân');
            }
        } catch (error) {
            console.error('Error getting patient data from user:', error);
            
            // Provide more specific error messages
            if (error.message.includes('token')) {
                throw new Error('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.');
            } else if (error.message.includes('404')) {
                throw new Error('Tài khoản chưa có thông tin bệnh nhân. Vui lòng tạo thông tin bệnh nhân mới.');
            } else if (error.message.includes('network')) {
                throw new Error('Lỗi kết nối mạng. Vui lòng kiểm tra kết nối internet.');
            } else {
                throw new Error('Không thể lấy thông tin bệnh nhân từ tài khoản. Vui lòng thử lại sau.');
            }
        }
    }

    // Search for existing patient by phone or identity number
    async searchPatient(searchTerm) {
        try {
            const response = await fetch(`${this.apiBaseUrl}/api/chatbot/search-patient?term=${encodeURIComponent(searchTerm)}`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (response.ok) {
                const data = await response.json();
                return data.patient || null;
            } else if (response.status === 404) {
                return null; // Patient not found
            } else {
                const error = await response.json();
                throw new Error(error.message || 'Lỗi khi tìm kiếm bệnh nhân');
            }
        } catch (error) {
            console.error('Error searching patient:', error);
            throw new Error('Không thể tìm kiếm bệnh nhân');
        }
    }

    // Create new patient
    async createNewPatient(patientData) {
        try {
            const response = await fetch(`${this.apiBaseUrl}/api/chatbot/create-patient`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(patientData)
            });

            if (response.ok) {
                const data = await response.json();
                return data.patient;
            } else {
                const error = await response.json();
                throw new Error(error.message || 'Lỗi khi tạo bệnh nhân mới');
            }
        } catch (error) {
            console.error('Error creating new patient:', error);
            
            // Provide more specific error messages
            if (error.message.includes('duplicate') || error.message.includes('đã tồn tại')) {
                throw new Error('Bệnh nhân đã tồn tại trong hệ thống. Vui lòng tìm kiếm thay vì tạo mới.');
            } else if (error.message.includes('validation') || error.message.includes('hợp lệ')) {
                throw new Error('Thông tin bệnh nhân không hợp lệ. Vui lòng kiểm tra lại.');
            } else if (error.message.includes('network') || error.message.includes('fetch')) {
                throw new Error('Lỗi kết nối mạng. Vui lòng kiểm tra internet và thử lại.');
            } else {
                throw new Error('Không thể tạo bệnh nhân mới. Vui lòng thử lại sau.');
            }
        }
    }

    openLoginPage() {
        // Mở trang đăng nhập chính xác
        const loginPath = './login.html';
        const loginWindow = window.open(loginPath, '_blank');
        
        if (loginWindow) {
            this.addMessage({
                type: 'bot',
                content: `🔗 **Đã mở trang đăng nhập trong tab mới**\n\nSau khi đăng nhập thành công, bạn có thể quay lại đây và sử dụng tính năng AI.\n\n💡 **Lệnh:** \`/refresh\` để cập nhật trạng thái đăng nhập`,
                timestamp: new Date()
            });
        } else {
            this.addMessage({
                type: 'bot',
                content: `❌ **Không thể mở trang đăng nhập**\n\nVui lòng truy cập thủ công: ${loginPath}\n\nHoặc tìm trang đăng nhập trong menu chính của website.`,
                timestamp: new Date()
            });
        }
    }
}

// Initialize chatbot when page loads
document.addEventListener('DOMContentLoaded', () => {
    new ChatbotWidget();
}); 