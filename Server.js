//GLOBAL VARIABLES
var server = require('http').createServer();
var io = require('socket.io')(server);
var DbManager = require("./Server/DbManager.js");
var dbM = new DbManager();


var allPlayersLogged = {};
var mapNameInGameIdDatabase = {};



io.sockets.on('connection', function(socket){
    console.log('Client connected is '+socket.id);  


	socket.on('register', onRegister);
	socket.on('login', onLogin);
	socket.on('disconnect', function(){});
});

server.listen(process.env.PORT||3000);
console.log("Server started.");


var onRegister = function(data){
	console.log(data);

	var socketRef = this;
    dbM.InserIntoUsers(data["username"],data["password"],data["email"], function(err) {
        if(err)
            console.log(err);
        if(err==="duplicate")
            socketRef.emit("usernameExist");
        else
            socketRef.emit("registerSuccesfull");
    });
}

var checkAlreadyLog = function(name){
    for(var socketId in allPlayersLogged){
        if(allPlayersLogged[socketId] === name)
            return true;
    }
    return false;
}

var onLogin = function(data){
    var socketRef = this;
    if(!checkAlreadyLog(data["username"]))
    {
        dbM.CheckLogin(data["username"],data["password"], function(err,id) {
            if(err)
                console.log(err);

            if(err==="fail")
                socketRef.emit("wrongData");
            else if(err==="succes")
            {
                socketRef.emit("loginSuccesfull",{
                    username : data["username"]
                });


                allPlayersLogged[socketRef.id] = data["username"];
                //to do update databse for log
                //send array of connected frinds
                //console.log("Dupa logare idu meu este "+id);
                mapNameInGameIdDatabase[data["username"]] = id;
                dbM.MakeLoginOnOff(data["username"],true);

                //io.sockets.emit('updateListFriends');

            }
        });
    }
    else
    {
        socketRef.emit("alreadyLoged");
    }
}