using System;
using RestSharp;

namespace ordersNTransfers
{
    class apirest
    {

        public string getInf ()
        {
            // version 13
            //var client = new RestClient($"http://190.111.2.117:8086/api/manufacturing.order/search?domain=[('state','=','checking')]&fields=['id','name','date_order','company_id','company_to_id', 'manufacturer_id','state']");

            var client = new RestClient($"http://52.162.252.13:8086/api/manufacturing.order/search?domain=[('state','=','checking')]&fields=['id','name','date_order','company_id','company_to_id', 'manufacturer_id','state']");


            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("api_key", "L1QSL42PPMSIAYGBCMIWSNMZJJ812Z6R");
            request.AddHeader("db_name", "grs_tests");
            IRestResponse response = client.Execute(request);
            return response.Content;
        }


        public string getFullInfo(string _recurso, string _idRecurso)
        {
            IRestClient cliente;
            RestRequest peticion;
            IRestResponse response;
            string respuesta;
            string preUrl;

            try
            {
                preUrl = $"http://{ordersNTransfers.Properties.Resources.hostname}/api/{_recurso}/{_idRecurso}";

                var builder = new UriBuilder(preUrl);
                string url = builder.ToString();

                cliente = new RestClient();
                cliente = new RestClient(url);
                cliente.Timeout = -1;
                peticion = new RestRequest(Method.GET);
                peticion.AddHeader("api_key", ordersNTransfers.Properties.Resources.key);
                peticion.AddHeader("db_name", ordersNTransfers.Properties.Resources.dbname);

                response = cliente.Execute(peticion);
                respuesta = response.Content;
            }
            catch (Exception ex)
            {
                respuesta = $"Ocurrió un problema al procesar la peticion {_recurso}: {ex.Message}";
            }

            return respuesta;
        }

        public string getInfo(string _recurso, string[] _params, bool _test)
        {
            IRestClient cliente;
            RestRequest peticion;
            IRestResponse response;
            string respuesta;
            string preUrl;
            bool primera;

            try
            {
                if (_test)
                { preUrl = $"http://192.168.11.48:8069/api/{_recurso}"; }
                else
                { preUrl = $"http://{ordersNTransfers.Properties.Resources.hostname}/api/{_recurso}"; }

                primera = true;

                foreach (string _param in _params)
                {
                    if (primera) 
                    { 
                        preUrl += "?";
                        primera = false;
                    }
                    else { preUrl += "&"; }

                    preUrl += _param;
                }

                var builder = new UriBuilder(preUrl);
                string url = builder.ToString();

                cliente = new RestClient();
                cliente = new RestClient(url);
                cliente.Timeout = -1;
                peticion = new RestRequest(Method.GET);

                if (_test)
                {
                    peticion.AddHeader("api_key", ordersNTransfers.Properties.Resources.key);
                    peticion.AddHeader("db_name", "OMS15");
                }
                else
                {
                    peticion.AddHeader("api_key", ordersNTransfers.Properties.Resources.key);
                    peticion.AddHeader("db_name", ordersNTransfers.Properties.Resources.dbname);
                }

                response = cliente.Execute(peticion);

                respuesta = response.Content;

            }
            catch (Exception ex)
            {
                respuesta = $"Ocurrió un problema al procesar la peticion {_recurso}: {ex.Message}";
            }

            return respuesta;
        }

        public string putInfo(string _recurso, int _registro, string _body) 
        {
            IRestClient cliente;
            RestRequest peticion;
            IRestResponse response;
            string respuesta;
            string preUrl;

            try
            {
                preUrl = $"http://{ordersNTransfers.Properties.Resources.hostname}/api/{_recurso}/{_registro}";
                var builder = new UriBuilder(preUrl);
                string url = builder.ToString();

                cliente = new RestClient();
                cliente = new RestClient(url);
                cliente.Timeout = -1;
                peticion = new RestRequest(Method.PUT);

                peticion.AddHeader("api_key", ordersNTransfers.Properties.Resources.key);
                peticion.AddHeader("db_name", ordersNTransfers.Properties.Resources.dbname);
                peticion.AddHeader("Content-Type", "text/plain");
                peticion.AddParameter("text/plain", _body, ParameterType.RequestBody);

                response = cliente.Execute(peticion);
                respuesta = response.Content;
            }
            catch (Exception ex)
            {
                respuesta = $"Ocurrió un problema al procesar la peticion {_recurso}: {ex.Message}";
            }

            return respuesta;
        }

    }
}
