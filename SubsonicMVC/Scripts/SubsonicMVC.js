/// <reference path="jquery-1.9.0.js" />
/// <reference path="jquery-ui-1.10.0.js" />
/// <reference path="knockout-2.2.1.debug.js" />
/// <reference path="add-on/jplayer.playlist.js" />

(function () {
    var vm = {
        artists: null,
        selectedArtist: null,
        albums: null,
        album: null
    },
    playlist = null,
    playUrl = "home/Play/";

    // Helper to change the ui state of an element to selected.
    function selectEl(el, className) {
        $(className).removeClass("ui-state-highlight");
        $(el).addClass("ui-state-highlight");
    }

    // Artist selected, loaded albums for that artist
    function artistSelected(artistEl) {
        selectEl(artistEl, ".artistName");

        vm.selectedArtist = ko.contextFor(artistEl);

        $.ajax({
            url: "home/Artist",
            data: { id: vm.selectedArtist.$data.Id() },
            async: true,
            success: function (response) {
                $("#albumsContainer").html($("#albumsTemplate").html());
                vm.albums = ko.mapping.fromJS(response);

                // Do we have a current album? If so remove all songs
                if (vm.album) {
                    vm.album.Album.Songs.removeAll();
                }

                ko.applyBindings(vm.albums, $("#albumsContainer")[0]);
                $(".album").draggable({
                    start: function (event, ui) {
                        var album = ko.contextFor(event.target);
                        $.data(ui.helper, "album", album);
                    },
                    revert: "invalid",
                    helper: "clone",
                    stack: ".album"
                });
            }
        });
    }

    // Retrieves an album from the server
    function getAlbum(options) {
        var album = options.albumEl ? ko.contextFor(options.albumEl) : options.album;

        $.ajax({
            url: "home/Album",
            data: { id: album.$data.Id() },
            success: function (response) {
                options.success(response);
            }
        });
    }

    // Load songs for album
    function albumSelected(albumEl) {
        selectEl(albumEl, ".album");

        getAlbum({
            albumEl: albumEl,
            success: function (response) {
                $("#albumContainer").html($("#albumTemplate").html());
                vm.album = ko.mapping.fromJS(response);
                ko.applyBindings(vm.album, $("#albumContainer")[0]);
                $(".song").hover(function () { $(this).addClass('ui-state-hover'); }, function () { $(this).removeClass('ui-state-hover'); });

                $(".song").draggable({
                    start: function (event, ui) {
                        var song = ko.contextFor(event.target);
                        $.data(ui.helper, "song", song);
                    },
                    revert: "invalid",
                    helper: "clone",
                    stack: ".song"
                });
            }
        });
    }

    function highlight(el) {
        el.effect("highlight").effect("highlight").effect("highlight");
    }

    // Song selected, play it immediately (for now)
    function songSelected(songEl) {
    /*
        selectEl(songEl, ".song");

        var song = ko.contextFor(songEl);

        $("#jquery_jplayer_1").jPlayer("setMedia", {
            mp3: playUrl + song.$data.Id()
        }).jPlayer("play");*/
    }

    // initialize the JSPlayer component
    function setupJSPlayer() {
        // Setup our jplayer
        $("#jquery_jplayer_1").jPlayer({
            ready: function () {
            },
            solution: "flash",
            swfPath: "/Scripts"
        });

        $("#jquery_jplayer_1").bind($.jPlayer.event.play, null, function (event) {
            var media = event.jPlayer.status.media;
            $(".posterArt img").attr("src", media.poster).attr("alt", media.artist);
        });

        // Setup our jplayer playlist
        playlist = new jPlayerPlaylist({
            jPlayer: "#jquery_jplayer_1",
            cssSelectorAncestor: "#jp_container_1"
        },
        [], {
            playlistOptions: {
                enableRemoveControls: true
            },
            swfPath: "/Scripts",
            supplied: "mp3"
        });

        $("#jp_container_1").droppable({
            accept: '.song,.album',
            over: function () {
                $('.jp-playlist').addClass("dropHighlight");
            },
            out: function () {
                $('.jp-playlist').removeClass("dropHighlight");
            },
            drop: function (event, ui) {
                if (ui.helper.hasClass("song")) {
                    var song = $.data(ui.helper, "song"),
                    playlistItem = {
                        title: song.$data.Title(),
                        artist: vm.selectedArtist.$data.Name(),
                        mp3: playUrl + song.$data.Id(),
                        poster: vm.album.Album.CoverArtUrl()
                    };

                    highlight(ui.draggable);
                    $('.jp-playlist').removeClass("dropHighlight");

                    playlist.add(playlistItem);

                    /*
                    // attach the playlist item to the newly added li element. Makes it easy to get it 
                    //  back when sorting the list.
                    $(".jp-playlist ul li:last").data("playlistItem", playlistItem);
                    $(".jp-playlist ul").sortable("refresh");*/
                }
                else if (ui.helper.hasClass("album")) {
                    getAlbum({
                        album: $.data(ui.helper, "album"),
                        success: function (response) {
                            highlight(ui.draggable);
                            $('.jp-playlist').removeClass("dropHighlight");

                            $.each(response.Album.Songs, function (i, song) {
                                playlist.add({
                                    title: song.Title,
                                    artist: vm.selectedArtist.$data.Name(),
                                    mp3: playUrl + song.Id,
                                    poster: response.Album.CoverArtUrl
                                });
                            });
                        }
                    });
                }
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

        $(document).on("click", ".song.listItem", function () {
            songSelected(this);
        });

        // Set the playlist as sortable
        /*
        $(".jp-playlist ul").sortable({
        stop: function (event, ui) {
        playlist.playlist = [];
        $(".jp-playlist ul li").each(function () {
        playlist.playlist.push($(this).removeClass("jp-playlist-current").data("playlistItem"));
        });

        $(".jp-playlist ul li:first").addClass("jp-playlist-current");
        }
        });*/

        artistSearcher();
    }

    function artistSearcher() {
        var searchEl = $("#searchArtists"),
            artistsContainer = $("#artistsContainer");

        searchEl.bind("change keydown keyup", function (event) {
            var text = searchEl.val().toLowerCase();

            if (text.length === 0) {
                searchEl.parent().removeClass("searching");

                // Reshow the headers
                $(".ui-accordion-header", artistsContainer).show();

                // Show all artists
                $(".artistName", artistsContainer).show();

                // Show the currently expanded list of artists
                $(".ui-accordion-header-active+.ui-accordion-content", artistsContainer).show();

                // Make sure the non-active ones are not visible
                $(".ui-accordion-content", artistsContainer).not(".ui-accordion-header-active+.ui-accordion-content").hide();
            }
            else {
                searchEl.parent().addClass("searching");

                // hide the headers and show all the content blocks
                $(".ui-accordion-header:visible", artistsContainer).hide();
                $(".ui-accordion-content").show();

                $(".artistName:visible", artistsContainer).each(function () {
                    var artistEl = $(this);
                    console.log("item");

                    if (artistEl.text().toLowerCase().indexOf(text) === -1) {
                        artistEl.hide();
                    }
                    else {
                        artistEl.parent().show();
                        artistEl.show();
                    }
                });

                $(".ui-accordion-content", artistsContainer).not(".ui-accordion-content:has(.artistName:visible)").hide();
            }
        }).focusin(function () {
            $("label[for=searchArtists]").hide();
        }).focusout(function () {
            $("label[for=searchArtists]")[searchEl.val().length ? "hide" : "show"](); 
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