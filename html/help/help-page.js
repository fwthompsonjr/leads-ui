
const actv = "active"
const storage_key = 'theme';
const toggleButton = document.getElementById('darkModeToggle');
// Get current URL path
const currentPath = window.location.pathname.replace('/help', '');
// Get all nav links
const navLinks = document.querySelectorAll('.nav-link');
// Get content placeholder
const placeHolder = document.getElementById('help-content-place-holder');
const toggle_icons = {
    "dark": '🌙',
    "light": '☀️',
    "name": 'dark-mode',
    "types": {
        "dark": 'dark',
        "light": 'light'
    },
    getStatus: function () {
        return document.body.classList.contains(toggle_icons.name);
    },
    getStatusCode: function () {
        let isDark = toggle_icons.getStatus();
        return isDark ? toggle_icons.types.dark : toggle_icons.types.light;
    },
    getToggleIcon: function () {
        let isDark = toggle_icons.getStatus();
        return isDark ? toggle_icons.light : toggle_icons.dark;
    }
}
let view_profile = {
    onload: function () {
        const carousel = document.getElementById('viewProfileCarousel');

        if (null == carousel || undefined == carousel) return;
        view_profile.set_section('name');
        view_profile.set_action('name');
        view_profile.set_item(0);
        const dvitems = carousel.querySelectorAll(".carousel-item")
        
        carousel.addEventListener('slid.bs.carousel', function (event) {
            const activeIndex = event.to;
            const dv = dvitems[activeIndex];
            view_profile.set_item(activeIndex);
            let dcontext = dv.getAttribute('data-context');
            if (null != dcontext && undefined != dcontext) {
                view_profile.set_section(dcontext);
                view_profile.set_action(dcontext);
            }
        });
    },
    set_section: function (name) {
        const sections = document.getElementById('view-profile-sections');
        let li = sections.querySelectorAll('li[data-context]');
        li.forEach(l => {
            let isactive = l.getAttribute('data-context') == name;
            if (isactive) {
                l.classList.add('text-primary');
            } else {
                l.classList.remove('text-primary');
            }
        });
    },
    set_action: function (name) {
        const sections = document.getElementById('dv-view-profile-actions');
        let lists = sections.querySelectorAll('ol');
        lists.forEach(l => {
            let isactive = l.getAttribute('data-context') == name;
            if (isactive) {
                l.classList.remove('d-none');
            } else {
                l.classList.add('d-none');
            }
        });
    },
    set_item: function (id) {
        const sections = document.getElementById('dv-view-profile-actions');
        let lists = sections.querySelectorAll('li[data-index]');
        lists.forEach(l => {
            let isactive = l.getAttribute('data-index') == id;
            if (isactive) {
                l.classList.add('text-info');
            } else {
                l.classList.remove('text-info');
            }
        });
    }
}

let view_searches = {
    onload: function () {
        const carousel = document.getElementById('viewSearchHistoryCarousel');

        if (null == carousel || undefined == carousel) return;
        view_searches.set_section('fields');
        view_searches.set_action('fields');
        view_searches.set_item(0);
        const dvitems = carousel.querySelectorAll(".carousel-item")

        carousel.addEventListener('slid.bs.carousel', function (event) {
            const activeIndex = event.to;
            const dv = dvitems[activeIndex];
            view_searches.set_item(activeIndex);
            let dcontext = dv.getAttribute('data-context');
            if (null != dcontext && undefined != dcontext) {
                view_searches.set_section(dcontext);
                view_searches.set_action(dcontext);
            }
        });
    },
    set_section: function (name) {
        const sections = document.getElementById('view-searches-sections');
        let li = sections.querySelectorAll('li[data-context]');
        li.forEach(l => {
            let isactive = l.getAttribute('data-context') == name;
            if (isactive) {
                l.classList.add('text-primary');
            } else {
                l.classList.remove('text-primary');
            }
        });
    },
    set_action: function (name) {
        const sections = document.getElementById('dv-view-searches-actions');
        let lists = sections.querySelectorAll('ol');
        lists.forEach(l => {
            let isactive = l.getAttribute('data-context') == name;
            if (isactive) {
                l.classList.remove('d-none');
            } else {
                l.classList.add('d-none');
            }
        });
    },
    set_item: function (id) {
        const sections = document.getElementById('dv-view-searches-actions');
        let lists = sections.querySelectorAll('li[data-index]');
        lists.forEach(l => {
            let isactive = l.getAttribute('data-index') == id;
            if (isactive) {
                l.classList.add('text-info');
            } else {
                l.classList.remove('text-info');
            }
        });
    }
}

