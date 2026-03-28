namespace WinFormsApp2.Models
{
    public enum TipoMensaje
    {
        Normal,
        Marker
    }

    public class Mensaje
    {
        public string Id { get; set; } = string.Empty;
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public TipoMensaje Tipo { get; set; }
        public string Contenido { get; set; } = string.Empty;

        public string Canal => $"{Origen}->{Destino}";

        public override string ToString()
        {
            if (Tipo == TipoMensaje.Marker)
            {
                return $"[MARKER] {Origen} -> {Destino}";
            }

            return $"[{Id}] {Origen} -> {Destino}: {Contenido}";
        }
    }
}