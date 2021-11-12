namespace Heavens.Core.Authorizations;

public class JWTSettings
{
    /// <summary>
    /// 
    /// </summary>
    public bool ValidateIssuerSigningKey { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string IssuerSigningKey { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool ValidateIssuer { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ValidIssuer { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool ValidateAudience { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string ValidAudience { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public bool ValidateLifetime { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long ExpiredTime { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public long ClockSkew { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public string Algorithm { get; set; }
}
