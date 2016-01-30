
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

$(document).ready(function () {
    $('#btnGetFavourites').click(function() {
        var url = $(this).data('action');
        $.get(url, function(data) {

        });
    });
    $('#btnGetVisited').click(function () {
        var url = $(this).data('action');
        $.get(url, function (data) {

        });
    });
    $('#getPlacesByTextBtn').click(function () {
        var url = $(this).data('action');
        var text = $('#userSearchText').val();
        if (text != null && text != " ") {
            $.get(url, { text: text }, function(data) {
                alert(data);
            });
        }
    });

    $('#placeCategory').change(function() {
        var selectedValue = $(this).val();
        var url = $(this).data('action');
        if (selectedValue != "Empty") {
            $.get(url, { category: selectedValue }, function(data) {

            });
        }

    });
});