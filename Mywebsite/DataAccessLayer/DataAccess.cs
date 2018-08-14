using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;

namespace DataAccessLayer

{
    public static class DataAccess
    {
        static string conString = ConfigurationManager.ConnectionStrings["SqlConnectionString"].ToString();

        public static DataSet GetDataSet(string ProcNameORQuery, CommandType commandType, SqlParameter[] mySqlParam = null)
        {
            SqlConnection mycon = new SqlConnection(conString);
            SqlCommand mycmd = new SqlCommand();
            mycmd.CommandType = commandType;
            mycmd.Connection = mycon;
            mycmd.CommandText = ProcNameORQuery;
            mycmd.CommandTimeout = 0;
            if (mySqlParam != null)
                mycmd.Parameters.AddRange(mySqlParam);
            if (mycon.State == ConnectionState.Closed)
                mycon.Open();
            SqlDataAdapter myda = new SqlDataAdapter(mycmd);
            DataSet myds = new DataSet();
            myda.Fill(myds);
            if (mycon.State == ConnectionState.Open)
                mycon.Close();
            return myds;
        }

        public static DataTable GetDataTable(string ProcNameORQuery, CommandType commandType, Hashtable ht = null)
        {
            SqlParameter[] mySqlParam = null;

            if (ht != null)
                mySqlParam = GetSqlParametersfromHash(ht);

            DataTable mydt = new DataTable();
            SqlConnection mycon = new SqlConnection(conString);
            SqlCommand mycmd = new SqlCommand();
            mycmd.CommandType = commandType;
            mycmd.Connection = mycon;
            mycmd.CommandText = ProcNameORQuery;
            mycmd.CommandTimeout = 0;
            if (mySqlParam != null)
                mycmd.Parameters.AddRange(mySqlParam);
            if (mycon.State == ConnectionState.Closed)
                mycon.Open();
            SqlDataAdapter myda = new SqlDataAdapter(mycmd);
            myda.Fill(mydt);
            if (mycon.State == ConnectionState.Open)
                mycon.Close();
            return mydt;
        }

        public static string ExecuteDMLQuery(string ProcNameORQuery, CommandType commandType, Hashtable ht)
        {
            SqlParameter[] mySqlParam = null;

            if (ht != null)
            {
                mySqlParam = GetSqlParametersfromHash(ht);
                SqlParameter objSqlParameter = new SqlParameter("FLAG", SqlDbType.VarChar, 4000);
                objSqlParameter.Direction = ParameterDirection.Output;
                Array.Resize(ref mySqlParam, mySqlParam.Length + 1);
                mySqlParam[mySqlParam.Length - 1] = objSqlParameter;
            }

            SqlConnection mycon = new SqlConnection(conString);
            SqlCommand mycmd = new SqlCommand();
            mycmd.CommandType = commandType;
            mycmd.Connection = mycon;
            mycmd.CommandText = ProcNameORQuery;
            mycmd.CommandTimeout = 0;
            if (mySqlParam != null)
                mycmd.Parameters.AddRange(mySqlParam);
            if (mycon.State == ConnectionState.Closed)
                mycon.Open();

            mycmd.ExecuteNonQuery();

            string retvalue = "";
            if (mycmd.Parameters.Contains("FLAG"))
                retvalue = mycmd.Parameters["FLAG"].Value.ToString();

            mycmd.Cancel();
            mycmd.Dispose();
            if (mycon.State == ConnectionState.Open)
                mycon.Close();

            return retvalue;
        }

        public static object ExecuteScaler(string ProcNameORQuery, CommandType commandType, SqlParameter[] mySqlParam = null)
        {
            object retValue;
            SqlConnection mycon = new SqlConnection(conString);
            SqlCommand mycmd = new SqlCommand();
            mycmd.CommandType = commandType;
            mycmd.Connection = mycon;
            mycmd.CommandText = ProcNameORQuery;
            if (mySqlParam != null)
                mycmd.Parameters.AddRange(mySqlParam);
            if (mycon.State == ConnectionState.Closed)
                mycon.Open();
            retValue = mycmd.ExecuteScalar();
            if (mycon.State == ConnectionState.Open)
                mycon.Close();

            return retValue;
        }

        public static SqlDataReader GetDataReader(string ProcNameORQuery, CommandType commandType, SqlParameter[] mySqlParam = null)
        {
            SqlConnection mycon = new SqlConnection(conString);
            SqlCommand mycmd = new SqlCommand();
            mycmd.CommandType = commandType;
            mycmd.Connection = mycon;
            mycmd.CommandText = ProcNameORQuery;
            if (mySqlParam != null)
                mycmd.Parameters.AddRange(mySqlParam);
            if (mycon.State == ConnectionState.Closed)
                mycon.Open();
            SqlDataReader mydr = mycmd.ExecuteReader(CommandBehavior.CloseConnection);
            return mydr;
        }
        public static SqlParameter[] GetSqlParametersfromHash(Hashtable HT)
        {
            SqlParameter[] param = new SqlParameter[HT.Keys.Count];
            int index = 0;
            foreach (DictionaryEntry item in HT)
            {
                param[index] = new SqlParameter("@" + item.Key.ToString(), item.Value);
                index++;
            }

            return param;

        }

