public class BottleView : ButtonView
{
    public void OnSelected(bool selected)
    {
        if (selected)
        {
            AudioEngine.PlaySfx("ColorSortBottleButtonDown");
        }
    }

    public void OnBallInserted()
    {
        AudioEngine.PlaySfx("ColorSortBallInsert", true);
    }
}