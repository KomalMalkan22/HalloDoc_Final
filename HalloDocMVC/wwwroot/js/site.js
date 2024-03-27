/* Tab Color */
$(document).ready(function () {
    $(".t-tab > .position-relative >  .btn > .rounded").click(function () {
        $(".t-tab > .position-relative >  .btn > .rounded").removeClass("activeTab");
        $(this).addClass("activeTab");
    });    
    $(".t-filter > .btn").click(function () {
        $(".t-filter > .btn").removeClass("activeFilter");
        $(this).addClass("activeFilter");
    });
});

/* Phone Input */

const phoneInputField = document.querySelector("#phone");
const phoneInput = window.intlTelInput(phoneInputField, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});

const phoneInputField1 = document.querySelector("#phone1");
const phoneInput1 = window.intlTelInput(phoneInputField1, {
    utilsScript:
        "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/17.0.8/js/utils.js",
});

/* File Name */
function DisplayFileName() {
    const fileInput = document.getElementById('files');
    if (fileInput.files.length > 0) {
        document.getElementById('fileName').innerHTML = fileInput.files[0].name;
    }
}

function LoadPhysician(formType) {
    var region = $("#" + formType + " .region").val();
    $.ajax({
        type: "POST",
        url: '/Actions/ProviderbyRegion?RegionId=' + region,
        cache: false,
        success: function (response) {
            var s = '';
            for (var i = 0; i < response.length; i++) {
                s += '<option value="' + response[i].physicianid + '">' + response[i].firstname + ' ' + response[i].lastname + '</option>';
            }
            $("#" + formType + " .provider").html(s);
        },
        error: function () {
            alert("Error while checking Physician.");
        }
    });
}

window.onload = function (e) {
    toggleFormElementsCloseCase('true')
    toggleFormElementsEncounter('true')
    toggleFormElementsViewCase('true')
    setupNoteButtons();
}

/* Close Case */
function toggleFormElementsCloseCase(bDisabled) {
    var inputs = document.getElementsByClassName("input");
    var submit = document.getElementById("save");
    var editprofile = document.getElementById("edit");
    for (var i = 0; i < inputs.length; i++) {
        inputs[i].disabled = !inputs[i].disabled;
    }
    if (inputs[0].disabled) {
        submit.style.display = "none";
        editprofile.style.display = "block";
    } else {
        submit.style.display = "block";
        editprofile.style.display = "none";
    }
    document.getElementById("FirstName").disabled = (1 == 1);
    document.getElementById("LastName").disabled = (1 == 1);
    document.getElementById("DateOfBirth").disabled = (1 == 1);
}         

/* Encounter */
function toggleFormElementsEncounter(bDisabled) {
    var inputs = document.getElementsByClassName("input");
    for (var i = 0; i < inputs.length; i++) {
        inputs[i].disabled = !inputs[i].disabled;
    }
}

/* Send Order */
function GetProfession() {
    var id = $("#ProfessionType").val();
    $.ajax({
        type: "POST",
        url: '/Actions/ProfessionByType?HealthProfessionId=' + id,
        cache: false,
        success: function (response) {
            console.log(response);
            var s = '<option value="-1">Please Select</option>';
            for (var i = 0; i < response.length; i++) {
                s += '<option value="' + response[i].vendorId + '">' + response[i].vendorName + '</option>';
            }
            $("#Business").html(s);
        },
        error: function () {
            alert("Error while checking Profession Types.");
        }
    });
}

function GetData() {
    var id = $("#Business").val();
    $.ajax({
        type: "POST",
        url: '/Actions/SelectProfessionalById?VendorId=' + id,
        cache: false,
        success: function (response) {
            console.log(response);
            $("#Email").val(response.email);
            $("#FaxNumber").val(response.faxnumber);
            $("#BusinessContact").val(response.businesscontact);
        },
        error: function () {
            alert("Error while checking for data.");
        }
    });
}

