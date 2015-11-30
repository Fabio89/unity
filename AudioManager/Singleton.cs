using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instanceReference;

    private static object instanceLock = new object();

    protected virtual void Awake()
    {
        if (instanceReference != null)
        {
            //Debug.LogWarning("[Singleton] Destroying duplicate of " + typeof(T));
            Destroy(gameObject);
        }
    }

    public static T instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] instance '" + typeof(T) +
                    "' already destroyed on application quit." +
                    " Won't create again - returning null.");
                return null;
            }

            lock (instanceLock)
            {
                if (instanceReference == null)
                {
                    instanceReference = (T)FindObjectOfType(typeof(T));
                    DontDestroyOnLoad(instanceReference.gameObject);

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                            " - there should never be more than 1 singleton!" +
                            " Reopening the scene might fix it.");
                        return instanceReference;
                    }

                    if (instanceReference == null)
                    {
                        GameObject singleton = new GameObject();
                        instanceReference = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] An instance of " + typeof(T) +
                            " is needed in the scene, so '" + singleton +
                            "' was created with DontDestroyOnLoad.");
                    }
                }

                return instanceReference;
            }
        }       
    }

    private static bool applicationIsQuitting = false;
    /// <summary>
    /// When Unity quits, it destroys objects in a random order.
    /// In principle, a Singleton is only destroyed when application quits.
    /// If any script calls instance after it have been destroyed, 
    ///   it will create a buggy ghost object that will stay on the Editor scene
    ///   even after stopping playing the Application. Really bad!
    /// So, this was made to be sure we're not creating that buggy ghost object.
    /// </summary>
    public void OnDestroy()
    {
        if (this == instanceReference) applicationIsQuitting = true;
    }
}
