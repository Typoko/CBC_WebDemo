var pageID;

//"#2@2@0@4@1#3@1@5@0@2#3@6@5@0@0#8@2@0@4@3" tuloste pienmapista
//x,y,width,height,wall position

on("ready", function () {
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
});

function funcCreatePath(element) {

    //#2@2@0@4@1

    var pathC = element.split("@");

    if (pathC[4] == 0) {
        pathC[0] = ((parseInt(pathC[0]) + parseInt(pathC[2]) / 2) * 70).toString();
        pathC[1] = ((parseInt(pathC[1]) + 1) * 70 - 8).toString();
        pathC[2] = (parseInt(pathC[2]) * 70 + 20).toString();
        pathC[3] = "0";
    } else if (pathC[4] == 1) {
        pathC[0] = ((parseInt(pathC[0]) + 1) * 70 - 8).toString();
        pathC[1] = ((parseInt(pathC[1]) + parseInt(pathC[3]) / 2) * 70).toString();
        pathC[2] = "0";
        pathC[3] = (parseInt(pathC[3]) * 70 + 20).toString();
    } else if (pathC[4] == 2) {
        pathC[0] = ((parseInt(pathC[0]) + parseInt(pathC[2]) / 2) * 70).toString();
        pathC[1] = (parseInt(pathC[1]) * 70 + 12).toString();
        pathC[2] = (parseInt(pathC[2]) * 70 + 20).toString();
        pathC[3] = "0";
    } else {
        pathC[0] = (parseInt(pathC[0]) * 70 + 12).toString();
        pathC[1] = ((parseInt(pathC[1]) + parseInt(pathC[3]) / 2) * 70).toString();
        pathC[2] = "0";
        pathC[3] = (parseInt(pathC[3]) * 70 + 20).toString();
    }

    //sendChat("0",pathC[0]);
    //sendChat("1",pathC[1]);
    //sendChat("2",pathC[2]);
    //sendChat("3",pathC[3]);

    var pathWidth = "3";
    var pathHeight = "3";

    if (pathC[2] != "0") {
        pathWidth = pathC[2];
    }

    if (pathC[3] != "0") {
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
};

//!MapFlipper #0@1@0@4@1#1@5@5@0@2#1@0@5@0@0#6@1@0@4@3
//!MapFlipper #2@2@0@4@ (väärin)

