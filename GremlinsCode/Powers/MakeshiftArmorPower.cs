using BaseLib.Abstracts;
using BaseLib.Extensions;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gremlins.GremlinsCode.Powers;

public class MakeshiftArmorPower : GremlinsPowerModel
{
    public MakeshiftArmorPower()
    {
        WithVar("AttacksLeft", 7);
    }

    protected override int? SecondAmount => DynamicVars["AttacksLeft"].IntValue;


    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card.Type != CardType.Attack) return;
        DynamicVars["AttacksLeft"].UpgradeValueBy(-1);
        if (DynamicVars["AttacksLeft"].IntValue <= 0)
        {
            await PowerCmd.Apply<ArtifactPower>(ctx, Owner, Amount, Owner, null);
            DynamicVars["AttacksLeft"].ResetToBase();
            DynamicVars["AttacksLeft"].UpgradeValueBy(7);
        }

        this.InvokeSecondAmountChanged();
    }
}