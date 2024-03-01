$(document).ready(function () {

    // $("#FieldDetailsViewHistoryChartContent").hide();
    // $("#FieldDetailsViewHistoryEnvironmentalOutcomesChartContent").hide();
    $("#CurrentAdvisorOperationDetailslink").hide();
    $("#FertilizerandPhosphorusModal").hide();
    $("#FieldRiskModal").hide();
    //  $("#CurrentfinancialDetailsEditcropnamelink").hide();
    $("#soilindiciesPopup").hide();
    $("#financialDetailsEditcropnamepopup").hide();
    $("#soilindiciesAdvisorPopup").hide();
    $('#MZFinanciPlanname').hide();
    $('#MZFinanciPlanssubmit').hide();
    $('#PreviewFSRTableModel').hide();
    $('#PreviewRusle2TableModel').hide();
    $("#Fieldlocatinn_fieldError").hide();
    $("#AdminOperationBulkFSrTable").hide();
    $("#BulkFsrRusle2Value").hide();
    //$("#btn_PublishFSRtoWeb").hide();//publih fsr
    //ExportFieldData , ManageFieldLocationData, CalculateFSRData page in used this dropdoen ID
    if (window.location.pathname == '/Admin/ExportFieldData' ||
        window.location.pathname == '/Admin/ManageFieldLocationData' ||
        window.location.pathname == '/Admin/CalculateFSRData' ||
        window.location.pathname == '/WebApp/AdminOperations') {
        var selectElement = document.getElementById('ProducerOnChnage');
        selectElement.selectedIndex = 0;

        var producerSelect = $('#ProducerFields');
        producerSelect.empty();
        producerSelect.append($('<option>', {
            value: '',
            text: 'Select Field',
            selected: 'selected' // Set the selected attribute
        }));
    }
    if (window.location.pathname == '/Common/MyFieldsDetails')
        getGeometryImageFieldInformation();
    //datatable responsive added class
    if (window.location.pathname == '/Producer/Landing') {
        $('#ProducerLandingFarm').closest('.col-sm-12').addClass('overflow-auto');
    }
    if (window.location.pathname == '/Producer/MyFields') {
        $('#ProducerMyfields').closest('.col-sm-12').addClass('overflow-auto');
    }

    if (window.location.pathname == '/Producer/MyFarms') {
        $('#ProducerLandingFarm').closest('.col-sm-12').addClass('overflow-auto');
    }
    if (window.location.pathname == '/Admin/MyFields') {
        $('#AdminMyFields').closest('.col-sm-12').addClass('overflow-auto');
    }
    if (window.location.pathname == '/Common/Advisors') {
        $('#CommonAdvisor').closest('.col-sm-12').addClass('overflow-auto');
    }
    if (window.location.pathname == '/Common/Producers') {
        $('#CommonProducer').closest('.col-sm-12').addClass('overflow-auto');
    }
    if (window.location.pathname == '/Common/FinancialDetailsEdit') {
        $("#loading").fadeOut();
    }
    if (window.location.pathname == '/Advisor/Landing') {
        $('#AdvisorLandingFields').closest('.col-sm-12').addClass('overflow-scroll');
        $('#AdvisorLandingProducer').closest('.col-sm-12').addClass('overflow-scroll');
    }



});
if (window.location.pathname == '/Common/FinancialDetailsEdit') {
    $("#loading").fadeIn();
}

if (window.location.pathname == '/Admin/ExportFieldData') {
    $(document).ready(function () {
        $("#ExportDataTable").hide();
        func_GetFieldsFromProducer();
    });
}

$("#financialCalcOptionDataForm").click(function () {
    $("#loading").fadeIn();
    $("#financialCalcOptionDataForm").submit();
});
$("#myfieldsdetailsgo").click(function () {
    $("#loading").fadeIn();
    $("#myForm").submit();
});
//--START--financial hide show
// '/Common / MyFieldsDetails'  page in financial table partation current/advisor plan vise separete display
if (location.pathname == '/Common/MyFieldsDetails') {
    $("#pills-MyFields-tab").click(function () {
        console.log("advisor pla button clicked");
        $(".financial_Advisor_Current_tab_hideshow").show();
    })
    $("#pills-MyFarms-tab").click(function () {
        console.log("current tab button clicked");
        $(".financial_Advisor_Current_tab_hideshow").hide();
    })
}
//--END--financial hide show

// MyFields details page FSR View history link
$("#CurrentOperationDetailslink").click(function () {
    $(".fsrdata").show();
    $(".fsrdataAdvisor").show();
    $(".environmentalOutcomes").show();
    $(".finacialResult").show();
    $("#soilindiciesPopup").hide();
    var elements = document.getElementsByClassName("CurrentOperationDetails");
    for (var i = 0; i < elements.length; i++) {
        elements[i].style.display = "none";
    }
    var FieldDetailsViewHistoryChartContent = document.getElementById("FieldDetailsViewHistoryChartContent");
    FieldDetailsViewHistoryChartContent.style.display = 'none';
    var FieldDetailsViewHistoryEnvironmentalOutcomesChartContent = document.getElementById("FieldDetailsViewHistoryEnvironmentalOutcomesChartContent");
    FieldDetailsViewHistoryEnvironmentalOutcomesChartContent.style.display = 'none';
    $("#FertilizerandPhosphorusModal").hide();
    $("#FieldRiskModal").hide();
    //show outcome and financial data
    $("#environmentalOutcomesSection").show();
    $('.finacialResult').show();
});

$('#ViewHistorylink').click(function () {
    $("#loading").fadeIn();
    var field_Id = $('#id').val();
    var DropdownTo = $('#HistoryDropdownTo').val();
    var DropdownFrom = $('#HistoryDropdownfrom').val();
    $.ajax({
        url: '/Common/ViewFSRViewHistoryPopup/',
        type: 'GET',
        cache: false,
        dataType: 'HTML',
        data: { id: field_Id, firstValue: DropdownFrom, secondValue: DropdownTo },
        success: function (data) {
            var chartlist = JSON.parse(data);
            if (chartlist.success) {
                var viewchartlist = JSON.parse(chartlist.data);
                myFieldsFSRChart(viewchartlist);
            }
            $("#loading").fadeOut();
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $("#loading").fadeOut();
        }
    });
});

// myFieldsFSR Details Charts
function myFieldsFSRChart(fsrChartDataList) {
    $(".fsrdata").hide();
    var elements = document.getElementsByClassName("CurrentOperationDetails");
    for (var i = 0; i < elements.length; i++) {
        elements[i].style.display = "block";
    }

    // $("#CurrentOperationDetailslink").show();
    var elements = document.getElementById("FieldDetailsViewHistoryChartContent");
    elements.style.display = 'block';
    //hide outcome and financial data
    $("#environmentalOutcomesSection")[0].style.setProperty('display', 'none', 'important');
    $('.finacialResult').hide();


    var xaxis = [];
    var yaxis = [];
    for (var i = 0; i < fsrChartDataList.length; i++) {
        var rowData = fsrChartDataList[i];

        xaxis.push(rowData.FieldYear);
        yaxis.push(rowData.FsrVal !== null ? rowData.FsrVal.toFixed(2) : 0);

    }

    //var years = [...new Set(fsrChartDataList.map(item => item.FieldYear))];
    //var crops = [...new Set(fsrChartDataList.map(item => item.Crop))];


    //var initialData = crops.map(crop => {
    //    return {
    //        x: years,
    //        y: years.map(year => {
    //            var entry = fsrChartDataList.find(item => item.FieldYear === year && item.Crop === crop);
    //            return entry ? entry.FsrVal : 0;
    //        }),
    //        type: 'bar',
    //        name: crop
    //    };
    //});

    //myFieldsFSRChart(xaxis, yaxis);

    var trace1 = {
        x: xaxis,
        y: yaxis,
        mode: 'markers',
        type: 'scatter',
        marker: {
            size: 12,
            color: 'green'
        }
    };

    var layout = {
        wrap: true,
        //barmode: 'group',
        xaxis: {
            title: {
                text: 'Year',
                font: {
                    size: 18,
                    color: '#073d1b',
                    family: 'Roboto Slab, serif',
                    weight: 'bold'
                }
            },
            type: 'category',
            autotick: true,
            ticks: 'outside',
            tick0: 0,
            //dtick: 0.25,
            //ticklen: 8,
            //tickwidth: 4,
            //tickcolor: '#000'
            tickfont: {
                size: 14,
                color: 'black',
                family: 'Roboto Slab, serif',
                weight: 'bold'
            }
        },
        yaxis: {
            title: {
                text: 'Field Stewardship Rating',
                font: {
                    size: 18,
                    color: '#073d1b',
                    family: 'Roboto Slab, serif',
                    weight: 'bold'
                }
            },
            tickfont: {
                size: 14,
                color: 'black',
                family: 'Roboto Slab, serif',
                weight: 'bold'
            }
        },
        modebar: {
            orientation: 'h',
            bgcolor: '#ffffff',
            color: '#073d1b',
            activecolor: '#073d1b',
            position: 'left'
        }
        //title: '2000 Toronto January Weather'
    };

    var data = [trace1];
    Plotly.newPlot('FieldsFSRViewHistory', data, layout);

}

$("#CurrentOperationDetailsEnvironmentalOutcomeslink").click(function (e) {
    e.preventDefault();
    $(".environmentalOutcomes").show();
    $(".fsrdata").show();
    $(".finacialResult").show();
    $(".fsrdataAdvisor").show();
    // $("#FieldDetailsViewHistoryEnvironmentalOutcomesChartContent").hide();
    $("#EnvironmentalOutcomesMoldboldTbl").hide();
    $("#EnvironmentalOutcomesAlflfaTbl").hide();
    var elements = document.getElementById("FieldDetailsViewHistoryEnvironmentalOutcomesChartContent");
    elements.style.display = 'none';
});

