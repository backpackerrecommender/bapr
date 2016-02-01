function GetLocation() {
    if (navigator.geolocation) {
        var pos = navigator.geolocation.getCurrentPosition(function (position) {

            var button = document.getElementById("useCurrentLocation");
            var lat = position.coords.latitude;
            var lng = position.coords.longitude


            setHiddenCoordinates(lat, lng);

            //default user location
            var url = $("#RedirectTo").val();
            window.location.href = url.replace("latitude=0", "latitude=" + lat).replace("longitude=0", "longitude=" + lng);
        });
    } else {
        alert("not supported");
    }
}

function initialize() {
    var input = document.getElementById("userOtherLocation");
    var options = {
        language: 'en-GB',
        types: ['(cities)']
    }
    var autocomplete = new google.maps.places.Autocomplete(input, options);
    google.maps.event.addListener(autocomplete, 'place_changed', function () {
        var place = autocomplete.getPlace();
        var lat = place.geometry.location.lat();
        var lng = place.geometry.location.lng();
        input.value = place.name;


        setHiddenCoordinates(lat, lng);
        var url = $("#RedirectTo").val();
        window.location.href = url.replace("latitude=0", "latitude=" + lat).replace("longitude=0", "longitude=" + lng);
    });

};

function setHiddenCoordinates(lat, lng) {
    $('abbr.latitude').val(lat);
    $('abbr.latitude').attr('title', lat);

    $('abbr.longitude').val('title', lng);
    $('abbr.longitude').attr('title', lat);
}

