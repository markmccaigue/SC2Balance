var MapsRunner = (function () {

    var mapsRunnerConstructor = function () {
        this.mapToId = [];
        this.id = 1;
        this.dataBuilder = new DataBuilder();
        this.chartBuilder = new ChartBuilder();
    };

    mapsRunnerConstructor.prototype = {
        loadMaps: function () {
            return loadMaps(this);
        }
    };

    function renderRowsForMaps(mapBalances, me) {
        $.each(mapBalances, function (i, mapBalance) {
            renderRowForMap(mapBalance, me);
        });
    }

    function renderRowForMap(mapBalance, me) {
        if (me.mapToId[mapBalance.Map]) {
            return;
        }
        
        var id = me.id++;
        me.mapToId[mapBalance.Map] = id;
        var container = $('#mapsDiv');
        container.append(
            '<div id="map' + id + '"class="row">' +
                '<div>' +
                '<div id=mapName' + id + '></div>' +
                '</div>' +
                '<div class="row mapsRow">' +
                '<div class="col-lg-3 mini-balance overall"><div class="maps-row-padder"></div></div>' +
                '<div class="col-lg-3 mini-balance tvp"><div class="maps-row-padder"></div></div>' +
                '<div class="col-lg-3 mini-balance tvz"><div class="maps-row-padder"></div></div>' +
                '<div class="col-lg-3 mini-balance zvp"><div class="maps-row-padder"></div></div>' +
                '</div>' +
            '</div>'
        );
        $('#mapName' + id).text(mapBalance.Map);
    }

    function buildChart(me, selector, data) {
        me.chartBuilder.buildPieChart(selector, data);
        // This element ensures that the container is the correct size before we draw the chart,
        // but once the chart is added to the DOM we no longer need the padding, so we can remove it.
        // This allows us to use CSS to set the size of the container as a percentage.
        $(selector + ' .maps-row-padder').remove();
    }

    function loadMaps(me) {

        LoadingManager().startLoad();
        $.ajax({
            url: "/api/data/GetMapBalanceForSevenDays"
        }).done(function (mapBalances) {
            renderRowsForMaps(mapBalances, me);
            $.each(mapBalances, function(i, mapBalance) {
                var data = me.dataBuilder.getDataFromWinRate(mapBalance.WinRate);
                var selector = '#map' + me.mapToId[mapBalance.Map] + ' .overall';
                buildChart(me, selector, data);
            });
            LoadingManager().endLoad();
        });
        
        LoadingManager().startLoad();
        $.ajax({
            url: "/api/data/GetMapRaceBalanceForSevenDays"
        }).done(function (mapBalances) {
            renderRowsForMaps(mapBalances, me);
            $.each(mapBalances, function (i, mapBalance) { // TODO: Pull out into helper method
                var tvpData = me.dataBuilder.getDataFromRaceWinRate(mapBalance.RaceWinRate, 'TVP');
                var tvpSelector = '#map' + me.mapToId[mapBalance.Map] + ' .tvp';
                buildChart(me, tvpSelector, tvpData);
                var zvpData = me.dataBuilder.getDataFromRaceWinRate(mapBalance.RaceWinRate, 'ZVP');
                var zvpSelector = '#map' + me.mapToId[mapBalance.Map] + ' .zvp';
                buildChart(me, zvpSelector, zvpData);
                var tvzData = me.dataBuilder.getDataFromRaceWinRate(mapBalance.RaceWinRate, 'TVZ');
                var tvzSelector = '#map' + me.mapToId[mapBalance.Map] + ' .tvz';
                buildChart(me, tvzSelector, tvzData);
            });
            LoadingManager().endLoad();
        });
    }

    return mapsRunnerConstructor;
})();