using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Pagos
{
    [Key]
    public int PagoId { get; set; }
    [Required(ErrorMessage = "La Fecha es requerida")]
    public DateTime Fecha { get; set; }
    [Required(ErrorMessage = "La PersonaId es requerida")]
    public int PersonaId { get; set; }
    [Required(ErrorMessage = "El Concepto es requerida")]
    public string? Concepto { get; set; }
    [Required(ErrorMessage = "El Monto es requerida")]
    public int Monto { get; set; }

    [ForeignKey("PagoId")]
    
    public virtual List<PagosDetalles> Detalle { get; set; } = new List<PagosDetalles>();
    [ForeignKey("PersonaId")]
    public virtual List<Personas>Personas { get; set; } = new List<Personas>();

}