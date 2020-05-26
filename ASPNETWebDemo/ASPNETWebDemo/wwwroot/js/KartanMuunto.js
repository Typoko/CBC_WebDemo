var ruutuLaatta = document.getElementById("ruutuLaatta");
var varjo_0 = document.getElementById("varjo_0");
var varjo_1 = document.getElementById("varjo_1");
var varjo_2 = document.getElementById("varjo_2");
var varjo_3 = document.getElementById("varjo_3");
var varjoKulma_0 = document.getElementById("varjoKulma_0");
var varjoKulma_1 = document.getElementById("varjoKulma_1");
var varjoKulma_2 = document.getElementById("varjoKulma_2");
var varjoKulma_3 = document.getElementById("varjoKulma_3");
var ruutuTekstuuri = document.getElementById("ruutuTekstuuri");
var ruutuTekstuuri400x400 = document.getElementById("ruutuTekstuuri400x400");
var pShadow = document.getElementById("30pShadow");
var canvasContext = document.getElementById("karttaCanvas").getContext("2d");

var createButton = document.getElementById("createButton");
var latausContext = document.getElementById("latausCanvas").getContext("2d");
var karttaKuva = document.getElementById("karttaKuva");
var imgOW = document.getElementById("imgOffsetWidth");
var imgOH = document.getElementById("imgOffsetHeight");
var imgSquare = document.getElementById("imgSquare");

let colUlko = "#333333";
let colSeina = "#494949";
let colLattia = "#777777";


var printMap = false;

function loadPreviewImage() {
    if (document.getElementById("imgPath").value) {
        printMap = false;
        loadWithParameters();
    }
}

//Kopioidaan wall locations clipboardille
function copyWallLocations() {
    var txtWalls = document.getElementById("txtWallLocations");
    txtWalls.style.display = "inline";
    //alert(txtWalls.value);
    txtWalls.select();
    document.execCommand('copy');
    txtWalls.style.display = "none";
    alert("Wall information has been copied to your clipboard.");
}

//Avetaan sivu uudestaan paramentrien kanssa
function loadWithParameters()
{
    let strLocation = "\\home\\index\\?";

    //if (document.getElementById("imgPath").value) {
        strLocation += "imageUrl=" + document.getElementById("imgPath").value + "&";
    //}
    
    //if (document.getElementById("imgSquare").value) {
        strLocation += "rKoko=" + document.getElementById("imgSquare").value + "&";
    //}

    //if (document.getElementById("imgOffsetWidth").value) {
        strLocation += "oWidth=" + document.getElementById("imgOffsetWidth").value + "&";
    //}

    //if (document.getElementById("imgOffsetHeight").value) {
        strLocation += "oHeight=" + document.getElementById("imgOffsetHeight").value + "&";
    //}
    if (document.getElementById("styleOutline").checked) {
        strLocation += "oStyle=styleOutline&";
    }
    else {
        strLocation += "oStyle=styleDonjon&";
    }

    strLocation += "pMap=" + printMap + "&";

    strLocation = strLocation.slice(0, -1);

    window.location.href = strLocation;
}

//imageUrl=https:/ /i.imgur.com/AbsNs83.jpg,rKoko=20,oWidth=0,oHeight=0
function setParametersToInput() {

    if (window.location.href.includes("?")) {

        let strLocation = window.location.href;

        let arrParameters = strLocation.split("?");

        arrParameters = arrParameters[1].split("&");

        //alert(arrParameters[0].slice(arrParameters[0].indexOf("=")));

        document.getElementById("imgPath").value = arrParameters[0].slice(arrParameters[0].indexOf("=") + 1);
        document.getElementById("imgSquare").value = arrParameters[1].slice(arrParameters[1].indexOf("=") + 1);
        document.getElementById("imgOffsetWidth").value = arrParameters[2].slice(arrParameters[2].indexOf("=") + 1);
        document.getElementById("imgOffsetHeight").value = arrParameters[3].slice(arrParameters[3].indexOf("=") + 1);
        document.getElementById(arrParameters[4].slice(arrParameters[4].indexOf("=") + 1)).checked = true;
        printMap = arrParameters[5].slice(arrParameters[5].indexOf("=") + 1);
    }
    else {
        document.getElementById("redrawButton").disabled = true;
        document.getElementById("createButton").disabled = true;
    }

}

