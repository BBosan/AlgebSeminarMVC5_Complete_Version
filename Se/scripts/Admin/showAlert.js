//https://stackoverflow.com/questions/13550477/twitter-bootstrap-alert-message-close-and-open-again
//https://stackoverflow.com/questions/24203617/bootstrap-alert-in-a-fixed-floating-div-at-the-top-of-page
function showAlert(element) {
    $(element).css({ 'opacity': 0, 'background-color': "#ff4d4d" }).slideDown("slow").animate(
    { opacity: 1, 'background-color': "#121416" },
    { queue: false, duration: "slow" });

    setTimeout(function () {
        $(element).slideUp("slow").animate(
    { opacity: 0 },
    { queue: false, duration: "slow" });
    }, 1000);
}