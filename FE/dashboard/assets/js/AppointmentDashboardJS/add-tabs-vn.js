// add-tabs-vn.js
// Tạo lại hoàn toàn 4 tab bằng JavaScript với format đồng nhất
document.addEventListener('DOMContentLoaded', function() {
    // Function tạo table header với format thống nhất
    function createTableHeader() {
        return `
            <div class="table-responsive">
                <table class="table border-end border-start align-middle rounded" style="width: 100%;">
                    <thead class="table-dark">
                        <tr>
                            <th scope="col" style="text-align: left !important; padding: 12px 8px !important;">STT</th>
                            <th scope="col" style="text-align: left !important; padding: 12px 8px !important;">Họ Và Tên Bệnh Nhân</th>
                            <th scope="col" style="text-align: left !important; padding: 12px 8px !important;">Tên Bác Sĩ</th>
                            <th scope="col" style="text-align: left !important; padding: 12px 8px !important;">Phòng Khám</th>
                            <th scope="col" style="text-align: left !important; padding: 12px 8px !important;">Ngày</th>
                            <th scope="col" class="text-center" style="text-align: center !important; text-align-last: center !important; -webkit-text-align-last: center !important; padding: 12px 8px !important; width: 80px !important;">Ca</th>
                            <th scope="col" class="text-center" style="text-align: center !important; text-align-last: center !important; -webkit-text-align-last: center !important; padding: 12px 8px !important; width: 120px !important;">Trạng Thái</th>
                            <th scope="col" style="text-align: center !important; padding: 12px 8px !important;">Hành Động</th>
                        </tr>
                    </thead>
                    <tbody>
                        <!-- Dữ liệu động sẽ render ở đây -->
                    </tbody>
                </table>
            </div>
        `;
    }

    // Đợi một chút để đảm bảo DOM đã sẵn sàng
    setTimeout(function() {
        // Lưu nút "Add Appointment" và "Filter" trước khi xóa
        const tabList = document.getElementById('appointment-table-tab');
        let addAppointmentButton = null;
        let filterDropdown = null;
        
        if (tabList) {
            // Lưu nút "Add Appointment"
            const addButton = tabList.querySelector('a[href="#offcanvasAppointmentAdd"]');
            if (addButton) {
                addButton.parentElement.remove(); // Xóa li chứa nút
                addAppointmentButton = addButton.outerHTML;
                console.log('Đã lưu nút "Add Appointment"');
            }
            
            // Lưu dropdown filter
            const filterLi = tabList.querySelector('.dropdown');
            if (filterLi) {
                filterLi.remove();
                filterDropdown = filterLi.outerHTML;
                console.log('Đã lưu dropdown filter');
            }
            
            // Xóa tất cả tab buttons hiện tại
            tabList.innerHTML = '';
            console.log('Đã xóa tất cả tab buttons hiện tại');
        }

        // Xóa tất cả tab panes hiện tại
        const tabContent = document.querySelector('.tab-content');
        if (tabContent) {
            tabContent.innerHTML = '';
            console.log('Đã xóa tất cả tab panes hiện tại');
        }

        // Tạo lại 4 tab buttons
        const tabs = [
            { id: 'upcoming', text: 'Sắp tới', active: true },
            { id: 'inprogress', text: 'Đang khám', active: false },
            { id: 'completed', text: 'Đã hoàn thành', active: false },
            { id: 'cancelled', text: 'Đã hủy', active: false }
        ];

        // Thêm lại nút "Add Appointment" và "Filter" vào đầu
        if (addAppointmentButton) {
            const addLi = document.createElement('li');
            addLi.innerHTML = addAppointmentButton;
            tabList.appendChild(addLi);
            console.log('Đã thêm lại nút "Add Appointment"');
        }
        
        if (filterDropdown) {
            const filterLi = document.createElement('li');
            filterLi.innerHTML = filterDropdown;
            tabList.appendChild(filterLi);
            console.log('Đã thêm lại dropdown filter');
        }

        tabs.forEach((tab, index) => {
            // Tạo tab button
            const tabLi = document.createElement('li');
            tabLi.className = "nav-item";
            tabLi.setAttribute("role", "presentation");
            tabLi.innerHTML = `
                <button class="nav-link btn ${tab.active ? 'active' : ''}" type="button" data-bs-toggle="tab" 
                        data-bs-target="#${tab.id}" role="tab" aria-selected="${tab.active}">${tab.text}</button>
            `;
            tabList.appendChild(tabLi);

            // Tạo tab pane
            const tabPane = document.createElement('div');
            tabPane.className = `tab-pane ${tab.active ? 'active' : ''}`;
            tabPane.id = tab.id;
            tabPane.setAttribute("role", "tabpanel");
            tabPane.innerHTML = createTableHeader();
            tabContent.appendChild(tabPane);

            console.log(`Đã tạo tab: ${tab.text} (${tab.id})`);
        });

        console.log('Hoàn thành tạo lại 4 tab với format đồng nhất và giữ nguyên nút Add Appointment');
        
        // Trigger event để thông báo rằng tabs đã được tạo xong
        const event = new CustomEvent('tabsReady', { detail: { tabs: tabs } });
        document.dispatchEvent(event);
    }, 100);
}); 