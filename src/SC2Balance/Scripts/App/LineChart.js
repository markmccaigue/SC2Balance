; (function ($) {

    var pluginName = 'lineChart';

    var id = 1;

    function LineChart(elements, options) {
        this.$elements = $(elements);
        var thisId = pluginName + id++;
        this.$elements.addClass(thisId);
        this.selector = '.' + thisId;

        var chartSpacing = 10;

        var width = this.$elements.width();
        var height = this.$elements.height();
        var margin = { top: 20, right: 80, bottom: 30, left: 50 }; 
        var x = d3.time.scale().range([0, width - margin.left - margin.right]);
        var y = d3.scale.linear().range([height - margin.top - margin.bottom, 0]);
        var colour = d3.scale.category10();

        // TODO: Some of these options aren't really reusable
        // need to refactor a little bit to expose them in a more granular manner
        // but for now we're tied a bit to the history chart
        var defaults = {
            data: [],
            width: width,
            height: height,
            margin: margin,
            xTicks: 3,
            xTicksFormat: function (d) { return d; },
            xData: function (d) { return x(d.DateTime); },
            yData: function (d) { return y(d.WinRate); },
            domainFilter: function (key) { return true },
            domainValue: function (d) { //TODO: Unused, see above
                return { DateTime: d.DateTime, WinRate: +d[name] };
            },
            extent: function (d) { return d },
            preProcess: function (d) { return; },
            yDomain: function (v) { return v; },
            yAxisLabel: "",
            datum: function (d) { return { name: d.name, value: d.values[d.values.length - 1] }; },
            lineColour: function (d) { return colour(d.name); },
            lineLabel: function (d) { return d },
            transform: function (d) { return "translate(" + x(d.value.DateTime) + "," + y(d.value.WinRate) + ")"; },
            x: x,
            y: y,
            colour: colour
        };

        this.options = $.extend({}, defaults, options);
    }

    LineChart.prototype = {
        destroy: function () {
            return destroy(this);
        },
        init: function () {
            return init(this);
        }
    };

    $.fn[pluginName] = function (options, interval) {

        return this.each(function () {

            var $this = $(this);

            if (!options || typeof options === 'object' || typeof options === 'function') {
                if (!$this.data(pluginName)) {
                    var instance = new LineChart($this, options, interval);
                    $this.data(pluginName, instance);
                    instance.init();
                    return $this;
                }
            } else {
                if ($this.data(pluginName)[options]) {
                    return $this.data(pluginName)[options].apply($this.data(pluginName), Array.prototype.slice.call(arguments, 1)) || $this;
                }
            }
            return $this;

        });
    };

    function init(me) {

        var margin = me.options.margin;
        var width = me.options.width - margin.left - margin.right;
        var height = me.options.height - margin.top - margin.bottom;

        var xAxis = d3.svg.axis()
            .scale(me.options.x)
            .orient("bottom")
            .ticks(me.options.xTicks)
            .tickFormat(me.options.xTicksFormat);

        var yAxis = d3.svg.axis()
            .scale(me.options.y)
            .orient("left");

        var line = d3.svg.line()
            .interpolate("basis")
            .x(me.options.xData)
            .y(me.options.yData);

        var svg = d3.select(me.selector).append("svg")
            .attr("width", width + margin.left + margin.right)
            .attr("height", height + margin.top + margin.bottom)
            .append("g")
            .attr("transform", "translate(" + margin.left + "," + margin.top + ")");

        
        var data = me.options.data;

        me.options.colour.domain(d3.keys(data[0].WinRate).filter(me.options.domainFilter));

        data.forEach(me.options.preProcess);
        
        var series = me.options.colour.domain().map(function (name) {
            return {
                name: name.substring(0, name.length - 7),
                values: data.map(function (d) {
                    return { DateTime: d.DateTime, WinRate: d.WinRate[name] };
                })
            };
        });

        me.options.x.domain(d3.extent(data, me.options.extent));

        me.options.y.domain([
            d3.min(series, function (c) { return d3.min(c.values, me.options.yDomain); }),
            d3.max(series, function (c) { return d3.max(c.values, me.options.yDomain); })
        ]);

        svg.append("g")
            .attr("class", "x axis")
            .attr("transform", "translate(0," + height + ")")
            .call(xAxis);

        svg.append("g")
            .attr("class", "y axis")
            .call(yAxis)
            .append("text")
            .attr("transform", "rotate(-90)")
            .attr("y", 6)
            .attr("dy", ".71em")
            .style("text-anchor", "end")
            .text(me.options.yAxisLabel);

        var serie = svg.selectAll(".serie")
            .data(series)
            .enter().append("g")
            .attr("class", "serie");

        serie.append("path")
            .attr("class", "line")
            .attr("d", function (d) { return line(d.values); })
            .style("stroke", me.options.lineColour);

        serie.append("text")
            .datum(me.options.datum)
            .attr("transform", me.options.transform)
            .attr("x", 3)
            .attr("dy", ".35em")
            .text(me.options.lineLabel);
    }

    function destroy(me) {
        return $(me.selector).removeData(pluginName);
    }

})(jQuery);