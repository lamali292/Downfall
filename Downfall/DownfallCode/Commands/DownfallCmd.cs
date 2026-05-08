using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Commands;

public class DownfallCmd
{
    
    public static Task GainTempHp(PlayerChoiceContext ctx, CardModel card)
     => PowerCmd.Apply<TempHpPower>(ctx, card.Owner.Creature, card.DynamicVars["TempHP"].BaseValue, card.Owner.Creature,
         card);
}