using UnityEditor;

namespace MeowMeowTools.ExcelTools
{
    public partial class ExcelManager
    {
		public CfgTestA CfgTestA;
        
        public void ReferenceScriptableObject()
        {
            string[] guids;
			string path;
			guids = AssetDatabase.FindAssets("ScriptableObjectTestA t:ScriptableObject", new []{"Assets/Tools/MeowMeowTools/ExcelTools/Data/ScriptableObject"});
			path = AssetDatabase.GUIDToAssetPath(guids[0]);
			CfgTestA = AssetDatabase.LoadAssetAtPath<CfgTestA>(path);
			CfgTestA.Init();
        }
    }
}
