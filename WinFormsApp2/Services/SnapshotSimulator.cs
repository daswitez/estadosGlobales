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
            AgregarLog("Escenario didáctico sugerido:");
            AgregarLog("1) Iniciar snapshot en P1");
            AgregarLog("2) Entregar siguiente (MARKER P1->P2)");
            AgregarLog("3) Enviar mensaje P3->P2");
            AgregarLog("4) Entregar siguiente (MARKER P1->P3)");
            AgregarLog("5) Evento interno en P3");
            AgregarLog("6) Entregar siguiente (M1 llega a P2 como en tránsito)");
            AgregarLog("7) Entregar siguiente (MARKER P3->P2)");
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
            AgregarLog($"P1 guardó su estado local capturado: {p1.EstadoGuardado}.");

            EncolarMarker("P1", "P2");
            EncolarMarker("P1", "P3");

            if (p1.CanalesEntrada.Count == 0)
            {
                AgregarLog("P1 completó su parte del snapshot.");
            }
        }

        public void EnviarMensajeNormalP3aP2()
        {
            Proceso p3 = Procesos["P3"];
            p3.EstadoLocalActual++;

            Mensaje mensaje = new Mensaje
            {
                Id = $"M{_contadorMensajes++}",
                Origen = "P3",
                Destino = "P2",
                Tipo = TipoMensaje.Normal,
                Contenido = "Dato desde P3"
            };

            ColaMensajes.Enqueue(mensaje);

            AgregarLog($"P3 envió {mensaje.Id} hacia P2.");
            AgregarLog($"P3 cambió su estado actual al enviar: {p3.EstadoLocalActual}.");
        }

        public void EjecutarEventoInternoEnP3()
        {
            Proceso p3 = Procesos["P3"];
            p3.EstadoLocalActual++;

            if (p3.SnapshotGuardado)
            {
                AgregarLog($"P3 realizó un evento interno después de capturar.");
                AgregarLog($"P3 ahora tiene Estado actual = {p3.EstadoLocalActual} y Estado capturado = {p3.EstadoGuardado}.");
            }
            else
            {
                AgregarLog($"P3 realizó un evento interno antes de capturar. Nuevo estado actual: {p3.EstadoLocalActual}.");
            }
        }

        public Mensaje? ObtenerSiguienteMensajePendiente()
        {
            if (ColaMensajes.Count == 0)
            {
                return null;
            }

            return ColaMensajes.Peek();
        }

        public string ObtenerPrediccionSiguienteAccion()
        {
            Mensaje? siguiente = ObtenerSiguienteMensajePendiente();
            if (siguiente == null)
            {
                return "¿Qué pasará?: No hay mensajes en tránsito que entregar.";
            }

            Proceso receptor = Procesos[siguiente.Destino];
            string canal = siguiente.Canal;

            if (siguiente.Tipo == TipoMensaje.Marker)
            {
                if (!receptor.SnapshotGuardado)
                {
                    return $"¿Qué pasará?: {receptor.Nombre} recibirá un MARKER desde {siguiente.Origen}.\n" +
                           $"⮑ Regla 1: Como {receptor.Nombre} NO ha guardado su estado, ¡lo guardará ahora! Cerrará el canal {canal} y reenviará marcadores.";
                }
                else
                {
                    return $"¿Qué pasará?: {receptor.Nombre} recibirá un MARKER desde {siguiente.Origen}.\n" +
                           $"⮑ Regla 2: {receptor.Nombre} ya había guardado su estado. Solo cerrará permanentemente el canal {canal} para la captura.";
                }
            }
            else
            {
                if (receptor.SnapshotGuardado &&
                    receptor.MarkerRecibidoPorCanal.ContainsKey(canal) &&
                    !receptor.MarkerRecibidoPorCanal[canal])
                {
                    return $"¿Qué pasará?: {receptor.Nombre} recibirá un mensaje normal desde {siguiente.Origen}.\n" +
                           $"⮑ Regla 3: Como {receptor.Nombre} ya capturó, pero el canal {canal} aún no está cerrado con un Marker, " +
                           $"¡este mensaje queda atrapado EN TRÁNSITO!";
                }
                else
                {
                    return $"¿Qué pasará?: {receptor.Nombre} recibirá un mensaje normal desde {siguiente.Origen}.\n" +
                           $"⮑ Evento normal: Lo procesará y aumentará su reloj/estado local.";
                }
            }
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

        public void EjecutarEscenarioDidactico()
        {
            AgregarLog("=== Inicio del escenario automático didáctico ===");

            IniciarSnapshotEnP1();
            EntregarSiguienteMensaje();   // MARKER P1->P2
            EnviarMensajeNormalP3aP2();   // P3 sube de 8 a 9
            EntregarSiguienteMensaje();   // MARKER P1->P3, P3 captura 9 y encola marker a P2
            EjecutarEventoInternoEnP3();  // P3 sube a 10, snapshot queda 9
            EntregarSiguienteMensaje();   // M1 llega a P2 y queda en tránsito
            EntregarSiguienteMensaje();   // MARKER P3->P2 cierra canal

            AgregarLog("=== Fin del escenario automático didáctico ===");
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
                AgregarLog($"{receptor.Nombre} guardó su estado local capturado: {receptor.EstadoGuardado}.");

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
                string estadoActual = proceso.EstadoLocalActual.ToString();
                string estadoCapturado = proceso.EstadoGuardado.HasValue
                    ? proceso.EstadoGuardado.Value.ToString()
                    : "No guardado";

                string diferencia = "-";
                if (proceso.EstadoGuardado.HasValue)
                {
                    int delta = proceso.EstadoLocalActual - proceso.EstadoGuardado.Value;
                    diferencia = delta == 0 ? "0" : (delta > 0 ? $"+{delta}" : delta.ToString());
                }

                lineas.Add(
                    $"{proceso.Nombre} | Actual: {estadoActual} | Capturado: {estadoCapturado} | Δ: {diferencia} | En tránsito: {proceso.ObtenerMensajesEnTransitoTexto()}"
                );
            }

            return string.Join(Environment.NewLine, lineas);
        }
    }
}