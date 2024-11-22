namespace Vitec.CameraHead.UnitTests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Models.Configuration;
using MotionTest;
using Xunit;

public class StudioConfigurationTests
{
    [Fact]
    public void ConfigurationBuilder_ShouldLoadTargetNamesConfiguration()
    {
        var configuration = Configuration.BuildConfiguration(Assembly.GetExecutingAssembly());
        var targets = configuration
            .GetConfigurationSection<IEnumerable<string>>(Constants.Configuration.Targets)
            .ToArray();

        targets.Should().NotBeNull();
        targets.Should().NotBeEmpty();
        targets.Should().HaveCount(4);
        targets.Should().Contain(x => x == "Target 1");
    }

    [Fact]
    public void ConfigurationBuilder_ShouldLoadCameraHeadConfiguration()
    {
        var configuration = Configuration.BuildConfiguration(Assembly.GetExecutingAssembly());
        var cameraHeads = configuration
            .GetConfigurationSection<IEnumerable<CameraHead>>(Constants.Configuration.CameraHeads)
            .ToArray();

        cameraHeads.Should().NotBeNull();
        cameraHeads.Should().NotBeEmpty();
        cameraHeads.Should().HaveCount(5);
        cameraHeads.Should().Contain(x => x.Name == "Test Head - Fixed pan/tilt velocity");
        cameraHeads[0].TargetPositions.Should().NotBeNull();
        cameraHeads[0].TargetPositions.Should().NotBeEmpty();
        cameraHeads[0].TargetPositions[0].Should().BeEquivalentTo(new TargetPosition {Pan = 5.0, Tilt = 4.4});
    }

    [Fact]
    public void ConfigurationExtensions_ShouldCreateStudioFromConfiguration()
    {
        var configuration = Configuration.BuildConfiguration(Assembly.GetExecutingAssembly());

        var studio = configuration.CreateStudio();

        studio.Should().NotBeNull();
        studio.Targets.Should().NotBeNull();
        studio.Targets.Should().NotBeEmpty();
        studio.Targets.Should().HaveCount(4);
    }

    [Fact]
    public void ConfigurationBuilderWhenConfigurationNull_ShouldThrowException()
    {
        var configuration = new ConfigurationBuilder().Build();

        var createStudio = () => configuration.CreateStudio();
        createStudio.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ConfigurationBuilderWhenTargetsEmpty_ShouldThrowException()
    {
        var configuration = BuildConfiguration(["EmptyTargets.json"]);

        var createStudio = () => configuration.CreateStudio();
        createStudio.Should().Throw<ArgumentNullException>()
            .WithMessage(FormatParameterError(Constants.ErrorMessages.TargetNamesRequired, "targetNames"));
    }

    [Fact]
    public void ConfigurationBuilderWhenCameraHeadsEmpty_ShouldThrowException()
    {
        var configuration = BuildConfiguration(["EmptyCameraHeads.json"]);

        var createStudio = () => configuration.CreateStudio();
        createStudio.Should().Throw<ArgumentNullException>()
            .WithMessage(FormatParameterError(Constants.ErrorMessages.CameraHeadsRequired, "cameraHeads"));
    }

    [Fact]
    public void ConfigurationBuilderWhenCameraHeadTypeEmpty_ShouldThrowException()
    {
        var configuration = BuildConfiguration(["EmptyCameraHeadType.json"]);

        var createStudio = () => configuration.CreateStudio();
        createStudio.Should().Throw<ArgumentNullException>()
            .WithMessage(FormatParameterError(Constants.ErrorMessages.CameraHeadTypeRequired, "Type"));
    }

    [Fact]
    public void ConfigurationBuilderWhenCameraHeadTypInvalid_ShouldThrowException()
    {
        var configuration = BuildConfiguration(["InvalidCameraHeadType.json"]);

        var createStudio = () => configuration.CreateStudio();
        createStudio.Should().Throw<InvalidOperationException>()
            .WithMessage(string.Format(Constants.ErrorMessages.InvalidCameraHeadsType, "Fred"));
    }

    private static IConfiguration BuildConfiguration(string[] jsonFiles = null)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory());

        if (jsonFiles != null)
            foreach (var jsonFile in jsonFiles)
                builder.AddJsonFile(jsonFile);

        return builder.Build();
    }

    private string FormatParameterError(string error, string parameterType )
    {
        return $"{error} (Parameter '{parameterType}')";
    }
}