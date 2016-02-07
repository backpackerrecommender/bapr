var map;
function initMap(latitude, longitude) {
    var latLng = { lat: latitude, lng: longitude };

     map = new google.maps.Map(document.getElementById('map'), {
        zoom: 10,
        center: latLng
    });


    setUserCurrentPositionMarker(latLng, map);
}

function setUserCurrentPositionMarker(latLng, map) {
     var marker = new google.maps.Marker({
             position: latLng,
             map: map,
                     title: 'My location!',
                     });
     marker.setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png');
}

$(document).ready(function () {
    var allMarkers =[];
    $body = $("body");
    $(document).on({
        ajaxStart: function () { $body.addClass("loading"); },
        ajaxStop: function () { $body.removeClass("loading"); }
    });
    $("#tabs").tabs();
    $('#btnGetFavourites').click(function () {
        insertHTML("locationDetails", "");
        var url = $(this).data('action');
        $.get(url, function (data) {
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
            $.get(url, { text: text, latitude: latitude, longitude: longitude }, function (data) {
               locReadyForDisplay = deserializeLocations(data);
                displayMarkers(locReadyForDisplay);
            });
        }
    });

    $('#placeCategory').change(function () {
        insertHTML("locationDetails", "");

        var selectedValue = $(this).val();
        var url = $(this).data('action');
        var latitude = $('.latitude').attr('title');
        var longitude = $('.longitude').attr('title');
        if (selectedValue != "Empty") {
            $.get(url, { category: selectedValue, latitude: latitude, longitude: longitude }, function (data) {
                locReadyForDisplay = deserializeLocations(data);
                displayMarkers(locReadyForDisplay);
            });
        }

    });
});

function deserializeLocations(locations) {
    if (locations != null && locations != "") {
        try {
            locReadyForDisplay = JSON.parse(locations);
            return locReadyForDisplay;
            }
            catch (ex) {
                console.error(ex);
                return [];
            }
    }
return [];
}

function clearOverlays() {
  for (var i = 0; i < allMarkers.length; i++) {
    allMarkers[i].setMap(null);
    }
  allMarkers.length = 0;
}

function displayMarkers(locations) {
    clearOverlays();
    var latitude = $('.latitude').attr('title');
    var longitude = $('.longitude').attr('title');
    var latLng = {
        lat: parseFloat(latitude), lng: parseFloat(longitude)
    }

    $.each(locations, function (key, location) {
        addMarker(location, map);
    });
}

function addMarker(location, map) {
    var latLng = { lat: location.latitude, lng: location.longitude };
    var marker = new google.maps.Marker({
        position: latLng,
        map: map,
        title: location.name,

    });
    marker.set("location", location);

    marker.addListener('click', function () {
        var loc = marker.get("location");

        html = getLocationDetails(loc);
        insertHTML("locationDetails", html);
    });
    allMarkers.push(marker);
}

function getLocationDetails(location) {
    html = "<h4>" + location.name + "</h4>";
    var loc = JSON.stringify(location);
    html += getHtmlForCheckBoxes(loc);
    html += "<br/>"
    $.each(location.attributes, function (key, detail) {
        if (detail.Type == "string" || detail.Type == "number") {
            html += "<div><b>" + detail.Name + "</b>: "  + detail.Value + "</div>";
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