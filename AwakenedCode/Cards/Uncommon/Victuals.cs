using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Victuals : AwakenedCardModel
{
    public Victuals() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithEnergy(2, 1);
        WithKeywords(CardKeyword.Exhaust);
        WithKeyword(AwakenedKeyword.Chant);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await ChantCmd.Chant(cardPlay,
            () => PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner)
            );
    }
    
}