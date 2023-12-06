using Neptunee.EntityFrameworkCore.MultiLanguage.Types;

namespace EventManagement.Infrastructure.FakeTranslate;

public interface IFakeTranslate
{
    void Translate(params MultiLanguageProperty[] props);
}