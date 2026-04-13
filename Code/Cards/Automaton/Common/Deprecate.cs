using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
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
public class Deprecate() : AutomatonCardModel(0, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy), IEncodable
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WeakPower>(1)];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(DownfallKeywords.Encode),
        HoverTipFactory.FromPower<WeakPower>()
    ];

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await PowerCmd.Apply<WeakPower>(cardPlay.Target, DynamicVars.Weak.BaseValue, Owner.Creature, this);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Weak.UpgradeValueBy(1);
    }
}