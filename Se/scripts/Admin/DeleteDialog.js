$(document).ready(function () {

    var hidden_desc_TempHTML;
    var temp_active;

    function unsetAll() {
        //$("h3").hide().show("slide", { direction: "right" }, "normal").effect({ effect: "bounce", duration: "normal" }); //UI

        console.log("cleared");

        $("#idU").val(null);

        $("#idUDisplay").animate({ "opacity": 0 }, "normal", function () {
            $(this).empty().css("opacity", 1);
        });

        $("#remove").animate({ "opacity": 0 }, "normal", function () {
            $(this).css({ "display": "none", "opacity": 1 });
        });

        $(".checkbox-inline").animate({ "opacity": 0 }, "normal", function () {
            $(this).css({ "display": "none", "opacity": 1 });
        });

        $("#listaSeminara li button").removeClass("active");
        temp_active = undefined;
        hidden_desc_TempHTML = undefined;

        //modal
        $("#listaSeminara").val(null);
        $("#hidden_desc").hide();
        $('#hidden_desc_container').css({ opacity: 0.5, "background-color": "#3d444a" });
        $("#cancel").css({ "border-radius": "6px 6px 6px 6px", width: "100%" });
        $("#ok").css({ width: 0, opacity: 0, display: "none" });
    }

    $("#check").on("invalid", function () {
        this.setCustomValidity('Potvrdite ili stisnite cancel!');
    });

    $("#check").on("input", function () {
        this.setCustomValidity('');
    });

    //ON BUTTON/CHECKBOX CLICK CHANGE CLASS
    $("#checkbox_button_wrapper").click(function () {
        var chkbx = $('#check');
        chkbx.prop("checked", !chkbx.prop("checked"));
        var checked = chkbx.is(':checked');
        if (checked) {
            $(this).switchClass("btn-default", "btn-danger", 0);
        }
        else {
            $(this).switchClass("btn-danger", "btn-default", 0);
        }
    });


    $("#transfer").click(function () {
        $('#hidden_desc').html(hidden_desc_TempHTML);
        $("#mojModal").modal({
            keyboard: false,
            backdrop: "static"
        });
    });

    //X (Remove) ICON
    $("#remove").on("click", function () {
        $("#checkbox_button_wrapper").switchClass("btn-danger", "btn-default", 0);

        $('#check').prop({ 'checked': false });
        $("#check").prop({ "disabled": true, "required": false });
        unsetAll();
    });

    //Modal OK Button
    $("#mojModal").on("click", "#ok", function () {

        hidden_desc_TempHTML = $('#hidden_desc').html();
        temp_active = $("#listaSeminara .active");

        var selected_text = temp_active.text();

        console.log("Selected Text: " + selected_text);

        if (temp_active) {
            console.log("temp_active_OK: " + temp_active);
            $("#check").prop({ "disabled": false, "required": true });
            $("#remove").show();
            $("#idUDisplay").text(selected_text);
            $(".checkbox-inline").css('display', 'inline-block');
            $('#hidden_desc_container').css({ opacity: 0.5, "background-color": "#3d444a" });
            $("#idU").val(temp_active.val());
        }

    });

    //ON LIST CLICK
    $("#listaSeminara li button").on("click", function () {

        if ($("#ok").css("opacity") === "0") {
            $("#cancel").css({ "border-radius": "6px 0 0 6px" });
            $("#ok").css({ "display": "inline-block", "opacity": 1 });
            $("#ok").animate({ width: "50%" }, {
                easing: "linear",
                queue: false,
                duration: "slow"
            });
            $("#cancel").animate({ width: "50%" }, {
                easing: "linear",
                queue: false,
                duration: "slow"
            });
        }

        var selected_val = $(this).val();
        var desc_container = $('#hidden_desc_container');
        if (selected_val) { //ovo je od kad sam koristion DropDown
            $("#listaSeminara li button").removeClass("active");
            $(this).addClass("active");

            $.post("/Seminari/SelectedSeminarInfo", { 'id': selected_val }, function (data) {
                if (desc_container.css("opacity") === "0.5") {
                    desc_container.animate({ "background-color": "#eee" }, "fast", "linear", false)
                        .animate({ "opacity": 1, "background-color": "#3d444a" }, "slow", "linear", false);
                }

                $('#hidden_desc').html(data).hide().slideDown('slow');
            });

            console.log("ID:" + selected_val);
        }
        else {
            $("#hidden_desc").empty();
        }
    });

    //Modal CANCEL Button
    $("#mojModal").on("click", "#cancel", function () {

        $("#listaSeminara li button").removeClass("active");
        $(temp_active).addClass("active");
        $('#hidden_desc_container').css({ opacity: 0.5, "background-color": "#3d444a" });

        //if (!temp_active) {
        //    console.log("temp_active_cancel: " + temp_active);
        //    $("#cancel").css({ "border-radius": "6px 6px 6px 6px", width: "100%" });
        //    $("#ok").css({ width: 0, opacity: 0, display: "none" });
        //    $("#hidden_desc").hide();
        //}
    });

    $("#mojModal").on("hidden.bs.modal", function (e) {
        if (!temp_active) {
            console.log("temp_active_cancel: " + temp_active);
            $("#cancel").css({ "border-radius": "6px 6px 6px 6px", width: "100%" });
            $("#ok").css({ width: 0, opacity: 0, display: "none" });
            $("#hidden_desc").hide();
        }
    });

});