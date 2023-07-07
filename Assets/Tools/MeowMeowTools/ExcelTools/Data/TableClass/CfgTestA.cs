using System;
namespace MeowMeowTools.ExcelTools
{
    [Serializable]
    public class TableTestA : TableBase
    {
		/// <summary>
		/// 测试.
		/// </summary>
		public string test;
		/// <summary>
		/// 测试1.
		/// </summary>
		public string test1;
		/// <summary>
		/// 测试2.
		/// </summary>
		public string test2;
		/// <summary>
		/// 名字.
		/// </summary>
		public string name;
		/// <summary>
		/// 年龄.
		/// </summary>
		public int age;
		/// <summary>
		/// 描述.
		/// </summary>
		public string descripts;
    }

    public class CfgTestA : CfgBase<TableTestA>
    {
    }
}
