using System;
using Npgsql;
using System.Data;

using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO;
using ClosedXML.Excel;


namespace ordersNTransfers
{
    class postgreSQLobj
    {
        static string connectionString;
        static string cnnStatus;

        public postgreSQLobj(Boolean _test)
        {
            string database;

            if (_test) { database = ordersNTransfers.Properties.Resources.dbname; }
            else { database = ordersNTransfers.Properties.Resources.dbname; }

            //postgreSQLobj.connectionString = $"Persist Security Info = False; database= {database}; server= {trasladoPedidoCRM.Properties.Resources.MSsrv}; user id= {trasladoPedidoCRM.Properties.Resources.MSui}; Password = {trasladoPedidoCRM.Properties.Resources.MSpwd}; Port= {trasladoPedidoCRM.Properties.Resources.MSprt}";
            if (_test)
            {
                postgreSQLobj.connectionString = $"Host = 192.168.11.48; port=5432; Database = OMS15; Username = odoo; Password = AdminOdoo2022@@##";
                postgreSQLobj.cnnStatus = "OK";
            } else
            {
                postgreSQLobj.connectionString = $"Host = 192.168.11.44; port=5432; Database = {database}; Username = odoo; Password = AdminOdoo2022@@##";
                postgreSQLobj.cnnStatus = "OK";
            }
        }

        public static DataTable cierraTransitos(string _connectionString, string _sp)
        {
            DataTable dt = new DataTable();

            try
            {
                using (NpgsqlConnection sqlCon = new NpgsqlConnection(_connectionString))
                {
                    using (NpgsqlDataAdapter SqlDa = new NpgsqlDataAdapter())
                    {
                        sqlCon.Open();

                        using (NpgsqlCommand cmm = new NpgsqlCommand($"select * from {_sp}();", sqlCon))
                        {
                            cmm.CommandType = CommandType.Text;
                            SqlDa.SelectCommand = cmm;
                            SqlDa.Fill(dt);
                        }
                        sqlCon.Close();
                    }
                }
                postgreSQLobj.cnnStatus = "Procedimiento " + _sp + " ejecutado con exito. ";
                return dt;
            }
            catch (Exception ex)
            {
                postgreSQLobj.cnnStatus = "Ocurrio un problema al ejecutar [" + _sp + "]: " + ex.Message;
                dt.Dispose();
                return null;
            }
        }

        public static DataTable getInfo(string _connectionString, string _sp, int _categoria, string _compania)
        {
            DataTable dt = new DataTable();

            try
            {
                using (NpgsqlConnection sqlCon = new NpgsqlConnection(_connectionString))
                {
                    using (NpgsqlDataAdapter SqlDa = new NpgsqlDataAdapter())
                    {
                        sqlCon.Open();

                        using (NpgsqlCommand cmm = new NpgsqlCommand(_sp, sqlCon))
                        {
                            cmm.CommandType = CommandType.StoredProcedure;
                            cmm.Parameters.AddWithValue("_categ_id", _categoria);
                            cmm.Parameters.AddWithValue("_operacion", _compania);
                            SqlDa.SelectCommand = cmm;
                            SqlDa.Fill(dt);
                        }
                        sqlCon.Close();
                    }
                }
                postgreSQLobj.cnnStatus = "Procedimiento " + _sp + " ejecutado con exito. ";

                return dt;
            }
            catch (Exception ex)
            {
                postgreSQLobj.cnnStatus = "Ocurrio un problema al ejecutar [" + _sp + "]: " + ex.Message;
                dt.Dispose();
                return null;
            }
        }

        public static DataTable getInfo(string _connectionString, string _sp, int _id_linea)
        {
            DataTable dt = new DataTable();

            try
            {
                using (NpgsqlConnection sqlCon = new NpgsqlConnection(_connectionString))
                {
                    using (NpgsqlDataAdapter SqlDa = new NpgsqlDataAdapter())
                    {
                        sqlCon.Open();

                        using (NpgsqlCommand cmm = new NpgsqlCommand(_sp, sqlCon))
                        {
                            cmm.CommandType = CommandType.StoredProcedure;
                            cmm.Parameters.AddWithValue("id_linea", _id_linea);
                            SqlDa.SelectCommand = cmm;
                            SqlDa.Fill(dt);
                        }
                        sqlCon.Close();
                    }
                }
                postgreSQLobj.cnnStatus = "Procedimiento " + _sp + " ejecutado con exito. ";
                return dt;
            }
            catch (Exception ex)
            {
                postgreSQLobj.cnnStatus = "Ocurrio un problema al ejecutar [" + _sp + "]: " + ex.Message;
                dt.Dispose();
                return null;
            }
        }

