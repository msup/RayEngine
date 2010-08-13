namespace WpfOpenTK.OpenGL.Engine
    {
    public enum RenderingParameter
        {
        IntegrationStep,

        AmbientLightColor,
        DiffuseLightColor,
        SpecularLightColor,

        AmbientLightCoeff,
        DiffuseLightCoeff,
        SpecularLightCoeff,
        SpecularPowerFactor,

        LightPosition,

        TransluentSpaceSkip,
        TransluentSpaceSkipStep,
        TransluentSpaceThreshold,

        EnhanceWithGradient,
        GradientModulationCoeff,

        EnhanceWithNormal,
        NormalModulationCoeff,

        TransluencyCorrectionType,
        TransluencyCorrectionFactor,

        FinalAlphaMixColor,
        FinalAlphaMixThreshold,
        FinalAlphaMixFactor,
        }
    }