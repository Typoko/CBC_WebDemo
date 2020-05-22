var pageID;

on("chat:message", function (msg) {
    if (msg.type == "api" && msg.content.indexOf("!MapFlipper") == 0 && playerIsGM(msg.playerid)) {
        var selected = msg.selected;
        if (selected === undefined) {
            sendChat("API", "Please select a character.");
            return;
        }

        var tok = getObj("graphic", selected[0]._id);
        pageID = tok.get("pageid");

        sendChat("Map Flipper", "Creating walls.");

        var paths = msg.content.split("#");
        paths.splice(0, 1);
        paths.forEach(funcCreatePath);

        sendChat("Map Flipper", "Walls created.");
    }
});

function funcCreatePath(element) {

    var pathC = element.split("@");

    var pathWidth = "3";
    var pathHeight = "3";

    if (pathC[2] != 0) {
        pathWidth = pathC[2];
    }

    if (pathC[3] != 0) {
        pathHeight = pathC[3];
    }

    createObj("path", {
        left: pathC[0],
        top: pathC[1],
        pageid: pageID,
        layer: "walls",
        width: parseInt(pathWidth),
        height: parseInt(pathHeight),
        stroke_width: 3,
        path: '[["M",0,0],["L",' + pathC[2] + ',' + pathC[3] + ']]'
    });
}

//"{\"RuutuTable\":[{\"OnkoAvoin\":false,\"Seinat\":[false,false,false,false],\"Kulmat\":[false,false,true,false]},{\"OnkoAvoin\":false,\"Seinat\":[false,false,false,false],\"Kulmat\":[true,false,false,false]}],\"MinX\":0,\"MinY\":0,\"MaxX\":7,\"MaxY\":6,\"TulosteX\":140,\"TulosteY\":120}"

//!MapFlipper #500@200@170@0#500@400@170@0

//!MapFlipper #left: 500, top: 200, pageid: pageID, layer: "walls", width: 140, height: 5, stroke_width: 1, path: '[["M",0,0],["L",140,0]]'#left: 500, top: 400, pageid: pageID, layer: "walls", width: 140, height: 5, stroke_width: 1, path: '[["M",0,0],["L",140,0]]'