using Collector.CollectorCode.CustomEnums;
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Collector.CollectorCode.Cards.Basic;

[Pool(typeof(CollectorCardPool))]
public class Fireball : CollectorCardModel
{
    public Fireball() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithTorchheadDamage(10, 3);
        WithUpgradeChangingCardTip<Burn, Ember>();
        WithTip(CollectorKeyword.Torchhead);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    /// <summary>Torchhead hits what you pick; while it targets ALL enemies (Equip Axe) there is nothing to pick.</summary>
    public override TargetType TargetType => _owner != null && IsMutable && ShouldTorcheadTargetAll
        ? TargetType.AllEnemies
        : TargetType.AnyEnemy;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await TorchheadCmd.Attack(ctx, this, cardPlay, attack => attack.WithHitFx("vfx/vfx_molten_fist", tmpSfx: "blunt_attack.mp3"));
        if (IsUpgraded)
            await DownfallCardCmd.GiveCards<Ember>(Owner, PileType.Hand, 1);
        else
            await DownfallCardCmd.GiveCards<Burn>(Owner, PileType.Hand, 1);
    }
}
