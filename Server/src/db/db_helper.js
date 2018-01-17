var DBItem = require('./db_item.js');

var _data = {}; // uid : DBItem

var DBHelper = {};

DBHelper.queryByName = function(name){
    for(var uid in _data){
        var item = _data[uid];
        if(item.name == name){
            return item;
        }
    }

    return null;
}

DBHelper.queryByUID = function(uid){
    console.log('_data is : ', _data);
    var item = _data[uid];
    return item;
};

DBHelper.insert = function(item){
    if(!item){
        return false;
    }
    
    for(var uid in _data){
        if(item == _data[uid]){
            return false;
        }
    }

    _data[item.uid] = item;

    return true;
}

module.exports = DBHelper;