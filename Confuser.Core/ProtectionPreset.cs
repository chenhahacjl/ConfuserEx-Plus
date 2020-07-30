namespace Confuser.Core
{
    /// <summary>
    ///     Various presets of protections.
    /// </summary>
    public enum ProtectionPreset
    {
        /// <summary> The protection does not belong to any preset. </summary>
        无保护 = 0,

        /// <summary> The protection provides basic security. </summary>
        基础保护 = 1,

        /// <summary> The protection provides normal security for public release. </summary>
        正常保护 = 2,

        /// <summary> The protection provides better security with observable performance impact. </summary>
        更好保护 = 3,

        /// <summary> The protection provides strongest security with possible incompatibility. </summary>
        最强保护 = 4
    }
}