using System;
using UnityEngine;

public class AutoSingletonAttribute : Attribute
{
    public bool AutoCreate;

    public AutoSingletonAttribute(bool Create)
    {
        AutoCreate = Create;
    }
}

[AutoSingleton(true)]
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T s_Instance;
    private static bool s_Destoryed;
    public static T Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = GetInstance();
            }
            return s_Instance;
        }
    }

    protected static T GetInstance()
    {
        if (s_Instance == null && !s_Destoryed)
        {
            s_Instance = (T)FindObjectOfType(typeof(T));

            if (s_Instance == null)
            {
                object[] Attributes = typeof(T).GetCustomAttributes(typeof(AutoSingletonAttribute), true);
                if (Attributes.Length > 0)
                {
                    bool AutoCreate = ((AutoSingletonAttribute)Attributes[0]).AutoCreate;
                    if (AutoCreate == false)
                    {
                        return null;
                    }
                }

                GameObject Singleton = new GameObject("[Singleton]" + typeof(T).Name);
                if (Singleton != null)
                {
                    s_Instance = Singleton.AddComponent<T>();
                    s_Instance.Init();
                }

                GameObject BootObject = GameObject.Find("Boot");
                if (BootObject != null)
                {
                    Singleton.transform.SetParent(BootObject.transform);
                }
            }
        }
        return s_Instance;
    }

    public static void DestroyInstance()
    {
        if (s_Instance != null)
        {
            Destroy(s_Instance.gameObject);
        }

        s_Destoryed = true;
        s_Instance = null;
    }

    protected virtual void Awake()
    {
        if (s_Instance != null && s_Instance.gameObject != gameObject)
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }
        else if (s_Instance == null)
        {
            s_Instance = GetComponent<T>();
        }

        DontDestroyOnLoad(gameObject.transform.root);

        Init();
    }

    protected virtual void OnDestroy()
    {
        if (s_Instance != null && s_Instance.gameObject == gameObject)
        {
            s_Instance = null;
        }
    }

    protected virtual void Init()
    {

    }

    protected virtual void Uninit()
    {

    }

    protected virtual void OnApplicationQuit()
    {
        Uninit();
        s_Instance = null;
    }
}
