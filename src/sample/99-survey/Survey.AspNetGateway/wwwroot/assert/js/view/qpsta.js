/* File Created: 九月 11, 2013 */
define(function (require, exports, module) {
    exports.init = function () {
        $("a.mclose").click(function () {
            CloseModalDialog(null, false, null);
        });
    };
});