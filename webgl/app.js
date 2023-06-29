


//google
//var ws = new WebSocket("wss://umbvh.org:3000");

//local
//var ws = new WebSocket("ws://localhost:3000");

var ws = new ReconnectingWebSocket("wss://vrlab.cs.umb.edu");
let userId = "";


function receiveMissionMessageFromUnity(message) {

    elem = document.getElementById('mission-message');
    elem.innerHTML = message;
}




//ws.addEventListener('ping', () => {
//    console.log('WebSocket received ping');
//});

//ws.addEventListener('pong', () => {
//    console.log('WebSocket received pong');
//});




function receiveDemographicsFromUnity(gender, age, nationality, familiarity) {

    let msg = {};

    msg.type = "demographics";
    msg.id = getParamFromURL('workerId');
	console.log(msg.id);
    msg.age = age;
    msg.gender = gender;
    msg.nationality = nationality;
    msg.familiarity = familiarity;

    
    ws.send(JSON.stringify(msg));


    
}
function receiveCompletedFromUnity() {  
	var message = { data: "completed" };
	window.parent.postMessage(message, "*");
}

function receiveSurveyFromUnity(surveyType, quality, responses, crowdPersonality) {
    let msg = {};

    msg.type = surveyType;
    msg.quality = quality;    
    msg.id = getParamFromURL('workerId');
 
    msg.responses = JSON.stringify(responses);    
    msg.crowdPersonality = crowdPersonality;

    ws.send(JSON.stringify(msg));

// if(surveyType == 'postStudy')
	//receiveCompletedFromUnity();
}


function receiveUserStatsFromUnity(timeSpent, fightCnt, punchCnt, avgSpeed, totalDist, collectedItemCnt, stolenItemCnt, finalItemCnt, crowdPersonality) {

    let msg = {};

    msg.type = "stats";
    msg.id = getParamFromURL('workerId');
    msg.timeSpent = timeSpent;
    msg.fightCnt = fightCnt;
    msg.punchCnt = punchCnt;
    msg.avgSpeed = avgSpeed;
    msg.totalDist = totalDist;
    msg.collectedItemCnt = collectedItemCnt;
    msg.stolenItemCnt = stolenItemCnt;
    msg.finalItemCnt = finalItemCnt;
    msg.crowdPersonality = crowdPersonality;

    ws.send(JSON.stringify(msg));

}



//function sendUserId() {

//    var userId = getParamFromURL('workerId');
//    console.log(userId);
//    console.log(MyUnityInstance);

//    MyUnityInstance.SendMessage("RealPlayerMannequinFP", "ReceiveUserIdFromPage", userId);
    
//}


//var timerID = 0;
//function keepAlive() {
//    var timeout = 1000;
//    if (ws.readyState == ws.OPEN) {
//        ws.send('');

//    }
//    timerId = setTimeout(keepAlive, timeout);
//}
//function cancelKeepAlive() {
//    if (timerId) {
//        clearTimeout(timerId);
//    }
//}
//keepAlive();


function ping() {
    ws.send('ping');
    console.log("ping");
    tm = setTimeout(function () {

        /// ---connection closed ///

        ws.close();
    }, 5000);
}

function pong() {
    //console.log('pong');
    clearTimeout(tm);
}

ws.onopen = function (event) {
    console.log('Connection is open ...');
    setInterval(ping, 10000);
    
    //ws.send(msg);
};
ws.onerror = function (err) {
    console.log('err: ', err);
}
ws.onmessage = function (event) {

    var msg = event.data;
    if (msg == 'pong') {
        pong();
        return;
    }


    console.log(msg);

    if (msg== "completed")
        receiveCompletedFromUnity();

    //if (event.data.userId != "") { //I received the userId from Mturk
    //    //wait till unity is loaded
    //    sendUserId();
    //}
};
ws.onclose = function () {
    console.log("Connection is closed...");
}

function getParamFromURL(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regexS = "[\?&]" + name + "=([^&#]*)";
    var regex = new RegExp(regexS);
    var results = regex.exec(window.location.href);
    if (results == null) {
        if (userId == "")
            userId = generateId();
        return userId;
    }
    else{
	  if(results[1] =="")
	     userId = generateId();

        return results[1];
    }
}


function generateId() {

    return new Date().valueOf();
}

//from here on leave everything as it is
function addId() {
    //setting default values. If the parsing of the workerId fails,
    //99999 will be transmitted.
    var workerId = "99999";
    var iFrameURL = document.location.toString();
    var temp = "";
    //parsing and extracting the workerId
    if (iFrameURL.indexOf("workerId") > 0) {
        if (iFrameURL.indexOf("?") > 0) {
            temp = iFrameURL.split("?")[1];
            if (temp.indexOf("&") > 0) {
                temp = temp.split("&")[2];
                if (temp.indexOf("=") > 0) {
                    workerId = temp.split("=")[1];
                }
            }
        }
    }
    //appending the workerId to the link
    document.links[0].href += "&mtwid=" + workerId;
}
