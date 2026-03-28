using System.Collections.Generic;
using System.Linq;

namespace WinFormsApp2.Models
{
    public class Proceso
    {
        public string Nombre { get; }
        public int EstadoLocalActual { get; set; }

        public bool SnapshotGuardado { get; set; }
        public int? EstadoGuardado { get; set; }

        public List<string> CanalesEntrada { get; }
        public List<string> CanalesSalida { get; }

        public Dictionary<string, bool> MarkerRecibidoPorCanal { get; }
        public Dictionary<string, List<string>> MensajesEnTransitoPorCanal { get; }

        public Proceso(
            string nombre,
            int estadoInicial,
            List<string> canalesEntrada,
            List<string> canalesSalida)
        {
            Nombre = nombre;
            EstadoLocalActual = estadoInicial;
            CanalesEntrada = canalesEntrada;
            CanalesSalida = canalesSalida;

            SnapshotGuardado = false;
            EstadoGuardado = null;

            MarkerRecibidoPorCanal = new Dictionary<string, bool>();
            MensajesEnTransitoPorCanal = new Dictionary<string, List<string>>();

            foreach (string canal in CanalesEntrada)
            {
                MarkerRecibidoPorCanal[canal] = false;
                MensajesEnTransitoPorCanal[canal] = new List<string>();
            }
        }

        public void GuardarSnapshot()
        {
            if (!SnapshotGuardado)
            {
                SnapshotGuardado = true;
                EstadoGuardado = EstadoLocalActual;
            }
        }

        public bool SnapshotCompleto()
        {
            return CanalesEntrada.All(canal => MarkerRecibidoPorCanal[canal]);
        }

        public void ReiniciarSnapshot()
        {
            SnapshotGuardado = false;
            EstadoGuardado = null;

            foreach (string canal in CanalesEntrada)
            {
                MarkerRecibidoPorCanal[canal] = false;
                MensajesEnTransitoPorCanal[canal].Clear();
            }
        }

        public string ObtenerMensajesEnTransitoTexto()
        {
            var partes = new List<string>();

            foreach (var entrada in MensajesEnTransitoPorCanal)
            {
                string canal = entrada.Key;
                List<string> mensajes = entrada.Value;

                if (mensajes.Count > 0)
                {
                    partes.Add($"{canal}: {string.Join(", ", mensajes)}");
                }
            }

            return partes.Count == 0 ? "Ninguno" : string.Join(" | ", partes);
        }
    }
}