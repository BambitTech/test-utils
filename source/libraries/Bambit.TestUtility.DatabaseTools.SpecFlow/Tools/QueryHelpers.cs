using System.Reflection;

namespace Bambit.TestUtility.DatabaseTools.SpecFlow.Tools;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   Provides methods to load and run scripts from files, either on file system or embedded in the dlls. </summary>
///
/// <remarks>   Law Metzler, 7/30/2024. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public static class QueryHelpers
{
    private static readonly Lazy<Dictionary<string, string>> LazyQueryDictionary = new();

    private static Dictionary<string, string> QueryDictionary => LazyQueryDictionary.Value;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Retrieves loaded script by name </summary>
    ///
    /// <remarks>   Law Metzler, 7/30/2024. </remarks>
    ///
    /// <exception cref="KeyNotFoundException"> Thrown when the name has not been loaded previously. </exception>
    ///
    /// <param name="name"> The name to use when referencing the query.  If this name has already
    ///                     been used, it will be overwritten with the new value. </param>
    ///
    /// <returns>   The script that was loaded </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static string GetScript(string name)
    {
        if (QueryDictionary.TryGetValue(name, out string? value))
            return value;
        throw new KeyNotFoundException($"No query named '{name}' has been registered");
    }

    /// <summary>
    /// Registers a query with a name for later referencing.
    /// </summary>
    /// <param name="name">The name to use when referencing the query.  If this name has already been used, it will be overwritten with the new value.</param>
    /// <param name="query">The query to register.</param>
    public static void RegisterNamedScript(string name, string query)
    {
        QueryDictionary[name] = query;
    }
    /// <summary>
    /// Loads a file embedded inside a module for later referencing, storing the contents of the file with the supplied name.
    /// </summary>
    /// <param name="name">The name to use when referencing the query.  If this name has already been used, it will be overwritten with the new value.</param>
    /// <param name="assembly">Reference to the assembly in which the file is embedded.</param>
    /// <param name="filePath">Path of the file (manifest resource stream path) inside the assembly.</param>
    public static void RegisterNamedScriptFromEmbeddedFile( string name, Assembly assembly, string filePath)
    {
        using Stream? initFileStream = assembly.GetManifestResourceStream(filePath) ?? throw new FileNotFoundException(
            $"Could not load script {filePath} from context path. Assembly at Location:{filePath}. ");
        RegisterNamedScript(name, initFileStream);
    }

    /// <summary>
    /// Loads a file containing a query for later referencing, storing the contents of the file with the supplied name.
    /// </summary>
    /// <param name="name">The name to use when referencing the query.  If this name has already been used, it will be overwritten with the new value.</param>
    /// <param name="filePath">Relative location (from the executing dll) of the file.</param>
    public static void RegisterNamedScriptFromFile(string name, string filePath)
    {
        using Stream stream=File.OpenRead(filePath);
        RegisterNamedScript(name, stream);
    }


    private static void RegisterNamedScript(string name, Stream stream)
    {
        TextReader reader = new StreamReader(stream);
        string query = reader.ReadToEnd();
        RegisterNamedScript(name, query);
    }
        
}