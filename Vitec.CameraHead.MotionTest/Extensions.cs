﻿namespace Vitec.CameraHead.MotionTest;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Models;
using Models.Configuration;

/// <summary>
///     Static extension helper methods
/// </summary>
public static class Extensions
{
    /// <summary>
    ///     Create Studio from configuration data
    /// </summary>
    /// <param name="configuration"></param>
    /// <remarks>
    ///     The configuration data was intentionally structured differently to the Studio model to demonstrate
    ///     using extension methods and Linq to load and project the data from one format to another
    /// </remarks>
    /// <returns></returns>
    public static Studio CreateStudio(this IConfiguration configuration)
    {
        // Load studio data from configuration

        var targetNames = configuration
            .GetConfigurationSection<IEnumerable<string>>(Constants.Configuration.Targets);

        var cameraHeads = configuration
            .GetConfigurationSection<IEnumerable<CameraHead>>(Constants.Configuration.CameraHeads)?.ToArray();

        return new Studio(
            targetNames.ToTargets(cameraHeads),
            // ReSharper disable once AssignNullToNotNullAttribute
            cameraHeads.Select(x => x.ToCameraHead()));
    }

    private static ICameraHead ToCameraHead(this CameraHead cameraHead)
    {
        if (string.IsNullOrWhiteSpace(cameraHead.Type)) throw new ArgumentNullException(nameof(cameraHead.Type), Constants.ErrorMessages.CameraHeadTypeRequired);

        var cameraType = Type.GetType(cameraHead.Type);

        if (cameraType == null) throw new InvalidOperationException(string.Format(Constants.ErrorMessages.InvalidCameraHeadsType, cameraHead.Type));

        // Build constructor arguments with Type.Missing for optional parameters which are not supplied
        var constructorArgs = new object[]
        {
            cameraHead.Name,
            cameraHead.PanVelocity,
            cameraHead.TiltVelocity,
            TimeProvider.System
        };

        return (ICameraHead) Activator.CreateInstance(cameraType,
            BindingFlags.CreateInstance |
            BindingFlags.Public |
            BindingFlags.Instance,
            null,
            constructorArgs,
            CultureInfo.CurrentCulture);
    }

    private static IEnumerable<Target> ToTargets(this IEnumerable<string> targetNames, IEnumerable<CameraHead> cameraHeads)
    {
        if (targetNames == null) throw new ArgumentNullException(nameof(targetNames), Constants.ErrorMessages.TargetNamesRequired);

        if (cameraHeads == null) throw new ArgumentNullException(nameof(cameraHeads), Constants.ErrorMessages.CameraHeadsRequired);

        // Project configuration settings into Target creating Shots from position info on the Camera Heads
        // Example of using indexing in Linq queries
        return targetNames.Select((x, index) =>
            new Target(x, cameraHeads
                .Select(y => new Shot(y.Name, y.ToPosition(index))).ToArray()));
    }

    /// <summary>
    ///     Map nth target position to new instance of a Position
    /// </summary>
    /// <param name="cameraHead"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private static Position ToPosition(this CameraHead cameraHead, int index)
    {
        var targetPosition = cameraHead.TargetPositions[index];
        return new Position(targetPosition.Pan, targetPosition.Tilt);
    }

    /// <summary>
    ///     Calc total distance to travel in both Pan and Tilt axes
    ///     Use this to calculate progress. We can use the combined value as both change over time
    ///     and it is easier than determining which is the longer distance each time.
    /// </summary>
    /// <param name="startPosition"></param>
    /// <param name="endPosition"></param>
    /// <returns></returns>
    public static double DistanceTo(this Position startPosition, Position endPosition)
    {
        return Math.Abs(endPosition.Pan - startPosition.Pan) + Math.Abs(endPosition.Tilt - startPosition.Tilt);
    }
}