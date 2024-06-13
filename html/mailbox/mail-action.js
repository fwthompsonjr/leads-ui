let mailbox = {
    "isWorking": false,
    "controls": {
        "template": "mail-item-template",
        "maillist": "dv-mail-item-list",
        "preview": "dv-mail-item-preview",
        "previewframe": "#dv-mail-item-preview-frame",
        "textarea": "tarea-json",
        "previewtemplate": "tarea-preview-html",
        "previewcurrent": "tarea-current-html",
        "nodata": "dv-mail-item-no-mail",
        "subheader": "mailbox-sub-header"
    },
    "clear": function () {
        let mbox = mailbox.controls.maillist;
        let element = document.getElementById(mbox);
        while (true) {
            if (element.children.length == 0) { break; }
            let itms = element.children.length - 1;
            let target = element.children[itms];
            element.removeChild(target);
        }
    },
    "createChild": function () {
        let mbox = mailbox.controls.maillist;
        let template = mailbox.controls.template;
        const node = document.getElementById(template);
        const clone = node.cloneNode(true);
        let element = document.getElementById(mbox);
        clone.removeAttribute("id");
        clone.setAttribute("data-item-index", "1");
        element.appendChild(clone);
    },
    "getElementText": function (elementName) {
        try {
            let js = document.getElementById(elementName).innerText.trim();
            return js;
        } catch {
            return "";
        }
    },
    "preview": {
        "clear": function () {
            let html = mailbox.getElementText(mailbox.controls.previewtemplate);
            document.getElementById(mailbox.controls.preview).innerHTML = html;
        },
        "populate": function () {
            let html = mailbox.getElementText(mailbox.controls.previewcurrent);
            if (html.length == 0) {
                mailbox.preview.clear();
                return;
            }
            document.getElementById(mailbox.controls.preview).innerHTML = html;
        }
    },
    "populate": function () {
        if (mailbox.isWorking) { return; }
        mailbox.isWorking = true;
        try {
            mailbox.clear();
            mailbox.preview.clear();
            let mbox = mailbox.controls.maillist;
            let js = mailbox.getElementText(mailbox.controls.textarea);
            let list = JSON.parse(js);
            for (const element of list) {
                mailbox.createChild();
                let target = document.getElementById(mbox).lastChild;
                mailbox.populateNode(target, element);
            }
            mailbox.onPopulationCompleted();

        } finally {
            mailbox.isWorking = false;
        }
    },
    "populateNode": function (target, obj) {
        let header = target.children[0];
        let detail = target.children[1];
        let subject = header.children[0];
        let createDt = header.children[1];
        let toaddress = detail.children[0];
        let fromaddress = detail.children[1];
        let indexId = detail.children[2];
        subject.innerText = obj["subject"];
        createDt.innerText = obj["createDate"];
        toaddress.innerText = "".concat("To: ", obj["toAddress"]);
        fromaddress.innerText = "".concat("From: ", obj["fromAddress"]);
        indexId.innerText = obj["id"];
    },
    "onPopulationCompleted": function () {
        const txt = "Correspondence";
        const mbox = document.getElementById(mailbox.controls.maillist);
        const count = mbox.childElementCount;
        const nodata = document.getElementById(mailbox.controls.nodata);
        const heading = document.getElementById(mailbox.controls.subheader);
        if (count == 0) {
            nodata.setAttribute("data-item-count", "0");
            heading.innerText = txt;
            return;
        }
        nodata.setAttribute("data-item-count", "1");
        heading.innerText = "".concat(txt, " ( ", count, " )");
    },
    "fetch": {
        "data": function () {
            // get json per user
        },
        "item": function (id) {
            // get html content based on item id
        }
    }
}

function fetch_item(id) {
    if (isNaN(id)) { return; }
    mailbox.fetch.item(id);
}

mailbox.populate();