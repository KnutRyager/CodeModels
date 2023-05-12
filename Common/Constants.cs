using System;

namespace Common.Util;

public static class Constants
{
    public const string DEFAULT_PC_USER = "E-K-n";
    public static readonly string DEFAULT_USER_FOLDER = $@"C:\Users\{DEFAULT_PC_USER}\";
    public static readonly string DEFAULT_DCOCUMENTS_FOLDER = $@"{DEFAULT_USER_FOLDER}Documents\";
    public static readonly string DEFAULT_PROGRAM_LOCATION = $@"{DEFAULT_USER_FOLDER}source\repos\TheEverythingAPI\";
    public static readonly string DEFAULT_RESOURCE_LOCATION = $@"{DEFAULT_PROGRAM_LOCATION}resources\";
    public const string DEFAULT_SEPARATOR = ";";
    public static readonly string FILE_NEW_LINE = Environment.NewLine;
    public const string DEFAULT_SHEET_NAME = "Ark 1";
}
