var ChartBuilder = (function () {

    var chartBuilderConstructor = function () {
    };

    chartBuilderConstructor.prototype = {
        buildPieChart: function (selector, data) {
            return buildPieChart(this, selector,data);
        },
        buildLineChart: function (selector, data) {
            return buildLineChart(this, selector, data);
        },
    };

    function buildPieChart(me, selector, data) {
        $(selector).pieChart(getPieChartOptionsForData(data));
    }

    function buildLineChart(me, selector, data) {
        $(selector).lineChart(getLineChartOptionsForData(data));
    }

    function getPieChartOptionsForData(data) {
        return {
            data: data,
            value: function (d) { return d.winRate; },
            text: function (d) { return d.data.race; },
            secondText: function(d) { return Math.round(d.data.winRate * 10) / 10 + "%"; },
            fill: function (d) {
                switch (d.data.race) {
                    case 'Terran':
                        return 'rgb(66, 172, 226)';
                    case 'Protoss':
                        return '#1CB25C';
                    case 'Zerg':
                        return 'rgb(228, 92, 81)';
                    default:
                        return 'black';
                }
            },
            textFill: function (d) { return '#f4f4f4'; }
        };
    }

    function getLineChartOptionsForData(data) {
        return defaults = {
            data: data,
            xTicksFormat: function (d) { return d.toLocaleDateString(); },
            domainFilter: function (key) { return key !== "DateTime"; },
            domainValue: function (d) {
                return { date: d.DateTime, WinRate: +d[name] };
            },
            extent: function (d) { return d.DateTime; },
            preProcess: function (d) {
                var parseDate = d3.time.format.iso.parse;
                d.DateTime = parseDate(d.DateTime);
            },
            yDomain: function (v) { return v.WinRate; },
            yAxisLabel: "Race",
            datum: function (d) { return { name: d.name, value: d.values[d.values.length - 1] }; },
            lineLabel: function (d) { return d.name; },
            lineColour: function (d) {
                var map = {
                    Terran: '#42ace2',
                    Protoss: '#1cb25c',
                    Zerg: '#e45c51'
                };
                return map[d.name];
            }
        };
    }

    return chartBuilderConstructor;
})();