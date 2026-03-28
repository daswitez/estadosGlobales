using System;
using System.Windows.Forms;
using WinFormsApp2.Services;

namespace WinFormsApp2.UI
{
    public partial class MainForm : Form
    {
        private readonly SnapshotSimulator _simulator;

        public MainForm()
        {
            InitializeComponent();
            _simulator = new SnapshotSimulator();
            ActualizarUI();
        }

        private void btnIniciarSnapshot_Click(object sender, EventArgs e)
        {
            _simulator.IniciarSnapshotEnP1();
            ActualizarUI();
        }

        private void btnEnviarP3P2_Click(object sender, EventArgs e)
        {
            _simulator.EnviarMensajeNormalP3aP2();
            ActualizarUI();
        }

        private void btnEntregar_Click(object sender, EventArgs e)
        {
            _simulator.EntregarSiguienteMensaje();
            ActualizarUI();
        }

        private void btnEscenario_Click(object sender, EventArgs e)
        {
            _simulator.EjecutarEscenarioCompleto();
            ActualizarUI();
        }

        private void btnReiniciar_Click(object sender, EventArgs e)
        {
            _simulator.Reiniciar();
            ActualizarUI();
        }

        private void ActualizarUI()
        {
            var p1 = _simulator.Procesos["P1"];
            var p2 = _simulator.Procesos["P2"];
            var p3 = _simulator.Procesos["P3"];

            lblP1EstadoActual.Text = $"Estado actual: {p1.EstadoLocalActual}";
            lblP1Snapshot.Text = $"Snapshot: {(p1.EstadoGuardado?.ToString() ?? "No guardado")}";
            lblP1Transito.Text = $"En tránsito: {p1.ObtenerMensajesEnTransitoTexto()}";

            lblP2EstadoActual.Text = $"Estado actual: {p2.EstadoLocalActual}";
            lblP2Snapshot.Text = $"Snapshot: {(p2.EstadoGuardado?.ToString() ?? "No guardado")}";
            lblP2Transito.Text = $"En tránsito: {p2.ObtenerMensajesEnTransitoTexto()}";

            lblP3EstadoActual.Text = $"Estado actual: {p3.EstadoLocalActual}";
            lblP3Snapshot.Text = $"Snapshot: {(p3.EstadoGuardado?.ToString() ?? "No guardado")}";
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
        }
    }
}