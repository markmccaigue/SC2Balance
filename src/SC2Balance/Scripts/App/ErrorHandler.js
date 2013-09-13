function logError(errorMsg, url, lineNumber) {
    /*$.ajax({
        url: 'http://www.sc2balance.com/Home/LogJavaScriptError',
        data: {
            message: errorMsg,
            errorUrl: url,
            lineNumber: lineNumber
        }
    });TODO*/
};

window.onerror = logError;

$(document).ajaxError(function (event, xhr, ajaxOptions, thrownError) {
    logError(thrownError, ajaxOptions.url, 0);
});

$.handleError = function (s, xhr, status, e) {
    $(document).ajaxError(e, xhr, s, status);
}