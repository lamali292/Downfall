using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Displays;
using Downfall.Code.Interfaces;
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
public class Separator() : AutomatonCardModel(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy), IEncodable
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, ValueProp.Move),
        new DamageVar("ExtraDamage", 6, ValueProp.Move)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(DownfallKeywords.Encode)
    ];

    public async Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        var maxSlots = AutomatonCmd.GetMax(Owner);
        var isMiddle = encodeContext is { IsFromFunction: true, SlotIndex: > 0 }
                       && encodeContext.SlotIndex < maxSlots - 1;
        var amount = DynamicVars.Damage.IntValue + (isMiddle ? DynamicVars["ExtraDamage"].IntValue : 0);

        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(amount).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(ctx);
    }

    public LocString? GetEncodeLocString(EncodeContext context)
    {
        var maxSlots = AutomatonCmd.GetMax(Owner);
        var isMiddle = context is { IsFromFunction: true, SlotIndex: > 0 }
                       && context.SlotIndex < maxSlots - 1;

        if (!isMiddle)
            return IEncodable.BuildEncodeLocString(this);

        var loc = new LocString("encode", Id.Entry + ".encode");
        var doubled = new DamageVar(DynamicVars.Damage.IntValue + DynamicVars["ExtraDamage"].IntValue,
            DynamicVars.Damage.Props);
        doubled.SetOwner(this);
        loc.Add(doubled);
        return loc;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
        DynamicVars["ExtraDamage"].UpgradeValueBy(2);
    }
}