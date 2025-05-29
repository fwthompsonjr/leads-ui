let account_settings = {
    "onload": function () {
        const listItems = document.querySelectorAll("ul[name='account-settings-options'] li");
        const accordionItems = document.querySelectorAll('.accordion-item');
        if (listItems.length == 0) { return; }
        listItems.forEach(li => {
            li.addEventListener('click', () => {
                const selectedText = li.textContent.trim();

                // Remove active class from all list items
                listItems.forEach(item => item.classList.remove('text-primary'));
                li.classList.add('text-primary');

                accordionItems.forEach(item => {
                    const button = item.querySelector('.accordion-button');
                    const collapse = item.querySelector('.accordion-collapse');
                    const buttonText = button.textContent.trim();

                    if (buttonText === selectedText) {
                        // Expand if not already shown
                        if (!collapse.classList.contains('show')) {
                            new bootstrap.Collapse(collapse, { toggle: true });
                        }
                        button.classList.add('text-primary');
                    } else {
                        // Collapse if currently shown
                        if (collapse.classList.contains('show')) {
                            new bootstrap.Collapse(collapse, { toggle: true });
                        }
                        button.classList.remove('text-primary');
                    }
                });
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
    }
}