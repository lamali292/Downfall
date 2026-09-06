using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Interfaces;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Collector.CollectorCode.Cards.Basic;

[Pool(typeof(CollectorCardPool))]
public class Fireball : CollectorCardModel
{
    public Fireball() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithKeyword(CollectorKeyword.Pyre);
        WithTip(CardKeyword.Retain);
        WithDamage(18, 3);
        WithUpgradeChangingCardTip<Burn, Ember>();
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        if (cardPlay.Target == null) return;

        await CommonActions.CardAttack(this, cardPlay).WithHitFx("vfx/vfx_molten_fist", tmpSfx: "blunt_attack.mp3")
            .Execute(ctx);
        if (IsUpgraded)
        {
            await DownfallCardCmd.GiveCards<Ember>(Owner, PileType.Hand, 1);
        }
        else
        {
            await DownfallCardCmd.GiveCards<Burn>(Owner, PileType.Hand, 1);
        }
    }
}