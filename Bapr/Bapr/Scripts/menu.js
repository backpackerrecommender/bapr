function initMap(latitude, longitude) {
    var latLng = { lat: latitude, lng: longitude };

    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 4,
        center: latLng
    });

    var marker = new google.maps.Marker({
        position: latLng,
        map: map,
        title: 'Hello World!'
    });
}