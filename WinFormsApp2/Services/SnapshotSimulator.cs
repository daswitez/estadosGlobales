using System;
using System.Collections.Generic;
using System.Linq;
using WinFormsApp2.Models;

namespace WinFormsApp2.Services
{
    public class SnapshotSimulator
    {
        public Dictionary<string, Proceso> Procesos { get; private set; }
        public Queue<Mensaje> ColaMensajes { get; private set; }
        public List<string> LogEventos { get; private set; }

        private int _contadorMensajes;
        private bool _snapshotIniciado;

        public SnapshotSimulator()
        {
            Procesos = new Dictionary<string, Proceso>();
            ColaMensajes = new Queue<Mensaje>();
            LogEventos = new List<string>();

            Inicializar();
        }

        public void Inicializar()
        {
            Procesos = new Dictionary<string, Proceso>
            {
                {
                    "P1",
                    new Proceso(
                        "P1",
                        5,
                        new List<string>(),
                        new List<string> { "P1->P2", "P1->P3" }
                    )
                },
                {
                    "P2",
                    new Proceso(
                        "P2",
                        2,
                        new List<string> { "P1->P2", "P3->P2" },
                        new List<string>()
                    )
                },
                {
                    "P3",
                    new Proceso(
                        "P3",
                        8,
                        new List<string> { "P1->P3" },
                        new List<string> { "P3->P2" }
                    )
                }
            };

            ColaMensajes = new Queue<Mensaje>();
            LogEventos = new List<string>();
            _contadorMensajes = 1;
            _snapshotIniciado = false;

            AgregarLog("Simulación reiniciada.");
            AgregarLog("Estados iniciales: P1=5, P2=2, P3=8.");
        }

        public void Reiniciar()
        {
            Inicializar();
        }

        public void IniciarSnapshotEnP1()
        {
            if (_snapshotIniciado)
            {
                AgregarLog("El snapshot ya fue iniciado.");
                return;
            }

            _snapshotIniciado = true;

            Proceso p1 = Procesos["P1"];
            p1.GuardarSnapshot();

            AgregarLog("P1 inició el snapshot.");
            AgregarLog($"P1 guardó su estado local: {p1.EstadoGuardado}.");

            EncolarMarker("P1", "P2");
            EncolarMarker("P1", "P3");

            if (p1.CanalesEntrada.Count == 0)
            {
                AgregarLog("P1 completó su parte del snapshot.");
            }
        }

        public void EnviarMensajeNormalP3aP2()
        {
            Mensaje mensaje = new Mensaje
            {
                Id = $"M{_contadorMensajes++}",
                Origen = "P3",
                Destino = "P2",
                Tipo = TipoMensaje.Normal,
                Contenido = "Dato desde P3"
            };

            ColaMensajes.Enqueue(mensaje);
            AgregarLog($"P3 encoló mensaje normal hacia P2: {mensaje.Id}.");
        }

        public void EntregarSiguienteMensaje()
        {
            if (ColaMensajes.Count == 0)
            {
                AgregarLog("No hay mensajes pendientes en la cola.");
                return;
            }

            Mensaje mensaje = ColaMensajes.Dequeue();
            RecibirMensaje(mensaje);
        }

        public void EjecutarEscenarioCompleto()
        {
            AgregarLog("=== Inicio del escenario automático ===");

            IniciarSnapshotEnP1();

            // 1) Llega marker de P1 a P2
            EntregarSiguienteMensaje();

            // 2) P3 manda un mensaje normal a P2
            EnviarMensajeNormalP3aP2();

            // 3) Llega ese mensaje normal a P2 antes del marker desde P3
            EntregarSiguienteMensaje();

            // 4) Llega marker de P1 a P3
            EntregarSiguienteMensaje();

            // 5) Llega marker de P3 a P2
            EntregarSiguienteMensaje();

            AgregarLog("=== Fin del escenario automático ===");
        }

        private void EncolarMarker(string origen, string destino)
        {
            Mensaje marker = new Mensaje
            {
                Id = $"MK-{origen}-{destino}",
                Origen = origen,
                Destino = destino,
                Tipo = TipoMensaje.Marker,
                Contenido = "MARKER"
            };

            ColaMensajes.Enqueue(marker);
            AgregarLog($"{origen} encoló MARKER hacia {destino}.");
        }

        private void RecibirMensaje(Mensaje mensaje)
        {
            Proceso receptor = Procesos[mensaje.Destino];
            string canal = mensaje.Canal;

            if (mensaje.Tipo == TipoMensaje.Marker)
            {
                ProcesarMarker(receptor, mensaje);
                return;
            }

            AgregarLog($"{mensaje.Destino} recibió mensaje normal {mensaje.Id} desde {mensaje.Origen}.");

            if (receptor.SnapshotGuardado &&
                receptor.MarkerRecibidoPorCanal.ContainsKey(canal) &&
                !receptor.MarkerRecibidoPorCanal[canal])
            {
                receptor.MensajesEnTransitoPorCanal[canal].Add(mensaje.Id);
                AgregarLog($"{mensaje.Id} se registró como mensaje en tránsito en el canal {canal}.");
            }
            else
            {
                receptor.EstadoLocalActual++;
                AgregarLog($"{receptor.Nombre} procesó {mensaje.Id}. Nuevo estado local: {receptor.EstadoLocalActual}.");
            }
        }

        private void ProcesarMarker(Proceso receptor, Mensaje marker)
        {
            string canal = marker.Canal;

            AgregarLog($"{receptor.Nombre} recibió MARKER desde {marker.Origen}.");

            if (!receptor.SnapshotGuardado)
            {
                receptor.GuardarSnapshot();
                AgregarLog($"{receptor.Nombre} guardó su estado local: {receptor.EstadoGuardado}.");

                if (receptor.MarkerRecibidoPorCanal.ContainsKey(canal))
                {
                    receptor.MarkerRecibidoPorCanal[canal] = true;
                    AgregarLog($"{receptor.Nombre} marcó el canal {canal} como cerrado.");
                }

                foreach (string canalSalida in receptor.CanalesSalida)
                {
                    string[] partes = canalSalida.Split("->");
                    string origen = partes[0];
                    string destino = partes[1];
                    EncolarMarker(origen, destino);
                }
            }
            else
            {
                if (receptor.MarkerRecibidoPorCanal.ContainsKey(canal))
                {
                    receptor.MarkerRecibidoPorCanal[canal] = true;
                    AgregarLog($"{receptor.Nombre} cerró el canal {canal} para el snapshot.");
                }
            }

            if (receptor.CanalesEntrada.Count == 0 || receptor.SnapshotCompleto())
            {
                AgregarLog($"{receptor.Nombre} completó su parte del snapshot.");
            }
        }

        private void AgregarLog(string texto)
        {
            LogEventos.Add(texto);
        }

        public List<string> ObtenerColaMensajesTexto()
        {
            return ColaMensajes.Select(m => m.ToString()).ToList();
        }

        public string ObtenerResumenSnapshotGlobal()
        {
            List<string> lineas = new List<string>();

            foreach (Proceso proceso in Procesos.Values.OrderBy(p => p.Nombre))
            {
                string estado = proceso.EstadoGuardado.HasValue
                    ? proceso.EstadoGuardado.Value.ToString()
                    : "No guardado";

                lineas.Add(
                    $"{proceso.Nombre} | Estado guardado: {estado} | En tránsito: {proceso.ObtenerMensajesEnTransitoTexto()}"
                );
            }

            return string.Join(Environment.NewLine, lineas);
        }
    }
}