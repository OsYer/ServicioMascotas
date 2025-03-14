using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Npgsql;
using ServicioMascotas.Models;

namespace ServicioMascotas
{
	// NOTA: puede usar el comando "Rename" del menú "Refactorizar" para cambiar el nombre de clase "Mascotas" en el código, en svc y en el archivo de configuración a la vez.
	// NOTA: para iniciar el Cliente de prueba WCF para probar este servicio, seleccione Mascotas.svc o Mascotas.svc.cs en el Explorador de soluciones e inicie la depuración.
	public class Mascotas : IMascotas
	{
        private string _connectionString = ConfigurationManager.ConnectionStrings["PostgreSQLConn"].ConnectionString;
                public List<Mascota> ObtenerMascotas()
        {
            List<Mascota> lista = new List<Mascota>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT id, nombre, especie, raza, edad, peso, sexo, id_usuario FROM mascotas";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Mascota
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Especie = reader.GetString(2),
                            Raza = reader.GetString(3),
                            Edad = reader.GetInt32(4),
                            Peso = reader.GetDecimal(5),
                            Sexo = reader.GetChar(6),
                            IdUsuario = reader.GetInt32(7)
                        });
                    }
                }
            }

            return lista;
        }

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
