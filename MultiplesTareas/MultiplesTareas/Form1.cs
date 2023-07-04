using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace MultiplesTareas
{
    public partial class Form1 : Form
    {
        private string apiURL;
        private HttpClient httpClient;
        public Form1()
        {
            InitializeComponent();
            apiURL = "https://localhost:7012";
            httpClient = new HttpClient();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Visible = true;

            var tarjetas = ObtenerTarjetasDeCredito(5);
            var stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            
            try
            {
                ProcesarTarjetas(tarjetas);
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show($"Operacion finalizada en {stopwatch.ElapsedMilliseconds / 1000.0} segundos");


            pictureBox1.Visible = false;
        }

        private async Task<string> ObtenerSaludo(string nombre)
        {
            using (var respuesta = await httpClient.GetAsync($"{apiURL}/saludos/{nombre}"))
            {
                respuesta.EnsureSuccessStatusCode();
                var saludo = await respuesta.Content.ReadAsStringAsync();
                return saludo;
            }
        }

        private List<string> ObtenerTarjetasDeCredito(int cantidadDeTarjetas)
        {
            var tarjetas = new List<string>();
            for (int i = 0; i < cantidadDeTarjetas; i++)
            {
                tarjetas.Add(i.ToString().PadLeft(16, '0'));
            }
            return tarjetas;
        }

        private async Task ProcesarTarjetas(List<string> tarjetas)
        {
            var tareas = new List<Task>();
            foreach(var tarjeta in tarjetas)
            {
                var json = JsonConvert.SerializeObject(tarjeta);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var respuestaTask = httpClient.PostAsync($"{apiURL}/tarjetas", content);
                tareas.Add(respuestaTask);    
            }

            await Task.WhenAll(tareas);
        }
    }
}