(function () {
    var t, e, n, i, o; i = "createElementNS" in document && document.createElementNS("http://www.w3.org/2000/svg", "svg").createSVGRect ? "svg" : "no-svg",
        e = "geolocation" in navigator ? "geolocation" : "no-geolocation",
        o = "createTouch" in document ? "touch" : "no-touch",
        n = document.createElement("input"), n.setAttribute("type", "file"),
        t = n.disabled ? "no-inputfile" : "inputfile",
        document.body.className += " " + i + " " + e + " " + o + " " + t
}).call(this);