
function initMap(latitude, longitude) {
    var latLng = { lat: latitude, lng: longitude };

    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 10,
        center: latLng
    });


    var marker = new google.maps.Marker({
        position: latLng,
        map: map,
        title: 'Hello World!'
    });
}

$(document).ready(function () {
    $("#tabs").tabs();
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
        insertHTML("locationDetails", "");

        var url = $(this).data('action');
        var text = $('#userSearchText').val();
        var latitude = $('.latitude').attr('title');
        var longitude = $('.longitude').attr('title');
        if (text != null && text != "") {
            $.get(url, { text: text, latitude:latitude, longitude:longitude }, function(data) {
                displayMarkers(data);
            });
        }
    });

    $('#placeCategory').change(function() {
        insertHTML("locationDetails", "");

        var selectedValue = $(this).val();
        var url = $(this).data('action');
        var latitude = $('.latitude').attr('title');
        var longitude = $('.longitude').attr('title');
        if (selectedValue != "Empty") {
            $.get(url, { category: selectedValue, latitude: latitude, longitude: longitude }, function (data) {

            });
        }

    });
});

function displayMarkers(locations) {
    var latLng = {
        lat: 55.37911, lng: 13.710938
    }//
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 2,
        center: latLng
    });
    $.each(locations, function (key, location) {
        addMarker(location, map);
    });
}

function addMarker(location, map) {
    var latLng = { lat: location.latitude, lng: location.longitude };
    var marker = new google.maps.Marker({
        position: latLng,
        map: map,
        title: location.name
    });
    marker.set("location", location);

    marker.addListener('click', function () {
        var loc = marker.get("location");

        html = getLocationDetails(loc);
        insertHTML("locationDetails", html);
    });
}

function getLocationDetails(location) {
    html = "<h4>" + location.name + "</h4>";
    $.each(location.attributes, function (key, detail) {
        if (detail.Type == "string" || detail.Type == "number") {
            html += "<p><b>" + detail.Name + "</b></p>" + "<p>" + detail.Value + "</p>";
        }
        else if (detail.Type == "bool") {
            html += "<input type='checkbox' name='" + detail.Name + "' ";
            if (detail.Value == "True")
                html += "checked";
            html += ">" + detail.Name + "<br>";
        }
    });
    var loc = JSON.stringify({ location: location });
    html += getHtmlForButtons(loc);

    return html;
}

function getHtmlForButtons(loc) {
    var btn = "<button type='button' class='btn btn-danger btn-lg' ";
    btn += "id = 'favBtn'";
    btn += "onclick = 'addToFavorites()'";
    btn += "data-loc='" + loc + "'";
    btn += ">Add to favorites" + "</button>";

    btn += "<button type='button' class='btn btn-danger btn-lg' ";
    btn += "id = 'visitedBtn'";
    btn += "onclick = 'addToVisited()'";
    btn += "data-loc='" + loc + "'";
    btn += ">Add to visited" + "</button>";

    return btn;
}

function addToFavorites() {
    var test = $('#favBtn').data('loc');
    var url = "/Bapr/AddToFavorites";

    $.get(url, location, function (data) {
        alert('back');
    });
}

function addToVisited() {
    var location = $('#visitedBtn').data('loc');
    var url = "/Bapr/AddToVisited";

    $.get(url, { location: location }, function (data) {
        alert('back');
    });
}

function insertHTML(id, html) {
    var el = document.getElementById(id);

    el.innerHTML = html;
}