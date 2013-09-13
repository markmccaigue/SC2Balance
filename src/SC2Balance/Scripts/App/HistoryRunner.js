var HistoryRunner = (function () {

    var historyRunnerConstructor = function () {
        this.dataBuilder = new DataBuilder();
        this.chartBuilder = new ChartBuilder();
    };

    historyRunnerConstructor.prototype = {
        loadHistory: function () {
            return loadHistory(this);
        }
    };

    function loadHistory(me) {
        LoadingManager().startLoad();
        $.ajax({
            url: "/api/data/GetBalanceHistory"
        }).done(function (balanceHistory) {
            var data = me.dataBuilder.getDataFromBalanceHistory(balanceHistory);
            me.chartBuilder.buildLineChart('#historyDiv', data);
            LoadingManager().endLoad();
        });
    }

    return historyRunnerConstructor;
})();