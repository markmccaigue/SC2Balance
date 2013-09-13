var BalanceRunner = (function () {
    
    var balanceRunnerConstructor = function () {
        this.dataBuilder = new DataBuilder();
        this.chartBuilder = new ChartBuilder();
    };
    
    balanceRunnerConstructor.prototype = {
        loadBalance: function () {
            return loadBalance(this);
        }
    };

    function loadBalance(me) {

        LoadingManager().startLoad();
        $.ajax({
            url: "/api/data/GetRaceBalanceForSevenDays"
        }).done(function (raceWinRate) {
            me.chartBuilder.buildPieChart('#balanceDiv .mini-balance.tvp', me.dataBuilder.getDataFromRaceWinRate(raceWinRate, 'TVP'));
            me.chartBuilder.buildPieChart('#balanceDiv .mini-balance.tvz', me.dataBuilder.getDataFromRaceWinRate(raceWinRate, 'TVZ'));
            me.chartBuilder.buildPieChart('#balanceDiv .mini-balance.zvp', me.dataBuilder.getDataFromRaceWinRate(raceWinRate, 'ZVP'));
            LoadingManager().endLoad();
        });

        LoadingManager().startLoad();
        $.ajax({
            url: "/api/data/GetBalanceForSevenDays"
        }).done(function (winRate) {
            me.chartBuilder.buildPieChart('#balanceDiv .balance-main', me.dataBuilder.getDataFromWinRate(winRate));
            LoadingManager().endLoad();
        });
    }

    return balanceRunnerConstructor;
})();