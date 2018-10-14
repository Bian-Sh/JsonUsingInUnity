using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickType;
using UnityEngine.UI;
using System;
using QFramework.TimeExtend;
using Newtonsoft.Json;
using System.Linq;

public class ReadInfoVarJson4douyu : MonoBehaviour
{
    public TextAsset jsonData;
    public Text 文本数据;//显示第几集
    public Image 大头贴; //显示封面
    public Button 上一页;
    public Button 下一页;
    public Text pageinfo;
    private Coroutine cachedCor;
    private ImageData data;
    private Rl[] vlist;
    private int index = 0;
    private void Awake()
    {
        //json反序列化
        data = JsonConvert.DeserializeObject<ImageData>(jsonData.text);
        //data = JsonUtility.FromJson<ImageData>(jsonData.text); //没有可序列化属性标识的类不会被序列化，继而报错
        vlist = data.data.rl;
    }
    void Start()
    {
        //按钮事件注册
        上一页.onClick.AddListener(() => OnButtonClicked(上一页.name));
        下一页.onClick.AddListener(() => OnButtonClicked(下一页.name));

        index = -1;
        OnButtonClicked("下一页");
    }

    private void OnButtonClicked(string bt)
    {
        if (null == cachedCor)
        {
            if (bt == "上一页")
            {
                index--;
            }
            else if (bt == "下一页")
            {
                index++;
            }

            if (index < 0)
            {
                index = vlist.Length - 1; //从尾开始
            }
            else if (index > vlist.Length - 1)
            {
                index = 0; //从头开始
            }

            string picUrl = vlist[index].rs1;
            string msg = string.Format("<size=24>昵称：</size>{0}\n<size=24>签名：</size>{1}\n<size=24>标签：</size>{2}", vlist[index].nn, vlist[index].rn, string.Join("、", vlist[index].utag.Select(v => v.name).ToArray()));
            cachedCor = StartCoroutine(GetCoverVarUrl(picUrl, (V) =>
            {
                文本数据.text = msg; //显示第几集
                pageinfo.text = string.Format("{0}/{1}", index + 1, vlist.Length);
                大头贴.sprite = V; //显示封面
                大头贴.SetNativeSize();
                cachedCor = null;
            }));
        }
        else
        {
            Debug.LogWarning("点击频率过快！");
        }
    }

    IEnumerator GetCoverVarUrl(string url, Action<Sprite> OnComplected)
    {
        WWW www = new WWW(url);
        Timer.AddTimer(1f, "ForCor").OnCompleted(() =>  //使用定时器安放一个超时检测
        {
            Debug.LogWarning("连接超时！");
            www.Dispose();
            StopCoroutine(cachedCor);
            cachedCor = null;
        });
        yield return www;
        if (www.isDone && string.IsNullOrEmpty(www.error))
        {
            Timer.DelTimer("ForCor"); //只要访问完成，就将定时器销毁，停止计时检测
            Texture2D texture = new Texture2D(0, 0);
            www.LoadImageIntoTexture(texture);
            if (null != texture)
            {
                Rect rect = new Rect(Vector2.zero, new Vector2(texture.width, texture.height));
                Sprite sprite = Sprite.Create(texture, rect, Vector2.zero);
                OnComplected?.Invoke(sprite);
            }
        }
        www.Dispose();
    }

}
