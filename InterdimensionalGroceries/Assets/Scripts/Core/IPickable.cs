namespace InterdimensionalGroceries.Core
{
    public interface IPickable
    {
        void OnPickedUp();
        void OnDropped();
        void OnThrown(float force);
    }
}
