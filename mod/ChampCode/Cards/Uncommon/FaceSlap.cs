using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class FaceSlap : ChampCardModel
{
    public FaceSlap() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
        WithPower<VulnerablePower>(2, 1);
        WithBerserkerCombo();
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await ChampCmd.BerserkerCombo(cardPlay, async () =>
        {
            if (cardPlay.Target != null)
                await CommonActions.Apply<VulnerablePower>(ctx, cardPlay.Target, this);
        });
    }
}