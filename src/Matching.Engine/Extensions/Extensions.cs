using Matching.Engine.Scoring;
using System.Data;
using System.Data.SqlClient;

namespace Matching.Engine.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var services = builder.Services;

        services.AddSingleton<IMatchingRepository, MatchingRepository>();
        services.AddSingleton<IClusteringRepository, ClusteringRepository>();

        services.Configure<List<MatchingOption>>(configuration.GetSection("ProcessorConfig:MatchingOptions"));
    }

    public static void RegisterMatchers(this ContainerBuilder builder)
    {
        builder.RegisterTypes(ReflectionUtils.GetMatchingTypes<IMatcher>())
            .AsImplementedInterfaces()
            .Keyed<IMatcher>(t => ReflectionUtils.GetMatchingTypeKey(t));

        builder.Register(c => new MatcherCache(c.Resolve<IComponentContext>()))
            .SingleInstance();
    }

    public static void RegisterScorers(this ContainerBuilder builder, IConfiguration configuration)
    {
        var matchingOptions = configuration.GetRequiredSection("ProcessorConfig:MatchingOptions").Get<List<MatchingOption>>();

        var bayesTypes = ReflectionUtils.GetMatchingTypes<IBayesFactor>().ToDictionary(getBayesKey);

        foreach (var matchingOption in matchingOptions.ToDictionary(m => m.MatchingKey))
        {
            var matchingKey = matchingOption.Key;
            var scoringOptions = matchingOption.Value.ScoringOptions;

            foreach (var field in scoringOptions.Fields.ToDictionary(f => f.FieldKey))
            {
                string fieldKey = field.Key;
                var bayesFactors = field.Value.BayesFactors.ToDictionary(b => b.ComparatorKey);

                foreach (var bayes in bayesFactors)
                {
                    var bayesKey = bayes.Key;
                    var options = bayes.Value.Bayes;

                    var instance = (IBayesFactor)Activator.CreateInstance(bayesTypes[bayesKey]) ??
                        throw new Exception($"Could not register {typeof(IBayesFactor).FullName}");

                    instance.Factors = options;

                    if (instance.IsValid(out string[] requiredFields) is false)
                    {
                        var key = ReflectionUtils.GetBayesTypeKey(instance.GetType());
                        string fields = string.Join(", ", requiredFields);
                        string error = $"Missing configuration for section '{matchingKey}.{fieldKey}.{key}'. Required fields: '{fields}'";
                        throw new Exception(error);
                    }

                    builder.Register(c => instance)
                        .Keyed($"{matchingKey}.{fieldKey}.{bayesKey}", typeof(IBayesFactor))
                        .SingleInstance();
                }

            }

        }

        builder.Register(c => new BayesCache(c.Resolve<IComponentContext>()));

        static string getBayesKey(Type type) => ReflectionUtils.GetBayesTypeKey(type);
    }

}
