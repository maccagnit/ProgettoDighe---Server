﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigheDBTest.Database
{
    class Table
    {
        private string Name;
        private SqlConnection connection = DatabaseInstance.GetConnection();

        public Table(string name)
        {
            this.Name = name;
        }

        protected string Insert(List<string> columnNames, List<string> values, bool getIdentity = true)
        {
            SqlCommand command = connection.CreateCommand();
            string commandText = "INSERT INTO " + Name + " (";
            foreach (string column in columnNames)
                commandText += column + ", ";
            commandText = commandText.Substring(0, commandText.Length - 2);
            commandText += $") " + (getIdentity ? "output INSERTED.ID " : "") + " VALUES(";
            foreach (string value in values)
                commandText += "'" + value + "'" + ", ";
            commandText = commandText.Substring(0, commandText.Length - 2);
            commandText += ")";
            command.CommandText = commandText;
            
            return getIdentity ? command.ExecuteScalar().ToString() : command.ExecuteNonQuery().ToString();
        }

        protected bool Exists(string columnName, string value)
        {
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * from " + Name + " WHERE " + columnName + " = '" + value + "'";
            bool exists = false;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                exists = reader.Read();
            }
            return exists;
        }

        protected SqlDataReader LoadData(string columnName, string id)
        {
            if (!Exists(columnName, id))
                throw new Exception("Non esiste la riga");
            SqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT * from " + Name + " WHERE " + columnName + " = '" + id + "'";
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            return reader;
        }
    }
}
