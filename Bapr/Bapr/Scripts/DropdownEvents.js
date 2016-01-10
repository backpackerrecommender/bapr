function GetLocation() {
    if (navigator.geolocation) {
        var pos = navigator.geolocation.getCurrentPosition(function (position) {
            alert(position.coords.latitude + " " + position.coords.longitude);
        });
    } else {
        alert("not supported");
        // x.innerHTML = "Geolocation is not supported by this browser.";
    }
}