/* View Case */
function toggleFormElementsViewCase(bDisabled) {
    var inputs = document.getElementsByClassName("input");
    var submit = document.getElementById("save");
    var editprofile = document.getElementById("edit");
    for (var i = 0; i < inputs.length; i++) {
        inputs[i].disabled = !inputs[i].disabled;
    }
    if (inputs[0].disabled) {
        submit.style.display = "none";
        editprofile.style.display = "block";
    } else {
        submit.style.display = "block";
        editprofile.style.display = "none";
    }
}

function model_ViewCase(id) {
    document.getElementById('PatientName_ModelCancelCase').innerText = modelViewCase.firstName + " " + modelViewCase.lastName;
    document.getElementById('RequestID_Input_ModelCancelCase').value = id;
    document.getElementById('RequestID_Input_ModelAssignCase').value = id;
}

/* View Notes */
function setupNoteButtons() {
    $("#PhysicianNotes").click(function () {
        var text = $('#PhysicianTxt').html().trim();
        $("#ChangeNotes").val(text);
        $("#ChangeNotes").attr("name", "physiciannotes");
        $("#DataNote").html("Physician Note");
    });

    $("#AdminNotes").click(function () {
        var text = $('#AdminTxt').html().trim();
        console.log(text);
        $("#ChangeNotes").val(text);
        $("#ChangeNotes").attr("name", "adminnotes");
        $("#DataNote").html("Admin Note");
    });
}

/* Dashboard */
function SearchSubmit() {
    $('#filterForm').submit();
}

/* New */
function model_New(id) {
    document.getElementById('PatientName_ModelCancelCase').innerText = modeldata.list[id].patientName;
    document.getElementById('RequestID_Input_ModelCancelCase').value = modeldata.list[id].requestId;
    document.getElementById('RequestID_Input_ModelAssignCase').value = modeldata.list[id].requestId;
    document.getElementById('RequestID_Input_ModelBlockCase').value = modeldata.list[id].requestId;
    document.getElementById('PatientName_ModelBlockCase').innerText = modeldata.list[id].patientName;
}

/* Pending */
function model_Pending(id) {
    document.getElementById('RequestID_Input_ModalTransferPhysician').value = modeldata.list[id].requestId;
    document.getElementById('RequestID_Input_ModalClearCase').value = modeldata.list[id].requestId;
    document.getElementById("RequestId_SendAgreement").value = modeldata.list[id].requestId;
    document.getElementById("PhoneNumber_SendAgreement").value = modeldata.list[id].patientPhoneNumber;
    document.getElementById("Email_SendAgreement").value = modeldata.list[id].email;
}


/* ToClose */
function model_ToClose(id) {
    document.getElementById('RequestID_Input_ModalClearCase').value = modeldata.list[id].requestId;
}

/* Pagination */
function setpagenumber(e) {
    if (e == 'previous') {
        $('#currentpagevalue').val(modeldata.currentPage - 1);
    }
    else if (e == 'next') {
        $('#currentpagevalue').val(modeldata.currentPage + 1);
    }
    else {
        var current = e;
        $('#currentpagevalue').val(e);
    }
    $("#paginationform").submit();
}

/* Login */
function CheckLoginValidation() {
    var password = $("#Password").val();
    var email = $("#Email").val();

    if (password != null && email != null) {
        if (ValidateEmail(email) == true) {
            $('#LoginForm').submit();
        }
        else {
            $('.emailerror').show();
        }
    }
    else {
        $('.emailerror').show();
    }
}
function ValidateEmail(email) {
    var allowed = /^([a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$)/;
    return allowed.test(email);
}

function passtoggle(field) {
    var x = document.getElementById(field);
    if (x.type == "password") {
        x.type = "text";
        document.querySelector("#" + field + " i.fa.fa-eye").style.display = "block";
        document.querySelector("#" + field + " i.fa.fa-eye-slash").style.display = "none";
    }
    else {
        x.type = "password";
        document.querySelector("#" + field + " i.fa.fa-eye").style.display = "none";
        document.querySelector("#" + field + " i.fa.fa-eye-slash").style.display = "block";
    }
}
