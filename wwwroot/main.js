/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./src/createAccount.ts":
/*!******************************!*\
  !*** ./src/createAccount.ts ***!
  \******************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {


var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
const jquery_1 = __importDefault(__webpack_require__(/*! jquery */ "jquery"));
const highlightColors = 'border-red-500 background-red-100';
class AccountCreationUtil {
    constructor($form) {
        this.$form = $form;
        this.$form.on('submit', e => this.onSubmit(e));
        this.$form.find('#Username').on('blur', (e) => __awaiter(this, void 0, void 0, function* () { return yield this.onCheckUsername(e); }));
        this.$form.find('#Email').on('blur', (e) => __awaiter(this, void 0, void 0, function* () { return yield this.onCheckEmail(e); }));
    }
    addValidationError(msg, ctrlName) {
        const $list = (0, jquery_1.default)('.form-summary ul'), $ctrl = (0, jquery_1.default)(`#${ctrlName}`);
        let $existing = $list.find(`li[data-for="${ctrlName}"]`);
        if ($existing.length) {
            $existing.find('label').text(msg);
        }
        else {
            const $error = (0, jquery_1.default)(`<label for="${ctrlName}"/>`).text(msg);
            $existing = (0, jquery_1.default)(`<li data-for="${ctrlName}"/>`);
            $existing.append($error);
            $list.append($existing.show());
        }
        $ctrl.addClass(highlightColors);
    }
    clearError(ctrlName) {
        const $error = (0, jquery_1.default)(`.form-summary li[data-for="${ctrlName}"]`);
        const $ctrl = (0, jquery_1.default)(`#${ctrlName}`);
        $error.length && $error.remove();
        $ctrl.length && $ctrl.removeClass(highlightColors);
    }
    /**
     * Check to see if the email address is already registered
     * @param e
     */
    onCheckEmail(e) {
        return __awaiter(this, void 0, void 0, function* () {
            const $t = (0, jquery_1.default)(e.target);
            const emailAddress = ($t.val() || '').trim();
            if (emailAddress.length) {
                const checkEmailAddress = new Promise((resolve, reject) => {
                    try {
                        fetch(`${siteConfig.backendUrl}/api/account/emailExists/${emailAddress}`)
                            .then(resp => {
                            if (!resp.ok) {
                                this.addValidationError('oops, something went wrong', 'Email');
                            }
                            else {
                                return resp.json();
                            }
                        })
                            .then((data) => {
                            if (typeof data === 'object') {
                                if (data.error) {
                                    this.addValidationError(data.error, 'Email');
                                }
                                else if (data.emailExists) {
                                    this.addValidationError('Email address is already registered', 'Email');
                                }
                                else {
                                    this.clearError('Email');
                                }
                            }
                            else
                                this.addValidationError('Backend returned unexpected results; Try again later', 'Email');
                        });
                    }
                    catch (e) {
                        reject(e);
                    }
                });
                yield checkEmailAddress;
            }
            else {
                this.addValidationError('Email address is required', 'Email');
            }
        });
    }
    /**
     * Check to see if the username is already in use
     * @param e
     */
    onCheckUsername(e) {
        return __awaiter(this, void 0, void 0, function* () {
            const $t = (0, jquery_1.default)(e.target);
            const username = ($t.val() || '').trim();
            if (username.length) {
                const checkUsername = new Promise((resolve, reject) => {
                    try {
                        fetch(`${siteConfig.backendUrl}/api/account/userExists/${username}`)
                            .then(resp => {
                            if (!resp.ok) {
                                this.addValidationError('oops, something went wrong', 'Username');
                            }
                            else {
                                return resp.json();
                            }
                        })
                            .then((data) => {
                            if (data.error) {
                                this.addValidationError(data.error, 'Username');
                            }
                            else if (data.userExists) {
                                this.addValidationError('Username already exists', 'Username');
                            }
                            else {
                                this.clearError('Username');
                            }
                        });
                    }
                    catch (e) {
                        reject(e);
                    }
                });
                yield checkUsername;
            }
            else {
                this.addValidationError('Username is required', 'Username');
            }
        });
    }
    /**
     * Check the form
     * @param e
     * @returns
     */
    onSubmit(e) {
        let validState = true;
        let values = {};
        ',Username,Password,Password2,Email,Name'.split(',').filter(s => s.length).forEach(fn => {
            const $inp = (0, jquery_1.default)(`#${fn}`);
            const val = $inp.val().trim();
            values[fn] = val;
            if (val.length === 0) {
                $inp.addClass(highlightColors);
                validState = false;
            }
            else {
                $inp.removeClass(highlightColors);
            }
        });
        if (values['Password'] !== values['Password2']) {
            const $ctrl = (0, jquery_1.default)('#Password2');
            this.addValidationError('Passwords do not match', 'Password2');
            $ctrl.val('');
            $ctrl.focus();
        }
        if (!validState) {
            e.preventDefault();
            e.stopPropagation();
        }
        return validState;
    }
}
exports["default"] = AccountCreationUtil;
(0, jquery_1.default)(document).ready(() => {
    const $form = (0, jquery_1.default)('#createUserForm, #uploadProfilePicture');
    if ($form.length === 1) {
        const util = new AccountCreationUtil($form);
    }
});


/***/ }),

/***/ "./src/overlayControl.ts":
/*!*******************************!*\
  !*** ./src/overlayControl.ts ***!
  \*******************************/
/***/ (function(__unused_webpack_module, exports, __webpack_require__) {


var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", ({ value: true }));
exports.OverlayControl = void 0;
const jquery_1 = __importDefault(__webpack_require__(/*! jquery */ "jquery"));
class OverlayControl {
    constructor() {
        this.$element = (0, jquery_1.default)('blogOverlay');
    }
    hide() {
        if (this.$element.hasClass('active')) {
            this.$element.removeClass('active');
        }
    }
    show() {
        if (!this.$element.hasClass('active')) {
            this.$element.addClass('active');
        }
    }
}
exports.OverlayControl = OverlayControl;


/***/ }),

/***/ "jquery":
/*!*************************!*\
  !*** external "jQuery" ***!
  \*************************/
/***/ ((module) => {

module.exports = jQuery;

/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/************************************************************************/
var __webpack_exports__ = {};
// This entry need to be wrapped in an IIFE because it need to be isolated against other modules in the chunk.
(() => {
var exports = __webpack_exports__;
/*!*********************!*\
  !*** ./src/main.ts ***!
  \*********************/
var __webpack_unused_export__;

__webpack_unused_export__ = ({ value: true });
__webpack_require__(/*! ./createAccount */ "./src/createAccount.ts");
__webpack_require__(/*! ./overlayControl */ "./src/overlayControl.ts");

})();

/******/ })()
;
//# sourceMappingURL=main.js.map