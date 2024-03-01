
function NodataChartImagis() {
    $("#ProductionSummaryGraph").html('<img style="width:473px;" src="../images/chart_empty_exspense 1.jpg" alt="images"/>');
  //  $("#ProductionSummaryGraph").html('<img style="width:473px;" src="../images/chart_empty_exspense.jpg" alt="images"/>');
    $("#CostProductionInDirectCostsGraph").html('<img style="width: 1150px;" src="../images/chart_empty_indirect_cost.jpg" alt="images"/>');
    $("#CostProductionDirectCostsGraph").html('<img style="width: 1150px;" src="../images/chart_empty_direct_cost.jpg" alt="images"/>');
}

//Cost of Production Summary graph
function CostProductionSummaryGraph(yaxis) {

    var col = {
        x: ['Total <br> Direct<br>  Costs', 'Total <br> Indirect<br>  (Fixed)Costs', 'Total <br> Production<br>  Costs'],
        y: yaxis,
        name: 'SF Zoo',
        type: 'bar',
        marker: { color: '#073d1b' }
    };
    var data = [col];
    var layout = {
        xaxis: {
            tickangle:0,
            tickfont: { color: 'blace' }
        },
        barmode: 'group', bargap: 0.15, bargroupgap: 0.1, legend: {
            "orientation": "h",
            x: 0.01,
            y: 1.2
        }
    };
    Plotly.newPlot('ProductionSummaryGraph', data, layout);


}

//Cost of Production In Direct Costs Graph
function InDirectCostGraph(Yaxis_InDirectCost) {
    //console.log(Yaxis_InDirectCost)
    var col = {
        x: ['Labor', 'Leases', 'Farm <br> Insurance', 'Dues & <br> Fees', 'Utilities', 'Interest', 'Mac/Bld <br> Depreciation', 'Real <br> Estate Taxes', 'Miscellaneous',],
        y: Yaxis_InDirectCost,
        type: 'bar',
        marker: { color: '#073d1b' }
    };
    var data = [col];

    var layout = {
        title: 'Indirect Cost',
        xaxis: {
            tickangle: 0,
            tickfont: { color: 'blace'}
        },
        legend: {
            "orientation": "h",
            x: 0.01,
            y: 1.2
        },

        barmode: 'group',
        bargap: 0.15, bargroupgap: 0.1
    };

    Plotly.newPlot('CostProductionInDirectCostsGraph', data, layout);
}

//Cost of Production Direct Costs Graph
function DirectCostGraph(Yaxis_DirectCost) {
    var col = {
        x: ['Fertilizer', 'Crop <br> Chemicals', 'Other Crop', 'Energy', 'Repair & <br> Maintenance', 'Custom <br> Hire', 'Hired <br> Labor', 'Irrigation', 'Leases', 'Other'],
        y: Yaxis_DirectCost,
        type: 'bar',
        marker: { color: '#073d1b' }
    };
    var data = [col];

    var layout = {
        title: 'Direct Cost',
        xaxis: {
            tickangle: 0,
            tickfont: { color: 'blace' }
        },
        legend: {
            "orientation": "h",
            x: 0.01,
            y: 1.2
        },

        barmode: 'group',
        bargap: 0.15, bargroupgap: 0.1
    };

    Plotly.newPlot('CostProductionDirectCostsGraph', data, layout);
}