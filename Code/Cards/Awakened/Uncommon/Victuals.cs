using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Victuals : AwakenedCardModel, IChantable
{
    public Victuals() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithVar("Energy", 2, 1);
        WithKeywords(CardKeyword.Exhaust);
        WithTip(DownfallKeyword.Chant);
    }

    public async Task PlayChantEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
    }
}