#pragma warning disable S2326
using OpenTelemetry.Resources;

namespace DevHabit.Api.Services.Sorting;

public sealed record SortMapping(string SortField, string PropertyName, bool Reverse = false);

public interface ISortMappingDefinition;

public sealed class SortMappingDefinition<TSource, TDestination> : ISortMappingDefinition
{
    public required SortMapping[] Mappings { get; init; }
}

public sealed class SortMappingProvider(IEnumerable<ISortMappingDefinition> sortMappingDefinitions)
{
    public SortMapping[] GetMappings<TSource, TDefinition>()
    {
        SortMappingDefinition<TSource, TDefinition>? sortMappingDefinition = sortMappingDefinitions
        .OfType<SortMappingDefinition<TSource, TDefinition>>()
        .FirstOrDefault();

        if(sortMappingDefinition is null)
        {
            throw new InvalidOperationException(
                $"The mapping from '{typeof(TSource).Name}' into '{typeof(TDefinition).Name}' isn't defined.");
        }
        return sortMappingDefinition.Mappings;
    }
    
}
