// extra-ui-cleanup.js
(function() {
    // 1. Ẩn/xóa phân trang
    document.querySelectorAll('.card-navigation, .pagination, .dataTables_paginate, .card-footer, .pagination-wrapper').forEach(el => el.style.display = 'none');

    // 2. Ẩn/xóa All Cart (giỏ hàng)
    document.querySelectorAll('[aria-labelledby="notification-cart"], .cart-icons, .badge-notification, .sub-drop .card:has(h5), .nav-list-icon .cart-icons').forEach(el => el.style.display = 'none');
    document.querySelectorAll('a[href="#"][id*="notification-cart"], .cart-icons, .badge-notification').forEach(el => el.style.display = 'none');

    // 3. Ẩn/xóa cài đặt (settings/customizer)
    document.querySelectorAll('.live-customizer, .btn-setting, [data-bs-target="#live-customizer"], .offcanvas.live-customizer').forEach(el => el.style.display = 'none');
    // Ẩn menu "Cài Đặt" nếu có
    document.querySelectorAll('a.nav-link, span, li').forEach(el => {
        if (el.textContent && (el.textContent.trim() === 'Cài Đặt' || el.textContent.trim() === 'Cài đặt')) {
            el.style.display = 'none';
        }
    });

    // 4. Ẩn/xóa tìm kiếm
    // KHÔNG ẩn phần chỉnh cỡ chữ (có class .iq-font-style hoặc id #font-size-sm/md/lg)
    document.querySelectorAll('.search-input, [placeholder="Search..."], [placeholder="Tìm kiếm..."], .navbar-nav .dropdown.iq-responsive-menu, .navbar-nav .dropdown.me-0').forEach(el => el.style.display = 'none');
    // Ẩn input tìm kiếm và icon, nhưng KHÔNG ẩn .iq-font-style
    document.querySelectorAll('input[placeholder*="Tìm kiếm"], input[placeholder*="Search"], .input-group-text').forEach(el => {
        // Nếu không nằm trong .iq-font-style thì ẩn
        if (!el.closest('.iq-font-style')) el.style.display = 'none';
    });

    // 5. Ẩn menu "Trang" nếu có
    document.querySelectorAll('a.nav-link, span, li').forEach(el => {
        if (el.textContent && el.textContent.trim() === 'Trang') {
            el.style.display = 'none';
        }
    });

    // KHÔNG ẩn phần Avatar người dùng (icon/avatar, dropdown profile/logout)
    // => Không động chạm tới .icon-40, .dropdown-menu[aria-labelledby="navbarDropdown"], ...
})(); 