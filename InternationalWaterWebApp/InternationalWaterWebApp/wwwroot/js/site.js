var copiedData = {};
var financialplansdetailsData = [];
var DataQualityBGrade = false;
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
    //$('[data-toggle="tooltip"]').tooltip({
    //    trigger: 'manual'
    //}).on('mouseenter', function () {
    //    var self = this;
    //    $(this).tooltip('show');
    //    $('.tooltip').on('mouseleave', function () {
    //        $(self).tooltip('hide');
    //    });
    //});

    //NOTE: Make current tab active
    const navLinks = $(".navbar-nav .nav-link");

    // Get the current URL
    const currentUrl = window.location.pathname;

    // Find the link that matches the current URL and activate it
    navLinks.each(function () {
        const linkUrl = $(this).attr("href");
        if (linkUrl === currentUrl) {
            $(this).addClass("active");
        }
    });

    if (window.location.pathname == '/Common/FinancialDetailsEdit') {
        $('input[name="Acres"]').blur();

        $(".mask").inputmask({
            alias: "currency",
            prefix: "",
            autoGroup: true,
            digits: 2,
            radixPoint: ".",
            rightAlign: false,
            unmaskAsNumber: true
        });
    }
    if (window.location.pathname == '/Common/AdvisorAddEdit') {
        $('#PhoneMobile').inputmask('(999)-999-9999');

    }
    if (window.location.pathname == '/Common/ProducersAddEdit') {
        $('#PhoneNumber').inputmask('(999)-999-9999');
    }
});

$("#nav_field").removeClass("active");
$("#pills-MyFields-tab").click(function () {
    $("#nav_field").addClass("active");
    $("#nav_producer").removeClass("active");

});
$("#pills-Producers-tab").click(function () {
    $("#nav_field").removeClass("active");
    $("#nav_producer").addClass("active");

});


////Start Redicet to testing page code (only Develpment)
if (window.location.pathname == '/Admin/Landing') {
    //$("input[name=Email]").val("admin@gmail.com");
    //$("input[name=Password]").val("test123");
    ////$('#aweberform').submit();
    //if ($("button").click()) {
    //location.replace("https://localhost:7234/Common/MyFieldsDetails?id=MTcz&name=TestTT");
    //location.replace('https://localhost:7234/Common/FinancialDetailsEdit?id=MTI1&name=1%20Wiese-T129R43S3-6315-522-1');
    //location.replace('https://localhost:7234/Common/FinancialDetailsEdit?id=MTE2&name=10%20Wiese-T129NR43WS12-5897');
};

$(document).ready(function () {
    $(".user").click(function () {
        var name = "John Doe"; // Replace with your actual user name

        // Update the modal content
        $("#userName").text(name);
        $("#roleDropdown").val(""); // Clear the dropdown selection

        // Show the modal
        $("#userModal").modal("show");
    });

    $("#saveButton").click(function () {
        var selectedRole = $("#roleDropdown").val();

        $("#userModal").modal("hide");
    });
});

$(document).ready(function () {
    if (window.location.pathname == '/Admin/Landing') {
        //admin landing farms
        $("#producer-table").DataTable({
            "pageLength": 50,
            "bLengthChange": false,

        });

        //admin landing fields
        $('#field-table').DataTable({
            "pageLength": 50,
            "bLengthChange": false,
            "order": [[1, "asc"]]
        });

    }
    if (window.location.pathname == '/Admin/MyFields') {
        //AdminMyFields
        $("#AdminMyFields").DataTable({
            "aLengthMenu": [10, 20, 50, 100],
            "pageLength": 50,
        });
    }
    if (window.location.pathname == '/Advisor/Landing') {
        //AdvisorLandingProducer
        $("#AdvisorLandingProducer").DataTable({
            "pageLength": 50,
            "bLengthChange": false
        });
        //AdvisorLandingProducer
        $("#AdvisorLandingFields").DataTable({
            "pageLength": 50,
            "bLengthChange": false
        });
        $('#btn_table_opt').appendTo('#AdvisorLandingProducer_filter');
    }

    if (window.location.pathname == '/Producer/Landing') {
        // ProducerLandingFarm
        $("#ProducerLandingFarm").DataTable({
            "aLengthMenu": [10, 20, 50, 100],
            "pageLength": 20,
            "searching": false,
            "bLengthChange": false,
        });
        //ProducerLandingField
        $("#ProducerLandingField").DataTable({
            "aLengthMenu": [10, 20, 50, 100],
            "pageLength": 20,
        });
    }
    if (window.location.pathname == '/Producer/MyFields') {
        //producerMyfields
        $("#ProducerMyfields").DataTable({
            "aLengthMenu": [10, 20, 50, 100],
            "pageLength": 20,
            "searching": false,
            "bLengthChange": false,
            "bInfo": false,
            "bPaginate": false,
        });
    }
    if (window.location.pathname == '/Producer/MyFarms') {
        //producerMyfields
        $("#ProducerLandingFarm").DataTable({
            "aLengthMenu": [10, 20, 50, 100],
            "pageLength": 20,
            "searching": false,
            "bLengthChange": false,
            "bInfo": false,
            "bPaginate": false,
        });
    }

    if (window.location.pathname == '/Common/Advisors') {
        //CommonAdvisor
        $("#CommonAdvisor").DataTable({
            "aLengthMenu": [10, 20, 50, 100],
            "pageLength": 20,
        });
        $('#btn_table_opt').appendTo('#CommonAdvisor_filter');
    }
    if (window.location.pathname == '/Common/Producers') {
        //CommonProducer
        $("#CommonProducer").DataTable({
            "aLengthMenu": [10, 20, 50, 100],
            "pageLength": 20,
        });
        $('#btn_table_opt').appendTo('#CommonProducer_filter');
    }
});

if (window.location.pathname == '/Common/MyFieldsDetails') {

    const tabList = document.getElementById('pills-tab');
    const activeTab = tabList.querySelector('.active');
    activeTabName = activeTab.getAttribute('aria-controls');

    if (activeTabName === 'pills-MyFields') {
        $(".MyFieldsenvironmentalopt").show();
    }
    else {
        $(".MyFieldsenvironmentalopt").hide();
    }
    function benchmarkOnOffSwitchMyFieldsDetails() {
        //environment outcomes switch
        if ($("#BenchmarkEnvironmentSwitch").prop('checked')) {
           
            $(".EnvironmentalOutcomesbanchmark").show();
            
        }
        else {
            $(".EnvironmentalOutcomesbanchmark").hide();
        }

        // financial switch
        if ($("#BenchmarkFinancialSwitch").prop('checked')) {
            $(".FinancialBenchColumn").show();
        }
        else {
            $(".FinancialBenchColumn").hide();
        }
    };

    //My fields details page Environment table benchmark Switch
    $("#BenchmarkEnvironmentSwitch").prop('checked', false);
    $("#BenchmarkEnvironmentSwitch").on('change', function () {
        benchmarkOnOffSwitchMyFieldsDetails();
    });

    //My fields details page financial table benchmark Switch
    $("#BenchmarkFinancialSwitch").prop('checked', false);
    $("#BenchmarkFinancialSwitch").on('change', function () {
        benchmarkOnOffSwitchMyFieldsDetails();
    });
    benchmarkOnOffSwitchMyFieldsDetails();
}

$(document).on('click', '.modal-close', function (e) {
    var modal = this.parentNode.parentNode.parentNode;
    modal.style.display = "none";
});

function openTabStrip(evt) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    $('#' + evt.currentTarget.getAttribute('data-id')).show();
    evt.currentTarget.className += " active";
}

