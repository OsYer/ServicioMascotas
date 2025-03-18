using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
        public bool AgregarMascota(Mascota mascota)
            {
            using (var conn = new NpgsqlConnection(_connectionString))
                {
                conn.Open();
                string query = "INSERT INTO mascotas (nombre, especie, raza, edad, peso, sexo, id_usuario) VALUES (@nombre, @especie, @raza, @edad, @peso, @sexo, @id_usuario)";

                using (var cmd = new NpgsqlCommand(query, conn))
                    {
                    cmd.Parameters.AddWithValue("@nombre", mascota.Nombre);
                    cmd.Parameters.AddWithValue("@especie", mascota.Especie);
                    cmd.Parameters.AddWithValue("@raza", mascota.Raza);
                    cmd.Parameters.AddWithValue("@edad", mascota.Edad);
                    cmd.Parameters.AddWithValue("@peso", mascota.Peso);
                    cmd.Parameters.AddWithValue("@sexo", mascota.Sexo);
                    cmd.Parameters.AddWithValue("@id_usuario", mascota.IdUsuario);

                    return cmd.ExecuteNonQuery() > 0; // Retorna true si la inserción fue exitosa
                    }
                }
            }
        public bool EliminarMascota(Mascota mascota)
            {
            Console.WriteLine("Solicitud recibida en EliminarMascota");

            if (mascota == null || mascota.Id <= 0)
                {
                return false; // Devuelve false si el objeto es nulo o el ID no es válido
                }

            using (var conn = new NpgsqlConnection(_connectionString))
                {
                conn.Open();
                string query = "DELETE FROM mascotas WHERE id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                    {
                    cmd.Parameters.AddWithValue("@id", mascota.Id);
                    return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
        public bool ActualizarMascota(Mascota mascota)
            {
            using (var conn = new NpgsqlConnection(_connectionString))
                {
                conn.Open();
                string query = "UPDATE mascotas SET nombre = @nombre, especie = @especie, raza = @raza, edad = @edad, peso = @peso, sexo = @sexo, id_usuario = @id_usuario WHERE id = @id";

                using (var cmd = new NpgsqlCommand(query, conn))
                    {
                    cmd.Parameters.AddWithValue("@id", mascota.Id);
                    cmd.Parameters.AddWithValue("@nombre", mascota.Nombre);
                    cmd.Parameters.AddWithValue("@especie", mascota.Especie);
                    cmd.Parameters.AddWithValue("@raza", mascota.Raza);
                    cmd.Parameters.AddWithValue("@edad", mascota.Edad);
                    cmd.Parameters.AddWithValue("@peso", mascota.Peso);
                    cmd.Parameters.AddWithValue("@sexo", mascota.Sexo);
                    cmd.Parameters.AddWithValue("@id_usuario", mascota.IdUsuario);

                    return cmd.ExecuteNonQuery() > 0; // Retorna true si la actualización fue exitosa
                    }
                }
            }
        public List<Mascota> ObtenerMascotas()
            {
            List<Mascota> lista = new List<Mascota>();

            using (var conn = new NpgsqlConnection(_connectionString))
                {
                conn.Open();
                string query = "SELECT * FROM mascotas";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                    {
                    lista = (from IDataRecord r in reader
                             select new Mascota
                                 {
                                 Id = r.GetInt32(0),
                                 Nombre = r.GetString(1),
                                 Especie = r.GetString(2),
                                 Raza = r.GetString(3),
                                 Edad = r.GetInt32(4),
                                 Peso = r.GetDecimal(5),
                                 Sexo = r.GetChar(6),
                                 IdUsuario = r.GetInt32(7)
                                 }).ToList();
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
