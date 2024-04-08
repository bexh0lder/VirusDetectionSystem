﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Reflection;
using System.IO;

namespace VirusDetectionSystem.Utilities
{
    public class SQLiteHelper
    {   /// <summary>
        /// 数据库名称
        /// </summary>
        private string _dbName = "Data.db";

        /// <summary>
        /// 表名称
        /// </summary>
        private string _tableName = "SweepRecord";

        private string VirusDB = "VirusSample";

        /// <summary>
        /// 数据库路径
        /// </summary>
        private string filePath = "";

        /// <summary>
        /// 连接对象
        /// </summary>
        private SQLiteConnection _SQLiteConn = null;

        /// <summary>
        /// 事务对象
        /// </summary>
        private SQLiteTransaction _SQLiteTrans = null;

        /// <summary>
        /// 事务运行标识
        /// </summary>
        private bool _IsRunTrans = false;

        /// <summary>
        /// 事务自动提交标识
        /// </summary>
        private bool _AutoCommit = false;

        /// <summary>
        /// 连接字符串
        /// </summary>
        private string _SQLiteConnString = null; 

        public string SQLiteConnString
        {
            set { this._SQLiteConnString = value; }
            get { return this._SQLiteConnString; }
        }

        /// <summary>
        /// 单例化
        /// </summary>
        private static SQLiteHelper instance = null;

