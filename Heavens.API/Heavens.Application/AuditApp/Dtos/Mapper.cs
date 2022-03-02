using Heavens.Application.UserApp.Dtos;

namespace Heavens.Application.AuditApp.Dtos;

public class Mapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Audit, AuditPage>()
            .Map(dest => dest.HasBody, src => true, src => !src.Body.IsEmpty())
            .Map(dest => dest.HasQuery, src => true, src => !src.Query.IsEmpty())
            .Map(dest => dest.HasException, src => true, src => !src.Exception.IsEmpty())
            .Map(dest => dest.ServiceName, src => src.ServiceName.Split(".", StringSplitOptions.None).Last(), src => !src.ServiceName.IsEmpty())
            .Ignore(dest => dest.Body)
            .Ignore(dest => dest.Query)
            .Ignore(dest => dest.ReturnValue);

        config.ForType<Audit, AuditDto>()
            .Map(dest => dest.ServiceName, src => src.ServiceName.Split(".", StringSplitOptions.None).Last(), src => !src.ServiceName.IsEmpty());
    }
}
