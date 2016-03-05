﻿using System.Data.SQLite;
using Tortuga.Chain.DataSources;
using Tortuga.Chain.SQLite.SQLite.CommandBuilders;

namespace Tortuga.Chain.SQLite
{
    /// <summary>
    /// Base class that represents a SQLite Datasource.
    /// </summary>
    public abstract class SQLiteDataSourceBase : DataSource<SQLiteCommand, SQLiteParameter>
    {
        /// <summary>
        /// Gets the database metadata.
        /// </summary>
        /// <value>The database metadata.</value>
        public abstract SQLiteMetadataCache DatabaseMetadata { get; }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="lockType">Type of the lock.</param>
        /// <returns>SQLiteSqlCall.</returns>
        public SQLiteSqlCall Sql(string sqlStatement, LockType lockType)
        {
            return new SQLiteSqlCall(this, sqlStatement, null, lockType);
        }

        /// <summary>
        /// Creates a operation based on a raw SQL statement.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="lockType">Type of the lock.</param>
        /// <returns>SQLiteSqlCall.</returns>
        public SQLiteSqlCall Sql(string sqlStatement, object argumentValue, LockType lockType)
        {
            return new SQLiteSqlCall(this, sqlStatement, argumentValue, lockType);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <returns></returns>
        public SQLiteTableOrView From(string tableOrViewName)
        {
            return new SQLiteTableOrView(this, tableOrViewName, null, null);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public SQLiteTableOrView From(string tableOrViewName, string whereClause)
        {
            return new SQLiteTableOrView(this, tableOrViewName, whereClause, null);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="whereClause"></param>
        /// <param name="argumentValue"></param>
        /// <returns></returns>
        public SQLiteTableOrView From(string tableOrViewName, string whereClause, object argumentValue)
        {
            return new SQLiteTableOrView(this, tableOrViewName, whereClause, argumentValue);
        }

        /// <summary>
        /// Creates a <see cref="SQLiteTableOrView" /> used to directly query a table or view
        /// </summary>
        /// <param name="tableOrViewName"></param>
        /// <param name="filterValue"></param>
        /// <returns></returns>
        public SQLiteTableOrView From(string tableOrViewName, object filterValue)
        {
            return new SQLiteTableOrView(this, tableOrViewName, filterValue);
        }

        /// <summary>
        /// Inserts the specified table name.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <returns>SQLiteInsertObject.</returns>
        public SQLiteInsertObject Insert(string tableName, object argumentValue)
        {
            return new SQLiteInsertObject(this, tableName, argumentValue);
        }

        /// <summary>
        /// Deletes the specified table name.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="argumentValue">The argument value.</param>
        /// <param name="options">The options.</param>
        /// <returns>SQLiteDeleteObject.</returns>
        public SQLiteDeleteObject Delete(string tableName, object argumentValue, DeleteOptions options = DeleteOptions.None)
        {
            return new SQLiteDeleteObject(this, tableName, argumentValue, options);
        }
    }
}