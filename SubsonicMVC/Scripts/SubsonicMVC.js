/// <reference path="jquery-1.9.0.js" />
/// <reference path="jquery-ui-1.10.0.js" />
/// <reference path="knockout-2.2.1.debug.js" />
/// <reference path="add-on/jplayer.playlist.js" />

(function () {
    var vm = {
        artists: null,
        albums: null,
        album: null
    },
    playlist = null,
    playUrl = "http://server:9001/rest/stream.view?c=me&v=1.8.0&u=anyone&p=anyone&id=";

    // Helper to change the ui state of an element to selected.
    function selectEl(el, className) {
        $(className).removeClass("ui-state-highlight");
        $(el).addClass("ui-state-highlight");
    }

    // Artist selected, loaded albums for that artist
    function artistSelected(artistEl) {
        selectEl(artistEl, ".artistName");

        var artist = ko.contextFor(artistEl);

        $.ajax({
            url: "home/Artist",
            data: { id: artist.$data.Id() },
            success: function (response) {
                $("#albumsContainer").html($("#albumsTemplate").html());
                vm.albums = ko.mapping.fromJS(response);

                // Do we have a current album? If so remove all songs
                if (vm.album) {
                    vm.album.Album.Songs.removeAll();
                }

                ko.applyBindings(vm.albums, $("#albumsContainer")[0]);
            }
        });
    }

    // Load songs for album
    function albumSelected(albumEl) {
        selectEl(albumEl, ".album");

        var album = ko.contextFor(albumEl);

        $.ajax({
            url: "home/Album",
            data: { id: album.$data.Id() },
            success: function (response) {
                $("#albumContainer").html($("#albumTemplate").html());
                vm.album = ko.mapping.fromJS(response);
                ko.applyBindings(vm.album, $("#albumContainer")[0]);
                $(".song").hover(function () { $(this).addClass('ui-state-hover'); }, function () { $(this).removeClass('ui-state-hover'); });

                $(".song").draggable({
                    revert: "invalid",
                    helper: "clone",
                    stack: ".song"
                });
            }
        });
    }

    // Song selected, play it immediately (for now)
    function songSelected(songEl) {
        selectEl(songEl, ".song");

        var song = ko.contextFor(songEl);

        $("#jquery_jplayer_1").jPlayer("setMedia", {
            mp3: playUrl + song.$data.Id()
        }).jPlayer("play");

    }

    // initialize the JSPlayer component
    function setupJSPlayer() {
        $("#jquery_jplayer_1").jPlayer({
            ready: function () {
            },
            swfPath: "/Scripts"
        });

        playlist = new jPlayerPlaylist({
            jPlayer: "#jquery_jplayer_1",
            cssSelectorAncestor: "#jp_container_1"
        },
        [], {
            swfPath: "/Scripts",
            supplied: "mp3"
        });

        $("#jp_container_1").droppable({
            accept: '.song',
            over: function () {
                $('.jp-playlist').addClass("dropHighlight");
            },
            out: function () {
                $('.jp-playlist').removeClass("dropHighlight");
            },
            drop: function (event, ui) {
                ui.draggable.remove();
                $('.jp-playlist').removeClass("dropHighlight");
                var el = $("<li></li>").append(ui.helper.clone(false).attr("style", null));
                $(".jp-playlist ul").append(el);
            }
        });
    }

    // Hookup event listeners to parts of the ui
    function setupUI() {
        setupJSPlayer();

        // Listen at the document level since these things come and go
        $(document).on("click", ".artistName", function () {
            artistSelected(this);
        });

        $(document).on("click", ".album", function () {
            albumSelected(this);
        });

        $(document).on("click", ".listItem", function () {
            songSelected(this);
        });
    }

    // get the list of artists and put them in the UI
    function getArtists() {
        var folderSelect = $("#musicFolderSelect");

        $.ajax({
            url: "home/Artists",
            success: function (response) {
                vm.artists = ko.mapping.fromJS(response);
                ko.applyBindings(vm.artists, $("#artistsContainer")[0]);
                $("#artists").accordion({
                    heightStyle: "content"
                });

                // Add a hover effect
                $(".artistName").hover(function () { $(this).addClass('ui-state-hover'); }, function () { $(this).removeClass('ui-state-hover'); });
            }
        });
    }

    $(function () {
        setupUI();
        getArtists();
    });
})();