using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Rare;

[Pool(typeof(AwakenedCardPool))]
public class GreatHex : AwakenedCardModel
{
    public GreatHex() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithKeyword(AwakenedKeyword.Chant);
        WithPower<GreatHexPower>(5, 3, false);
        WithTip<ManaburnPower>();
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await ChantCmd.Chant(cardPlay,
            () => CommonActions.Apply<GreatHexPower>(ctx, this, cardPlay)
            );
    }
}