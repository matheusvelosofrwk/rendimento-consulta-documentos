// ===================================
// JavaScript Centralizado - ConsultaDocumentos.Web
// ===================================

(function () {
    'use strict';

    // ========== Sidebar Toggle ==========
    const initSidebarToggle = () => {
        const sidebarToggle = document.querySelector('.navbar-toggle');
        const sidebar = document.querySelector('.sidebar');

        if (sidebarToggle && sidebar) {
            sidebarToggle.addEventListener('click', () => {
                sidebar.classList.toggle('collapsed');

                // Salvar estado no localStorage
                const isCollapsed = sidebar.classList.contains('collapsed');
                localStorage.setItem('sidebarCollapsed', isCollapsed);
            });

            // Restaurar estado do localStorage
            const sidebarCollapsed = localStorage.getItem('sidebarCollapsed');
            if (sidebarCollapsed === 'true') {
                sidebar.classList.add('collapsed');
            }
        }

        // Mobile: toggle sidebar visibility
        if (window.innerWidth <= 768 && sidebarToggle && sidebar) {
            sidebarToggle.addEventListener('click', (e) => {
                e.stopPropagation();
                sidebar.classList.toggle('active');
            });

            // Fechar sidebar ao clicar fora (mobile)
            document.addEventListener('click', (e) => {
                if (!sidebar.contains(e.target) && !sidebarToggle.contains(e.target)) {
                    sidebar.classList.remove('active');
                }
            });
        }
    };

    // ========== Dropdown Navigation ==========
    const initDropdownNav = () => {
        const dropdownToggles = document.querySelectorAll('.nav-dropdown-toggle');

        dropdownToggles.forEach(toggle => {
            toggle.addEventListener('click', (e) => {
                e.preventDefault();
                const parent = toggle.closest('.nav-dropdown');

                // Fechar outros dropdowns
                document.querySelectorAll('.nav-dropdown').forEach(dropdown => {
                    if (dropdown !== parent) {
                        dropdown.classList.remove('active');
                    }
                });

                parent.classList.toggle('active');
            });
        });
    };

    // ========== User Dropdown ==========
    const initUserDropdown = () => {
        const userDropdownToggle = document.querySelector('.user-dropdown-toggle');
        const userDropdown = document.querySelector('.user-dropdown');

        if (userDropdownToggle && userDropdown) {
            userDropdownToggle.addEventListener('click', (e) => {
                e.stopPropagation();
                userDropdown.classList.toggle('active');
            });

            // Fechar ao clicar fora
            document.addEventListener('click', (e) => {
                if (!userDropdown.contains(e.target)) {
                    userDropdown.classList.remove('active');
                }
            });
        }
    };

    // ========== Active Page Highlight ==========
    const highlightActivePage = () => {
        const currentPath = window.location.pathname;
        const navLinks = document.querySelectorAll('.nav-link-sidebar');

        navLinks.forEach(link => {
            const href = link.getAttribute('href');
            if (href && currentPath.includes(href) && href !== '/') {
                link.classList.add('active');

                // Expandir dropdown pai se necessário
                const parentDropdown = link.closest('.nav-dropdown');
                if (parentDropdown) {
                    parentDropdown.classList.add('active');
                }
            }
        });
    };

    // ========== Toast Notifications ==========
    window.showToast = (message, type = 'info') => {
        // Se SweetAlert2 estiver disponível
        if (typeof Swal !== 'undefined') {
            const Toast = Swal.mixin({
                toast: true,
                position: 'top-end',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true,
                didOpen: (toast) => {
                    toast.addEventListener('mouseenter', Swal.stopTimer);
                    toast.addEventListener('mouseleave', Swal.resumeTimer);
                }
            });

            Toast.fire({
                icon: type,
                title: message
            });
        } else {
            // Fallback simples
            alert(message);
        }
    };

    // ========== Confirm Delete com SweetAlert2 ==========
    window.confirmDelete = (id, controller, itemName) => {
        if (typeof Swal !== 'undefined') {
            Swal.fire({
                title: 'Confirmar Exclusão?',
                text: `Tem certeza que deseja excluir: ${itemName || 'este registro'}?`,
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#e74a3b',
                cancelButtonColor: '#858796',
                confirmButtonText: 'Sim, excluir!',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                if (result.isConfirmed) {
                    // Criar form e submeter
                    const form = document.createElement('form');
                    form.method = 'POST';
                    form.action = `/${controller}/Delete/${id}`;

                    // Token antiforgery
                    const token = document.querySelector('input[name="__RequestVerificationToken"]');
                    if (token) {
                        const tokenInput = document.createElement('input');
                        tokenInput.type = 'hidden';
                        tokenInput.name = '__RequestVerificationToken';
                        tokenInput.value = token.value;
                        form.appendChild(tokenInput);
                    }

                    document.body.appendChild(form);
                    form.submit();
                }
            });
        } else {
            // Fallback para confirm nativo
            if (confirm(`Tem certeza que deseja excluir: ${itemName || 'este registro'}?`)) {
                const form = document.createElement('form');
                form.method = 'POST';
                form.action = `/${controller}/Delete/${id}`;

                const token = document.querySelector('input[name="__RequestVerificationToken"]');
                if (token) {
                    const tokenInput = document.createElement('input');
                    tokenInput.type = 'hidden';
                    tokenInput.name = '__RequestVerificationToken';
                    tokenInput.value = token.value;
                    form.appendChild(tokenInput);
                }

                document.body.appendChild(form);
                form.submit();
            }
        }
        return false;
    };

    // ========== Loading Overlay ==========
    window.showLoading = () => {
        const overlay = document.createElement('div');
        overlay.className = 'loader-overlay';
        overlay.id = 'loadingOverlay';
        overlay.innerHTML = '<div class="spinner-modern"></div>';
        document.body.appendChild(overlay);
    };

    window.hideLoading = () => {
        const overlay = document.getElementById('loadingOverlay');
        if (overlay) {
            overlay.remove();
        }
    };

    // ========== DataTables Initialization ==========
    window.initDataTable = (tableSelector, options = {}) => {
        if (typeof $.fn.DataTable !== 'undefined') {
            const defaultOptions = {
                language: {
                    url: '//cdn.datatables.net/plug-ins/1.13.7/i18n/pt-BR.json'
                },
                responsive: true,
                pageLength: 10,
                dom: '<"row"<"col-sm-12 col-md-6"l><"col-sm-12 col-md-6"f>>rtip',
                ...options
            };

            return $(tableSelector).DataTable(defaultOptions);
        }
        return null;
    };

    // ========== Form Validation Enhancement ==========
    const enhanceFormValidation = () => {
        const forms = document.querySelectorAll('form');

        forms.forEach(form => {
            form.addEventListener('submit', function(e) {
                if (!form.checkValidity()) {
                    e.preventDefault();
                    e.stopPropagation();
                }
                form.classList.add('was-validated');
            });
        });
    };

    // ========== Input Masks ==========
    const initInputMasks = () => {
        if (typeof $ !== 'undefined' && typeof $.fn.mask !== 'undefined') {
            // CPF
            $('input[data-mask="cpf"]').mask('000.000.000-00', {
                reverse: false,
                placeholder: '___.___.___-__'
            });

            // CNPJ
            $('input[data-mask="cnpj"]').mask('00.000.000/0000-00', {
                reverse: false,
                placeholder: '__.___.___/____-__'
            });

            // Telefone
            $('input[data-mask="phone"]').mask('(00) 00000-0000', {
                reverse: false,
                placeholder: '(__) _____-____'
            });

            // CEP
            $('input[data-mask="cep"]').mask('00000-000', {
                reverse: false,
                placeholder: '_____-___'
            });

            // Data
            $('input[data-mask="date"]').mask('00/00/0000', {
                reverse: false,
                placeholder: '__/__/____'
            });
        }
    };

    // ========== Smooth Scroll ==========
    const initSmoothScroll = () => {
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                const href = this.getAttribute('href');
                if (href !== '#' && href !== '#!') {
                    e.preventDefault();
                    const target = document.querySelector(href);
                    if (target) {
                        target.scrollIntoView({
                            behavior: 'smooth',
                            block: 'start'
                        });
                    }
                }
            });
        });
    };

    // ========== Auto-hide Alerts ==========
    const initAutoHideAlerts = () => {
        const alerts = document.querySelectorAll('.alert[data-auto-dismiss]');

        alerts.forEach(alert => {
            const timeout = parseInt(alert.getAttribute('data-auto-dismiss')) || 5000;
            setTimeout(() => {
                const bsAlert = new bootstrap.Alert(alert);
                bsAlert.close();
            }, timeout);
        });
    };

    // ========== Tooltips & Popovers ==========
    const initTooltipsAndPopovers = () => {
        // Bootstrap Tooltips
        const tooltipTriggerList = [].slice.call(
            document.querySelectorAll('[data-bs-toggle="tooltip"]')
        );
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });

        // Bootstrap Popovers
        const popoverTriggerList = [].slice.call(
            document.querySelectorAll('[data-bs-toggle="popover"]')
        );
        popoverTriggerList.map(function (popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl);
        });
    };

    // ========== Initialize All ==========
    const init = () => {
        initSidebarToggle();
        initDropdownNav();
        initUserDropdown();
        highlightActivePage();
        enhanceFormValidation();
        initInputMasks();
        initSmoothScroll();
        initAutoHideAlerts();
        initTooltipsAndPopovers();

        console.log('ConsultaDocumentos.Web - Aplicação inicializada com sucesso!');
    };

    // ========== DOM Ready ==========
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }

})();
