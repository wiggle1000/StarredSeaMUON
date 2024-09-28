using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Database
{
    internal class SqliteCommandBuilder
    {
        public SqliteCommand command;
        /// <summary>
        /// note to self: always pass user strings as a param to avoid injection!
        /// </summary>
        /// <param name="commandString"></param>
        public SqliteCommandBuilder(SqliteConnection connection, string commandString)
        {
            command = connection.CreateCommand();
            command.CommandText = commandString;
        }

        public static implicit operator SqliteCommand(SqliteCommandBuilder b) => b.command;

        public SqliteCommandBuilder WithTextParam(string paramName, string arg)
        {
            command.Parameters.Add(paramName, SqliteType.Text).Value = arg;
            return this;
        }
        public SqliteCommandBuilder WithIntParam(string paramName, long arg)
        {
            command.Parameters.Add(paramName, SqliteType.Integer).Value = arg;
            return this;
        }
        public SqliteCommandBuilder WithRealParam(string paramName, double arg)
        {
            command.Parameters.Add(paramName, SqliteType.Real).Value = arg;
            return this;
        }
        public SqliteCommandBuilder WithTimeParam(string paramName, DateTime arg)
        {
            command.Parameters.Add(paramName, SqliteType.Text).Value = TimeToString(arg);
            return this;
        }
        public static DateTime StringToTime(string time)
        {
            return DateTime.ParseExact(time, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }
        public static string TimeToString(DateTime time)
        {
            return time.ToUniversalTime().ToString("O");
        }
        public SqliteCommandBuilder WithBlobParam(string paramName, object arg)
        {
            command.Parameters.Add(paramName, SqliteType.Blob).Value = arg;
            return this;
        }
    }
}
