using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Excel;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace MeowMeowTools.ExcelTools
{
    public class ExcelTools : EditorWindow{
        private static readonly string ExcelPath = "Assets/Tools/MeowMeowTools/ExcelTools/Data/Excels";
        private static readonly string TableClassPath = "Assets/Tools/MeowMeowTools/ExcelTools/Data/TableClass";
        private static readonly string ScriptableObjectPath = "Assets/Tools/MeowMeowTools/ExcelTools/Data/Resources";
        
        [MenuItem("Tools/MeowMeow Tools/Excel Tools/Generate Table Class", priority = 1)]
        public static void GenerateTableClass()
        {
            List<string> files = ParseExcelFiles();

            foreach (string file in files)
            {
                // 读取表格数据.
                IExcelDataReader reader = GetExcelDataRender(file);
                do
                {
                    int rowCounter = 0;
                    string className = string.Empty;
                    List<string> propertyNames = new List<string>();
                    List<string> propertyTypes = new List<string>();
                    List<string> propertyDescriptions = new List<string>();
                    while (reader.Read())
                    {
                        for (int colCounter = 0; colCounter < reader.FieldCount; colCounter++)
                        {
                            // 获取类名.
                            if (!reader.IsDBNull(colCounter) && colCounter == 0 && rowCounter == 0)
                            {
                                className = reader.Name;
                            }
                            // 获取属性描述.
                            if (!reader.IsDBNull(colCounter) && rowCounter == 0)
                            {
                                string value = reader.GetString(colCounter);
                                propertyDescriptions.Add(value);
                            }
                            // 获取属性名.
                            if (!reader.IsDBNull(colCounter) && rowCounter == 1)
                            {
                                string value = reader.GetString(colCounter);
                                propertyNames.Add(value);
                            }
                            // 获取属性类型.
                            if (!reader.IsDBNull(colCounter) && rowCounter == 2)
                            {
                                string value = reader.GetString(colCounter);
                                propertyTypes.Add(value);
                            }
                        }
                        rowCounter++;
                    }
                    
                    // 判断类名是否为空.
                    if (!string.IsNullOrEmpty(className))
                    {
                        string content = GenerateClassTemplate(className, propertyDescriptions, propertyNames, propertyTypes);
                        SaveClassTemplate(className, content);
                    }
                } while (reader.NextResult());
            
                // 关闭数据流.
                reader.Close();
            }
        }

        [MenuItem("Tools/MeowMeow Tools/Excel Tools/Generate ScriptableObject", priority = 2)]
        public static void GenerateScriptableObject()
        {
            List<string> files = ParseExcelFiles();
            List<string> classNames = new List<string>();

            foreach (string file in files)
            {
                // 读取表格数据.
                IExcelDataReader reader = GetExcelDataRender(file);
                do
                {
                    int rowCounter = 2;
                    string className = string.Empty;
                    Dictionary<int, List<string>> propertyValues = new Dictionary<int, List<string>>();
                    int key = 0;
                    reader.Read();
                    reader.Read();
                    reader.Read();
                    while (reader.Read())
                    {
                        for (int colCounter = 0; colCounter < reader.FieldCount; colCounter++)
                        {
                            // 获取类名.
                            if (!reader.IsDBNull(colCounter) && colCounter == 0 && rowCounter == 3)
                            {
                                className = reader.Name;
                            }
                            // 获取属性值.
                            if (!reader.IsDBNull(colCounter))
                            {
                                string value = reader.GetString(colCounter);
                                if (colCounter == 0)
                                {
                                    key = int.Parse(value);
                                    List<string> newStrs = new List<string>();
                                    newStrs.Add(key.ToString());
                                    propertyValues.Add(key, newStrs);
                                }
                                else
                                {
                                    propertyValues[key].Add(value);
                                }
                            }
                        }
                        rowCounter++;
                    }
                    
                    // 判断类名是否为空.
                    if (!string.IsNullOrEmpty(className))
                    {
                        GenerateScriptableObjectTemplate(className, propertyValues);
                        classNames.Add(className);
                    }
                } while (reader.NextResult());

                string content = GenerateExcelTablePropertyTemplate(classNames);
                SaveExcelTablePropertyTemplate(content);
                
                // 关闭数据流.
                reader.Close();
            }
        }

        // 解析Excel文件路径.
        private static List<string> ParseExcelFiles()
        {
            // 设置表格后缀格式.
            string[] searchPatterns = { "*.xls", "*.xlsx" };
            
            // 获取所有表格文件路径.
            List<string> files = new List<string>();
            foreach (string pattern in searchPatterns)
            {
                IEnumerable<string> matchingFiles = Directory.EnumerateFiles(ExcelPath, pattern, SearchOption.AllDirectories);
                files.AddRange(matchingFiles);
            }

            return files;
        }

        // 获取Excel数据渲染器.
        private static IExcelDataReader GetExcelDataRender(string path)
        {
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            IExcelDataReader reader = ExcelReaderFactory.CreateBinaryReader(stream);
            stream.Close();
            return reader;
        }

        // 生成类模板.
        private static string GenerateClassTemplate(string className, List<string> propertyDescriptions, List<string> propertyNames, List<string> propertyTypes)
        {
            string scriptTemplate = 
@"using System;
namespace MeowMeowTools.ExcelTools
{
    [Serializable]
    public class #CFG_NAME# : CfgBase
    {#SCRIPTABLE_OBJECT_FIELDS#
    }

    public class #TABLE_NAME# : TableBase<#CFG_NAME#>
    {
    }
}
";
            string propertyFields = string.Empty;
            for (int i = 1; i < propertyNames.Count; i++){
                propertyFields += $"\r\n\t\t/// <summary>";
                propertyFields += $"\r\n\t\t/// {propertyDescriptions[i]}.";
                propertyFields += $"\r\n\t\t/// </summary>";
                propertyFields += $"\r\n\t\tpublic {propertyTypes[i]} {propertyNames[i]};";
            }

            // 替换模板中的占位符
            string scriptContent = scriptTemplate.Replace("#CFG_NAME#", $"Cfg{className}")
                .Replace("#TABLE_NAME#", $"Table{className}")
                .Replace("#SCRIPTABLE_OBJECT_FIELDS#", propertyFields);

            return scriptContent;
        }
        
        // 生成表索引模版.
        private static string GenerateExcelTablePropertyTemplate(List<string> classNames){
            string scriptTemplate = 
@"using UnityEditor;
using UnityEngine;
namespace MeowMeowTools.ExcelTools
{
    public partial class ExcelManager
    {#SCRIPTABLE_OBJECT_FIELDS#
        
        public void ReferenceScriptableObject()
        {#FIND_SCRIPTABLE_OBJECT_FIELDS#
        }
    }
}
";
            string excelTableFields = string.Empty;
            for (int i = 0; i < classNames.Count; i++)
            {
                excelTableFields += $"\r\n\t\t[HideInInspector]";
                excelTableFields += $"\r\n\t\tpublic Table{classNames[i]} Table{classNames[i]};";
            }

            string findScriptableObjectFields = string.Empty;
            for (int i = 0; i < classNames.Count; i++){
                findScriptableObjectFields += $"\r\n\t\t\tTable{classNames[i]} = Resources.Load<Table{classNames[i]}>(\"ScriptableObject{classNames[i]}\");";
                findScriptableObjectFields += $"\r\n\t\t\tTable{classNames[i]}.Init();";
            }

            // 替换模板中的占位符
            string scriptContent = scriptTemplate.Replace("#SCRIPTABLE_OBJECT_FIELDS#", excelTableFields)
                .Replace("#FIND_SCRIPTABLE_OBJECT_FIELDS#", findScriptableObjectFields);

            return scriptContent;
        }
        
        // 保存生成的类模板.
        private static void SaveClassTemplate(string className, string scriptContent)
        {
            // 保存文件的路径
            string savePath = $"{TableClassPath}/Table{className}.cs";

            if (!string.IsNullOrEmpty(savePath))
            {
                // 保存生成的脚本文件
                File.WriteAllText(savePath, scriptContent);
                AssetDatabase.Refresh();

                Debug.Log($"Class table Generated: {className} Successfully!");
            }
        }
        
        // 保存生成的表索引模版.
        private static void SaveExcelTablePropertyTemplate(string scriptContent){
            // 保存文件的路径
            string savePath = $"{TableClassPath}/ExcelTableProperty.cs";

            if (!string.IsNullOrEmpty(savePath))
            {
                // 保存生成的脚本文件
                File.WriteAllText(savePath, scriptContent);
                AssetDatabase.Refresh();

                Debug.Log($"Excel Table Property Generated Successfully!");
            }
        }
        
        // 生成ScriptableObject模板.
        private static void GenerateScriptableObjectTemplate(string className, Dictionary<int, List<string>> propertyValues)
        {
            // 生成ScriptableObject.
            ScriptableObject scriptableObject = ScriptableObject.CreateInstance($"Table{className}");

            // 生成SerializedObject.
            SerializedObject serializedObject = new SerializedObject(scriptableObject);

            // 获取序列化属性迭代器.
            SerializedProperty property = serializedObject.GetIterator();
            // 获取字典迭代器.
            Dictionary<int, List<string>>.Enumerator enumerator = propertyValues.GetEnumerator();
            int index = 0;
            while (property.NextVisible(true))
            {
                if (property.type == "ArraySize")
                {
                    property.arraySize = propertyValues.Count;
                }
                // 下面添加需要解析的属性.
                else if (property.type == "string")
                {
                    property.stringValue = enumerator.Current.Value[index++];
                }
                else if (property.type == "int")
                {
                    property.intValue = int.Parse(enumerator.Current.Value[index++]);
                }
                else if (property.type == "float")
                {
                    property.floatValue = float.Parse(enumerator.Current.Value[index++]);
                }

                if (property.name == "data" && property.type == $"Cfg{className}")
                {
                    index = 0;
                    enumerator.MoveNext();
                }
            }

            // 应用修改.
            serializedObject.ApplyModifiedProperties();

            // 保存文件.
            string assetPath = $"{ScriptableObjectPath}/ScriptableObject{className}.asset";
            AssetDatabase.CreateAsset(scriptableObject, assetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"ScriptableObject Generated: {className} Successfully!");
        }
    }
}