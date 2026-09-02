using BaseLib.Utils;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Cards.Rare;

[Pool(typeof(HexaghostCardPool))]
public class Flashbang : HexaghostCardModel
{
    public Flashbang() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(5, 1);
        WithPower<WeakPower>(1, 1);
        WithPower<FlashbangPower>(3, 1, false);
        WithTip<StrengthPower>();
        WithTip(HexaghostTip.Ignite);
    }

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (!HexaghostCmd.IsIgnited(Owner)) return;
        await CommonActions.Apply<FlashbangPower>(ctx, this, cardPlay);
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
    }
}

public class FlashbangPower : TemporaryDebuffPowerWrapper<Flashbang, StrengthPower>
{
}