﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace legallead.logging.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("legallead.logging.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {
        ///	&quot;target&quot;: &quot;local&quot;,
        ///	&quot;endpoints&quot;:
        ///	{
        ///		&quot;local&quot;: &quot;127.0.0.1&quot;,
        ///		&quot;test&quot;: &quot;db-lead-restored.cmmu8tkizri9.us-east-2.rds.amazonaws.com&quot;,
        ///		&quot;production&quot;: &quot;35.224.202.163&quot;
        ///	},
        ///	&quot;passcodes&quot;:
        ///	{
        ///		&quot;local&quot;: &quot;legal.lead.home.passcode&quot;,
        ///		&quot;test&quot;: &quot;legal.lead.test.passcode&quot;,
        ///		&quot;production&quot;: &quot;legal.lead.last.passcode&quot;
        ///	},
        ///	&quot;secrets&quot;: {
        ///		&quot;local&quot;: &quot;jGJEauBDBO757dZo/eYL4iC1PUMsi8R5i0mBNXZh1YbsRt8aKWC3ELIheVqvHabU&quot;,
        ///		&quot;test&quot;: &quot;m/1Ata5kMzR+oEX0y8RHgOvav+k4LuEIbnSJDbhA2G5pMywLOb3eBufnQzSEN+ef&quot;,
        ///		&quot;productio [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string connectionstring_json {
            get {
                return ResourceManager.GetString("connectionstring-json", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to [
        ///	{ &quot;name&quot;: &quot;error&quot;, &quot;source&quot;: &quot;wlogpermissions&quot; }
        ///].
        /// </summary>
        internal static string datasouce_json {
            get {
                return ResourceManager.GetString("datasouce-json", resourceCulture);
            }
        }
    }
}
