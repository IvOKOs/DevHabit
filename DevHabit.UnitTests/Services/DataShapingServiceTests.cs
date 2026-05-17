using DevHabit.Api.Services;
using System.Collections.Generic;
using System.Dynamic;

namespace DevHabit.UnitTests.Services;

public class DataShapingServiceTests
{
    private readonly DataShapingService _dataShapingService = new();

    private sealed record TestDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required int Value { get; set; }
    }

    [Fact]
    public void ShapeData_ShouldReturnCorrectFields_WhenFieldsAreSpecified()
    {
        // Arrange
        string fields = "id,name,value";
        var entity = new TestDto
        {
            Id = "t_123",
            Name = "Test",
            Description = "lala",
            Value = 1
        };

        // Act
        ExpandoObject result = _dataShapingService.ShapeData(entity, fields);

        // Assert
        IDictionary<string, object?> dict = result;
        Assert.Equal(3, dict.Count);
        Assert.Equal(entity.Id, (string)dict["Id"]!);
        Assert.Equal(entity.Value, dict["Value"]);
        Assert.False(dict.ContainsKey("Description"));
    }

    [Fact]
    public void ShapeData_ShouldReturnAllFields_WhenFieldsAreNull()
    {
        // Arrange
        string? fields = null;
        var entity = new TestDto
        {
            Id = "t_123",
            Name = "Test",
            Description = "lala",
            Value = 1
        };

        // Act
        ExpandoObject result = _dataShapingService.ShapeData(entity, fields);

        // Assert
        IDictionary<string, object?> dict = result;
        Assert.Equal(4, dict.Count);
        Assert.Equal(entity.Id, dict["Id"]);
        Assert.Equal(entity.Name, dict["Name"]);
        Assert.Equal(entity.Description, dict["Description"]);
        Assert.Equal(entity.Value, dict["Value"]);
    }
}
