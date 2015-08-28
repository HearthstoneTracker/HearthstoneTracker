namespace HearthCap.Data
{
    public interface IEntityWithId<out T>
    {
        T Id { get; }
    }
}