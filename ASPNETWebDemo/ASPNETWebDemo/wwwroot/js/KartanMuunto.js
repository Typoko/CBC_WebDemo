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
var pShadow = document.getElementById("30pShadow");
var canvasContext = document.getElementById("karttaCanvas").getContext("2d");



function loadWithImage()
{
    window.location.href = "\\home\\index\\?imageUrl=" + document.getElementById("imgPath").value;

}

function createMap(Model) {
    
    //Alustetaan canvas kokonaan kopioitavaksi
    canvasContext.translate(-(Model.MinX * 20), -(Model.MinY* 20));
    canvasContext.beginPath();
    canvasContext.fillStyle = "#333333";
    canvasContext.fillRect(0, 0, ((Model.MaxX + 1) * 20), ((Model.MaxY + 1) * 20));
    canvasContext.stroke();

    for (let i = Model.MinX; i < Model.MaxX; i++)
    {
        for (let j = Model.MinY; j < Model.MaxY; j++)
        {
            if (Model.RuutuTable[i * Model.MaxY + j].OnkoAvoin) {
                drawRuutu(i,j);
            }

            drawRuutuSeinat(i, j, Model.RuutuTable[i * Model.MaxY + j]);
            drawRuutuKulmat(i, j, Model.RuutuTable[i * Model.MaxY + j]);
            drawSeinaVarjo(i, j, Model.RuutuTable[i * Model.MaxY + j]);
            
        }
    }

    document.getElementById("karttaKuva").style = "display:none";
    document.getElementById("karttaCanvas").style = "";

}

function createMapTesti(obj) {

    //for (a in obj) {
    //    alert(a);
    //}

    alert("testi");

    alert(obj.RuutuTable[0].Seinat)
    alert(obj.MinY)
    
}

function drawRuutu(i, j) {
    canvasContext.fillStyle = "#777777";
    canvasContext.beginPath();
    canvasContext.fillRect((i * 20), (j * 20), 20, 20);
    canvasContext.drawImage(ruutuLaatta, (i * 20), (j * 20));
    canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20));
    canvasContext.stroke();
}

function drawRuutuSeinat(i, j,ruutu) {

    canvasContext.beginPath();

    //alert(i + "\n" + j + "\n" + seina + "\n" + onkoA);
    if (ruutu.OnkoAvoin == false) {
        canvasContext.fillStyle = "#555555";
        if (ruutu.Seinat[0]) {
            canvasContext.fillRect((i * 20), (j * 20), 20, 5);
            canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20), 20, 5);
        }
        if (ruutu.Seinat[1]) {
            //alert(i + "\n" + j + "\n" + seina + "\n" + onkoA);
            canvasContext.fillRect((i * 20 + 15), (j * 20), 5, 20);
            canvasContext.drawImage(ruutuTekstuuri, (i * 20 + 15), (j * 20), 5, 20);
        }
        if (ruutu.Seinat[2]) {
            canvasContext.fillRect((i * 20), (j * 20+15), 20, 5);
            canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20+15), 20, 5);
        }
        if (ruutu.Seinat[3]) {
            canvasContext.fillRect((i * 20), (j * 20), 5, 20);
            canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20), 5, 20);
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
        canvasContext.fillStyle = "#555555";
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
        canvasContext.fillStyle = "#555555";
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








