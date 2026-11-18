using Florarium.Api.Dtos.Me;
using Florarium.Api.Exceptions;
using Florarium.Data;
using Florarium.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Florarium.Api.Services;

/// <summary>Сервис AddressesService.</summary>
public class AddressesService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    /// <summary>Выполняет GetUserAddressesAsync.</summary>
    /// <exception cref="Florarium.Api.Exceptions.ApiException">Возникает при нарушении бизнес-правил операции.</exception>
    public async Task<List<AddressDto>> GetUserAddressesAsync(Guid userId, CancellationToken ct)
    {
        return await _db.Addresses
            .AsNoTracking()
            .Where(a => a.CustomerId == userId)
            .Select(a => new AddressDto
            {
                Id = a.Id,
                Label = a.Label,
                Street = a.Street,
                Apartment = a.Apartment,
                Entrance = a.Entrance,
                Floor = a.Floor,
                Intercom = a.Intercom
            })
            .ToListAsync(ct);
    }

    /// <summary>Выполняет CreateAddressAsync.</summary>
    /// <exception cref="Florarium.Api.Exceptions.ApiException">Возникает при нарушении бизнес-правил операции.</exception>
    public async Task<AddressDto> CreateAddressAsync(Guid userId, CreateAddressRequest request, CancellationToken ct)
    {
        var address = new Address
        {
            CustomerId = userId,
            Label = request.Label,
            Street = request.Street,
            Apartment = request.Apartment,
            Entrance = request.Entrance,
            Floor = request.Floor,
            Intercom = request.Intercom
        };

        _db.Addresses.Add(address);
        await _db.SaveChangesAsync(ct);

        return new AddressDto
        {
            Id = address.Id,
            Label = address.Label,
            Street = address.Street,
            Apartment = address.Apartment,
            Entrance = address.Entrance,
            Floor = address.Floor,
            Intercom = address.Intercom
        };
    }
}