function loadWithImage() {
    //alert("loadwithimage");
    if (document.getElementById("imgPath").value) {
        //alert("Printmap: " + printMap);
        if (printMap == "true") {
            //alert("Printmap: " + printMap);
            document.getElementById("createButton").click();
        }

        latausContext.clearRect(0, 0, latausContext.width, latausContext.height);
        latausContext.drawImage(karttaKuva, 0, 0);
        drawGrid(karttaKuva.width, karttaKuva.height, imgSquare.value, imgOW.value, imgOH.value);
    }
}

function drawGrid(iWidth, iHeight, r, shiftX, shiftY) {

    //alert(iWidth + "\n" + iHeight + "\n" + r + "\n" + shiftX + "\n" + shiftY);

    latausContext.strokeStyle = "red";
    latausContext.lineWidth = 0.5;
    latausContext.beginPath();
    for (var x = +r + +shiftX; x < iWidth; x += +r) {
        latausContext.moveTo(x, 0);
        latausContext.lineTo(x, iHeight);
    }

    for (var x = +r + +shiftY; x <= iHeight; x += +r) {
        latausContext.moveTo(0,x);
        latausContext.lineTo(iWidth, x);
    }
    latausContext.stroke();
}

function previewChanged() {

    if (window.location.href.includes("?")) {

        let strLocation = window.location.href;

        let arrParameters = strLocation.split("?");

        arrParameters = arrParameters[1].split("&");

        if (document.getElementById("imgPath").value !== arrParameters[0].slice(arrParameters[0].indexOf("=") + 1)) {
            return true;
        }
        if (document.getElementById("imgSquare").value !== arrParameters[1].slice(arrParameters[1].indexOf("=") + 1)) {
            return true;
        }
        if (document.getElementById("imgOffsetWidth").value !== arrParameters[2].slice(arrParameters[2].indexOf("=") + 1)) {
            return true;
        }
        if (document.getElementById("imgOffsetHeight").value !== arrParameters[3].slice(arrParameters[3].indexOf("=") + 1)) {
            return true;
        }        
    }
    return false;
}


function createMap(Model) {
        
    if (createButton.innerHTML == "Create Map") {
        if (previewChanged()) {
            printMap = true;
            loadWithParameters();
        }
        else {
            //Alustetaan canvas kokonaan kopioitavaksi
            canvasContext.translate(-(Model.MinX * 20), -(Model.MinY * 20));
            canvasContext.beginPath();
            canvasContext.fillStyle = colUlko;
            canvasContext.fillRect(0, 0, ((Model.MaxX + 1) * 20), ((Model.MaxY + 1) * 20));
            canvasContext.stroke();

            for (let i = Model.MinX; i < Model.MaxX; i++) {
                for (let j = Model.MinY; j < Model.MaxY; j++) {
                    if (Model.RuutuTable[i * Model.MaxY + j].OnkoAvoin) {
                        drawRuutu(i, j);
                    }

                    drawRuutuSeinat(i, j, Model.RuutuTable[i * Model.MaxY + j]);
                    drawRuutuKulmat(i, j, Model.RuutuTable[i * Model.MaxY + j]);
                    drawSeinaVarjo(i, j, Model.RuutuTable[i * Model.MaxY + j]);

                }
            }

            document.getElementById("latausCanvas").style = "display:none";
            document.getElementById("karttaCanvas").style = "";
            createButton.innerHTML = "Return to Preview";
            document.getElementById("redrawButton").disabled = true;
            printMap = false;
        }
    }
    else {
        document.getElementById("latausCanvas").style = "";
        document.getElementById("karttaCanvas").style = "display:none";
        createButton.innerHTML = "Create Map";
        document.getElementById("redrawButton").disabled = false;
    }
    
}

function drawTekstuuriOsa(dtoWidth, dtoHeight, dx, dy) {
    //alert("alku");
    sx = Math.floor(Math.random() * (399 - dtoWidth));
    sy = Math.floor(Math.random() * (399 - dtoHeight));
    //alert("Randomoitu: \n"+sx+"\n"+sy);
    canvasContext.drawImage(ruutuTekstuuri400x400, sx, sy, dtoWidth, dtoHeight, dx, dy, dtoWidth, dtoHeight);
    //canvasContext.drawImage(ruutuTekstuuri, dx, dy);
    //alert("loppu");
}

