// simple-api-test.js
// Script test API đơn giản
console.log('🚀 Simple API Test Script Loaded');

// Test ngay lập tức
setTimeout(async function() {
    console.log('🚀 === SIMPLE API TEST START ===');
    
    try {
        console.log('🔗 Testing API...');
        const response = await fetch('https://localhost:7097/api/appointment/list');
        console.log('📡 Response status:', response.status);
        
        if (response.ok) {
            const data = await response.json();
            console.log('✅ API Response:', data);
            console.log('📊 Data type:', typeof data);
            console.log('📊 Is array:', Array.isArray(data));
            
            if (data && data.success && data.data) {
                console.log('✅ Success format detected');
                console.log('📊 Data count:', data.data.length);
                if (data.data.length > 0) {
                    console.log('📊 First appointment:', data.data[0]);
                    console.log('📊 First appointment keys:', Object.keys(data.data[0]));
                }
            } else if (Array.isArray(data)) {
                console.log('✅ Array format detected');
                console.log('📊 Array length:', data.length);
                if (data.length > 0) {
                    console.log('📊 First appointment:', data[0]);
                    console.log('📊 First appointment keys:', Object.keys(data[0]));
                }
            } else {
                console.log('⚠️ Unknown format:', data);
            }
        } else {
            console.error('❌ API failed:', response.status);
        }
    } catch (error) {
        console.error('❌ Test failed:', error);
    }
    
    console.log('🚀 === SIMPLE API TEST END ===');
}, 1000); 