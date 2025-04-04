using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using Npgsql;
using ServicioMascotas.Models;

namespace ServicioMascotas
{
    [ServiceBehavior]
    public class Mascotas : IMascotas
    {
        private string _connectionString = "Host=192.168.15.225;Port=5432;Username=postgres;Password=admin;Database=mascotas_db;";
        public void HandleOptionsRequest()
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
        }


        private NpgsqlConnection ObtenerConexion()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public bool AgregarMascota(Mascota mascota)
        {
            if (mascota == null) return false;

            const string query = "INSERT INTO mascotas (nombre, especie, raza, edad, peso, sexo, id_usuario, activo) VALUES (@nombre, @especie, @raza, @edad, @peso, @sexo, @id_usuario, true)";

            try
            {
                using (var conn = ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", mascota.Nombre);
                        cmd.Parameters.AddWithValue("@especie", mascota.Especie);
                        cmd.Parameters.AddWithValue("@raza", mascota.Raza);
                        cmd.Parameters.AddWithValue("@edad", mascota.Edad);
                        cmd.Parameters.AddWithValue("@peso", mascota.Peso);
                        cmd.Parameters.AddWithValue("@sexo", mascota.Sexo);
                        cmd.Parameters.AddWithValue("@id_usuario", mascota.IdUsuario);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en AgregarMascota: {ex.Message}");
                return false;
            }
        }

        public bool EliminarMascota(Mascota mascota)
        {
            if (mascota == null || mascota.Id <= 0) return false;

            const string query = "UPDATE mascotas SET activo = false, fecha_edicion = NOW() WHERE id = @id";
            try
            {
                using (var conn = ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", mascota.Id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en EliminarMascota: {ex.Message}");
                return false;
            }
        }

        public bool ActualizarMascota(Mascota mascota)
        {
            if (mascota == null || mascota.Id <= 0) return false;

            const string query = "UPDATE mascotas SET nombre = @nombre, especie = @especie, raza = @raza, edad = @edad, peso = @peso, sexo = @sexo, fecha_edicion = NOW() WHERE id = @id";

            try
            {
                using (var conn = ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", mascota.Id);
                        cmd.Parameters.AddWithValue("@nombre", mascota.Nombre);
                        cmd.Parameters.AddWithValue("@especie", mascota.Especie);
                        cmd.Parameters.AddWithValue("@raza", mascota.Raza);
                        cmd.Parameters.AddWithValue("@edad", mascota.Edad);
                        cmd.Parameters.AddWithValue("@peso", mascota.Peso);
                        cmd.Parameters.AddWithValue("@sexo", mascota.Sexo);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarMascota: {ex.Message}");
                return false;
            }
        }

        public List<Mascota> ObtenerMascotas()
        {
            List<Mascota> lista = new List<Mascota>();

            const string query = "SELECT * FROM mascotas WHERE activo = true";

            try
            {
                using (var conn = ObtenerConexion())
                {
                    conn.Open();
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
                                     IdUsuario = r.GetInt32(7),
                                     FechaRegistro = r.GetDateTime(8),
                                     FechaEdicion = r.IsDBNull(9) ? (DateTime?)null : r.GetDateTime(9),
                                     Activo = r.GetBoolean(10)
                                 }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerMascotas: {ex.Message}");
            }

            return lista;
        }
        public List<Mascota> ObtenerMascotasActualizadas(string fecha)
        {
            List<Mascota> lista = new List<Mascota>();

            DateTime desde;
            if (!DateTime.TryParse(fecha, out desde))
            {
                return lista; // Retorna vacío si la fecha es inválida
            }

            const string query = @"
        SELECT * 
        FROM mascotas 
        WHERE 
            (fecha_edicion > @desde OR fecha_registro > @desde)
            AND activo = true";

            try
            {
                using (var conn = ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@desde", desde);

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
                                         IdUsuario = r.GetInt32(7),
                                         FechaRegistro = r.GetDateTime(8),
                                         FechaEdicion = r.IsDBNull(9) ? (DateTime?)null : r.GetDateTime(9),
                                         Activo = r.GetBoolean(10)
                                     }).ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerMascotasActualizadas: {ex.Message}");
            }

            return lista;
        }
        public List<int> ObtenerMascotasEliminadas(string fecha)
        {
            List<int> eliminadas = new List<int>();

            if (!DateTime.TryParse(fecha, out DateTime desde))
                return eliminadas;

            const string query = @"
        SELECT id 
        FROM mascotas 
        WHERE activo = false AND fecha_edicion > @desde";

            try
            {
                using (var conn = ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@desde", desde);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                eliminadas.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerMascotasEliminadas: {ex.Message}");
            }

            return eliminadas;
        }

        public string ProbarConexion()
        {
            try
            {
                using (var conn = ObtenerConexion())
                {
                    conn.Open();
                    return "¡Conexión exitosa a PostgreSQL!";
                }
            }
            catch (Exception ex)
            {
                return $"Error de conexión: {ex.Message}";
            }
        }
    }
}
