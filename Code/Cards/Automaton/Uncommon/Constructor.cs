using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Automaton.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Constructor() : AutomatonCardModel(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self), IEncodable
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(5, ValueProp.Move),
        new BlockVar("ExtraBlock", 5, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(DownfallKeywords.Encode)
    ];

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        var isFirst = encodeContext is { IsFromFunction: true, SlotIndex: 0 };
        var amount = DynamicVars.Block.IntValue + (isFirst ? DynamicVars["ExtraBlock"].IntValue : 0);
        await CreatureCmd.GainBlock(Owner.Creature, amount, DynamicVars.Block.Props, cardPlay);
    }

    public LocString? GetEncodeLocString(EncodeContext context)
    {
        if (context is not { IsFromFunction: true, SlotIndex: 0 })
            return IEncodable.BuildEncodeLocString(this);

        // Build a loc string with the doubled value
        var loc = new LocString("encode", Id.Entry + ".encode");
        var doubled = new BlockVar(DynamicVars.Block.IntValue + DynamicVars["ExtraBlock"].IntValue,
            DynamicVars.Block.Props);
        doubled.SetOwner(this);
        loc.Add(doubled);
        return loc;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2);
        DynamicVars["ExtraBlock"].UpgradeValueBy(2);
    }
}