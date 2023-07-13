using Sirenix.OdinInspector;
using UnityEngine;

namespace WeAreGladiators.Utilities
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        // Properties
        #region
        public static T Instance { get; private set; }

        [Header("Singleton Properties")]
        [SerializeField] bool dontDestroyOnLoad;
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 20)]
        #endregion

        // Singleton Creation Logic
        #region
        protected virtual void Awake()
        {
            BuildSingleton();
        }
        protected void BuildSingleton()
        {
            if (!Instance)
            {
                Instance = GetComponent<T>();
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(Instance);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion


    }
}