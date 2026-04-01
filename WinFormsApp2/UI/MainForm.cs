using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using WinFormsApp2.Models;
using WinFormsApp2.Services;

namespace WinFormsApp2.UI
{
    public partial class MainForm : Form
    {
        private readonly SnapshotSimulator _simulator;

        private readonly Dictionary<string, PointF> _nodePositions = new();
        private AnimatedMessage? _currentAnimation;
        private readonly HashSet<string> _highlightedTransitChannels = new();

        private Button _btnVelocidad = new Button();
        private Label _lblExplicacionPaso = new Label();
        private float _animationSpeed = 0.04f;
        private int _pasoEscenario = 0;

        private readonly string[] _explicacionesPasos = new string[]
        {
            "Bienvenido al Tutorial Guiado. Haz clic en el botón para iniciar el snapshot desde P1.",
            "PASO 1 completado: P1 guardó su estado (5) y envió Markers azules por sus canales de salida.\n" +
            "➤ Regla 1: Al iniciar, un proceso guarda su estado y envía Markers. Haz clic para entregar el Marker a P2.",
            "PASO 2 completado: El Marker llegó a P2. Como P2 no tenía foto, aplicó Regla 2: guardó su estado (2).\n" +
            "➤ El canal P1→P2 ahora tiene 🔒. Haz clic para que P3 envíe un mensaje normal a P2.",
            "PASO 3 completado: P3 envió datos normales (M1) a P2. P3 NO sabe del snapshot, sigue trabajando.\n" +
            "➤ Este mensaje viajará por el canal P3→P2. Haz clic para entregar el Marker a P3.",
            "PASO 4 completado: El Marker llegó a P3. P3 aplicó Regla 2: guardó su estado (9) y reenvió Marker a P2.\n" +
            "➤ P3 ahora vigila (🔴) sus canales. Haz clic para ejecutar un evento interno en P3.",
            "PASO 5 completado: P3 hizo un cálculo interno (estado subió a 10). ¡Pero su foto dice 9!\n" +
            "➤ ACTUAL ≠ CAPTURADO demuestra que el sistema NO se detuvo. Haz clic para entregar M1 a P2.",
            "PASO 6 completado: ¡CASO CLAVE! M1 llegó a P2, pero P2 ya guardó su foto y el canal P3→P2 NO tiene Marker aún.\n" +
            "➤ Regla 3: M1 se registra como MENSAJE EN TRÁNSITO (línea roja). Haz clic para el último Marker.",
            "PASO 7 FINAL: El Marker de P3 llegó a P2. Canal P3→P2 cerrado con 🔒.\n" +
            "➤ P2 recibió Markers por TODOS sus canales. ¡SNAPSHOT COMPLETO! El estado global es consistente."
        };

        public MainForm()
        {
            InitializeComponent();

            _simulator = new SnapshotSimulator();

            ConfigurarNodos();

            _btnVelocidad.Text = "Vel. Normal ►";
            _btnVelocidad.Location = new Point(1075, 430);
            _btnVelocidad.Size = new Size(85, 35);
            _btnVelocidad.Click += btnVelocidad_Click;
            this.Controls.Add(_btnVelocidad);

            _lblExplicacionPaso.Location = new Point(20, 670);
            _lblExplicacionPaso.Size = new Size(1140, 55);
            _lblExplicacionPaso.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            _lblExplicacionPaso.ForeColor = Color.MidnightBlue;
            _lblExplicacionPaso.BackColor = Color.FromArgb(232, 245, 255);
            _lblExplicacionPaso.Padding = new Padding(8, 4, 8, 4);
            _lblExplicacionPaso.BorderStyle = BorderStyle.FixedSingle;
            _lblExplicacionPaso.Text = _explicacionesPasos[0];
            this.Controls.Add(_lblExplicacionPaso);

            btnEscenario.Text = "▶ Tutorial: Iniciar Snapshot";

            animationTimer.Interval = 30;
            animationTimer.Tick += animationTimer_Tick;

            panelCanvas.Paint += panelCanvas_Paint;
            panelCanvas.Resize += panelCanvas_Resize;

            ActualizarUI();
        }

        private void ConfigurarNodos()
        {
            _nodePositions["P1"] = new PointF(140, 110);
            _nodePositions["P2"] = new PointF(620, 110);
            _nodePositions["P3"] = new PointF(380, 290);
        }

