using UnityEngine;
using System.Collections;

using UnityEngine.UI;
using SocketIO;

public class Game : MonoBehaviour
{
    public Button btn_ready = null;
    public Text txt_ready_p2 = null;
    public Text txt_ready_p3 = null;

    private string m_uid = "";
    private SocketIOComponent socket = null;

    private int m_seat_no = -1;

    // Use this for initialization
    void Start()
    {

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("s2m_seat_all", OnSeatAll);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetGameInfo(string uid, JSONObject data)
    {
        m_uid = uid;

        data.GetField(ref m_seat_no, "seat_no");
    }

    public void OnReady()
    {
        if (m_uid == "")
            return;

        System.Collections.Generic.Dictionary<string, string> args = new System.Collections.Generic.Dictionary<string, string>();
        args.Add("uid", m_uid);

        JSONObject data = new JSONObject(args);
        socket.Emit("c2s_ready_toggle", data);
    }

    void OnSeatAll(SocketIOEvent e)
    {
        if (!btn_ready)
            return;

        Text self = btn_ready.GetComponentInChildren<Text>();
        if (!self || !txt_ready_p2 || !txt_ready_p3)
            return;

        self.text = "没有人";

        txt_ready_p2.text = txt_ready_p3.text = "没有人";

        if(m_seat_no < 0)
            return;

        System.Collections.Generic.Dictionary<string, string> data = e.data.ToDictionary();
        foreach (var seat in data)
        {
            int seat_no = System.Convert.ToInt32(seat.Key);
            JSONObject v = new JSONObject(seat.Value);

            int status = 0;
            v.GetField(ref status, "status");
            if(status != 0)
                continue;

            bool ready = false;
            v.GetField(ref ready, "ready");

            int offset = m_seat_no - 0;
            int ui_no = seat_no + offset;

            if (ui_no == 0)
                self.text = ready ? "准备中" : "未准备";
            else if (ui_no == 1)
                txt_ready_p2.text = ready ? "准备中" : "未准备";
            else if (ui_no == 2)
                txt_ready_p3.text = ready ? "准备中" : "未准备";
            else
                Debug.LogError("seat error!");
        }
    }
}
