// Set the dimensions of the canvas / graph
var margin = { top: 30, right: 20, bottom: 30, left: 50 },
    width = 600 - margin.left - margin.right,
    height = 270 - margin.top - margin.bottom;

// Parse the date / time
var formatDate = d3.time.format("%Y");

// Set the ranges
var x = d3.time.scale().range([0, width]);
var y = d3.scale.linear().range([height, 0]);

// Define the axes
var xAxis = d3.svg.axis().scale(x)
    .orient("bottom").ticks(2);

var yAxis = d3.svg.axis().scale(y)
    .orient("left").ticks(2);

// Define the line
var valueline = d3.svg.line()
    .x(function (d) { return x(d.date); })
    .y(function (d) { return y(d.close); });


// Get the data
function dashboard(id, data) {
    data.forEach(function (d) {
        d.date = formatDate.parse(d.item1);
        d.close = +d.item2;
    });

    // Adds the svg canvas
    var svg = d3.select(id).append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform",
        "translate(" + margin.left + "," + margin.top + ")");

    // Scale the range of the data
    x.domain(d3.extent(data, function (d) { return d.date; }));
    y.domain([0, d3.max(data, function (d) { return d.close; })]);

    // Add the valueline path.
    svg.append("path")
        .attr("class", "line")
        .attr("d", valueline(data));

    // Add the X Axis
    svg.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(0," + height + ")")
        .call(xAxis);

    // Add the Y Axis
    svg.append("g")
        .attr("class", "y axis")
        .call(yAxis)
        .selectAll("text")
        .style("text-anchor", "end")
        .attr("dx", "-.8em")
        .attr("dy", ".15em");
};

// ** Update data section (Called from the onclick)
function updateData(id, data) {
        data.forEach(function (d) {
            d.date = formatDate.parse(d.item1);
            d.close = +d.item2+5;
        });

        // Scale the range of the data again 
        x.domain(d3.extent(data, function (d) { return d.date; }));
        y.domain([0, d3.max(data, function (d) { return d.close; })]);

        // Select the section we want to apply our changes to
        var svg = d3.select(id).transition();

        // Make the changes
        svg.select(".line")   // change the line
            .duration(750)
            .attr("d", valueline(data));
            svg.select(".x.axis") // change the x axis
            .duration(750)
            .call(xAxis);
        svg.select(".y.axis") // change the y axis
            .duration(750)
            .call(yAxis)
            .selectAll("text")
            .style("text-anchor", "end")
            .attr("dx", "-.8em")
            .attr("dy", ".15em");
}


$(document).ready(function () {
    $.getJSON('/Account/BtCount', function (data) {
        dashboard('#btCount', data);
    });
    $('#updateButton').click(function (e) {
        e.preventDefault();
        $.getJSON('/Account/BtCount', function (data) {
            updateData('#btCount', data);
        });
    });
});