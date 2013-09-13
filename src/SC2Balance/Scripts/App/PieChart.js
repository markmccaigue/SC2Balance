; (function ($) {

    var pluginName = 'pieChart';

    var id = 1;

    function PieChart(elements, options) {
        this.$elements = $(elements);
        var thisId = pluginName + id++;
        this.$elements.addClass(thisId);
        this.selector = '.'+thisId;
        var defaultSide = Math.min(this.$elements.width(), this.$elements.height());
        var chartSpacing = 10;
        var defaults = {            
            data: [],
            width: defaultSide,
            height: defaultSide,
            radius: (defaultSide / 2) - chartSpacing,
            sort: null,
            value: function (d) { return d; },
            fill: function (d) { return 'white'; },
            text: function (d) {return d.data; },
            textFill: function (d) { return 'black'; },
            chartSpacing: chartSpacing,
            secondText: function(d) { return d.data; }
        };
        this.options = $.extend({}, defaults, options);
    }

    PieChart.prototype = {
        destroy: function () {
            return destroy(this);
        },
        init: function () {
            return init(this);
        }
    };

    $.fn[pluginName] = function (options, interval) {

        return this.each(function() {

            var $this = $(this);

            if (!options || typeof options === 'object' || typeof options === 'function') {
                if (!$this.data(pluginName)) {
                    var instance = new PieChart($this, options, interval);
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

        var arc = d3.svg.arc()
            .outerRadius(me.options.radius)
            .innerRadius(0);

        var pie = d3.layout.pie()
            .sort(me.options.sort)
            .value(me.options.value);

        var svg = d3.select(me.selector).append("svg")
            .attr("width", me.options.width)
            .attr("height", me.options.height)
            .append("g")
            .attr("transform", "translate(" + me.options.width / 2 + "," + me.options.height / 2 + ")");

        var descriptors = pie(me.options.data);

        var g = svg.selectAll(".arc")
            .data(descriptors)
            .enter().append("g")
            .attr("class", "arc");

        g.append("path")
            .attr("d", arc)
            .style("fill", me.options.fill);

        g.append("text")
            .attr("transform", function(d) { return "translate(" + arc.centroid(d) + ")"; })
            .attr("dy", ".35em")
            .style("text-anchor", "middle")
            .text(me.options.text)
            .attr("fill", me.options.textFill);

        g.append("text")
            .attr("transform", function (d) { return "translate(" + arc.centroid(d) + ")"; })
            .attr("dy", "1.9em")
            .style("text-anchor", "middle")
            .text(me.options.secondText)
            .attr("fill", me.options.textFill);

        g.append("div").attr('class', 'pop');
    }


    function destroy(me) {
        return $(me.selector).removeData(pluginName);
    }

})(jQuery);