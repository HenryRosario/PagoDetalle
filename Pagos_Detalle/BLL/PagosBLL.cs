using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

public class PagosBLL
{
    private Contexto _contexto;

    public PagosBLL(Contexto contexto)
    {
        _contexto = contexto;
    }
    public async Task<bool> Existe(int pagosId)
    {
        return await _contexto.Pagos.AnyAsync(o => o.PagoId == pagosId);
    }

    public async Task<bool> Guardar(Pagos pagos)
    {
        var existe = await Existe(pagos.PagoId);

        if (!existe)
            return await this.Insertar(pagos);
        else
            return await this.Modificar(pagos);
    }

    private async Task<bool> Insertar(Pagos pago)
    {
        await _contexto.Pagos.AddAsync(pago);

        foreach (var item in pago.Detalle)
        {
            var prestamo = await _contexto.Prestamos.FindAsync(item.PrestamoId);
            prestamo!.Balance -= item.ValorPagado;
        }

        var persona = await _contexto.Personas.FindAsync(pago.PersonaId);
        persona!.Balance -= pago.Monto;

        var insertados = await _contexto.SaveChangesAsync();

        return insertados > 0;
    }
    private async Task<bool> Modificar(Pagos pagoActual)
    {
        var pagoAnterior = await _contexto.Pagos
             .Where(p => p.PagoId == pagoActual.PagoId)
             .AsTracking()
             .SingleOrDefaultAsync();


        var personaAnterior = await _contexto.Personas.FindAsync(pagoAnterior!.PersonaId);
        personaAnterior!.Balance += pagoAnterior.Monto;

        foreach (var item in pagoAnterior.Detalle)
        {
            var prestamos = await _contexto.Prestamos.FindAsync(item.PrestamoId);
            prestamos!.Balance += item.ValorPagado;
        }

        await _contexto.Database.ExecuteSqlRawAsync($"Delete FROM PagosDetalle Where PagoId = {pagoActual.PagoId}");

        foreach (var item in pagoActual.Detalle)
        {
            _contexto.Entry(item).State = EntityState.Added;

            var prestamo = await _contexto.Prestamos.FindAsync(item.PrestamoId);
            prestamo!.Balance -= item.ValorPagado;
        }

        var persona = await _contexto.Personas.FindAsync(pagoActual!.PersonaId);
        persona!.Balance -= pagoActual.Monto;

        _contexto.Entry(pagoActual).State = EntityState.Modified;

        var cantidad = await _contexto.SaveChangesAsync();

        _contexto.Entry(pagoActual).State = EntityState.Detached;

        return cantidad > 0;
    }

    public async Task<bool> Eliminar(Pagos pago)
    {

        var persona = await _contexto.Personas.FindAsync(pago.PersonaId);
        persona!.Balance += pago.Monto;

        foreach (var item in pago.Detalle)
        {
            var prestamos = await _contexto.Prestamos.FindAsync(item.PrestamoId);
            prestamos!.Balance += item.ValorPagado;
        }

        _contexto.Entry(pago).State = EntityState.Deleted;

        var cantidad = await _contexto.SaveChangesAsync();

        return cantidad > 0;
    }

    public async Task<Pagos?> Buscar(int pagoId)
    {
        var pago = await _contexto.Pagos
        .Where(o => o.PagoId == pagoId)
        .Include(o => o.Detalle)
        .AsTracking()
        .SingleOrDefaultAsync();

        return pago;
    }

    public async Task<List<Pagos>> GetList(Expression<Func<Pagos, bool>> Criterio)
    {
        return await _contexto.Pagos
            .Where(Criterio)
            .AsTracking()
            .ToListAsync();
    }

}