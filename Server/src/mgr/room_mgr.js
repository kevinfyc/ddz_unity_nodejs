var Room = require('../model/room.js');

var RoomMgr = function(){
    this.rooms = {}; // room_no : room

    this._current_no = 0;
};

RoomMgr.prototype.playerCreateRoom = function(player, room_name){
    this.createRoom(player, room_name);
}

RoomMgr.prototype.playerJoinRoom = function(player, room_no){
    return this.playerJoin(player, room_no);
}

RoomMgr.prototype.playerJoin = function(player, room_no){
    var ret = {};

    ret.status = 11; // 0 成功 10 房间已满 11 没找到房间

    for(var room in this.rooms){
        if(room != room_no){
            continue;
        }

        if(this.rooms[room].getEmptySeatCount() < 3){
            for(var seat_no in this.rooms[room].seats){
                if(!this.rooms[room].seats[seat_no]){
                    this.rooms[room].seats[seat_no] = player;
                    player.seat_no = seat_no;
                    break;
                }
            }

            player.room_no = this.rooms[room].room_no;
            ret.status= 0;
            break;
        }else{
            ret.status= 10;
        }
    }

    return ret;
};

RoomMgr.prototype.getAllRooms = function(){
    var ret = [];
    for(var room_no in this.rooms){
        var room = this.rooms[room_no];
        ret.push([room.room_no, room.room_name, room.status, room.getEmptySeatCount()]);
    }

    return ret;
}

RoomMgr.prototype.createRoom = function(player, room_name){
    var room_no = ++this._current_no;
    var seat_no = 0;

    this.rooms[room_no] = new Room(room_no, room_name);
    player.room_no = room_no;
    player.seat_no = seat_no;
    this.rooms[room_no].seats[seat_no] = player;
};

RoomMgr.prototype.size = function(){
    var ret = 0;
    for(var i in this.rooms){
        if(this.rooms.hasOwnProperty(i)){
            ret++;
        }
    }

    return ret;
};


RoomMgr.prototype.getPlayerBySocketID = function(id){
    for(var room_no in this.rooms){
        var room = this.rooms[room_no];
        for(var seat_no in room.seats){
            var player = room.seats[seat_no];

            if(player && player.socketID == id){
                return player;
            }
        }
    }

    return null;
}

module.exports = RoomMgr;