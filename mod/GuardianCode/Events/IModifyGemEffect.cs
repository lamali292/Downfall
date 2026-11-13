using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Events;

public interface IModifyGemEffect
{
    decimal ModifyGemEffect(GemModel model, decimal baseValue, CardModel? card);
}