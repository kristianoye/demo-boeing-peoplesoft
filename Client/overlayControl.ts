import $ from "jquery";

export class OverlayControl {
    constructor() {
        this.$element = $('blogOverlay');
    }

    public $element: JQuery<HTMLDivElement>;

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