        public static string GetCurrectDateTime(string DateTimeFormat)
        {
            string qstr = "SELECT GETDATE()";
            object cDateTime = ExecuteScaler(qstr, CommandType.Text);
            return string.Format("{0:" + DateTimeFormat + "}", cDateTime);
        }
        public static System.Collections.ArrayList GetObjectArraylist(Type type, DataTable dataTable)
        {
            System.Collections.ArrayList arrayList = new System.Collections.ArrayList();

            foreach (DataRow row in dataTable.Rows)
            {
                object TargetObject = type.GetConstructor(System.Type.EmptyTypes).Invoke(null);
                FillObject(dataTable.Columns, row, TargetObject);

                arrayList.Add(TargetObject);
            }
            return arrayList;
        }
        public static void FillObject(DataColumnCollection columns, DataRow row, object tagetObject)
        {
            Type type = tagetObject.GetType();

            foreach (DataColumn column in columns)
            {
                string fieldName = column.ColumnName;	//FieldName
                Type fieldType;						//FieldType

                if (type.GetProperty(column.ColumnName) != null)
                {
                    fieldType = tagetObject.GetType().GetProperty(column.ColumnName).PropertyType;

                    if (fieldType.Equals(typeof(Int32)))
                    {
                        if (row[fieldName] == DBNull.Value)
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue(tagetObject, 0, new object[0]);
                        else
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue
                                (tagetObject, Int32.Parse(row[fieldName].ToString()), new object[0]);
                    }
                    else if (fieldType.Equals(typeof(string)))
                    {
                        if (row[fieldName] == DBNull.Value)
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue(tagetObject, String.Empty, new object[0]);
                        else
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue
                                (tagetObject, row[fieldName].ToString(), new object[0]);
                    }
                    else if (fieldType.Equals(typeof(float)))
                    {
                        if (row[fieldName] == DBNull.Value)
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue(tagetObject, 0, new object[0]);
                        else
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue
                                (tagetObject, float.Parse(row[fieldName].ToString()), new object[0]);
                    }
                    else if (fieldType.Equals(typeof(bool)))
                    {
                        if (row[fieldName] == DBNull.Value)
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue(tagetObject, false, new object[0]);
                        else
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue
                                (tagetObject, bool.Parse(row[fieldName].ToString()), new object[0]);
                    }
                    else if (fieldType.Equals(typeof(DateTime)))
                    {
                        if (row[fieldName] == DBNull.Value)
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue(tagetObject, DateTime.Parse("1/1/1900"), new object[0]);
                        else
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue
                                (tagetObject, DateTime.Parse(row[fieldName].ToString()), new object[0]);
                    }
                    else if (fieldType.Equals(typeof(double)))
                    {
                        if (row[fieldName] == DBNull.Value)
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue(tagetObject, 0, new object[0]);
                        else
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue
                                (tagetObject, double.Parse(row[fieldName].ToString()), new object[0]);
                    }
                    else if (fieldType.Equals(typeof(Int16)))
                    {
                        if (row[fieldName] == DBNull.Value)
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue(tagetObject, 0, new object[0]);
                        else
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue
                                (tagetObject, Int16.Parse(row[fieldName].ToString()), new object[0]);
                    }
                    else if (fieldType.Equals(typeof(Int64)))
                    {
                        if (row[fieldName] == DBNull.Value)
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue(tagetObject, 0, new object[0]);
                        else
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue
                                (tagetObject, Int64.Parse(row[fieldName].ToString()), new object[0]);
                    }
                    else if (fieldType.Equals(typeof(Decimal)))
                    {
                        if (row[fieldName] == DBNull.Value)
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue(tagetObject, 0, new object[0]);
                        else
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue
                                (tagetObject, Decimal.Parse(row[fieldName].ToString()), new object[0]);
                    }
                    else if (fieldType.Equals(typeof(char)))
                    {
                        if (row[fieldName] == DBNull.Value)
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue(tagetObject, ' ', new object[0]);
                        else
                            tagetObject.GetType().GetProperty(column.ColumnName).SetValue
                                (tagetObject, row[fieldName].ToString().ToCharArray()[0], new object[0]);
                    }
                }
            }
        }

}//end class
}//end namespace