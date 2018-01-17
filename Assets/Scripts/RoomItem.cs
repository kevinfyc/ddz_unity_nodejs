using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using SocketIO;

public class RoomItem : MonoBehaviour
{
    public Text txt_name = null;
    public Text txt_status = null;
    public Text txt_count = null;

    private int m_no = -1;

    private string m_uid = "";
    private SocketIOComponent socket = null;

    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("s2m_xyst_accept", OnXystAccept);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetInfo(string uid, int no, string name, int status, int count)
    {
        m_no = no;
        m_uid = uid;

        if (txt_name)
            txt_name.text = name;

        if (txt_status)
        {
            // 1: 等待 2: 开始 3: 结束
            switch(status)
            {
                case 1:
                    txt_status.text = "等待中";
                    break;

                case 2:
                    txt_status.text = "游戏中";
                    break;

                case 3:
                    txt_status.text = "已经结束";
                    break;

                default:
                    txt_status.text = "未知";
                    break;
            }
        }

        if (txt_count)
            txt_count.text = string.Format("空{0}人", count);
    }

    public void OnJoin()
    {
        if (m_no < 0)
            return;

        System.Collections.Generic.Dictionary<string, string> args = new System.Collections.Generic.Dictionary<string, string>();
        args.Add("uid", m_uid);
        args.Add("room_no", m_no.ToString());

        JSONObject data = new JSONObject(args);
        socket.Emit("c2s_xyst_accept", data);
    }

    void OnXystAccept(SocketIOEvent e)
    {

    }
}
