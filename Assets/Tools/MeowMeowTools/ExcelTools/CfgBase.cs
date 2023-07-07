using System.Collections.Generic;
using UnityEngine;

namespace MeowMeowTools.ExcelTools{
    public class CfgBase<T> : ScriptableObject where T : TableBase{
        [SerializeField]
        protected List<T> list;
        private Dictionary<int, T> dict;

        public void Init(){
            dict = new Dictionary<int, T>();
            foreach (var item in list)
            {
                dict.Add(item.id, item);
            }
        }

        public T GetData(int key){
            if (dict.TryGetValue(key, out T value)){
                return value;
            }
        
            return null;
        }

        public T this[int key]{
            get{
                if (dict.TryGetValue(key, out T value)){
                    return value;
                }
        
                return null;
            }
        }
    }
}