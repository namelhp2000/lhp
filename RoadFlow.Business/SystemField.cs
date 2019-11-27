using Newtonsoft.Json.Linq;
using RoadFlow.Mapper;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace RoadFlow.Business
{
    public class SystemField
    {
        // Fields
        private readonly RoadFlow.Data.SystemField systemFieldData = new RoadFlow.Data.SystemField();

        // Methods
        public int Add(RoadFlow.Model.SystemField systemField)
        {
            return this.systemFieldData.Add(systemField);
        }

        public int Delete(RoadFlow.Model.SystemField systemField)
        {
            return this.systemFieldData.Delete(systemField);
        }

        public List<RoadFlow.Model.SystemField> Delete(string ids)
        {
            List<RoadFlow.Model.SystemField> list = new List<RoadFlow.Model.SystemField>();
            char[] separator = new char[] { ',' };
            foreach (string str in ids.Split(separator))
            {
                RoadFlow.Model.SystemField button = this.Get(str.ToGuid());
                if (button != null)
                {
                    list.Add(button);
                }
            }
            this.systemFieldData.Delete(list.ToArray());
            return list;
        }

        public RoadFlow.Model.SystemField Get(Guid id)
        {
            return this.systemFieldData.Get(id);
        }

        public List<RoadFlow.Model.SystemField> GetAll()
        {
            return this.systemFieldData.GetAll();
        }

     
       

      
        public int Update(RoadFlow.Model.SystemField systemField)
        {
            return this.systemFieldData.Update(systemField);
        }


        public JArray GetAllJson()
        {
            JArray array = new JArray();
            foreach (RoadFlow.Model.SystemField button in this.GetAll())
            {
                array.Add(JObject.Parse(button.ToString()));
            }
            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tuples">查询语句</param>
        /// <param name="tuple1">排序</param>
        /// <returns></returns>
        public DataTable  SystemTable(ValueTuple<string,string>[] tuples, string  order)
        {
            StringBuilder builder1 = new StringBuilder();

            List<string> systemDataTables = RoadFlow.Utility.Config.systemDataTables;
            builder1.Append("  and  SystemTable  in (");
            for(int i=0;i< systemDataTables.Count;i++ )
            {
               if(i== systemDataTables.Count-1)
                {
                    builder1.Append("'" + systemDataTables[i] + "')");
                }
                else
                {
                    builder1.Append("'" + systemDataTables[i] + "',");
                }
            }


            foreach (ValueTuple<string, string> tuple in tuples )
            {
                if(!tuple.Item2.IsNullOrWhiteSpace())
                {
                    builder1.Append("   and " + tuple.Item1 + "  like  '%" + tuple.Item2 + "%' ");
                }
            }


            builder1.Append("  order by  "  + order);

            DataTable dataTable = new DataTable();
            string sqlstr = "select * from(SELECT top 20000 obj.name AS SystemTable,col.colorder AS TableId,col.name AS FieldName,ISNULL(ep.[value], '') AS FieldDescribe,t.name AS FieldType, col.length AS FieldDecimalDigits,ISNULL(COLUMNPROPERTY(col.id, col.name, 'Scale'), 0) AS FieldLength,CASE WHEN COLUMNPROPERTY(col.id, col.name, 'IsIdentity') = 1 THEN '√' ELSE '' END AS FieldIdentity,CASE WHEN EXISTS(SELECT 1 FROM dbo.sysindexes si INNER JOIN dbo.sysindexkeys sik ON si.id = sik.id AND si.indid = sik.indid INNER JOIN dbo.syscolumns sc ON sc.id = sik.id AND sc.colid = sik.colid INNER JOIN dbo.sysobjects so ON so.name = si.name AND so.xtype = 'PK' WHERE sc.id = col.id  AND sc.colid = col.colid) THEN '√'  ELSE '' END AS FieldPrimaryKey, CASE WHEN col.isnullable = 1 THEN '√'  ELSE ''  END AS FieldAllowEmpty,  ISNULL(comm.text, '') AS FieldDefaultValue  FROM dbo.syscolumns col  LEFT JOIN dbo.systypes t ON col.xtype = t.xusertype  inner JOIN dbo.sysobjects obj ON col.id = obj.id  AND obj.xtype = 'U'  AND obj.status >= 0  LEFT JOIN dbo.syscomments comm ON col.cdefault = comm.id LEFT JOIN sys.extended_properties ep ON col.id = ep.major_id AND col.colid = ep.minor_id  AND ep.name = 'MS_Description'  LEFT JOIN sys.extended_properties epTwo ON obj.id = epTwo.major_id AND epTwo.minor_id = 0  AND epTwo.name = 'MS_Description'  ORDER BY obj.name) SystemField  where  1 = 1  ";
            using (DataContext context = new DataContext())
            {
                dataTable = context.GetDataTable(sqlstr+ builder1.ToString());
            }

            return dataTable;
        }



        public DataTable SystemTable1()
        {
            StringBuilder builder1 = new StringBuilder();

            List<string> systemDataTables = RoadFlow.Utility.Config.systemDataTables;
            builder1.Append("  and  SystemTable  in (");
            for (int i = 0; i < systemDataTables.Count; i++)
            {
                if (i == systemDataTables.Count - 1)
                {
                    builder1.Append("'" + systemDataTables[i] + "')");
                }
                else
                {
                    builder1.Append("'" + systemDataTables[i] + "',");
                }
            }


          
            builder1.Append("  order by  SystemTable asc  "  );

            DataTable dataTable = new DataTable();
            string sqlstr = "select * from(SELECT top 20000 obj.name AS SystemTable,col.colorder AS TableId,col.name AS FieldName,ISNULL(ep.[value], '') AS FieldDescribe,t.name AS FieldType, col.length AS FieldDecimalDigits,ISNULL(COLUMNPROPERTY(col.id, col.name, 'Scale'), 0) AS FieldLength,CASE WHEN COLUMNPROPERTY(col.id, col.name, 'IsIdentity') = 1 THEN '√' ELSE '' END AS FieldIdentity,CASE WHEN EXISTS(SELECT 1 FROM dbo.sysindexes si INNER JOIN dbo.sysindexkeys sik ON si.id = sik.id AND si.indid = sik.indid INNER JOIN dbo.syscolumns sc ON sc.id = sik.id AND sc.colid = sik.colid INNER JOIN dbo.sysobjects so ON so.name = si.name AND so.xtype = 'PK' WHERE sc.id = col.id  AND sc.colid = col.colid) THEN '√'  ELSE '' END AS FieldPrimaryKey, CASE WHEN col.isnullable = 1 THEN '√'  ELSE ''  END AS FieldAllowEmpty,  ISNULL(comm.text, '') AS FieldDefaultValue  FROM dbo.syscolumns col  LEFT JOIN dbo.systypes t ON col.xtype = t.xusertype  inner JOIN dbo.sysobjects obj ON col.id = obj.id  AND obj.xtype = 'U'  AND obj.status >= 0  LEFT JOIN dbo.syscomments comm ON col.cdefault = comm.id LEFT JOIN sys.extended_properties ep ON col.id = ep.major_id AND col.colid = ep.minor_id  AND ep.name = 'MS_Description'  LEFT JOIN sys.extended_properties epTwo ON obj.id = epTwo.major_id AND epTwo.minor_id = 0  AND epTwo.name = 'MS_Description'  ORDER BY obj.name) SystemField  where  1 = 1  ";
            using (DataContext context = new DataContext())
            {
                dataTable = context.GetDataTable(sqlstr + builder1.ToString());
            }

            return dataTable;
        }


    }



 

}