        private void btnIniciarSnapshot_Click(object sender, EventArgs e)
        {
            _simulator.IniciarSnapshotEnP1();
            ActualizarCanalesResaltados();
            ActualizarUI();
        }

        private void btnEnviarP3P2_Click(object sender, EventArgs e)
        {
            _simulator.EnviarMensajeNormalP3aP2();
            ActualizarUI();
        }

        private void btnEventoInternoP3_Click(object sender, EventArgs e)
        {
            _simulator.EjecutarEventoInternoEnP3();
            ActualizarUI();
        }

        private void btnEntregar_Click(object sender, EventArgs e)
        {
            IniciarAnimacionSiguienteMensaje();
        }

        private void btnEscenario_Click(object sender, EventArgs e)
        {
            if (_currentAnimation != null) return;

            switch (_pasoEscenario)
            {
                case 0:
                    _simulator.IniciarSnapshotEnP1();
                    btnEscenario.Text = "▶ Paso 2: Entregar Marker a P2";
                    break;
                case 1:
                    IniciarAnimacionSiguienteMensaje();
                    btnEscenario.Text = "▶ Paso 3: P3 envía datos a P2";
                    break;
                case 2:
                    _simulator.EnviarMensajeNormalP3aP2();
                    btnEscenario.Text = "▶ Paso 4: Entregar Marker a P3";
                    break;
                case 3:
                    IniciarAnimacionSiguienteMensaje();
                    btnEscenario.Text = "▶ Paso 5: Evento interno en P3";
                    break;
                case 4:
                    _simulator.EjecutarEventoInternoEnP3();
                    btnEscenario.Text = "▶ Paso 6: Entregar M1 a P2";
                    break;
                case 5:
                    IniciarAnimacionSiguienteMensaje();
                    btnEscenario.Text = "▶ Paso 7 (Final): Marker a P2";
                    break;
                case 6:
                    IniciarAnimacionSiguienteMensaje();
                    btnEscenario.Text = "✔ Tutorial Completado";
                    break;
                default:
                    return;
            }

            if (_pasoEscenario < 7) _pasoEscenario++;

            // Actualizar la explicación del paso
            if (_pasoEscenario < _explicacionesPasos.Length)
            {
                _lblExplicacionPaso.Text = _explicacionesPasos[_pasoEscenario];
            }

            ActualizarCanalesResaltados();
            ActualizarUI();
        }

        private void btnVelocidad_Click(object? sender, EventArgs e)
        {
            if (_animationSpeed == 0.04f)
            {
                _animationSpeed = 0.015f;
                _btnVelocidad.Text = "Vel. Lenta ◄";
            }
            else if (_animationSpeed == 0.015f)
            {
                _animationSpeed = 0.12f;
                _btnVelocidad.Text = "Vel. Rápida ◄►";
            }
            else
            {
                _animationSpeed = 0.04f;
                _btnVelocidad.Text = "Vel. Normal ►";
            }
        }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            animationTimer.Stop();
            _currentAnimation = null;
            _highlightedTransitChannels.Clear();
            _pasoEscenario = 0;
            btnEscenario.Text = "▶ Tutorial: Iniciar Snapshot";
            _lblExplicacionPaso.Text = _explicacionesPasos[0];

            _simulator.Reiniciar();
            ActualizarUI();
        }

        private void IniciarAnimacionSiguienteMensaje()
        {
            if (_currentAnimation != null)
            {
                return;
            }

            Mensaje? siguiente = _simulator.ObtenerSiguienteMensajePendiente();

            if (siguiente == null)
            {
                _simulator.LogEventos.Add("No hay mensajes pendientes en la cola.");
                ActualizarUI();
                return;
            }

            PointF start = _nodePositions[siguiente.Origen];
            PointF end = _nodePositions[siguiente.Destino];

            _currentAnimation = new AnimatedMessage
            {
                Mensaje = siguiente,
                Start = start,
                End = end,
                Progress = 0f
            };

            animationTimer.Start();
        }

        private void animationTimer_Tick(object? sender, EventArgs e)
        {
            if (_currentAnimation == null)
            {
                animationTimer.Stop();
                return;
            }

            _currentAnimation.Progress += _animationSpeed;

            if (_currentAnimation.Progress >= 1f)
            {
                _currentAnimation.Progress = 1f;
                animationTimer.Stop();

                _simulator.EntregarSiguienteMensaje();
                _currentAnimation = null;

                ActualizarCanalesResaltados();
                ActualizarUI();
                return;
            }

            panelCanvas.Invalidate();
        }

