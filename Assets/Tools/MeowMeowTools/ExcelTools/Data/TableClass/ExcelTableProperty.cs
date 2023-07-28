using UnityEditor;
using UnityEngine;
namespace MeowMeowTools.ExcelTools
{
    public partial class ExcelManager
    {
		[HideInInspector]
		public TableTestA TableTestA;
        
        public void ReferenceScriptableObject()
        {
			TableTestA = Resources.Load<TableTestA>("ScriptableObjectTestA");
			TableTestA.Init();
        }
    }
}
