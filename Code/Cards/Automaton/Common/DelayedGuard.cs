using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Extensions;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Automaton.Common;

[Pool(typeof(AutomatonCardPool))]
public class DelayedGuard() : AutomatonCardModel(0, CardType.Skill, CardRarity.Common, TargetType.Self), IEncodable
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<BlockNextTurnPower>(7)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        DownfallKeyword.Encode.ToHoverTip(),
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        await PowerCmd.Apply<BlockNextTurnPower>(Owner.Creature, DynamicVars.Power<BlockNextTurnPower>().BaseValue,
            Owner.Creature, this);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Power<BlockNextTurnPower>().UpgradeValueBy(3);
    }
}