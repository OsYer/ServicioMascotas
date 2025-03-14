using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Npgsql;

namespace ServicioMascotas
{
	// NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Mascotas" en el código, en svc y en el archivo de configuración a la vez.
	// NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Mascotas.svc o Mascotas.svc.cs en el Explorador de soluciones e inicie la depuración.
	public class Mascotas : IMascotas
	{
        private string _connectionString = ConfigurationManager.ConnectionStrings["PostgreSQLConn"].ConnectionString;

        public string ProbarConexion()
        {
            try
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    return "¡Conexión exitosa a PostgreSQL!";
                }
            }
            catch (Exception ex)
            {
                return "Error de conexión: " + ex.Message;
            }
        }
    }
}
