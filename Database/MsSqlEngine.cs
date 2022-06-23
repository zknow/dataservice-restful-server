using System;
using HttpDataServer.Models;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;

//Doc:https://linq2db.github.io/index.html
namespace HttpDataServer.Database
{
    public class MsSqlEngine : DataConnection
    {
        public ITable<Account> Accounts { get { return this.GetTable<Account>(); } }
        public ITable<Device> Devices { get { return this.GetTable<Device>(); } }

        public MsSqlEngine(string connString) : base(SqlServerTools.GetDataProvider(), connString)
        {
            (this as IDataContext).CloseAfterUse = false;
        }

        public void CreateTabls()
        {
            try { this.CreateTable<Account>(); } catch (Exception) { }
            try { this.CreateTable<Device>(); } catch (Exception) { }
        }
    }
}