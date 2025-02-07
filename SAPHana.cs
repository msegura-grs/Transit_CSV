using System;
using SAPbobsCOM;
using System.Data;
using System.Data.Odbc;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1;

namespace ordersNTransfers
{
    class SAPHana
    {
        private static Company sapCompany;
        private static Documents sapPurchaseOrder;
        private static Documents sapOrder;
        private static BusinessPartners sapBP;
        private static BusinessPartners sapBPC;
        private static int lRetCode;
        private static string sErrMsg;
        private static string sCnnStatus;
        private static int lErrCode;
        private static bool conectado;
        private static string estado;
        public static string Estado { get => estado; set => estado = value; }


        #region "Constructor->conexion y desconexión del sistema HANA"
        /// <summary>
        /// inicializa conexión con SAP HANA DB
        /// </summary>
        /// <paramref name="_compania">empresa con que desea realizar el intercambio de información
        /// <paramref name="_test">ambiente de pruebas
        public SAPHana(string _compania, Boolean _test)
        {
            SAPHana.conectado = false;
            SAPHana.sapCompany = new Company
            {
                DbServerType = BoDataServerTypes.dst_HANADB,
                Server = ordersNTransfers.Properties.Resources.SHSVR + ":" + ordersNTransfers.Properties.Resources.SHPRT,
                language = BoSuppLangs.ln_Spanish,
                UseTrusted = false,
                DbUserName = "SYSTEM",
                DbPassword = "Aez1NtM6"
            };

            switch (_compania)
            {
                case "GRS-GR": // Global Reach
                    if (_test)
                    {
                        SAPHana.sapCompany.CompanyDB = "TEST_GLOBALREACH";
                        SAPHana.sapCompany.UserName = "AdmOMS";
                        SAPHana.sapCompany.Password = "FFVu7N6s7s";
                    }
                    else
                    {
                        SAPHana.sapCompany.CompanyDB = "GLOBALREACH";
                        SAPHana.sapCompany.UserName = "AdmOMS";          // Francisco Alvarez 
                        SAPHana.sapCompany.Password = "FFVu7N6s7s";
                    }
                    break;

                case "GRS-CD": // Centro de distribución
                    if (_test)
                    {
                        SAPHana.sapCompany.CompanyDB = "";
                        SAPHana.sapCompany.UserName = "";
                        SAPHana.sapCompany.Password = "";
                    }
                    else
                    {
                        SAPHana.sapCompany.CompanyDB = "DISPOR";
                        SAPHana.sapCompany.UserName = "";
                        SAPHana.sapCompany.Password = "";
                    }
                    break;

                case "GRS-HN": // Honduras
                    if (_test)
                    {
                        SAPHana.sapCompany.CompanyDB = "";
                        SAPHana.sapCompany.UserName = ""; // 
                        SAPHana.sapCompany.Password = "";
                    }
                    else
                    {
                        SAPHana.sapCompany.CompanyDB = "ALCANCE_HN";
                        SAPHana.sapCompany.UserName = "AdmOMS"; // "HNSCM"; // scm Honduras
                        SAPHana.sapCompany.Password = "FFVu7N6s7s"; // "GRS2013";
                    }
                    break;

                case "GRS-GT": // Guatemala
                    if (_test)
                    {
                        SAPHana.sapCompany.CompanyDB = "TEST_GT";
                        SAPHana.sapCompany.UserName = "AdmOMS";
                        SAPHana.sapCompany.Password = "FFVu7N6s7s";
                    }
                    else
                    {
                        SAPHana.sapCompany.CompanyDB = "ALCANCE_GT";
                        SAPHana.sapCompany.UserName = "AdmOMS"; // "LSCMO"; // scm Guatemala
                        SAPHana.sapCompany.Password = "FFVu7N6s7s"; // "Grs2019";
                    }
                    break;

                case "GRS-ES": // El Salvador
                    if (_test)
                    {
                        SAPHana.sapCompany.CompanyDB = "";
                        SAPHana.sapCompany.UserName = "";
                        SAPHana.sapCompany.Password = "";
                    }
                    else
                    {
                        SAPHana.sapCompany.CompanyDB = "ALCANCE_ES";
                        SAPHana.sapCompany.UserName = "AdmOMS"; // "HNSCM"; // scm Honduras
                        SAPHana.sapCompany.Password = "FFVu7N6s7s"; // "GRS2013";
                    }
                    break;

                case "GRS-CR": // Costa Rica
                    if (_test)
                    {
                        SAPHana.sapCompany.CompanyDB = "";
                        SAPHana.sapCompany.UserName = "";
                        SAPHana.sapCompany.Password = "";
                    }
                    else
                    {
                        SAPHana.sapCompany.CompanyDB = "ALCANCE_CR";
                        SAPHana.sapCompany.UserName = "AdmOMS"; // "LSCMO"; // scm Guatemala
                        SAPHana.sapCompany.Password = "FFVu7N6s7s"; // "Grs2023"; 
                    }
                    break;

                case "GRS-NI": // Nicaragua 
                    if (_test)
                    {
                        SAPHana.sapCompany.CompanyDB = "";
                        SAPHana.sapCompany.UserName = "";
                        SAPHana.sapCompany.Password = "";
                    }
                    else
                    {
                        SAPHana.sapCompany.CompanyDB = "ALCANCE_NI";
                        SAPHana.sapCompany.UserName = "AdmOMS"; // "LSCMO"; // scm Guatemala
                        SAPHana.sapCompany.Password = "FFVu7N6s7s"; // "Grs-2017";
                    }
                    break;

                case "GRS-USA": //Estados Unidos
                    if (_test)
                    {
                        SAPHana.sapCompany.CompanyDB = "";
                        SAPHana.sapCompany.UserName = "";
                        SAPHana.sapCompany.Password = "";
                    }
                    else
                    {
                        SAPHana.sapCompany.CompanyDB = "GRS_USA";
                        SAPHana.sapCompany.UserName = "AdmOMS";
                        SAPHana.sapCompany.Password = "FFVu7N6s7s";
                    }
                    break;
                case "DST-GT": //Distriterra gt
                    if (_test)
                    {
                        SAPHana.sapCompany.CompanyDB = "TEST_DISTRITERRA";
                        SAPHana.sapCompany.UserName = "GTSDS";
                        SAPHana.sapCompany.Password = "AKLwyn76#$";
                    }
                    else
                    {
                        SAPHana.sapCompany.CompanyDB = "DISTRITERRA_GT";
                        SAPHana.sapCompany.UserName = "GTSDS";
                        SAPHana.sapCompany.Password = "AKLwyn76#$";
                    }
                    break;
            }

            if (SAPHana.sapCompany.CompanyDB.Length != 0)
            {
                SAPHana.lRetCode = SAPHana.sapCompany.Connect();

                if (SAPHana.lRetCode != 0)
                {
                    SAPHana.sapCompany.GetLastError(out SAPHana.lErrCode, out SAPHana.sErrMsg);
                    SCnnStatus = "Fallo en conexión con [" + SAPHana.sapCompany.CompanyDB + "] [" + SAPHana.LErrCode.ToString() + "]: " + SAPHana.SErrMsg;
                }
                if (SAPHana.sapCompany.Connected)
                {
                    SAPHana.LErrCode = 0;
                    SAPHana.SErrMsg = "";
                    SAPHana.conectado = true;
                    SCnnStatus = "Conectado a " + SAPHana.sapCompany.CompanyDB;
                }
            }
            else
            {
                SAPHana.LErrCode = -1;
                SAPHana.SErrMsg = "";
                SCnnStatus = "La compañia [" + _compania + "] no se encuentra configurada en este módulo.";
            }
        }

