using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Interfaces;
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Cards.Uncommon;

[Pool(typeof(AutomatonCardPool))]
public class Constructor : AutomatonCardModel, IEncodable
{
    public Constructor() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(5, 2);
        WithVars(new BlockVar("ExtraBlock", 5, ValueProp.Move).WithUpgrade(2));
        
    }

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
}