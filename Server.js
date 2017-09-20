//GLOBAL VARIABLES
var server = require('http').createServer();
var io = require('socket.io')(server);
var fs = require('fs');
var DbManager = require("./Server/DbManager.js");
var dbM = new DbManager();


var allPlayersLogged = {};
var mapNameInGameIdDatabase = {};



io.sockets.on('connection', function(socket){
    console.log('Client connected is '+socket.id);  


	socket.on('register', onRegister);
	socket.on('login', onLogin);
	socket.on('disconnect', onClientDisconnect);
	socket.on('avatarImg',onNewPhoto);
	socket.on('getPhoto', onGetPhoto);
	socket.on("addFriend",onAddFriend);

});

server.listen(process.env.PORT||3000);
console.log("Server started.");

var onClientDisconnect = function(){
	if(allPlayersLogged[this.id]){
        dbM.MakeLoginOnOff(allPlayersLogged[this.id],false);
        delete allPlayersLogged[allPlayersLogged[this.id]];
        delete allPlayersLogged[this.id];
        //io.sockets.emit('updateListFriends');
    }
}
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
        dbM.CheckLogin(data["username"],data["password"], function(err,resultrow) {
            if(err)
                console.log(err);

            if(err==="fail")
                socketRef.emit("wrongData");
            else if(err==="succes")
            {
                socketRef.emit("loginSuccesfull",{
                    username : resultrow["username"],
                    email: resultrow["email"],
                    nomatches : resultrow["nomatches"],
                    nomatcheswon: resultrow["nomatcheswon"],
                    photourl : resultrow["photourl"]
                });


                allPlayersLogged[socketRef.id] = data["username"];
                //to do update databse for log
                //send array of connected frinds
                //console.log("Dupa logare idu meu este "+id);
                mapNameInGameIdDatabase[data["username"]] = resultrow["idusers"];
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

var onNewPhoto = function(data){
	var dataphoto = data.photo.replace(/^data:image\/\w+;base64,/, "");
	var buf = new Buffer(dataphoto, 'base64');
	var path = './Server/Photos/'+data["username"]+".png";
	fs.exists(path, function(exists) {
				  if(exists) {
				    //DELETE AND RECREATE
				    console.log('File exists. Deleting now ...');
				    fs.unlinkSync(path);
				    fs.writeFileSync(path, buf);
				  } 
				  else {
				    //NEW FILE
				    console.log('New phot');
					fs.writeFileSync(path, buf);
				  }
		});
	dbM.SetPathOfPhoto(data["username"],path);
}

var onGetPhoto = function(data){
	var socketRef = this;
	var path = data["photourl"];
	var base64_data_photo = new Buffer(fs.readFileSync(path)).toString('base64');
	socketRef.emit('photobase64',{
		photoBase64:base64_data_photo
	});
}

var getSocketIdOfUser = function(name,mySocketId){
    for(var sockId in allPlayersLogged){
        if(allPlayersLogged[sockId] === name && sockId!==mySocketId)
            return sockId;
    }
    return -1;
}

var onAddFriend = function(data){
    var socketId = getSocketIdOfUser(data["myfriend"],this.id);
    if(socketId!==-1){
        console.log("Jucatorul "+data["myfriend"]+" cu "+socketId+" este online");      

        dbM.InsertFriend(mapNameInGameIdDatabase[allPlayersLogged[this.id]], data["myfriend"]);
        this.emit("newFriend",{
            name: allPlayersLogged[socketId]
        });

        io.to(socketId).emit("newFriend",{
            name: allPlayersLogged[this.id]
        });
    }
    else
    {
        console.log("Jucatorul "+data["myfriend"]+ " nu este online ");
        this.emit("playerNotOnline");
    }
}
