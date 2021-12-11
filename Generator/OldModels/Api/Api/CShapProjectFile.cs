
//// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
///// <remarks/>
//[System.Serializable()]
//[System.ComponentModel.DesignerCategory("code")]
//[System.Xml.Serialization.XmlType(AnonymousType = true)]
//[System.Xml.Serialization.XmlRoot(Namespace = "", IsNullable = false)]
//public partial class Project
//{

//    private ProjectPropertyGroup propertyGroupField;

//    private ProjectPackageReference[] itemGroupField;

//    private string sdkField;

//    /// <remarks/>
//    public ProjectPropertyGroup PropertyGroup
//    {
//        get => propertyGroupField;
//        set => propertyGroupField = value;
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlArrayItem("PackageReference", IsNullable = false)]
//    public ProjectPackageReference[] ItemGroup
//    {
//        get => itemGroupField;
//        set => itemGroupField = value;
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlAttribute()]
//    public string Sdk
//    {
//        get => sdkField;
//        set => sdkField = value;
//    }
//}

///// <remarks/>
//[System.Serializable()]
//[System.ComponentModel.DesignerCategory("code")]
//[System.Xml.Serialization.XmlType(AnonymousType = true)]
//public partial class ProjectPropertyGroup
//{

//    private string[] targetFrameworkField;

//    private decimal langVersionField;

//    private string nullableField;

//    /// <remarks/>
//    [System.Xml.Serialization.XmlElement("TargetFramework")]
//    public string[] TargetFramework
//    {
//        get => targetFrameworkField;
//        set => targetFrameworkField = value;
//    }

//    /// <remarks/>
//    public decimal LangVersion
//    {
//        get => langVersionField;
//        set => langVersionField = value;
//    }

//    /// <remarks/>
//    public string Nullable
//    {
//        get => nullableField;
//        set => nullableField = value;
//    }
//}

///// <remarks/>
//[System.Serializable()]
//[System.ComponentModel.DesignerCategory("code")]
//[System.Xml.Serialization.XmlType(AnonymousType = true)]
//public partial class ProjectPackageReference
//{

//    private string includeField;

//    private string versionField;

//    /// <remarks/>
//    [System.Xml.Serialization.XmlAttribute()]
//    public string Include
//    {
//        get => includeField;
//        set => includeField = value;
//    }

//    /// <remarks/>
//    [System.Xml.Serialization.XmlAttribute()]
//    public string Version
//    {
//        get => versionField;
//        set => versionField = value;
//    }
//}

