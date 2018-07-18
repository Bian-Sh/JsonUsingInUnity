using QFramework.TimeExtend;
using UnityEngine;

public class Test01 : MonoBehaviour {

    private void OnGUI()
    {
        if (GUILayout.Button("开始"))
        {
            float cachedtime = 0;  
            Timer.RemoveAll(); //为避免用户的疯狂点击，将上一次的定时取消
            Timer.AddTimer(5,"Test01").OnUpdated((v)=>  //演示01：每帧执行事件
            {
                if (Mathf.CeilToInt(v*5)>cachedtime)      //演示02：计时，倒计时就是总时长减去这个当前进度罢了
                {
                    cachedtime = Mathf.CeilToInt(v * 5);
                    Debug.LogFormat("计时：第{0}秒钟！",cachedtime);
                }
            }).OnCompleted(()=>  //演示03：定时完成执行
            {
                Debug.Log("5秒计时结束");
            });
        }
    }
}
