using CodeModels.Models;
using CodeModels.Models.Primitives.Expression.Reference;

namespace ApiGenerator.Settings;

public record ControllerSettings(
    ControllerNamePrefix NamePrefix,
    InstanceType InstanceType,
    BoolSetting IncludeErrorAnnotation,
    IType? ExceptionType,
    RestEndpointSettings RestEndpointSettings)
{
    public static ControllerSettings Create(
        ControllerNamePrefix namePrefix = ControllerNamePrefix.Controller,
        InstanceType instanceType = InstanceType.Auto,
        BoolSetting includeErrorAnnotation = BoolSetting.Auto,
        IType? exceptionType = default,
        RestEndpointSettings? restEndpointSettings = null)
    {
        var restEndpointsSettings = (restEndpointSettings ?? RestEndpointSettings.Default) with
        { IsStatic = instanceType is InstanceType.Static or InstanceType.Auto ? BoolSetting.True : BoolSetting.False };
        var settings = new ControllerSettings(
            NamePrefix: namePrefix,
            InstanceType: instanceType,
            IncludeErrorAnnotation: includeErrorAnnotation,
            ExceptionType: exceptionType,
            RestEndpointSettings: restEndpointsSettings);

        return settings;
    }

    public static readonly ControllerSettings Default = Create();
}