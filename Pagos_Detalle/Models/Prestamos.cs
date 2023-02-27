using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Prestamos
    {
        [Key]
        
        public int PrestamoId { get; set; }

        [Required(ErrorMessage ="La Fecha es requerida")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage ="La fecha vence es requerida")]
        public DateTime Vence { get; set; }

        [Required(ErrorMessage ="El PersonaId es requerido")]
        
        public int PersonaId { get; set; }

        [Required(ErrorMessage ="El Concepto es requerido")]
        public string? Concepto { get; set; }

        [Required(ErrorMessage ="El monto es requerido")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage ="El balance es requerido")] 
        [ForeignKey("PrestamoId")]
        public decimal Balance { get; set; }
       
        
}