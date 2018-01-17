using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SocketIO;

public class Rooms : MonoBehaviour
{
    public GridLayoutGroup grid = null;

    public InputField eidt_name = null;

    private string m_uid = "";
    private SocketIOComponent socket = null;

    public Game game = null;


    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();


        socket.On("s2c_xyst_create_room", OnXystCreateRoom);
        socket.On("s2c_xyst_accept", OnXystAccept);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCreateRoom()
    {
        if (!socket || m_uid == "" || !eidt_name)
            return;

        System.Collections.Generic.Dictionary<string, string> args = new System.Collections.Generic.Dictionary<string, string>();
        args.Add("uid", m_uid);
        args.Add("name", eidt_name.text);

        JSONObject data = new JSONObject(args);
        socket.Emit("c2s_xyst_create_room", data);
    }

    void OnXystCreateRoom(SocketIOEvent e)
    {
        // 0 成功 1 用户不存在 2 用户未登录
        int status = 0;

        e.data.GetField(ref status, "status");

        switch (status)
        {
            case 0:
                gameObject.SetActive(false);
                if (game)
                {
                    game.gameObject.SetActive(true);
                    game.SetGameInfo(m_uid, e.data);
                }
                break;

            default:
                break;
        }
    }

    void OnXystAccept(SocketIOEvent e)
    {
        // 0 成功 1 用户不存在 2 用户未登录 10 房间已满 11 没找到房间
        int status = 0;

        e.data.GetField(ref status, "status");

        switch (status)
        {
            case 0:
                gameObject.SetActive(false);
                if(game)
                {
                    game.gameObject.SetActive(true);
                    game.SetGameInfo(m_uid, e.data);
                }
                break;

            default:
                break;
        }
    }

    public void SetRooms(SocketIOComponent socket, string uid, JSONObject data)
    {
        m_uid = uid;

        Transform grid_trans = grid.GetComponent<Transform>();
        RoomItem[] children = grid_trans.GetComponentsInChildren<RoomItem>();
        for(int i = 0; i < children.Length; ++i)
        {
            RoomItem item = children[i];
            Destroy(item.gameObject);
        }

        JSONObject room_info = data.GetField("room_info");
        for (int i = 0; i < room_info.Count; i++)
        {
            RoomItem item = (RoomItem)Instantiate(Resources.Load<RoomItem>("room_item"));

            JSONObject room_item = room_info[i];
            if (room_item.Count != 4)
                continue;

            int no = System.Convert.ToInt32(room_item[0].ToString());
            string name = room_item[1].ToString();
            int status = System.Convert.ToInt32(room_item[2].ToString());
            int count = System.Convert.ToInt32(room_item[3].ToString());
            item.SetInfo(m_uid, no, name, status, count);

            item.transform.parent = grid_trans;
            item.transform.localScale = Vector3.one;
        }
    }
}
