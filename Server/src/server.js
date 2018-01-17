var app = require('express')();
var http = require('http').Server(app);
var io = require('socket.io')(http);

var DBHelper = require('./db/db_helper.js');
var DBItem = require('./db/db_item.js');

var Player = require('./model/player.js');
var UUID = require('./util/UUID.js');

var RoomMgr = require('./mgr/room_mgr.js');

var roomMgr = new RoomMgr();

io.on('connection', function(socket){
	console.log('a user connected');

	//断线
	socket.on('disconnect', function() {
		var player = roomMgr.getPlayerBySocketID(socket.id);
		if(player){
			socket.leave(player.room_no);
		}
	});

	socket.on('c2s_register', function(data){
		var item = DBHelper.queryByName(data.name);
		var ret = {};
		ret.status = 1; // 0 成功 1已经存在
		if(!item){
			ret.status = 0;

			item = new DBItem(new UUID().id, data.name);
			item.psd = data.psd;
			item.score = 500;

			DBHelper.insert(item);
		}

		ret.name = item.name;
		ret.uid = item.uid;

		console.log('[SERVER] s2c_register ', ret);
		socket.emit('s2c_register', ret);
	});

	socket.on('c2s_login', function(data){
		var item = DBHelper.queryByName(data.name);

		var ret = {};
		ret.status = 0; // 0 成功 1 用户不存在 2 密码不对
		if(!item){
			ret.status = 1;
		}else{
			if(item.psd != data.psd){
				ret.status = 2;
			}else{
				ret.status = 0;
				ret.uid = item.uid;
			}
		}
		console.log('[SERVER] s2c_login ', ret);
		socket.emit('s2c_login', ret);
	});

  socket.on('c2s_xyst_rooms', function (data) {
		var item = DBHelper.queryByUID(data.uid);

		var ret = {};
		ret.status = 0; // 0 成功 1 用户不存在 2 用户未登录

		if(!item){
			ret.status = 1;
			console.log("[SERVER]", data.uid + " is't exist!");
			socket.emit('s2c_xyst_rooms', ret);
			return;
		}
		
		if(item.status == 0){ // 未登录
			ret.status = 2;
			console.log("[SERVER]", data.uid + " not login!");
			socket.emit('s2c_xyst_rooms', ret);
			return;
		}
		
		var room_info = roomMgr.getAllRooms();

		ret.room_info = room_info;
		ret.status = 0;

		console.log('[SERVER] s2c_xyst_rooms ', ret);
		socket.emit('s2c_xyst_rooms', ret);
	});

	socket.on('c2s_xyst_create_room', function (data) {
		var item = DBHelper.queryByUID(data.uid);
		
		var ret = {};
		ret.status = 0; // 0 成功 1 用户不存在 2 用户未登录

		if(!item){
			ret.status = 1;
			console.log("[SERVER]", data.uid + " is't exist!");
			socket.emit('c2s_xyst_create_room', ret);
			return;
		}
		
		if(item.status == 0){ // 未登录
			ret.status = 2;
			console.log("[SERVER]", data.uid + " not login!");
			socket.emit('c2s_xyst_create_room', ret);
			return;
		}

		var player = new Player(item.name, socket.id, item.uid);
		player.score = item.score;

		roomMgr.playerCreateRoom(player, data.name);
		
		ret.status = 0;
		ret.seat_no = player.seat_no;

		console.log('[SERVER] s2c_xyst_create_room ', ret);
		socket.emit('s2c_xyst_create_room', ret);

		socket.join(player.room_no);
		
		//给该房间其它玩家广播信息
		socket.broadcast.to(player.room_no).emit('s2m_seat_all', roomMgr.rooms[player.room_no].seats);

		var room_info = roomMgr.getAllRooms();
		var rooms_ret = {};

		rooms_ret.room_info = room_info;
		rooms_ret.status = 0;

		console.log('[SERVER] s2c_xyst_rooms ', rooms_ret);
		socket.emit('s2c_xyst_rooms', rooms_ret);
	});

	socket.on('c2s_xyst_accept', function (data) {
		var item = DBHelper.queryByUID(data.uid);
		
		var ret = {};
		ret.status = 0; // 0 成功 1 用户不存在 2 用户未登录

		if(!item){
			ret.status = 1;
			console.log("[SERVER]", data.uid + " is't exist!");
			socket.emit('s2c_xyst_accept', ret);
			return;
		}
		
		if(item.status == 0){ // 未登录
			ret.status = 2;
			console.log("[SERVER]", data.uid + " not login!");
			socket.emit('s2c_xyst_accept', ret);
			return;
		}

		var player = new Player(item.name, socket.id, item.uid);
		player.score = item.score;

		ret.status = roomMgr.playerJoinRoom(player, data.room_no);
		ret.seat_no = player.seat_no;
		socket.join(player.room_no);
		
		//给该房间其它玩家广播信息
		socket.broadcast.to(player.room_no).emit('s2m_seat_all', roomMgr.rooms[player.room_no].seats);

		console.log('[SERVER] s2c_xyst_accept ', ret);

		socket.emit('s2c_xyst_accept', ret);
		var room_info = roomMgr.getAllRooms();
		var rooms_ret = {};

		rooms_ret.room_info = room_info;
		rooms_ret.status = 0;

		console.log('[SERVER] s2c_xyst_rooms ', rooms_ret);
		socket.emit('s2c_xyst_rooms', rooms_ret);
	});

});

http.listen(4567, function(){
  console.log('listening on *:3000');
});