let account_settings = {
    "onload": function () {
        const listItems = document.querySelectorAll('.account-settings ul li');
        const accordionItems = document.querySelectorAll('.accordion-item');
        const carousel = document.getElementById('changePasswordCarousel');
        const steps = document.querySelectorAll('#change-password-steps li');
        const dvpwdcaptions = carousel.querySelectorAll(".carousel-caption")

        dvpwdcaptions.forEach(dvi => {
            let childern = Array.prototype.slice.call(dvi.children, 0);
            childern.forEach(c => c.classList.add('d-none'));
        });
        carousel.addEventListener('slid.bs.carousel', function (event) {
            const activeIndex = event.to;

            steps.forEach((step, index) => {
                let dvi = dvpwdcaptions[index];
                let childern = Array.prototype.slice.call(dvi.children, 0);
                let txt = childern[1].innerText;
                let actionTbx = document.getElementById('change-password-action');
                if (null !== actionTbx) { actionTbx.innerText = txt; }
                if (index === activeIndex) {
                    step.classList.add('text-primary');
                } else {
                    step.classList.remove('text-primary');
                }
            });
        });
        accordionItems.forEach((item, index) => {
            const button = item.querySelector('.accordion-button');
            const collapse = item.querySelector('.accordion-collapse');

            // When accordion is shown
            collapse.addEventListener('show.bs.collapse', () => {
                button.classList.add('text-primary');
                // Match <li> by innerText
                const label = button.textContent.trim();
                listItems.forEach(li => {
                    if (li.textContent.trim() === label) {
                        li.classList.add('text-primary');
                    }
                });
            });

            // When accordion is hidden
            collapse.addEventListener('hide.bs.collapse', () => {
                button.classList.remove('text-primary');

                const label = button.textContent.trim();
                listItems.forEach(li => {
                    if (li.textContent.trim() === label) {
                        li.classList.remove('text-primary');
                    }
                });
            });
        });
    },
    "highlight_selection": function (indx) {
        if (isNaN(indx)) { return; }
        let ul = Array.prototype.slice.call(document.getElementsByTagName('ul'), 0);
        let ulsteps = ul.find(x => x.getAttribute('name') == 'change-password-steps');
        if (null == ulsteps) { return; }
        let items = Array.prototype.slice.call(ulsteps.getElementsByTagName('li'), 0);
        for (let i = 0; i < items.length; i++) {
            let li = items[i];
            if (i == indx) {
                li.classList.add('text-primary');
            } else {
                li.classList.remove('text-primary');
            }
        }
    },
    "handle_click": function () {
        document.querySelectorAll("li[name='account-settings-option']").forEach((item, index) => {
            item.style.cursor = 'pointer';
            item.addEventListener("click", () => {
                const accordionIds = ["collapsePassword", "collapseProfile", "collapseSearchHistory", "collapseInvoices"];
                const targetId = accordionIds[index];
                const target = document.getElementById(targetId);

                if (target) {
                    const bsCollapse = new bootstrap.Collapse(target, {
                        toggle: false
                    });

                    if (target.classList.contains("show")) {
                        bsCollapse.hide();
                    } else {
                        bsCollapse.show();
                    }
                }
            });
        });
    }
}

let orchestrator = {
    "title": function () {
        let links = Array.prototype.slice.call(navLinks, 0);
        let target = links.find(f => f.classList.contains(actv));
        if (undefined === target || null === target) { return }
        let txt = "".concat("Topic: ", target.innerText);
        document.title = txt;
    },
    "toggle": function () {
        // Apply click event handler for dark mode
        toggleButton.addEventListener('click', () => {
            let isDark = toggle_icons.getStatus();
            if (isDark) { document.body.classList.remove(toggle_icons.name) }
            else { document.body.classList.toggle(toggle_icons.name); }
            localStorage.setItem(storage_key, toggle_icons.getStatusCode());
            toggleButton.innerText = toggle_icons.getToggleIcon();
        });
    },
    "placeholder": function () {
        placeHolder.style.display = 'block';
        let links = Array.prototype.slice.call(navLinks, 0);
        let target = links.find(f => f.classList.contains(actv));
        if (undefined === target || null === target) { return; }
        placeHolder.style.display = 'none';
    },
    "navigation": function () {
        navLinks.forEach(link => {
            let linkRef = link.href.replace('#', '');
            // Compare href with current path
            if (currentPath.length > 0 && linkRef.length > 0 && linkRef.indexOf(currentPath) >= 0) {
                link.classList.add(actv);
            } else {
                link.classList.remove(actv);
            }
        });
        orchestrator.title();
    },
    "window": function () {
        // Apply saved theme on load
        window.addEventListener('DOMContentLoaded', () => {
            const theme = localStorage.getItem(storage_key);
            if (theme != toggle_icons.types.dark) {
                document.body.classList.remove(toggle_icons.name);
                toggleButton.textContent = toggle_icons.dark;
                localStorage.setItem(storage_key, toggle_icons.types.light);
            } else {
                toggleButton.textContent = toggle_icons.light;
                localStorage.setItem(storage_key, toggle_icons.types.dark);
            }
        });
    },
    "initialize": function () {
        orchestrator.window();
        orchestrator.toggle();
        orchestrator.navigation();
        orchestrator.placeholder();
        window.addEventListener('DOMContentLoaded', account_settings.onload);
        window.addEventListener('DOMContentLoaded', view_profile.onload);
        window.addEventListener('DOMContentLoaded', view_searches.onload);
        account_settings.handle_click();
    }
}
orchestrator.initialize();