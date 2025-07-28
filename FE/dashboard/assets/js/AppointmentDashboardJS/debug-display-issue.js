// debug-display-issue.js
// Script để debug vấn đề không hiển thị dữ liệu
console.log('🔍 Debug Display Issue Script Loaded');

// Kiểm tra sau 3 giây
setTimeout(function() {
    console.log('🔍 === DEBUGGING DISPLAY ISSUE ===');
    
    // 1. Kiểm tra API response
    console.log('🔗 Testing API response...');
    fetch('https://localhost:7097/api/appointment/list')
        .then(response => response.json())
        .then(data => {
            console.log('📡 Raw API Response:', data);
            console.log('📊 Data type:', typeof data);
            console.log('📊 Is array:', Array.isArray(data));
            
            let appointments = [];
            if (data && data.success && data.data) {
                appointments = data.data;
                console.log('✅ Success format detected');
            } else if (Array.isArray(data)) {
                appointments = data;
                console.log('✅ Array format detected');
            } else {
                console.log('⚠️ Unknown format');
                return;
            }
            
            console.log('📊 Appointments count:', appointments.length);
            if (appointments.length > 0) {
                console.log('📊 First appointment:', appointments[0]);
                console.log('📊 First appointment keys:', Object.keys(appointments[0]));
            }
            
            // 2. Test filter logic
            console.log('🔍 Testing filter logic...');
            const today = new Date();
            today.setHours(0, 0, 0, 0);
            
            // Test tab 0 (Sắp tới)
            const tab0Filtered = appointments.filter(appointment => {
                let appointmentDate;
                try {
                    appointmentDate = new Date(appointment.date);
                    appointmentDate.setHours(0, 0, 0, 0);
                } catch (e) {
                    return false;
                }
                
                return appointmentDate >= today && (
                    appointment.status === 'PENDING' ||
                    appointment.status === 'pending' ||
                    appointment.status === 'SCHEDULED' ||
                    appointment.status === 'scheduled' ||
                    appointment.status === 'Scheduled'
                );
            });
            console.log('📊 Tab 0 (Sắp tới) filtered count:', tab0Filtered.length);
            
            // Test tab 1 (Đang khám)
            const tab1Filtered = appointments.filter(appointment => {
                return appointment.status === 'IN_PROGRESS' ||
                       appointment.status === 'in_progress' ||
                       appointment.status === 'InProgress' ||
                       appointment.status === 'In_Progress' ||
                       appointment.status === 'INPROGRESS' ||
                       appointment.status === 'inprogress' ||
                       appointment.status === 'Inprogress';
            });
            console.log('📊 Tab 1 (Đang khám) filtered count:', tab1Filtered.length);
            
            // Test tab 2 (Đã hoàn thành)
            const tab2Filtered = appointments.filter(appointment => {
                return appointment.status === 'COMPLETED' ||
                       appointment.status === 'completed' ||
                       appointment.status === 'Completed' ||
                       appointment.status === 'Complete' ||
                       appointment.status === 'complete';
            });
            console.log('📊 Tab 2 (Đã hoàn thành) filtered count:', tab2Filtered.length);
            
            // Test tab 3 (Đã hủy)
            const tab3Filtered = appointments.filter(appointment => {
                return appointment.status === 'CANCELLED' ||
                       appointment.status === 'cancelled' ||
                       appointment.status === 'Cancelled' ||
                       appointment.status === 'Cancel' ||
                       appointment.status === 'cancel';
            });
            console.log('📊 Tab 3 (Đã hủy) filtered count:', tab3Filtered.length);
            
            // 3. Test data mapping
            console.log('🔍 Testing data mapping...');
            if (appointments.length > 0) {
                const appointment = appointments[0];
                const patientName = appointment.name || appointment.patientName || appointment.patient?.name || 'N/A';
                const doctorName = appointment.doctorName || appointment.doctor?.name || 'N/A';
                const clinicName = appointment.clinic || appointment.clinicName || appointment.clinic?.name || 'N/A';
                const date = appointment.date || appointment.appointmentDate || 'N/A';
                const shift = appointment.shift || appointment.shiftName || 'N/A';
                const status = appointment.status || 'N/A';
                
                console.log('📊 Mapped data for first appointment:', {
                    patientName,
                    doctorName,
                    clinicName,
                    date,
                    shift,
                    status
                });
            }
            
            // 4. Check DOM elements
            console.log('🔍 Checking DOM elements...');
            const tabPanes = document.querySelectorAll('.tab-pane');
            console.log('📊 Tab panes found:', tabPanes.length);
            
            tabPanes.forEach((pane, index) => {
                const table = pane.querySelector('table');
                const tbody = table?.querySelector('tbody');
                console.log(`📊 Tab ${index}:`, {
                    id: pane.id,
                    hasTable: !!table,
                    hasTbody: !!tbody,
                    tbodyChildren: tbody?.children?.length || 0,
                    tbodyHTML: tbody?.innerHTML?.substring(0, 100) + '...' || 'No tbody'
                });
            });
            
            // 5. Check if data is being cleared
            console.log('🔍 Checking if data is being cleared...');
            const allTbodies = document.querySelectorAll('tbody');
            allTbodies.forEach((tbody, index) => {
                console.log(`📊 Tbody ${index}:`, {
                    children: tbody.children.length,
                    hasLoadingMessage: tbody.innerHTML.includes('Đang tải'),
                    hasNoDataMessage: tbody.innerHTML.includes('Không có dữ liệu'),
                    hasDataRows: tbody.querySelectorAll('tr[data-appointment-id]').length
                });
            });
            
        })
        .catch(error => {
            console.error('❌ API test failed:', error);
        });
    
    console.log('🔍 === END DEBUGGING ===');
}, 3000); 