function myEnvironmentalFieldsFSRChart() {
    $("#loading").fadeIn();
    var field_Id = $('#id').val();
    var field_name = $('#name').val();
    var Outcomechart = $('#Outcomechart').val();
    var DropdownFrom = $('#DropdownFrom').val();
    var DropdownTo = $('#DropdownTo').val();
    $.ajax({
        url: '/Common/Get_EnvironmentalOutcomesChart',
        type: 'GET',
        data: { id: field_Id, name: field_name, DropdownFrom: DropdownFrom, DropdownTo: DropdownTo, Outcomechart: Outcomechart },
        success: function (data) {
            $("#loading").fadeOut();
            if (data.success == true) {
                $(".environmentalOutcomes")[0].style.setProperty('display', 'none', 'important');
                $(".fsrdata").hide();
                $(".finacialResult").hide();
                $(".fsrdataAdvisor").hide();
                // $("#FieldDetailsViewHistoryEnvironmentalOutcomesChartContent").show();
                var elements = document.getElementById("FieldDetailsViewHistoryEnvironmentalOutcomesChartContent");
                elements.style.display = 'block';

                var yaxistitle = 'Inches per year';
                if (Outcomechart === 'surface_water_field_runoff' || Outcomechart === 'water_retained_by_soil') {
                    yaxistitle = 'Acre-Feet per year';
                }
                else if (Outcomechart === 'soil_erosion' || Outcomechart === 'soil_leaving_field' || Outcomechart === 'downstream_sediment_impact_field' || Outcomechart === 'downstream_phosphorus_impact_field') {
                    yaxistitle = 'Tons per year';
                }
                else if (Outcomechart === 'phosphorus_mobilized' || Outcomechart === 'phosphorus_leaving_field') {
                    yaxistitle = 'Lbs per year';
                }
                else if (Outcomechart === 'field_stewardship_rating') {
                    yaxistitle = 'Field Stewardship Rating';
                }

                var xaxis = [];
                var yaxis = [];
                for (var i = 0; i < JSON.parse(data['data']).Table.length; i++) {
                    var rowData = JSON.parse(data['data']).Table[i];
                    xaxis.push(rowData.year_crop);
                    yaxis.push(rowData._outcomes_column !== null ? rowData._outcomes_column.toFixed(2) : 0);
                }

                var trace1 = {
                    x: xaxis,
                    y: yaxis,
                    mode: 'markers',
                    type: 'scatter',
                    marker: {
                        size: 12,
                        color: 'green'
                    }
                };
                var layout = {
                    xaxis: {
                        title: {
                            text: 'Year',
                            font: {
                                size: 18,
                                color: 'black',
                                family: 'Roboto Slab, serif',
                                weight: 'bold'
                            }
                        },
                        autotick: true,
                        ticks: 'outside',
                        tickfont: {
                            size: 14,
                            color: 'black',
                            family: 'Roboto Slab, serif',
                        }

                    },
                    yaxis: {
                        title: {
                            text: yaxistitle,
                            font: {
                                size: 18,
                                color: 'black',
                                family: 'Roboto Slab, serif',
                                weight: 'bold'
                            }
                        },
                        tickfont: {
                            size: 14,
                            color: 'black',
                            family: 'Roboto Slab, serif',
                            weight: 'bold'
                        }
                    },
                    modebar: {
                        orientation: 'h',
                        bgcolor: '#ffffff',
                        color: '#073d1b',
                        activecolor: '#073d1b',
                        position: 'left'
                    }
                };

                var data1 = [trace1];
                Plotly.newPlot('FieldsEnvironmentalOutcomesViewHistory', data1, layout);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            $("#loading").fadeOut();
        }
    });
}

$(function () {
    $('#ViewEnvironmentalHistorylink').click(function (e) {
        e.preventDefault();
        myEnvironmentalFieldsFSRChart();
    });
});

$('#PrintcropPopupopen').click(function () {
    PrintcropPopupopen();
});

function PrintcropPopupopen() {
    var field_Id = $('#id').val();
    $.ajax({
        url: '/Common/PrintCropCertificationPopup/',
        type: 'GET',
        cache: false,
        dataType: 'HTML',
        data: { id: field_Id },
        success: function (form) {
            if (form != null) {
                var modal = document.getElementById("printcropcertificationModal");
                modal.style.display = "block";
                $('#printcropcertificationModal').find('.modalBody').html(null);
                $('#printcropcertificationModal').find('.modalBody').html(form);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

$(function () {
    $('#printcropcertification').click(function (e) {
        e.preventDefault();
        $("#loading").fadeIn();
        var field_Id = $('#printid').val();
        var field_name = $('#printname').val();
        var DropdownTo = $('#printDropdownTo').val();
        var DropdownFrom = $('#printDropdownFrom').val();
        $.ajax({
            url: '/Common/PrintCropCertification',
            type: 'GET',
            dataType: 'html',
            cache: false,
            data: { id: field_Id, name: field_name, DropdownFrom: DropdownFrom, DropdownTo: DropdownTo },
            success: function (result) {
                //console.log(result)
                if (result != null) {
                    var $iframe = $('<iframe>', { srcdoc: result });
                    // Hide the iframe initially
                    $iframe.hide();
                    // Append the iframe to the current page
                    $('body').append($iframe);
                    // Trigger the print dialog for the iframe
                    $iframe[0].contentWindow.print();
                    // Clean up the iframe after printing is complete (optional)
                    $iframe.on('afterprint', function () {
                        $iframe.remove();
                    });
                }
                $("#loading").fadeOut();
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $("#loading").fadeOut();
            }
        });
    });

});

$('#Fertilizeropen').click(function () {
    PopUpFertilizerandPhosphorus("Fertilizeropen");
});

$('#Phosphorusopen').click(function () {
    PopUpFertilizerandPhosphorus("Phosphorusopen");
});

function PopUpFertilizerandPhosphorus(param1) {
    var field_Id = $('#id').val();
    var actiontype = param1;
    $.ajax({
        url: '/Common/FertilizerandPhosphorusPopup/',
        type: 'GET',
        cache: false,
        dataType: 'HTML',
        data: { id: field_Id, actiontype: actiontype },
        success: function (form) {
            $("#FertilizerandPhosphorusModal").show();
            // $("#CurrentOperationDetailslink").show();
            var elements = document.getElementsByClassName("CurrentOperationDetails");
            for (var i = 0; i < elements.length; i++) {
                elements[i].style.display = "block";
            }
            $(".fsrdata").hide();
            $(".environmentalOutcomes").hide();
            $(".finacialResult").hide();
            var FieldDetailsViewHistoryChartContent = document.getElementById("FieldDetailsViewHistoryChartContent");
            FieldDetailsViewHistoryChartContent.style.display = 'none';
            if (form != null) {
                var FertilizerandPhosphorusModaltitle = document.getElementById("FertilizerandPhosphorusModaltitle");
                if (actiontype == "Fertilizeropen") {
                    FertilizerandPhosphorusModaltitle.innerText = "Nitrogen Fertilizer 4R Levels";
                }
                else {
                    FertilizerandPhosphorusModaltitle.innerText = "Phosphorus 4R Levels";
                }
                $('#FertilizerandPhosphorusModaldetails').html(null);
                $('#FertilizerandPhosphorusModaldetails').html(form);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

$('#fieldPRiskSedimentopen').click(function () {
    PopUpFieldRisk("tp");
});

$('#fieldRiskSedimentopen').click(function () {
    PopUpFieldRisk("sed");
});

function PopUpFieldRisk(param1) {
    var field_Id = $('#id').val();
    var actiontype = param1;
    $.ajax({
        url: '/Common/FieldRiskPopup/',
        type: 'GET',
        cache: false,
        dataType: 'HTML',
        data: { id: field_Id, actiontype: actiontype },
        success: function (form) {
            $("#FieldRiskModal").show();
            // $("#CurrentOperationDetailslink").show();
            var elements = document.getElementsByClassName("CurrentOperationDetails");
            for (var i = 0; i < elements.length; i++) {
                elements[i].style.display = "block";
            }
            $(".fsrdata").hide();
            $(".environmentalOutcomes").hide();
            $(".finacialResult").hide();
            var FieldDetailsViewHistoryChartContent = document.getElementById("FieldDetailsViewHistoryChartContent");
            FieldDetailsViewHistoryChartContent.style.display = 'none';
            if (form != null) {
                var FieldRiskModaltitle = document.getElementById("FieldRiskModaltitle");
                if (actiontype == "sed") {
                    FieldRiskModaltitle.innerText = "Soil Loss from Field";
                }
                else {
                    FieldRiskModaltitle.innerText = "Total Phosphorus Loss from Field";
                }
                $('#FieldRiskModaldetails').html(null);
                $('#FieldRiskModaldetails').html(form);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

$("#CurrentfinancialDetailsEditcropnamelink").click(function (e) {
    e.preventDefault();
    $(".environmentalOutcomes").show();
    $(".fsrdata").show();
    $(".finacialResult").show();
    $("#pills-tabContent").show();
    var financialDetailsEditcropnamepopup = document.getElementById("financialDetailsEditcropnamepopup");
    financialDetailsEditcropnamepopup.style.display = 'none';
    var CurrentfinancialDetailsEditcropnamelink = document.getElementById("CurrentfinancialDetailsEditcropnamelink");
    CurrentfinancialDetailsEditcropnamelink.style.display = 'none';
    $("#financialDetailsEditcropnamedetails").hide();
    var elements = document.getElementsByClassName("CurrentOperationDetails");
    for (var i = 0; i < elements.length; i++) {
        elements[i].style.display = "none";
    }
});

function FinancialYearPopup(year, crop, field_option_id) {

    var field_Id = $('#id').val();
    var DropdownTo = $('#DropdownTo').val();
    var DropdownFrom = $('#DropdownFrom').val();

    $.ajax({
        url: '/Common/FinancialYearPopup/',
        type: 'GET',
        cache: false,
        dataType: 'HTML',
        data: { id: field_Id, year: year, crop: crop, field_option_id: field_option_id },
        success: function (formFinancial) {
            if (formFinancial != null) {
                var modal = document.getElementById("viewzonefinancialsModal");
                modal.style.display = "block";
                $('#viewzonefinancialsModal').find('.modalBody').html(null);
                $('#viewzonefinancialsModal').find('.modalBody').html(formFinancial);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function FinancialDetailsEditcropnamePopUp(id, field_option_id) {
    var field_Id = $('#id').val();
    var field_name = $('#name').val();
    var DropdownTo = $('#DropdownTo').val();
    var DropdownFrom = $('#DropdownFrom').val();
    $.ajax({
        url: '/Common/FinancialDetailsEditcropname/',
        type: 'GET',
        cache: false,
        dataType: 'HTML',
        data: { id: field_Id, name: field_name, DropdownFrom: DropdownFrom, DropdownTo: DropdownTo, field_option_id: field_option_id },
        success: function (form) {
            if (form != null) {
                var modal = document.getElementById("viewzonefinancialsModal");
                modal.style.display = "none";
                func_FinancialDetailResultchart(id);


                $(".fsrdata").hide();
                $(".environmentalOutcomes")[0].style.setProperty('display', 'none', 'important');
                $("#pills-tabContent").hide();
                $(".finacialResult").hide();
                var financialDetailsEditcropnamepopup = document.getElementById("financialDetailsEditcropnamepopup");
                financialDetailsEditcropnamepopup.style.display = 'block';
                var CurrentfinancialDetailsEditcropnamelink = document.getElementById("CurrentfinancialDetailsEditcropnamelink");
                CurrentfinancialDetailsEditcropnamelink.style.display = 'block';
                var FieldDetailsViewHistoryEnvironmentalOutcomesChartContent = document.getElementById("FieldDetailsViewHistoryEnvironmentalOutcomesChartContent");
                FieldDetailsViewHistoryEnvironmentalOutcomesChartContent.style.display = 'none';

                $("#financialDetailsEditcropnamedetails").show();
                $('#financialDetailsEditcropnamedetails').html(null);
                $('#financialDetailsEditcropnamedetails').html(form);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function func_FinancialDetailResultchart(id) {
    $.ajax({
        url: '/Common/FinancialResultChartsData',
        type: 'GET',
        dataType: 'JSON',
        data: { fieldId: $('#fieldId').val(), zydid: id },
        success: function (data) {
            if (data.message == 'No Data' || data == "" || data == null) {
                NodataChartImagis();
            }
            else {
                var yaxis = [];
                var Yaxis_InDirectCost = [];
                var Yaxis_DirectCost = [];
                var chartDataValues = data.chartDataValues.chartdata;
                var chartDataZidValues = data.chartDataValues.chartzydiddata;

                yaxis.push(chartDataZidValues[0].totalDirectCost, chartDataZidValues[0].totalInDirectCost, chartDataZidValues[0].totalProductionCost);

                Yaxis_InDirectCost.push(chartDataZidValues[0].otherExpensesHiredLabor, chartDataZidValues[0].otherExpensesMachineryLease, chartDataZidValues[0].farm_insurance, chartDataZidValues[0].dues_Fees, chartDataZidValues[0].utilities, chartDataZidValues[0].interest, chartDataZidValues[0].machine_bldg_depreciation, chartDataZidValues[0].real_estate_taxes, chartDataZidValues[0].otherExpensesOtherOverheadExpenses);

                Yaxis_DirectCost.push(chartDataZidValues[0].fartilizer, chartDataZidValues[0].crop_Chemicals, chartDataZidValues[0].other_Crop, chartDataZidValues[0].energy, chartDataZidValues[0].repair_Maintenance, chartDataZidValues[0].custom_Hire, chartDataZidValues[0].hired_Labor, chartDataZidValues[0].irrigation, chartDataZidValues[0].leases, chartDataZidValues[0].other);

                // var NetReturn = parseFloat(yaxis[2]).toFixed(2) / parseFloat(chartDataValues[0].acres).toFixed(2);
                $("#SquareNetReturn").html("<b>$" + parseFloat(chartDataZidValues[0].netReturn).toFixed(2) + "</b>");

                var regionalBenchmarkValue = chartDataZidValues[0].regionalBenchmark;

                $("#SquareRegionalBenchmark").html("<b>" + (regionalBenchmarkValue === 0 ? "N/A" : "$" + parseFloat(regionalBenchmarkValue).toFixed(2)) + "</b>");

                //Net return and repayment box value
                $("#NetReturnBreakevenBushelBox").html("<b>$" + parseFloat(chartDataZidValues[0].brkEvenBushelNetreturn).toFixed(2) + "</b>");
                $("#NetReturnBreakevenYieldAcresBox").html("<b>$" + parseFloat(chartDataZidValues[0].brkEvenYieldNetreturn).toFixed(2) + "</b>");

                $("#RepaymentBreakevenBushelBox").html("<b>$" + parseFloat(chartDataZidValues[0].brkEvenBushelRepayment).toFixed(2) + "</b>");
                $("#RepaymentBreakevenYieldAcresBox").html("<b>$" + parseFloat(chartDataZidValues[0].brkEvenYieldRepayment).toFixed(2) + "</b>");

                CostProductionSummaryGraphFinancial(yaxis);
                InDirectCostGraphFinancial(Yaxis_InDirectCost);
                DirectCostGraphFinancial(Yaxis_DirectCost);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            NodataChartImagis();
        }
    });
}

function NodataChartImagis() {
    $("#ProductionSummaryGraph").html('<img style="width:473px;" src="../images/chart_empty_exspense 1.jpg" alt="images"/>');
    //  $("#ProductionSummaryGraph").html('<img style="width:473px;" src="../images/chart_empty_exspense.jpg" alt="images"/>');
    $("#CostProductionInDirectCostsGraph").html('<img style="width: 1150px;" src="../images/chart_empty_indirect_cost.jpg" alt="images"/>');
    $("#CostProductionDirectCostsGraph").html('<img style="width: 1150px;" src="../images/chart_empty_direct_cost.jpg" alt="images"/>');
}

//Cost of Production Summary graph
function CostProductionSummaryGraphFinancial(yaxisData) {
    //console.log(yaxisData);
    var col = {
        x: ['Total <br> Direct<br>  Costs', 'Total <br> Indirect<br>  (Fixed)Costs', 'Total <br> Production<br>  Costs'],
        y: yaxisData,
        name: 'SF Zoo',
        type: 'bar',
        marker: { color: '#073d1b' }
    };
    var data = [col];
    var layout = {
        xaxis: {
            tickangle: 0,
            tickfont: { color: 'blace' }
        },
        barmode: 'group', bargap: 0.15, bargroupgap: 0.1, legend: {
            "orientation": "h",
            x: 0.01,
            y: 1.2
        },
        modebar: {
            orientation: 'h',
            bgcolor: '#ffffff',
            color: '#073d1b',
            activecolor: '#073d1b',
            position: 'left'
        },
    };
    Plotly.newPlot('ProductionSummaryGraph', data, layout);
}

//Cost of Production In Direct Costs Graph
function InDirectCostGraphFinancial(Yaxis_InDirectCost) {
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
            tickfont: { color: 'blace' }
        },
        legend: {
            "orientation": "h",
            x: 0.01,
            y: 1.2
        },

        barmode: 'group',
        bargap: 0.15, bargroupgap: 0.1,
        modebar: {
            orientation: 'h',
            bgcolor: '#ffffff',
            color: '#073d1b',
            activecolor: '#073d1b',
            position: 'left'
        },
    };

    Plotly.newPlot('CostProductionInDirectCostsGraph', data, layout);
}

//Cost of Production Direct Costs Graph
function DirectCostGraphFinancial(Yaxis_DirectCost) {
    //console.log(Yaxis_DirectCost);
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
        bargap: 0.15, bargroupgap: 0.1,
        modebar: {
            orientation: 'h',
            bgcolor: '#ffffff',
            color: '#073d1b',
            activecolor: '#073d1b',
            position: 'left'
        },
    };
    Plotly.newPlot('CostProductionDirectCostsGraph', data, layout);
}

$("#CurrentAdvisorOperationDetailslink").click(function () {
    $(".fsrdataAdvisor").show();
    $(".fsrdata").show();
    $(".environmentalOutcomes").show();
    $(".finacialResult").show();
    $("#soilindiciesAdvisorPopup").hide();
    $("#CurrentAdvisorOperationDetailslink").hide();

    var elements = document.getElementById("FieldDetailsViewHistoryChartContent");
    elements.style.display = 'none';
    const tabList = document.getElementById('pills-tab');

    if (activeTabName === 'pills-MyFields') {
        $("#MyFieldsenvironmentalOutcomesSection").hide();
    }
    else {
        $("#MyFieldsenvironmentalOutcomesSection").show();
    }
});

$('.fieldStewardship').click(function () {
    $("#loading").fadeIn();
    const Frsdatatext = $(this).data('value');
    fieldStewardshipPopupopen(Frsdatatext);
});

$('.fieldStewardship1').click(function () {
    $("#loading").fadeIn();
    const Frsdatatext = $(this).data('value');
    fieldStewardshipPopupopen(Frsdatatext);
});

function drawChart(chartidname, data) {
    var datatable = google.visualization.arrayToDataTable(data);
    var usersColumn = data.slice(1).map(function (row) {
        return row[1];
    });

    var minValue = Math.min(...usersColumn);
    var maxValue = Math.max(...usersColumn);

    // var adjustedMinValue = (minValue > 1) ? (minValue + 200) : (minValue + 0.200);
    // var adjustedmaxValue = (maxValue > 1) ? (maxValue + 200) : (maxValue + 0.200);

    var adjustedMinValue = (minValue < 0) ?
        ((minValue % 1 !== 0) ? (minValue - 0.30) : (minValue - 30)) :
        ((minValue % 1 !== 0) ? (minValue + 0.30) : (minValue + 30));

    var adjustedmaxValue = (maxValue < 0) ?
        ((maxValue % 1 !== 0) ? (maxValue - 0.30) : (maxValue - 30)) :
        ((maxValue % 1 !== 0) ? (maxValue + 0.30) : (maxValue + 30));


    var options = {
        title: 'Bubble Chart',
        height: 40, // Set the height of the chart to 50 pixels
        width: '100%',
        chartArea: {
            height: '80%', // Set the chart area height to 100% to fill the container
            width: '100%',
            right: '0%',
            top: '0%',
            bottom: '0%',
            backgroundColor: '#073d1b' // Set the background color of the chart area to none
        },
        hAxis: {
            textPosition: 'in', // Hide the x-axis labels
            baselineColor: '#073d1b',
            minValue: adjustedMinValue,
            maxValue: adjustedmaxValue,
            gridlines: {
                color: 'transparent'
            },
            axisLine: {
                color: 'red',
                width: 1
            },
            textStyle: {
                color: '#fff',
                fontSize: 13
            },

        },
        vAxis: {
            // Hide the y-axis
            textPosition: 'none',
            baselineColor: 'none',
            gridlines: {
                color: 'transparent'
            }
        },
        bubble: {
            textStyle: {
                fontSize: 10,
                bold: true,
                italic: true,
                auraColor: 'none'
            }
        },
        colors: ['#83d209'],
        sizeAxis: {
            maxSize: 15
        },
        tooltip: {
            textStyle: {
                fontSize: 11
            }
        }
    };

    //var chart = new google.visualization.BubbleChart(document.getElementById(chartidname));

    // chart.draw(datatable, options);
    var chartContainers = document.getElementsByClassName(chartidname);

    for (var i = 0; i < chartContainers.length; i++) {
        var chart = new google.visualization.BubbleChart(chartContainers[i]);
        chart.draw(datatable, options);
    }
}

function createCallback(chartIdName, data) {
    return function () {
        drawChart(chartIdName, data);
    };
}

function fetchDataAndPopulateModel(chartData, charttype) {
    bubblechartprepare(chartData);
}

function fieldStewardshipPopupopen(Frsdatatext) {
    var field_Id = $('#id').val();
    var DropdownFrom = $('#DropdownFrom').val();
    var DropdownTo = $('#DropdownTo').val();
    $.ajax({
        url: '/Common/FSRChartPopup/',
        type: 'GET',
        cache: false,
        dataType: 'HTML',
        data: { id: field_Id, DropdownFrom: DropdownFrom, DropdownTo: DropdownTo, chartType: Frsdatatext },
        success: function (response) {
            if (response !== null) {
                $("#loading").fadeIn();
                $(".fsrdata").hide();
                $(".fsrdataAdvisor").hide();
                $(".environmentalOutcomes")[0].style.setProperty('display', 'none', 'important');
                $(".finacialResult").hide();
                $("#CurrentAdvisorOperationDetailslink").show();

                var elements = document.getElementById("FieldDetailsViewHistoryChartContent");
                elements.style.display = 'none';
                const tabList = document.getElementById('pills-tab');
                const activeTab = tabList.querySelector('.active');
                activeTabName = activeTab.getAttribute('aria-controls');

                var elements = document.getElementsByClassName("CurrentOperationDetails");
                for (var i = 0; i < elements.length; i++) {
                    elements[i].style.display = "block";
                }

                if (activeTabName === 'pills-MyFarms') {
                    $("#soilindiciesPopup").show();
                    $('#soilindiciesPopup').html(null);
                    $('#soilindiciesPopup').html(response);
                }
                else {
                    $("#soilindiciesAdvisorPopup").show();
                    $('#soilindiciesAdvisorPopup').html(null);
                    $('#soilindiciesAdvisorPopup').html(response);
                }




                $(".SoilChartdata").hide();
                $(".NutrientChartdata").hide();
                $(".WaterChartdata").hide();
                $(".SurfaceWaterQualityChartData").hide();
                $(".FertilizerChartData").hide();

                if (Frsdatatext === 'rating_nutrient') {
                    $(".NutrientChartdata").show();
                }
                else if (Frsdatatext === 'rating_water') {
                    // WaterChartdatamodal.style.display = "block";
                    $(".WaterChartdata").show();
                }
                else if (Frsdatatext === 'rating_surface') {
                    // SurfaceWaterQualityChartDatamodal.style.display = "block";
                    $(".SurfaceWaterQualityChartData").show();
                    if (activeTabName === 'pills-MyFields') {
                        $(".SurfaceWaterQualitybanchmark").show();
                    }
                    else {
                        $(".SurfaceWaterQualitybanchmark").hide();
                    }
                }
                else if (Frsdatatext == "rating_soil") {
                    //SoilChartdatamodal.style.display = "block";
                    $(".SoilChartdata").show();
                }
                else {
                    // FertilizerChartDatamodal.style.display = "block";
                    $(".FertilizerChartData").show();
                    if (activeTabName === 'pills-MyFields') {
                        $(".SurfaceWaterQualitybanchmark").show();
                    }
                    else {
                        $(".SurfaceWaterQualitybanchmark").hide();
                    }
                }
                $("#loading").fadeOut();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function convertArrayToDataTable(dataArray) {
    const tabList = document.getElementById('pills-tab');
    const activeTab = tabList.querySelector('.active');
    activeTabName = activeTab.getAttribute('aria-controls');

    // Create an empty array to hold the converted data
    var dataTableArray = [];

    // Add the column headers as the first row in the DataTable
    dataTableArray.push(['bubblename', 'Value', 'size']);

    // Loop through each object in the dataArray and extract the required data
    dataArray.forEach(function (item) {
        var rowData = [item.bubblename, item.xaxis, item.size];
        dataTableArray.push(rowData);
    });

    if (activeTabName === 'pills-MyFarms') {
        var bubblenamesToRemove = ['O1', 'O2'];

        dataTableArray = dataTableArray.filter(function (rowData) {
            return !bubblenamesToRemove.includes(rowData[0]);
        });
    }
    return dataTableArray;
}

function bubblechartprepare(chartData) {
    $("#loading").fadeIn();
    google.charts.load('current', { packages: ['corechart'] });

    if (chartData.ChartType === 'rating_nutrient') {
        if (chartData.TpMobinag && chartData.TpMobinag.length !== 0) {
            var DtTpMobinagdata = convertArrayToDataTable(chartData.TpMobinag);

            google.charts.setOnLoadCallback(createCallback('chart-containerTPMobInAg', DtTpMobinagdata));
        }
        //if (chartData.TPMobdeinag && chartData.TPMobdeinag.length !== 0) {
        //    var dtTPMobdeinagdata = convertArrayToDataTable(chartData.TPMobdeinag);
        //    google.charts.setOnLoadCallback(createCallback('chart-containerTPMobDeInAg', dtTPMobdeinagdata));
        //}
        //if (chartData.Tpexinag && chartData.Tpexinag.length !== 0) {
        //    var dtTpexinagdata = convertArrayToDataTable(chartData.Tpexinag);
        //    google.charts.setOnLoadCallback(createCallback('chart-containerTPExInAg', dtTpexinagdata));
        //}
        if (chartData.Tpexdeinag && chartData.Tpexdeinag.length !== 0) {
            var dtTpexdeinagdata = convertArrayToDataTable(chartData.Tpexdeinag);
            google.charts.setOnLoadCallback(createCallback('chart-containerTPExDeInAg', dtTpexdeinagdata));
        }
        if (chartData.TPReBeln && chartData.TPReBeln.length !== 0) {
            var dtTPReBelndata = convertArrayToDataTable(chartData.TPReBeln);
            google.charts.setOnLoadCallback(createCallback('chart-containerTPReBeln', dtTPReBelndata));
        }
    }
    else if (chartData.ChartType === 'rating_water') {
        if (chartData.InBeln && chartData.InBeln.length !== 0) {
            var dtInBelndata = convertArrayToDataTable(chartData.InBeln);
            google.charts.setOnLoadCallback(createCallback('chart-containerInBeln', dtInBelndata));
        }
        //if (chartData.InDeInAg && chartData.InDeInAg.length !== 0) {
        //    var dtInDeInAgdata = convertArrayToDataTable(chartData.InDeInAg);
        //    google.charts.setOnLoadCallback(createCallback('chart-containerInDeInAg', dtInDeInAgdata));
        //}
        //if (chartData.RuBeIn && chartData.RuBeIn.length !== 0) {
        //    var dtRuBeIndata = convertArrayToDataTable(chartData.RuBeIn);
        //    google.charts.setOnLoadCallback(createCallback('chart-containerRuBeIn', dtRuBeIndata));
        //}
        if (chartData.RuDeInAg && chartData.RuDeInAg.length !== 0) {
            var dtRuDeInAgdata = convertArrayToDataTable(chartData.RuDeInAg);
            google.charts.setOnLoadCallback(createCallback('chart-containerRuDeInAg', dtRuDeInAgdata));
        }
        if (chartData.IrrUseEffBeIn && chartData.IrrUseEffBeIn.length !== 0) {
            var dtIrrUseEffBeIndata = convertArrayToDataTable(chartData.IrrUseEffBeIn);
            google.charts.setOnLoadCallback(createCallback('chart-containerIrrUseEffBeIn', dtIrrUseEffBeIndata));
        }
    }
    else if (chartData.ChartType === 'rating_soil') {

        if (chartData.SoWaErBeIndata && chartData.SoWaErBeIndata.length !== 0) {
            var dtSoWaErBeIndata = convertArrayToDataTable(chartData.SoWaErBeIndata);
            //var newColumn = ['bubblename', chartData.SoWaErBeIndata[0].tooltip, 'size'];

            //dtSoWaErBeIndata.unshift(newColumn);

            google.charts.setOnLoadCallback(createCallback('chart-container', dtSoWaErBeIndata));
        }
        //if (chartData.SoFoDeInAgdata && chartData.SoFoDeInAgdata.length !== 0) {
        //    var dtSoFoDeInAgdata = convertArrayToDataTable(chartData.SoFoDeInAgdata);
        //    google.charts.setOnLoadCallback(createCallback('chart-containerSoFoDeInAg', dtSoFoDeInAgdata));
        //}
        //if (chartData.SoWaErInAg && chartData.SoWaErInAg.length !== 0) {
        //    var dtSoWaErInAgdata = convertArrayToDataTable(chartData.SoWaErInAg);
        //    google.charts.setOnLoadCallback(createCallback('chart-containerSoWaErInAg', dtSoWaErInAgdata));
        //}
        if (chartData.SoWaErDeInAg && chartData.SoWaErDeInAg.length !== 0) {
            var dtSoWaErDeInAgdata = convertArrayToDataTable(chartData.SoWaErDeInAg);
            //var newColumn = ['bubblename', chartData.SoWaErDeInAg[0].tooltip, 'size'];

            //dtSoWaErDeInAgdata.unshift(newColumn);
            google.charts.setOnLoadCallback(createCallback('chart-containerSoWaErDeInAg', dtSoWaErDeInAgdata));
        }
        if (chartData.SoReBeIn && chartData.SoReBeIn.length !== 0) {
            var dtSoReBeIndata = convertArrayToDataTable(chartData.SoReBeIn);
            google.charts.setOnLoadCallback(createCallback('chart-containerSoReBeIn', dtSoReBeIndata));
        }
    }
    $("#loading").fadeOut();
}

//upload precip file (calculate FSR data)
if (window.location.pathname == '/Admin/CalculateFSRData') {
    var producerSelect = $('#ProducerFields');
    producerSelect.append($('<option>', {
        value: '',
        text: 'Select Field',
        selected: 'selected' // Set the selected attribute
    }));


    //GetprecipitationYear();
    //func_GetFieldsFromProducer();
    //NOTE: Upload new Precip file
    $("#btn_CalculateFSRPrecipFileUpload").on("click", function (event) {
        $("#loading").fadeIn();
        event.preventDefault();
        var formData = new FormData($("#CalculateFSRPrecipFileUploadForm")[0]);
        var field_Id = $('#ProducerFields').val();
        formData.append("Field_Id", field_Id);
        $.ajax({
            url: "/Admin/CalculateFSRPrecipFileUpload",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data) {
                $("#loading").fadeOut();
                alert(data.message);
                if (data.success == true) {
                    $(".excelUploadError").hide();
                    $('#SelectPrecipFileDropdown').append(new Option(data.filename, data.filename, true, true));
                    GetprecipitationYear();
                }
                else {
                    $(".excelUploadError").show();
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                $("#loading").fadeOut();
            }
        });
    });

    //NOTE: For selecting existing Precip dropdown item
    $("#btn_ApplyExistingPrecipFileData").on("click", function (event) {
        $("#loading").fadeIn();
        event.preventDefault();
        var formData = new FormData();
        var file_Name = $('#SelectPrecipFileDropdown').val(); // selected excel file dropdown
        var field_Id = $('#ProducerFields').val(); //get selected producer_field_id

        formData.append("Field_Id", field_Id);
        formData.append("File_Name", file_Name);

        $.ajax({
            url: "/Admin/ApplyExistingPrecipFileDropdownData",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data) {
                $("#loading").fadeOut();
                alert(data.message);

                if (data.success == true) {
                    GetprecipitationYear();
                    //window.location.href
                    //$(".excelUploadErrorDropdown").show();
                }
                else {
                    //$(".excelUploadErrorDropdown").hide();
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                $("#loading").fadeOut();
            }
        });
    });

    //POST: Upload irrigation file
    $("#btn_CalculateFSRIrrigationFile").on("click", function (event) {
        if ($('#txt_CO_Years').val() != '') {
            $("#loading").fadeIn();
            event.preventDefault();
            var formData = new FormData($("#CalculateFSRIrrigationFileUploadForm")[0]);
            formData.append("total_years", $('#txt_CO_Years').val());
            formData.append("Field_id", parseInt($("#ProducerFields").val()));
            $.ajax({
                url: "/Admin/CalculateFSRDUploadIrrigation",
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (data) {
                    $("#loading").fadeOut();
                    if (data.success == true) {
                        $('#irrigationUploadFiles').html("");
                        func_Get_IrrigatedFilesFromFieldId();
                        $("#excelUploadErrorIrrigation").hide();
                    }
                    else {
                        $("#excelUploadErrorIrrigation").html(`<span class="Complsry">* &nbsp;</span>${data.message}`);
                        $("#excelUploadErrorIrrigation").show();
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    $("#loading").fadeOut();
                }
            });
        }
    });

    //NOTE: For Calculating Benchmarks
    $("#btn_CalculateFSRBenchmarks").on("click", function (event) {
        if ($("#ProducerFields").val() != "") {
            $('#calc_BenchmarksError').hide();
            $("#onprocess").show();
            $("#offprocess").hide();
            $("#btn_CalculateFSRBenchmarks").attr("disabled", true);
            event.preventDefault();
            var Field_id = parseInt($("#ProducerFields").val());
            var Field_Option_id = parseInt($("#FSRFieldOptionNameDropdown").val());
            $.ajax({
                url: "/Admin/CalculateBenchmark",
                type: "GET",
                data: { field_Id: Field_id, FieldOptionId: Field_Option_id },
                success: function (data) {
                    if (data.success == true) {
                        $("#btn_CalculateFSRBenchmarks").attr("disabled", false);
                        $("#onprocess").hide();
                        $("#offprocess").show();
                        //$(".excelUploadError").hide();
                        //function call to append all rown on table
                        Step5AdjustInputValue();
                    }
                    else {
                        alert(data.message);
                        $("#onprocess").hide();
                        //$(".excelUploadError").show();
                        $("#offprocess").hide();
                        //$("#UploadRusle2Data").val(null);
                        $("#btn_CalculateFSRBenchmarks").attr("disabled", false);
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    $("#onprocess").hide();
                    $("#btn_CalculateFSRBenchmarks").attr("disabled", false);
                    alert(errorThrown);
                    // Handle the error response
                }
            });
        }
    });

    //NOTE: For PreviewFSR
    $("#btn_PreviewFSR").on("click", function (event) {
        //save input field, save button event
        func_btn_PreviewFSR_Save(event);

        $("#loading").fadeIn();

        //step6 value----Start----
        var DownstreamIssue = $("#DownstreamIssue").val();
        var StreamReach = $("#StreamReach").val();
        var WatershedName = $("#WatershedName").val();
        var MaxAllowedGov = $("#MaxAllowedGov").val();
        var SedimentGoal = $("#SedimentGoal").val();
        var TpGoal = $("#TpGoal").val();
        var PercContrUpstreamPrp = $("#PercContrUpstreamPrp").val();
        //step6 value----END----
        //step7 value----START---
        var PercFieldPrpContr = $("#PercFieldPrpContr").val();
        var UpstreamdrainagePrpContr = $("#UpstreamdrainagePrpContr").val();
        var TotalAreaUpstreamDrainagePr = $("#TotalAreaUpstreamDrainagePr").val();
        var TotalArea = $("#TotalArea").val();
        var AreaContrPrp = $("#AreaContrPrp").val();
        var SedPrpDelRatio = $("#SedPrpDelRatio").val();
        var TpPrpDelRatio = $("#TpPrpDelRatio ").val();
        var SedPrpNonpointSourceT = $("#SedPrpNonpointSourceT").val();
        var TpPrpNonpointSourceLb = $("#TpPrpNonpointSourceLb").val();
        //step7 value----End---

        if (
            DownstreamIssue !== null && DownstreamIssue.trim() !== '' &&
            StreamReach !== null && StreamReach.trim() !== '' &&
            WatershedName !== null && WatershedName.trim() !== '' &&
            MaxAllowedGov !== null && MaxAllowedGov.trim() !== '' &&
            SedimentGoal !== null && SedimentGoal.trim() !== '' &&
            TpGoal !== null && TpGoal.trim() !== '' &&
            PercContrUpstreamPrp !== null && PercContrUpstreamPrp.trim() !== '' &&
            PercFieldPrpContr !== null && PercFieldPrpContr.trim() !== '' &&
            UpstreamdrainagePrpContr !== null && UpstreamdrainagePrpContr.trim() !== '' &&
            TotalAreaUpstreamDrainagePr !== null && TotalAreaUpstreamDrainagePr.trim() !== '' &&
            TotalArea !== null && TotalArea.trim() !== '' &&
            AreaContrPrp !== null && AreaContrPrp.trim() !== '' &&
            SedPrpDelRatio !== null && SedPrpDelRatio.trim() !== '' &&
            TpPrpDelRatio !== null && TpPrpDelRatio.trim() !== '' &&
            SedPrpNonpointSourceT !== null && SedPrpNonpointSourceT.trim() !== '' &&
            TpPrpNonpointSourceLb !== null && TpPrpNonpointSourceLb.trim() !== ''

        ) {
            event.preventDefault();
            var Field_id = parseInt($("#ProducerFields").val());
            var Field_option_id = $("#FSRFieldOptionNameDropdown").val();

            //test code 
            //alert(`Preview FSR button clicke field_id=${Field_id}, Field_option_id=${Field_option_id}`);
            //func_previewFsrTablePartial();

            $.ajax({
                url: "/Admin/CalculatePreviewFSR",
                type: "GET",
                data: { field_Id: Field_id, field_Option_Id: Field_option_id },
                success: function (data) {
                    if (data.success) {
                        $("#loading").fadeOut();
                        alert(data.message);
                        //Swal.fire({
                        //    text: data.message,
                        //    icon: 'success'
                        //});
                        //partial view call function
                        func_previewFsrTablePartial();

                    } else {
                        alert(data.message);
                        //Swal.fire({
                        //    icon: "error",
                        //    text: data.message,
                        //});
                        $("#loading").fadeOut();
                    }
                },
                error: function (xhr, textStatus, errorThrown) {
                    // Handle the error response
                    $("#loading").fadeOut();
                    alert(errorThrown);
                    console.error("AJAX Error:", textStatus, errorThrown);
                }
            });
        }
        else {
            $("#loading").fadeOut();
            alert("All Input fields are required");
        }
    });
}

//NOTE: Selected fields acording fetch all uploaded excel files list
function func_Get_OperationYearsFromFieldId() {
    if (window.location.pathname == '/Admin/CalculateFSRData') {
        //$("#btn_PublishFSRtoWeb").hide(); //publih fsr
        //step:5 adjust value function
        $("#FSRFieldOptionNameDropdown").val(1);//default value current set(current=1) 
        Step5AdjustInputValue();
        GetprecipitationYear();

        $('#PreviewFSRTableModel').hide();
    }

    var Field_id = $("#ProducerFields").val();
    var Field_option_id = $("#FSRFieldOptionNameDropdown").val();
    var OptionNameDropdown = $('#FSRFieldOptionNameDropdown');
    $.ajax({
        url: '/Admin/Get_OperationYearsAndNameFromFieldId',
        type: 'POST',
        data: { field_Id: Field_id, field_Option_Id: Field_option_id },
        success: function (data) {
            $('#txt_CO_Years').val('');
            $('#Currunt_operation_Year').html("");
            $('#txt_CO_Years2').val('');
            $('#Currunt_operation_Year2').html("");
            func_Get_IrrigatedFilesFromFieldId();
            if (data.success) {
                if (data.data.length != 0) {
                    var years = data.data.map(obj => `${obj["year"]}`).join(', ');
                    $('#Currunt_operation_Year').append(years);
                    $('#txt_CO_Years').val(years);

                    $('#Currunt_operation_Year2').append(years);
                    $('#txt_CO_Years2').val(years);
                    if (data.data[0].filename == "")
                        $("#SelectPrecipFileDropdown").val($("#SelectPrecipFileDropdown option:first").val());
                    else
                        $("#SelectPrecipFileDropdown").val(data.data[0].filename);
                } else {
                    $('#Currunt_operation_Year').html("<span>No record found.</span>");
                    $('#Currunt_operation_Year2').html("<span>No record found.</span>");
                }
                //get fieldoptionName and append in dropdown
                var optionName = JSON.parse(data.optionDropdown);

                if (optionName.length > 0) {
                    OptionNameDropdown.empty();
                    for (var i = 0; i < optionName.length; i++) {
                        if (optionName[i].field_option_id !== 5 && optionName[i].field_option_id !== 6 && optionName[i].field_option_id !== 7) {
                            OptionNameDropdown.append($('<option>', {
                                value: optionName[i].field_option_id,
                                text: optionName[i].name
                            }));
                        }
                    }
                } else {
                    OptionNameDropdown.empty();
                    OptionNameDropdown.append($('<option>', { value: null, text: "Select Option" }));
                }
                //check to this selected field in benchmark calculated or not

            }
            else {
                $('#Currunt_operation_Year').html("<span>No record found.</span>");
                $('#Currunt_operation_Year2').html("<span>No record found.</span>");
            }
            //NOTE: Show/Hide irrigation tab if years are found
            if ($('#txt_CO_Years').val() != '')
                $("#div_IrrigationFileUpload").show();
            else
                $("#div_IrrigationFileUpload").hide();
        },
        error: function () {
            alert('Error retrieving producer list.');
        }
    });
}

function func_Get_IrrigatedFilesFromFieldId() {
    //hide benchmark process note
    $("#offprocess").hide();
    $("#onprocess").hide();
    var Field_id = $("#ProducerFields").val();
    $.ajax({
        url: '/Admin/GetIrrigatedFilesFromFieldId',
        type: 'POST',
        data: { field_Id: Field_id },
        success: function (data) {
            $('#irrigationUploadFiles').html("");
            if (data.success) {
                for (var i = 0; i < data.data.length; i++) {
                    //$('#irrigationUploadFiles').append(`<a target="_blank" href="/IrrigationFile/${data.data[i].filename}"> ${data.data[i].filename} </a>`);
                    $('#irrigationUploadFiles').append(`
                        <a target="_blank" href="/IrrigationFile/${data.data[i].filename}"> ${data.data[i].filename} </a>
                         <a href="#" onclick="func_RemoveFSRIrrigationFile()"><i class="bi bi bi-trash text-danger"></i></a>
                    `);
                }
            }
            else {
                $('#irrigationUploadFiles').html("<span>No files available.</span>");
            }
        },
        error: function () {
            alert('Error retrieving producer list.');
        }
    });
}

function func_previewFsrTablePartial() {
    //$("#loading").fadeIn(); //publih fsr
    var field_id = $("#ProducerFields").val();
    var field_option_id = $("#FSRFieldOptionNameDropdown").val();

    $.ajax({
        url: '/Admin/PreviewFSRDataTable',
        type: 'GET',
        data: { field_Id: field_id, field_Option_Id: field_option_id },
        success: function (response) {
            //$("#loading").fadeOut();  //publih fsr
            //$("#btn_PublishFSRtoWeb").show(); //publih fsr
            $("#PreviewFSRTableModel").show();
            $('#PreviewFSRTableModel').html(null);
            $('#PreviewFSRTableModel').html(response);
            $("#PreviewFSRTableModel").find('#PreviewFSRTable').DataTable({
                "pageLength": 50,
                "bLengthChange": false,
            });
            $("#PreviewFSRTableModel").find('#PreviewFSRTable2').DataTable({
                "pageLength": 50,
                "bLengthChange": false,
            });
            //add
            $('#PreviewFSRTable').closest('.col-sm-12').addClass('overflow-auto');
            $('#PreviewFSRTable2').closest('.col-sm-12').addClass('overflow-auto');
        },
        error: function () {
        }
    });
}

function func_RemoveFSRIrrigationFile() {
    var Field_id = $("#ProducerFields").val();
    var fileName = $("#irrigationUploadFiles a").attr("href").split("/").pop();
    var msg = "Are you sure you want to delete all irrigation data?";
    if (confirm(msg) == true) {

        $.ajax({
            url: '/Admin/RemoveFSRIrrigationFile',
            type: 'POST',
            data: { field_Id: Field_id, fileName: fileName },
            success: function (data) {
                if (data.success == true) {
                    alert("Irrigation data deleted successfully.");
                    func_Get_IrrigatedFilesFromFieldId()
                }
                else {
                    alert("Irrigation not deleted.");
                }
            },
            error: function () {
                alert('func_RemoveFSRIrrigationFile Error throw');
            }
        });
    } else {
        //console.log("Irrigation confirm cancle btn click .");
    }
}

if (window.location.pathname == '/Admin/UploadRusle2Data') {
    $("#AdminUploadRusle2data").on("click", function (event) {
        $("#onprocess").show();
        $("#offprocess").hide();
        event.preventDefault();
        var formData = new FormData($("#UploadRusle2Data")[0]);
        $.ajax({
            url: "/Admin/UploadRusle2Data",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data) {
                if (data.success == true) {
                    $('#hdn_FieldIds').val(data.fieldIDs.join(','));
                    $("#onprocess").hide();
                    $("#offprocess").show();
                    $("#PreviewRusle2TableModel").hide();
                    var element = document.getElementById("sucessmessage");
                    element.innerText = data.message;
                    $(".excelUploadError").hide();
                }
                else {
                    $("#onprocess").hide();
                    $(".excelUploadError").show();
                    $("#PreviewRusle2TableModel").hide();
                    var spanElements = document.getElementsByClassName('excelUploadError');
                    if (spanElements !== undefined) {
                        for (var i = 0; i < spanElements.length; i++) {
                            spanElements[i].innerHTML = data.message;
                        }
                    }
                    $("#offprocess").hide();
                    $("#UploadRusle2Data").val(null);
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                // Handle the error response
            }
        });
    });

    $("#AdminbulkUploadRusle2data").on("click", function (event) {
        $("#onprocess").show();
        $("#offprocess").hide();
        event.preventDefault();
        var formData = new FormData($("#UploadRusle2Data")[0]);
        $.ajax({
            url: "/Admin/UploadBulkRusle2Data",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data) {
                if (data.success == true) {
                    $('#hdn_FieldIds').val(data.fieldIDs.join(','));
                    $("#onprocess").hide();
                    $("#offprocess").show();
                    $("#PreviewRusle2TableModel").hide();
                    var element = document.getElementById("sucessmessage");
                    element.innerText = data.message;
                    $(".excelUploadError").hide();
                }
                else {
                    $("#onprocess").hide();
                    $(".excelUploadError").show();
                    $("#PreviewRusle2TableModel").hide();
                    var spanElements = document.getElementsByClassName('excelUploadError');
                    if (spanElements !== undefined) {
                        for (var i = 0; i < spanElements.length; i++) {
                            spanElements[i].innerHTML = data.message;
                        }
                    }
                    $("#offprocess").hide();
                    $("#UploadRusle2Data").val(null);
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                // Handle the error response
            }
        });
    });

    function func_GetRusle2Results() {
        $("#onprocess").show();
        $("#offprocess").hide();
        $.ajax({
            url: "/Admin/GetRusle2Results",
            type: "POST",
            data: { fieldIDs: $("#hdn_FieldIds").val() },
            success: function (data) {
                if (data.success == true) {
                    $("#onprocess").hide();
                    $("#offprocess").show();
                    var element = document.getElementById("sucessmessage");
                    element.innerText = data.message;
                    $(".excelUploadError").hide();
                    $("#PreviewRusle2TableModel").show();
                    var jsonData = JSON.parse(data.datatable);
                    $("#PreviewRusle2TableModel").find('#PreviewRusle2Table').DataTable({
                        destroy: true,
                        "pageLength": 50,
                        "bLengthChange": false,
                        "searching": false,
                        data: jsonData,
                        columns: [
                            { data: 'field_id', orderable: false },
                            { data: 'Zone_name', orderable: false },
                            { data: 'year', orderable: false },
                            { data: 'option', orderable: false },
                            { data: 'crop_name', orderable: false },
                            { data: 'tillage_name', orderable: false },
                            {
                                data: 'field_sediment_delivery_ratio', orderable: false, render: function (data, type, row) {
                                    // Format the data with three decimal places
                                    if (type === 'display') {
                                        if (data === 0) {
                                            return '0'; // Display '0' for zero values
                                        } else {
                                            return parseFloat(data).toFixed(2).replace(/(\.0+|0+)$/, '');
                                        }
                                    }
                                    return data;
                                }
                            },
                            {
                                data: 'field_tp_delivery_ratio', orderable: false, render: function (data, type, row) {
                                    // Format the data with three decimal places
                                    if (type === 'display') {
                                        if (data === 0) {
                                            return '0'; // Display '0' for zero values
                                        } else {
                                            return parseFloat(data).toFixed(2).replace(/(\.0+|0+)$/, '');
                                        }
                                    }
                                    return data;
                                }
                            },
                            {
                                data: 'total_sed_mass_leaving', orderable: false, render: function (data, type, row) {
                                    // Format the data with three decimal places
                                    if (type === 'display') {
                                        if (data === 0) {
                                            return '0'; // Display '0' for zero values
                                        } else {
                                            return parseFloat(data).toFixed(3).replace(/(\.0+|0+)$/, '');
                                        }
                                    }
                                    return data;
                                }
                            },
                            {
                                data: 'NET_C_FACTOR', orderable: false, render: function (data, type, row) {
                                    // Format the data with three decimal places
                                    if (type === 'display') {
                                        if (data === 0) {
                                            return '0'; // Display '0' for zero values
                                        } else {
                                            return parseFloat(data).toFixed(3).replace(/(\.0+|0+)$/, '');
                                        }
                                    }
                                    return data;
                                }
                            },
                            { data: 'CLIMATE_PTR', orderable: false },
                            { data: 'SOIL_PTR', orderable: false },
                            { data: 'MAN_BASE_PTR', orderable: false },
                            { data: 'slope_percentage', orderable: false }
                        ]
                    });
                }
                else {
                    alert(data.message);
                    $("#onprocess").hide();
                    $(".excelUploadError").show();
                    var spanElements = document.getElementsByClassName('excelUploadError');
                    if (spanElements !== undefined) {
                        for (var i = 0; i < spanElements.length; i++) {
                            spanElements[i].innerHTML = data.message;
                        }
                    }
                    $("#offprocess").hide();
                    $("#PreviewRusle2TableModel").hide();
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                // Handle the error response
            }
        });
    }
}

// Get data in fields dropdown above selected producer  (Field dropdown)
function func_GetFieldsFromProducer() {

    //func_Get_OperationYearsFromFieldId();
    //func_Get_OperationYearsFromFieldId();
    $("#FSRFieldOptionNameDropdown").val(1);//default value current set(current=1) 
    var ProducerId = $("#ProducerOnChnage").val();

    $("#FSRFieldOptionNameDropdown").empty();
    $("#FSRFieldOptionNameDropdown").append($('<option>', { value: null, text: "Select Option" }));

    $.ajax({
        url: '/Admin/GetFieldsByProducerIdDropDown',
        type: 'POST',
        data: { producerId: ProducerId },
        success: function (fielddata) {
            var producerSelect = $('#ProducerFields');
            if (fielddata.success) {
                if (window.location.pathname !== '/Admin/ExportFieldData') {
                    producerSelect.empty();
                    producerSelect.append($('<option>', {
                        value: '',
                        text: 'Select Field',
                        selected: 'selected' // Set the selected attribute
                    }));
                    for (var i = 0; i < fielddata.data.length; i++) {
                        producerSelect.append($('<option>', {
                            value: fielddata.data[i].field_Id,
                            text: fielddata.data[i].field_Name
                        }));
                    }
                }
                else if (window.location.pathname === '/Admin/ExportFieldData') {
                    //producerSelect.find('option').not(':first').remove();
                    producerSelect.empty();
                    if (fielddata.data.length > 0) {
                        for (var i = 0; i < fielddata.data.length; i++) {
                            producerSelect.append($('<option>', {
                                value: fielddata.data[i].field_Id,
                                text: fielddata.data[i].field_Name
                            }));
                        }
                    } else {
                        producerSelect.append($('<option>', {
                            value: '',
                            text: 'Select Field',
                            selected: 'selected' // Set the selected attribute
                        }));
                    }

                    GetFieldsTableForExportData("AllField");
                }
                func_Get_OperationYearsFromFieldId();

                var Field_option_id = $("#FSRFieldOptionNameDropdown").val();
                func_CalculateFSREnableDesableInputPropByOption(Field_option_id);
            }
            else {
                producerSelect.empty();

                if (window.location.pathname === '/Admin/ExportFieldData') {
                    producerSelect.find('option').not(':first').remove();
                }
                producerSelect.append($('<option>', {
                    value: '',
                    text: 'Select Field',
                    selected: 'selected' // Set the selected attribute
                }));
                $("#myTable").html(null);
            }
        },
        error: function () {
            alert('Error retrieving producer list.');
        }
    });
}

function GetprecipitationYear() {
    var Field_id = $("#ProducerFields").val();
    $.ajax({
        url: '/Common/PrecipitationTableYear',
        type: 'GET',
        data: { field_Id: Field_id },
        success: function (data) {
            $('#PrecipitationYear').html("");

            if (data.success) {
                const parsedData = JSON.parse(data.data);

                if (parsedData.Table.length > 0) {
                    const years = $.map(parsedData.Table, function (item) {
                        return item.year;
                    }).join(', ');
                    $('#PrecipitationYear').append(years);
                } else {
                    $('#PrecipitationYear').html("<span>No record found.</span>");

                }

            }
            else {
                $('#PrecipitationYear').html("<span>No record found.</span>");
            }
        },
        error: function () {
            $('#PrecipitationYear').html("<span>No record found.</span>");
        }
    });
}

function GetsingleFieldsTableForExpostData() {
    var selectedValue = document.getElementById('ProducerFields').value;
    if (selectedValue === '') {
        GetFieldsTableForExportData("AllField");
    }
    else {
        GetFieldsTableForExportData("SingleField");
    }
}

$('#exportaAllFieldDataBtn').click(function () {
    GetFieldsTableForExportData("AllFieldDownload");
});

//Note: get step-5 dropdown value
function func_CalculateFSREnableDesableInputPropByOption(Field_option_id) {
    if (Field_option_id == 3 || Field_option_id == 4) {
        //desable property when selected Operationid 3(Option2) and 2(Option3) 
        $(".disabledTag").attr('disabled', 'disabled');
        $("#irrigationUploadFiles").css("pointer-events", "none");
        //step 5,6,7 read only
        $(".disabledTag input").attr('disabled', 'disabled');
        $(".disabledTag select").attr('disabled', 'disabled')

    } else {
        $(".disabledTag").removeAttr('disabled');
        $(".disabledTag input").removeAttr('disabled');
        $(".disabledTag select").removeAttr('disabled');
        $("#irrigationUploadFiles").removeAttr('style');

    }
}
function Step5AdjustInputValue() {
    var Field_id = $("#ProducerFields").val();
    var Field_option_id = $("#FSRFieldOptionNameDropdown").val();

    func_CheckBenchmarkByFieldIdIndOptionId();
    func_CalculateFSREnableDesableInputPropByOption(Field_option_id);

    $.ajax({
        url: '/Admin/GetStep567AdjustInputValue',
        type: 'POST',
        data: { field_Id: Field_id, field_option_id: Field_option_id },
        success: function (data) {
            $('#Step5AdjustInputValueTable tbody').empty();
            $('#Step5HydroMzValueTable tbody').empty();
            $('#Step5AdjustInputTotalpValueTable tbody').empty();
            $('#Step5TotalpMzValueTable tbody').empty();
            $('#Step5AdjustInputSoillossValueTable tbody').empty();
            $('#Step5SoillossMzValueTable tbody').empty();
            if (data.success) {
                //---------------Hydrology Start------------
                var Hydrology = JSON.parse(data.hydrology);
                var Totalp = JSON.parse(data.totalp);
                var Soilloss = JSON.parse(data.soilLoss);
                var Step6 = JSON.parse(data.step6Value);
                var Step7 = JSON.parse(data.step7Value);
                var Step5MzInput = JSON.parse(data.step5MzInputValue);

                $("#FieldId").val(Hydrology.Table[0].field_id);
                // hydrologyLambda input box
                //$("#HydrologyLambda").val(Hydrology.Table[0].lambda);
                Hydrology.Table[0].lambda !== null && Hydrology.Table[0].lambda !== "" ? (Hydrology.Table[0].lambda = Number(Hydrology.Table[0].lambda).toFixed(2), $("#HydrologyLambda").val(Hydrology.Table[0].lambda)) : null;

                // hydrologySlope input box
                //$("#HydrologySlope").val(Hydrology.Table[0].slope);

                //set Hydrology MZ and ZYD data

                $("#hydrology_ZydId").val(Hydrology.Table[0].id);
                $("#hydrology_MzId").val(Hydrology.Table[0].management_zones_id);


                for (var i = 0; i < Step5MzInput.Table.length; i++) {
                    $('#Step5HydroMzValueTable tbody').append(`<tr> <td>${Step5MzInput.Table[i].zone_name}</td> <td><input class="form-control p-1 w-25" id="HydrologySlope${i}" maxlength="50" placeholder="0" type="number" value="${Step5MzInput.Table[i].slope}"><input id="HydrologySlope${i}_mzId" type="hidden" value="${Step5MzInput.Table[i].management_zones_id}"></td></tr>`);
                    $('#Step5TotalpMzValueTable tbody').append(`<tr>
                    <td>${Step5MzInput.Table[i].zone_name}</td> 
                    <td><input class="form-control p-1 w-25" id="Totalp_TPDissolved${i}" maxlength="50" placeholder="0" type="number" value="${Step5MzInput.Table[i].perc_tp_dissolved !== null ? Step5MzInput.Table[i].perc_tp_dissolved.toFixed(2) : '0.00'}"></td>
                    <td><input class="form-control p-1 w-25" id="Totalp_TPSediment${i}" maxlength="50" placeholder="0" type="number" value="${Step5MzInput.Table[i].perc_tp_sediment !== null ? Step5MzInput.Table[i].perc_tp_sediment.toFixed(2) : '0.00'}"></td>
                    <td><input class="form-control p-1 w-25" id="Totalp_FieldTPDeleveryRatio${i}" maxlength="50" placeholder="0" type="number" value="${Step5MzInput.Table[i].field_tp_delivery_ratio !== null ? Step5MzInput.Table[i].field_tp_delivery_ratio : '0.000'}">
                    <input id="TotalpMzinputBoxRow${i}_mzId" type="hidden" value="${Step5MzInput.Table[i].management_zones_id}">
                    </td>
                    </tr>`);
                    $('#Step5SoillossMzValueTable tbody').append(`<tr> <td>${Step5MzInput.Table[i].zone_name}</td> <td><input class="form-control p-1 w-25" id="Soilloss_SedimentDeleveryRatio${i}" maxlength="50" placeholder="0" type="number" value="${Step5MzInput.Table[i].field_sediment_delivery_ratio !== null ? Step5MzInput.Table[i].field_sediment_delivery_ratio.toFixed(3) : '0.00'}">
                    <input id="Soilloss_SedimentDeleveryRatio${i}_mzId" type="hidden" value="${Step5MzInput.Table[i].management_zones_id}">
                    </td></tr>`);

                }
                // table in append value   
                for (var i = 0; i < Hydrology.Table.length; i++) {
                    $('#Step5AdjustInputValueTable tbody').append(`<tr><td>${Hydrology.Table[i].zone_name}</td><td>${Hydrology.Table[i].crop}</td><td>${Hydrology.Table[i].year}</td><td>${Hydrology.Table[i].option}</td><td>${Hydrology.Table[i].cn1}</td></tr>`);
                }
                //---------------Hydrology End------------
                //--------------Total p Start-------------

                //perc_tp_dissolved input box
                $("#Totalp_TPDissolved").val(Totalp.Table[0].perc_tp_dissolved !== null ? Totalp.Table[0].perc_tp_dissolved.toFixed(2) : '0.00');

                //perc_tp_sediment input box
                $("#Totalp_TPSediment").val(Totalp.Table[0].perc_tp_sediment !== null ? Totalp.Table[0].perc_tp_sediment.toFixed(2) : '0.00');

                //field_tp_delivery_ratio input box
                $("#Totalp_FieldTPDeleveryRatio").val(Totalp.Table[0].field_tp_delivery_ratio !== null ? Totalp.Table[0].field_tp_delivery_ratio.toFixed(2) : '0.00');
                //console.log(Totalp.Table)

                // table in append value
                for (var i = 0; i < Totalp.Table.length; i++) {
                    $('#Step5AdjustInputTotalpValueTable tbody').append(`<tr><td>${Totalp.Table[i].zone_name}</td><td>${Totalp.Table[i].crop}</td><td>${Totalp.Table[i].year}</td><td>${Totalp.Table[i].option}</td>
                        <td><input class="form-control p-1" id="Totalp_TPStructural${i}" maxlength="50" placeholder="0" type="number" value="${Totalp.Table[i].tp_structure !== null ? Totalp.Table[i].tp_structure.toFixed(3) : '0.000'}"></td>
                        <td><input class="form-control p-1" id="Totalp_TPManagement${i}" maxlength="50" placeholder="0" type="number" value="${Totalp.Table[i].tp_management !== null ? Totalp.Table[i].tp_management.toFixed(3) : '0.000'}"></td>
                        <td><input class="form-control p-1" id="Totalp_TPDrainageFactor${i}" maxlength="50" placeholder="0" type="number" value="${Totalp.Table[i].tp_drainage_factor !== null ? Totalp.Table[i].tp_drainage_factor.toFixed(2) : '0.00'}"></td>
                        <input id="Totalp_Row${i}_ZydId" type="hidden" value="${Totalp.Table[i].id}">
                        <input id="Totalp_Row${i}_MzId" type="hidden" value="${Totalp.Table[i].management_zones_id}">
                        </tr>`);
                }

                //----------------Total p End-------------
                //----------------Soil Loss Start-----------
                //Soilloss_SoilUnitWeight
                $("#Soilloss_SoilUnitWeight").val(Soilloss.Table[0].soil_unit_weight !== null ? Soilloss.Table[0].soil_unit_weight.toFixed(2) : '0.00');
                //Soilloss_SoilFormationRate
                $("#Soilloss_SoilFormationRate").val(Soilloss.Table[0].soil_formation_rate !== null ? Soilloss.Table[0].soil_formation_rate.toFixed(2) : '0.00');
                //Soilloss_SedimentDeleveryRatio
                $("#Soilloss_SedimentDeleveryRatio").val(Soilloss.Table[0].field_sediment_delivery_ratio !== null ? Soilloss.Table[0].field_sediment_delivery_ratio.toFixed(2) : '0.00');

                //console.log(Soilloss.Table)
                for (var i = 0; i < Soilloss.Table.length; i++) {
                    $('#Step5AdjustInputSoillossValueTable tbody').append(`<tr><td>${Soilloss.Table[i].zone_name}</td><td>${Soilloss.Table[i].crop}</td><td>${Soilloss.Table[i].year}</td><td>${Soilloss.Table[i].option}</td>
                        <td><input class="form-control p-1" id="Soilloss_SedStructural${i}" maxlength="50" placeholder="0" type="number" value="${Soilloss.Table[i].sed_structure !== null ? Soilloss.Table[i].sed_structure.toFixed(3) : '0.000'}"></td>
                        <td><input class="form-control p-1" id="Soilloss_SedManagement${i}" maxlength="50" placeholder="0" type="number" value="${Soilloss.Table[i].sed_management !== null ? Soilloss.Table[i].sed_management.toFixed(3) : '0.000'}"></td>
                        <td><input class="form-control p-1" id="Soilloss_SedDrainageFactor${i}" maxlength="50" placeholder="0" type="number" value="${Soilloss.Table[i].sed_drainage_factor !== null ? parseFloat(Soilloss.Table[i].sed_drainage_factor).toFixed(2) : '0.00'}">
                        <input id="Soilloss_Row${i}_ZydId" type="hidden" value="${Soilloss.Table[i].id}">
                        <input id="Soilloss_Row${i}_MzId" type="hidden" value="${Soilloss.Table[i].management_zones_id}"></td>
                        </tr>`);
                }
                //----------------Soil Loss End-------------

                //---------------step6 value-----START------
                // console.log(Step6);
                // console.log(Step7);

                if (Step6.Table[0].downstream_new != null || Step6.Table[0].downstream_new != undefined) { $("#DownstreamIssue").val(`${Step6.Table[0].downstream_new}`); }
                //$("#DownstreamIssue").val(`${Step6.Table[0].downstream_new}`);
                if (Step6.Table[0].stream_reach != null || Step6.Table[0].stream_reach != undefined) { $("#StreamReach").val(Step6.Table[0].stream_reach); } else { $("#StreamReach").val(""); }
                if (Step6.Table[0].watershed_name != null || Step6.Table[0].watershed_name != undefined) { $("#WatershedName").val(Step6.Table[0].watershed_name); } else { $("#WatershedName").val(""); }

                if (Step6.Table[0].max_allowed_gov != null && Step6.Table[0].max_allowed_gov != undefined) { $("#MaxAllowedGov").val(`${Step6.Table[0].max_allowed_gov}`); } else {$("#MaxAllowedGov").val("true") }
                //$("#MaxAllowedGov").val(`${Step6.Table[0].max_allowed_gov}`);
                if (Step6.Table[0].sediment_goal != null && Step6.Table[0].sediment_goal != undefined && Step6.Table[0].sediment_goal != 0) { $("#SedimentGoal").val(`${Step6.Table[0].sediment_goal}`); } else { $("#SedimentGoal").val(30) }
                //$("#SedimentGoal").val(Step6.Table[0].SedimentGoal);
                if (Step6.Table[0].tp_goal != null && Step6.Table[0].tp_goal != undefined && Step6.Table[0].tp_goal != 0) { $("#TpGoal").val(`${Step6.Table[0].tp_goal}`); } else { $("#TpGoal").val(12) }
                //$("#TpGoal").val(Step6.Table[0].tp_goal);
                if (Step6.Table[0].perc_contr_upstream_prp != null && Step6.Table[0].perc_contr_upstream_prp != undefined && Step6.Table[0].perc_contr_upstream_prp != 0) { $("#PercContrUpstreamPrp").val(`${parseFloat(Step6.Table[0].perc_contr_upstream_prp).toFixed(2)}`); } else { $("#PercContrUpstreamPrp").val(100) }
                //$("#PercContrUpstreamPrp").val(Step6.Table[0].perc_contr_upstream_prp);
                //---------------step6 value-----END------
                //console.log(`${Step6.Table[0].max_allowed_gov}`);

                //---------------step7 value-----START------
                $("#PercFieldPrpContr").val(!isNaN(Step7.Table[0].perc_field_prp_contr) && Step7.Table[0].perc_field_prp_contr !== null && Step7.Table[0].perc_field_prp_contr !=0 ? parseFloat(Step7.Table[0].perc_field_prp_contr).toFixed(2) : "100.00");
                $("#UpstreamdrainagePrpContr").val(!isNaN(Step7.Table[0].total_area_upstream_prp) && Step7.Table[0].total_area_upstream_prp !== null ? parseFloat(Step7.Table[0].total_area_upstream_prp).toFixed(2) : "0.00");
                $("#TotalAreaUpstreamDrainagePr").val(!isNaN(Step7.Table[0].upstream_drainage_prp_contr) && Step7.Table[0].upstream_drainage_prp_contr !== null ? parseFloat(Step7.Table[0].upstream_drainage_prp_contr).toFixed(2) : "0.00");

                $("#TotalArea").val(!isNaN(Step7.Table[0].total_area) ? parseFloat(Step7.Table[0].total_area).toFixed(2) : "0.00");

                $("#AreaContrPrp").val(
                    !isNaN(Step7.Table[0].area_contr_prp) && Step7.Table[0].area_contr_prp !== null
                        ? parseFloat(Step7.Table[0].area_contr_prp).toFixed(2)
                        : $("#TotalArea").val()
                ); !isNaN(Step7.Table[0].upstream_drainage_prp_contr) && Step7.Table[0].upstream_drainage_prp_contr !== null ? parseFloat(Step7.Table[0].upstream_drainage_prp_contr).toFixed(2) : "0.00"
                $("#FielsAppInputCalc").val(Step7.Table[0].zone_runoff_avg === 0 ? 100 : Step7.Table[0].zone_runoff_avg);
                $("#SedPrpDelRatio").val(Step7.Table[0] && !isNaN(Step7.Table[0].sed_prp_del_ratio) && Step7.Table[0].sed_prp_del_ratio !== null ? parseFloat(Step7.Table[0].sed_prp_del_ratio) : "0.00");

                $("#TpPrpDelRatio").val(Step7.Table[0] && !isNaN(Step7.Table[0].tp_prp_del_ratio) && Step7.Table[0].tp_prp_del_ratio !== null ? parseFloat(Step7.Table[0].tp_prp_del_ratio) : "0.00");
                $("#SedPrpNonpointSourceT").val(Step7.Table[0] && !isNaN(Step7.Table[0].sed_prp_nonpointsource_t) && Step7.Table[0].sed_prp_nonpointsource_t !== null ? parseFloat(Step7.Table[0].sed_prp_nonpointsource_t).toFixed(2) : "0.00");
                $("#TpPrpNonpointSourceLb").val(Step7.Table[0] && !isNaN(Step7.Table[0].tp_prp_nonpointsource_lb) && Step7.Table[0].tp_prp_nonpointsource_lb !== null ? parseFloat(Step7.Table[0].tp_prp_nonpointsource_lb).toFixed(2) : "0.00");



                //calculation input
                $("#AreaContrPrp").on('input', function () {
                    Step7CalcInput();
                });
                $("#FielsAppInputCalc").on('input', function () {
                    Step7CalcInput();
                });
                Step7CalcInput();

                function Step7CalcInput() {
                    var AreaContrPrp = parseFloat($('#AreaContrPrp').val().replace(/[^\d.]/g, ''))
                    var FielsAppInputCalc = $("#FielsAppInputCalc").val();
                    var fieldAreaContributingPrp = parseFloat($('#FieldAreaContributingPrp').val().replace(/[^\d.]/g, ''))

                    var calc = AreaContrPrp * (FielsAppInputCalc / 100);
                    $("#FieldAreaContributingPrp").val(calc.toFixed(2));

                    var PrpAreaFieldNonContriCalculation = 100 - FielsAppInputCalc;

                    $("#PrpAreaFieldNonContri").val(PrpAreaFieldNonContriCalculation.toFixed(2));


                    var FiAreaNotContribRunoffLoadPRPCalculation = (AreaContrPrp * PrpAreaFieldNonContriCalculation) / 100;
                    $("#FiAreaNotContribRunoffLoadPRP").val(FiAreaNotContribRunoffLoadPRPCalculation.toFixed(2));


                }
                //when table in append data after table input property enable and desable by option
                func_CalculateFSREnableDesableInputPropByOption(Field_option_id);
                //---------------step6 value-----END------
            } else {
                $('#Step5AdjustInputValueTable tbody').html(`<tr> <td colspan="5">Data not available</td> </tr>`);
                $('#Step5HydroMzValueTable tbody').html(`<tr> <td colspan="2">Data not available</td> </tr>`);

                $('#Step5AdjustInputTotalpValueTable tbody').html(`<tr> <td colspan="7">Data not available</td> </tr>`);
                $('#Step5TotalpMzValueTable tbody').html(`<tr> <td colspan="4">Data not available</td> </tr>`);

                $('#Step5AdjustInputSoillossValueTable tbody').html(`<tr> <td colspan="7">Data not available</td> </tr>`);
                $('#Step5SoillossMzValueTable tbody').html(`<tr> <td colspan="2">Data not available</td> </tr>`);
            }
        },
        error: function () {
            alert('Error retrieving producer list.');
        }
    });
}
$("#btn_PreviewFSR_Save").on("click", function (event) {
    func_btn_PreviewFSR_Save(event);
    console.log("Save button clicked");
});

function func_btn_PreviewFSR_Save(event) {

    //$("#btn_PreviewFSR_Save").on("click", function (event) {
    var ObjInputValue = {};
    var ObjSoilloss = {};
    var ObjTotalp = {};
    var ObjHydrology = {};
    var ObjStep6 = {};
    var ObjStep7 = {};
    var ObjMzInput = {};
    var ObjStep5FieldValue = {};
    //field_id
    var FieldId = $("#FieldId").val();
    //table row count
    var SoilLossCount = $('#Step5AdjustInputSoillossValueTable tbody tr').length;
    //field - Soilloss_SoilUnitWeight
    var fi_Soilloss_SoilUnitWeight = $(`#fi_Soilloss_SoilUnitWeight`).val() || 0;
    //fi - Soilloss_SoilFormationRate
    var fi_Soilloss_SoilFormationRate = $(`#fi_Soilloss_SoilFormationRate${i}`).val() || 0;
    //mz - Soilloss_SedimentDeleveryRatio
    var mz_Soilloss_SedimentDeleveryRatio = $(`#mz_Soilloss_SedimentDeleveryRatio${i}`).val() || 0;


    for (var i = 0; i < SoilLossCount; i++) {
        var Soilloss_SedStructural = $(`#Soilloss_SedStructural${i}`).val() || 0;
        var Soilloss_SedManagement = $(`#Soilloss_SedManagement${i}`).val() || 0;
        var Soilloss_SedDrainageFactor = $(`#Soilloss_SedDrainageFactor${i}`).val() || 0;
        var Zydid = $(`#Soilloss_Row${i}_ZydId`).val() || 0;
        var Mzid = $(`#Soilloss_Row${i}_MzId`).val() || 0;

        // Store the values in the object
        ObjSoilloss[`Soil_lossRow${i}`] = { Zydid, Mzid, Soilloss_SedStructural, Soilloss_SedManagement, Soilloss_SedDrainageFactor, fi_Soilloss_SoilUnitWeight, fi_Soilloss_SoilFormationRate, mz_Soilloss_SedimentDeleveryRatio };
    }

    // -------total p --------------START-----------

    //table row count
    var TotalpCount = $('#Step5AdjustInputTotalpValueTable tbody tr').length;
    //field - Soilloss_SoilUnitWeight
    var mz_Totalp_TPDissolved = $("#Totalp_TPDissolved").val() || 0;
    //fi - Soilloss_SoilFormationRate
    var Totalp_TPSediment = $("#Totalp_TPSediment").val() || 0;
    //mz - Soilloss_SedimentDeleveryRatio
    var mz_Totalp_FieldTPDeleveryRatio = $("#Totalp_FieldTPDeleveryRatio").val() || 0;


    for (var i = 0; i < TotalpCount; i++) {
        var Totalp_TPStructural = $(`#Totalp_TPStructural${i}`).val() || 0;
        var Totalp_TPManagement = $(`#Totalp_TPManagement${i}`).val() || 0;
        var Totalp_TPDrainageFactor = $(`#Totalp_TPDrainageFactor${i}`).val() || 0;
        var Zydid1 = $(`#Totalp_Row${i}_ZydId`).val() || 0;
        var Mzid1 = $(`#Totalp_Row${i}_MzId`).val() || 0;

        // Store the values in the object
        ObjTotalp[`TotalpRow${i}`] = { FieldId, Zydid1, Mzid1, Totalp_TPStructural, Totalp_TPManagement, Totalp_TPDrainageFactor, mz_Totalp_TPDissolved, Totalp_TPSediment, mz_Totalp_FieldTPDeleveryRatio };
    }

    // -------total p --------------END-----------

    // -------Hydrology p --------START---------
    //HydrologyLambda

    var fi_HydrologyLambda = $("#HydrologyLambda").val() || 0;
    // hydrologySlope input box
    var MZ_HydrologySlope = $("#HydrologySlope").val() || 0;

    var hydrology_ZydId = $("#hydrology_ZydId").val() || 0;
    var Hydrology_MzId = $("#hydrology_MzId").val() || 0;

    ObjHydrology["Hydrology_data"] = { FieldId, fi_HydrologyLambda, MZ_HydrologySlope, hydrology_ZydId, Hydrology_MzId };

    // -------Hydrology p----------END-----------
    //step6 value----START---
    var DownstreamIssue = $("#DownstreamIssue").val() || 0;
    var StreamReach = $("#StreamReach").val() || 0;
    var WatershedName = $("#WatershedName").val() || 0;
    var MaxAllowedGov = $("#MaxAllowedGov").val() || 0;
    var SedimentGoal = $("#SedimentGoal").val() || 0;
    var TpGoal = $("#TpGoal").val() || 0;
    var PercContrUpstreamPrp = $("#PercContrUpstreamPrp").val() || 0;
    ObjStep6["Step6_data"] = { FieldId, DownstreamIssue, StreamReach, WatershedName, MaxAllowedGov, SedimentGoal, TpGoal, PercContrUpstreamPrp }
    //step6 value----END----
    //step7 value----START------
    var PercFieldPrpContr = $("#PercFieldPrpContr").val() || 0;
    var UpstreamdrainagePrpContr = parseFloat($('#UpstreamdrainagePrpContr').val().replace(/[^\d.]/g, '')).toFixed(2) || 0;
    var TotalAreaUpstreamDrainagePr = parseFloat($('#TotalAreaUpstreamDrainagePr').val().replace(/[^\d.]/g, '')).toFixed(2) || 0;
    var TotalArea = parseFloat($('#TotalArea').val().replace(/[^\d.]/g, '')).toFixed(2) || 0;
    var AreaContrPrp = parseFloat($('#AreaContrPrp').val().replace(/[^\d.]/g, '')).toFixed(2) || 0;
    var SedPrpDelRatio = $("#SedPrpDelRatio").val() || 0;
    var TpPrpDelRatio = $("#TpPrpDelRatio ").val() || 0;
    var SedPrpNonpointSourceT = parseFloat($('#SedPrpNonpointSourceT').val().replace(/[^\d.]/g, '')).toFixed(2) || 0;
    var TpPrpNonpointSourceLb = parseFloat($('#TpPrpNonpointSourceLb').val().replace(/[^\d.]/g, '')).toFixed(2) || 0;
    ObjStep7["Step7_data"] = { FieldId, PercFieldPrpContr, UpstreamdrainagePrpContr, TotalAreaUpstreamDrainagePr, TotalArea, AreaContrPrp, SedPrpDelRatio, TpPrpDelRatio, SedPrpNonpointSourceT, TpPrpNonpointSourceLb }

    //step7 value----SENDTART---
    //all field value----------------

    var Soilloss_SoilUnitWeight = $("#Soilloss_SoilUnitWeight").val() || 0;
    var Soilloss_SoilFormationRate = $("#Soilloss_SoilFormationRate").val() || 0;
    var HydrologyLambda = $("#HydrologyLambda").val() || 0;
    ObjStep5FieldValue = { FieldId, Soilloss_SoilUnitWeight, Soilloss_SoilFormationRate, HydrologyLambda };

    //All MZ input box for tep5-------------
    var Step5HydroMzValueTable = $('#Step5HydroMzValueTable tbody tr');
    var Step5TotalpMzValueTable = $('#Step5TotalpMzValueTable tbody tr');
    var Step5SoillossMzValueTable = $('#Step5SoillossMzValueTable tbody tr');
    if (Step5HydroMzValueTable != null && Step5TotalpMzValueTable != null && Step5SoillossMzValueTable != null) {
        for (var i = 0; i < Step5HydroMzValueTable.length; i++) {
            var HydrologySlope_mzId = $(`#HydrologySlope${i}_mzId`).val() || 0;
            var HydrologySlope = $(`#HydrologySlope${i}`).val() || 0;

            var Totalp_TPDissolved = $(`#Totalp_TPDissolved${i}`).val() || 0;
            var Totalp_TPSediment = $(`#Totalp_TPSediment${i}`).val() || 0;
            var Totalp_FieldTPDeleveryRatio = $(`#Totalp_FieldTPDeleveryRatio${i}`).val() || 0;
            var TotalpMzinputBoxRow = $(`#TotalpMzinputBoxRow${i}_mzId`).val() || 0;

            var Soilloss_SedimentDeleveryRatio = $(`#Soilloss_SedimentDeleveryRatio${i}`).val() || 0;
            var Soilloss_SedimentDeleveryRatio_mzId = $(`#Soilloss_SedimentDeleveryRatio${i}_mzId`).val() || 0

            // Store the values in the object
            ObjMzInput[`Spep5MzInputRow${i}`] = { HydrologySlope_mzId, HydrologySlope, Totalp_TPDissolved, Totalp_TPSediment, Totalp_FieldTPDeleveryRatio, TotalpMzinputBoxRow, Soilloss_SedimentDeleveryRatio, Soilloss_SedimentDeleveryRatio_mzId };
        }
    }
    //--------------------------------------

    ObjInputValue.SoilLoss = ObjSoilloss; //add soilloss value in object
    ObjInputValue.TotalP = ObjTotalp; //add totalp value in object
    ObjInputValue.Hydrology = ObjHydrology; //add totalp value in object
    ObjInputValue.Step6 = ObjStep6; //add Step6 value in object
    ObjInputValue.Step7 = ObjStep7; //add Step7 value in object
    ObjInputValue.Step5MzInput = ObjMzInput; //add mz input value in object   
    ObjInputValue.Step5FieldInput = ObjStep5FieldValue; //add allfield value in object   
    //console.log(ObjInputValue);

    $.ajax({
        url: '/Admin/UpdateStep567AdjustInputValue',
        type: 'POST',
        data: { AllInputData: JSON.stringify(ObjInputValue) },
        success: function (data) {
            if (event.target.innerText != 'Calculate FSR') {
                if (data.success) {
                    alert("Data saved successfully.");
                }
                else {
                    alert("Data not updated.");
                }
            }
        }
    });
    //});
}
function func_SetDefaultValuesInFieldLocation() {
    //1. Field Location Overview text
    $("#FieldLocationOverview").val(`A watershed is an area of land whose surface water drains to a common location or “outlet.” An outlet might be a drainage ditch just downstream from your field, a lake or reservoir, or a specific location along a stream or river. Watershed size varies considerably. 

The U.S. Geological Survey uses a formal system to name and describe watersheds based on their size. The watershed size of interest for describing stewardship is a “subwatershed” or 12-digit Hydrologic Unit Code (HUC). Subwatershed size averages 25,600 acres (40 sq. mi.) and ranges from 10,000 acres (15.6 sq. mi.) to 40,000 acres (62.5 sq. mi.). 

This size is important for describing stewardship because local, state and federal agencies often manage water and set water goals at this scale. 

The map shows the location of your field relative to the watershed outlet (yellow arrow). Water, sediment and nutrients leaving your field ultimately drain to the watershed outlet. As the proportion of the total amount sediment and total phosphorus from your field at the watershed outlet increases, so does the risk of environmental regulation.`);
    //2.management zone ovwrview
    $("#FieldLocationMzOverview").val(`Several factors influence your farming methods and crop yield across a field including the elevation and slope of the land, the types of soils, the quality of drainage and the depth of groundwater below the land surface. These factors are not uniform across your field. 

Management zone boundaries reflect the combination of factors including the field’s physical characteristics, yield, trafficability and operational challenges, as they change across the field. Subdividing the field into management zones allows you plan your operation to achieve the best use of each portion of the field. 

Each color represents a management zone (note: a single color means the entire field is one management zone). You can vary your operation by management zone.`);

    $("#Fieldlocatinn_fieldError").hide();
}

var fields_id = $("#ProducerFields").val();
if (fields_id != null) {

}
//image preview code
$("#FielsLocationImagUploadBtn").change(function () {
    $("#imgPreview").show();
    const file = this.files[0];
    if (file) {
        let reader = new FileReader();
        reader.onload = function (event) {
            $("#imgPreview")
                .attr("src", event.target.result);
        };
        reader.readAsDataURL(file);
    }
});

$("#FielsLocationImagCancleBtn").on("click", function (event) {
    $("#imgPreview").attr("src", "");
    $("#imgValueLocation").val("");
    $("#FielsLocationImagUploadBtn").val("");
});

$("#ManageFieldLocationDataGoBtn").on("click", function (event) {
    let fieldId = $("#ProducerFields").val();
    if (fieldId == null || fieldId == "") {
        $("#Fieldlocatinn_fieldError").show();
        $('#div_LocationData').hide();
    }
    else {
        $('#div_LocationData').show();
        $.ajax({
            url: "/Admin/GetManageFieldLocationImage",
            type: "GET",
            data: { Field_Id: fieldId },
            success: function (ManageFielddata) {
                if (ManageFielddata.success) {
                    var item = JSON.parse(ManageFielddata.data);
                    if (item.Table.length > 0) {
                        if (item.Table[0].field_location_img != null || item.Table[0].field_location_overview != null || item.Table[0].mz_overview != null) {
                            if (item.Table[0].field_location_img != null) {
                                var ImgName = item.Table[0].field_location_img;
                                $("#imgPreview").attr("src", "../OtherImages/" + ImgName);
                            }
                            var FieldLocationOverview = item.Table[0].field_location_overview;
                            var MzOverview = item.Table[0].mz_overview;
                            $("#FieldLocationOverview").val(FieldLocationOverview)
                            $("#FieldLocationMzOverview").val(MzOverview);
                            $("#Fieldlocatinn_fieldError").hide();
                            $("#imgValueLocation").val(ImgName);
                        }
                        else {
                            func_SetDefaultValuesInFieldLocation();
                        }
                    }
                }
                else
                    func_SetDefaultValuesInFieldLocation();
            },
            error: function (xhr, textStatus, errorThrown) {
                alert('Error retrieving fields list.');
                $("#Fieldlocatinn_fieldError").show();
            }
        });
    }
});

//get geometry

function getGeometryImageFieldInformation() {
    const fieldId = $("#fieldId").val();

    $.ajax({
        url: "/Common/Get_FieldInformationGeometry",
        type: "GET",
        data: { Field_Id: fieldId },
        success: function (data) {
            if (data.success === true) {
                // Parse field geometry
                const geoJsonData1 = JSON.parse(data.fieldGeometry);
                const MzGeoJson = data.mzGeometry.map((geometry) => JSON.parse(geometry));
                const pointx = data.pointx === '' ? 0 : JSON.parse(data.pointx);
                const pointy = data.pointy === '' ? 0 : JSON.parse(data.pointy);
                initMap(geoJsonData1, MzGeoJson, pointx, pointy);
            } else {
                // Show error message
                alert('Error retrieving fields list.');
                $("#Fieldlocatinn_fieldError").show();
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            // Show error message
            alert('Error retrieving fields list.');
            $("#Fieldlocatinn_fieldError").show();
        }
    });
}

//map initial Start...
function calculateDynamicCenter(geoJsonData) {
    let totalLat = 0;
    let totalLng = 0;
    let coords;
    if (geoJsonData != undefined || geoJsonData != null) {

        const features = geoJsonData.features;
        // Calculate the total sum of latitudes and longitudes
        for (let i = 0; i < features.length; i++) {
            coords = features[i].geometry.coordinates[0][0]; // Assuming the first set of coordinates contains the polygon
            // coords = features[i].geometry.coordinates; // Assuming the first set of coordinates contains the polygon
            for (let j = 0; j < coords.length; j++) {
                totalLat += coords[j][1];
                totalLng += coords[j][0];
                //totalLat += coords[1];
                //totalLng += coords[0];
            }
        }

        // Calculate the average latitude and longitude
        const averageLat = totalLat / (features.length * coords.length);
        const averageLng = totalLng / (features.length * coords.length);
        return { lat: averageLat, lng: averageLng };

    } else {
        return { lat: 44.065, lng: -114.745 };
    }
}

// Initialize and display the map
function initMap(geoJsonData1, MzGeoJson, pointx, pointy) {
    var bounds = new google.maps.LatLngBounds();
    const dy = calculateDynamicCenter(geoJsonData1);
    const map = new google.maps.Map(document.getElementById('map'), {
        center: dy, // Set initial center based on the first GeoJSON data
        mapTypeId: 'satellite',
        zoom: 12
    });

    if (pointx !== 0 || pointy !== 0) {
        var pointA = new google.maps.LatLng(dy);
        var pointB = new google.maps.LatLng(pointx, pointy);
        var lineCoordinates = [
            pointA,
            pointB
        ];
        bounds.extend(pointA);
        bounds.extend(pointB);
        var line = new google.maps.Polyline({
            path: [pointA, pointB],
            geodesic: true,
            strokeColor: '#FF0000', // Color of the line
            strokeOpacity: 1.0,
            strokeWeight: 2, // Width of the line
        });

        line.setMap(map);
        var arrowIcon = {
            path: google.maps.SymbolPath.BACKWARD_CLOSED_ARROW,
            strokeColor: '#FF0000',
            fillColor: '#FF0000',
            fillOpacity: 1.0,
        };

        var arrowMarker = new google.maps.Marker({
            position: lineCoordinates[lineCoordinates.length - 1],
            icon: arrowIcon,
            map: map,
            zIndex: 999 // Ensure the arrowhead is on top
        });

        if (pointx != 0 && pointy != 0) {
            const marker = new google.maps.Marker({
                position: { lat: pointx, lng: pointy }, // Coordinates of your desired location
                map: map, // The map to attach the marker to
                icon: {
                    path: google.maps.SymbolPath.FORWARD_CLOSED_ARROW,
                    strokeColor: '#FFFF00',
                    fillColor: '#FFFF00',
                    fillOpacity: 1,
                    scale: 5,
                    // anchor: new google.maps.Point(0,0)
                },
            });
        }
    }
    else {
        var pointA = new google.maps.LatLng(dy);
        bounds.extend(pointA);
        google.maps.event.addListenerOnce(map, 'bounds_changed', function () {
            // Wait for the 'bounds_changed' event to be triggered after fitting the bounds
            // Then, zoom out slightly by decreasing the zoom level (adjust the value as needed)
            map.setZoom(map.getZoom() - 7);
        });
    }

    // Add GeoJSON data as map overlays
    var geoJsonLayer1 = new google.maps.Data();
    geoJsonLayer1.addGeoJson(geoJsonData1);
    geoJsonLayer1.setStyle({
        fillColor: geoJsonData1.features[0].properties.border,
        strokeColor: geoJsonData1.features[0].properties.border,
        fillOpacity: 0.5
    });

    map.fitBounds(bounds);
    geoJsonLayer1.setMap(map);
    //get every loop new border color select
    const colorNames = ["Red", "Blue", "Yellow", "Purple", "Orange", "Pink", "Cyan", "Magenta", "Brown", "White", "Black", "Gray", "Lime", "Teal", "Aqua", "Maroon", "Navy", "Olive", "Silver", "Gold", "Indigo", "Coral", "Violet", "Turquoise"];

    //Note : Thia loop is fields zone map with diffrent color border render
    for (let i = 0; i < MzGeoJson.length; i++) {
        const geoJsonData2 = MzGeoJson[i];
        const colorIndex = i % colorNames.length; // Get a different color for each iteration

        const geoJsonLayer2 = new google.maps.Data();
        geoJsonLayer2.addGeoJson(geoJsonData2);
        geoJsonLayer2.setStyle({
            fillColor: colorNames[colorIndex],
            strokeColor: colorNames[colorIndex],
            fillOpacity: 0.5
        });
        geoJsonLayer2.setMap(map);
    }
}
//map initial End.

$('#exportDataBtn').click(function () {
    GetsingleFieldsTableForExportData();
});

//NOTE: Export table single field data export
function GetsingleFieldsTableForExportData() {
    var exportDataTable = document.getElementById("myTable");
    var producer_Id = $('select[name="ProducerId"]').val();
    var field_Id = $('select[id="ProducerFields"]').val();
    //get api in appsetting.json
    var api = $('#GetExportFieldApi').val();
    if (exportDataTable && exportDataTable.rows.length !== 0 && producer_Id != null && field_Id != '') {
        $("#ExportDataTable").table2excel({
            name: "Table2Excel",
            filename: "Export_RUSLE2 Data",
            fileext: ".xls"
        });

        //api call when click button
        //location.replace("https://axcendiw.azurewebsites.net/downloadFieldsAndZones/106");

        var mainUrl = `${api}/${producer_Id}/${field_Id}`;
        location.replace(mainUrl);
    }
}

//NOTE: Export table all field data export
function GetFieldsTableForExportData(actiontype) {
    $("#ExportDataTable").show();
    let fieldId = parseInt($("#ProducerFields").val());
    let Actiontype = actiontype;
    var ProducerId = $("#ProducerOnChnage").val();
    var apiUrl = $('#GetExportAllFieldApi').val();
    $.ajax({
        url: "/Admin/GetFieldsTableForExportData",
        type: "POST",
        data: { fieldId: fieldId, producerId: ProducerId, Actiontype: Actiontype },
        success: function (data) {
            if (data.success) {
                var tableHtml = "";
                for (var i = 0; i < data.data.length; i++) {
                    var boundariesexistHTML = data.data[i].boundariesexist
                        ? "Yes"
                        : "<span style='color: red;'>N/A</span>";
                    //var topologycheckHTML = "0 - 0 ";

                    // if (data.data[i].topologycheck != 'N/A') {
                    //topologycheckHTML = data.data[i].topologycheck.includes("red") ?
                    //    `<span style='color: red;'>${data.data[i].topologycheck.replace('red', '')}</span>` :
                    //    data.data[i].topologycheck;
                    // } 
                    var topologyCheckValue = data.data[i].topologycheck;
                    var topologyColor = data.data[i].topologycolor;

                    var topologycheckHTML = data.data[i].topologycheck !== 'N/A'
                        ? (topologyColor === 'red'
                            ? `<span style='color: red;'>${topologyCheckValue}</span>`
                            : topologyCheckValue)
                        : "0 - 0";
                    var dataenteredHTML = data.data[i].dataentered
                        ? "Yes"
                        : "<span style='color: red;'>N/A</span>";
                    var exportreadyHTML = data.data[i].exportready === 'Yes'
                        ? "Yes"
                        : "<span style='color: red;'>No</span>";
                    var row = "<tr><td>" + data.data[i].fieldId + "</td><td>" + data.data[i].fieldName + "</td><td>" + data.data[i].management_Zone_Id + "</td><td>" + data.data[i].management_Zone + "</td><td>" + data.data[i].year + "</td><td>" + data.data[i].yieldClass + "</td><td> " + boundariesexistHTML + "</td><td> " + topologycheckHTML + "</td><td> " + dataenteredHTML + "</td><td> " + exportreadyHTML + "</td><td> " + data.data[i].field_Option_Id + "</td><td> " + data.data[i].crop1_Name + "</td><td> " + data.data[i].crop1_Rusle2_Lookup_Value + "</td><td> " + data.data[i].primary_Tillage + "</td><td> " + data.data[i].primary_Tillage_Rusle2_Lookup + "</td><td> " + data.data[i].secondary_Tillage + "</td><td> " + data.data[i].secondary_Tillage_Rusle2_Lookup + "</td><td> " + data.data[i].net_C_Factor + "</td><td> " + data.data[i].cN3 + "</td></tr > ";
                    tableHtml += row;
                }
                $("#myTable").html(tableHtml);

                if (actiontype === "AllFieldDownload") {
                    var exportDataTable = document.getElementById("myTable");
                    if (exportDataTable && exportDataTable.rows.length !== 0) {
                        $("#ExportDataTable").table2excel({
                            name: "Table2Excel",
                            filename: "Export_RUSLE2 Data",
                            fileext: ".xls"
                        });

                        //api call when click button
                        //location.replace("https://axcendiw.azurewebsites.net/downloadFieldsAndZones/106");

                        var mainUrl = `${apiUrl}/${ProducerId}`;
                        location.replace(mainUrl);
                    }
                }
                // $("#exportDataBtn").show();
            }
            else {
                $("#myTable").html("<p class='mt - 2'> Data not available. </p>");
                // $("#exportDataBtn").hide();
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            alert('Error retrieving fields list.');
        }
    });
}

function func_CheckBenchmarkByFieldIdIndOptionId() {

    var Field_id = $("#ProducerFields").val();
    var Field_option_id = $("#FSRFieldOptionNameDropdown").val();
    if (Field_option_id == 3 || Field_option_id == 4) {
        var OptionNameDropdown = $('#FSRFieldOptionNameDropdown');
        $.ajax({
            url: '/Admin/CheckBenchmarkByFieldIdIndOptionId',
            type: 'GET',
            data: { field_Id: Field_id, field_Option_Id: Field_option_id },
            success: function (data) {
                if (data.success == true) {
                    if (data.benchmarkCheck == false) {
                        alert("You must calculate the results for the Current Operation prior to calculating a plan");
                        func_Get_OperationYearsFromFieldId();
                    }
                }
            },
            error: function () {
            }
        });
    }
}

//NOTE: '/WebApp/ApplyPrecipAndIrrigationFile'
function func_ApplyPrecipAndIrrigationFile(event) {
    if ($('#ProducerOnChnage').val() != '') {
        if (event.getAttribute('data-type') === 'Selected') {
            if ($('#ProducerFields').val() == '') {
                alert('You must have to select Field to proceed further.');
                return;
            }
        }
        $("#loading").fadeIn();
        //event.preventDefault();
        var formData = new FormData($("#ApplyPrecipAndIrrigationFile")[0]);
        formData.append("total_years", $('#txt_CO_Years').val());
        formData.append("Field_id", parseInt($("#ProducerFields").val()));
        formData.append("ProducerId", parseInt($("#ProducerOnChnage").val()));
        formData.append("type", event.getAttribute('data-type'));
        var file_Name = $('#SelectPrecipFileDropdown').val();
        formData.append("File_Name", file_Name);
        //NOTE: Set all the fieldIDs in string
        var fieldIDs = $('#ProducerFields option').map(function () {
            return $(this).val();
        }).get().filter(function (value) {
            return value.trim() !== ''; // Remove values that are empty or contain only whitespace
        });

        var fieldIDsString = fieldIDs.join(',');
        formData.append("fieldIDs", fieldIDsString);

        $.ajax({
            url: "/WebApp/ApplyPrecipAndIrrigationFile",
            type: "POST",
            data: formData,
            processData: false,
            contentType: false,
            success: function (data) {
                $("#loading").fadeOut();
                if (data.success == true) {
                    $("#excelUploadErrorIrrigation").hide();
                    if (data.yearsErrors != '')
                        $("#div_YearsErrors").find('label').html(data.yearsErrors.split(';').join('<br>'));
                    //$("#div_YearsErrors").find('h5').html(data.successMessage);
                    alert(data.successMessage);
                    window.location.reload();
                }
                else {
                    $("#excelUploadErrorIrrigation").html(`<span class="Complsry">* &nbsp;</span>${data.message}`);
                    $("#excelUploadErrorIrrigation").show();
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                $("#loading").fadeOut();
            }
        });
    }
    else
        alert('You must have to select Producer to proceed further.');
}
//rusle 2 bulk process
function func_BatchProcessingFSR(param) {

    buttonText = param.innerHTML.trim();
    var producerid = $("#ProducerOnChnage").val();
    var field_id = $("#ProducerFields").val();
    var a = $("#FSRBulkProcessOption").val();
    var optionsName = $("#FSRBulkProcessOption").children("option:selected").attr("id");
    var requestData = { ProducerId: producerid, Field_id: field_id, type: buttonText, options: optionsName };
    //clean error and table
    $("#div_BatchErrors label").text(null);
    $("#AdminOperationBulkFSrTable tbody").html(null);
    $("#BulkFsrRusle2Value tbody").html(null);
    //$("#AdminOperationBulkFSrTable").show();
    function func_PerformOperation() {
        $("#loading").fadeIn();
        $.ajax({
            url: "/WebApp/BulkBenchmarkAndCalculateFSRProcess",
            type: "POST",
            data: requestData,
            success: function (data) {
                if (data.success == true) {
                    alert("Benchmark and calculate FSR bulk process completed.");
                    response = data.responsesList;
                    console.log(response);
                    $("#AdminOperationBulkFSrTable").show();
                    $("#BulkFsrRusle2Value").show();
                    for (var i = 0; i < response.length; i++) {
                        //console.log(response[i]);
                        //status table
                        $("#AdminOperationBulkFSrTable tbody").append(`<tr>
                                    <td>${response[i].Producer_id}</td>
                                    <td>${response[i].Field_id}</td>
                                    <td>${response[i].BenchmarkSuccess}</td>
                                    <td>${response[i].CalcFSRSuccess}</td>
                                    <td>${response[i].CalcFSROption1Success}</td>
                                    <td>${response[i].CalcFSROption2Success}</td>
                        </tr>`);
                        //benchmak rusle2 view
                        var rusle2bench = response[i].Rusle2ResponseListBulk.Rusle2ResponseBench
                        var dataArraybench = JSON.parse(rusle2bench);
                        if (response.length > 1) {
                            dataArraybench = JSON.parse(dataArraybench)
                        }
                        dataArraybench.forEach(function (item) {
                            var rusle2response = JSON.parse(item.rusle2response);
                            var netCFactor = rusle2response.net_c_factor;
                            var textColor = netCFactor > 0.4 ? 'red' : 'black';

                            var row = "<tr>" +
                                "<td>" + rusle2response.fieldID + "</td>" +
                                "<td>" + rusle2response.mZoneID + "</td>" +
                                "<td>" + rusle2response.field_option_id + "</td>" +
                                "<td>" + rusle2response.year + "</td>" +
                                "<td>" + rusle2response.total_sed_mass_leaving + "</td>" +
                                "<td style='color: " + textColor + "'>" + netCFactor + "</td>" +
                                "</tr>";
                            $("#BulkFsrRusle2Value tbody").append(row);
                        });
                        //Op1 reusle2 view
                        if (response[i].Rusle2ResponseListBulk.Rusle2ResponseOp1 != null){
                            var rusle2Op1 = response[i].Rusle2ResponseListBulk.Rusle2ResponseOp1
                            var dataArrayOp1 = JSON.parse(rusle2Op1);
                            if (response.length > 1) {
                                dataArrayOp1 = JSON.parse(dataArrayOp1)
                            }
                            dataArrayOp1.forEach(function (item) {
                                var rusle2response = JSON.parse(item.rusle2response);
                                var netCFactor = rusle2response.net_c_factor;
                                var textColor = netCFactor > 0.4 ? 'red' : 'black';
                                var row = "<tr>" +
                                    "<td>" + rusle2response.fieldID + "</td>" +
                                    "<td>" + rusle2response.mZoneID + "</td>" +
                                    "<td>" + rusle2response.field_option_id + "</td>" +
                                    "<td>" + rusle2response.year + "</td>" +
                                    "<td>" + rusle2response.total_sed_mass_leaving + "</td>" +
                                    "<td style='color: " + textColor + "'>" + netCFactor + "</td>" +
                                    "</tr>";
                                $("#BulkFsrRusle2Value tbody").append(row);
                            });
                        }

                        //Op2 reusle2 view
                        if (response[i].Rusle2ResponseListBulk.Rusle2ResponseOp2 != null) {

                        var rusle2Op2 = response[i].Rusle2ResponseListBulk.Rusle2ResponseOp2
                        var dataArrayOp2 = JSON.parse(rusle2Op2);
                        if (response.length > 1) {
                            dataArrayOp2 = JSON.parse(dataArrayOp2)
                        }
                        dataArrayOp2.forEach(function (item) {
                            var rusle2response = JSON.parse(item.rusle2response);
                            var netCFactor = rusle2response.net_c_factor;
                            var textColor = netCFactor > 0.4 ? 'red' : 'black';
                            var row = "<tr>" +
                                "<td>" + rusle2response.fieldID + "</td>" +
                                "<td>" + rusle2response.mZoneID + "</td>" +
                                "<td>" + rusle2response.field_option_id + "</td>" +
                                "<td>" + rusle2response.year + "</td>" +
                                "<td>" + rusle2response.total_sed_mass_leaving + "</td>" +
                                "<td style='color: " + textColor + "'>" + netCFactor + "</td>" +
                                "</tr>";
                            $("#BulkFsrRusle2Value tbody").append(row);
                        });
                        }

                    }

                    $("#loading").fadeOut();
                } else {
                    alert(data.message);
                    $("#AdminOperationBulkFSrTable").hide();
                    $("#BulkFsrRusle2Value").hide();
                    $("#loading").fadeOut();
                }
            },
            error: function (xhr, textStatus, errorThrown) {
                $("#loading").fadeOut();
                $("#AdminOperationBulkFSrTable").hide();
                $("#BulkFsrRusle2Value").hide();
                console.log(errorThrown);
            }
        });
    }

    // Apply For All Producers
    if (buttonText == "Apply For All Producers") {
        func_PerformOperation();
    }
    // Apply For Selected Producer All Fields
    else if (buttonText == "Apply For Selected Producer All Fields" && producerid !== "") {
        func_PerformOperation();
    }
    // Apply For Selected Field
    else if (buttonText == "Apply For Selected Field" && producerid !== "" && field_id !== "") {
        func_PerformOperation();
    }
    // Invalid conditions
    else {
        $("#div_BatchErrors label").text("Please select Producer & Field option");
    }

}


function func_PublishFsr() {

    let Field_id = $("#ProducerFields").val();
    $.ajax({
        url: '/Admin/IsPublishFsr',
        type: 'GET',
        data: { fieldId: Field_id },
        success: function (data) {
            if (data.success == true) {
                alert("FSR publish to web successfully.");
            }
        },
        error: function () {
        }
    });
}

function func_IsValueChangeFinancialDetailsEdit(element) {
    //console.log(element);
    // (- zone_id-year-zyd_id) these type of perametr get
    var isConfirmed = confirm("By clicking confirm, all of the option data will be populated with financial data based on the existing field or farm data if it exists, otherwise it will be populated with industry standard financial data.");
    if (isConfirmed) {
        // Set the value and submit the form
        $(`#isValueChangeFinancialEdit${element}`).val("true");
        $(`#form${element}`).find('input[type="submit"]').click();
    } 

}
function func_SaveAllFinancialDataBtn() {
    var formIds = $("form[action='/Common/SaveFinancialData']").map(function () {
        return this.id;
    }).get();
    var formDataArray=[];
    for (var i = 0; i < formIds.length; i++) {
        formValues = $(`#${formIds[i]}`).serializeArray();
        formDataArray.push(formValues);
    }
    console.log(formDataArray);
    console.log(JSON.stringify(formDataArray));
    $.ajax({
        url: '/Common/SaveAllFinancialData',
        type: 'POST',
        data: { formsStringObject : JSON.stringify(formDataArray) },
        success: function (data) {
            if (data.success == true) {
                alert(data.message)
                window.location.reload();
            } else {
                alert(data.message)
            } 
        },
        error: function () {
        }
    });
}
