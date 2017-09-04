//GLOBAL VARIABLES
var server = require('http').createServer();
var io = require('socket.io')(server);
var DbManager = require("./Server/DbManager.js");
var dbM = new DbManager();



io.sockets.on('connection', function(socket){
    console.log('Client connected is '+socket.id);  

	socket.on('register', onRegister);
	socket.on('disconnect', function(){});
});

server.listen(process.env.PORT||3000);
console.log("Server started.");


var onRegister = function(data){
	console.log(data);

	var socket = this;
    dbM.InserIntoUsers(data["username"],data["password"],data["email"], function(err) {
        if(err)
            console.log(err);
        if(err==="duplicate")
            socket.emit("usernameExist");
        else
            socket.emit("registerSuccesfull");
    });
}