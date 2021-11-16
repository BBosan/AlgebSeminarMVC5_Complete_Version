//+++++++++++++++++++++++++++++++++++
const urlPath = "/Seminari/autoComplete";
const searchResultID = "#searchResult";
const inputID = "#Naziv";
const controlsClass = ".controls";

    function pageData(e) {
        $.ajax({
            type: "GET",
            url: urlPath,
            data: { "naziv": $(inputID).val(), "Page": e },
            dataType: "json",
            success: function (data) {
                createList(data);
                setNextPrevBtn(data);
            }
        });
    }

    function setNextPrevBtn(data) {

        if (data.Page === 0 && data.Count > 1) {
            $("#page_number_display").html("<strong>" + "Found: " + data.Count + "</strong>");
        }

        if (data.Page > 0) {
            $(searchResultID).prepend("<li class='controls btn btn-sm'><span class='glyphicon glyphicon-arrow-left'></span></li>"); 
            //$("#page_number_display").html("<strong>" + (data.Page + 1) + ". page" + "</strong>");

            $(controlsClass).first().prop('title', data.Page + ". page");

            $(controlsClass).first().on("click", function () {
                console.log("prev");
                pageData(data.Page - 1);
            });
        }

        if (data.Page < data.MaxPage) {
            $(searchResultID).append("<li class='controls btn btn-sm'><span class='glyphicon glyphicon-arrow-right'></span></li>");

            $(controlsClass).last().prop('title', data.Page+2 + ". page");

            $(controlsClass).last().on("click", function () {
                console.log("next");
                pageData(data.Page + 1);
            });
        }

        $(searchResultID+" li").not(".controls").on("click", function () {
            $(inputID).val($(this).text());
            $(searchResultID).empty();
            $("#myform").submit();
            $("#page_number_display").empty();
        });
    }

    function suggestion() {
        $(inputID).on("input", function () {
            $.ajax({
                url: urlPath,
                type: 'get',
                data: { "naziv": $(this).val() },
                dataType: 'json',
                beforeSend: function () {
                    $(inputID).css({ "background": "#eee url(../Content/images/LoaderIcon.gif) no-repeat right 3% center" });
                },
                success: function (data) {
                    $(inputID).css("background", "#fff");

                    var total = data.Count;
                    if (total > 0) {
                        console.log("total found: " + total);
                        createList(data);
                        setNextPrevBtn(data);
                    }
                    else {
                        $(searchResultID).empty();
                    }
                }
            });
        });
    }


    function createList(data) {
        console.log(data.Page);
        $(searchResultID).empty();
        $.each(data.Nazivi, function (i, item) {
            $(searchResultID).append("<li class='btn btn-sm' value='" + item + "'>" + item + "</li>"); //btn btn-sm
        });
    }
//+++++++++++++++++++++++++++++++++

