var ReadImage = function (file) {

    var reader = new FileReader;
    var image = new Image;

    reader.readAsDataURL(file);
    reader.onload = function (_file) {

        image.src = _file.target.result;
        image.onload = function () {

            var height = this.height;
            var width = this.width;
            var type = file.type;
            var size = ~~(file.size / 1024) + "KB"; // removes anything to the right of the decimal. Same as Math.floor(file.size / 1024) //https://stackoverflow.com/questions/5971645/what-is-the-double-tilde-operator-in-javascript

            //$("#targetImg").width(400);
            //$("#targetImg").attr('src', _file.target.result);
            $("#description").text("Size:" + size + ", " + height + "X" + width + ", " + type + "");

            $("#bgimage").css('background-image', 'url(' + _file.target.result + ')');

            $("#imgPreview").slideDown("normal");
        };
    };
};

var ClearPreview = function () {
    $("#imageBrowse").val('');
    $("#description").text('');
    $("#imgPreview").slideUp("fast");
    $("#fileName").text("No file selected");
};


var UploadImage = function () {

    var file = $("#imageBrowse").get(0).files;
    var data = new FormData();
    data.append("ImageFile", file[0]);
    //data.append("ProductName", "SamsungA8");

    $.ajax({
        type: "POST",
        url: "/Admin/ImageUpload",
        data: data,
        //async: true,
        cache: false,
        contentType: false,
        processData: false,
        timeout: 60000,
        success: function (response) {
            $("#description").html(response.msg);
            if (response.thumb) {
                $(".thumbPreview, .thumbPreviewProfileCard").prop("src", "/Content/images/UploadedImages/" + response.thumb);
            }
        },
        error: function (err) {
            alert(err.statusText);
        }
    });

};


var imageDelete = function () {
    $.ajax({
        type: "POST",
        url: "/Admin/ImageDelete",
        success: function (response) {
            $("#description").html("Image Deleted!");
            $(".thumbPreview").prop("src", "/Content/images/UploadedImages/none.png");
        },
        error: function (err) {
            alert(err.statusText);
        }
    });
};