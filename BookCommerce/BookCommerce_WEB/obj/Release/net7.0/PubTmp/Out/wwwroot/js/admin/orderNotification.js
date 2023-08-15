connectionNotification.on("TriggerNotification", (role) => {
    var cookieArr = document.cookie.split(";");
    var role = cookieArr[0].split("=");

    if (role[1] == "Admin" || role[1] == "Employee") {
        toastr.success(`New order has been placed. Please see manage orders.`);
    }
});
connectionNotification.start();