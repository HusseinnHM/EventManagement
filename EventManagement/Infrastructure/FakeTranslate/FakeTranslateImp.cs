using EventManagement.Common.LanguageKey;
using Neptunee.EntityFrameworkCore.MultiLanguage;
using Neptunee.EntityFrameworkCore.MultiLanguage.Types;

namespace EventManagement.Infrastructure.FakeTranslate;

public class FakeTranslateImp : IFakeTranslate
{
    public void Translate(params MultiLanguageProperty[] props)
    {
        foreach (var s in LanguageKeys.Other)
        {
            foreach (var prop in props)
            {
                prop.Upsert(s, $"{prop.GetFirst()} in {s}");
            }
        }
    }
}