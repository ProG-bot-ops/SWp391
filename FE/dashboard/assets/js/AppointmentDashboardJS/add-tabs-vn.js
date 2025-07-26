// add-tabs-vn.js
// 1. Đổi tên tab 'Yêu cầu' thành 'Hoàn thành'
// 2. Thêm tab mới 'Đã hủy' với bảng tương tự, tbody rỗng
(function() {
    // Đổi tên tab 'Yêu cầu' thành 'Hoàn thành'
    const tabBtn = document.querySelector('button[data-bs-target="#request"]');
    if (tabBtn) tabBtn.textContent = "Hoàn thành";
    const tabNav = document.querySelector('a[aria-controls="request"]');
    if (tabNav) tabNav.textContent = "Hoàn thành";

    // Thêm tab 'Đã hủy'
    const tabList = document.getElementById('appointment-table-tab');
    if (!tabList) return;
    // Kiểm tra đã có tab 'Đã hủy' chưa
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
                            <th>Ngày & Giờ</th>
                            <th>Lý do</th>
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