        public static JObject getManufacturingOrderShippingOverview(string _connectionString)
        {
            DataTable dt = new DataTable();

            try
            {
                using (NpgsqlConnection sqlCon = new NpgsqlConnection(_connectionString))
                {
                    using (NpgsqlDataAdapter SqlDa = new NpgsqlDataAdapter())
                    {
                        sqlCon.Open();

                        using (NpgsqlCommand cmm = new NpgsqlCommand("SELECT * FROM manufacturing_order_shipping_overview;", sqlCon))
                        {
                            cmm.CommandType = CommandType.Text;
                            SqlDa.SelectCommand = cmm;
                            SqlDa.Fill(dt);
                        }
                        sqlCon.Close();
                    }
                }

                // Agrupación de datos
                var groupedData = dt.AsEnumerable()
                    .GroupBy(row => new
                    {
                        Id = row["id"],
                        OrderName = row["order_name"],
                        Name = row["name"],
                        DateOrder = row["date_order"],
                        State = row["state"],
                        BookingNumber = row["booking_number"],
                        BlNumber = row["bl_number"],
                        Notes = row["notes"],
                        Eta = row["eta"],
                        NewEta = row["new_eta"],
                        Etd = row["etd"],
                        PurchaseOrder = row["purchase_order"],
                        InvoiceComercial = row["invoice_comercial"],
                        CourierGuide = row["courier_guide"],
                        Incoterm = row["incoterm"],
                        CompanyToId = row["company_to_id"],
                        CompanyToName = row["company_to_name"],
                        CountryToId = row["country_to_id"],
                        CountryToName = row["country_to_name"],
                        IsCfrCheck = row["is_cfr_check"],
                        CfrFreight = row["cfr_freight"]
                    })
                    .Select(group => new JObject
                    {
                        ["id"] = group.Key.Id != DBNull.Value ? Convert.ToInt32(group.Key.Id) : 0,
                        ["order_name"] = group.Key.OrderName != DBNull.Value ? group.Key.OrderName.ToString() : null,
                        ["name"] = group.Key.Name != DBNull.Value ? group.Key.Name.ToString() : null,
                        ["date_order"] = group.Key.DateOrder != DBNull.Value ? Convert.ToDateTime(group.Key.DateOrder) : (DateTime?)null,
                        ["state"] = group.Key.State != DBNull.Value ? group.Key.State.ToString() : null,
                        ["booking_number"] = group.Key.BookingNumber != DBNull.Value ? group.Key.BookingNumber.ToString() : null,
                        ["bl_number"] = group.Key.BlNumber != DBNull.Value ? group.Key.BlNumber.ToString() : null,
                        ["notes"] = group.Key.Notes != DBNull.Value ? group.Key.Notes.ToString() : null,
                        ["eta"] = group.Key.Eta != DBNull.Value ? Convert.ToDateTime(group.Key.Eta) : (DateTime?)null,
                        ["new_eta"] = group.Key.NewEta != DBNull.Value ? Convert.ToDateTime(group.Key.NewEta) : (DateTime?)null,
                        ["etd"] = group.Key.Etd != DBNull.Value ? Convert.ToDateTime(group.Key.Etd) : (DateTime?)null,
                        ["purchase_order"] = group.Key.PurchaseOrder != DBNull.Value ? group.Key.PurchaseOrder.ToString() : null,
                        ["invoice_comercial"] = group.Key.InvoiceComercial != DBNull.Value ? group.Key.InvoiceComercial.ToString() : null,
                        ["courier_guide"] = group.Key.CourierGuide != DBNull.Value ? group.Key.CourierGuide.ToString() : null,
                        ["incoterm"] = group.Key.Incoterm != DBNull.Value ? group.Key.Incoterm.ToString() : null,
                        ["company_to_id"] = group.Key.CompanyToId != DBNull.Value ? Convert.ToInt32(group.Key.CompanyToId) : 0,
                        ["company_to_name"] = group.Key.CompanyToName != DBNull.Value ? group.Key.CompanyToName.ToString() : null,
                        ["country_to_id"] = group.Key.CountryToId != DBNull.Value ? Convert.ToInt32(group.Key.CountryToId) : 0,
                        ["country_to_name"] = group.Key.CountryToName != DBNull.Value ? group.Key.CountryToName.ToString() : null,
                        ["is_cfr_check"] = group.Key.IsCfrCheck != DBNull.Value ? Convert.ToBoolean(group.Key.IsCfrCheck) : false,
                        ["cfr_freight"] = group.Key.CfrFreight != DBNull.Value ? Convert.ToDouble(group.Key.IsCfrCheck) : 0.00,
                        ["lines_ids"] = new JArray(
                            group.Select(row => new JObject
                            {
                                ["id"] = row["lines_id"] != DBNull.Value ? Convert.ToInt32(row["lines_id"]) : 0,
                                ["name"] = row["lines_name"] != DBNull.Value ? row["lines_name"].ToString() : null
                            })
                        ),
                        ["company_to_id"] = new JArray(
                            group.Select(row => new JObject
                            {
                                ["id"] = row["company_to_id"] != DBNull.Value ? Convert.ToInt32(row["company_to_id"]) : 0,
                                ["name"] = row["company_to_name"] != DBNull.Value ? row["company_to_name"].ToString() : null
                            })
                        ),
                        ["country_to_id"] = new JArray(
                            group.Select(row => new JObject
                            {
                                ["id"] = row["country_to_id"] != DBNull.Value ? Convert.ToInt32(row["country_to_id"]) : 0,
                                ["name"] = row["country_to_name"] != DBNull.Value ? row["country_to_name"].ToString() : null
                            })
                        ),
                        ["shipper_id"] = new JArray(
                            group.Select(row => new JObject
                            {
                                ["id"] = row["shipper_id"] != DBNull.Value ? Convert.ToInt32(row["shipper_id"]) : 0,
                                ["name"] = row["shipper_name"] != DBNull.Value ? row["shipper_name"].ToString() : null
                            })
                        ),
                        ["pol_id"] = new JArray(
                            group.Select(row => new JObject
                            {
                                ["id"] = row["pol_id"] != DBNull.Value ? Convert.ToInt32(row["pol_id"]) : 0,
                                ["name"] = row["pol_name"] != DBNull.Value ? row["pol_name"].ToString() : null
                            })
                        )
                    });

                // Crear el objeto final con "success" y "data"
                JObject result = new JObject
                {
                    ["success"] = groupedData.Any(),
                    ["data"] = new JArray(groupedData)
                };

                return result;
            }
            catch (Exception ex)
            {
                postgreSQLobj.cnnStatus = "Ocurrió un problema al ejecutar la vista manufacturing_order_shipping_overview: " + ex.Message;
                return new JObject
                {
                    ["success"] = false,
                    ["error"] = ex.Message,
                    ["data"] = new JArray()
                };
            }
        }


