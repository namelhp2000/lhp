using RoadFlow.Utility.Cache;
using RoadFlow.Data.SqlInterface;
using RoadFlow.Mapper;
using RoadFlow.Model;
using RoadFlow.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace RoadFlow.Data
{
    public class DbconnnectionSql
    {
        // Fields
        private readonly string connStr;
        private readonly RoadFlow.Model.DbConnection dbConnectionModel;
        private readonly string dbType;

        // Methods
        public DbconnnectionSql(RoadFlow.Model.DbConnection dbConnection)
        {
            this.dbType = dbConnection.ConnType.ToLower();
            this.connStr = dbConnection.ConnString;
            this.dbConnectionModel = dbConnection;
        }

        public DbconnnectionSql(string dataBaseType)
        {
            this.dbType = dataBaseType.ToLower();
            this.connStr = string.Empty;
            this.dbConnectionModel = null;
        }

        // Properties
        public ISql SqlInstance
        {
            get
            {
                string dbType = this.dbType;
                switch (dbType)
                {
                    case "sqlserver":
                        return new RoadFlow.Data.SqlInterface.SqlServer(this.dbConnectionModel, this.dbType);

                    case "mysql":
                        return new RoadFlow.Data.SqlInterface.MySql(this.dbConnectionModel, this.dbType);
                }
                if (dbType != "oracle")
                {
                    throw new Exception("不支持的数据库类型");
                }
                return new RoadFlow.Data.SqlInterface.Oracle(this.dbConnectionModel, this.dbType);
            }
        }
    }


}



