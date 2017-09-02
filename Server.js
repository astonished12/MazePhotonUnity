var server = require('http').createServer();
var io = require('socket.io')(server);

io.on('connection', function(client){
  
  console.log("Un nou client ");
  client.on('event', onFirstAttemp);
  client.on('disconnect', function(){});
});

server.listen(process.env.PORT||3000);
console.log("Server started.");

var onFirstAttemp = function(){
	console.log("First attemp");
}