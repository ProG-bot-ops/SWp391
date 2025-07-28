// debug-issue.js
// Script để debug vấn đề không hiển thị dữ liệu
console.log('🔍 Debug Issue Script Loaded');

// Kiểm tra sau 2 giây
setTimeout(function() {
    console.log('🔍 === DEBUGGING DISPLAY ISSUE ===');
    
    // 1. Kiểm tra API endpoints
    console.log('📊 AppointmentAPI exists:', !!window.AppointmentAPI);
    if (window.AppointmentAPI) {
        console.log('📊 AppointmentAPI.appointments.list:', window.AppointmentAPI.appointments.list);
    } else {
        console.error('❌ AppointmentAPI not found!');
    }
    
    // 2. Kiểm tra appState
    console.log('📊 appState exists:', !!window.appState);
    if (window.appState) {
        console.log('📊 appState.appointments:', window.appState.appointments);
        console.log('📊 appState.appointments.length:', window.appState.appointments.length);
    } else {
        console.error('❌ appState not found!');
    }
    
    // 3. Kiểm tra AppointmentLoader
    console.log('📊 AppointmentLoader exists:', !!window.AppointmentLoader);
    if (window.AppointmentLoader) {
        console.log('📊 AppointmentLoader instance:', window.AppointmentLoader);
    } else {
        console.error('❌ AppointmentLoader not found!');
    }
    
    // 4. Kiểm tra DOM elements
    const tabPanes = document.querySelectorAll('.tab-pane');
    console.log('📊 Tab panes found:', tabPanes.length);
    tabPanes.forEach((pane, index) => {
        console.log(`📊 Tab ${index}:`, {
            id: pane.id,
            className: pane.className,
            hasTable: !!pane.querySelector('table'),
            hasTbody: !!pane.querySelector('tbody'),
            tbodyChildren: pane.querySelector('tbody')?.children?.length || 0
        });
    });
    
    // 5. Kiểm tra counter element
    const counterElements = document.querySelectorAll('[data-counter]');
    console.log('📊 Counter elements found:', counterElements.length);
    counterElements.forEach((el, index) => {
        console.log(`📊 Counter ${index}:`, {
            tagName: el.tagName,
            className: el.className,
            id: el.id,
            dataCounter: el.getAttribute('data-counter'),
            textContent: el.textContent.trim()
        });
    });
    
    // 6. Test API call
    console.log('🔗 Testing API call...');
    if (window.AppointmentAPI && window.AppointmentAPI.appointments.list) {
        fetch(window.AppointmentAPI.appointments.list)
            .then(response => {
                console.log('📡 API Response status:', response.status);
                console.log('📡 API Response ok:', response.ok);
                return response.json();
            })
            .then(data => {
                console.log('📡 API Response data:', data);
                console.log('📊 Data type:', typeof data);
                console.log('📊 Is array:', Array.isArray(data));
                
                if (data && data.success && data.data) {
                    console.log('✅ Success format detected');
                    console.log('📊 Data count:', data.data.length);
                } else if (Array.isArray(data)) {
                    console.log('✅ Array format detected');
                    console.log('📊 Array length:', data.length);
                } else {
                    console.log('⚠️ Unknown format');
                }
            })
            .catch(error => {
                console.error('❌ API call failed:', error);
            });
    } else {
        console.error('❌ Cannot test API - AppointmentAPI not available');
    }
    
    // 7. Kiểm tra script loading order
    console.log('📊 Script loading check:');
    const scripts = document.querySelectorAll('script[src*="AppointmentDashboardJS"]');
    console.log('📊 AppointmentDashboardJS scripts found:', scripts.length);
    scripts.forEach((script, index) => {
        console.log(`📊 Script ${index}:`, script.src.split('/').pop());
    });
    
    console.log('🔍 === END DEBUGGING ===');
}, 2000); 