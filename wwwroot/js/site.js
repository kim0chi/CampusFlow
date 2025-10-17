// ==================== Sidebar Navigation JavaScript ====================

$(document).ready(function() {
    // Sidebar toggle for mobile
    const sidebar = $('#sidebar');
    const sidebarToggle = $('#sidebarToggle');
    const sidebarOverlay = $('#sidebarOverlay');

    // Toggle sidebar on mobile
    sidebarToggle.on('click', function() {
        sidebar.toggleClass('active');
        sidebarOverlay.toggleClass('active');
        $('body').toggleClass('sidebar-open');
    });

    // Close sidebar when clicking overlay
    sidebarOverlay.on('click', function() {
        sidebar.removeClass('active');
        sidebarOverlay.removeClass('active');
        $('body').removeClass('sidebar-open');
    });

    // Submenu toggle functionality
    $('.menu-item.has-submenu').on('click', function(e) {
        e.preventDefault();
        const submenuId = $(this).data('submenu');
        const submenu = $(`#submenu-${submenuId}`);

        // Toggle current submenu
        $(this).toggleClass('active');
        submenu.toggleClass('show');

        // Close other submenus (optional - comment out if you want multiple open)
        // $('.menu-item.has-submenu').not(this).removeClass('active');
        // $('.submenu').not(submenu).removeClass('show');
    });

    // Prevent submenu links from closing the submenu
    $('.submenu-item').on('click', function(e) {
        e.stopPropagation();
    });

    // Auto-expand parent menu if submenu item is active
    $('.submenu-item.active').each(function() {
        $(this).closest('.submenu').addClass('show');
        $(this).closest('.submenu').prev('.menu-item.has-submenu').addClass('active');
    });

    // Remember sidebar state on desktop (optional)
    const sidebarState = localStorage.getItem('sidebarState');
    if (window.innerWidth > 768) {
        if (sidebarState === 'collapsed') {
            sidebar.addClass('collapsed');
        }
    }

    // Close sidebar on mobile when navigating
    $('.menu-item:not(.has-submenu)').on('click', function() {
        if (window.innerWidth <= 768) {
            sidebar.removeClass('active');
            sidebarOverlay.removeClass('active');
            $('body').removeClass('sidebar-open');
        }
    });

    // Handle window resize
    $(window).on('resize', function() {
        if (window.innerWidth > 768) {
            sidebar.removeClass('active');
            sidebarOverlay.removeClass('active');
            $('body').removeClass('sidebar-open');
        }
    });

    // Smooth scrolling for anchor links
    $('a[href^="#"]').on('click', function(e) {
        const target = $(this.getAttribute('href'));
        if (target.length) {
            e.preventDefault();
            $('html, body').stop().animate({
                scrollTop: target.offset().top - 80
            }, 500);
        }
    });

    // Add keyboard shortcuts
    $(document).on('keydown', function(e) {
        // ESC key to close sidebar on mobile
        if (e.key === 'Escape' && sidebar.hasClass('active')) {
            sidebar.removeClass('active');
            sidebarOverlay.removeClass('active');
            $('body').removeClass('sidebar-open');
        }
    });

    // Prevent body scroll when sidebar is open on mobile
    $('body').on('touchmove', function(e) {
        if (sidebar.hasClass('active') && !$(e.target).closest('.sidebar').length) {
            e.preventDefault();
        }
    });
});
