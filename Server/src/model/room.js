var Room = function(no, name){
    // 房间号
    this.room_no = no;
    // 房间名次
    this.room_name = name;

    // 座位
    this.seats = {
        0:null,
        1:null,
        2:null
    }

    // 当前状态 1: 等待 2: 开始 3: 结束
    this.status = 1;
}

Room.prototype.getEmptySeatCount = function(){
    var ret = 0;

    for(var seat in this.seats){
        if(!this.seats[seat]){
            ret++;
        }
    }

    return ret;
}


module.exports = Room;