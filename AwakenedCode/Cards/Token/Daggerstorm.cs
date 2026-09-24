using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Daggerstorm : AwakenedCardModel
{
    public Daggerstorm() : base(2, CardType.Power, CardRarity.Token, TargetType.Self)
    {
        WithPower<DaggerstormPower>(4, 2, false);
    }

    public override Texture2D? CustomFrame =>
        ResourceLoader.Load<Texture2D>("res://Awakened/images/dimension/roguebook_power.png");

    public override Material? CreateCustomFrameMaterial => ShaderUtils.GenerateHsv(1, 1, 1);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<DaggerstormPower>(ctx, this);
    }
}