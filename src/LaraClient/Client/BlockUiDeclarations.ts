interface JQueryStatic {
    blockUI(property?: blockUISettings): void;
    unblockUI(property?: blockUISettings): void;
}

interface blockUISettings {
    message?: any;
    title?: string;
    draggable?: boolean;
    theme?: boolean;
    css?: blockUI_CSS_Settings;
    themedCSS?: blockUI_ThemedCSS_Settings;
    overlayCSS?: blockUI_OverlayCSS_Settings;
    cursorReset?: string;
    growlCSS?: blockUI_GrowlCSS_Settings;
    iframeSrc?: any;
    forceIframe?: boolean;
    baseZ?: number;
    centerX?: boolean;
    centerY?: boolean;
    allowBodyStretch?: boolean;
    bindEvents?: boolean;
    constrainTabKey?: boolean;
    fadeIn?: number;
    fadeOut?: number;
    timeout?: number;
    showOverlay?: boolean;
    onBlock?: Function;
    onUnblock?: Function;
    quirksmodeOffsetHack?: number;
    blockMsgClass?: string;
    ignoreIfBlocked?: boolean;
}

interface blockUI_CSS_Settings {
    padding?: number;
    margin?: number;
    width?: string;
    top?: string;
    left?: string;
    textAlign?: string;
    color?: string;
    border?: string;
    backgroundColor?: string;
    cursor?: string;
}

interface blockUI_ThemedCSS_Settings {
    width?: string;
    top?: string;
    left?: string;
}

interface blockUI_OverlayCSS_Settings {
    backgroundColor?: string;
    opacity?: number;
    cursor?: string;
}

interface blockUI_GrowlCSS_Settings {
    width?: string;
    top?: string;
    left?: string;
    right?: string;
    border?: string;
    padding?: string;
    opacity?: number;
    cursor?: string;
    color?: string;
    backgroundColor?: string;
}
