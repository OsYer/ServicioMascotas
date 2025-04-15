using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using Npgsql;
using ServicioMascotas.Models;

namespace ServicioMascotas
{
    [ServiceBehavior]
    public class Usuarios : IUsuarios
    {
        private string _connectionString = "Host=192.168.15.225;Port=5432;Username=postgres;Password=admin;Database=servicio_mascotas;";

        private NpgsqlConnection ObtenerConexion()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public void HandleOptionsRequest()
        {
            WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
        }

        public List<Usuario> ObtenerUsuarios()
        {
            var lista = new List<Usuario>();
            const string query = "SELECT * FROM usuarios";

            try
            {
                using (var conn = ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        lista = (from IDataRecord r in reader
                                 select new Usuario
                                 {
                                     Id = r.GetInt32(0),
                                     Nombre = r.GetString(1),
                                     Correo = r.GetString(2),
                                     Telefono = r.IsDBNull(3) ? null : r.GetString(3),
                                     Direccion = r.IsDBNull(4) ? null : r.GetString(4),
                                     FechaRegistro = r.GetDateTime(5)
                                 }).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ObtenerUsuarios: {ex.Message}");
            }

            return lista;
        }

        public string AgregarUsuario(Usuario usuario)
            {
            const string query = "INSERT INTO usuarios (nombre, correo, telefono, direccion, fecha_registro) VALUES (@nombre, @correo, @telefono, @direccion, @fecha_registro)";
            try
                {
                using (var conn = ObtenerConexion())
                    {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                        {
                        cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
                        cmd.Parameters.AddWithValue("@correo", usuario.Correo);
                        cmd.Parameters.AddWithValue("@telefono", (object)usuario.Telefono ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@direccion", (object)usuario.Direccion ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@fecha_registro", DateTime.UtcNow);

                        int filas = cmd.ExecuteNonQuery();
                        if (filas > 0)
                            {
                            return "✅ Usuario insertado correctamente.";
                            }
                        else
                            {
                            return "⚠️ No se insertó ningún registro.";
                            }
                        }
                    }
                }
            catch (Exception ex)
                {
                return $"❌ Error en AgregarUsuario: {ex.Message}";
                }
            }

        public bool ActualizarUsuario(Usuario usuario)
        {
            const string query = "UPDATE usuarios SET nombre = @nombre, correo = @correo, telefono = @telefono, direccion = @direccion WHERE id = @id";
            try
            {
                using (var conn = ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", usuario.Id);
                        cmd.Parameters.AddWithValue("@nombre", usuario.Nombre);
                        cmd.Parameters.AddWithValue("@correo", usuario.Correo);
                        cmd.Parameters.AddWithValue("@telefono", (object)usuario.Telefono ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@direccion", (object)usuario.Direccion ?? DBNull.Value);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en ActualizarUsuario: {ex.Message}");
                return false;
            }
        }

        public bool EliminarUsuario(Usuario usuario)
        {
            const string query = "DELETE FROM usuarios WHERE id = @id";
            try
            {
                using (var conn = ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", usuario.Id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en EliminarUsuario: {ex.Message}");
                return false;
            }
        }
    }
}
