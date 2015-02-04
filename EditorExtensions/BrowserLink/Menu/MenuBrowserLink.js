/// <reference path="../_intellisense/browserlink.intellisense.js" />
/// <reference path="../_intellisense/jquery-1.8.2.js" />

(function (browserLink, $) {
    /// <param name="browserLink" value="bl" />
    /// <param name="$" value="jQuery" />

    var _menu, _bar, _fixed, _toggle;

    function CreateMenu() {
        _menu = document.createElement("bl");
        _menu.addButton = AddButton;
        _menu.addCheckbox = AddCheckbox;
        _menu.style.display = "none";

        var logo = document.createElement("img");
        logo.src = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACgAAAAoCAMAAAC7IEhfAAACplBMVEX/////AAB/AAD/fwD/VQD/Pz//VSriVBzdRCLuVSLfTx/hSy3lTCbdTSHfSirgUSjjSCTlTCLkSSjkUCjmTiTiTyfoTyfhTifiTCbmTCbjTSThSyXkTCbkSyXjSyXjTiXjTCTjSyTkTSbiTCTlTCTjSyXkTSXkTCTiSyfkTCblTSbjTCbkTCbkTCXlTSbjTCbiTSbkTCblTSXkTSXkTSXkTCXjTSXjTSbkTSXjTSXjTSbkTSXkTSbjTCbjTSXkTCbkTCbiTSbkTSbjTCbjTSXjTCXjTSbjTCXkTCXjTCXkTCXjTCXjTSXjTCbjTSbkTSbjTCbkTSbkTCbjTCbjTSbjTCXkTCbkTSbjTSbjTCXkTSbjTCXjTSXkTSXjTCbkTSb////xZSnkTibrWSjoVCftXijpVifqVyfqWCfqd1rlTibrWijtXSjob1DvYinwZivxZCnlTybyfUr0jmL6z7376OP76uX97+r99PL99/T9+ffmUSbpVyfmUCbkUCnqWSfmUSfqfGDmXzzmYT/rWyjrfmLsWyjsXCjsbkbsiG7mZUTnUiftflvtinDtkHjuXyjuYCjulH3vYSjnUyfwYinwZCnnYT/waC3nZEPnZkTxazHxcDjxcDnxcTryeETye0jyfErnZkXyfk3ysJ/zglHzi1/zjF/zjWLztaXztqb0i17kUCr0kmn0u6z1mXH1rZH2poP2yLz2zsP3rY73r5D30cf4w674xK741s75zLj50L753NXoVSf65uD73M/74db75Nr75uD75+LobU3obk776ub77Oj77ur859z86OD88Ov88e/88/D89fT97+njTyn98ezjUSvlUCb99/X9+Pb9+Pfpdln+9fL+9/X++ff++vj/+/n//Pv//fz//f3pd1rpeFv//v6yzbn+AAAAX3RSTlMAAQICAwQGCQ8PEBEUFxgZHB4mJiotLTQ1NTg9Q0RKS1NUVlpaXmBhYmpwcXJ1d3h9f4CHjo+Xl5uenqSlpqittLW5u7zExsvL0tLY2dvh4eLk6Onq7O/w8fL3+fv7/UUm+e4AAAJFSURBVHhehdJlYxQ7GIbhLO5erEhxitNiRU6RLl5cCpQ+mZW6u+Du7u7u7u7ubuefMJkkm2F2u70/JXmvT0kIIVEoqShiFFkijOTQDkuODHOJgJ3DcCt0auZWA+Ec9vQPk4EeHIb4hwuAEA6D/cPZQDCHQf7hDCCIw0Bf8PCXy1lpBnQBgRzW9gEPFlJK75zZocPlQC0OK3nDQ7pjFehwJlCBQ9s0Kzz2gSrowFQb4Y2G3tu5kK34ShXcCIwioqEAVj2m52Zx9+k9NcFUYIiEAwBc149z7jOXwF2hgGuA/hL2FpDmrwR+f2arb6euCegGwiQMNeDZu5T+3J/wnIEnB7RsARcBoRK2N+CReTf10f9snr1T80An0E7Clhxi/j1KmT29WVMwDmghYWMBsdSAJ52aCd4AGklYV8CUFwb8dWm7Ca4HAiSsyuED5j6y+a3dChYBVSQsG8PgxXxKX67dksNAXpYHJiK6DJGNF/f48A+wNZetfpwX95gOjCOehnN4ZRP0lj2iIgY3AMMUjAAW5tKjx2G0l0kJ1wCDFOwL4HUKZEteKegG+ijY3fIf9zzzwMVANwU7W6CWmcfh021JQCcF21qhlvmG0u9XT6Rp8UAbBZt6QW3f7Qu7NL1YoImC9a1Q5QLqKVizeDgHqKFg+WJhagZiyhHVFF8w3R1XBGASMTXSC65LcjlgNMIMq7f+b6KC8cmx78Cb0K9VNfJvpRt2HTwd5qLtXRqUIj6r3DxsrFBjejWrSPxlC+gYMXlghzrE2l/nse3AksfajAAAAABJRU5ErkJggg==";
        logo.title = "Web Essentials";
        logo.alt = logo.title;
        logo.className = "logo";

        _menu.appendChild(logo);

        _bar = document.createElement("blbar");
        _menu.appendChild(_bar);

        document.body.appendChild(_menu);
        logo.draggable = true;

        AddStyles();

        _fixed = _menu.addCheckbox("Auto-hide", "This will auto-hide this menu. Click the CTRL key to make it visible", true, function () {
            browserLink.invoke("ToggleVisibility", !this.checked);
            if (this.checked) {
                _menu.style.display = "none";
            }
        });

        return _menu;
    }

    function AddButton(text, tooltip, callback) {
        var button = document.createElement("blbutton");
        button.innerHTML = text;
        button.title = tooltip;
        button.disabled = false;

        button.onclick = function () {
            if (!this.disabled) {
                callback(arguments);
            }
        };

        _bar.insertBefore(button, _fixed);

        button.enable = function () {
            $(this).removeClass("bldisabled");
            this.disabled = false;
        };

        button.disable = function () {
            $(this).addClass("bldisabled");
            this.disabled = true;
        };

        return button;
    }

    function AddCheckbox(text, tooltip, checked, callback) {
        var item = document.createElement("blcheckbox");

        var checkbox = document.createElement("input");
        checkbox.checked = checked;
        checkbox.type = "checkbox";
        checkbox.title = tooltip;
        checkbox.onclick = callback;
        var id = ("_" + Math.random()).replace('.', '_');
        checkbox.id = id;

        var label = document.createElement("label");
        label.innerHTML = text;
        label.title = tooltip;
        label.style.fontWeight = "normal";
        label.htmlFor = id;

        item.checked = function (value) {
            if (typeof (value) === typeof (undefined)) {
                return checkbox.checked;
            }

            return checkbox.checked = value;
        };

        item.appendChild(label);
        label.appendChild(checkbox);

        if (!_fixed) {
            _bar.appendChild(item);
        }
        else {
            _bar.insertBefore(item, _fixed);
        }

        item.enable = function () {
            $(checkbox).removeClass("bldisabled");
            $(label).removeClass("bldisabled");
            checkbox.removeAttribute("disabled");
            checkbox.removeAttribute("disabled");
        };

        item.disable = function () {
            $(checkbox).addClass("bldisabled");
            $(label).addClass("bldisabled");
            checkbox.disabled = "disabled";
            label.disabled = "disabled";
            this.style.opacity = ".6";
        };

        return item;
    }

    function AddStyles() {
        var style = document.createElement("style");
        style.innerHTML =
            "bl {position: fixed; left: 10px; bottom: 5px; opacity: .3; color:black; z-index:2147483638; }" +
            "bl:hover {opacity: .95;}" +
            "bl .logo {width: 40px; margin-right: 8px; vertical-align:baseline; }" +
            "blbar {background: #d8d8d8; display: inline-block; font:13px arial; position: relative; top: -15px; border-radius: 5px; padding: 4px 3px}" +
            "blbar img { margin: -2px 0 0 2px; }" +
            "blbar label {display: inline; cursor: pointer; font: 13px arial; }" +
            "blbar blcheckbox {margin: 0 4px;}" +
            "blbar blbutton, blbar blcheckbox:not(:last-child) {border-right: 1px solid #b8b6b6; padding-right: 7px}" +
            "blbar input {margin: -3px auto auto 3px!important; vertical-align: middle;}" +
            "blbar blbutton:not([disabled]) {cursor: pointer; display: inline-block; margin: 0 4px; }" +
            "blbar blbutton:hover {text-decoration: underline;}" +
            ".bldisabled {cursor:default !important; opacity:.5}" +
            ".bldisabled:hover {text-decoration:none;}";

        document.body.appendChild(style);
    }

    $(document).keyup(function (e) {
        if (e.keyCode === 17 && _toggle) { // Ctrl
            if (_menu.style.display !== "block")
                _menu.style.display = "block";
            else
                _menu.style.display = "none";
        }

        _toggle = false;
    });

    $(document).keydown(function (e) {
        _toggle = e.keyCode === 17;// Ctrl
    });

    window.browserLink.menu = CreateMenu();

    return {
        setVisibility: function (visible) { // Can be called from the server-side extension
            _menu.style.display = visible ? "block" : "none";
            _fixed.checked(!visible);
        }
    };
});