using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class ChosenVerse : AwakenedCardModel
{
    public ChosenVerse() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(4, 1);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var power = await CommonActions.ApplySelf<ChosenVersePower>(ctx, this, 2);
        if (power == null) return;
        var block = Hook.ModifyBlock(CombatState!, Owner.Creature, DynamicVars.Block.IntValue, ValueProp.Move, this,
            cardPlay, out _);
        power.SetBlock(block);
        power.CardPlay = cardPlay;
    }
}