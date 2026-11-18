namespace Florarium.Domain.Entities;

/// <summary>Роль пользователя, определяющая доступные операции.</summary>
public class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public short Rights { get; set; }

    public ICollection<User> Users { get; set; } = [];
}