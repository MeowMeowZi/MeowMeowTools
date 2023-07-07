1. 可以在ExcelTools.cs中自定义路径
2. ExcelPath: 表格路径
3. TableClassPath: 表结构类路径
4. ScriptableObjectPath: SO数据路径
5. 场景中添加空物体并挂载脚本ExcelManager
6. 改表结构需要先执行 Tools/MeowMeow Tools/Excel Tools/Generate Table Class, 然后执行Tools/MeowMeow Tools/Excel Tools/Generate ScriptableObject, 如果只改了数据只需要执行后者
7. 使用方式: ExcelManager.Instance.xxCfg[id].property;
8. 目前支持int, float, string类型，可在ExcelTools.cs 302行自行添加
9. 表格数据结构可参考TestA.xlsx