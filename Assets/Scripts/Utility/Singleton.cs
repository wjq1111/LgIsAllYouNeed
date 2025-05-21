public class Singleton<T> where T : class, new()
{
    private static T s_Instance;

    protected Singleton()
    {

    }

    public static void CreateInstance()
    {
        if (s_Instance == null)
        {
            s_Instance = new T();
            (s_Instance as Singleton<T>).Init();
        }
    }

    public static void DestroyInstance()
    {
        if (s_Instance != null)
        {
            (s_Instance as Singleton<T>).Uninit();
            s_Instance = null;
        }
    }

    public static T Instance
    {
        get
        {
            if (s_Instance == null)
            {
                CreateInstance();
            }
            return s_Instance;
        }
    }

    public static T GetInstance()
    {

        if (s_Instance == null)
        {
            CreateInstance();
        }

        return s_Instance;
    }

    public static bool HasInstance()
    {
        return s_Instance != null;
    }

    public virtual void Init()
    {

    }

    public virtual void Uninit()
    {

    }
}
