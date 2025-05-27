using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ConfigMgr : Singleton<ConfigMgr>
{
    private string minionConfigName = "Assets/StreamingAssets/Config/Minion.json";

    public MinionConfig MinionConfig = new MinionConfig();

    public void LoadFromJson()
    {
        MinionConfig = LoadConfigFromJSON<MinionConfig>(minionConfigName);
        MinionConfig.InitMap();
    }

    // 通用加载配置函数
    private T LoadConfigFromJSON<T>(string path)
    {
        Debug.Log(path);
        string json;
        json = File.ReadAllText(path);
        Debug.Log(json);

        return JsonConvert.DeserializeObject<T>(json);
    }
}