function drawRuutu(i, j) {
    canvasContext.fillStyle = colLattia;
    canvasContext.beginPath();
    canvasContext.fillRect((i * 20), (j * 20), 20, 20);
    drawTekstuuriOsa(20, 20, (i * 20), (j * 20));
    drawTekstuuriOsa(20, 20, (i * 20), (j * 20));
    canvasContext.drawImage(ruutuLaatta, (i * 20), (j * 20));
    //canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20));
    canvasContext.stroke();
}

function drawRuutuSeinat(i, j,ruutu) {

    canvasContext.beginPath();

    //alert(i + "\n" + j + "\n" + seina + "\n" + onkoA);
    if (ruutu.OnkoAvoin == false) {
        canvasContext.fillStyle = colSeina;
        if (ruutu.Seinat[0]) {
            canvasContext.fillRect((i * 20), (j * 20), 20, 5);
            drawTekstuuriOsa(20, 5, (i * 20), (j * 20));
            //canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20), 20, 5);
        }
        if (ruutu.Seinat[1]) {
            //alert(i + "\n" + j + "\n" + seina + "\n" + onkoA);
            canvasContext.fillRect((i * 20 + 15), (j * 20), 5, 20);
            drawTekstuuriOsa(5, 20, (i * 20 + 15), (j * 20));
            //canvasContext.drawImage(ruutuTekstuuri, (i * 20 + 15), (j * 20), 5, 20);
        }
        if (ruutu.Seinat[2]) {
            canvasContext.fillRect((i * 20), (j * 20+15), 20, 5);
            drawTekstuuriOsa(20, 5, (i * 20), (j * 20 + 15));
            //canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20+15), 20, 5);
        }
        if (ruutu.Seinat[3]) {
            canvasContext.fillRect((i * 20), (j * 20), 5, 20);
            drawTekstuuriOsa(5, 20, (i * 20), (j * 20));
            //canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20), 5, 20);
        }
    }
    else {
        
    }
    //alert("Pois Seina.");

    canvasContext.stroke();
}

function drawRuutuKulmat(i, j, ruutu) {

    canvasContext.beginPath();

    //alert(i + "\n" + j + "\n" + seina + "\n" + onkoA);
    if (ruutu.OnkoAvoin == false) {
        canvasContext.fillStyle = colSeina;
        if (ruutu.Kulmat[0]) {
            canvasContext.fillRect((i * 20), (j * 20), 5, 5);
            canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20), 5, 5);
            canvasContext.drawImage(pShadow, (i * 20), (j * 20), 5, 1);
            canvasContext.drawImage(pShadow, (i * 20), (j * 20), 1, 5);
            canvasContext.drawImage(pShadow, (i * 20+4), (j * 20), 1, 5);
            canvasContext.drawImage(pShadow, (i * 20), (j * 20+4), 5, 1);
        }
        if (ruutu.Kulmat[1]) {
            canvasContext.fillRect((i * 20 + 15), (j * 20), 5, 5);
            canvasContext.drawImage(ruutuTekstuuri, (i * 20+15), (j * 20), 5, 5);
            canvasContext.drawImage(pShadow, (i * 20+15), (j * 20), 5, 1);
            canvasContext.drawImage(pShadow, (i * 20+15), (j * 20), 1, 5);
            canvasContext.drawImage(pShadow, (i * 20+19), (j * 20), 1, 5);
            canvasContext.drawImage(pShadow, (i * 20+15), (j * 20+4), 5, 1);
        }
        if (ruutu.Kulmat[2]) {
            canvasContext.fillRect((i * 20 + 15), (j * 20 + 15), 5, 5);
            canvasContext.drawImage(ruutuTekstuuri, (i * 20+15), (j * 20+15), 5, 5);
            canvasContext.drawImage(pShadow, (i * 20+15), (j * 20+15), 5, 1);
            canvasContext.drawImage(pShadow, (i * 20+15), (j * 20+15), 1, 5);
            canvasContext.drawImage(pShadow, (i * 20+19), (j * 20+15), 1, 5);
            canvasContext.drawImage(pShadow, (i * 20+15), (j * 20+19), 5, 1);
        }
        if (ruutu.Kulmat[3]) {
            canvasContext.fillRect((i * 20), (j * 20 + 15), 5, 5);
            canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20+15), 5, 5);
            canvasContext.drawImage(pShadow, (i * 20), (j * 20+15), 5, 1);
            canvasContext.drawImage(pShadow, (i * 20), (j * 20+15), 1, 5);
            canvasContext.drawImage(pShadow, (i * 20+4), (j * 20+15), 1, 5);
            canvasContext.drawImage(pShadow, (i * 20), (j * 20+19), 5, 1);
        }
    }
    else {
        if (ruutu.Kulmat[0]) {
            canvasContext.drawImage(varjoKulma_0, (i * 20), (j * 20));
        }
        if (ruutu.Kulmat[1]) {
            canvasContext.drawImage(varjoKulma_1, (i * 20), (j * 20));
        }
        if (ruutu.Kulmat[2]) {
            canvasContext.drawImage(varjoKulma_2, (i * 20), (j * 20));
        }
        if (ruutu.Kulmat[3]) {
            canvasContext.drawImage(varjoKulma_3, (i * 20), (j * 20));
        }
    }

    
    //alert("Pois Seina.");

    canvasContext.stroke();
}