//for (int i = Model.MinX; i <= Model.MaxX; i++)
//{
//    for (int j = Model.MinY; j <= Model.MaxY; j++)
//    {
//        if (!Model.RuutuTable[i, j].OnkoAvoin) {
//            if (Model.RuutuTable[i, j].Seinat[0]) {
//                    canvasContext.fillRect((i * 20), (j * 20), 20, 5);
//                    canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20), 20, 5);
//                    //canvasContext.drawImage(pShadow, (i * 20), (j * 20), 20, 1);
//                    //canvasContext.drawImage(pShadow, (i * 20), (j * 20+4), 20, 1);
//            }
//            if (Model.RuutuTable[i, j].Seinat[1]) {
//                    canvasContext.fillRect((i * 20 + 15), (j * 20), 5, 20);
//                    canvasContext.drawImage(ruutuTekstuuri, (i * 20 + 15), (j * 20), 5, 20);
//                    //canvasContext.drawImage(pShadow, (i * 20+15), (j * 20), 1, 20);
//                    //canvasContext.drawImage(pShadow, (i * 20+19), (j * 20), 1, 20);
//            }
//            if (Model.RuutuTable[i, j].Seinat[2]) {
//                    canvasContext.fillRect((i * 20), (j * 20+15), 20, 5);
//                    canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20+15), 20, 5);
//                    //canvasContext.drawImage(pShadow, (i * 20), (j * 20+15), 20, 1);
//                    //canvasContext.drawImage(pShadow, (i * 20), (j * 20+19), 20, 1);
//            }
//            if (Model.RuutuTable[i, j].Seinat[3]) {
//                    canvasContext.fillRect((i * 20), (j * 20), 5, 20);
//                    canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20), 5, 20);
//                    //canvasContext.drawImage(pShadow, (i * 20), (j * 20), 1, 20);
//                    //canvasContext.drawImage(pShadow, (i * 20+4), (j * 20), 1, 20);
//            }
//            if (Model.RuutuTable[i, j].Kulmat[0]) {
                
//                    canvasContext.fillRect((i * 20), (j * 20), 5, 5);
//                    canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20), 5, 5);
//                    canvasContext.drawImage(pShadow, (i * 20), (j * 20), 5, 1);
//                    canvasContext.drawImage(pShadow, (i * 20), (j * 20), 1, 5);
//                    canvasContext.drawImage(pShadow, (i * 20+4), (j * 20), 1, 5);
//                    canvasContext.drawImage(pShadow, (i * 20), (j * 20+4), 5, 1);
                
//            }
//            if (Model.RuutuTable[i, j].Kulmat[1]) {
                
//                    canvasContext.fillRect((i * 20+15), (j * 20), 5, 5);
//                    canvasContext.drawImage(ruutuTekstuuri, (i * 20+15), (j * 20), 5, 5);
//                    canvasContext.drawImage(pShadow, (i * 20+15), (j * 20), 5, 1);
//                    canvasContext.drawImage(pShadow, (i * 20+15), (j * 20), 1, 5);
//                    canvasContext.drawImage(pShadow, (i * 20+19), (j * 20), 1, 5);
//                    canvasContext.drawImage(pShadow, (i * 20+15), (j * 20+4), 5, 1);
                
//            }
//            if (Model.RuutuTable[i, j].Kulmat[2]) {
                
//                    canvasContext.fillRect((i * 20+15), (j * 20+15), 5, 5);
//                    canvasContext.drawImage(ruutuTekstuuri, (i * 20+15), (j * 20+15), 5, 5);
//                    canvasContext.drawImage(pShadow, (i * 20+15), (j * 20+15), 5, 1);
//                    canvasContext.drawImage(pShadow, (i * 20+15), (j * 20+15), 1, 5);
//                    canvasContext.drawImage(pShadow, (i * 20+19), (j * 20+15), 1, 5);
//                    canvasContext.drawImage(pShadow, (i * 20+15), (j * 20+19), 5, 1);
                
//            }
//            if (Model.RuutuTable[i, j].Kulmat[3]) {
                
//                    canvasContext.fillRect((i * 20), (j * 20+15), 5, 5);
//                    canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20+15), 5, 5);
//                    canvasContext.drawImage(pShadow, (i * 20), (j * 20+15), 5, 1);
//                    canvasContext.drawImage(pShadow, (i * 20), (j * 20+15), 1, 5);
//                    canvasContext.drawImage(pShadow, (i * 20+4), (j * 20+15), 1, 5);
//                    canvasContext.drawImage(pShadow, (i * 20), (j * 20+19), 5, 1);
                
