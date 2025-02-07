using System;
using System.IO;

namespace ordersNTransfers
{
    class logObj
    {
        private static string rutaLog;

        public logObj(string _rutaLog)
        {
            StreamWriter sw;

            if (_rutaLog.Length > 0)
            {
                logObj.rutaLog = _rutaLog;

                if (!File.Exists(_rutaLog))
                {
                    sw = new StreamWriter(_rutaLog, false);
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// envia entrada al archivo log
        /// </summary>
        /// <param name="_entrada">información a publicar en el log
        /// <param name="_incluirEspacio">incluir una linea vacia luego de la información
        public void entryLog(string _entrada, Boolean _incluirEspacio = false)
        {
            StreamWriter sw = new StreamWriter(logObj.rutaLog, true);
            Console.WriteLine(_entrada);
            sw.WriteLine(_entrada);

            if (_incluirEspacio)
            {
                Console.WriteLine("");
                sw.WriteLine("");
            }
            sw.Close();
        }
    }
}
