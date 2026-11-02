namespace Eclipse.Core.Pooling
{
    public interface IPoolable
    {
        void OnSpawned();

        void OnDespawned();
    }
}