        private void ActualizarCanalesResaltados()
        {
            _highlightedTransitChannels.Clear();

            foreach (var proceso in _simulator.Procesos.Values)
            {
                foreach (var entrada in proceso.MensajesEnTransitoPorCanal)
                {
                    if (entrada.Value.Count > 0)
                    {
                        _highlightedTransitChannels.Add(entrada.Key);
                    }
                }
            }
        }

        private void ActualizarUI()
        {
            var p1 = _simulator.Procesos["P1"];
            var p2 = _simulator.Procesos["P2"];
            var p3 = _simulator.Procesos["P3"];

            lblP1EstadoActual.Text = $"Estado actual: {FormatearEstadoConDelta(p1)}";
            lblP1Snapshot.Text = $"Capturado: {(p1.EstadoGuardado?.ToString() ?? "No guardado")}";
            lblP1Transito.Text = $"En tránsito: {p1.ObtenerMensajesEnTransitoTexto()}";

            lblP2EstadoActual.Text = $"Estado actual: {FormatearEstadoConDelta(p2)}";
            lblP2Snapshot.Text = $"Capturado: {(p2.EstadoGuardado?.ToString() ?? "No guardado")}";
            lblP2Transito.Text = $"En tránsito: {p2.ObtenerMensajesEnTransitoTexto()}";

            lblP3EstadoActual.Text = $"Estado actual: {FormatearEstadoConDelta(p3)}";
            lblP3Snapshot.Text = $"Capturado: {(p3.EstadoGuardado?.ToString() ?? "No guardado")}";
            lblP3Transito.Text = $"En tránsito: {p3.ObtenerMensajesEnTransitoTexto()}";

            lstCola.Items.Clear();
            foreach (string item in _simulator.ObtenerColaMensajesTexto())
            {
                lstCola.Items.Add(item);
            }

            lstEventos.Items.Clear();
            foreach (string log in _simulator.LogEventos)
            {
                lstEventos.Items.Add(log);
            }

            txtResumen.Text = _simulator.ObtenerResumenSnapshotGlobal();

            panelCanvas.Invalidate();
        }

        private string FormatearEstadoConDelta(Proceso proceso)
        {
            if (!proceso.EstadoGuardado.HasValue)
            {
                return proceso.EstadoLocalActual.ToString();
            }

            int delta = proceso.EstadoLocalActual - proceso.EstadoGuardado.Value;

            if (delta == 0)
            {
                return $"{proceso.EstadoLocalActual} (Δ=0)";
            }

            string deltaTexto = delta > 0 ? $"+{delta}" : delta.ToString();
            return $"{proceso.EstadoLocalActual} (Δ={deltaTexto})";
        }

        private void panelCanvas_Resize(object? sender, EventArgs e)
        {
            panelCanvas.Invalidate();
        }

        private void panelCanvas_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            DibujarCanal(g, "P1", "P2");
            DibujarCanal(g, "P1", "P3");
            DibujarCanal(g, "P3", "P2");

            DibujarNodo(g, "P1");
            DibujarNodo(g, "P2");
            DibujarNodo(g, "P3");

            if (_currentAnimation != null)
            {
                DibujarMensajeAnimado(g, _currentAnimation);
            }

