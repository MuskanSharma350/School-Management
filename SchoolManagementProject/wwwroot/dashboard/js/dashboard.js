const toggleDarkMode = () => {
    const elements = [
        document.body,
        document.querySelector('.sidebar'),
        document.querySelector('.topbar'),
        document.querySelector('.breadcrumb'),
        document.querySelector('.logout-btn')
    ];

    const cards = document.querySelectorAll('.card');

    elements.forEach(el => el && el.classList.toggle('dark-mode'));
    cards.forEach(card => card.classList.toggle('dark-mode'));
};

const animateCards = () => {
    document.querySelectorAll('.card').forEach(card => {
        card.classList.add('animate__animated', 'animate__fadeInUp');
    });
};

const loadNotifications = () => {
    const notificationIcon = document.querySelector('.notifications i');

    if (notificationIcon) {
        notificationIcon.classList.add('animate__animated', 'animate__shakeX');
        setTimeout(() => {
            if (notificationIcon) notificationIcon.classList.remove('animate__shakeX');
        }, 1000);
    }
};

const showNotifications = () => {
    const notifications = document.querySelector('.notifications');

    if (notifications) {
        notifications.innerHTML = `<i class="fas fa-bell"></i><span class="badge">3</span>`;
        setTimeout(() => {
            const badge = notifications.querySelector('.badge');
            if (badge) badge.remove();
        }, 3000);
    }
};

const addLogoutAnimation = () => {
    const logoutButton = document.querySelector('.logout-btn');
    if (logoutButton) {
        logoutButton.addEventListener('click', function () {
            this.classList.add('animate__animated', 'animate__bounce');
            setTimeout(() => this.classList.remove('animate__bounce'), 1000);
        });
    }
};
const toggleSidebar = () => {
    const sidebar = document.querySelector('.sidebar');
    sidebar.classList.toggle('collapsed');
};


const handleSubmenus = () => {
    $('.sidebar-menu li > a[data-toggle="submenu"]').click(function (e) {
        e.preventDefault();

        const submenu = $(this).next('.submenu'); 
        $('.submenu').not(submenu).slideUp(); 
        submenu.slideToggle(300); 
    });

    $('.submenu li > a').click(function () {
    });
};

window.onload = () => {
    animateCards();
    showNotifications();
    loadNotifications();
    addLogoutAnimation();

    const darkModeToggle = document.querySelector('#darkModeToggle');
    if (darkModeToggle) {
        darkModeToggle.addEventListener('click', toggleDarkMode);
    }

    const sidebarToggle = document.querySelector('#toggleSidebar');
    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', toggleSidebar);
    }
};

$(document).ready(() => {
    handleSubmenus();
});
