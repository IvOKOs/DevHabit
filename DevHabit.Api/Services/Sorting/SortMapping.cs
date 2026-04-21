#pragma warning disable S2326
using OpenTelemetry.Resources;

namespace DevHabit.Api.Services.Sorting;

public sealed record SortMapping(string SortField, string PropertyName, bool Reverse = false);