            DibujarLeyenda(g);
            DibujarPanelPredictivo(g);
        }

        private void DibujarPanelPredictivo(Graphics g)
        {
            if (_currentAnimation != null)
            {
                return;
            }

            string prediccion = _simulator.ObtenerPrediccionSiguienteAccion();
            
            RectangleF rect = new RectangleF(10, panelCanvas.Height - 70, panelCanvas.Width - 20, 60);
            using Brush bgBrush = new SolidBrush(Color.FromArgb(240, 248, 255));
            using Pen border = new Pen(Color.SteelBlue, 2);
            g.FillRectangle(bgBrush, rect);
            g.DrawRectangle(border, Rectangle.Round(rect));

            using Font font = new Font("Segoe UI", 11, FontStyle.Bold);
            using Brush textBrush = new SolidBrush(Color.DarkSlateGray);
            
            g.DrawString(prediccion, font, textBrush, rect, new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
        }

        private void DibujarLeyenda(Graphics g)
        {
            float x = panelCanvas.Width - 195;
            float y = 8;
            float ancho = 185;
            float alto = 110;

            using Brush bgBrush = new SolidBrush(Color.FromArgb(220, 255, 255, 255));
            using Pen borderPen = new Pen(Color.DimGray, 1);
            g.FillRectangle(bgBrush, x, y, ancho, alto);
            g.DrawRectangle(borderPen, x, y, ancho, alto);

            using Font titleFont = new Font("Segoe UI", 9, FontStyle.Bold);
            using Font itemFont = new Font("Segoe UI", 8, FontStyle.Regular);
            Brush textBrush = Brushes.Black;

            g.DrawString("LEYENDA", titleFont, textBrush, x + 55, y + 4);

            // Nodo gris = sin snapshot
            using (Brush grayBrush = new SolidBrush(Color.FromArgb(180, 230, 230, 230)))
                g.FillEllipse(grayBrush, x + 8, y + 24, 12, 12);
            g.DrawString("Sin snapshot", itemFont, textBrush, x + 26, y + 23);

            // Nodo verde = snapshot OK
            using (Brush greenBrush = new SolidBrush(Color.FromArgb(180, 230, 255, 180)))
                g.FillEllipse(greenBrush, x + 8, y + 42, 12, 12);
            g.DrawString("Foto guardada (OK)", itemFont, textBrush, x + 26, y + 41);

            // Nodo amarillo = divergente
            using (Brush yellowBrush = new SolidBrush(Color.FromArgb(200, 255, 236, 179)))
                g.FillEllipse(yellowBrush, x + 8, y + 60, 12, 12);
            g.DrawString("Actual != Capturado", itemFont, textBrush, x + 26, y + 59);

            // Marker vs Normal
            g.FillEllipse(Brushes.RoyalBlue, x + 8, y + 78, 12, 12);
            g.DrawString("Marker (senal)", itemFont, textBrush, x + 26, y + 77);

            g.FillEllipse(Brushes.DarkOrange, x + 100, y + 78, 12, 12);
            g.DrawString("Normal", itemFont, textBrush, x + 118, y + 77);

            // Iconos
            g.FillEllipse(Brushes.Red, x + 8, y + 96, 8, 8);
            g.DrawString("= Vigilando", itemFont, textBrush, x + 18, y + 94);
            g.FillRectangle(Brushes.DimGray, x + 95, y + 96, 8, 8);
            g.DrawString("= Cerrado", itemFont, textBrush, x + 105, y + 94);
        }

        private void DibujarCanal(Graphics g, string origen, string destino)
        {
            string canal = $"{origen}->{destino}";
            PointF start = _nodePositions[origen];
            PointF end = _nodePositions[destino];

            bool enTransito = _highlightedTransitChannels.Contains(canal);

            Color color = enTransito ? Color.Crimson : Color.Gray;
            float width = enTransito ? 4f : 2f;

            using Pen pen = new Pen(color, width);
            pen.CustomEndCap = new AdjustableArrowCap(5, 5);

            PointF startEdge = CalcularPuntoEnBorde(start, end, 48);
            PointF endEdge = CalcularPuntoEnBorde(end, start, 48);

            g.DrawLine(pen, startEdge, endEdge);

            float midX = (startEdge.X + endEdge.X) / 2f;
            float midY = (startEdge.Y + endEdge.Y) / 2f;

            using Font font = new Font("Segoe UI", 9, FontStyle.Bold);
            using Brush brush = new SolidBrush(color);

            g.DrawString(canal, font, brush, midX - 34, midY - 22);

            if (enTransito)
            {
                g.DrawString("En tránsito", font, brush, midX - 38, midY - 4);
            }

            // --- Didactics: Draw channel status icons at destination ---
            var procesoDestino = _simulator.Procesos[destino];
            if (procesoDestino.SnapshotGuardado)
            {
                bool markerRecibido = procesoDestino.MarkerRecibidoPorCanal.ContainsKey(canal) && procesoDestino.MarkerRecibidoPorCanal[canal];
                
                float iconX = endEdge.X - 15;
                float iconY = endEdge.Y - 25;
                
                if (markerRecibido)
                {
                    // Candado cerrado: rectángulo con texto
                    using Font lockFont = new Font("Segoe UI", 8, FontStyle.Bold);
                    g.FillRectangle(Brushes.DimGray, iconX - 2, iconY + 2, 36, 16);
                    g.DrawString("CERRADO", lockFont, Brushes.White, iconX, iconY + 2);
                }
                else
                {
                    // Círculo rojo pulsante + texto Grabando
                    using Font recFont = new Font("Segoe UI", 8, FontStyle.Bold);
                    g.FillEllipse(Brushes.Red, iconX - 50, iconY + 4, 10, 10);
                    g.DrawString("GRABANDO", recFont, Brushes.Crimson, iconX - 38, iconY + 2);
                }
            }
        }

        private void DibujarNodo(Graphics g, string nombre)
        {
            var proceso = _simulator.Procesos[nombre];
            PointF center = _nodePositions[nombre];

            RectangleF rect = new RectangleF(center.X - 52, center.Y - 52, 104, 104);

            bool capturado = proceso.SnapshotGuardado;
            bool divergente = proceso.EstadoGuardado.HasValue && proceso.EstadoLocalActual != proceso.EstadoGuardado.Value;

            Color fill;
            Color border;

            if (!capturado)
            {
                fill = Color.FromArgb(180, 230, 230, 230);
                border = Color.DimGray;
            }
            else if (divergente)
            {
                fill = Color.FromArgb(200, 255, 236, 179);
                border = Color.DarkOrange;
            }
            else
            {
                fill = Color.FromArgb(180, 230, 255, 180);
                border = Color.ForestGreen;
            }

            using Brush fillBrush = new SolidBrush(fill);
            using Pen borderPen = new Pen(border, 3);

            g.FillEllipse(fillBrush, rect);
            g.DrawEllipse(borderPen, rect);

            using Font titleFont = new Font("Segoe UI", 13, FontStyle.Bold); // Size 11 -> 13
            using Font textFont = new Font("Segoe UI", 9, FontStyle.Bold);   // Bold y Size 8 -> 9
            using Font badgeFont = new Font("Segoe UI", 8, FontStyle.Bold);  // Size 7 -> 8
            using Brush textBrush = new SolidBrush(Color.Black);
            using Brush badgeBrush = new SolidBrush(border);

            string snapshotTexto = proceso.EstadoGuardado?.ToString() ?? "-";

            g.DrawString(nombre, titleFont, textBrush, center.X - 16, center.Y - 35);
            g.DrawString($"Actual: {proceso.EstadoLocalActual}", textFont, textBrush, center.X - 35, center.Y - 8);
            g.DrawString($"Capt. : {snapshotTexto}", textFont, textBrush, center.X - 35, center.Y + 12);

            if (divergente)
            {
                g.DrawString("ACTUAL != CAPT.", badgeFont, badgeBrush, center.X - 45, center.Y + 33);
            }
        }

        private void DibujarMensajeAnimado(Graphics g, AnimatedMessage animated)
        {
            PointF pos = animated.GetCurrentPosition();

            Color color = animated.Mensaje.Tipo == TipoMensaje.Marker ? Color.RoyalBlue : Color.DarkOrange;
            string texto = animated.Mensaje.Tipo == TipoMensaje.Marker ? "MK" : animated.Mensaje.Id;

            RectangleF rect = new RectangleF(pos.X - 18, pos.Y - 12, 36, 24);

            using Brush fill = new SolidBrush(color);
            using Pen border = new Pen(Color.Black, 1.5f);
            using Brush textBrush = new SolidBrush(Color.White);
            using Font font = new Font("Segoe UI", 8, FontStyle.Bold);
            using StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            g.FillEllipse(fill, rect);
            g.DrawEllipse(border, rect);
            g.DrawString(texto, font, textBrush, rect, sf);
        }

        private PointF CalcularPuntoEnBorde(PointF desde, PointF hacia, float radio)
        {
            float dx = hacia.X - desde.X;
            float dy = hacia.Y - desde.Y;
            float distancia = (float)Math.Sqrt(dx * dx + dy * dy);

            if (distancia == 0)
            {
                return desde;
            }

            float ux = dx / distancia;
            float uy = dy / distancia;

            return new PointF(
                desde.X + ux * radio,
                desde.Y + uy * radio
            );
        }
    }
}