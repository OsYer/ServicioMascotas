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
        private string _connectionString = "Host=192.168.15.225;Port=5432;Username=postgres;Password=admin;Database=servicio_mascotas;";
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

            const string query = @"INSERT INTO mascotas (nombre, especie, raza, edad, peso, sexo, id_usuario, fecha_registro, fecha_edicion, activo) VALUES (@nombre, @especie, @raza, @edad, @peso, @sexo, @id_usuario, @fecha_registro, @fecha_edicion, true)";

            try
            {
                using (var conn = ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        DateTime ahoraUtc = DateTime.UtcNow;

                        cmd.Parameters.AddWithValue("@nombre", mascota.Nombre);
                        cmd.Parameters.AddWithValue("@especie", mascota.Especie);
                        cmd.Parameters.AddWithValue("@raza", mascota.Raza);
                        cmd.Parameters.AddWithValue("@edad", mascota.Edad);
                        cmd.Parameters.AddWithValue("@peso", mascota.Peso);
                        cmd.Parameters.AddWithValue("@sexo", mascota.Sexo);
                        cmd.Parameters.AddWithValue("@id_usuario", mascota.IdUsuario);
                        cmd.Parameters.AddWithValue("@fecha_registro", ahoraUtc);
                        cmd.Parameters.AddWithValue("@fecha_edicion", ahoraUtc);

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
            const string query = @"UPDATE mascotas SET activo = false, fecha_edicion = @fecha_edicion WHERE id = @id";
            try
            {
                using (var conn = ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", mascota.Id);
                        cmd.Parameters.AddWithValue("@fecha_edicion", DateTime.UtcNow);
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

            const string query = "UPDATE mascotas SET nombre = @nombre, especie = @especie, raza = @raza, edad = @edad, peso = @peso, sexo = @sexo, fecha_edicion = @fecha_edicion WHERE id = @id";

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
                        cmd.Parameters.AddWithValue("@fecha_edicion", DateTime.UtcNow);
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
        public List<Mascota> ObtenerMascotasFiltro(FiltroMascotas filtro)
            {
            List<Mascota> lista = new List<Mascota>();
            string query;

            DateTime? fecha = filtro?.Fecha;

            if (fecha.HasValue)
                {
                query = "SELECT id, nombre, especie, raza, edad, peso, sexo, id_usuario, fecha_registro, fecha_edicion, activo FROM mascotas WHERE fecha_edicion > @desde AND activo = true";
                }
            else
                {
                query = "SELECT id, nombre, especie, raza, edad, peso, sexo, id_usuario, fecha_registro, fecha_edicion, activo FROM mascotas WHERE activo = true";
                }

            try
                {
                using (var conn = ObtenerConexion())
                    {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                        {
                        if (fecha.HasValue)
                            {
                            cmd.Parameters.AddWithValue("@desde", fecha.Value);
                            }

                        using (var reader = cmd.ExecuteReader())
                            {
                            while (reader.Read())
                                {
                                lista.Add(new Mascota
                                    {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Nombre = reader["nombre"].ToString(),
                                    Especie = reader["especie"].ToString(),
                                    Raza = reader["raza"].ToString(),
                                    Edad = Convert.ToInt32(reader["edad"]),
                                    Peso = Convert.ToDecimal(reader["peso"]),
                                    Sexo = Convert.ToChar(reader["sexo"]),
                                    IdUsuario = Convert.ToInt32(reader["id_usuario"]),
                                    FechaRegistro = Convert.ToDateTime(reader["fecha_registro"]),
                                    FechaEdicion = reader["fecha_edicion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["fecha_edicion"]),
                                    Activo = Convert.ToBoolean(reader["activo"])
                                    });
                                }
                            }
                        }
                    }
                }
            catch (Exception ex)
                {
                Console.WriteLine($"Error en ObtenerMascotasFiltro: {ex.Message}");
                }

            return lista;
            }
        public List<Mascota> ObtenerMascotasFiltroFecha(StringFechaFiltro filtro)
        {
            List<Mascota> lista = new List<Mascota>();
            string query;
            DateTime? fecha = null;

            if (!string.IsNullOrWhiteSpace(filtro?.Fecha))
            {
                try
                {
                    fecha = DateTime.Parse(filtro.Fecha, null, System.Globalization.DateTimeStyles.RoundtripKind);
                    Console.WriteLine($"✔ Fecha recibida y parseada correctamente: {fecha.Value.ToString("o")}");
                }
                catch (FormatException)
                {
                    Console.WriteLine("❌ Formato de fecha inválido: " + filtro.Fecha);
                    throw new WebFaultException<string>(
                        "Formato de fecha inválido. Usa formato ISO 8601, por ejemplo: '2025-04-14T22:38:25Z'",
                        System.Net.HttpStatusCode.BadRequest
                    );
                }
            }

            if (fecha.HasValue)
            {
                query = "SELECT id, nombre, especie, raza, edad, peso, sexo, id_usuario, fecha_registro, fecha_edicion, activo FROM mascotas WHERE fecha_edicion > @desde AND activo = true";
            }
            else
            {
                query = "SELECT id, nombre, especie, raza, edad, peso, sexo, id_usuario, fecha_registro, fecha_edicion, activo FROM mascotas WHERE activo = true";
            }

            try
            {
                using (var conn = ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        if (fecha.HasValue)
                        {
                            cmd.Parameters.AddWithValue("@desde", fecha.Value);
                        }

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                lista.Add(new Mascota
                                {
                                    Id = Convert.ToInt32(reader["id"]),
                                    Nombre = reader["nombre"].ToString(),
                                    Especie = reader["especie"].ToString(),
                                    Raza = reader["raza"].ToString(),
                                    Edad = Convert.ToInt32(reader["edad"]),
                                    Peso = Convert.ToDecimal(reader["peso"]),
                                    Sexo = Convert.ToChar(reader["sexo"]),
                                    IdUsuario = Convert.ToInt32(reader["id_usuario"]),
                                    FechaRegistro = Convert.ToDateTime(reader["fecha_registro"]),
                                    FechaEdicion = reader["fecha_edicion"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["fecha_edicion"]),
                                    Activo = Convert.ToBoolean(reader["activo"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerMascotasFiltroFecha: {ex.Message}");
            }

            return lista;
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
