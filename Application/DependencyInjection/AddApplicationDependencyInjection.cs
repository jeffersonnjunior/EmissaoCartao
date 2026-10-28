using Application.Cartoes.EmitirCartoes;
using Application.Cartoes.ObterCartoes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyInjection;

public static class AddApplicationDependencyInjection
{
    public static void DependencyInjectionApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        _ = configuration;

        services.AddScoped<IEmitirCartoesReaction, EmitirCartoesReaction>();
        services.AddScoped<EmitirCartoesFromPropostaGeradaHandler>();
        services.AddScoped<ObterCartoesHandler>();
    }
}
