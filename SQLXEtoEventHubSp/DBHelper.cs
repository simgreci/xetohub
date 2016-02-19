﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Xml;
using System.IO;

namespace SQLXEtoEventHubSp
{
    public class DBHelper
    {
        public static bool XESessionExist(string sessionName)
        {
            bool exists = false;
            using (SqlConnection conn = new SqlConnection("Server=localhost;Trusted_Connection=True;"))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.Parameters.Add(BuildSqlParam("session_name", sessionName));
                    cmd.CommandText = "select 1 from sys.dm_xe_sessions where name = @session_name";
                    cmd.CommandType = System.Data.CommandType.Text;
                    conn.Open();
                    if (cmd.ExecuteScalar() != null)
                        exists = true;
                }
            }
            return exists;
        }

        public static XESession GetSession(string sessionName)
        {
            using (SqlConnection conn = new SqlConnection("Server=localhost;Trusted_Connection=True;"))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Parameters.Add(BuildSqlParam("session_name", sessionName));
                    cmd.Connection = conn;
                    cmd.CommandText = @"select s.address, s.name, t.target_data 
                                from sys.dm_xe_sessions s
                                inner
                                join sys.dm_xe_session_targets t on s.address = t.event_session_address
                                where s.name = @session_name
                                and t.target_name = 'event_file'
                                ";
                    cmd.CommandType = System.Data.CommandType.Text;
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                        throw new Exception(sessionName);

                    reader.Read();

                    return new XESession(
                        reader["name"].ToString(),
                        ExtractXESessionFilePath(Convert.ToString(reader["target_data"]))
                        );
                }
            }
        }

        private static string ExtractXESessionFilePath(string targetData)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(targetData);
            FileInfo f = new FileInfo(doc.SelectSingleNode("EventFileTarget/File").Attributes["name"].Value);
            return f.DirectoryName;
        }

        private static SqlParameter BuildSqlParam(string paramName, object value)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = paramName;
            if (value.Equals(null))
                param.Value = DBNull.Value;
            else
                param.Value = value;
            return param;
        }
    }
}