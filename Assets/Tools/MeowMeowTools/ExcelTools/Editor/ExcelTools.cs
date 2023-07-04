using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Excel;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace MeowMeowTools.ExcelTools
{
    // public class ExcelToolsSettings
    // {
    //     public string ExcelPath;
    //     public string TableClassPath;
    //     public string ScriptableObjectPath;
    // }
    
    public class ExcelTools : EditorWindow
    {
        // private static ExcelToolsSettings settings;
        
        // [MenuItem("Tools/MeowMeow Tools/Excel Tools/Settings", priority = 0)]
        // public static void Settings()
        // {
        //     ExcelTools window = GetWindow<ExcelTools>();
        //     window.Show();
        //
        //     settings = new ExcelToolsSettings();
        //
        //     // 读取配置.
        //     // if (File.Exists(Application.dataPath + "Assets/Tools/MeowMeowTools/ExcelTools/Cfg/ExcelToolsSettings.json"))
        //     // {
        //     //     string json = File.ReadAllText(Application.dataPath + "Assets/Tools/MeowMeowTools/ExcelTools/Cfg/ExcelToolsSettings.json");
        //     //     settings = JsonConvert.DeserializeObject<ExcelToolsSettings>(json);
        //     // }
        // }
        //
        // private void OnGUI()
        // {
        //     EditorGUILayout.BeginVertical();
        //     
        //         // 表格路径选择.
        //         EditorGUILayout.BeginHorizontal();
        //             EditorGUILayout.LabelField("Excel Path:", GUILayout.Width(150f));
        //             EditorGUILayout.TextField(settings.ExcelPath);
        //             if (GUILayout.Button("Path", GUILayout.Width(50f)))
        //             {
        //                 settings.ExcelPath = EditorUtility.OpenFolderPanel("ExcelPath", "", "");
        //             }
        //         EditorGUILayout.EndHorizontal();
        //     
        //         // 表格类路径选择.
        //         EditorGUILayout.BeginHorizontal();
        //             EditorGUILayout.LabelField("Select Table Class Path:", GUILayout.Width(150f));
        //             EditorGUILayout.TextField(settings.TableClassPath);
        //             if (GUILayout.Button("Path", GUILayout.Width(50f)))
        //             {
        //                 settings.TableClassPath = EditorUtility.OpenFolderPanel("TableClass", "", "");
        //             }
        //         EditorGUILayout.EndHorizontal();
        //         
        //         // ScriptableObject路径选择.
        //         EditorGUILayout.BeginHorizontal();
        //             EditorGUILayout.LabelField("ScriptableObject Path:", GUILayout.Width(150f));
        //             EditorGUILayout.TextField(settings.ScriptableObjectPath);
        //             if (GUILayout.Button("Path", GUILayout.Width(50f)))
        //             {
        //                 settings.ScriptableObjectPath = EditorUtility.OpenFolderPanel("ScriptableObjectPath", "", "");
        //             }
        //         EditorGUILayout.EndHorizontal();
        //         
        //         // 保存设置.
        //         if (GUILayout.Button("Save Settings", GUILayout.Width(100f)))
        //         {
        //             string json = JsonConvert.SerializeObject(settings);
        //             if (!Directory.Exists(Application.dataPath + "/Tools/MeowMeowTools/ExcelTools/Cfg/"))
        //             {
        //                 Directory.CreateDirectory(Application.dataPath + "/Tools/MeowMeowTools/ExcelTools/Cfg/");
        //             }
        //
        //             if (!File.Exists(Application.dataPath + "/Tools/MeowMeowTools/ExcelTools/Cfg/ExcelToolsSettings.json"))
        //             {
        //                 File.Create(Application.dataPath + "/Tools/MeowMeowTools/ExcelTools/Cfg/ExcelToolsSettings.json");
        //             }
        //             File.WriteAllText(Application.dataPath + "/Tools/MeowMeowTools/ExcelTools/Cfg/ExcelToolsSettings.json", json);
        //         }
        //
        //     EditorGUILayout.EndVertical();
        // }

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
                    while (reader.Read())
                    {
                        for (int colCounter = 0; colCounter < reader.FieldCount; colCounter++)
                        {
                            // 获取类名.
                            if (!reader.IsDBNull(colCounter) && colCounter == 0 && rowCounter == 0)
                            {
                                className = reader.Name;
                            }
                            // 获取属性名.
                            if (!reader.IsDBNull(colCounter) && rowCounter == 0)
                            {
                                string value = reader.GetString(colCounter);
                                propertyNames.Add(value);
                            }
                            // 获取属性类型.
                            if (!reader.IsDBNull(colCounter) && rowCounter == 1)
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
                        string content = GenerateClassTemplate(className, propertyNames, propertyTypes);
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
                    while (reader.Read())
                    {
                        for (int colCounter = 0; colCounter < reader.FieldCount; colCounter++)
                        {
                            // 获取类名.
                            if (!reader.IsDBNull(colCounter) && colCounter == 0 && rowCounter == 2)
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
                    }
                } while (reader.NextResult());
            
                // 关闭数据流.
                reader.Close();
            }
        }

        // 解析Excel文件路径.
        private static List<string> ParseExcelFiles()
        {
            // 设置路径.
            string path = "Assets/Tools/MeowMeowTools/ExcelTools/Excels";
            // 设置表格后缀格式.
            string[] searchPatterns = { "*.xls", "*.xlsx" };
            
            // 获取所有表格文件路径.
            List<string> files = new List<string>();
            foreach (string pattern in searchPatterns)
            {
                IEnumerable<string> matchingFiles = Directory.EnumerateFiles(path, pattern, SearchOption.AllDirectories);
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
        private static string GenerateClassTemplate(string className, List<string> propertyNames, List<string> propertyTypes)
        {
            string scriptTemplate = 
@"using System;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class #TABLE_NAME#
{#SCRIPTABLE_OBJECT_FIELDS#
}

public class #CFG_NAME# : ScriptableObject
{
    public List<#TABLE_NAME#> list;
    public Dictionary<int, #TABLE_NAME#> dict;
    
    public void Init()
    {
        dict = new Dictionary<int, #TABLE_NAME#>();
        foreach (var item in list)
        {
            dict.Add(item.Id, item);
        }
    }
}
";
            string propertyFields = string.Empty;
            for (int i = 0; i < propertyNames.Count; i++)
            {
                propertyFields += $"\n    public {propertyTypes[i]} {propertyNames[i]};";
            }

            // 替换模板中的占位符
            string scriptContent = scriptTemplate.Replace("#TABLE_NAME#", $"{className}Table")
                .Replace("#CFG_NAME#", $"{className}Cfg")
                .Replace("#SCRIPTABLE_OBJECT_FIELDS#", propertyFields);

            return scriptContent;
        }
        
        // 保存生成的类模板.
        private static void SaveClassTemplate(string className, string scriptContent)
        {
            // 保存文件的路径
            string savePath = $"Assets/Tools/MeowMeowTools/ExcelTools/TableClass/{className}Cfg.cs";

            if (!string.IsNullOrEmpty(savePath))
            {
                // 保存生成的脚本文件
                File.WriteAllText(savePath, scriptContent);
                AssetDatabase.Refresh();

                Debug.Log($"Class table generated: {className} Successfully!");
            }
        }
        
        // 生成ScriptableObject模板.
        private static void GenerateScriptableObjectTemplate(string className, Dictionary<int, List<string>> propertyValues)
        {
            // 生成ScriptableObject.
            ScriptableObject scriptableObject = ScriptableObject.CreateInstance($"{className}Cfg");

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

                if (property.name == "data" && property.type == $"{className}Table")
                {
                    index = 0;
                    enumerator.MoveNext();
                }
            }

            // 应用修改.
            serializedObject.ApplyModifiedProperties();
            
            // 执行初始化方法, 将列表存入字典中.
            MethodInfo methodInfo = scriptableObject.GetType().GetMethod("Init");
            methodInfo.Invoke(scriptableObject, null);

            // 保存文件.
            string assetPath = $"Assets/Tools/MeowMeowTools/ExcelTools/ScriptableObject/{className}ScriptableObject.asset";
            AssetDatabase.CreateAsset(scriptableObject, assetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"ScriptableObject generated: {className} Successfully!");
        }
    }
}