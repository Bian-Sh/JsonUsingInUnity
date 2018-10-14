//#pragma warning disable IDE1006 // 命名样式

public class ImageData
{
    public int code { get; set; }
    public string msg { get; set; }
    public Data data { get; set; }
}

public class Data
{
    public Ct ct { get; set; }
    public Rl[] rl { get; set; }
    public int pgcnt { get; set; }
}

public class Ct
{
    public int iv { get; set; }
    public int ivcv { get; set; }
    public int tag { get; set; }
    public string tn { get; set; }
    public int vmcrr { get; set; }
    public string vmcm { get; set; }
}

public class Rl
{
    public int rid { get; set; }
    /// <summary>
    /// 签名
    /// </summary>
    public string rn { get; set; }
    public int uid { get; set; }
    /// <summary>
    /// 昵称 
    /// </summary>
    public string nn { get; set; }
    public int cid1 { get; set; }
    public int cid2 { get; set; }
    public int cid3 { get; set; }
    public int iv { get; set; }
    public string av { get; set; }
    public int ol { get; set; }
    public string url { get; set; }
    public string c2url { get; set; }
    public string c2name { get; set; }
    public Icdata icdata { get; set; }
    public int dot { get; set; }
    public int subrt { get; set; }
    public int topid { get; set; }
    public int bid { get; set; }
    public int gldid { get; set; }
    /// <summary>
    /// 大头贴
    /// </summary>
    public string rs1 { get; set; }
    public string rs16 { get; set; }
    /// <summary>
    /// 标签
    /// </summary>
    public Utag[] utag { get; set; }
    public int rpos { get; set; }
    public int rgrpt { get; set; }
    public string rkic { get; set; }
    public int rt { get; set; }
    public int ot { get; set; }
    public int clis { get; set; }
    public int chanid { get; set; }
    public Icv1[][] icv1 { get; set; }
}

public class Icdata
{
    public _302 _302 { get; set; }
    public _304 _304 { get; set; }
}

public class _302
{
    public string url { get; set; }
    public int w { get; set; }
    public int h { get; set; }
}

public class _304
{
    public string url { get; set; }
    public int w { get; set; }
    public int h { get; set; }
}

public class Utag
{
    public string name { get; set; }
    public int id { get; set; }
}

public class Icv1
{
    public int id { get; set; }
    public string url { get; set; }
    public int score { get; set; }
    public int w { get; set; }
    public int h { get; set; }
}
#pragma warning restore IDE1006 // 命名样式
