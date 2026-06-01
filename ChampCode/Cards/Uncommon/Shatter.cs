using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class Shatter : ChampCardModel
{
    public Shatter() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(14, 2);
        WithPower<VulnerablePower>(1, 1);
        WithPower<WeakPower>(1, 1);
        WithTip(ChampTip.Stance);
        WithTip(ChampTip.Combo);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override bool ShouldGlowGoldInternal =>
        Owner.ShouldBerserkerComboTrigger() || Owner.ShouldDefensiveComboTrigger();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target).Execute(ctx);
        if ((!Owner.ShouldDefensiveComboTrigger() && !Owner.ShouldBerserkerComboTrigger()) ||
            cardPlay.Target == null) return;
        await CommonActions.Apply<VulnerablePower>(ctx, cardPlay.Target, this);
        await CommonActions.Apply<WeakPower>(ctx, cardPlay.Target, this);
    }
}