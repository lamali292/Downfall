using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Interfaces;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Gems;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;

namespace Guardian.GuardianCode.Cards.Abstract;

#pragma warning disable STS001
[Pool(typeof(GuardianCardPool))]
public class FragmentedGemCard : GemCard<FragmentedGem>;

[Pool(typeof(GuardianCardPool))]
public class Ruby : GemCard<RubyGem>;

[Pool(typeof(GuardianCardPool))]
public class Sapphire : GemCard<SapphireGem>;

[Pool(typeof(GuardianCardPool))]
public class Tourmaline : GemCard<TourmalineGem>;

[Pool(typeof(GuardianCardPool))]
public class Amber : GemCard<AmberGem>;

[Pool(typeof(GuardianCardPool))]
public class Amethyst : GemCard<AmethystGem>;

[Pool(typeof(GuardianCardPool))]
public class Aquamarine : GemCard<AquamarineGem>;

[Pool(typeof(GuardianCardPool))]
public class Emerald : GemCard<EmeraldGem>;

[Pool(typeof(GuardianCardPool))]
public class Garnet : GemCard<GarnetGem>;

[Pool(typeof(GuardianCardPool))]
public class Opal : GemCard<OpalGem>;

[Pool(typeof(GuardianCardPool))]
public class Citrine : GemCard<CitrineGem>;

[Pool(typeof(GuardianCardPool))]
public class Onyx : GemCard<OnyxGem>;

[Pool(typeof(GuardianCardPool))]
public class Rutile : GemCard<RutileGem>;

[Pool(typeof(GuardianCardPool))]
public class Diamond : GemCard<DiamondGem>
{
    public Diamond()
    {
        WithKeyword(CardKeyword.Unplayable);
    }

    protected override bool IsPlayable => false;
    protected override int CanonicalEnergyCost => -1;
}

[Pool(typeof(GuardianCardPool))]
public class Bismuth : GemCard<BismuthGem>
{
    public Bismuth()
    {
        //WithKeyword(CardKeyword.Unplayable);
    }
   //protected override bool IsPlayable => false;
   protected override int CanonicalEnergyCost => 1;
}

#pragma warning restore STS001

public abstract class GemCard<T> : GuardianCardModel, IGemCard, IGemSocketCard, ICustomTypePlaque, IModfyCardDescription
    where T : GemModel
{
    protected GemCard() : base(0, CardType.Skill, CardRarity.None, TargetType.Self)
    {
        _titleLocString = GuardianModelDb.Gem<T>().Title;
        WithKeyword(GuardianKeyword.Gem);
        CardModifier.AddModifier(this, GuardianModelDb.Gem<T>().ToMutable());
    }

    public override bool CanBeGeneratedInCombat => false;

    public override CardRarity Rarity => GuardianModelDb.Gem<T>().Rarity;
    public override int MaxUpgradeLevel => 0;

    public LocString GetTypePlaqueName => new("gameplay_ui", "GUARDIAN-GEM");

    public GemModel CanonicalGemModel => GuardianModelDb.Gem<T>();

    public GemModel GemModel =>
        CardModifier.DirectModifiers(this).OfType<GemModel>().First();

    public int GemSlots => 0;

    public LocString ModifyDescription(LocString oldLocString)
    {
        return new LocString("cards", "GUARDIAN-GEM_CARD.description");
    }
}

public interface IGemCard
{
    GemModel GemModel { get; }
    GemModel CanonicalGemModel { get; }
}