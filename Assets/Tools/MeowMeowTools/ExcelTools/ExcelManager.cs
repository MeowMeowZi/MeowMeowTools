using UnityEngine;

namespace MeowMeowTools.ExcelTools{
    public partial class ExcelManager : MonoBehaviour{
        private static ExcelManager _instance;

        public static ExcelManager Instance{
            get{
                // 如果实例尚未存在，则在需要时创建一个新实例
                if (_instance == null)
                {
                    // 在场景中查找现有实例
                    _instance = FindObjectOfType<ExcelManager>();

                    // 如果场景中不存在实例，则创建一个新的空对象并添加MySingleton组件
                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject("ExcelManager");
                        _instance = singletonObject.AddComponent<ExcelManager>();
                    }

                    // 防止实例在场景切换时被销毁
                    DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

        private void Awake(){
            // 确保只有一个实例存在
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
            }
            
            GetType().GetMethod("ReferenceScriptableObject")?.Invoke(this, null);
        }
    }
}