// This is for advisor inside tabs
function openTabStrip_new(evt) {
    var i, tabcontent, tablinks;
    tabcontent = document.getElementsByClassName("tabcontent_new");
    for (i = 0; i < tabcontent.length; i++) {
        tabcontent[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tablinks_new");
    for (i = 0; i < tablinks.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" active", "");
    }
    $('#' + evt.currentTarget.getAttribute('data-id')).show();
    evt.currentTarget.className += " active";
}


if (window.location.pathname == '/Admin/CalculateFSRData') {
    $(".mask").inputmask({
        alias: "currency",
        prefix: "",
        autoGroup: true,
        digits: 2,
        radixPoint: ".",
        rightAlign: false,
        unmaskAsNumber: true
    });
}

const oldValues = {};

// Function to set the old value for a textbox


//NOTE: Calculattion of totals in financial detail edit page
function sendInputValue(button) {
    var id = button.id;
    var parts = id.split('-');
    var formId = parts[1] + '-' + parts[2] + '-' + parts[3] ;
    var form = $("#form-" + formId);
    $($(this)[0]).find("#DataQualityGrade-" + formId).val('A');

    $(`#form-${formId}`).on("keydown", function (event) {
        // Check if the pressed key is "Enter"
        if (event.key === "Enter") {
            event.preventDefault();
        }
    });

    $(`#form-${formId}`).on("input", function (event) {
        // oldvalue = event.target.defaultValue;
        if (DataQualityBGrade === true) {
            var copiedData = [getcopiedData(button)];
            const inputString = event.target.id;
            const regex = /^[^-]+/;
            const match = inputString.match(regex);
            const splitstring = match[0];
            var financialplansValue= financialplansdetailsData.map(function (item) {
                return item[splitstring];
            });
            var CurrentValue = copiedData.map(function (item) {
                return item[splitstring];
            });
            if (financialplansValue[0] != CurrentValue[0]) {
                $($(this)[0]).find("#DataQualityGrade-" + formId).val('B');
            }
            else {
                if (financialplansValue[0] === CurrentValue[0]) {
                    $($(this)[0]).find("#DataQualityGrade-" + formId).val('C');
                }
                else {
                    $($(this)[0]).find("#DataQualityGrade-" + formId).val('A');
                }
            }
        }
        else {
            $($(this)[0]).find("#DataQualityGrade-" + formId).val('A');
        }
    });

    /*var Acres = parseFloat($(form.find('input[name="Acres"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Acres"]')[0]).val().replace(/[^\d.]/g, '')) : 0;*/
    var ExpectedYield = parseFloat($(form.find('input[name="ExpectedYield"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="ExpectedYield"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    var OperatorShare = parseFloat($(form.find('input[name="OperatorShare"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OperatorShare"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    var CommoditySalePrice = parseFloat($(form.find('input[name="CommoditySalePrice"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="CommoditySalePrice"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    var OtherProductReturn = parseFloat($(form.find('input[name="OtherProductReturn"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OtherProductReturn"]')[0]).val().replace(/[^\d.]/g, '')) : 0;


    ////TotalProductReturn
    var totalAcres = (ExpectedYield * OperatorShare / 100 * CommoditySalePrice);
    var total1 = totalAcres + OtherProductReturn;
    // var TotalProductRevenue = (ExpectedYield * OperatorShare) * CommoditySalePrice + OtherProductReturn;
    //a = new Intl.NumberFormat('en-US').format(total1.toFixed(2));
    //$(form.find('span[id="TotalProductReturn"]')[0]).text(new Intl.NumberFormat('en-US').format(TotalProductRevenue.toFixed(2)));
    //$(form.find('input[id="TotalProductReturnInput"]')[0]).val(TotalProductRevenue.toFixed(2));



    $(form.find('span[id="TotalProductReturn"]')[0]).text(parseFloat(total1.toFixed(2)).toLocaleString('en-US', { minimumFractionDigits: 2 }));
    $(form.find('input[id="TotalProductReturnInput"]')[0]).val(parseFloat(total1.toFixed(2)).toLocaleString('en-US', { minimumFractionDigits: 2 }));



    // Gross Reevenue
    /*const HedgeGainLoss = parseFloat($(form.find('input[name="HedgeGainLoss"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="HedgeGainLoss"]')[0]).val().replace(/[^\d.]/g, '')) : 0;*/

    const HedgeGainLossInput = $(form.find('input[name="HedgeGainLoss"]')[0]);
    const hedgeGainLossValue = HedgeGainLossInput.val().replace(/[^\d.-]/g, ''); // Allow decimal point and negative sign

    const HedgeGainLoss = parseFloat(hedgeGainLossValue) ? parseFloat(hedgeGainLossValue) : 0;

    const CropInsurancePayments = parseFloat($(form.find('input[name="CropInsurancePayments"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="CropInsurancePayments"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const StewardshipPayments = parseFloat($(form.find('input[name="StewardshipPayments"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="StewardshipPayments"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const GovernmentPayments = parseFloat($(form.find('input[name="GovernmentPayments"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="GovernmentPayments"]')[0]).val().replace(/[^\d.]/g, '')) : 0;


    const totalTotalGrossRevenue = HedgeGainLoss + CropInsurancePayments + StewardshipPayments + GovernmentPayments;
    const totalTotalGrossRevenueCkeck = HedgeGainLoss + CropInsurancePayments + StewardshipPayments + GovernmentPayments;
    const totalGrossRevenueInput = $(form.find('input[name="TotalGrossRevenue"]')[0]);
    const totalGrossRevenueSpan = $(form.find('span[class="TotalGrossRevenue"]'));
    const OtherRevenueValueCheck = totalGrossRevenueInput.val();
    if (totalTotalGrossRevenueCkeck > 0) {
        totalGrossRevenueInput.prop('readonly', true).val(totalTotalGrossRevenue.toFixed(2));
        totalGrossRevenueSpan.text("$ " + (totalTotalGrossRevenue + total1).toLocaleString('en-US', { minimumFractionDigits: 2 }));

    }else {
        if ((OtherRevenueValueCheck == NaN || OtherRevenueValueCheck == undefined || OtherRevenueValueCheck == "" || OtherRevenueValueCheck == 0) && totalTotalGrossRevenue == 0) {
            totalGrossRevenueInput.val(0);
        }
        totalGrossRevenueInput.prop('readonly', false);
        const totalGrossRevenueInputVal = parseFloat(totalGrossRevenueInput.val().replace(/[^\d.]/g, ''));
        totalGrossRevenueSpan.text("$ " + (totalGrossRevenueInputVal + total1).toLocaleString('en-US', { minimumFractionDigits: 2 }));
    }



    // TotalFertilizerExpenses
    const Seed = parseFloat($(form.find('input[name="Seed"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Seed"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Nitrogen = parseFloat($(form.find('input[name="Nitrogen"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Nitrogen"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Phosphorus = parseFloat($(form.find('input[name="Phosphorus"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Phosphorus"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Potash = parseFloat($(form.find('input[name="Potash"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Potash"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Sulfur = parseFloat($(form.find('input[name="Sulfur"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Sulfur"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Lime = parseFloat($(form.find('input[name="Lime"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Lime"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const SeedOther = parseFloat($(form.find('input[name="SeedOther"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="SeedOther"]')[0]).val().replace(/[^\d.]/g, '')) : 0;

    const totalFertilizerExpenses = Nitrogen + Phosphorus + Potash + Sulfur + Lime + SeedOther;
    $(form.find('span[id="TotalFertilizerExpenses"]')[0]).text(totalFertilizerExpenses.toLocaleString('en-US', { minimumFractionDigits: 2 }));
    $(form.find('input[name="TotalFertilizerExpenses"]')[0]).text(totalFertilizerExpenses.toLocaleString('en-US', { minimumFractionDigits: 2 }));
    $(form.find('span[id="TotalSeedrExpenses"]')[0]).text(Seed.toLocaleString('en-US', { minimumFractionDigits: 2 }));



    // TotalCropChemicalExpenses
    const Herbicide = parseFloat($(form.find('input[name="Herbicide"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Herbicide"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Fungicide = parseFloat($(form.find('input[name="Fungicide"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Fungicide"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Insecticide = parseFloat($(form.find('input[name="Insecticide"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Insecticide"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const CropChemicalOther = parseFloat($(form.find('input[name="CropChemicalOther"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="CropChemicalOther"]')[0]).val().replace(/[^\d.]/g, '')) : 0;

    const totalTotalCropChemicalExpenses = Herbicide + Fungicide + Insecticide + CropChemicalOther;
    $(form.find('span[id="TotalCropChemicalExpenses"]')[0]).text(totalTotalCropChemicalExpenses.toLocaleString('en-US', { minimumFractionDigits: 2 }));
    $(form.find('input[name="TotalCropChemicalExpenses"]')[0]).val (totalTotalCropChemicalExpenses.toLocaleString('en-US', { minimumFractionDigits: 2 }));



    // TotalRepairMaintenance
    const Miscellaneous = parseFloat($(form.find('input[name="Miscellaneous"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Miscellaneous"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const CoverCrop = parseFloat($(form.find('input[name="CoverCrop"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="CoverCrop"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Insurance = parseFloat($(form.find('input[name="Insurance"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Insurance"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const DryingPropane = parseFloat($(form.find('input[name="DryingPropane"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="DryingPropane"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const EquipmentFuel = parseFloat($(form.find('input[name="EquipmentFuel"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="EquipmentFuel"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Machinery = parseFloat($(form.find('input[name="Machinery"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Machinery"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Buildings = parseFloat($(form.find('input[name="Buildings"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Buildings"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const RepairMaintenanceOther = parseFloat($(form.find('input[name="RepairMaintenanceOther"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="RepairMaintenanceOther"]')[0]).val().replace(/[^\d.]/g, '')) : 0;


    // const totalTotalRepairMaintenance = Miscellaneous + CoverCrop + Insurance + DryingPropane + EquipmentFuel + Machinery + Buildings + RepairMaintenanceOther;
    const totalTotalRepairMaintenance = Machinery + Buildings + RepairMaintenanceOther;
    const OtherCropExpenses = Miscellaneous + CoverCrop + Insurance;
    const TotalEnergyExpenses = DryingPropane + EquipmentFuel;
    $(form.find('span[id="TotalRepairMaintenance"]')[0]).text(totalTotalRepairMaintenance.toLocaleString('en-US', { minimumFractionDigits: 2 }));
    $(form.find('input[name="TotalRepairMaintenance"]')[0]).val(totalTotalRepairMaintenance.toLocaleString('en-US', { minimumFractionDigits: 2 }));
    $(form.find('span[id="OtherCropExpenses"]')[0]).text(OtherCropExpenses.toLocaleString('en-US', { minimumFractionDigits: 2 }));
    $(form.find('span[id="TotalEnergyExpenses"]')[0]).text(TotalEnergyExpenses.toLocaleString('en-US', { minimumFractionDigits: 2 }));


    // TotalCustomHire

    const Driver = parseFloat($(form.find('input[name="Driver"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Driver"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Equipment = parseFloat($(form.find('input[name="Equipment"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Equipment"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const CustomApplicationt = parseFloat($(form.find('input[name="CustomApplicationt"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="CustomApplicationt"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const CustomHireOther = parseFloat($(form.find('input[name="CustomHireOther"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="CustomHireOther"]')[0]).val().replace(/[^\d.]/g, '')) : 0;

    const totalTotalCustomHire = Driver + Equipment + CustomApplicationt + CustomHireOther;
    $(form.find('span[id="TotalCustomHire"]')[0]).text(totalTotalCustomHire.toLocaleString('en-US', { minimumFractionDigits: 2 }));
    $(form.find('input[name="TotalCustomHire"]')[0]).val(totalTotalCustomHire.toLocaleString('en-US', { minimumFractionDigits: 2 }));


    const HiredLabor = $(form.find('input[name="HiredLabor"]')[0]).val() ? parseFloat($(form.find('input[name="HiredLabor"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Repairs = $(form.find('input[name="Repairs"]')[0]).val() ? parseFloat($(form.find('input[name="Repairs"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const FuelElectricity = $(form.find('input[name="FuelElectricity"]')[0]).val() ? parseFloat($(form.find('input[name="FuelElectricity"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const LeasesMachinery = $(form.find('input[name="LeasesMachinery"]')[0]).val() ? parseFloat($(form.find('input[name="LeasesMachinery"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const LeasesBuildings = $(form.find('input[name="LeasesBuildings"]')[0]).val() ? parseFloat($(form.find('input[name="LeasesBuildings"]')[0]).val().replace(/[^\d.]/g, '')) : 0;

    const TotalHiredLabor = HiredLabor + Repairs + FuelElectricity;
    const TotalLeases = LeasesMachinery + LeasesBuildings;
    $(form.find('span[id="TotalHiredLabor"]')[0]).text(TotalHiredLabor.toLocaleString('en-US', { minimumFractionDigits: 2 }));
    $(form.find('span[id="TotalLeases"]')[0]).text(TotalLeases.toLocaleString('en-US', { minimumFractionDigits: 2 }));

    const LandRent = parseFloat($(form.find('input[name="LandRent"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="LandRent"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const StewardshipImplementation = parseFloat($(form.find('input[name="StewardshipImplementation"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="StewardshipImplementation"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Storage = parseFloat($(form.find('input[name="Storage"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Storage"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Supplies = parseFloat($(form.find('input[name="Supplies"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Supplies"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Utilities = parseFloat($(form.find('input[name="Utilities"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Utilities"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const FreightTrucking = parseFloat($(form.find('input[name="FreightTrucking"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="FreightTrucking"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const Marketing = parseFloat($(form.find('input[name="Marketing"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="Marketing"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const InterestOperating = parseFloat($(form.find('input[name="InterestOperating"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="InterestOperating"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const OtherCosts = parseFloat($(form.find('input[name="OtherCosts"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OtherCosts"]')[0]).val().replace(/[^\d.]/g, '')) : 0;


    const totalTotalDirectotherExpense = LandRent + StewardshipImplementation + Storage + Supplies + Utilities + FreightTrucking + Marketing + InterestOperating + OtherCosts;

    $(form.find('span[id="TotalDirectOtherExpenses"]')[0]).text(totalTotalDirectotherExpense.toLocaleString('en-US', { minimumFractionDigits: 2 }));

    const totalTotalDirectExpense = Seed + totalFertilizerExpenses + totalTotalCropChemicalExpenses + OtherCropExpenses + totalTotalRepairMaintenance + TotalEnergyExpenses + totalTotalCustomHire + totalTotalDirectotherExpense + TotalHiredLabor + TotalLeases;

    const totalReturnOverDirectExpenseperAcre = totalTotalGrossRevenue / totalTotalDirectExpense;

    if (totalTotalDirectExpense > 0) {
        $(form.find('input[name="ReturnOverDirectExpenseperAcre"]')[0]).prop('readonly', true);
        $(form.find('input[name="ReturnOverDirectExpenseperAcre"]')[0]).val(totalReturnOverDirectExpenseperAcre.toFixed(2));
        $(form.find('span[class="ReturnOverDirectExpenseperAcre"]')).text("$ " + totalReturnOverDirectExpenseperAcre.toLocaleString('en-US', { minimumFractionDigits: 2 }));
    }
    else {
        $(form.find('input[name="ReturnOverDirectExpenseperAcre"]')[0]).prop('readonly', false);
        var ReturnOverDirectExpenseperAcreInput = parseFloat($(form.find('input[name="ReturnOverDirectExpenseperAcre"]')[0]).val().replace(/[^\d.]/g, ''));
        if (ReturnOverDirectExpenseperAcreInput !== "0.00" && ReturnOverDirectExpenseperAcreInput !== "0") {
            $(form.find('span[class="ReturnOverDirectExpenseperAcre"]')).text("$ " + ReturnOverDirectExpenseperAcreInput.toLocaleString('en-US', { minimumFractionDigits: 2 }));
        } else {
            $(form.find('span[class="ReturnOverDirectExpenseperAcre"]')).text("0.00");
        }

    }
    if (totalTotalDirectExpense > 0) {
        $(form.find('input[name="TotalDirectExpense"]')[0]).prop('readonly', true);
        $(form.find('input[name="TotalDirectExpense"]')[0]).val(totalTotalDirectExpense.toFixed(2));
        $(form.find('span[class="TotalDirectExpense"]')).text("$ " + totalTotalDirectExpense.toLocaleString('en-US', { minimumFractionDigits: 2 }));
    }
    else {
        $(form.find('input[name="TotalDirectExpense"]')[0]).prop('readonly', false);
        var totalTotalDirectExpenseInput = parseFloat($(form.find('input[name="TotalDirectExpense"]')[0]).val().replace(/[^\d.]/g, ''));
        if (totalTotalDirectExpenseInput !== "0.00" && totalTotalDirectExpenseInput !== "0") {
            $(form.find('span[class="TotalDirectExpense"]')).text("$ " + totalTotalDirectExpenseInput.toLocaleString('en-US', { minimumFractionDigits: 2 }));
        } else {
            $(form.find('span[class="TotalDirectExpense"]')).text("0.00");
        }
    }

    // OverheadExpensesTotalCustomHire

    const OverheadExpensesDriver = parseFloat($(form.find('input[name="OverheadExpensesDriver"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OverheadExpensesDriver"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const OverheadExpensesEquipment = parseFloat($(form.find('input[name="OverheadExpensesEquipment"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OverheadExpensesEquipment"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const OverheadExpensesCustomApplication = parseFloat($(form.find('input[name="OverheadExpensesCustomApplication"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OverheadExpensesCustomApplication"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const OverheadExpensesOther = parseFloat($(form.find('input[name="OverheadExpensesOther"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OverheadExpensesOther"]')[0]).val().replace(/[^\d.]/g, '')) : 0;


    const totalOverheadExpensesTotalCustomHire = OverheadExpensesDriver + OverheadExpensesEquipment + OverheadExpensesCustomApplication + OverheadExpensesOther;
    $(form.find('span[id="OverheadExpensesTotalCustomHire"]')[0]).text(totalOverheadExpensesTotalCustomHire.toLocaleString('en-US', { minimumFractionDigits: 2 }));
    $(form.find('input[name="OverheadExpensesTotalCustomHire"]')[0]).text(totalOverheadExpensesTotalCustomHire.toLocaleString('en-US', { minimumFractionDigits: 2 }));


    //
    const OtherExpensesHiredLabor = parseFloat($(form.find('input[name="OtherExpensesHiredLabor"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OtherExpensesHiredLabor"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const OtherExpensesMachineryLease = parseFloat($(form.find('input[name="OtherExpensesMachineryLease"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OtherExpensesMachineryLease"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const OtherExpensesBuildingLeases = parseFloat($(form.find('input[name="OtherExpensesBuildingLeases"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OtherExpensesBuildingLeases"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const OtherExpensesFarmInsurance = parseFloat($(form.find('input[name="OtherExpensesFarmInsurance"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OtherExpensesFarmInsurance"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const OtherExpensesUtilities = parseFloat($(form.find('input[name="OtherExpensesUtilities"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OtherExpensesUtilities"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const OtherExpensesDuesProfessionalFees = parseFloat($(form.find('input[name="OtherExpensesDuesProfessionalFees"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OtherExpensesDuesProfessionalFees"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const OtherExpensesInterest = parseFloat($(form.find('input[name="OtherExpensesInterest"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OtherExpensesInterest"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const OtherExpensesMachineBuildingDepreciation = parseFloat($(form.find('input[name="OtherExpensesMachineBuildingDepreciation"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OtherExpensesMachineBuildingDepreciation"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const OtherExpensesRealEstateTaxes = parseFloat($(form.find('input[name="OtherExpensesRealEstateTaxes"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OtherExpensesRealEstateTaxes"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const OtherExpensesOtherOverheadExpenses = parseFloat($(form.find('input[name="OtherExpensesOtherOverheadExpenses"]')[0]).val().replace(/[^\d.]/g, '')) ? parseFloat($(form.find('input[name="OtherExpensesOtherOverheadExpenses"]')[0]).val().replace(/[^\d.]/g, '')) : 0;

    const totalTotalOverheadOtherExpenses = OtherExpensesHiredLabor + OtherExpensesMachineryLease + OtherExpensesBuildingLeases + OtherExpensesFarmInsurance + OtherExpensesUtilities + OtherExpensesDuesProfessionalFees + OtherExpensesInterest + OtherExpensesMachineBuildingDepreciation + OtherExpensesRealEstateTaxes + OtherExpensesOtherOverheadExpenses;

    $(form.find('span[id="OverheadExpensesTotalOtherOtherExpenses"]')[0]).text(totalTotalOverheadOtherExpenses.toLocaleString('en-US', { minimumFractionDigits: 2 }));

    const totalTotalOverheadExpenses = totalOverheadExpensesTotalCustomHire + totalTotalOverheadOtherExpenses;
    //$(form.find('input[name="TotalOverheadExpenses"]')[0]).val(totalTotalOverheadExpenses);
    //$(form.find('span[class="TotalOverheadExpenses"]')).text(totalTotalOverheadExpenses);
    if (totalTotalOverheadExpenses > 0) {
        $(form.find('input[name="TotalOverheadExpenses"]')[0]).prop('readonly', true);
        $(form.find('input[name="TotalOverheadExpenses"]')[0]).val(totalTotalOverheadExpenses.toFixed(2));
        $(form.find('span[class="TotalOverheadExpenses"]')).text("$ " + totalTotalOverheadExpenses.toLocaleString('en-US', { minimumFractionDigits: 2 }));

    }
    else {
        $(form.find('input[name="TotalOverheadExpenses"]')[0]).prop('readonly', false);
        var totalTotalOverheadExpensesInput = parseFloat($(form.find('input[name="TotalOverheadExpenses"]')[0]).val().replace(/[^\d.]/g, ''));
        if (totalTotalOverheadExpensesInput !== "0.00" && totalTotalOverheadExpensesInput !== "0") {
            $(form.find('span[class="TotalOverheadExpenses"]')).text("$ " + totalTotalOverheadExpensesInput.toLocaleString('en-US', { minimumFractionDigits: 2 }));
        } else {
            $(form.find('span[class="TotalOverheadExpenses"]')).text("0.00");
        }
    }
    //

    const FinancingIncomeTaxes = $(form.find('input[name="FinancingIncomeTaxes"]')[0]).val() ? parseFloat($(form.find('input[name="FinancingIncomeTaxes"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const FinancingOwnerWithdrawal = $(form.find('input[name="FinancingOwnerWithdrawal"]')[0]).val() ? parseFloat($(form.find('input[name="FinancingOwnerWithdrawal"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    const FinancingPrincipalPayment = $(form.find('input[name="FinancingPrincipalPayment"]')[0]).val() ? parseFloat($(form.find('input[name="FinancingPrincipalPayment"]')[0]).val().replace(/[^\d.]/g, '')) : 0;
    //console.log(FinancingIncomeTaxes, FinancingOwnerWithdrawal, FinancingPrincipalPayment)
    const TotalFinancing = FinancingIncomeTaxes + FinancingOwnerWithdrawal + FinancingPrincipalPayment;
    if (TotalFinancing > 0) {

        $(form.find('input[name="TotalFinancing"]')[0]).prop('readonly', true);
        $(form.find('input[name="TotalFinancing"]')[0]).val(TotalFinancing.toFixed(2));
        $(form.find('span[class="TotalFinancing"]')[0]).text("$ " + TotalFinancing.toLocaleString('en-US', { minimumFractionDigits: 2 }));
    }
    else {
        $(form.find('input[name="TotalFinancing"]')[0]).prop('readonly', false);
        var TotalFinancingInput = parseFloat($(form.find('input[name="TotalFinancing"]')[0]).val().replace(/[^\d.]/g, ''));
        if (TotalFinancingInput !== "0.00" && TotalFinancingInput !== "0") {
            $(form.find('span[class="TotalFinancing"]')[0]).text("$ " + TotalFinancingInput.toLocaleString('en-US', { minimumFractionDigits: 2 }));
        } else {
            $(form.find('span[class="TotalFinancing"]')[0]).text("0.00");
        }

    }
}

function getcopiedData(button) {
    var id = button.id; //$(this)[0].id; // "copyButton-573-2021"

    var parts = id.split('-');
    var formId = parts[1] + '-' + parts[2] + '-' + parts[3];

    var form = $("#" + "form-" + formId);

    var copiedData = {};
    copiedData.ExpectedYield = $($(form[0]).find('input[name="ExpectedYield"]')[0]).val();
    // copiedData.Acres = $($(form[0]).find('input[name="Acres"]')[0]).val();
    copiedData.OperatorShare = $($(form[0]).find('input[name="OperatorShare"]')[0]).val();
    copiedData.CommoditySalePrice = $($(form[0]).find('input[name="CommoditySalePrice"]')[0]).val();
    copiedData.OtherProductReturn = $($(form[0]).find('input[name="OtherProductReturn"]')[0]).val();
    copiedData.TotalProductReturn = $($(form[0]).find('span[id="TotalProductReturn"]')[0]).text();
    copiedData.HedgeGainLoss = $($(form[0]).find('input[name="HedgeGainLoss"]')[0]).val();
    copiedData.CropInsurancePayments = $($(form[0]).find('input[name="CropInsurancePayments"]')[0]).val();
    copiedData.StewardshipPayments = $($(form[0]).find('input[name="StewardshipPayments"]')[0]).val();
    copiedData.GovernmentPayments = $($(form[0]).find('input[name="GovernmentPayments"]')[0]).val();
    copiedData.TotalGrossRevenue = $($(form[0]).find('input[name="TotalGrossRevenue"]')[0]).val();
    copiedData.Seed = $($(form[0]).find('input[name="Seed"]')[0]).val();
    copiedData.Nitrogen = $($(form[0]).find('input[name="Nitrogen"]')[0]).val();
    copiedData.Phosphorus = $($(form[0]).find('input[name="Phosphorus"]')[0]).val();
    copiedData.Potash = $($(form[0]).find('input[name="Potash"]')[0]).val();
    copiedData.Sulfur = $($(form[0]).find('input[name="Sulfur"]')[0]).val();
    copiedData.Lime = $($(form[0]).find('input[name="Lime"]')[0]).val();
    copiedData.SeedOther = $($(form[0]).find('input[name="SeedOther"]')[0]).val();
    copiedData.TotalFertilizerExpenses = $($(form[0]).find('span[id="TotalFertilizerExpenses"]')[0]).text();
    copiedData.Herbicide = $($(form[0]).find('input[name="Herbicide"]')[0]).val();
    copiedData.Fungicide = $($(form[0]).find('input[name="Fungicide"]')[0]).val();
    copiedData.Insecticide = $($(form[0]).find('input[name="Insecticide"]')[0]).val();
    copiedData.CropChemicalOther = $($(form[0]).find('input[name="CropChemicalOther"]')[0]).val();
    copiedData.TotalCropChemicalExpenses = $($(form[0]).find('span[id="TotalCropChemicalExpenses"]')[0]).text();
    copiedData.Miscellaneous = $($(form[0]).find('input[name="Miscellaneous"]')[0]).val();
    copiedData.CoverCrop = $($(form[0]).find('input[name="CoverCrop"]')[0]).val();
    copiedData.Insurance = $($(form[0]).find('input[name="Insurance"]')[0]).val();
    copiedData.DryingPropane = $($(form[0]).find('input[name="DryingPropane"]')[0]).val();
    copiedData.EquipmentFuel = $($(form[0]).find('input[name="EquipmentFuel"]')[0]).val();
    copiedData.Machinery = $($(form[0]).find('input[name="Machinery"]')[0]).val();
    copiedData.Buildings = $($(form[0]).find('input[name="Buildings"]')[0]).val();
    copiedData.RepairMaintenanceOther = $($(form[0]).find('input[name="RepairMaintenanceOther"]')[0]).val();
    copiedData.TotalRepairMaintenance = $($(form[0]).find('span[id="TotalRepairMaintenance"]')[0]).text();
    copiedData.Driver = $($(form[0]).find('input[name="Driver"]')[0]).val();
    copiedData.Equipment = $($(form[0]).find('input[name="Equipment"]')[0]).val();
    copiedData.CustomApplicationt = $($(form[0]).find('input[name="CustomApplicationt"]')[0]).val();
    copiedData.CustomHireOther = $($(form[0]).find('input[name="CustomHireOther"]')[0]).val();
    copiedData.TotalCustomHire = $($(form[0]).find('span[id="TotalCustomHire"]')[0]).text();
    copiedData.HiredLabor = $($(form[0]).find('input[name="HiredLabor"]')[0]).val();
    copiedData.Repairs = $($(form[0]).find('input[name="Repairs"]')[0]).val();
    copiedData.FuelElectricity = $($(form[0]).find('input[name="FuelElectricity"]')[0]).val();
    copiedData.LeasesMachinery = $($(form[0]).find('input[name="LeasesMachinery"]')[0]).val();
    copiedData.LeasesBuildings = $($(form[0]).find('input[name="LeasesBuildings"]')[0]).val();
    copiedData.LandRent = $($(form[0]).find('input[name="LandRent"]')[0]).val();
    copiedData.StewardshipImplementation = $($(form[0]).find('input[name="StewardshipImplementation"]')[0]).val();
    copiedData.Storage = $($(form[0]).find('input[name="Storage"]')[0]).val();
    copiedData.Supplies = $($(form[0]).find('input[name="Supplies"]')[0]).val();
    copiedData.Utilities = $($(form[0]).find('input[name="Utilities"]')[0]).val();
    copiedData.FreightTrucking = $($(form[0]).find('input[name="FreightTrucking"]')[0]).val();
    copiedData.Marketing = $($(form[0]).find('input[name="Marketing"]')[0]).val();
    copiedData.InterestOperating = $($(form[0]).find('input[name="InterestOperating"]')[0]).val();
    copiedData.OtherCosts = $($(form[0]).find('input[name="OtherCosts"]')[0]).val();
    copiedData.TotalDirectExpense = $($(form[0]).find('input[name="TotalDirectExpense"]')[0]).val();
    copiedData.ReturnOverDirectExpenseperAcre = $($(form[0]).find('input[name="ReturnOverDirectExpenseperAcre"]')[0]).val();
    copiedData.OverheadExpensesDriver = $($(form[0]).find('input[name="OverheadExpensesDriver"]')[0]).val();
    copiedData.OverheadExpensesEquipment = $($(form[0]).find('input[name="OverheadExpensesEquipment"]')[0]).val();
    copiedData.OverheadExpensesCustomApplication = $($(form[0]).find('input[name="OverheadExpensesCustomApplication"]')[0]).val();
    copiedData.OverheadExpensesOther = $($(form[0]).find('input[name="OverheadExpensesOther"]')[0]).val();
    copiedData.OverheadExpensesTotalCustomHire = $($(form[0]).find('span[id="OverheadExpensesTotalCustomHire"]')[0]).text();
    copiedData.OtherExpensesHiredLabor = $($(form[0]).find('input[name="OtherExpensesHiredLabor"]')[0]).val();
    copiedData.OtherExpensesMachineryLease = $($(form[0]).find('input[name="OtherExpensesMachineryLease"]')[0]).val();
    copiedData.OtherExpensesBuildingLeases = $($(form[0]).find('input[name="OtherExpensesBuildingLeases"]')[0]).val();
    copiedData.OtherExpensesFarmInsurance = $($(form[0]).find('input[name="OtherExpensesFarmInsurance"]')[0]).val();
    copiedData.OtherExpensesUtilities = $($(form[0]).find('input[name="OtherExpensesUtilities"]')[0]).val();
    copiedData.OtherExpensesDuesProfessionalFees = $($(form[0]).find('input[name="OtherExpensesDuesProfessionalFees"]')[0]).val();
    copiedData.OtherExpensesInterest = $($(form[0]).find('input[name="OtherExpensesInterest"]')[0]).val();
    copiedData.OtherExpensesMachineBuildingDepreciation = $($(form[0]).find('input[name="OtherExpensesMachineBuildingDepreciation"]')[0]).val();
    copiedData.OtherExpensesRealEstateTaxes = $($(form[0]).find('input[name="OtherExpensesRealEstateTaxes"]')[0]).val();
    copiedData.OtherExpensesOtherOverheadExpenses = $($(form[0]).find('input[name="OtherExpensesOtherOverheadExpenses"]')[0]).val();
    copiedData.TotalOverheadExpenses = $($(form[0]).find('input[name="TotalOverheadExpenses"]')[0]).val();
    copiedData.FinancingIncomeTaxes = $($(form[0]).find('input[name="FinancingIncomeTaxes"]')[0]).val();
    copiedData.FinancingOwnerWithdrawal = $($(form[0]).find('input[name="FinancingOwnerWithdrawal"]')[0]).val();
    copiedData.FinancingPrincipalPayment = $($(form[0]).find('input[name="FinancingPrincipalPayment"]')[0]).val();
    copiedData.TotalFinancing = $($(form[0]).find('input[name="TotalFinancing"]')[0]).val();
    return copiedData;
}

function alertcopysamefieldsFunction(button) {
    var result = confirm("Are you sure you want to apply this financial data to related crops for this field?");

    // Check the user's response
    if (result) {
        copyFunction(button);
    }
}

//NOTE: Copy data from once tab to another tab(year) in financial detail edit page
function copyFunction(button) {
    var field_Id = $('#Fieldid').val();
    var field_Name = $('#Name').val();
    var id = button.id; //$(this)[0].id; // "copyButton-573-2021"

    var parts = id.split('-');
    var formId = parts[1] + '-' + parts[2] + '-' + parts[3];

    var form = $("#" + "form-" + formId);
    var copiedData = getcopiedData(button);

    var sourceCrop = $($(form[0]).find('.accordion-collapse .crop-name')[0]).text().trim(); //$($(form[0]).find('input[name="CropName"]')[0]).val();
    if (sourceCrop.length == 0) {
        alert("For 'Copy to all similar zones', Crop name is required.");
        return;
    }

    $.ajax({
        url: '/Common/Copysamefields/',
        type: 'GET',
        cache: false,
        dataType: 'json',
        data: { id: field_Id, name: field_Name, copiedData: JSON.stringify(copiedData), cropname: sourceCrop },
        success: function (response) {
            if (response.success) {
                var successMessage = response.message;
                alert(successMessage);
                window.location.reload();
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function alertcopysameProducerfieldsFunction(button) {
    var result = confirm("Are you sure you want to apply this financial data to related crops for this field \\ this producer?");

    // Check the user's response
    if (result) {
        copysameProducerfieldsFunction(button);
    }
}

function copysameProducerfieldsFunction(button) {
    var field_Id = $('#Fieldid').val();
    var field_Name = $('#Name').val();

    var copiedData = getcopiedData(button);

    var id = button.id; //$(this)[0].id; // "copyButton-573-2021"

    var parts = id.split('-');
    var formId = parts[1] + '-' + parts[2] + '-' + parts[3];

    var form = $("#" + "form-" + formId);

    var sourceCrop = $($(form[0]).find('.accordion-collapse .crop-name')[0]).text().trim(); //$($(form[0]).find('input[name="CropName"]')[0]).val();
    if (sourceCrop.length == 0) {
        alert("For 'Copy to all similar zones', Crop name is required.");
        return;
    }

    $.ajax({
        url: '/Common/CopysameProducerfields/',
        type: 'GET',
        cache: false,
        dataType: 'json',
        data: { id: field_Id, name: field_Name, copiedData: JSON.stringify(copiedData), cropname: sourceCrop },
        success: function (response) {
            if (response.success) {
                var successMessage = response.message;
                alert(successMessage);
                window.location.reload();
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}


function hideAndShowElements() {
    $(".fsrdata").show();
    $(".fsrdataAdvisor").show();
    $(".environmentalOutcomes").show();
    $(".finacialResult").show();
    $("#pills-tabContent").show();
    $("#environmentalOutcomesSection").show();
    $("#soilindiciesPopup").hide();
    $("#CurrentOperationDetailslink").hide();
    $("#FertilizerandPhosphorusModal").hide();
    $("#FieldRiskModal").hide();
    $("#CurrentfinancialDetailsEditcropnamelink").hide();
    $("#financialDetailsEditcropnamedetails").hide();
    $("#EnvironmentalOutcomesMoldboldTbl").hide();
    $("#EnvironmentalOutcomesAlflfaTbl").hide();
    $("#CurrentAdvisorOperationDetailslink").hide();
    $("#soilindiciesAdvisorPopup").hide();

    var FieldDetailsViewHistoryChartContent = document.getElementById("FieldDetailsViewHistoryChartContent");
    if (FieldDetailsViewHistoryChartContent !== null) {
        FieldDetailsViewHistoryChartContent.style.display = 'none';
    }
    var elements = document.getElementById("FieldDetailsViewHistoryEnvironmentalOutcomesChartContent");
    if (elements !== null) {
        elements.style.display = 'none';
    }
    var financialDetailsEditcropnamepopup = document.getElementById("financialDetailsEditcropnamepopup");
    if (financialDetailsEditcropnamepopup !== null) {
        financialDetailsEditcropnamepopup.style.display = 'none';
    }
    var CurrentfinancialDetailsEditcropnamelink = document.getElementById("CurrentfinancialDetailsEditcropnamelink");
    if (CurrentfinancialDetailsEditcropnamelink !== null) {
        CurrentfinancialDetailsEditcropnamelink.style.display = 'none';
    }

}

//NOTE: field_information tab button click to hide and show some content
$("#pills-MyFields-tab").click(function () {
    hideAndShowElements();
   
    $("#BenchmarkEnvironmentSwitch").prop('checked', false);
    $(".EnvironmentalOutcomesbanchmark")[0].style.setProperty('display', 'none', 'important');
    $(".MyFieldsenvironmentalopt").show();
    $("#MyFieldsenvironmentalOutcomesSection")[0].style.setProperty('display', 'none', 'important');

});
$("#pills-MyFarms-tab").click(function () {
    hideAndShowElements();
    $("#MyFieldsenvironmentalOutcomesSection").show();
    $("#BenchmarkEnvironmentSwitch").prop('checked', false);
    $(".EnvironmentalOutcomesbanchmark")[0].style.setProperty('display', 'none', 'important');
    $(".MyFieldsenvironmentalopt").hide();
  
});
$("#pills-FieldInformation-tab").click(function () {
    hideAndShowElements();
    $("#environmentalOutcomesSection")[0].style.setProperty('display', 'none', 'important');
    $(".finacialResult").hide();
    $(".MyFieldsenvironmentalopt").hide();
});

$('#MZFinanciPlansnamesubmit').click(function () {
    $('#MZFinanciPlanname').show();
    $('#MZFinanciPlanssubmit').show();
});

function checkInput() {
    const inputField = document.getElementById('MZFinanciPlanname');
    const saveButton = document.querySelectorAll('.MZFinanciPlanssubmitshowhide');

    //if (inputField.value.trim() !== '') {
    //    saveButton.style.display = 'inline-block';
    //} else {
    //    saveButton.style.display = 'none';
    //}
    for (const button of saveButton) {
        if (inputField.value.trim() !== '') {
            button.style.display = 'inline-block';
        } else {
            button.style.display = 'none';
        }
    }
}

function savefinanciplans(button) {
    var copiedData = getcopiedData(button);
    var FinanciPlan_Name = $('#MZFinanciPlanname').val();
    var field_Id = $('#Fieldid').val();
    var field_Name = $('#Name').val();
    var id = button.id; //$(this)[0].id; // "copyButton-573-2021"

    var parts = id.split('-');
    var formId = parts[1] + '-' + parts[2] + '-' + parts[3];

    var form = $("#" + "form-" + formId);

    $.ajax({
        url: '/Common/FinancialPlansSave/',
        type: 'GET',
        cache: false,
        dataType: 'json',
        data: { id: field_Id, name: field_Name, copiedData: JSON.stringify(copiedData), plansname: FinanciPlan_Name },
        success: function (response) {
            if (response.success) {
                var successMessage = response.message;
                alert(successMessage);
                window.location.reload();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });

}

function btnapplyfinancialplansdetails(button) {

    // var financialPlans_Select = $("#financialPlansSelect").val();
    var selectElement = document.getElementById("financialPlansSelect");
    var selectedValue = selectElement.value;
    $.ajax({
        url: '/Common/GetFinancialPlans/',
        type: 'GET',
        cache: false,
        dataType: 'json',
        data: { financialPlansSelect: selectedValue },
        success: function (response) {
            if (response.success) {
                DataQualityBGrade = true;

                var plansdata = JSON.parse(response.data);

                var id = button.id; //$(this)[0].id; // "copyButton-573-2021"

                var parts = id.split('-');
                var zoneYearId = '-' + parts[1] + '-' + parts[2] + '-' + parts[3] ; // "573-2021"
                var formId = parts[1] + '-' + parts[2] + '-' + parts[3];
                var form = $("#form-" + formId);
                $("#DataQualityGrade" + zoneYearId).val('C');
                $("#ExpectedYield" + zoneYearId).val(plansdata[0].expectedyield);
                $("#OperatorShare" + zoneYearId).val(plansdata[0].operatorpercent);
                $("#CommoditySalePrice" + zoneYearId).val(plansdata[0].cashprice);
                $("#OtherProductReturn" + zoneYearId).val(plansdata[0].otherreturn);
                $("#TotalProductReturn").text(plansdata[0].croprevenue);
                $("#TotalProductReturn").val(plansdata[0].croprevenue);
                $("#HedgeGainLoss" + zoneYearId).val(plansdata[0].hedge_gain_loss);
                $("#CropInsurancePayments" + zoneYearId).val(plansdata[0].cropinspayment);
                $("#StewardshipPayments" + zoneYearId).val(plansdata[0].stewardship_payments);
                $("#GovernmentPayments" + zoneYearId).val(plansdata[0].govtpayments);
                // $(".TotalGrossRevenue").text(plansdata[0].totalgrossrevenue);
                // $("input[name='TotalGrossRevenue']").val(plansdata[0].totalgrossrevenue);
                $("#Seed" + zoneYearId).val(plansdata[0].seed_direct);
                $("#Nitrogen" + zoneYearId).val(plansdata[0].nfert_direct);
                $("#Phosphorus" + zoneYearId).val(plansdata[0].pfert_direct);
                $("#Potash" + zoneYearId).val(plansdata[0].potfert_direct);
                $("#Sulfur" + zoneYearId).val(plansdata[0].sfert_direct);
                $("#Lime" + zoneYearId).val(plansdata[0].lime_direct);
                $("#SeedOther" + zoneYearId).val(plansdata[0].otherfert_direct);
                $("#TotalFertilizerExpenses").text(plansdata[0].totalfertilizerexpenses);
                $("#Herbicide" + zoneYearId).val(plansdata[0].herbicide_direct);
                $("#Fungicide" + zoneYearId).val(plansdata[0].funicide_direct);
                $("#Insecticide" + zoneYearId).val(plansdata[0].insecticide_direct);
                $("#CropChemicalOther" + zoneYearId).val(plansdata[0].otherchem_direct);
                $("#TotalCropChemicalExpenses").text(plansdata[0].totfert_direct);
                $("#Miscellaneous" + zoneYearId).val(plansdata[0].cropmisc_direct);
                $("#CoverCrop" + zoneYearId).val(plansdata[0].covercrop_direct);
                $("#Insurance" + zoneYearId).val(plansdata[0].cropinsur_direct);
                $("#DryingPropane" + zoneYearId).val(plansdata[0].drypropane_direct);
                $("#EquipmentFuel" + zoneYearId).val(plansdata[0].equipfuel_direct);
                $("#Machinery" + zoneYearId).val(plansdata[0].machrepair_direct);
                $("#Buildings" + zoneYearId).val(plansdata[0].bldgrepair_direct);
                $("#RepairMaintenanceOther" + zoneYearId).val(plansdata[0].otherrepair_direct);
                $("#TotalRepairMaintenance").text(plansdata[0].totothercrop_direct);
                $("#Driver" + zoneYearId).val(plansdata[0].customdriver_direct);
                $("#Equipment" + zoneYearId).val(plansdata[0].customequiphire_direct);
                $("#CustomApplicationt" + zoneYearId).val(plansdata[0].customapp_direct);
                $("#CustomHireOther" + zoneYearId).val(plansdata[0].othercustomhire_direct);
                $("#HiredLabor" + zoneYearId).val(plansdata[0].hiredlabor_direct);
                $("#Repairs" + zoneYearId).val(plansdata[0].irrigationrepair_direct);
                $("#FuelElectricity" + zoneYearId).val(plansdata[0].irrigpower_direct);
                $("#LeasesMachinery" + zoneYearId).val(plansdata[0].machinery_direct);
                $("#LeasesBuildings" + zoneYearId).val(plansdata[0].building_direct);
                $("#LandRent" + zoneYearId).val(plansdata[0].landrent_direct);
                $("#StewardshipImplementation" + zoneYearId).val(plansdata[0].stewardship_direct);
                $("#Storage" + zoneYearId).val(plansdata[0].storage_direct);
                $("#Supplies" + zoneYearId).val(plansdata[0].supplies_direct);
                $("#Utilities" + zoneYearId).val(plansdata[0].utilities_direct);
                $("#FreightTrucking" + zoneYearId).val(plansdata[0].freighttrucking_direct);
                $("#Marketing" + zoneYearId).val(plansdata[0].marketing);
                $("#InterestOperating" + zoneYearId).val(plansdata[0].interest_direct);
                $("#OtherCosts" + zoneYearId).val(plansdata[0].other_direct);
                $("#TotalDirectExpense" + zoneYearId).val(plansdata[0].totdirectexp);
                // $(".TotalDirectExpense").text(plansdata[0].totdirectexp);
                // $("input[name='TotalDirectExpense']").val(plansdata[0].totdirectexp);
                $("#ReturnOverDirectExpenseperAcre" + zoneYearId).val(plansdata[0].returnovrdirect);
                $("#OverheadExpensesDriver" + zoneYearId).val(plansdata[0].customdriver_oexp);
                $("#OverheadExpensesEquipment" + zoneYearId).val(plansdata[0].customequiphire_oexp);
                $("#OverheadExpensesCustomApplication" + zoneYearId).val(plansdata[0].customapp_oexp);
                $("#OverheadExpensesOther" + zoneYearId).val(plansdata[0].othercustomhire_oexp);
                $("#OverheadExpensesTotalCustomHire").text(plansdata[0].totcustomhire_direct);
                $("#OtherExpensesHiredLabor" + zoneYearId).val(plansdata[0].hiredlabor_oexp);
                $("#OtherExpensesMachineryLease" + zoneYearId).val(plansdata[0].mach_lease_oepx);
                $("#OtherExpensesBuildingLeases" + zoneYearId).val(plansdata[0].build_leas_oepx);
                $("#OtherExpensesFarmInsurance" + zoneYearId).val(plansdata[0].farminsurance_oexp);
                $("#OtherExpensesUtilities" + zoneYearId).val(plansdata[0].utility_oexp);
                $("#OtherExpensesDuesProfessionalFees" + zoneYearId).val(plansdata[0].dues_oexp);
                $("#OtherExpensesInterest" + zoneYearId).val(plansdata[0].interest_oexp);
                $("#OtherExpensesMachineBuildingDepreciation" + zoneYearId).val(plansdata[0].depreciation_oexp);
                $("#OtherExpensesRealEstateTaxes" + zoneYearId).val(plansdata[0].realesttax_oexp);
                $("#OtherExpensesOtherOverheadExpenses" + zoneYearId).val(plansdata[0].misc_oexp);
                // $(".TotalOverheadExpenses").text(plansdata[0].totoverheadexp);
                $("input[name='TotalOverheadExpenses']").val(plansdata[0].totoverheadexp);
                $("#FinancingIncomeTaxes" + zoneYearId).val(plansdata[0].inctax_finance);
                $("#FinancingOwnerWithdrawal" + zoneYearId).val(plansdata[0].ownerwithdrawl_finance);
                $("#FinancingPrincipalPayment" + zoneYearId).val(plansdata[0].princpayment_finance);
                $("#TotalFinancing" + zoneYearId).val(plansdata[0].totalfinancial);
                // $(".TotalFinancing").text(plansdata[0].totalfinancial);
                $("#TotalSeedrExpenses").text(plansdata[0].seed_direct.toLocaleString('en-US', { minimumFractionDigits: 2 }));


                const totalGrossRevenueInput = $(form.find('input[name="TotalGrossRevenue"]')[0]);
                const totalGrossRevenueSpan = $(form.find('span[class="TotalGrossRevenue"]')[0]);

                if (parseFloat(plansdata[0].totdirectexp) > 0) {
                    totalGrossRevenueInput.prop('readonly', true).val(parseFloat(plansdata[0].totdirectexp).toFixed(2));
                    totalGrossRevenueSpan.text("$ " + plansdata[0].totdirectexp.toLocaleString('en-US', { minimumFractionDigits: 2 }));

                } else {
                    totalGrossRevenueInput.prop('readonly', false);
                    const totalGrossRevenueInputVal = parseFloat(plansdata[0].totdirectexp.replace(/[^\d.]/g, ''));
                    totalGrossRevenueSpan.text(totalGrossRevenueInputVal !== "0.00" && totalGrossRevenueInputVal !== "0" ? "$ " + totalGrossRevenueInputVal.toLocaleString('en-US', { minimumFractionDigits: 2 }) : "0.00");
                }

                if (parseFloat(plansdata[0].totdirectexp) > 0) {
                    $(form.find('input[name="TotalDirectExpense"]')[0]).prop('readonly', true);
                    $(form.find('input[name="TotalDirectExpense"]')[0]).val(plansdata[0].totdirectexp.toFixed(2));
                    $(form.find('span[class="TotalDirectExpense"]')).text("$ " + plansdata[0].totdirectexp.toLocaleString('en-US', { minimumFractionDigits: 2 }));
                }
                else {
                    (form.find('input[name="TotalDirectExpense"]')[0]).prop('readonly', false);
                    var totalTotalDirectExpenseInput = parseFloat($(form.find('input[name="TotalDirectExpense"]')[0]).val().replace(/[^\d.]/g, ''));
                    if (totalTotalDirectExpenseInput !== "0.00" && totalTotalDirectExpenseInput !== "0") {
                        $(form.find('span[class="TotalDirectExpense"]')).text("$ " + totalTotalDirectExpenseInput.toLocaleString('en-US', { minimumFractionDigits: 2 }));
                    } else {
                        $(form.find('span[class="TotalDirectExpense"]')).text("$ 0.00");
                    }
                }


                if (parseFloat(plansdata[0].totoverheadexp) > 0) {
                    $(form.find('input[name="TotalOverheadExpenses"]')[0]).prop('readonly', true);
                    $(form.find('input[name="TotalOverheadExpenses"]')[0]).val(parseFloat(plansdata[0].totoverheadexp).toFixed(2));
                    $(form.find('span[class="TotalOverheadExpenses"]')).text("$ " + plansdata[0].totoverheadexp.toLocaleString('en-US', { minimumFractionDigits: 2 }));

                }
                else {
                    $(form.find('input[name="TotalOverheadExpenses"]')[0]).prop('readonly', false);
                    var totalTotalOverheadExpensesInput = parseFloat($(form.find('input[name="TotalOverheadExpenses"]')[0]).val().replace(/[^\d.]/g, ''));
                    if (totalTotalOverheadExpensesInput !== "0.00" && totalTotalOverheadExpensesInput !== "0") {
                        $(form.find('span[class="TotalOverheadExpenses"]')).text("$ " + totalTotalOverheadExpensesInput.toLocaleString('en-US', { minimumFractionDigits: 2 }));
                    } else {
                        $(form.find('span[class="TotalOverheadExpenses"]')).text("0.00");
                    }
                }

                if (parseFloat(plansdata[0].totalfinancial) > 0) {

                    $(form.find('input[name="TotalFinancing"]')[0]).prop('readonly', true);
                    $(form.find('input[name="TotalFinancing"]')[0]).val(parseFloat(plansdata[0].totalfinancial).toFixed(2));
                    $(form.find('span[class="TotalFinancing"]')[0]).text("$ " + plansdata[0].totalfinancial.toLocaleString('en-US', { minimumFractionDigits: 2 }));
                }
                else {
                    $(form.find('input[name="TotalFinancing"]')[0]).prop('readonly', false);
                    var TotalFinancingInput = parseFloat($(form.find('input[name="TotalFinancing"]')[0]).val().replace(/[^\d.]/g, ''));
                    if (TotalFinancingInput !== "0.00" && TotalFinancingInput !== "0") {
                        $(form.find('span[class="TotalFinancing"]')[0]).text("$ " + TotalFinancingInput.toLocaleString('en-US', { minimumFractionDigits: 2 }));
                    } else {
                        $(form.find('span[class="TotalFinancing"]')[0]).text("0.00");
                    }
                }

                var OtherCropExpenses = parseFloat(plansdata[0].cropmisc_direct) + parseFloat(plansdata[0].covercrop_direct) + parseFloat(plansdata[0].cropinsur_direct);
                $("#OtherCropExpenses").text(OtherCropExpenses.toLocaleString('en-US', { minimumFractionDigits: 2 }));

                var TotalEnergyExpenses = parseFloat(plansdata[0].drypropane_direct) + parseFloat(plansdata[0].equipfuel_direct);
                $("#TotalEnergyExpenses").text(TotalEnergyExpenses.toLocaleString('en-US', { minimumFractionDigits: 2 }));

                const totalTotalCustomHire = parseFloat(plansdata[0].customdriver_direct) + parseFloat(plansdata[0].customequiphire_direct) + parseFloat(plansdata[0].customapp_direct) + parseFloat(plansdata[0].othercustomhire_direct) ;
                $("#TotalCustomHire").text(totalTotalCustomHire.toLocaleString('en-US', { minimumFractionDigits: 2 }));

                const TotalHiredLabor = parseFloat(plansdata[0].hiredlabor_direct) + parseFloat(plansdata[0].irrigationrepair_direct) + parseFloat(plansdata[0].irrigpower_direct);

                const TotalLeases = parseFloat(plansdata[0].machinery_direct) + parseFloat(plansdata[0].building_direct);
                $("#TotalHiredLabor").text(TotalHiredLabor.toLocaleString('en-US', { minimumFractionDigits: 2 }));
                $("#TotalLeases").text(TotalLeases.toLocaleString('en-US', { minimumFractionDigits: 2 }));

                const totalTotalDirectotherExpense = parseFloat(plansdata[0].landrent_direct) + parseFloat(plansdata[0].stewardship_direct) + parseFloat(plansdata[0].storage_direct) + parseFloat(plansdata[0].supplies_direct) + parseFloat(plansdata[0].utilities_direct) + parseFloat(plansdata[0].freighttrucking_direct) + parseFloat(plansdata[0].marketing )+ parseFloat(plansdata[0].interest_direct )+ parseFloat(plansdata[0].other_direct);

                $("#TotalDirectOtherExpenses").text(totalTotalDirectotherExpense.toLocaleString('en-US', { minimumFractionDigits: 2 }));


                const totalTotalOverheadOtherExpenses = parseFloat(plansdata[0].hiredlabor_oexp) + parseFloat(plansdata[0].mach_lease_oepx) + parseFloat(plansdata[0].build_leas_oepx) + parseFloat(plansdata[0].farminsurance_oexp) + parseFloat(plansdata[0].utility_oexp) + parseFloat(plansdata[0].dues_oexp) + parseFloat(plansdata[0].interest_oexp) + parseFloat(plansdata[0].depreciation_oexp) + parseFloat(plansdata[0].realesttax_oexp) + parseFloat(plansdata[0].misc_oexp);

                $("#OverheadExpensesTotalOtherOtherExpenses").text(totalTotalOverheadOtherExpenses.toLocaleString('en-US', { minimumFractionDigits: 2 }));
                financialplansdetailsData = [getcopiedData(button)];
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });

};

function alertdeleteProducerFunction() {
    var result = confirm("Are you sure you want to delete this producer and all of their associated field data?");

    // Check the user's response
    if (result) {
        deleteProducer();
    }
}

function deleteProducer() {
    var producer_id = document.getElementById("ProducerId").value;
    var userId = document.getElementById("userId").value;
    $.ajax({
        url: '/Common/DeleteProducer/',
        type: 'GET',
        cache: false,
        dataType: 'json',
        data: { ProducerId: producer_id },
        success: function (response) {
            if (response.success) {
                if (userId === "1") {
                    window.location.href = "/Common/Producers";
                }
                else if (userId === "2") {
                    window.location.href = "/Advisor/Landing";
                }
                else {
                    window.location.href = "/Common/Producers";
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function alertdeleteAdvisorFunction() {
    var result = confirm("Are you sure you want to delete this Advisor? Note: Producers will remain, administrator can assign a new Advisor");

    // Check the user's response
    if (result) {
        deleteAdvisor();
    }
}

function deleteAdvisor() {
    var advisors_id = document.getElementById("AdvisorsId").value;
    var userId = document.getElementById("userId").value;
    $.ajax({
        url: '/Common/DeleteAdvisor/',
        type: 'GET',
        cache: false,
        dataType: 'json',
        data: { AdvisorsId: advisors_id },
        success: function (response) {
            if (response.success) {
                if (userId === "3") {
                    window.location.href = "/Producers/Advisors";
                }
                else {
                    window.location.href = "/Common/Advisors";
                }
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function OpenFarmPopupFunction() {
    var ProducerId = document.getElementById("ProducerId").value;
    $.ajax({
        url: '/Common/FarmPopup/',
        type: 'GET',
        cache: false,
        dataType: 'HTML',
        data: { ProducerId: ProducerId },
        success: function (formFarm) {
            if (formFarm != null) {
                var modal = document.getElementById("viewzonefarmModal");
                modal.style.display = "block";
                $('#viewzonefarmModal').find('.modalBody').html(null);
                $('#viewzonefarmModal').find('.modalBody').html(formFarm);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}

function editFarmName(farmId) {
    // Hide the original farm name and show the input for editing
    $("#farmName_" + farmId).hide();
    $("#editFarmName_" + farmId).val($("#farmName_" + farmId).text()).show();
    $("#savefarmbutton_" + farmId).show();
    $("#cancelfarmbutton_" + farmId).show();
    $("#editfarmbutton_" + farmId).hide();
}

function saveFarmName(farmId) {
    // Get the edited farm name from the input
    var editedFarmName = $("#editFarmName_" + farmId).val();

    // Update the displayed farm name and hide the input
    $("#farmName_" + farmId).text(editedFarmName).show();
   
    var farmname = $("#farmName_" + farmId).text();
    var farmNameValue = farmname.trim();
    var errorEditFarmName = document.getElementById("errorEditFarmName_" + farmId);
    if (farmNameValue === '') {
        errorEditFarmName.textContent = 'Farm Name cannot be empty';
        return false; // Prevent form submission
    } else {
        errorEditFarmName.textContent = '';
        $.ajax({
            url: '/Common/SaveFarmName/',
            type: 'GET',
            cache: false,
            dataType: 'HTML',
            data: { FarmId: farmId, farmname: farmname },
            success: function (formFarm) {
                $("#editFarmName_" + farmId).hide();
                $("#editfarmbutton_" + farmId).show();
                $("#savefarmbutton_" + farmId).hide();
                $('#cancelfarmbutton_' + farmId).hide();
                if (formFarm != null) {
                    var modal = document.getElementById("viewzonefarmModal");
                    modal.style.display = "block";
                    $('#viewzonefarmModal').find('.modalBody').html(null);
                    $('#viewzonefarmModal').find('.modalBody').html(formFarm);
                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
            }
        });
    }

   
}

function cancelEdit(farmId) {
    var editedFarmName = $("#editFarmName_" + farmId).val();
    $("#farmName_" + farmId).text(editedFarmName).show();
    var farmname = $("#farmName_" + farmId).text();
    var farmNameValue = farmname.trim();
    var errorEditFarmName = document.getElementById("errorEditFarmName_" + farmId);
    if (farmNameValue === '') {
        errorEditFarmName.textContent = 'Farm Name cannot be empty';
        return false; 
    } else {
        errorEditFarmName.textContent = '';
        $("#editFarmName_" + farmId).hide();
        $("#editfarmbutton_" + farmId).show();
        $("#savefarmbutton_" + farmId).hide();
        $('#cancelfarmbutton_' + farmId).hide();
    }

}

$("#closeviewfarm").click(function () {
    window.location.reload();
});

function alertdeleteFieldFunction() {
    var result = confirm("Are you sure you want to delete this field and associated data?");

    // Check the user's response
    if (result) {
        deleteField();
    }
}

function deleteField() {
    var Field_id = document.getElementById("Field_Id").value;
    var userId = document.getElementById("userId").value;
    $.ajax({
        url: '/Common/DeleteField/',
        type: 'GET',
        cache: false,
        dataType: 'json',
        data: { FieldId: Field_id },
        success: function (response) {
            if (response.success) {
                //var successMessage = response.message;
                //alert(successMessage);
                if (userId === "1") {
                    window.location.href = "/Admin/MyFields";
                }
                else if (userId === "2") {
                    window.location.href = "/Advisor/Landing";
                }
                else {
                    window.location.href = "/Producer/MyFields";
                }
            }

        },
        error: function (xhr, ajaxOptions, thrownError) {
        }
    });
}