function drawSeinaVarjo(i, j, ruutu) {

    canvasContext.beginPath();

    //alert(i + "\n" + j + "\n" + seina + "\n" + onkoA);
    if (ruutu.OnkoAvoin == false) {
        canvasContext.fillStyle = colSeina;
        if (ruutu.Seinat[0]) {
            canvasContext.drawImage(pShadow, (i * 20), (j * 20), 20, 1);
            canvasContext.drawImage(pShadow, (i * 20), (j * 20+4), 20, 1);
        }
        if (ruutu.Seinat[1]) {
            canvasContext.drawImage(pShadow, (i * 20 + 15), (j * 20), 1, 20);
            canvasContext.drawImage(pShadow, (i * 20+19), (j * 20), 1, 20);
        }
        if (ruutu.Seinat[2]) {
            canvasContext.drawImage(pShadow, (i * 20), (j * 20 + 15), 20, 1);
            canvasContext.drawImage(pShadow, (i * 20), (j * 20+19), 20, 1);
        }
        if (ruutu.Seinat[3]) {
            canvasContext.drawImage(pShadow, (i * 20), (j * 20), 1, 20);
            canvasContext.drawImage(pShadow, (i * 20+4), (j * 20), 1, 20);
        }
    }
    else {
        if (ruutu.Seinat[0]) {
            canvasContext.drawImage(varjo_0, (i * 20), (j * 20));
        }
        if (ruutu.Seinat[1]) {
            canvasContext.drawImage(varjo_1, (i * 20), (j * 20));
        }
        if (ruutu.Seinat[2]) {
            canvasContext.drawImage(varjo_2, (i * 20), (j * 20));
        }
        if (ruutu.Seinat[3]) {
            canvasContext.drawImage(varjo_3, (i * 20), (j * 20));
        }
    }
    //alert("Pois Seina.");

    canvasContext.stroke();
}

var imgPath = document.getElementById("imgPath");
imgPath.addEventListener("keyup", function (event) {
    // Number 13 is the "Enter" key on the keyboard
    if (event.keyCode === 13) {
        // Cancel the default action, if needed
        event.preventDefault();
        // Trigger the button element with a click
        document.getElementById("loadPreviewPath").click();
    }
});
var imgPath = document.getElementById("imgOffsetWidth");
imgPath.addEventListener("keyup", function (event) {
    // Number 13 is the "Enter" key on the keyboard
    if (event.keyCode === 13) {
        // Cancel the default action, if needed
        event.preventDefault();
        // Trigger the button element with a click
        document.getElementById("redrawButton").click();
    }
});
var imgPath = document.getElementById("imgOffsetHeight");
imgPath.addEventListener("keyup", function (event) {
    // Number 13 is the "Enter" key on the keyboard
    if (event.keyCode === 13) {
        // Cancel the default action, if needed
        event.preventDefault();
        // Trigger the button element with a click
        document.getElementById("redrawButton").click();
    }
});
var imgPath = document.getElementById("imgSquare");
imgPath.addEventListener("keyup", function (event) {
    // Number 13 is the "Enter" key on the keyboard
    if (event.keyCode === 13) {
        // Cancel the default action, if needed
        event.preventDefault();
        // Trigger the button element with a click
        document.getElementById("redrawButton").click();
    }
});

//function testiCanvasLataus() {
//    var testiImageDraw = document.getElementById("testiKuva");
//    //var testicanvas = document.getElementById("testiTulosteCanvas").getContext("2d");
//    canvasContext.drawImage(testiImageDraw, 0, 0);
//}

//Ladattaessa asetetaan parametrin arvot input bokseihin
setParametersToInput();
//Ladataan alustava kuva esi-Canvasiin
loadWithImage();

//testiCanvasLataus();