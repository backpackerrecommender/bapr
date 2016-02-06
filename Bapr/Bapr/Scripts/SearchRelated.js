
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
        insertHTML("locationDetails", "");
        var url = $(this).data('action');
        $.get(url, function(data) {
            displayMarkers(data);
        });
    });
    $('#btnGetVisited').click(function () {
        insertHTML("locationDetails", "");
        var url = $(this).data('action');
        $.get(url, function (data) {
            displayMarkers(data);
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
    var latitude = $('.latitude').attr('title');
    var longitude = $('.longitude').attr('title');
    var latLng = {
        lat: parseFloat(latitude), lng: parseFloat(longitude)
    }
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 7,
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
    var loc = JSON.stringify(location);
    html += getHtmlForCheckBoxes(loc);

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

    return html;
}

function getHtmlForCheckBoxes(loc) {
    var location = JSON.parse(loc);
    var checkBox = "<input type='checkbox'";
    checkBox += "id = 'favBtn'";
    checkBox += "onchange = 'addToFavorites()'";
    checkBox += "data-loc='" + loc + "'";
    if (location.IsFavorite == true)
        checkBox += "checked";
    checkBox += ">Favourite<br>";

    checkBox += "<input type='checkbox'";
    checkBox += "id = 'visitedBtn'";
    checkBox += "onchange = 'addToVisited()'";
    checkBox += "data-loc='" + loc + "'";
    if (location.IsVisited == true)
        checkBox += "checked";
    checkBox += ">Visited<br>";

    return checkBox;
}

function insertHTML(id, html) {
    var el = document.getElementById(id);

    el.innerHTML = html;
}

function addToFavorites() {
    var location = $('#favBtn').data('loc');
    var url = "/Bapr/MarkLocation";
    location.IsFavorite = $('#favBtn').is(":checked");

    markLocation(url, location, "fav");
}

function addToVisited() {
    var location = $('#visitedBtn').data('loc');
    var url = "/Bapr/MarkLocation";
    location.IsVisited = $('#visitedBtn').is(":checked");

    markLocation(url, location, "visited");
}

//mark location as visited/favorite
function markLocation(url, location, type) {
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ location: location, type: type }),
        done: function (data) {
        }
    });
}