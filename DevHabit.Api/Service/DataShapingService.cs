using System.Collections.Concurrent;
using System.Dynamic;
using System.Reflection;

namespace DevHabit.Api.Service;

public sealed class DataShapingService
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertiesCache = new();

    public ExpandoObject ShapeData<T>(T entity, string? fields)
    {
        HashSet<string> fieldsToSet = fields?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        PropertyInfo[] propertyInfos = PropertiesCache.GetOrAdd(
            typeof(T),
            t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));// get props of the generic type

        if (fieldsToSet.Any())
        {
            propertyInfos = propertyInfos
            .Where(p => fieldsToSet.Contains(p.Name))
            .ToArray();// filter out only the props that are requested
        }

        IDictionary<string, object?> shapedObject = new ExpandoObject();
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
            shapedObject[propertyInfo.Name] = propertyInfo.GetValue(entity);
        }
        return (ExpandoObject)shapedObject;
    }

    public List<ExpandoObject> ShapeData<T>(
        IEnumerable<T> entities,// resources
        string? fields)// fields that I want to include in the data shaped response;
                      // fields is not nullable since we can't do data shaping if we don't have the fields

    {
        HashSet<string> fieldsToSet = fields?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase) ?? [];

        PropertyInfo[] propertyInfos = PropertiesCache.GetOrAdd(
            typeof(T),
            t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));// get props of the generic type

        if (fieldsToSet.Any())
        {
            propertyInfos = propertyInfos
            .Where(p => fieldsToSet.Contains(p.Name))
            .ToArray();// filter out only the props that are requested
        }
        

        List<ExpandoObject> shapedObjects = [];
        foreach(T entity in entities)
        {
            IDictionary<string, object?> shapedObject = new ExpandoObject();
            foreach(PropertyInfo propertyInfo in propertyInfos)
            {
                shapedObject[propertyInfo.Name] = propertyInfo.GetValue(entity);
            }
            shapedObjects.Add((ExpandoObject)shapedObject);
        }
        return shapedObjects;
    }

    public bool Validate<T>(string? fields)
    {
        if (string.IsNullOrWhiteSpace(fields))
        {
            return true;// empty fields is valid, because we return the whole dto
        }

        var fieldsToSet = fields
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        PropertyInfo[] propertyInfos = PropertiesCache.GetOrAdd(
            typeof(T),
            t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance));

        return fieldsToSet.All(f => propertyInfos.Any(p => p.Name.Equals(f, StringComparison.OrdinalIgnoreCase)));
    }
}
