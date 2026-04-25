using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Core.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.Code.Cards.Hexaghost.Uncommon;

[Pool(typeof(HexaghostCardPool))]
public class Reversal : HexaghostCardModel
{
    public Reversal() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(5, 1);
        WithVar(new RepeatVar(2));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (HexaghostCmd.IsIgnited(Owner))
        {
            // Here it's maybe relevant if the Extinguish happens between attack 1 and attacks 2 and 3
            // or if this is fine. Might be relevant for Vigor that its one attack.
            await HexaghostCmd.Extinguish(Owner);
            await CommonActions.CardAttack(this, cardPlay, 1 + DynamicVars.Repeat.IntValue).Execute(ctx);
        }
        else
        {
            await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        }
    }
}