$('#btnSendMessageToAdvisor').click(function () {
    PopUpAddNewAdmin();
    $("#btnSubmitAddUserForm").prop("onclick", null).off("click");
    $("#btnSubmitAddUserForm").click(function () {
        onAddNewUser();
    })
});

function PopUpAddNewAdmin() {
    $.ajax({
        url: '/Producer/GetAdvisorMessageForm/',
        type: 'GET',
        cache: false,
        dataType: 'HTML',
        success: function (form) {

            if (form != null) {
                var modal = document.getElementById("CommonModal");
                modal.style.display = "block";
                //$('#AddUserCommonModal').find('#modalTitle').html("Add New Admin");
                $('#CommonModal').find('.modalBody').html(null);
                $('#CommonModal').find('.modalBody').html(form);
                $('#btnSubmitAddUserForm').show();
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
            alert('Error - ' + thrownError);
        }
    });
}

function IsAddNewUserFormValid() {
    let IsValid = true;

    if ($('#Subject').val().trim() == "") {
        IsValid = false;
    }
    if ($('#Body').val().trim() == "") {
        IsValid = false;
    }
    return IsValid;
}

function onAddNewUser() {
    if (IsAddNewUserFormValid()) {
        $(".preloader").fadeIn();
        var formData = new FormData();

        formData.append("Subject", $('#Subject').val());
        formData.append("Body", $('#Body').val().trim());

        $.ajax({
            url: '/Producer/SendEmailToAdvisor/',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            dataType: 'JSON',
            success: function (data) {
                $(".preloader").fadeOut();
                if (data.success) {
                    console.log("Email Sent Successfully.");
                    location.reload();
                } else {
                    console.log("An error has occured while sending an email");
                    location.reload();

                }
            },
            error: function (xhr, ajaxOptions, thrownError) {
                $(".preloader").fadeOut();
                var modal = document.getElementById("CommonModal");
                modal.style.display = "none"
                console.log('Error - ' + thrownError);
                location.reload();
            }
        });
    }
    else {
        $('.validator').change(function () {
            let val = $(this).val().trim();
            if (val == '') {
                $(this).addClass('is-invalid');
                if ($(this).next(".invalid-feedback").length == 0) {
                    $(this).after("<div class='invalid-feedback' >This Field is Required.</div>");
                }
            }
            else {
                $(this).removeClass('is-invalid');
                $(this).next(".invalid-feedback").remove();
            }
        });
        $('.validator').trigger('change');
    }
}