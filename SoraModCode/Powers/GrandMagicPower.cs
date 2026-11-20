using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using SoraMod.SoraModCode.Keywords;
using SoraMod.SoraModCode.Powers;

namespace SoraMod.SoraModCode.Powers;

public class GrandMagicPower() : SoraModPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;

    // +1 duration (and other effects) to magic card's statuses

    public override async Task BeforeCardPlayed(CardPlay play)
    {
        var card = play.Card;
        if (card.Keywords.Contains(SoraModKeywords.Magic) && card.Owner.Creature == Owner)
        {
            foreach (var kvp in card.DynamicVars)
            {
                var key = kvp.Key;
                var value = kvp.Value;

                // ✅ Check if it's a PowerVar by checking the type
                Type valueType = value.GetType();
                if (valueType.IsGenericType &&
                    valueType.GetGenericTypeDefinition() == typeof(PowerVar<>))
                {
                    // ✅ Get the power type from the generic argument
                    Type powerType = valueType.GetGenericArguments()[0];

                    // ✅ Get the BaseValue using the property
                    var baseValueProp = valueType.GetProperty("BaseValue");
                    if (baseValueProp == null) continue;

                    decimal originalAmount = (decimal)baseValueProp.GetValue(value);

                    // ✅ Set the new value (try property first, then field)
                    if (baseValueProp.CanWrite)
                    {
                        baseValueProp.SetValue(value, originalAmount + 1);
                    }
                    else
                    {
                        var field = valueType.GetField("_baseValue",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                        if (field != null)
                        {
                            field.SetValue(value, originalAmount + 1);
                        }
                    }
                }
            }
        }

        await Task.CompletedTask;
    }

}