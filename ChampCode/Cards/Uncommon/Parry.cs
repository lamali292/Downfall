using BaseLib.Utils;
using Champ.ChampCode.Cards.Common;
using Champ.ChampCode.Core;
using Champ.ChampCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Uncommon;

[Pool(typeof(ChampCardPool))]
public class Parry : ChampCardModel
{
    public Parry() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(8, 4);
        WithPower<CounterPower>(4, 2);
        this.WithPower<ParryingPower>(1, false);
        this.WithTip<RiposteStrike>();
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<CounterPower>(ctx, this);
        await CommonActions.ApplySelf<ParryingPower>(ctx, this);
    }
}