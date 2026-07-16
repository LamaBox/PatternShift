using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ClassicCardValidator : BaseCardValidator
{
    public override bool Validate(List<CardData> cardList)
    {
        if (cardList.Count != 3)
        {
            GD.PrintErr("Invalid number of cards. Expected 3, got " + cardList.Count); //Сообщение об ошибке, если количество карточек не равно 3
            return false;
        }

        //Проверка всех свойств карточек на соответствие правилам игры "Set"
        return CheckProperty(cardList, c => c.Figure) &&
               CheckProperty(cardList, c => c.Color) &&
               CheckProperty(cardList, c => c.Fill) &&
               CheckProperty(cardList, c => c.Count);
    }
    private bool CheckProperty<T>(List<CardData> cardList, Func<CardData, T> propertySelector)
    {
        var firstValue = propertySelector(cardList[0]);
        var secondValue = propertySelector(cardList[1]);
        var thirdValue = propertySelector(cardList[2]);
        
        return (firstValue.Equals(secondValue) && secondValue.Equals(thirdValue)) ||
               (!firstValue.Equals(secondValue) && !secondValue.Equals(thirdValue) && !firstValue.Equals(thirdValue));
    }
}