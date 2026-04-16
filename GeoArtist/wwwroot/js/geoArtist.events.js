window.GeoArtist = window.GeoArtist || {};

window.GeoArtist.events = (function () {
    function emit(name, detail) {
        try {
            document.dispatchEvent(new CustomEvent(name, { detail }));
        } catch (error) {
            console.error("GeoArtist.emit: failed to dispatch event.", name, error);
        }
    }

    return {
        emit
    };
})();