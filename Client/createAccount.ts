import $ from 'jquery';

const highlightColors = 'border-red-500 background-red-100';

interface IUserCheckResponse {
    userExists: boolean;
    username: string;
    error?: string;
}

interface IEmailCheckResponse {
    emailExists: boolean;
    email: string;
    error?: string;
}

export default class AccountCreationUtil {
   
    constructor($form: JQuery<HTMLFormElement>) {
        this.$form = $form;

        this.$form.on('submit', e => this.onSubmit(e));
        this.$form.find('#Username').on('blur', async e => await this.onCheckUsername(e));
        this.$form.find('#Email').on('blur', async e => await this.onCheckEmail(e));
    }

    public $form: JQuery<HTMLFormElement>;

    public addValidationError(msg: string, ctrlName: string): void {
        const $list = $('.form-summary ul'),
            $ctrl = $(`#${ctrlName}`);
        let $existing = $list.find(`li[data-for="${ctrlName}"]`);

        if ($existing.length) {
            $existing.find('label').text(msg);
        } else {
            const $error = $(`<label for="${ctrlName}"/>`).text(msg);
            $existing = $(`<li data-for="${ctrlName}"/>`);
            $existing.append($error);
            $list.append($existing.show());
        }
        $ctrl.addClass(highlightColors);
    }

    public clearError(ctrlName: string): void {
        const $error = $(`.form-summary li[data-for="${ctrlName}"]`);
        const $ctrl = $(`#${ctrlName}`);

        $error.length &&  $error.remove();
        $ctrl.length && $ctrl.removeClass(highlightColors);
    }

    /**
     * Check to see if the email address is already registered
     * @param e
     */
    public async onCheckEmail(e: JQuery.BlurEvent<HTMLElement>) {
        const $t = $(e.target);
        const emailAddress = ($t.val() as string || '').trim();

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
                        .then((data: IEmailCheckResponse) => {
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
                        })
                }
                catch (e) {
                    reject(e);
                }
            });

            await checkEmailAddress;
        }
        else {
            this.addValidationError('Email address is required', 'Email');
        }
    }

    /**
     * Check to see if the username is already in use
     * @param e
     */
    public async onCheckUsername(e: JQuery.BlurEvent<HTMLElement>) {
        const $t = $(e.target);
        const username = ($t.val() as string || '').trim();

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
                        .then((data: IUserCheckResponse) => {
                            if (data.error) {
                                this.addValidationError(data.error, 'Username');
                            }
                            else if (data.userExists) {
                                this.addValidationError('Username already exists', 'Username');
                            }
                            else {
                                this.clearError('Username');
                            }
                        })
                }
                catch (e) {
                    reject(e);
                }
            });

            await checkUsername;
        }
        else {
            this.addValidationError('Username is required', 'Username');
        }
    }

    /**
     * Check the form
     * @param e
     * @returns
     */
    public onSubmit(e: JQuery.SubmitEvent<HTMLFormElement>) {
        let validState = true;
        let values: { [key: string]: string } = {};

        ',Username,Password,Password2,Email,Name'.split(',').filter(s => s.length).forEach(fn => {
            const $inp: JQuery<HTMLInputElement> = $(`#${fn}`);
            const val: string = ($inp.val() as string).trim();

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
            const $ctrl = $('#Password2');
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

$(document).ready(() => {
    const $form = $('#createUserForm, #uploadProfilePicture');

    if ($form.length === 1) {
        const util = new AccountCreationUtil($form as JQuery<HTMLFormElement>);
    }
});
