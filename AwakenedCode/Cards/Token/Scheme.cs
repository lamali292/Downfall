using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class Scheme : AwakenedCardModel
{
    public Scheme() : base(1, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithPower<SchemePower>(1, 1);
    }

    public override Texture2D? CustomFrame =>
        ResourceLoader.Load<Texture2D>("res://Awakened/images/dimension/vault_skill.png");

    public override Material? CreateCustomFrameMaterial => ShaderUtils.GenerateHsv(1, 1, 1);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SchemePower>(ctx, this);
    }
}