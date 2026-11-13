using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class ManaShield : AwakenedCardModel
{
    public ManaShield() : base(2, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithBlock(14, 4);
        WithTip(AwakenedTip.Conjure);
    }

    public override Texture2D? CustomFrame =>
        ResourceLoader.Load<Texture2D>("res://Awakened/images/dimension/gordian_skill.png");

    public override Material? CreateCustomFrameMaterial => ShaderUtils.GenerateHsv(1, 1, 1);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await AwakenedCmd.Conjure(Owner);
        var card = Owner.RunState.Rng.CombatCardGeneration
            .NextItem(Owner.Hand.Where(c => c is ISpell && c.EnergyCost.GetResolved() > 0));
        card?.EnergyCost.AddThisCombat(-1);
    }
}