        public void desconecta()
        {
            if (SAPHana.conectado)
            {
                SAPHana.sapCompany.Disconnect();
                SAPHana.conectado = false;
            }
        }

        #endregion

        #region "gestion de proveedor y cliente"

        /// <summary>
        /// recoge información de cliente para documento de venta
        /// </summary>
        /// <param name="_transit">objeto json con toda la información del transito</param>
        /// <param name="_cardCode">codigo de cliente</param>
        /// <param name="_lg">objeto para registro de eventos</param>
        /// <param name="_compania">operacion involucrada</param>
        /// <param name="_test">true: ambiente de pruebas; false: ambiente productivo</param>
        public bool buscaCliente(JToken _transit, string _cardCode, logObj _lg, string _compania, bool _test, string _incoterm)
        {
            bool resultado = false;
            string query;
            SAPbobsCOM.Recordset rsCustomer;

            try
            {
                if (_cardCode.Length == 0)
                //if (_incoterm != "local")
                {
                    _lg.entryLog($"Codigo de cliente '{_cardCode}' incoterm {_incoterm}");
                    resultado = true;
                }
                else
                {
                    sapBPC = (SAPbobsCOM.BusinessPartners)sapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);
                    query = $"SELECT * FROM \"OCRD\" WHERE \"CardCode\" = trim('{_cardCode}')";
                    rsCustomer = (SAPbobsCOM.Recordset)sapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    rsCustomer.DoQuery(query);

                    if (rsCustomer.RecordCount == 0)
                    {
                        registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), (_compania.Length == 0 ? "GRS-GR" : _compania), 9, $"No se encontro informacion para el cliente {_cardCode}", _test, _lg);
                    }
                    else
                    {
                        sapBPC.Browser.Recordset = rsCustomer;
                        sapBPC.Browser.MoveFirst();
                        _lg.entryLog($"Cliente: {sapBPC.CardCode}: {sapBPC.CardName}");
                        resultado = true;
                    }
                }
            }
            catch (Exception ex)
            {
                SAPHana.LErrCode = -1;
                SAPHana.SErrMsg = $"Ocurrió un problema al consultar el cliente. [{ex.Source}: {ex.Message}]";
                registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _compania, SAPHana.lErrCode, SAPHana.sErrMsg, _test, _lg);
            }

            return resultado;
        }


        /// <summary>
        /// recoge información de proveedor para documento de compra
        /// </summary>
        /// <param name="_transit">objeto json con toda la información del transito</param>
        /// <param name="_Code">codigo de proveedor</param>
        /// <param name="_local">true: Transaccion entre operaciones; false:procedente de China</param>
        /// <param name="_lg">objeto para registro de eventos</param>
        /// <param name="_compania">operacion involucrada</param>
        /// <param name="_test">true: ambiente de pruebas; false: ambiente productivo</param>
        /// <param name="_piNumber">arreglo con información sobre las PI en el transito en proceso</param>
        public bool buscaProveedor(JToken _transit, string _Code, bool _local, logObj _lg, string _compania, bool _test, ref string[,] _piNumber)
        {
            bool resultado = false;
            string query;
            string mensaje;
            SAPbobsCOM.Recordset rsPrv;

            try
            {
                sapBP = (SAPbobsCOM.BusinessPartners)sapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners);

                for (int i = 0; i < _piNumber.GetLength(0); i++)
                {
                    if (_local)
                    { query = $"select * from \"OCRD\" WHERE \"CardCode\" = '{_Code}' LIMIT 1; "; }
                    else
                    //{ query = $"select * from \"OCRD\" WHERE \"IndustryC\" = (SELECT \"IndCode\" FROM \"OOND\" WHERE \"IndName\" = '{_piNumber[i, 1]}' LIMIT 1); "; }
                    { query = $"select * from \"OCRD\" WHERE \"CardCode\" = (SELECT \"CardCode\" FROM \"CRD1\" where \"Address\" = trim('{_piNumber[i, 1]}') LIMIT 1); "; }

                    rsPrv = (SAPbobsCOM.Recordset)sapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    rsPrv.DoQuery(query);

                    if (rsPrv.RecordCount == 0)
                    {
                        if (_local)
                        { mensaje = $"No se encontro informacion para el proveedor {_Code}"; }
                        else
                        { mensaje = $"No se encontro informacion para el proveedor {_piNumber[i, 1]}"; }

                        registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), (_compania.Length == 0 ? "GRS-GR" : _compania), 10, mensaje, _test, _lg);
                    }
                    else
                    {
                        sapBP.Browser.Recordset = rsPrv;
                        sapBP.Browser.MoveFirst();
                        _lg.entryLog($"Proveedor: {sapBP.CardCode} [{_piNumber[i, 1]}]: {sapBP.CardName}");

                        _piNumber[i, 2] = sapBP.CardCode;
                        _piNumber[i, 3] = sapBP.CardName;
                        _piNumber[i, 4] = sapBP.Address;
                        resultado = true;
                    }
                }
            }
            catch (Exception ex)
            {
                SAPHana.LErrCode = -1;
                SAPHana.SErrMsg = $"Ocurrió un problema al consultar el proveedor. [{ex.Source}: {ex.Message}]";
                registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), (_compania.Length == 0 ? "GRS-GR" : _compania), SAPHana.lErrCode, SAPHana.sErrMsg, _test, _lg);
            }

            return resultado;
        }

        #endregion

        #region "Documentos de compra"

        /// <summary>
        /// genera orden de compra
        /// </summary>
        /// <param name="_transit">objeto json con toda la información del transito</param>
        /// <param name="_lineasOrden">arreglo de objetos json con la información de las lineas de detalle</param>
        /// <param name="_serie"></param>
        /// <param name="_lg">objeto para registro de eventos</param>
        /// <param name="_operacion">operacion involucrada</param>
        /// <param name="_test">true: ambiente de pruebas; false: ambiente productivo</param>
        /// <param name="_piNumber">arreglo con información sobre las PI en el transito en proceso</param>
        /// <param name="_posicion">fila de _piNumber con que se esta trabajando</param>
        public double generaOrdenCompraOperacion(JToken _transit, JToken[] _lineasOrden, int _serie, logObj _lg, string _operacion, bool _test, string[,] _piNumber, int _posicion, int _userSAP, DataTable _dtInv)
        {
            int resultado = 0;
            double ordenCompra = -1;
            string query;
            double pesoBruto = 0;
            double pesoNeto = 0;
            bool IsCFRCheck;

            SAPbobsCOM.Recordset rsPed;
            float precioHana;
            bool continuar = true;
            DataRow[] dtRw;
            string codigoArticulo = "";

            try
            {
                sapPurchaseOrder = (SAPbobsCOM.Documents)sapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
                query = $"SELECT * FROM \"OPOR\" WHERE \"CANCELED\" = 'N' AND \"U_Transito_Odoo\" = trim('{_transit["id"]}') AND \"U_Pi_Number\" = trim('{_piNumber[_posicion, 0]}')";

                rsPed = (SAPbobsCOM.Recordset)sapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                rsPed.DoQuery(query);

                if (rsPed.RecordCount != 0)
                {
                    sapPurchaseOrder.Browser.Recordset = rsPed;
                    sapPurchaseOrder.Browser.MoveFirst();
                    ordenCompra = sapPurchaseOrder.DocNum;
                    _lg.entryLog($"Transito ODOO [{_transit["id"]}] {_transit["name"]} en [{SAPHana.sapCompany.CompanyDB}] Orden de compra No. [{sapPurchaseOrder.DocEntry}] {sapPurchaseOrder.DocNum}");
                }
                else
                {
                    sapPurchaseOrder.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    sapPurchaseOrder.CardCode = _piNumber[_posicion, 2];
                    sapPurchaseOrder.CardName = _piNumber[_posicion, 3];
                    sapPurchaseOrder.Address = _piNumber[_posicion, 4];
                    sapPurchaseOrder.SalesPersonCode = _userSAP;
                     sapPurchaseOrder.Series = _serie;

                    if (_piNumber[_posicion, 3].Length > 50)
                    { sapPurchaseOrder.PayToCode = _piNumber[_posicion, 3].Substring(0, 50); }
                    else
                    { sapPurchaseOrder.PayToCode = _piNumber[_posicion, 3]; }

                    sapPurchaseOrder.NumAtCard = _transit["invoice_comercial"].ToString();

                    sapPurchaseOrder.DocDate = DateTime.Now;
                    sapPurchaseOrder.TaxDate = DateTime.Now;
                    sapPurchaseOrder.VatDate = DateTime.Now;
                    sapPurchaseOrder.DocDueDate = DateTime.Parse(_transit["eta"].ToString());
                    sapPurchaseOrder.GroupHandWritten = BoYesNoEnum.tYES;

                    if (_operacion == "GRS-GR")
                    { sapPurchaseOrder.GroupNumber = int.Parse(_piNumber[_posicion, 7].ToString()); }
                    else
                    { sapPurchaseOrder.GroupNumber = int.Parse(_piNumber[_posicion, 8].ToString()); }

                    sapPurchaseOrder.DocType = BoDocumentTypes.dDocument_Items;

                    if (_operacion != "GRS-USA")
                    {
                        sapPurchaseOrder.DocCurrency = "USD";
                    }
                    else
                    {
                        sapPurchaseOrder.DocCurrency = "$";
                    }
                    /*
                     * PI: numero de PI
                     * FACTURA: comercial_invoice
                     * BOOKING: booking_number
                     * BL: bl_number
                     * ETA: eta
                     */

                    IsCFRCheck = _transit["is_cfr_check"].ToObject<bool>();
                    if (IsCFRCheck)
                    {
                        sapPurchaseOrder.Lines.ItemCode = "Flete Maritimo";
                        sapPurchaseOrder.Lines.Quantity = 1;
                        sapPurchaseOrder.Lines.TaxCode = "EXE";
                        sapPurchaseOrder.Lines.VatGroup = "EXE";
                        sapPurchaseOrder.Lines.UnitPrice = double.Parse(_transit["cfr_freight"].ToString());
                        sapPurchaseOrder.Lines.LineTotal = double.Parse(_transit["cfr_freight"].ToString());
                        sapPurchaseOrder.Lines.Add();
                    }


                    sapPurchaseOrder.Comments = $"PI: {_piNumber[_posicion, 0]}\n FACTURA: {_transit["invoice_comercial"]}\nBOOKING: {_transit["booking_number"]}\nBL: {_transit["bl_number"]}\nETA: {_transit["eta"]}";
                    sapPurchaseOrder.UserFields.Fields.Item("U_Transito_Odoo").Value = _transit["id"].ToString();
                    sapPurchaseOrder.UserFields.Fields.Item("U_Transito_Name").Value = _transit["name"].ToString();
                    sapPurchaseOrder.UserFields.Fields.Item("U_FECHA").Value = DateTime.Parse(_transit["eta"].ToString());
                    sapPurchaseOrder.UserFields.Fields.Item("U_NuevoETA").Value = DateTime.Parse(_transit["eta"].ToString());
                    sapPurchaseOrder.UserFields.Fields.Item("U_Guatex").Value = _transit["bl_number"].ToString();
                    sapPurchaseOrder.UserFields.Fields.Item("U_Pi_Number").Value = _piNumber[_posicion, 0];

                    foreach (JToken linea in _lineasOrden)
                    {
                        precioHana = 0;
                        pesoNeto = 0;
                        pesoBruto = 0;

                        if (linea != null)
                        {
                            if (continuar)
                            {
                                if (linea["pi_number"].ToString() == _piNumber[_posicion, 0])
                                {
                                    if (linea["product_id"][0]["name"].ToString() != "Spare Parts")
                                    {
                                        if ((linea["product_country_code"].ToString().Length > 0) && (linea["product_country_code"].ToString() != "False"))
                                        { codigoArticulo = linea["product_country_code"].ToString(); }
                                        else
                                        { codigoArticulo = linea["product_id"][0]["name"].ToString(); }

                                        _lg.entryLog($"Codigo artículo [{codigoArticulo}]");

                                        dtRw = _dtInv.Select($"ItemCode = '{linea["product_id"][0]["name"]}'");

                                        if (dtRw.Length == 0)
                                        {
                                            registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, 11, $"Precio venta de {linea["product_id"][0]["name"]} no disponible", _test, _lg);
                                            continuar = false;
                                        }
                                        else
                                        {
                                            precioHana = float.Parse(dtRw[0]["Price"].ToString());
                                        }
                                    }
                                    else
                                    {
                                        codigoArticulo = linea["product_id"][0]["name"].ToString();
                                    }

                                    if (continuar)
                                    {
                                        _lg.entryLog($"Articulo: {codigoArticulo}");
                                        sapPurchaseOrder.Lines.ItemCode = codigoArticulo;

                                        if (_operacion == "GRS-CR")
                                        { sapPurchaseOrder.Lines.WarehouseCode = "01 B"; }
                                        else
                                        { sapPurchaseOrder.Lines.WarehouseCode = "01"; }

                                        sapPurchaseOrder.Lines.Quantity = double.Parse(linea["product_qty"].ToString());
                                        sapPurchaseOrder.Lines.TaxCode = "EXE";
                                        sapPurchaseOrder.Lines.VatGroup = "EXE";

                                        sapPurchaseOrder.Lines.Volume = 0; //"volumen"

                                        sapPurchaseOrder.Lines.UnitPrice = precioHana;
                                        sapPurchaseOrder.Lines.LineTotal = sapPurchaseOrder.Lines.Quantity * sapPurchaseOrder.Lines.UnitPrice;

                                        sapPurchaseOrder.Lines.UserFields.Fields.Item("U_Contenedor").Value = linea["container_number"].ToString();

                                        /* se deshabilita ya que los campos no estan disponibles en las operaciones
                                         * 
                                        sapPurchaseOrder.Lines.UserFields.Fields.Item("U_Volumen").Value = linea["volumen"].ToString();

                                        if (double.TryParse(linea["total_gross_weight"].ToString(), out pesoBruto))
                                        { sapPurchaseOrder.Lines.UserFields.Fields.Item("U_PesoBruto").Value = Math.Round(pesoBruto, 5).ToString(); } 
                                        else
                                        { _lg.entryLog($"Peso bruto no valido: {linea["total_gross_weight"]}"); }

                                        if (double.TryParse(linea["total_net_weight"].ToString(), out pesoNeto))
                                        { sapPurchaseOrder.Lines.UserFields.Fields.Item("U_PesoNeto").Value = Math.Round(pesoNeto, 5).ToString(); }
                                        else
                                        { _lg.entryLog($"Peso neto no valido: {linea["total_net_weight"]}"); }
                                        */
                                        sapPurchaseOrder.Lines.Add();

                                    }
                                }
                            }
                        }
                    }

                    if (continuar)
                    {

                        resultado = sapPurchaseOrder.Add();

                        if (resultado != 0)
                        {
                            SAPHana.sapCompany.GetLastError(out SAPHana.lErrCode, out SAPHana.sErrMsg);
                            _lg.entryLog(SAPHana.lErrCode.ToString() + ": " + SAPHana.sErrMsg);
                            registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, SAPHana.lErrCode, SAPHana.sErrMsg, _test, _lg);
                        }
                        else
                        {
                            query = $"SELECT * FROM \"OPOR\" WHERE \"CANCELED\" = 'N' AND \"U_Transito_Odoo\" = trim('{_transit["id"]}') AND \"U_Pi_Number\" = trim('{_piNumber[_posicion, 0]}')";
                            rsPed = (SAPbobsCOM.Recordset)sapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            rsPed.DoQuery(query);

                            if (rsPed.RecordCount > 0)
                            {
                                sapPurchaseOrder.Browser.Recordset = rsPed;
                                sapPurchaseOrder.Browser.MoveFirst();
                                ordenCompra = sapPurchaseOrder.DocNum;
                            }
                            _lg.entryLog($"Transito ODOO [{_transit["id"]}] {_transit["name"]} en [{SAPHana.sapCompany.CompanyDB}] Orden de compra No. [{sapPurchaseOrder.DocEntry}] {sapPurchaseOrder.DocNum}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAPHana.LErrCode = -3;
                SAPHana.SErrMsg = $"Ocurrió un problema al construir la Orden de compra. [{ex.Source}: {ex.Message}]";
                registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, SAPHana.lErrCode, SAPHana.sErrMsg, _test, _lg);
            }

            return ordenCompra;
        }

        /// <summary>
        /// genera orden de compra
        /// </summary>
        /// <param name="_transit">objeto json con toda la información del transito</param>
        /// <param name="_lineasOrden">arreglo de objetos json con la información de las lineas de detalle</param>
        /// <param name="_serie"></param>
        /// <param name="_lg">objeto para registro de eventos</param>
        /// <param name="_operacion">operacion involucrada</param>
        /// <param name="_test">true: ambiente de pruebas; false: ambiente productivo</param>
        /// <param name="_piNumber">arreglo con información sobre las PI en el transito en proceso</param>
        /// <param name="_posicion">fila de _piNumber con que se esta trabajando</param>
        public double generaOrdenCompra(JToken _transit, JToken[] _lineasOrden, int _serie, logObj _lg, string _operacion, bool _test, string[,] _piNumber, int _posicion, int _userSAP)
        {
            int resultado = 0;
            double ordenCompra = -1;
            double pesoBruto = 0;
            double pesoNeto = 0;
            string query;
            bool IsCFRCheck;
            SAPbobsCOM.Recordset rsPed;
            bool procesarLinea;

            try
            {
                sapPurchaseOrder = (SAPbobsCOM.Documents)sapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders);
                query = $"SELECT * FROM \"OPOR\" WHERE \"CANCELED\" = 'N' AND \"U_Transito_Odoo\" = trim('{_transit["id"]}') AND \"U_Pi_Number\" = trim('{_piNumber[_posicion, 0]}')";

                rsPed = (SAPbobsCOM.Recordset)sapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                rsPed.DoQuery(query);

                if (rsPed.RecordCount != 0)
                {
                    sapPurchaseOrder.Browser.Recordset = rsPed;
                    sapPurchaseOrder.Browser.MoveFirst();
                    ordenCompra = sapPurchaseOrder.DocNum;
                    _lg.entryLog($"Transito ODOO [{_transit["id"]}] {_transit["name"]} en [{SAPHana.sapCompany.CompanyDB}] Orden de compra No. [{sapPurchaseOrder.DocEntry}] {sapPurchaseOrder.DocNum}");
                }
                else
                {
                    sapPurchaseOrder.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    sapPurchaseOrder.CardCode = _piNumber[_posicion, 2];
                    sapPurchaseOrder.CardName = _piNumber[_posicion, 3];
                    sapPurchaseOrder.Address = _piNumber[_posicion, 4];
                    sapPurchaseOrder.SalesPersonCode = _userSAP;
                     sapPurchaseOrder.Series = _serie;

                    if (_piNumber[_posicion, 3].Length > 50)
                    { sapPurchaseOrder.PayToCode = _piNumber[_posicion, 3].Substring(0, 50); }
                    else
                    { sapPurchaseOrder.PayToCode = _piNumber[_posicion, 3]; }

                    sapPurchaseOrder.NumAtCard = _transit["invoice_comercial"].ToString();

                    sapPurchaseOrder.DocDate = DateTime.Now;
                    sapPurchaseOrder.TaxDate = DateTime.Now;
                    sapPurchaseOrder.VatDate = DateTime.Now;
                    sapPurchaseOrder.DocDueDate = DateTime.Parse(_transit["eta"].ToString());
                    sapPurchaseOrder.GroupHandWritten = BoYesNoEnum.tYES;

                    if (_operacion == "GRS-GR")
                    { sapPurchaseOrder.GroupNumber = int.Parse(_piNumber[_posicion, 7].ToString()); }
                    else
                    { sapPurchaseOrder.GroupNumber = int.Parse(_piNumber[_posicion, 8].ToString()); }

                    sapPurchaseOrder.DocType = BoDocumentTypes.dDocument_Items;

                    if (_operacion != "GRS-USA")
                    {
                        sapPurchaseOrder.DocCurrency = "USD";
                    }
                    if (_operacion == "GRS-USA")
                    {
                        sapPurchaseOrder.DocCurrency = "USD";
                    }

                    IsCFRCheck = _transit["is_cfr_check"].ToObject<bool>();
                    if (IsCFRCheck) {

                        
                        sapPurchaseOrder.Lines.ItemCode = "Flete Maritimo";
                        sapPurchaseOrder.Lines.Quantity = 1;
                        sapPurchaseOrder.Lines.TaxCode = "EXE";
                        sapPurchaseOrder.Lines.VatGroup = "EXE";
                        sapPurchaseOrder.Lines.UnitPrice = double.Parse(_transit["cfr_freight"].ToString());
                        sapPurchaseOrder.Lines.LineTotal = double.Parse(_transit["cfr_freight"].ToString());
                        sapPurchaseOrder.Lines.Add();
                    }


                    /*
                     * PI: numero de PI
                     * FACTURA: comercial_invoice
                     * BOOKING: booking_number
                     * BL: bl_number
                     * ETA: eta
                     */

                    sapPurchaseOrder.Comments = $"PI: {_piNumber[_posicion, 0]}\n FACTURA: {_transit["invoice_comercial"]}\nBOOKING: {_transit["booking_number"]}\nBL: {_transit["bl_number"]}\nETA: {_transit["eta"]}";
                    sapPurchaseOrder.UserFields.Fields.Item("U_Transito_Odoo").Value = _transit["id"].ToString();
                    sapPurchaseOrder.UserFields.Fields.Item("U_Transito_Name").Value = _transit["name"].ToString();
                    sapPurchaseOrder.UserFields.Fields.Item("U_FECHA").Value = DateTime.Parse(_transit["eta"].ToString());
                    sapPurchaseOrder.UserFields.Fields.Item("U_NuevoETA").Value = DateTime.Parse(_transit["eta"].ToString());
                    sapPurchaseOrder.UserFields.Fields.Item("U_Guatex").Value = _transit["bl_number"].ToString();
                    sapPurchaseOrder.UserFields.Fields.Item("U_Pi_Number").Value = _piNumber[_posicion, 0];

                    foreach (JToken linea in _lineasOrden)
                    {
                        procesarLinea = false;
                        pesoNeto = 0;
                        pesoBruto = 0;

                        if (linea != null)
                        {
                            if (linea["product_id"][0]["name"].ToString() != "Spare Parts")
                            {

                                if (linea["product_id"][0]["name"].ToString() == "Spare Parts")
                                {
                                    if (linea["container_number"].ToString() == _piNumber[_posicion, 10])
                                    { procesarLinea = true; }
                                }
                                else
                                {
                                    if (linea["pi_number"].ToString() == _piNumber[_posicion, 0])
                                    { procesarLinea = true; }
                                }

                                if (procesarLinea)
                                {
                                    _lg.entryLog($"Articulo: {linea["product_id"][0]["name"]}");
                                    sapPurchaseOrder.Lines.ItemCode = linea["product_id"][0]["name"].ToString();

                                    if (_operacion == "GRS-CR")
                                    { sapPurchaseOrder.Lines.WarehouseCode = "01 B"; }
                                    else
                                    { sapPurchaseOrder.Lines.WarehouseCode = "01"; }

                                    sapPurchaseOrder.Lines.Quantity = double.Parse(linea["product_qty"].ToString());
                                    sapPurchaseOrder.Lines.TaxCode = "EXE";
                                    sapPurchaseOrder.Lines.VatGroup = "EXE";
                                    //sapPurchaseOrder.Lines.Price = 0; //price_unit
                                    sapPurchaseOrder.Lines.UnitPrice = double.Parse(linea["price_unit"].ToString());
                                    sapPurchaseOrder.Lines.LineTotal = double.Parse(linea["price_subtotal"].ToString());
                                    sapPurchaseOrder.Lines.UserFields.Fields.Item("U_Contenedor").Value = linea["container_number"].ToString();

                                    // if (_operacion == "GRS-GR")
                                    {
                                        if (linea["volumen"].ToString().Length > 10)
                                        { sapPurchaseOrder.Lines.UserFields.Fields.Item("U_Volumen").Value = linea["volumen"].ToString().Substring(0, 10); }
                                        else
                                        { sapPurchaseOrder.Lines.UserFields.Fields.Item("U_Volumen").Value = linea["volumen"].ToString(); }

                                        if (double.TryParse(linea["total_gross_weight"].ToString(), out pesoBruto))
                                        { sapPurchaseOrder.Lines.UserFields.Fields.Item("U_PesoBruto").Value = Math.Round(pesoBruto, 5).ToString(); } // (Math.Round(sapPurchaseOrder.Lines.Quantity * double.Parse(linea["gross_weight"].ToString()), 5)).ToString();
                                        else
                                        { _lg.entryLog($"Peso bruto no valido: {linea["total_gross_weight"]}"); }

                                        if (double.TryParse(linea["total_net_weight"].ToString(), out pesoNeto))
                                        { sapPurchaseOrder.Lines.UserFields.Fields.Item("U_PesoNeto").Value = Math.Round(pesoNeto, 5).ToString(); }// (Math.Round(sapPurchaseOrder.Lines.Quantity * double.Parse(linea["net_weight"].ToString()), 5)).ToString();
                                        else
                                        { _lg.entryLog($"Peso neto no valido: {linea["total_net_weight"]}"); }
                                    }

                                    sapPurchaseOrder.Lines.Add();
                                }
                            }
                        }
                    }
                    resultado = sapPurchaseOrder.Add();

                    if (resultado != 0)
                    {
                        SAPHana.sapCompany.GetLastError(out SAPHana.lErrCode, out SAPHana.sErrMsg);
                        _lg.entryLog(SAPHana.lErrCode.ToString() + ": " + SAPHana.sErrMsg);
                        registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, SAPHana.lErrCode, SAPHana.sErrMsg, _test, _lg);
                    }
                    else
                    {
                        query = $"SELECT * FROM \"OPOR\" WHERE \"CANCELED\" = 'N' AND \"U_Transito_Odoo\" = trim('{_transit["id"]}') AND \"U_Pi_Number\" = trim('{_piNumber[_posicion, 0]}')";
                        rsPed = (SAPbobsCOM.Recordset)sapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        rsPed.DoQuery(query);

                        if (rsPed.RecordCount > 0)
                        {
                            sapPurchaseOrder.Browser.Recordset = rsPed;
                            sapPurchaseOrder.Browser.MoveFirst();
                            ordenCompra = sapPurchaseOrder.DocNum;
                        }
                        _lg.entryLog($"Transito ODOO [{_transit["id"]}] {_transit["name"]} en [{SAPHana.sapCompany.CompanyDB}] Orden de compra No. [{sapPurchaseOrder.DocEntry}] {sapPurchaseOrder.DocNum}");
                    }
                }
            }
            catch (Exception ex)
            {
                SAPHana.LErrCode = -3;
                SAPHana.SErrMsg = $"Ocurrió un problema al construir la Orden de compra. [{ex.Source}: {ex.Message}]";
                registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, SAPHana.lErrCode, SAPHana.sErrMsg, _test, _lg);
            }

            return ordenCompra;
        }

        #endregion

        #region "Documentos de venta"

        /// <summary>
        /// genera orden de venta
        /// </summary>
        /// <param name="_transit">objeto json con toda la información del transito</param>
        /// <param name="_lineasOrden">arreglo de objetos json con la información de las lineas de detalle</param>
        /// <param name="_operacion">operacion involucrada</param>
        /// <param name="_lg">objeto para registro de eventos</param>
        /// <param name="_test">true: ambiente de pruebas; false: ambiente productivo</param>
        /// <param name="_piNumber">arreglo con información sobre las PI en el transito en proceso</param>
        /// <param name="_posicion">fila de _piNumber con que se esta trabajando</param>
        /// <param name="_userSAP">codigo de usuario SAP que genera el documento</param>
        /// <param name="_dtInv">lista de precios de venta en Global Reach Sales Inc</param>
        public double generaOrdenVenta(JToken _transit, JToken[] _lineasOrden, string _operacion, logObj _lg, bool _test, string[,] _piNumber, int _posicion, int _userSAP, DataTable _dtInv)
        {
            int resultado = 0;
            double ordenVenta = -1;
            double pesoBruto = 0;
            double pesoNeto = 0;
            bool pasaInfo = true;
            bool IsCFRCheck;
            string query;
            DataRow[] dtRw;
            float precioHana;
            SAPbobsCOM.Recordset rsPed;
            bool continuar = true;
            int addedLines;

            try
            {
                sapOrder = (SAPbobsCOM.Documents)sapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
                query = $"SELECT * FROM \"ORDR\" WHERE \"CANCELED\" = 'N' AND \"U_Transito_Odoo\" = trim('{_transit["id"]}') AND \"U_Pi_Number\" = trim('{_piNumber[_posicion, 0]}')";

                rsPed = (SAPbobsCOM.Recordset)sapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                rsPed.DoQuery(query);

                if (rsPed.RecordCount != 0)
                {
                    sapOrder.Browser.Recordset = rsPed;
                    sapOrder.Browser.MoveFirst();
                    ordenVenta = sapOrder.DocNum;
                    _lg.entryLog($"Transito ODOO [{_transit["id"]}] {_transit["name"]} en [{SAPHana.sapCompany.CompanyDB}] Orden de venta No. [{sapOrder.DocEntry}] {sapOrder.DocNum}");
                }
                else
                {
                    sapOrder.HandWritten = SAPbobsCOM.BoYesNoEnum.tNO;
                    sapOrder.CardCode = sapBPC.CardCode;
                    sapOrder.CardName = sapBPC.CardName;

                    if (sapBPC.CardName.Length > 50)
                    { sapOrder.PayToCode = sapBPC.CardName.Substring(0, 50); }
                    else
                    { sapOrder.PayToCode = sapBPC.CardName; }

                    sapOrder.Address = sapBPC.Address;
                    sapOrder.SalesPersonCode = _userSAP;
                    Console.WriteLine(_userSAP);
                    sapOrder.NumAtCard = _transit["invoice_comercial"].ToString();
                    sapOrder.DocDate = DateTime.Now;
                    sapOrder.DocDueDate = DateTime.Parse(_transit["eta"].ToString());
                    sapOrder.GroupHandWritten = BoYesNoEnum.tYES;
                    sapOrder.GroupNumber = int.Parse(_piNumber[_posicion, 7].ToString());
                    sapOrder.DocType = BoDocumentTypes.dDocument_Items;

                    if (_operacion != "GRS-USA")
                    {
                        sapOrder.DocCurrency = "USD";
                    }
                    else
                    {
                        sapOrder.DocCurrency = "USD";
                    }
                    
                    IsCFRCheck = _transit["is_cfr_check"].ToObject<bool>();
                    if (IsCFRCheck) {
                        
                        sapPurchaseOrder.Lines.ItemCode = "Flete Maritimo";
                        sapPurchaseOrder.Lines.Quantity = 1;
                        sapPurchaseOrder.Lines.TaxCode = "EXE";
                        sapPurchaseOrder.Lines.VatGroup = "EXE";
                        sapPurchaseOrder.Lines.UnitPrice = double.Parse(_transit["cfr_freight"].ToString());
                        sapPurchaseOrder.Lines.LineTotal = double.Parse(_transit["cfr_freight"].ToString());
                        sapPurchaseOrder.Lines.Add();
                    }



                    sapOrder.Comments = $"PI: {_piNumber[_posicion, 0]}\n FACTURA: {_transit["invoice_comercial"]}\nBOOKING: {_transit["booking_number"]}\nBL: {_transit["bl_number"]}\nETA: {_transit["eta"]}";

                    sapOrder.UserFields.Fields.Item("U_Transito_Odoo").Value = _transit["id"].ToString();
                    sapOrder.UserFields.Fields.Item("U_Transito_Name").Value = _transit["name"].ToString();
                    sapOrder.UserFields.Fields.Item("U_FECHA").Value = DateTime.Parse(_transit["eta"].ToString());
                    sapOrder.UserFields.Fields.Item("U_NuevoETA").Value = DateTime.Parse(_transit["eta"].ToString());
                    sapOrder.UserFields.Fields.Item("U_Guatex").Value = _transit["bl_number"].ToString();
                    sapOrder.UserFields.Fields.Item("U_Pi_Number").Value = _piNumber[_posicion, 0];

                    addedLines = 0;

                    foreach (JToken linea in _lineasOrden)
                    {
                        precioHana = 0;
                        pesoNeto = 0;
                        pesoBruto = 0;
                        pasaInfo = true;

                        if ((linea["customer_name"].ToString() == null && linea["incoterm"].ToString().ToLower() == "local") && (linea["customer_name"].ToString().Trim() == "" && linea["incoterm"].ToString().ToLower() == "local"))
                        {
                            pasaInfo = false;
                        } else
                        {
                            pasaInfo = true;
                        }

                        if (linea != null)
                        {
                            if (continuar)
                            {
                                if ((linea["company_to_id"][0]["name"].ToString() == _operacion) && (linea["pi_number"].ToString() == _piNumber[_posicion, 0]) && pasaInfo)
                                {
                                    _lg.entryLog($"Articulo: {linea["product_id"][0]["name"]}");

                                    if (linea["product_id"][0]["name"].ToString() != "Spare Parts")
                                    {
                                        dtRw = _dtInv.Select($"ItemCode = '{linea["product_id"][0]["name"]}'");

                                        if (dtRw.Length == 0)
                                        {
                                            registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, 11, $"Precio venta de {linea["product_id"][0]["name"]} no disponible", _test, _lg);
                                            continuar = false;
                                        }
                                        else
                                        {
                                            precioHana = float.Parse(dtRw[0]["Price"].ToString());
                                        }
                                    }

                                    if (continuar)
                                    {
                                        sapOrder.Lines.ItemCode = linea["product_id"][0]["name"].ToString();
                                        sapOrder.Lines.WarehouseCode = "01";
                                        sapOrder.Lines.Quantity = double.Parse(linea["product_qty"].ToString());
                                        sapOrder.Lines.TaxCode = "EXE";
                                        sapOrder.Lines.VatGroup = "EXE";
                                        sapOrder.Lines.UnitPrice = precioHana; // double.Parse(linea["price_unit"].ToString());
                                        sapOrder.Lines.LineTotal = sapOrder.Lines.Quantity * sapOrder.Lines.UnitPrice; // double.Parse(linea["price_subtotal"].ToString());
                                        sapOrder.Lines.UserFields.Fields.Item("U_Contenedor").Value = linea["container_number"].ToString();

                                        if (linea["volumen"].ToString().Length > 10)
                                        { sapOrder.Lines.UserFields.Fields.Item("U_Volumen").Value = linea["volumen"].ToString().Substring(0, 10); }
                                        else
                                        { sapOrder.Lines.UserFields.Fields.Item("U_Volumen").Value = linea["volumen"].ToString(); }

                                        // sapOrder.Lines.UserFields.Fields.Item("U_Volumen").Value = linea["volumen"].ToString();

                                        if (double.TryParse(linea["total_gross_weight"].ToString(), out pesoBruto))
                                        { sapOrder.Lines.UserFields.Fields.Item("U_PesoBruto").Value = Math.Round(pesoBruto, 5).ToString(); } // (Math.Round(sapPurchaseOrder.Lines.Quantity * double.Parse(linea["gross_weight"].ToString()), 5)).ToString();
                                        else
                                        { _lg.entryLog($"Peso bruto no valido: {linea["total_gross_weight"]}"); }

                                        if (double.TryParse(linea["total_net_weight"].ToString(), out pesoNeto))
                                        { sapOrder.Lines.UserFields.Fields.Item("U_PesoNeto").Value = Math.Round(pesoNeto, 5).ToString(); }// (Math.Round(sapPurchaseOrder.Lines.Quantity * double.Parse(linea["net_weight"].ToString()), 5)).ToString();
                                        else
                                        { _lg.entryLog($"Peso neto no valido: {linea["total_net_weight"]}"); }

                                        sapOrder.Lines.Add();
                                        addedLines++;
                                    }
                                }
                                else
                                if ((linea["customer_name"].ToString() != "False") && (linea["customer_name"].ToString().Trim() != ""))
                                {
                                    ordenVenta = -50000;
                                }
                            }
                        }
                    }

                    if ((continuar) && (addedLines > 0))
                    {
                        resultado = sapOrder.Add();

                        if (resultado != 0)
                        {
                            SAPHana.sapCompany.GetLastError(out SAPHana.lErrCode, out SAPHana.sErrMsg);
                            registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, SAPHana.lErrCode, SAPHana.sErrMsg, _test, _lg);
                        }
                        else
                        {
                            query = $"SELECT * FROM \"ORDR\" WHERE \"CANCELED\" = 'N' AND \"U_Transito_Odoo\" = trim('{_transit["id"]}') AND \"U_Pi_Number\" = trim('{_piNumber[_posicion, 0]}')";
                            rsPed = (SAPbobsCOM.Recordset)sapCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            rsPed.DoQuery(query);

                            if (rsPed.RecordCount > 0)
                            {
                                sapOrder.Browser.Recordset = rsPed;
                                sapOrder.Browser.MoveFirst();
                                ordenVenta = sapOrder.DocNum;
                            }

                            _lg.entryLog($"Transito ODOO [{_transit["id"]}] {_transit["name"]} en [{SAPHana.sapCompany.CompanyDB}] Orden de venta No. [{sapOrder.DocEntry}] {sapOrder.DocNum}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SAPHana.LErrCode = -3;
                SAPHana.SErrMsg = $"Ocurrió un problema al construir la Orden de venta. [{ex.Source}: {ex.Message}]";
                registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, SAPHana.lErrCode, SAPHana.sErrMsg, _test, _lg);
            }

            return ordenVenta;
        }

        #endregion

        #region "utilitarios HANA"

        public DataTable getInventario(string _operacion, bool _test)
        {

               

            DataTable dt = new DataTable();
            string connectionString;
            string stComm;
            Estado = "OK";

            try
            {
                connectionString = ordersNTransfers.Properties.Resources.HCS; // "DSN=HANA; SERVERNODE=192.168.11.20:30015; UID=SYSTEM; PWD=Aez1NtM6";

                using (OdbcConnection sqlCon = new OdbcConnection(connectionString))
                {
                    using (OdbcDataAdapter SqlDa = new OdbcDataAdapter())
                    {
                        sqlCon.Open();

                        using (OdbcCommand cmm = new OdbcCommand())
                        {
                            cmm.Connection = sqlCon;
                            if (_test) 
                            {
                                stComm = $"CALL \"TEST_GRS\".\"PR_SL_OITM_SALEPRICE_GRS\" ('{_operacion}') ";
                            }
                            else
                            {
                                stComm = $"CALL \"GRS\".\"PR_SL_OITM_SALEPRICE_GRS\" ('{_operacion}') ";
                            }
                            
                            cmm.CommandText = stComm;
                            cmm.CommandType = CommandType.StoredProcedure;
                            SqlDa.SelectCommand = cmm;
                            SqlDa.Fill(dt);
                        }

                        sqlCon.Close();
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                Estado = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// registra en tabla GRS.INCIDENTE_INTERFAZ información de eventos que impiden el traslado de información
        /// </summary>
        /// <param name="_id_odoo">llave primaria del registro que generó el evento</param>
        /// <param name="_name_oddo">identificador visible del registro que generó el evento</param>
        /// <param name="_operacion">operacion involucrada</param>
        /// <param name="_codigo_incidente">codigo de la excepcion generada durante el traslado</param>
        /// <param name="_incidente">detalle de la excepcion generada durante el traslado</param>
        /// <param name="_test">true: ambiente de pruebas; false: ambiente productivo</param>
        /// <param name="_lg">objeto para registro de eventos</param>
        public static bool registraBitacora(string _id_odoo, string _name_oddo, string _operacion, int _codigo_incidente, string _incidente, bool _test, logObj _lg)
        {
            string connectionString;
            string stComm;
            string pruebas = (_test ? "S" : "N");

            try
            {
                _lg.entryLog(_incidente);
                connectionString = ordersNTransfers.Properties.Resources.HCS; // "DSN=HANA; SERVERNODE=192.168.11.20:30015; UID=SYSTEM; PWD=Aez1NtM6";

                using (OdbcConnection sqlCon = new OdbcConnection(connectionString))
                {
                    using (OdbcDataAdapter SqlDa = new OdbcDataAdapter())
                    {
                        sqlCon.Open();

                        using (OdbcCommand cmm = new OdbcCommand())
                        {
                            cmm.Connection = sqlCon;
                            stComm = $"CALL \"GRS\".\"PR_IN_INCIDENTE_INTERFAZ\" ('{_id_odoo}', '{_name_oddo}', '{_operacion}', {_codigo_incidente}, '{_incidente}', {ordersNTransfers.Properties.Resources.id_interfaz}, '{pruebas}') ";
                            cmm.CommandText = stComm;
                            cmm.CommandType = CommandType.StoredProcedure;
                            SqlDa.InsertCommand = cmm;
                            SqlDa.InsertCommand.ExecuteNonQuery();
                        }

                        sqlCon.Close();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _lg.entryLog($"Error al registrar en bitacora interfaz: {ex.Source} - {ex.Message}");
                return false;
            }
        }


        public static DataTable getETA(string _source, string _compania, string _transito, string _PINumber)
        {
            DataTable dt = new DataTable();
            string connectionString;
            string stComm;
            Estado = "OK";

            try
            {
                connectionString = ordersNTransfers.Properties.Resources.HCS; // "DSN=HANA; SERVERNODE=192.168.11.20:30015; UID=SYSTEM; PWD=Aez1NtM6";

                using (OdbcConnection sqlCon = new OdbcConnection(connectionString))
                {
                    using (OdbcDataAdapter SqlDa = new OdbcDataAdapter())
                    {
                        sqlCon.Open();

                        using (OdbcCommand cmm = new OdbcCommand())
                        {
                            cmm.Connection = sqlCon;
                            stComm = $"CALL \"GRS\".\"{_source}\" ('{_transito}', '{_PINumber}', '{_compania}') ";
                            cmm.CommandText = stComm;
                            cmm.CommandType = CommandType.StoredProcedure;
                            SqlDa.SelectCommand = cmm;
                            SqlDa.Fill(dt);
                        }

                        sqlCon.Close();
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                Estado = ex.Message;
                return null;
            }
        }

        /// <summary>
        /// rastrea información de ingresos a bodega asociados a un transito especifico
        /// </summary>
        /// <param name="_source">procedimiento almacenado</param>
        /// <param name="_compania">operacion involucrada</param>
        /// <param name="_transito">numero de transito a rastrear</param>
        public static DataTable getIngresosBodega(string _source, string _compania, string _transito)
        {
            DataTable dt = new DataTable();
            string connectionString;
            string stComm;
            Estado = "OK";

            try
            {
                connectionString = ordersNTransfers.Properties.Resources.HCS;

                using (OdbcConnection sqlCon = new OdbcConnection(connectionString))
                {
                    using (OdbcDataAdapter SqlDa = new OdbcDataAdapter())
                    {
                        sqlCon.Open();

                        using (OdbcCommand cmm = new OdbcCommand())
                        {
                            cmm.Connection = sqlCon;
                            stComm = $"CALL \"GRS\".\"{_source}\" ('{_transito}', '{_compania}') ";
                            cmm.CommandText = stComm;
                            cmm.CommandType = CommandType.StoredProcedure;
                            SqlDa.SelectCommand = cmm;
                            SqlDa.Fill(dt);
                        }

                        sqlCon.Close();
                    }
                }

                return dt;
            }
            catch (Exception ex)
            {
                Estado = ex.Message;
                return null;
            }
        }

        public static int getInfo(string _source, int _create_uid_ID, string _compania, bool _test)
        {
            DataTable dt = new DataTable();
            DataRow dr;
            string connectionString;
            string stComm;
            Estado = "OK";
            int resultado = -99;
            string pruebas = (_test ? "S" : "N");

            try
            {
                connectionString = ordersNTransfers.Properties.Resources.HCS; // "DSN=HANA; SERVERNODE=192.168.11.20:30015; UID=SYSTEM; PWD=Aez1NtM6";

                using (OdbcConnection sqlCon = new OdbcConnection(connectionString))
                {
                    using (OdbcDataAdapter SqlDa = new OdbcDataAdapter())
                    {
                        sqlCon.Open();

                        using (OdbcCommand cmm = new OdbcCommand())
                        {
                            cmm.Connection = sqlCon;
                            stComm = $"CALL \"GRS\".\"{_source}\" ({_create_uid_ID}, '{_compania}', '{pruebas}') ";
                            cmm.CommandText = stComm;
                            cmm.CommandType = CommandType.StoredProcedure;
                            SqlDa.SelectCommand = cmm;
                            SqlDa.Fill(dt);
                        }

                        sqlCon.Close();

                        if (dt.Rows.Count > 0)
                        {
                            dr = dt.Rows[0];
                            resultado = int.Parse(dr[0].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Estado = ex.Message;
            }

            return resultado;
        }

        private static string evaluaSiNo(string _valor, bool _negar = false)
        {
            try
            {
                if (bool.Parse(_valor))
                {
                    return "Si";
                }
                else { return "No"; }
            }
            catch (Exception ex)
            { return "No"; }
        }

        private static BoYesNoEnum evaluaBoYesNoEnum(int _valor, bool _negar = false)
        {
            bool parseado = false;

            if (_valor == 1) { parseado = true; }

            if (_negar) { parseado = !parseado; }

            if (parseado)
            {
                return BoYesNoEnum.tYES;
            }
            else { return BoYesNoEnum.tNO; }
        }

        private static BoYesNoEnum evaluaBoYesNoEnum(string _valor, bool _negar = false)
        {
            bool parseado;

            try
            {
                if (_negar) { parseado = !bool.Parse(_valor); }
                else { parseado = bool.Parse(_valor); }

                if (parseado)
                {
                    return BoYesNoEnum.tYES;
                }
                else { return BoYesNoEnum.tNO; }
            }
            catch (Exception ex)
            {
                if (_negar)
                {
                    return BoYesNoEnum.tYES;
                }
                else
                {
                    return BoYesNoEnum.tNO;
                }
            }
        }

        private static string evaluaSiNo(int _valor, bool _negar = false)
        {
            string _respuesta = "";

            if (_valor == 1) { _respuesta = "Si"; }
            else { _respuesta = "No"; }

            if (_negar)
            {
                if (_respuesta == "Si") { return "No"; }
                else { return "Si"; }
            }
            else { return _respuesta; }
        }

        #endregion

        public static string SErrMsg { get => sErrMsg; set => sErrMsg = value; }
        public static int LErrCode { get => lErrCode; set => lErrCode = value; }
        public static string SCnnStatus { get => sCnnStatus; set => sCnnStatus = value; }

    }
}
