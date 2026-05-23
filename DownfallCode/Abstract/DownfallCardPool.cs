using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallCardPool<T> : CustomCardPoolModel
    where T : DownfallCharacterModel
{
    private static T Character => ModelDb.Character<T>();
    public override string Title => Character.CharId!;
    private static string ModId => Character.ModId!;

    public override string BigEnergyIconPath =>
        $"res://{ModId}/images/character/energy_icon.png";

    public override string TextEnergyIconPath =>
        $"res://{ModId}/images/character/energy_text_icon.png";

    public override float H => Character.CardColorH;
    public override float S => Character.CardColorS;
    public override float V => Character.CardColorV;


    public override Color DeckEntryCardColor => Character.DeckEntryCardColor;
    public override bool IsColorless => false;
}