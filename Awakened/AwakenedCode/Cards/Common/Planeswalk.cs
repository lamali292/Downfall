using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Cards.Common;

[Pool(typeof(AwakenedCardPool))]
public class Planeswalk : AwakenedCardModel
{
    public Planeswalk() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithEnergy(2, 1);
    }
    
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await DownfallCardCmd.GiveCard<Void>(Owner, PileType.Draw, CardPilePosition.Random);
    }
}