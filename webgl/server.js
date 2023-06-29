const express = require('express');
const app = express();
const http = require('http');
const path = require('path');
const port = 3000;
const fs = require('fs');
const WebSocket = require('ws');
var mysql = require('mysql');
var compression = require('compression');


app.use(compression());

// Use the whole root as static files to be able to serve the html file and
// the build folder
app.use(express.static(path.join(__dirname, '/'), {

    setHeaders: function (res, path) {
        if (path.endsWith(".unityweb")) {
            res.set("Content-Encoding", "br");
        }
    }

}));
// Send html on '/'path
app.get('/', (req, res) => {

    //res.set({'Content - Encoding':  "br"});


    res.header('Content-Encoding', 'br');
    //res.header('Content-Encoding', 'gz');
    res.sendFile(path.join(__dirname, + '/index.html'));
})


// Create the server and listen on port
server = http.createServer(app);
//.listen(port, () => {
// console.log(`Server running on localhost:${port}`);
//});



var dbName = 'mobsHostileAgentDB3';
const wss = new WebSocket.Server({ server });



const pool = mysql.createPool({
    connectionLimit: 500,
    host: "127.0.0.1",
    user: "root",
    password: "Motion12!",
    database: dbName, //second round of studies
});

server.listen(port, () => {
    console.log(`Server running on localhost:${port}`);
});


//function keepAlive(ws) {
//    const interval = setInterval(() => {
//        if (ws.readyState === ws.OPEN) {
//            ws.ping(() => {
//                ws.send('__ping__');
//            });
//        } else {
//            clearInterval(interval);
//        }
//    }, 2000); // Send a ping message every 1 seconds
//}



