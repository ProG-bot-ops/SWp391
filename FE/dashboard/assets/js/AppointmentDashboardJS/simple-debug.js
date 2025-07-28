// simple-debug.js
// File debug đơn giản để kiểm tra API response
(function() {
    'use strict';

    // Test API response
    window.testAPI = async function() {
        console.log('🧪 Testing API...');
        
        try {
            const response = await fetch('https://localhost:7097/api/appointment/list', {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            console.log('📡 Response Status:', response.status);
            console.log('📡 Response OK:', response.ok);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            console.log('✅ API Response:', data);
            console.log('📊 Data Type:', typeof data);
            console.log('📊 Is Array:', Array.isArray(data));
            
            if (data && data.length > 0) {
                console.log('📊 First appointment:', data[0]);
                console.log('📊 First appointment keys:', Object.keys(data[0]));
            }

            return data;
        } catch (error) {
            console.error('❌ API Error:', error);
            return null;
        }
    };

    // Auto test khi load
    window.addEventListener('load', function() {
        console.log('🧪 Simple Debug loaded');
        console.log('📤 Available function: testAPI()');
        
        // Auto run test after 2 seconds
        setTimeout(() => {
            console.log('🔄 Auto-running API test...');
            window.testAPI();
        }, 2000);
    });

})(); 