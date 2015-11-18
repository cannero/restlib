onload = function () {
    window.setInterval(switchBackground, 1500);
}

function switchBackground(){
    if (document.getElementById("divH1").style.backgroundColor == "lightblue"){
        document.getElementById("divH1").style.backgroundColor = "transparent";
    } else {
        document.getElementById("divH1").style.backgroundColor = "lightblue";
    }
}