        public static JObject GetManufacturingOrderShippingOverviewFromFile(string filePath)
        {
            DataTable dt = new DataTable();

            try
            {
                dt = CargarDatosDesdeArchivo(filePath);
                if (dt.Rows.Count == 0)
                {
                    return new JObject
                    {
                        ["success"] = false,
                        ["error"] = "No se encontraron datos en el archivo.",
                        ["data"] = new JArray()
                    };
                }

                // Agrupación de datos
                var groupedData = dt.AsEnumerable()
                    .GroupBy(row => new
                    {
                        Id = row["id"],
                        OrderName = row["order_name"],
                        Name = row["name"],
                        DateOrder = row["date_order"],
                        State = row["state"],
                        BookingNumber = row["booking_number"],
                        BlNumber = row["bl_number"],
                        Notes = row["notes"],
                        Eta = row["eta"],
                        NewEta = row["new_eta"],
                        Etd = row["etd"],
                        PurchaseOrder = row["purchase_order"],
                        InvoiceComercial = row["invoice_comercial"],
                        CourierGuide = row["courier_guide"],
                        Incoterm = row["incoterm"],
                        CompanyToId = row["company_to_id"],
                        CompanyToName = row["company_to_name"],
                        CountryToId = row["country_to_id"],
                        CountryToName = row["country_to_name"],
                        IsCfrCheck = row["is_cfr_check"],
                        CfrFreight = row["cfr_freight"]
                    })
                    .Select(group => new JObject
                    {
                        ["id"] = Convert.ToInt32(group.Key.Id),
                        ["order_name"] = group.Key.OrderName.ToString(),
                        ["name"] = group.Key.Name.ToString(),
                        ["date_order"] = Convert.ToDateTime(group.Key.DateOrder),
                        ["state"] = group.Key.State.ToString(),
                        ["booking_number"] = group.Key.BookingNumber.ToString(),
                        ["bl_number"] = group.Key.BlNumber.ToString(),
                        ["notes"] = group.Key.Notes.ToString(),
                        ["eta"] = Convert.ToDateTime(group.Key.Eta),
                        ["new_eta"] = Convert.ToDateTime(group.Key.NewEta),
                        ["etd"] = Convert.ToDateTime(group.Key.Etd),
                        ["purchase_order"] = group.Key.PurchaseOrder.ToString(),
                        ["invoice_comercial"] = group.Key.InvoiceComercial.ToString(),
                        ["courier_guide"] = group.Key.CourierGuide.ToString(),
                        ["incoterm"] = group.Key.Incoterm.ToString(),
                        ["company_to_id"] = Convert.ToInt32(group.Key.CompanyToId),
                        ["company_to_name"] = group.Key.CompanyToName.ToString(),
                        ["country_to_id"] = Convert.ToInt32(group.Key.CountryToId),
                        ["country_to_name"] = group.Key.CountryToName.ToString(),
                        ["is_cfr_check"] = Convert.ToBoolean(group.Key.IsCfrCheck),
                        ["cfr_freight"] = Convert.ToDouble(group.Key.CfrFreight),
                        ["lines_ids"] = new JArray(
                            group.Select(row => new JObject
                            {
                                ["id"] = row["lines_id"] != DBNull.Value ? Convert.ToInt32(row["lines_id"]) : 0,
                                ["name"] = row["lines_name"] != DBNull.Value ? row["lines_name"].ToString() : null
                            })
                        ),
                        ["company_to_id"] = new JArray(
                            group.Select(row => new JObject
                            {
                                ["id"] = Convert.ToInt32(row["company_to_id"]),
                                ["name"] = row["company_to_name"].ToString()
                            })
                        ),
                        ["country_to_id"] = new JArray(
                            group.Select(row => new JObject
                            {
                                ["id"] = Convert.ToInt32(row["country_to_id"]),
                                ["name"] = row["country_to_name"].ToString()
                            })
                        ),
                        ["shipper_id"] = new JArray(
                            group.Select(row => new JObject
                            {
                                ["id"] = Convert.ToInt32(row["shipper_id"]),
                                ["name"] = row["shipper_name"].ToString()
                            })
                        ),
                        ["pol_id"] = new JArray(
                            group.Select(row => new JObject
                            {
                                ["id"] = Convert.ToInt32(row["pol_id"]),
                                ["name"] = row["pol_name"].ToString()
                            })
                        )
                    });

                // Crear el objeto final con "success" y "data"
                JObject result = new JObject
                {
                    ["success"] = groupedData.Any(),
                    ["data"] = new JArray(groupedData)
                };

                return result;
            }
            catch (Exception ex)
            {
                return new JObject
                {
                    ["success"] = false,
                    ["error"] = ex.Message,
                    ["data"] = new JArray()
                };
            }
        }

        static DataTable CargarDatosDesdeArchivo(string filePath)
        {
            DataTable dt = new DataTable();

            if (filePath.EndsWith(".csv"))
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string[] headers = sr.ReadLine().Split(',');
                    foreach (string header in headers)
                        dt.Columns.Add(header.Trim());

                    while (!sr.EndOfStream)
                    {
                        string[] rows = sr.ReadLine().Split(',');
                        dt.Rows.Add(rows);
                    }
                }
            }
            else if (filePath.EndsWith(".xlsx")) // Leer Excel con ClosedXML
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RowsUsed();

                    foreach (var cell in rows.First().CellsUsed())
                        dt.Columns.Add(cell.Value.ToString());

                    foreach (var row in rows.Skip(1))
                    {
                        DataRow newRow = dt.NewRow();
                        int columnIndex = 0;
                        foreach (var cell in row.CellsUsed())
                        {
                            newRow[columnIndex] = cell.Value.ToString();
                            columnIndex++;
                        }
                        dt.Rows.Add(newRow);
                    }
                }
            }

            return dt;
        }



        public static string ConnectionString { get => connectionString; set => connectionString = value; }

        public static string CnnStatus { get => cnnStatus; set => cnnStatus = value; }

    }
}
