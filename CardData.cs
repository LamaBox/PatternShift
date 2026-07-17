using Godot;

public enum CardFigure { Ellipse, Rhomb, Snot }

public enum CardColor { Red, Green, Violet }

public enum CardFill { Empty, Striped, Full }

public enum CardCount { One, Two, Three }

[GlobalClass]
public partial class CardData : Resource
{
    public CardFigure Figure { get; set; }
    public CardColor Color { get; set; }
    public CardFill Fill { get; set; }
    public CardCount Count { get; set; }

    public CardData(CardFigure figure, CardColor color, CardFill fill, CardCount count)
    {
        Figure = figure;
        Color = color;
        Fill = fill;
        Count = count;
    }
}