wss.on('connection', ((ws) => {

    //keepAlive(ws);

    //con.connect(function (err) {
    pool.getConnection(function (err, connection) {
        console.log("Connected to web socket");

        if (err) {
            console.log(err);
            throw err;
        }
    

        connection.query("CREATE DATABASE IF NOT EXISTS " + dbName, function (err, result) {
            if (err) {
                console.log("Database creation error");
                console.log(err);
                throw err;
            }
            console.log("Database created");
        });

        connection.query("USE " + dbName, function (err, result) {
            if (err) {
                console.log("Database creation error");
                console.log(err);
                throw err;
            }
            console.log("Database created");
        });


        var sql = "CREATE TABLE IF NOT EXISTS  users (id VARCHAR(255), age INT, gender TEXT, nationality TEXT, familiarity INT, PRIMARY KEY (id))";
        connection.query(sql, function (err, result) {
            if (err) {
                console.log("users creation error");
                console.log(err);
                throw err;
            }
            console.log("Table created");
        });



        var sql = "CREATE TABLE IF NOT EXISTS  userStats (id VARCHAR(255), timeSpent FLOAT, fightCnt INT, punchCnt INT, avgSpeed FLOAT, totalDist FLOAT, collectedItemCnt INT, stolenItemCnt INT, finalItemCnt INT, crowdPersonality INT, PRIMARY KEY (id))";
        connection.query(sql, function (err, result) {
            if (err) {
                console.log("userStats creation error");
                console.log(err);
                throw err;
            }
            console.log("Table created");
        });

        var sql = "CREATE TABLE IF NOT EXISTS  preSurveyResponses (id VARCHAR(255), quality INT, preSurveyResponses TEXT, crowdPersonality INT, PRIMARY KEY (id))";
        connection.query(sql, function (err, result) {
            if (err) {
                console.log("preSurveyResponses creation error");
                console.log(err);
                throw err;
            }
            console.log("Table created");
        });

        var sql = "CREATE TABLE IF NOT EXISTS  postSurveyResponses (id VARCHAR(255), quality INT, postSurveyResponses TEXT, crowdPersonality INT, PRIMARY KEY (id))";
        connection.query(sql, function (err, result) {
            if (err) {
                console.log("postSurveyResponses creation error");
                console.log(err);
                throw err;
            }
            console.log("Table created");
        });

        var sql = "CREATE TABLE IF NOT EXISTS  personalityResponses (id VARCHAR(255), quality INT, responses TEXT, PRIMARY KEY (id))";
        connection.query(sql, function (err, result) {
            
            if (err) {
                console.log("personalityResponses creation error");
                console.log(err);
                throw err;
            }
            console.log("Table created");
        });
        connection.release();

    });

    ws.on('message', (message) => {
        if (message == 'ping') {
            //console.log("ping");
            ws.send('pong');
        }
        else if (message != '') {

            pool.getConnection(function (err, connection) {


                obj = JSON.parse(message);

                if (obj.type == "demographics") {
                    var sql = "INSERT IGNORE INTO users (id, age, gender, nationality, familiarity) VALUES( \"" + obj.id + "\" ," + obj.age + ", \" " + obj.gender + " \" ,\" " + obj.nationality + " \" , " + obj.familiarity + ")";
                    connection.query(sql, function (err, result) {
                        if (err) {
                            console.log("demographics error");
                            console.log(err);
                            throw err;
                        }
                        console.log("1 record for demographics inserted with id " + obj.id);
                        ws.send( "demographics" ); //let client know that user completed the study
                        console.log(result);
                    });
                }

                else if (obj.type == "personality") {

                    var sql = "INSERT IGNORE INTO personalityResponses (id, quality, responses) VALUES( \"" + obj.id + "\" , " + obj.quality + " ,\" " + obj.responses + "\" )";
                    connection.query(sql, function (err, result) {
                        if (err) {
                            console.log("personalityResponses insert error");
                            console.log(err);
                            throw err;
                        }
                        console.log(obj.responses);
                        console.log("1 record for personality inserted with id " + obj.id);
                        console.log(result);
                    });
                }
                else if (obj.type == "preStudy") {
                    var sql = "INSERT IGNORE INTO preSurveyResponses (id, quality, preSurveyResponses, crowdPersonality) VALUES( \"" + obj.id + "\" , " + obj.quality + " ,\" " + obj.responses + "\" , " + obj.crowdPersonality + "\)";
                    connection.query(sql, function (err, result) {
                        if (err) {
                            console.log("preSurveyResponses insert error");
                            console.log(err);
                            throw err;
                        }
                        console.log("1 record for preSurveyResponses inserted with id " + obj.id);
                        console.log(result);
                    });
                }
                else if (obj.type == "postStudy") {
                    var sql = "INSERT IGNORE INTO postSurveyResponses (id, quality, postSurveyResponses, crowdPersonality) VALUES( \"" + obj.id + "\" , " + obj.quality + " ,\" " + obj.responses + "\" , " + obj.crowdPersonality + "\)";
                    connection.query(sql, function (err, result) {
                        if (err) {
                            console.log("postSurveyResponses insert error");
                            console.log(err);
                            throw err;
                        }
                        console.log("1 record for postSurveyResponses inserted with id " + obj.id);


                        ws.send("completed"); //let client know that user completed the study
                        console.log(result);
                    });
                }
                else if (obj.type == "stats") {
                    var sql = "INSERT IGNORE INTO userStats (id, timeSpent, fightCnt, punchCnt, avgSpeed, totalDist, collectedItemCnt, stolenItemCnt, finalItemCnt, crowdPersonality) VALUES( \"" + obj.id + "\" ," + obj.timeSpent + ",  " + obj.fightCnt + ",  " + obj.punchCnt + ",  "
                        + obj.avgSpeed + " , " + obj.totalDist + " , " + obj.collectedItemCnt + " , " + obj.stolenItemCnt + " , " + obj.finalItemCnt + " , " + obj.crowdPersonality + "  )";
                    connection.query(sql, function (err, result) {
                        if (err) {
                            console.log("stats insert error");
                            console.log(err);
                            throw err;
                        }
                        console.log("1 record for stats inserted with id " + obj.id);
                        console.log(result);

                    });
                }
                connection.release();
            }
            );
        }

       
    });

    ws.on('close', () => {
        console.log('Websocket connection closed...');
    });

    ws.on('end', () => {
        console.log('Connection ended...');
        
    });

    ws.send('Hello Client');
}));

