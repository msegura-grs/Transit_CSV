using System;
using MySql.Data.MySqlClient;
using System.Data;

namespace ordersNTransfers
{
    class MySQLobj
    {
        static string connectionString;
        static string cnnStatus;

        public MySQLobj(Boolean _test)
        {
            string database;

            if (_test) { database = ordersNTransfers.Properties.Resources.MSdbT; }
            else { database = ordersNTransfers.Properties.Resources.MSdbP; }

            MySQLobj.connectionString = $"Persist Security Info = False; database= {database}; server= {ordersNTransfers.Properties.Resources.MSsrv}; user id= {ordersNTransfers.Properties.Resources.MSui}; Password = {ordersNTransfers.Properties.Resources.MSpwd}; Port= {ordersNTransfers.Properties.Resources.MSprt}";
            MySQLobj.cnnStatus = "OK";
        }

        public static string getManufacturerSupplyCode(string _connectionString, string _sp, string _compania, string _proveedor)
        {
            DataTable dt = new DataTable();
            string resultado = "";

            try
            {
                using (MySqlConnection sqlCon = new MySqlConnection(_connectionString))
                {
                    using (MySqlDataAdapter SqlDa = new MySqlDataAdapter())
                    {
                        sqlCon.Open();

                        using (MySqlCommand cmm = new MySqlCommand(_sp, sqlCon))
                        {
                            cmm.CommandType = CommandType.StoredProcedure;
                            cmm.Parameters.AddWithValue("_pais", _compania);
                            cmm.Parameters.AddWithValue("_display_name", _proveedor);
                            SqlDa.SelectCommand = cmm;
                            SqlDa.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                resultado = dt.Rows[0]["sap_supplierCode"].ToString();
                            }
                        }
                        sqlCon.Close();
                    }
                }

                MySQLobj.cnnStatus = "Procedimiento " + _sp + " ejecutado con exito.";
                return resultado;
            }
            catch (Exception ex)
            {
                MySQLobj.cnnStatus = "Ocurrio un problema al ejecutar [" + _sp + "]: " + ex.Message;
                dt.Dispose();
                return null;
            }
        }

        /*
        public static DataTable getInfo(string _connectionString, string _sp, string _compania)
        {
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection sqlCon = new MySqlConnection(_connectionString))
                {
                    using (MySqlDataAdapter SqlDa = new MySqlDataAdapter())
                    {
                        sqlCon.Open();

                        using (MySqlCommand cmm = new MySqlCommand(_sp, sqlCon))
                        {
                            cmm.CommandType = CommandType.StoredProcedure;
                            cmm.Parameters.AddWithValue ("_pais", _compania);
                            SqlDa.SelectCommand = cmm;
                            SqlDa.Fill(dt);
                        }
                        sqlCon.Close();
                    }
                }
                MySQLobj.cnnStatus = "Procedimiento " + _sp + " ejecutado con exito. " + _compania;
                return dt;
            }
            catch (Exception ex)
            {
                MySQLobj.cnnStatus = "Ocurrio un problema al ejecutar [" + _sp + "]: " + ex.Message;
                dt.Dispose();
                return null;
            }
        }

        public static DataTable getInfo(string _connectionString, string _sp, int _id_recibo, string _compania)
        {
            DataTable dt = new DataTable();

            try
            {
                using (MySqlConnection sqlCon = new MySqlConnection(_connectionString))
                {
                    using (MySqlDataAdapter SqlDa = new MySqlDataAdapter())
                    {
                        sqlCon.Open();

                        using (MySqlCommand cmm = new MySqlCommand(_sp, sqlCon))
                        {
                            cmm.CommandType = CommandType.StoredProcedure;
                            cmm.Parameters.AddWithValue("_pais", _compania);
                            cmm.Parameters.AddWithValue("_id_recibo", _id_recibo);
                            SqlDa.SelectCommand = cmm;
                            SqlDa.Fill(dt);
                        }
                        sqlCon.Close();
                    }
                }
                MySQLobj.cnnStatus = "Procedimiento " + _sp + " ejecutado con exito. " + _compania;
                return dt;
            }
            catch (Exception ex)
            {
                MySQLobj.cnnStatus = "Ocurrio un problema al ejecutar [" + _sp + "]: " + ex.Message;
                dt.Dispose();
                return null;
            }
        }

        public static Boolean putInfo(string _connectionString, string _sp, int _id_recibo, string _compania, int _reciboSAP)
        {
            try
            {
                using (MySqlConnection sqlCon = new MySqlConnection(_connectionString))
                {
                    using (MySqlDataAdapter SqlDa = new MySqlDataAdapter())
                    {
                        sqlCon.Open();

                        using (MySqlCommand cmm = new MySqlCommand(_sp, sqlCon))
                        {
                            cmm.CommandType = CommandType.StoredProcedure;
                            cmm.Parameters.AddWithValue("_pais", _compania);
                            cmm.Parameters.AddWithValue("_id", _id_recibo);
                            cmm.Parameters.AddWithValue("_DocNum", _reciboSAP);
                            cmm.ExecuteNonQuery();
                        }
                        sqlCon.Close();
                    }
                }
                MySQLobj.cnnStatus = "Procedimiento " + _sp + " ejecutado con exito. " + _compania;
                return true;
            }
            catch (Exception ex)
            {
                MySQLobj.cnnStatus = "Ocurrio un problema al ejecutar [" + _sp + "]: " + ex.Message;
                return false;
            }
        }

        public static Boolean putInfo(string _connectionString, string _sp, int _id_articulo, string _compania, string _articuloSAP, string _CRM_errorTraslado)
        {
            try
            {
                using (MySqlConnection sqlCon = new MySqlConnection(_connectionString))
                {
                    using (MySqlDataAdapter SqlDa = new MySqlDataAdapter())
                    {
                        sqlCon.Open();

                        using (MySqlCommand cmm = new MySqlCommand(_sp, sqlCon))
                        {
                            cmm.CommandType = CommandType.StoredProcedure;
                            cmm.Parameters.AddWithValue("_pais", _compania);
                            cmm.Parameters.AddWithValue("_id", _id_articulo);

                            if (_articuloSAP.Length > 0) { cmm.Parameters.AddWithValue("_sap_itemcode", _articuloSAP); }
                            else { cmm.Parameters.AddWithValue("_sap_error", _CRM_errorTraslado); }

                            cmm.ExecuteNonQuery();
                        }
                        sqlCon.Close();
                    }
                }
                MySQLobj.cnnStatus = "Procedimiento " + _sp + " ejecutado con exito. " + _compania;
                return true;
            }
            catch (Exception ex)
            {
                MySQLobj.cnnStatus = "Ocurrio un problema al ejecutar [" + _sp + "]: " + ex.Message;
                return false;
            }
        }
        */

        public static string ConnectionString { get => connectionString; set => connectionString = value; }

        public static string CnnStatus { get => cnnStatus; set => cnnStatus = value; }

    }
}