using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Relics;

[Pool(typeof(HexaghostRelicPool))]
public class SoulStone : HexaghostRelicModel
{

    public SoulStone() : base(RelicRarity.Rare)
    {
        WithTip(CardKeyword.Exhaust);
        WithTip(HexaghostTip.Ignite);
        WithCards(4);
    }
    
    private int _exhausted;
    private bool _isActivating;

    public override bool ShowCounter => CombatManager.Instance.IsInProgress;
    public override int DisplayAmount => _isActivating ? DynamicVars.Cards.IntValue : _exhausted;

    public override Task BeforeCombatStart()
    {
        _exhausted = 0;
        _isActivating = false;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        var amount = DynamicVars.Cards.IntValue;
        if (card.Owner != Owner) return;
        _exhausted++;
        Status = _exhausted >= amount - 1 ? RelicStatus.Active : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
        if (_exhausted < amount) return;
        _exhausted = 0;
        _ = DoActivateVisuals();
        await HexaghostCmd.Ignite(ctx, card.Owner);
    }
    //todo should happen after here and now + should let the player advance if it ignites a ghostflame, so the end of turn advance check should happen after ethereal cards exhaust just in case

    private async Task DoActivateVisuals()
    {
        _isActivating = true;
        Flash();
        Status = RelicStatus.Normal;
        InvokeDisplayAmountChanged();
        await Cmd.Wait(1f);
        _isActivating = false;
        InvokeDisplayAmountChanged();
    }
}