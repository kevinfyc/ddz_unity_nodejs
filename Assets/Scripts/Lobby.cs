using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using SocketIO;

public class Lobby : MonoBehaviour
{
    public Button btn_enter = null;
    public Button btn_register = null;
    public Button btn_login = null;

    public InputField edit_name = null;
    public InputField edit_psd = null;

    public Game game = null;
    public Rooms rooms = null;

    private SocketIOComponent socket;

    private string uid = "";

    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        if (btn_enter)
            btn_enter.onClick.AddListener(SendMessage_Rooms);
        if (btn_register)
            btn_register.onClick.AddListener(SendMessage_Register);
        if (btn_login)
            btn_login.onClick.AddListener(SendMessage_Login);

        socket.On("s2c_register", OnRegister);
        socket.On("s2c_login", OnLogin);
        socket.On("s2c_xyst_rooms", OnXystRooms);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendMessage_Rooms()
    {
        System.Collections.Generic.Dictionary<string, string> args = new System.Collections.Generic.Dictionary<string, string>();
        args.Add("uid", uid);
        JSONObject data = new JSONObject(args);
        socket.Emit("c2s_xyst_rooms", data);
    }

    public void SendMessage_Register()
    {
        if (!edit_name || !edit_psd)
            return;

        if(edit_name.text == "" || edit_psd.text == "")
            return;

        System.Collections.Generic.Dictionary<string, string> args = new System.Collections.Generic.Dictionary<string, string>();
        args.Add("name", edit_name.text);
        args.Add("psd", edit_psd.text);
        JSONObject data = new JSONObject(args);

        socket.Emit("c2s_register", data);
    }

    public void SendMessage_Login()
    {
        if (!edit_name || !edit_psd)
            return;

        if (edit_name.text == "" || edit_psd.text == "")
            return;

        System.Collections.Generic.Dictionary<string, string> args = new System.Collections.Generic.Dictionary<string, string>();
        args.Add("name", edit_name.text);
        args.Add("psd", edit_psd.text);
        JSONObject data = new JSONObject(args);

        socket.Emit("c2s_login", data);
    }

    public void OnRegister(SocketIOEvent e)
    {
        e.data.GetField(ref uid, "uid");
        Debug.Log("[SocketIO] s2c_register received: " + e.name + " " + e.data + " " + uid);
    }
    public void OnLogin(SocketIOEvent e)
    {
        e.data.GetField(ref uid, "uid");
        Debug.Log("[SocketIO] s2c_login received: " + e.name + " " + e.data + " " + uid);
    }
    public void OnXystRooms(SocketIOEvent e)
    {
        Debug.Log("[SocketIO] s2c_xyst_rooms received: " + e.name + " " + e.data);
        int status = 0;
        e.data.GetField(ref status, "status");

        if(status == 0) // 成功
        {
            gameObject.SetActive(false);
            if (rooms)
            {
                rooms.gameObject.SetActive(true);
                rooms.SetRooms(socket, uid, e.data);
            }
        }
    }
}
