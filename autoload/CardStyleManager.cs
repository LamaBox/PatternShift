public static class CardStyleManager
{
	public static CardStyle CurrentStyle { get; set; } = CardStyle.Default;
}

public enum CardStyle
{
	Default,
	Neon
}
