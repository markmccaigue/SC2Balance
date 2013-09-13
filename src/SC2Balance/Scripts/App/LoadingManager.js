var LoadingManager = function () {

    if (LoadingManager.prototype._singletonInstance) {
        return LoadingManager.prototype._singletonInstance;
    }
    LoadingManager.prototype._singletonInstance = this;

    this.loadingCount = 1;

    this.startLoad = function () {
        if (me.loadingCount == 0) {
            $('#loadingDiv').show();
        }
        me.loadingCount++;
    };

    this.endLoad = function() {
        me.loadingCount--;
        if (me.loadingCount == 0) {
            $('#loadingDiv').hide();
        }
    };

    var me = this;

    return LoadingManager.prototype._singletonInstance;
};