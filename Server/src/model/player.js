
var Player = function(name, sid, uid)
{
    this.name = name;
    this.socketID = sid;
    this.uid = uid;
    this.room_no = null;
    this.seta_no = null;

    this.ready = false;

    // 0 未登录 1 正常 2 离开 3 掉线 
    this.status = 0;
};

module.exports = Player;