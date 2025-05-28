using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ConfigMgr : Singleton<ConfigMgr>
{
    private string minionConfigName = "Assets/StreamingAssets/Config/Minion.json";
    private string stageConfigName = "Assets/StreamingAssets/Config/Stage.json";

    public MinionConfig MinionConfig = new MinionConfig();
    public StageConfig StageConfig = new StageConfig();

    public void LoadFromJson()
    {
        MinionConfig = LoadConfigFromJSON<MinionConfig>(minionConfigName);
        MinionConfig.InitMap();

        StageConfig = LoadConfigFromJSON<StageConfig>(stageConfigName);
        StageConfig.InitMap();
    }

    // 通用加载配置函数
    private T LoadConfigFromJSON<T>(string path)
    {
        string json;
        json = File.ReadAllText(path);

        return JsonConvert.DeserializeObject<T>(json);
    }
}
