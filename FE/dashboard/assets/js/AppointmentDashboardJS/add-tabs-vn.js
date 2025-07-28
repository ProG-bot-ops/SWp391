// add-tabs-vn.js
// 1. Đổi tên tab 'Yêu cầu' thành 'Hoàn thành'
// 2. Thêm tab mới 'Đang khám' giữa 'Sắp tới' và 'Hoàn thành'
// 3. Thêm tab mới 'Đã hủy' với bảng tương tự, tbody rỗng
(function() {
    // Đổi tên tab 'Yêu cầu' thành 'Hoàn thành'
    const tabBtn = document.querySelector('button[data-bs-target="#request"]');
    if (tabBtn) tabBtn.textContent = "Hoàn thành";
    const tabNav = document.querySelector('a[aria-controls="request"]');
    if (tabNav) tabNav.textContent = "Hoàn thành";

    // Thêm tab 'Đang khám'
    const tabList = document.getElementById('appointment-table-tab');
    if (!tabList) return;
    
    // Kiểm tra đã có tab 'Đang khám' chưa
    if (!tabList.querySelector('button[data-bs-target="#inprogress"]')) {
        const inProgressTab = document.createElement('li');
        inProgressTab.className = "nav-item";
        inProgressTab.innerHTML = `
            <button class="nav-link btn" type="button" data-bs-toggle="tab" data-bs-target="#inprogress" role="tab" aria-selected="false">Đang khám</button>
        `;
        // Chèn vào sau tab "Sắp tới" (upcoming) và trước tab "Hoàn thành" (request)
        const upcomingTab = tabList.querySelector('button[data-bs-target="#upcoming"]');
        const requestTab = tabList.querySelector('button[data-bs-target="#request"]');
        if (upcomingTab && requestTab) {
            const upcomingLi = upcomingTab.closest('li');
            upcomingLi.after(inProgressTab);
        } else {
            tabList.appendChild(inProgressTab);
        }
    }

    // Thêm tab 'Đã hủy'
    if (!tabList.querySelector('button[data-bs-target="#cancelled"]')) {
        const cancelTab = document.createElement('li');
        cancelTab.className = "nav-item";
        cancelTab.innerHTML = `
            <button class="nav-link btn" type="button" data-bs-toggle="tab" data-bs-target="#cancelled" role="tab" aria-selected="false">Đã hủy</button>
        `;
        tabList.appendChild(cancelTab);
    }

    // Thêm nội dung tab mới
    const tabContent = document.querySelector('.tab-content');
    if (!tabContent) return;
    
    // Thêm tab pane 'Đang khám'
    if (!tabContent.querySelector('#inprogress')) {
        const inProgressPane = document.createElement('div');
        inProgressPane.className = "tab-pane";
        inProgressPane.id = "inprogress";
        inProgressPane.setAttribute("role", "tabpanel");
        inProgressPane.innerHTML = `
            <div class="table-responsive">
                <table class="table border-end border-start align-middle rounded">
                    <thead class="table-dark">
                        <tr>
                            <th>STT</th>
                            <th>Họ và tên bệnh nhân</th>
                            <th>Tên bác sĩ</th>
                            <th>Phòng khám</th>
                            <th>Ngày</th>
                            <th>Ca</th>
                            <th>Trạng thái</th>
                            <th>Hành động</th>
                        </tr>
                    </thead>
                    <tbody>
                        <!-- Dữ liệu động sẽ render ở đây -->
                    </tbody>
                </table>
            </div>
        `;
        // Chèn vào sau tab pane "upcoming" và trước tab pane "request"
        const upcomingPane = tabContent.querySelector('#upcoming');
        const requestPane = tabContent.querySelector('#request');
        if (upcomingPane && requestPane) {
            upcomingPane.after(inProgressPane);
        } else {
            tabContent.appendChild(inProgressPane);
        }
    }
    
    // Thêm tab pane 'Đã hủy'
    if (!tabContent.querySelector('#cancelled')) {
        const cancelPane = document.createElement('div');
        cancelPane.className = "tab-pane";
        cancelPane.id = "cancelled";
        cancelPane.setAttribute("role", "tabpanel");
        cancelPane.innerHTML = `
            <div class="table-responsive">
                <table class="table border-end border-start align-middle rounded">
                    <thead class="table-dark">
                        <tr>
                            <th>STT</th>
                            <th>Họ và tên bệnh nhân</th>
                            <th>Tên bác sĩ</th>
                            <th>Phòng khám</th>
                            <th>Ngày</th>
                            <th>Ca</th>
                            <th>Trạng thái</th>
                            <th>Hành động</th>
                        </tr>
                    </thead>
                    <tbody>
                        <!-- Dữ liệu động sẽ render ở đây -->
                    </tbody>
                </table>
            </div>
        `;
        tabContent.appendChild(cancelPane);
    }
})(); 