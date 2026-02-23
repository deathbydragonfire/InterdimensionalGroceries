namespace InterdimensionalGroceries.Core
{
    public interface IClickable
    {
        void OnClick();
        bool CanClick() => true;
        string GetTooltipText() => "[LMB to Click]";
        UnityEngine.Color GetTooltipColor() => UnityEngine.Color.white;
    }
}