//            }
//        }
//    }
//}


//canvasContext.stroke();
//canvasContext.beginPath();


//for (int i = Model.MinX; i <= Model.MaxX; i++)
//{
//    for (int j = Model.MinY; j <= Model.MaxY; j++)
//    {
//        if (!Model.RuutuTable[i, j].OnkoAvoin) {
//            if (Model.RuutuTable[i, j].Seinat[0]) {
                
//                    canvasContext.drawImage(pShadow, (i * 20), (j * 20), 20, 1);
//                    canvasContext.drawImage(pShadow, (i * 20), (j * 20+4), 20, 1);
                
//            }
//            if (Model.RuutuTable[i, j].Seinat[1]) {
                
//                    canvasContext.drawImage(pShadow, (i * 20+15), (j * 20), 1, 20);
//                    canvasContext.drawImage(pShadow, (i * 20+19), (j * 20), 1, 20);
                
//            }
//            if (Model.RuutuTable[i, j].Seinat[2]) {
                
//                    canvasContext.drawImage(pShadow, (i * 20), (j * 20+15), 20, 1);
//                    canvasContext.drawImage(pShadow, (i * 20), (j * 20+19), 20, 1);
                
//            }
//            if (Model.RuutuTable[i, j].Seinat[3]) {
                
//                    canvasContext.drawImage(pShadow, (i * 20), (j * 20), 1, 20);
//                    canvasContext.drawImage(pShadow, (i * 20+4), (j * 20), 1, 20);
                
//            }
//        }
//    }
//}




//canvasContext.stroke();
//canvasContext.beginPath();
//canvasContext.fillStyle = "#777777";
//var ruutuLaatta = document.getElementById("ruutuLaatta");



//for (int i = Model.MinX; i <= Model.MaxX; i++)
//{
//    for (int j = Model.MinY; j <= Model.MaxY; j++)
//    {
//        if (Model.RuutuTable[i, j].OnkoAvoin) {
            
//                canvasContext.fillRect((i * 20), (j * 20), 20, 20);
//                canvasContext.drawImage(ruutuLaatta, (i * 20), (j * 20));
//                canvasContext.drawImage(ruutuTekstuuri, (i * 20), (j * 20));
            

//            if (Model.RuutuTable[i, j].Seinat[0]) {
                
//                    canvasContext.drawImage(varjo_0, (i * 20), (j * 20));
                
//            }
//            if (Model.RuutuTable[i, j].Seinat[1]) {
                
//                    canvasContext.drawImage(varjo_1, (i * 20), (j * 20));
                
//            }
//            if (Model.RuutuTable[i, j].Seinat[2]) {
                
//                    canvasContext.drawImage(varjo_2, (i * 20), (j * 20));
                
//            }
//            if (Model.RuutuTable[i, j].Seinat[3]) {
                
//                    canvasContext.drawImage(varjo_3, (i * 20), (j * 20));
                
//            }
//            if (Model.RuutuTable[i, j].Kulmat[0]) {
                
//                    canvasContext.drawImage(varjoKulma_0, (i * 20), (j * 20));
                
//            }
//            if (Model.RuutuTable[i, j].Kulmat[1]) {
                
//                    canvasContext.drawImage(varjoKulma_1, (i * 20), (j * 20));
                
//            }
//            if (Model.RuutuTable[i, j].Kulmat[2]) {
                
//                    canvasContext.drawImage(varjoKulma_2, (i * 20), (j * 20));
                
//            }
//            if (Model.RuutuTable[i, j].Kulmat[3]) {
                
//                    canvasContext.drawImage(varjoKulma_3, (i * 20), (j * 20));
                
//            }
//        }
//    }
//}


//    //var varjoKulma_2 = document.getElementById("varjoKulma_2");
//    //canvasContext.drawImage(varjoKulma_2, 160, 140);
//    canvasContext.stroke();