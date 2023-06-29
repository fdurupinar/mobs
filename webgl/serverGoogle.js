const express = require('express');
const app = express();
const https = require('https');
const path = require('path');
const port = 3000;
const fs = require('fs');
const WebSocket = require('ws');
var mysql = require('mysql');
var compression = require('compression');


app.use(compression());

// Use the whole root as static files to be able to serve the html file and
// the build folder
app.use(express.static(path.join(__dirname, '/')));
// Send html on '/'path
app.get('/', (req, res) => {

    //res.set({'Content - Encoding':  "br"});

    //res.header('Content-Encoding', 'br');
    res.header('Content-Encoding', 'gz');
    res.sendFile(path.join(__dirname, + '/index.html'));
})

var privateKey = fs.readFileSync('umbvh.org.key');
var certificate = fs.readFileSync('umbvh.org.crt');

var credentials = { key: privateKey, cert: certificate };
// Create the server and listen on port
//server = https.createServer(credentials, app).listen(port, () => {
//  console.log(`Server running on localhost:${port}`);
//});


server = https.createServer(credentials, app);

const wss = new WebSocket.Server({ server });

server.listen(port);

wss.on('connection', ((ws) => {

    
    var con = mysql.createConnection({
        host: "127.0.0.1",
        user: "root",
        password: "Motion12!"
    });

    con.connect(function (err) {
        console.log("Connected to web socket");

        
        if (err) throw err;



        con.query("CREATE DATABASE IF NOT EXISTS  mobsDB", function (err, result) {
            if (err) throw err;
            console.log("Database created");
        });

        con.query("USE mobsDB", function (err, result) {
            if (err) throw err;
            console.log("Database created");
        });
        

        var sql = "CREATE TABLE IF NOT EXISTS  users (id VARCHAR(255), age INT, gender TEXT, nationality TEXT, PRIMARY KEY (id))";
        con.query(sql, function (err, result) {
            if (err) throw err;
            console.log("Table created");
        });

        //var sql = "CREATE TABLE IF NOT EXISTS  preSurveyResponses (id VARCHAR(255), response" +
        //for (let i = 0; i < 17; i++) {
        //    sql += "" + i + ", response"; 
        //}
        
        //sql+= ""+i + ", PRIMARY KEY (id))";
        var sql = "CREATE TABLE IF NOT EXISTS  preSurveyResponses (id VARCHAR(255), responses JSON, PRIMARY KEY (id))";
        con.query(sql, function (err, result) {
            if (err) throw err;
            console.log("Table created");
        });

    });

    ws.on('message', (message) => {

        obj = JSON.parse(message);
        console.log(obj);
        if (obj.type == "demographics") {
            var sql = "INSERT IGNORE INTO users (id, age, gender, nationality) VALUES( \"" + obj.id + "\" ," + obj.age + ", \" " + obj.gender + " \" ,\" " + obj.nationality + " \" )";
            con.query(sql, function (err, result) {
                if (err) throw err;
                console.log("1 record inserted");

                console.log(result);
            });
        }
        else if (obj.type == "preSurvey") {
            var sql = "INSERT IGNORE INTO preSurveyResponsesers (id, responses) VALUES( \"" + obj.id + "\" ," + obj.responses + ")";

            console.log(obj.responses);
        }
     

    });
    ws.on('end', () =>  {
        console.log('Connection ended...');
    });
    ws.send('Hello Client');
}));

