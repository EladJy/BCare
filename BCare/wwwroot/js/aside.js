/* aside start graph */
//function dashboard2(id, fData) {

//    var barColor = '#ee5307';
//    function segColor(c) { return { low: "#807dba", mid: "#e08214", high: "#41ab5d" }[c]; }

//    // compute total for each state.
//    fData.forEach(function (d) { d.total = d.price; });

//    // function to handle histogram.
//    function histoGram(fD) {

//        var hG = {}, hGDim = { t: 20, r: 0, b: 130, l: 0 };
//        hGDim.w = 280 - hGDim.l - hGDim.r,
//        hGDim.h = 200 - hGDim.t - hGDim.b;

//        //create svg for histogram.
//        var hGsvg = d3.select(id).append("svg")
//            .attr("width", hGDim.w + hGDim.l + hGDim.r)
//            .attr("height", hGDim.h + hGDim.t + hGDim.b).append("g")
//            .attr("transform", "translate(" + hGDim.l + "," + hGDim.t + ")");

//        // create function for x-axis mapping.
//        var x = d3.scale.ordinal().rangeRoundBands([0, hGDim.w], 0.1)
//                .domain(fD.map(function (d) { return d[0]; }));

//        // Add x-axis to the histogram svg.
//        hGsvg.append("g").attr("class", "x axis")
//        .attr("transform", "translate(0," + hGDim.h + ")")
//        .call(d3.svg.axis().scale(x).orient("bottom"))
//        .selectAll("text")
//        .style("text-anchor", "start")
//        .style("font-size", "12px")
//        .attr("dy", "-0.3em")
//        .attr("dx", "0.8em")
//        .attr("transform", function (d) { return "rotate(90)" });

//        // Create function for y-axis map.
//        var y = d3.scale.linear().range([hGDim.h, 0])
//                .domain([0, d3.max(fD, function (d) { return d[1]; })]);

//        // Create bars for histogram to contain rectangles and freq labels.
//        var bars = hGsvg.selectAll(".bar").data(fD).enter()
//                .append("g").attr("class", "bar");

//        //create the rectangles.
//        bars.append("rect")
//            .attr("x", function (d) { return x(d[0]); })
//            .attr("y", function (d) { return y(d[1]); })
//            .attr("width", x.rangeBand())
//            .attr("height", function (d) { return hGDim.h - y(d[1]); })
//            .attr('fill', barColor)
//            .on("mouseover", mouseover)// mouseover is defined below.
//            .on("mouseout", mouseout);// mouseout is defined below.

//        //Create the frequency labels above the rectangles.
//        bars.append("text").text(function (d) { return d3.format(",")(d[1]) + "$" })
//            .attr("x", function (d) { return x(d[0]) + x.rangeBand() / 2; })
//            .attr("y", function (d) { return y(d[1]) - 5; })
//            .attr("text-anchor", "middle")
//            .style("font-size", "11px");


//        function mouseover(d) {  // utility function to be called on mouseover.
//            // filter for selected state.
//            var st = fData.filter(function (s) { return s.name == d[0]; })[0],
//                nD = d3.keys(st.price).map(function (s) { return { type: s, price: st.price[s] }; });

//            // call update functions of pie-chart and legend.
//            //  pC.update(nD);
//            //    leg.update(nD);
//        }

//        function mouseout(d) {    // utility function to be called on mouseout.
//            // reset the pie-chart and legend.
//            // pC.update(tF);
//            //leg.update(tF);
//        }

//        // create function to update the bars. This will be used by pie-chart.
//        hG.update = function (nD, color) {
//            // update the domain of the y-axis map to reflect change in frequencies.
//            y.domain([0, d3.max(nD, function (d) { return d[1]; })]);

//            // Attach the new data to the bars.
//            var bars = hGsvg.selectAll(".bar").data(nD);

//            // transition the height and color of rectangles.
//            bars.select("rect").transition().duration(500)
//                .attr("y", function (d) { return y(d[1]); })
//                .attr("height", function (d) { return hGDim.h - y(d[1]); })
//                .attr("fill", color);

//            // transition the frequency labels location and change value.
//            bars.select("text").transition().duration(500)
//                .text(function (d) { return d3.format(",")(d[1]) })
//                .attr("y", function (d) { return y(d[1]) - 5; });
//        }
//        return hG;
//    }

//    // calculate total frequency by segment for all state.
//    var tF = ['low', 'mid', 'high'].map(function (d) {
//        return { type: d, price: d3.sum(fData.map(function (t) { return t.price[d]; })) };
//    });

//    // calculate total frequency by state for all segment.
//    var sF = fData.map(function (d) { return [d.name, d.total]; });

//    var hG = histoGram(sF); // create the histogram.
//    // leg = legend(tF);  // create the legend.

//}
//$(document).ready(function () {
//    $.getJSON('/Graph/getPriceGraph', function (data) {
//        dashboard2('#prices', data);
//    });
//});
///* aside end graph */

$(document).ready(function () {
    requestTop5();
});
function requestTop5() {
    $.ajax({
        url: "/Home/top5",
        datatype: "json",
        type: "POST",
        success: function (data) {
            for (var i = 0; i < 5; i++) {
                $('.top5.n-' + i).html(data[i].somName); // name for each top 5
            }
        },
        error: function () {
            $('.top5.n-' + i).html("ERROR");
        }
    });
}