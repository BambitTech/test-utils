namespace Bambit.TestUtility.DatabaseTools.Reqnroll.Configuration;

/// <summary>
/// Handle the current configuration state for the Reqnroll tests
/// </summary>
public class ReqnrollUtilitiesConfiguration
{
    
    private string? NullIndicatorBacker;

    /// <summary>
    /// The default string to use for null values. 
    /// </summary>
    public const string DefaultNullStringIdentifier = "NULL";

    /// <summary>
    /// Creates a new instance, using thee <see cref="DefaultNullStringIdentifier"/> as the <see cref="NullStringIdentifier"/>
    /// </summary>
    public ReqnrollUtilitiesConfiguration() : this(DefaultNullStringIdentifier)
    {
    }

    /// <summary>
    /// Creates a new instance with the supplied string as the <see cref="NullStringIdentifier"/>
    /// </summary>
    /// <param name="nullStringIdentifier"></param>
    public ReqnrollUtilitiesConfiguration(string nullStringIdentifier)
    {
        NullStringIdentifier = nullStringIdentifier;
    }

    /// <summary>
    /// Clones the current settings. 
    /// </summary>
    /// <returns>A copy of the current settings</returns>
    /// <remarks>
    /// During initialization, a clone of the main settings is created for each scenario to allow scenario specific overwriting of values
    /// </remarks>
    public ReqnrollUtilitiesConfiguration Clone()
    {
        return new ReqnrollUtilitiesConfiguration
        {
            NullIndicatorBacker = DefaultNullStringIdentifier,
            TimeoutSeconds = TimeoutSeconds,
            NullStringIdentifier = NullStringIdentifier
        };
    }

    /// <summary>
    /// String used to represent <c>null</c> values
    /// </summary>
    public string NullStringIdentifier
    {
        get => NullIndicatorBacker ?? DefaultNullStringIdentifier;
        set => NullIndicatorBacker = value;
    }

    /// <summary>
    /// The default command timeout
    /// </summary>
    public int? TimeoutSeconds { get; set; }



}