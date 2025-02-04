using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BlazorApp1.Components.Pages;
using Microsoft.Extensions.Diagnostics.HealthChecks;

public class SupabaseService
{
    private readonly HttpClient _httpClient;
    private readonly string _supabaseKey;


    public SupabaseService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _supabaseKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InBsendsdHFhc2NhdmF0empremNtIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Mzg1NjI5MzYsImV4cCI6MjA1NDEzODkzNn0.shL8x2tihrEFxZqSJxseo7o8QJvHAIcfvRDB42XgMxE";

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseKey);
        _httpClient.DefaultRequestHeaders.Add("apikey", _supabaseKey);
    }

    // Ciudades desde la tabla "vuelos"
    public async Task<List<string>> GetCities()
    {
        var response = await _httpClient.GetFromJsonAsync<List<Viajes>>("/rest/v1/vuelos?select=origen");
        return response?.Select(a => a.origen).ToList() ?? new List<string>();
    }


    public async Task<List<Viajes>> GetFlights()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Viajes>>("/rest/v1/vuelos");
            return response ?? new List<Viajes>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener vuelos: {ex.Message}");
            return new List<Viajes>();
        }
    }

    // Nuevo método: Obtiene un vuelo por su ID
    public async Task<Viajes?> GetFlightById(int id)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<Viajes>($"vuelos?id=eq.{id}");
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener vuelo por ID: {ex.Message}");
            return null;
        }
    }


    public async Task<bool> CreateFlight(Viajes nuevoVuelo)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("vuelos", nuevoVuelo);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear vuelo: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> UpdateFlight(int id, Viajes vueloActualizado)
    {
        try
        {
            var response = await _httpClient.PatchAsJsonAsync($"/rest/v1/vuelos?id=eq.{id}", vueloActualizado);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar vuelo: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteFlight(int id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/rest/v1/aeropuertos?id=eq.{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al eliminar vuelo: {ex.Message}");
            return false;
        }
    }

    public class Viajes
    {
        public int id { get; set; }
        public DateTime fecha_salida { get; set; }
        public DateTime fecha_llegada { get; set; }
        public decimal precio_vuelo { get; set; }
        public string tipo_vuelo { get; set; }
        public TimeSpan tiempo_vuelo { get; set; }
        public string origen { get; set; }
        public string destino { get; set; }
    }
}
