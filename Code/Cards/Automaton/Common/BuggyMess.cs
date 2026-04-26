using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Downfall.Code.Cards.Automaton.Common;

[Pool(typeof(AutomatonCardPool))]
public class BuggyMess : AutomatonCardModel, IEncodable
{
    public BuggyMess() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithEnergy(1);
        WithTip(DownfallTip.Encode);
        WithTip(typeof(Dazed));
        WithEnergyTip();
        WithCostUpgradeBy(-1);
    }

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await DownfallCardCmd.Insert(ModelDb.Card<Dazed>(), Owner);
        await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
    }
}