        public static SQLiteHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SQLiteHelper();
                }
                return instance;
            }
        }

        public SQLiteHelper()
        {

            this._SQLiteConnString = $"Data Source={Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\{_dbName}";

            this.filePath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\{_dbName}";

            if (CheckDbIsExist())//数据库存在
            {
                NewTable(this._SQLiteConnString, _tableName);//新建数据表

                NewTable(this._SQLiteConnString, VirusDB);
            }
            else
            {
                return;
            }

        }

        /// <summary>
        /// 检测库是否存在
        /// </summary>
        /// 要检查数据库是否存在，我们可以使用SQL语句来查询系统表sqlite_master。sqlite_master表是SQLite数据库系统中的一个系统表，
        /// 它包含了数据库中所有表的信息。我们可以通过查询sqlite_master表中的特定表名来判断数据库是否存在。
        public bool CheckDbIsExist()
        {
            try
            {
                OpenDb();

                SQLiteCommand DbCmd = new SQLiteCommand();

                DbCmd.Connection = _SQLiteConn;

                DbCmd.CommandText = $"SELECT COUNT(*) FROM sqlite_master where type='table' and name='{_tableName}';";//是否存在扫描记录

                if (0 == Convert.ToInt32(DbCmd.ExecuteScalar()))//数据库存在
                {
                    return true;
                }
                else//数据库不存在
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("打开数据库：" + _dbName + "的连接失败：" + ex.Message);
            }
            finally
            {
                this._SQLiteConn.Close();
            }
        }
        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="dbPath">指定数据库文件</param>
        /// <param name="tableName">表名称</param>
        public void NewTable(string dbPath, string tableName)
        {
            if (_SQLiteConn.State != System.Data.ConnectionState.Open)
            {
                OpenDb();
            }

            SQLiteCommand cmd = new SQLiteCommand();
            cmd.Connection = _SQLiteConn;

            if (tableName == "VirusSample")
            {
                cmd.CommandText = $"CREATE TABLE {tableName}(SampleName varchar)";
            }
            else
            {
                cmd.CommandText = $"CREATE TABLE {tableName}(Time varchar,FileName varchar,FilePath varchar,Type varchar)";
            }

            cmd.ExecuteNonQuery();

            CloseDb();
        }

        /// <summary>
        /// 查询{_tableName}表数据
        /// </summary>
        public DataTable GetDataTable(string type)
        {
            try
            {
                string sql = $"Select * from {_tableName} where type='{type}' order by Time desc LIMIT 10";

                using (SQLiteConnection con = new SQLiteConnection(SQLiteConnString))
                {
                    using (SQLiteCommand cmd = new SQLiteCommand(sql, con))
                    {
                        con.Open();
                        SQLiteDataAdapter ad = new SQLiteDataAdapter(cmd);
                        DataTable tb = new DataTable();
                        ad.Fill(tb);
                        cmd.Dispose();
                        con.Close();
                        con.Dispose();
                        return tb;

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"查询表：SweepRecord失败：" + ex.Message);
                return null;

            }
        }

        public int DetectVirus(string sample)
        {
            try
            {
                string sql = $"SELECT * FROM {VirusDB} WHERE SampleName = '{sample}';";

                if (_SQLiteConn.State != System.Data.ConnectionState.Open)
                {
                    OpenDb();
                }

                using (SQLiteCommand cmd = new SQLiteCommand(sql, _SQLiteConn))
                {

                    int result = cmd.ExecuteNonQuery();

                    CloseDb();

                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"搜索样本病毒库错误：" + ex.Message);
            }
        }

        //插入病毒样本数据
        public void InsertSampleData(string sample)
        {
            SQLiteHelper.Instance.OpenDb();

            // 插入数据
            string commandStr = $"INSERT OR IGNORE INTO VirusSample (SampleName) VALUES (@SampleName)";

            using (SQLiteCommand insertDataCommand = new SQLiteCommand(commandStr, SQLiteHelper.Instance._SQLiteConn))
            {
                insertDataCommand.Parameters.AddWithValue("@SampleName", sample); // 设置参数值，避免SQL注入

                insertDataCommand.ExecuteNonQuery(); // 执行插入数据的SQL语句
            }

            SQLiteHelper.Instance.CloseDb();
        }

        //插入搜索记录
        public void InsertData(string time, string fileName, string filePath, string type)
        {
            SQLiteHelper.Instance.OpenDb();

            // 插入数据
            string commandStr = $"insert into SweepRecord(Time,FileName,FilePath,Type) values(@Time,@FileName,@FilePath,@Type)";
            using (SQLiteCommand insertDataCommand = new SQLiteCommand(commandStr, SQLiteHelper.Instance._SQLiteConn))
            {
                insertDataCommand.Parameters.AddWithValue("@Time", time); // 设置参数值，避免SQL注入
                insertDataCommand.Parameters.AddWithValue("@FileName", fileName); // 设置参数值，避免SQL注入
                insertDataCommand.Parameters.AddWithValue("@FilePath", filePath); // 设置参数值，避免SQL注入
                insertDataCommand.Parameters.AddWithValue("@Type", type); // 设置参数值，避免SQL注入

                insertDataCommand.ExecuteNonQuery(); // 执行插入数据的SQL语句
            }

            SQLiteHelper.Instance.CloseDb();

        }

        /// <summary>
        /// 打开当前数据库的连接
        /// </summary>
        /// <returns></returns>
        public Boolean OpenDb()
        {
            try
            {
                this._SQLiteConn = new SQLiteConnection(this._SQLiteConnString);
                this._SQLiteConn.Open();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("打开数据库：" + _dbName + "的连接失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void CloseDb()
        {
            if (this._SQLiteConn != null && this._SQLiteConn.State != ConnectionState.Closed)
            {
                if (this._IsRunTrans && this._AutoCommit)
                {
                    this.Commit();
                }
                this._SQLiteConn.Close();
                //this._SQLiteConn = null;
            }
        }

        /// <summary>
        /// 开始数据库事务
        /// </summary>
        public void BeginTransaction()
        {
            this._SQLiteConn.BeginTransaction();
            this._IsRunTrans = true;
        }

        /// <summary>
        /// 开始数据库事务
        /// </summary>
        /// <param name="isoLevel">事务锁级别</param>
        public void BeginTransaction(IsolationLevel isoLevel)
        {
            this._SQLiteConn.BeginTransaction(isoLevel);
            this._IsRunTrans = true;
        }

        /// <summary>
        /// 提交当前挂起的事务
        /// </summary>
        public void Commit()
        {
            if (this._IsRunTrans)
            {
                this._SQLiteTrans.Commit();
                this._IsRunTrans = false;
            }
        }
    }
}
