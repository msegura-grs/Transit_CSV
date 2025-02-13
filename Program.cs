using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ordersNTransfers
{
    class Program
    {
        static void Main(string[] args)
        {
            // uso general 
            Boolean test = false;
            int userSAP = 0;
            string estadoTransitoBarco = "fiscal_warehouse";  // "sailing";
            string estadoTransitoPuerto = "arrived_to_port";
            string estadoBodegaFiscal = "fiscal_warehouse";
            logObj lg;
            string faseActiva = "Inicio";
            DateTime dtStart = DateTime.Now;
            bool continuar;
            bool compraDirecta = false;
            bool compraLocal = true;
            string incoterm = "";
            int noOps = 11; //  AJUSTAR AL INCLUIR ESTADOS UNIDOS 

            // HANA
            SAPHana hn;
            SAPHana shGR;
            string spFact;
            //string spEta;
            DataTable dtInv = new DataTable();

            //                              ***     Operacion   ***
            // { "abreviatura operacion"
            // , "indicador inclusion en documento activo"
            // , "codigo cliente en Global Reach"
            // , "Nombre operacion ODOO"
            // , "codigo proveedor Global Reach en operación"
            // , "serie documento compra mercaderia" }

            string[,] operaciones = new string[,] { { "GRS-GT", "0", "CR00001", "GRS-GT", "PR00005", "12" } 
                                                  , { "GRS-HN", "0", "CR00003", "GRS-HN", "PR00003", "15" }
                                                  , { "GRS-ES", "0", "CR00002", "GRS-ES", "PR00001", "12" }
                                                  , { "GRS-NI", "0", "CR00004", "GRS-NI", "PR00002", "12" }
                                                  , { "GRS-CR", "0", "CR00005", "GRS-CR", "PR00001", "15" } 
                                                  , { "GRS-RD", "0", "C00010", "GRS-RD", "P00000", "15" }
                                                  , { "GRS-PR", "0", "C00006", "GRS-PR", "P00000", "15" }
                                                  , { "GRS-GR", "0", "", "GRS-GR", "", "15" }
                                                  , { "GRS-TT", "0", "C00019", "GRS-TT", "P00000", "15" }
                                                  , { "GRS-ECU", "0", "C00021", "GRS-ECU", "P00000", "15" }
                                                  , { "GRS-USA", "0", "CR00009", "GRS-USA", "PR00007", "13" } // PARA ESTADOS UNIDOS 
                                                  , { "DST-GT", "0", "C00025", "DST-GT", "PR00001", "15" } // PARA DUSTRITERRA
            };

            int shGlobalEC;
            string shGlobalStatus;
            double shGlobalDocOC; // orden de compra Global Reach
            double shGlobalDocOV; // orden de venta Global Reach

            int shOperacionEC;
            string shOperacionStatus;
            double shOperacionDocOC; // orden de compra 
            string codigoProveedorOP; // codigo de proveedor para la operacion
            DataTable tbIng;
//            DataTable tbETA;

            // API rest
            apirest ar = new apirest();
            JObject trs;
            JObject companys;
            JObject ordLn;
//            JObject ordETA;
//            JObject ordHd;
            string fabricante = "";
            string body;
            string[] _params;
            string[] _params1;
            JToken[] lineasOrden;
            string[,] piNumber;
            float diferencia;
            float qty_sap;
            int lineasAbiertas;
            string codigoArticulo;
            float cantidadSAP;

            // postgresql origen de datos
            postgreSQLobj pgObj;
            DataTable dtTransitosCierre;

            //while (1 == 1)
            {
                if (DateTime.Now > dtStart)
                {
                    lg = new logObj(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location.ToString()) + "\\logs\\ordersNTransfers" + DateTime.Now.ToString("yyyyMMdd") + ".txt");
                    lg.entryLog("Hora inicio: " + DateTime.Now.ToString());

                    try
                    {
                        faseActiva = "Recuperacion de transitos";
                        _params = new string[2];
                        _params[0] = $"domain =[('state','=','{estadoTransitoBarco}')]";
                        //_params[0] = $"domain =[('state','=','{estadoTransito}'),('name','=','SHIP/2023/01241')]";
                        // _params[1] = "fields =['activity_ids', 'activity_summary', 'amount_bl', 'bl_number', 'booking_number', 'create_date', 'date_order', 'display_name', 'name', 'id', 'lines_ids', 'shipper_id', 'eta']";
                        _params[1] = "fields =['bl_number', 'booking_number', 'create_date', 'date_order', 'display_name', 'name', 'id', 'lines_ids', 'shipper_id', 'eta', 'is_cfr_check' ,'cfr_freight']";
                        
                        _params1 = new string[2];
                        _params1[0] = $"domain =[('state','=','{estadoTransitoBarco}')]";
                        //_params[0] = $"domain =[('state','=','{estadoTransito}'),('name','=','SHIP/2023/01241')]";
                        // _params[1] = "fields =['activity_ids', 'activity_summary', 'amount_bl', 'bl_number', 'booking_number', 'create_date', 'date_order', 'display_name', 'name', 'id', 'lines_ids', 'shipper_id', 'eta']";
                        _params1[1] = "fields =['id', 'name', 'incoterm']";


                        pgObj = new postgreSQLobj(test);

                        //string filePath = "transitos.xlsx"; // O usa "transitos.csv"
                        //trs = postgreSQLobj.GetManufacturingOrderShippingOverviewFromFile(filePath);

                        pgObj = new postgreSQLobj(test);

                        trs = postgreSQLobj.getManufacturingOrderShippingOverview(postgreSQLobj.ConnectionString);

                        companys = obtieneJSon(ar.getInfo("res.company/search", _params1, test), lg);

                        if (!verificaOperacionAPI(trs["success"].ToString()))
                        {
                            lg.entryLog($"Ocurrio un problema al recoger la informacion de transitos");
                            lg.entryLog($"manufacturing.shipping/search");
                            lg.entryLog($"{trs["message"]}");
                            lg.entryLog(_params[0]);
                            lg.entryLog(_params[1]);
                        }
                        else
                        {
                            foreach (JToken transit in trs["data"])
                            {
                                lg.entryLog($"{transit["id"]}: {transit["name"]}, {transit["state"]}");
                                
                                 
                                { 
                                    if (transit["state"].ToString() == estadoTransitoBarco || transit["state"].ToString()  == estadoBodegaFiscal)
                                    {
                                        // lg.entryLog($"{transit["id"]}: {transit["name"]}");

                                        if (transit["name"].ToString() == "SHIP/2024/03248") //if (transit["name"].ToString() == "SHIP/2024/06967" || transit["name"].ToString() == "SHIP/2024/06968" || transit["name"].ToString() == "SHIP/2024/06969" || transit["name"].ToString() == "SHIP/2025/06971" || transit["name"].ToString() == "SHIP/2025/06973" || transit["name"].ToString() == "SHIP/2025/06976" || transit["name"].ToString() == "SHIP/2025/06977" || transit["name"].ToString() == "SHIP/2025/06983") //if(transit["name"].ToString() == "SHIP/2024/06967")//if (transit["name"].ToString() == "SHIP/2025/06964" || transit["name"].ToString() == "SHIP/2025/06963" || transit["name"].ToString() == "SHIP/2025/06962")//if (transit["name"].ToString() == "SHIP/2025/06961" || transit["name"].ToString() == "SHIP/2025/06960" || transit["name"].ToString() == "SHIP/2025/06959")//if (transit["name"].ToString() == "SHIP/2024/") //if (transit["name"].ToString() == "SHIP/2024/03006") { continue; }//"SHIP/2024/02362")  // ((transit["name"].ToString() == "SHIP/2023/01103") || (transit["name"].ToString() == "SHIP/2022/01038"))//((transit["name"].ToString() == "SHIP/2022/00548")) || (transit["name"].ToString() == "SHIP/2021/00185")) // || (transit["name"].ToString() == "SHIP/2021/00012") || (transit["name"].ToString() == "SHIP/2021/00101") || (transit["name"].ToString() == "SHIP/2021/00098") || (transit["name"].ToString() == "SHIP/2021/00136"))
                                        {
                                            faseActiva = "Verificacion de detalle y BL del transito, traslado de transitos";

                                            if (verificaDetalle(transit, "General", estadoTransitoBarco, lg, test) && verificaBL(transit, "General", estadoTransitoBarco, lg, test))
                                            {
                                                lineasOrden = new JToken[1];
                                                piNumber = new string[0, 0];
                                                compraDirecta = false; 
                                                compraLocal = true;
                                                faseActiva = "Clasificacion destino y fabricante en lineas, traslado de transitos";

                                                if (clasificaDestinoLineas(ar, ref operaciones, transit, ref lineasOrden, ref piNumber, ref fabricante, ref compraDirecta, ref compraLocal, ref incoterm, lg, test, noOps))
                                                {
                                                    for (int i = 0; i <= noOps; i++)
                                                    {

                                                        if (incoterm == "cfr") {
                                                            foreach (JToken company in companys["data"]) {
                                                                if (company["name"].ToString() == operaciones[i, 0]){
                                                                    incoterm = company["incoterm"].ToString();
                                                                    break;
                                                                }
                                                            }
                                                        }

                                                        if (operaciones[i, 1] == "1")
                                                        {

                                                            shGlobalDocOV = 0;
                                                            shGlobalDocOC = 0;

                                                            if ((!compraDirecta) || (incoterm == "cif") || (incoterm == "fob"))
                                                            {
                                                                // conexion con Global Reach
                                                                faseActiva = "Conectando a Global Reach";
                                                                shGR = new SAPHana("GRS-GR", test);
                                                                lg.entryLog(SAPHana.SCnnStatus);
                                                                shGlobalStatus = SAPHana.SCnnStatus;
                                                                shGlobalEC = SAPHana.LErrCode;
                                                                continuar = false;

                                                                if (shGlobalEC == 0)
                                                                {
                                                                    dtInv = shGR.getInventario(operaciones[i, 0], test);

                                                                    if ((dtInv.Rows.Count == 0) && (incoterm == "local"))
                                                                    {
                                                                        lg.entryLog($"No hay precios disponibles para la venta en Global Reach Sales Inc");
                                                                    }
                                                                    else
                                                                    {
                                                                        // revisar para GLOBAL REACH
                                                                        if (verificaCD(transit, int.Parse(piNumber[0, 9]), "GRS-GR", lg, test, ref userSAP))
                                                                        {
                                                                            //  se recoge información de proveedor
                                                                            faseActiva = "Recogiendo informacion del proveedor y operación cliente en Global Reach";

                                                                            if (shGR.buscaProveedor(transit, fabricante, false, lg, "GRS-GR", test, ref piNumber) && shGR.buscaCliente(transit, operaciones[i, 2], lg, "GRS-GR", test, incoterm))
                                                                            {
                                                                                if (buscaPlazoProveedor(transit, ar, lg, "GRS-GR", test, ref piNumber, compraLocal)) // 375
                                                                                {
                                                                                    // generación de orden de compra en Global Reach
                                                                                    faseActiva = "Generacion de orden de compra en Global Reach";

                                                                                    for (int ind = 0; ind < piNumber.GetLength(0); ind++)
                                                                                    {
                                                                                        //shGlobalDocOC = shGR.generaOrdenCompra(transit, lineasOrden, 15, lg, "GRS-GR", test, piNumber, ind, userSAP);
                                                                                        shGlobalDocOC = 1;

                                                                                        if (shGlobalDocOC <= 0) { lg.entryLog("Error al procesar Orden de Compra Global Reach"); }
                                                                                        else
                                                                                        {
                                                                                            actualizaDetalleOdoo(ar, lineasOrden, piNumber, ind, operaciones, i, "order_number", shGlobalDocOC.ToString(), lg);

                                                                                            //if (incoterm == "local") // (incoterm == "cif") || (incoterm == "fob")
                                                                                            {
                                                                                                // generacion de orden de venta en Global Reach
                                                                                                faseActiva = "Generacion de orden de venta en Global Reach";
                                                                                                //shGlobalDocOV = shGR.generaOrdenVenta(transit, lineasOrden, operaciones[i, 3], lg, test, piNumber, ind, userSAP, dtInv);

                                                                                                shGlobalDocOV = 1;

                                                                                                if (shGlobalDocOV <= 0)
                                                                                                {
                                                                                                    if (shGlobalDocOV != -50000)
                                                                                                    { lg.entryLog("Error al procesar Orden de Venta Global Reach"); }
                                                                                                    else
                                                                                                    { lg.entryLog("Orden procedente de CRM, no genera orden de venta en Global Reach"); }
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    actualizaDetalleOdoo(ar, lineasOrden, piNumber, ind, operaciones, i, "invoice_number", shGlobalDocOV.ToString(), lg);
                                                                                                    continuar = true;
                                                                                                }
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }

                                                                    shGR.desconecta();
                                                                }
                                                                else
                                                                {
                                                                    lg.entryLog($"Ocurrió un error de conexión con SAP HANA:\n {shGlobalStatus}");
                                                                }
                                                            }
                                                            else
                                                            { continuar = true; }

                                                            if (compraLocal) // Si incoterm != local || != landed compraLocal es false
                                                            {
                                                                if (continuar || incoterm.ToLower() == "landed")
                                                                {
                                                                    // conexion con operacion involucrada
                                                                    faseActiva = $"Conectando con {operaciones[i, 0]} como solicitante";
                                                                    hn = new SAPHana(operaciones[i, 0], test);
                                                                    lg.entryLog(SAPHana.SCnnStatus);
                                                                    shOperacionStatus = SAPHana.SCnnStatus;
                                                                    shOperacionEC = SAPHana.LErrCode;

                                                                    if (shOperacionEC == 0)
                                                                    {
                                                                        if (verificaCD(transit, int.Parse(piNumber[0, 9]), operaciones[i, 0], lg, test, ref userSAP))
                                                                        {
                                                                            //  se recoge información de proveedor
                                                                            faseActiva = "Recogiendo información de proveedor en operacion solicitante";

                                                                            if (compraDirecta) { codigoProveedorOP = fabricante; }
                                                                            else { codigoProveedorOP = operaciones[i, 4]; }

                                                                            if (hn.buscaProveedor(transit, codigoProveedorOP, !compraDirecta, lg, operaciones[i, 0], test, ref piNumber))
                                                                            {
                                                                                // generación de orden de compra en operacion involucrada

                                                                                if (buscaPlazoProveedor(transit, ar, lg, operaciones[i, 0], test, ref piNumber, compraLocal)) // 375
                                                                                {
                                                                                    faseActiva = "Generacion de orden de compra en operacion solicitante";
                                                                                    for (int ind = 0; ind < piNumber.GetLength(0); ind++)
                                                                                    {
                                                                                        if ((compraDirecta) || (dtInv.Rows.Count == 0))
                                                                                        {
                                                                                            shOperacionDocOC = hn.generaOrdenCompra(transit, lineasOrden, int.Parse(operaciones[i, 5]), lg, operaciones[i, 0], test, piNumber, ind, userSAP);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            shOperacionDocOC = hn.generaOrdenCompraOperacion(transit, lineasOrden, int.Parse(operaciones[i, 5]), lg, operaciones[i, 0], test, piNumber, ind, userSAP, dtInv);
                                                                                        }

                                                                                        if (shOperacionDocOC <= 0) { lg.entryLog($"Error al procesar Orden de compra {operaciones[i, 0]}"); }
                                                                                        else
                                                                                        {
                                                                                            faseActiva = "Actualizando lineas procesadas con numero de orden de compra generado en operacion solicitante";
                                                                                            actualizaDetalleOdoo(ar, lineasOrden, piNumber, ind, operaciones, i, "other_invoice_number", shOperacionDocOC.ToString(), lg);
                                                                                            lg.entryLog($"Generacion de documentos concluida con exito");
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }

                                                                        hn.desconecta();
                                                                    }
                                                                }
                                                            }

                                                            lg.entryLog("***", true);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    lg.entryLog("Ocurrio un problema al clasificar destino de las lineas del transito");
                                                    lg.entryLog("***", true);
                                                }
                                            }
                                        }
                                    }
                            }
                            }
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        lg.entryLog("", true);
                        lg.entryLog($"Excepcion en {faseActiva}");
                        lg.entryLog($"{ex.Source}: {ex.Message}");
                    }

                    pgObj = new postgreSQLobj(test);
                    dtTransitosCierre = postgreSQLobj.cierraTransitos(postgreSQLobj.ConnectionString, "fn_oms_transit_arrived");
                    lg.entryLog($"Cierre de cabeceras de transitos: {postgreSQLobj.CnnStatus}", true);

                    // pgObj
                    // hacer llamada a SELECT * FROM fn_up_transit_arrived();

                    lg.entryLog($"Hora fin: {DateTime.Now.ToString()}", true);
                    dtStart = DateTime.Now.AddMinutes(5);
                }
            }
        }

        #region "funciones complementarias"

        private static bool buscaPlazoProveedor(JToken _transit, apirest _ar, logObj _lg, string _compania, bool _test, ref string[,] _piNumber, bool _compraLocal) // 140 
        {
            bool resultado = false;
            bool faltoUno = false;
            int codPlazoOdoo;
            int codPlazoSap;
            JObject mnf;      // manufacturer
            JObject mnfDt;    // detalle manufacturer
            JObject prt;      // partner
            JObject prtDt;    // detalle partner

            for (int ind = 0; ind < _piNumber.GetLength(0); ind++)
            {
                mnf = obtieneJSon(_ar.getFullInfo($"res.manufacturer", _piNumber[ind, 6]), _lg);


                if (!verificaOperacionAPI(mnf["success"].ToString()))
                {
                    _lg.entryLog($"Ocurrio un problema al recoger informacion del fabricante");
                    _lg.entryLog($"res.manufacturer/{_piNumber[ind, 6]}");
                    _lg.entryLog($"{mnf["message"]}");
                    resultado = false;
                }
                else
                {
                    mnfDt = obtieneJSon(mnf["data"][0].ToString(), _lg);
                    prt = obtieneJSon(_ar.getFullInfo($"res.partner", mnfDt["parent_supplier_id"][0]["id"].ToString()), _lg);

                    if (!verificaOperacionAPI(prt["success"].ToString()))
                    {
                        _lg.entryLog($"Ocurrio un problema al recoger informacion del partner asociado al fabricante");
                        _lg.entryLog($"res.partner/{mnfDt["parent_supplier_id"][0]["id"]}");
                        _lg.entryLog($"{prt["message"]}");
                        resultado = false;
                    }
                    else
                    {
                        prtDt = obtieneJSon(prt["data"][0].ToString(), _lg);

                        if (prtDt["property_supplier_payment_term_id"].Count() == 0)
                        {
                            codPlazoOdoo = -100;
                            codPlazoSap = -99;
                            faltoUno = true;
                        }
                        else
                        {
                            codPlazoOdoo = int.Parse(prtDt["property_supplier_payment_term_id"][0]["id"].ToString());
                            codPlazoSap = SAPHana.getInfo("PR_SL_PLAZO_CREDITO_PROVEEDOR_IDSAP", codPlazoOdoo, _compania, _test); // plazo del proveedor en GLOBAL REACH
                        }

                        if (codPlazoSap == -99)
                        { SAPHana.registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _piNumber[ind, 5], 4, $"Plazo de credito de {_piNumber[ind, 1]} no definido o mapeado en SAP HANA para GRS-GR", _test, _lg); }
                        else
                        {
                            _piNumber[ind, 7] = codPlazoSap.ToString();

                            if (!_compraLocal)
                            { resultado = true; }
                            else
                            {
                                codPlazoSap = SAPHana.getInfo("PR_SL_PLAZO_CREDITO_PROVEEDOR_IDSAP", codPlazoOdoo, _piNumber[ind, 5], _test); // plazo del proveedor en la operación involucrada

                                if (codPlazoSap == -99)
                                { SAPHana.registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _piNumber[ind, 5], 4, $"Plazo de credito de {_piNumber[ind, 1]} no definido o mapeado en SAP HANA para {_piNumber[ind, 5]}", _test, _lg); }
                                else
                                {
                                    _piNumber[ind, 8] = codPlazoSap.ToString();
                                    resultado = true;
                                }
                            }
                        }
                    }
                }
            }

            if (faltoUno) { resultado = false; }
            return resultado;
        }

        private static bool actualizaDetalleOdoo (apirest _ar, JToken[] _lineasOrden, string[,] _piNumber, int _activePI, string[,] _operaciones, int _activeOP, string _tag, string _valor, logObj _lg)
        {
            bool resultado = false;
            string body;
            JObject ordLn;

            foreach (JToken linea in _lineasOrden)
            {
                if (linea != null)
                {
                    if ((linea["company_to_id"][0]["name"].ToString() == _operaciones[_activeOP, 3]) && (linea["pi_number"].ToString() == _piNumber[_activePI, 0]))
                    {
                        body = $"{{ \"{_tag}\": \"{_valor}\" }}";

                        ordLn = obtieneJSon(_ar.putInfo("manufacturing.order.lines", int.Parse(linea["id"].ToString()), body), _lg);

                        if (!verificaOperacionAPI(ordLn["success"].ToString()))
                        {
                            _lg.entryLog($"Ocurrio un problema al actualizar linea de detalle en ODOO");
                            _lg.entryLog($"manufacturing.order.lines/{linea["id"]}");
                            _lg.entryLog($"{ordLn["message"]}");
                            _lg.entryLog(body);
                            resultado = false;
                        }
                        else
                        { resultado = true; }
                    }
                }
            }

            return resultado;
        }

        private static bool clasificaDestinoLineas(apirest _ar, ref string[,] _operaciones, JToken _transit, ref JToken[] _lineasOrden, ref string[,] _piNumber, ref string _fabricante, ref bool _compraDirecta, ref bool _compraLocal, ref string _incoterm, logObj _lg, bool _test, int _noOps)
        {
            JObject ordLn;
            JObject ordLnDt;
            string[] _params;
            string[] temp;
            int indice = 0;
            List<string> piList = new List<string>();
            string manufacturer_id_name = "";
            string manufacturer_id_id = "";
            string criterio;
            string comodin;
            int userId;
            string containerNo;
            int sparePartsPos;
            bool boolSpareParts;

            MySQLobj ms = new MySQLobj(_test);

            try
            {
                for (int i = 0; i <= _noOps; i++)
                { _operaciones[i, 1] = "0"; }

                _lineasOrden = new JToken[int.Parse(_transit["lines_ids"].Children().Count().ToString())];
                _fabricante = "";
                
                foreach (JToken linea in _transit["lines_ids"]) // se verifica los paises de destino en las lineas
                {
                    boolSpareParts = false;
                    _params = new string[0];
                    ordLn = obtieneJSon(_ar.getInfo($"manufacturing.order.lines/{linea["id"]}", _params, _test), _lg);

                    if (!verificaOperacionAPI(ordLn["success"].ToString()))
                    {
                        _lg.entryLog($"Ocurrio un problema al recoger informacion de linea {linea["id"]}");
                        _lg.entryLog($"{ordLn["message"]}");
                        return false;
                    }

                    ordLnDt = obtieneJSon(ordLn["data"][0].ToString(), _lg);

                    if (ordLnDt["product_id"][0]["name"].ToString() != "Spare Parts")
                    {
                        _lineasOrden[indice] = ordLnDt;

                        userId = int.Parse(ordLnDt["create_uid"][0]["id"].ToString());
                        containerNo = ordLnDt["container_number"].ToString();

                        if (ordLnDt["product_id"][0]["name"].ToString() == "Spare Parts")
                        { boolSpareParts = true; }

                        indice++;

                        if (((Newtonsoft.Json.Linq.JContainer)ordLnDt["manufacturer_id"]).Count > 0)
                        {
                            manufacturer_id_name = ordLnDt["manufacturer_id"][0]["name"].ToString();
                            manufacturer_id_id = ordLnDt["manufacturer_id"][0]["id"].ToString();
                        }

                        if (bool.Parse(ordLnDt["is_directly"].ToString()))
                        {
                            _compraDirecta = true;
                            manufacturer_id_name = MySQLobj.getManufacturerSupplyCode(MySQLobj.ConnectionString, "PR_sl_itm_fabricante_pais", ordLnDt["company_to_id"][0]["name"].ToString(), ordLnDt["manufacturer_id"][0]["name"].ToString());
                        }

                        _incoterm = ordLnDt["incoterm"].ToString().ToLower();

                        if (_incoterm == "local" || _incoterm == "landed")
                        {
                            _compraLocal = true; 
                        } else
                        {
                            _compraLocal = false;
                        }
                        

                        criterio = ordLnDt["pi_number"].ToString() + "|" + manufacturer_id_name;

                        if (_fabricante.Length == 0)
                        { _fabricante = manufacturer_id_name; }

                        if (ordLnDt["company_to_id"].ToString() == "[]")
                        {
                            _lg.entryLog($"La línea {ordLnDt["number"]} no tiene operación de destino configurada");
                            return false;
                        }
                        else
                        {
                            switch (ordLnDt["company_to_id"][0]["name"].ToString())
                            {
                                case "GRS-GT":
                                    _operaciones[0, 1] = "1";
                                    criterio += "|GRS-GT";
                                    if (userId == 2) { userId = 8; }
                                    break;
                                case "GRS-HN":
                                    _operaciones[1, 1] = "1";
                                    criterio += "|GRS-HN";
                                    if (userId == 2) { userId = 18; }
                                    break;
                                case "GRS-ES":
                                    _operaciones[2, 1] = "1";
                                    criterio += "|GRS-ES";
                                    if (userId == 2) { userId = 18; }
                                    break;
                                case "GRS-NI":
                                    _operaciones[3, 1] = "1";
                                    if (userId == 2) { userId = 8; }
                                    criterio += "|GRS-NI";
                                    break;
                                case "GRS-CR":
                                    _operaciones[4, 1] = "1";
                                    if (userId == 2) { userId = 8; }
                                    criterio += "|GRS-CR";
                                    break;
                                case "GRS-RD":
                                    _operaciones[5, 1] = "1";
                                    if (userId == 2) { userId = 12; }
                                    criterio += "|GRS-RD";
                                    break;

                                case "GRS-PR":
                                    _operaciones[6, 1] = "1";
                                    if (userId == 2) { userId = 12; }
                                    criterio += "|GRS-PR";
                                    break;

                                case "GRS-GR":
                                    _operaciones[7, 1] = "1";
                                    if (userId == 2) { userId = 12; }
                                    criterio += "|GRS-GR";
                                    break;

                                case "GRS-TT":
                                    _operaciones[8, 1] = "1";
                                    if (userId == 2) { userId = 12; }
                                    criterio += "|GRS-TT";
                                    break;

                                case "GRS-ECU":
                                    _operaciones[9, 1] = "1";
                                    if (userId == 2) { userId = 12; }
                                    criterio += "|GRS-ECU";
                                    break;

                                case "GRS-USA":
                                    _operaciones[10, 1] = "1";
                                    if (userId == 2) { userId = 12;  }
                                    criterio += "|GRS-USA";
                                    break;
                                
                                case "DST-GT":
                                    _operaciones[11, 1] = "1";
                                    if (userId == 2) { userId = 12;  }
                                    criterio += "|DST-GT";
                                    break;

                                default:
                                    _lg.entryLog($"La operación {ordLnDt["company_to_id"][0]["name"]} no se encuentra configurada");
                                    return false;
                                    //break;
                            }

                            criterio += $"|{manufacturer_id_id}|{userId}|{containerNo}|{(boolSpareParts ? 1 : 0)}";

                            if (!piList.Contains(criterio))
                            {
                                piList.Add(criterio);
                                _lg.entryLog($"{criterio}\n-*-");
                            }
                        }
                    }
                }

                _params = piList.ToArray();
                _piNumber = new string[_params.Length, 12];
                indice = 0;
                criterio = "";
                comodin = "";

                foreach (string p in _params)
                {
                    temp = p.Split("|");
                    _piNumber[indice, 0] = temp[0]; // PI number
                    _piNumber[indice, 1] = temp[1]; // Fabricante nombre
                    _piNumber[indice, 2] = "";
                    _piNumber[indice, 3] = "";
                    _piNumber[indice, 4] = "";
                    _piNumber[indice, 5] = temp[2]; // operacion 
                    _piNumber[indice, 6] = temp[3]; // Fabricante codigo 
                    _piNumber[indice, 7] = "";
                    _piNumber[indice, 8] = "";
                    _piNumber[indice, 9] = temp[4]; // Creador de la línea 
                    _piNumber[indice, 10] = temp[5]; // Contenedor 
                    _piNumber[indice, 11] = temp[6]; // Spare parts 

                    if (temp[6] == "1")
                    {
                        criterio += $"{comodin}{indice}";
                        comodin = ",";
                    }

                    indice++;
                }

                if (criterio.Length > 0)
                {
                    temp = criterio.Split(",");

                    foreach (string i in temp)
                    {
                        sparePartsPos = int.Parse(i);

                        for (int pos = 0; pos <= _piNumber.GetUpperBound(0); pos++)
                        {
                            if (pos != sparePartsPos)
                            {
                                if ((_piNumber[pos, 10] == _piNumber[sparePartsPos, 10]) && (_piNumber[pos, 11] == "0"))
                                {
                                    _piNumber[sparePartsPos, 0] = _piNumber[pos, 0]; // PI number
                                    _piNumber[sparePartsPos, 1] = _piNumber[pos, 1]; // Fabricante nombre
                                    _piNumber[sparePartsPos, 5] = _piNumber[pos, 5]; // operacion 
                                    _piNumber[sparePartsPos, 6] = _piNumber[pos, 6]; // Fabricante codigo 
                                    pos = _piNumber.GetUpperBound(0) + 1;
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _lg.entryLog($"execute[ clasificaDestinoLineas: \n{ex.Message}");
                return false;
            }
        }

        private static JObject obtieneJSon(string _origen, logObj _lg)
        {
            JObject jObj;

            try
            {
                using (StringReader srd = new StringReader(_origen))
                {
                    using (JsonTextReader reader = new JsonTextReader(srd))
                    {
                        jObj = (JObject)JToken.ReadFrom(reader);
                    }
                }
                return jObj;
            }
            catch (Exception ex)
            {
                _lg.entryLog($"execute[ obtieneJSon (string, string, string): {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// verifica creador del documento en ODOO y su equivalente en SAP
        /// </summary>
        /// <param name="_transit">información del transito</param>
        /// <param name="_createUid">creador del transito</param>
        /// <param name="_operacion">operacion a que se dirige el transito</param>
        /// <param name="_lg">bitacora de eventos</param>
        /// <param name="_test">apuntar a ambiente pruebas?</param>
        /// <param name="_userSAP">usuario de SAP correspondiente al creador del transito</param>
        private static bool verificaCD(JToken _transit, int _createUid, string _operacion, logObj _lg, bool _test, ref int _userSAP)
        {
            bool evaluacion = false;

            try
            {
                _userSAP = SAPHana.getInfo("PR_SL_USUARIO_ODOO_SAP", _createUid, _operacion, _test);

                if (_userSAP > 0)
                { evaluacion = true; }
                else
                { SAPHana.registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, 5, $"Creador del documento {_createUid} no definido o mapeado en SAP HANA", _test, _lg); }
                                                                        }
            catch (Exception ex)
            {
                SAPHana.registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, 5, $"verificaCD: {ex.Source} - {ex.Message}", _test, _lg);
            }

            return evaluacion;

        }

        private static bool verificaBL(JToken _transit, string _operacion, string _estadoTransito, logObj _lg, bool _test)
        {
            DateTime fecha;
            bool evaluacion = false;
            int codigo = 8;

            try
            {
                if (_transit["state"].ToString() == _estadoTransito)
                {
                    if (_transit["bl_number"].ToString().ToLower() != "false") 
                    {
                        codigo = 6;

                        if (_transit["eta"].ToString().ToLower() != "false")
                        {
                            fecha = DateTime.Parse(_transit["eta"].ToString());
                            evaluacion = true;
                        }
                        else
                        { SAPHana.registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, codigo, "Sin ETA definido", _test, _lg); }
                    }
                    else
                    { SAPHana.registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, codigo, "Sin No. BL definido", _test, _lg); }
                }
            }
            catch (Exception ex)
            {
                SAPHana.registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, codigo, $"verificaBL: {ex.Source} - {ex.Message}", _test, _lg);
            }

            return evaluacion;
        }

        private static bool verificaDetalle(JToken _transit, string _operacion, string _estadoTransito, logObj _lg, bool _test)
        {
            bool evaluacion = false;

            try
            {
                if (_transit["state"].ToString() == _estadoTransito)
                {
                    if (!(_transit["lines_ids"] is null)) { evaluacion = true; }
                    else 
                    {
                        SAPHana.registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, 7, "Sin detalle definido", _test, _lg);
                    }
                }
            }
            catch (Exception ex) 
            {
                SAPHana.registraBitacora(_transit["id"].ToString(), _transit["name"].ToString(), _operacion, 7, $"verificaDetalle: {ex.Source} - {ex.Message}", _test, _lg);
            }

            return evaluacion;
        }
        
        private static bool verificaOperacionAPI(string _success)
        {
            if (_success.ToLower() == "true") { return true; }
            else { return false; }
        }

        #endregion
    }
}
