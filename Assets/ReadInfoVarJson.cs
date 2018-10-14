using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QuickType;
using UnityEngine.UI;
using System;
using QFramework.TimeExtend;
public class ReadInfoVarJson : MonoBehaviour
{
    public TextAsset jsonData;
    public Image image; //显示封面
    public Text text;//显示第几集
    public Button button;//点击使用浏览器访问选中的集数

    private Coroutine cachedCor;
    private VideoData data;
    private Vlist[] vlist;
    private int index = 0;

    void Start()
    {
        data = VideoData.FromJson(jsonData.text);
        vlist = data.Data.Vlist;
      
    }



    IEnumerator GetCoverVarUrl(string url, Action<Sprite> OnComplected)
    {
        WWW www = new WWW(url);
        Timer.AddTimer(0.5f, "ForCor").OnCompleted(() =>  //使用定时器安放一个超时检测
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

    private void OnGUI()
    {

        if (GUILayout.Button("下一集"))
        {
            if (null == cachedCor)
            {
                if (index > vlist.Length - 1)
                {
                    Debug.LogWarning("显示完毕了呀");
                    index = 0; //从头开始
                    return;
                }
                string picUrl = vlist[index].Vpic;
                string msg = string.Format("{0}\n<size=14>{1}</size>", vlist[index].ShortTitle, vlist[index].Vt);
                string videoUrl = vlist[index].Vurl;

                cachedCor = StartCoroutine(GetCoverVarUrl(picUrl, (V) =>
                {
                    text.text = msg; //显示第几集
                    image.sprite = V; //显示封面
                    image.SetNativeSize();
                    cachedCor = null;
                    button.GetComponentInChildren<Text>().text = vlist[index].ShortTitle.Split(' ')[1]; //按钮加个标题
                    button.onClick.RemoveAllListeners(); //移除所有的监听
                    button.onClick.AddListener(() =>     //开启新的监听
                    {
                        System.Diagnostics.Process.Start(videoUrl);  //开启这个链接
                                                                     //System.Diagnostics.Process.Start(vlist[index].Vurl);  //BUG语句 闭包在这里会导致访问的是下一集的链接，因为这个Action里面使用了index，而他随后被自增，所以当点击事件发生时，index已经被加1，故而它访问的是下一条链接，解决办法就是，使用局部变量“videoUrl”缓存这条数据即可
                    });
                    index++;
                }));
            }
            else
            {
                Debug.LogWarning("点击频率过快！");
            }
        }
    }
}
