using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ConfigMgr : Singleton<ConfigMgr>
{
    private string minionConfigName = "Assets/StreamingAssets/Config/Minion.json";
    private string stageConfigName = "Assets/StreamingAssets/Config/Stage.json";
    private string baseHandbookConfigName = "Assets/StreamingAssets/Config/BaseHandbook.json";
    private string cardConfigName = "Assets/StreamingAssets/Config/Card.json";
    private string effectConfigName = "Assets/StreamingAssets/Config/Effect.json";


    public MinionConfig MinionConfig = new MinionConfig();
    public StageConfig StageConfig = new StageConfig();
    public BaseHandbookConfig BaseHandbookConfig = new BaseHandbookConfig();
    public CardConfig CardConfig = new CardConfig();
    public EffectConfig EffectConfig = new EffectConfig();

    public void LoadFromJson()
    {
        MinionConfig = LoadConfigFromJSON<MinionConfig>(minionConfigName);
        MinionConfig.InitMap();

        StageConfig = LoadConfigFromJSON<StageConfig>(stageConfigName);
        StageConfig.InitMap();

        BaseHandbookConfig = LoadConfigFromJSON<BaseHandbookConfig>(baseHandbookConfigName);
        BaseHandbookConfig.InitMap();

        CardConfig = LoadConfigFromJSON<CardConfig>(cardConfigName);
        CardConfig.InitMap();

        EffectConfig = LoadConfigFromJSON<EffectConfig>(effectConfigName);
        EffectConfig.InitMap();
    }

    // 通用加载配置函数
    private T LoadConfigFromJSON<T>(string path)
    {
        string json;
        json = File.ReadAllText(path);

        return JsonConvert.DeserializeObject<T>(json);
    }
}
