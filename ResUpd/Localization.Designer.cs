﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18047
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ResUpd {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Localization {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Localization() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ResUpd.Localization", typeof(Localization).Assembly);
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
        ///   Looks up a localized string similar to Output successfully saved
        ///{0}.
        /// </summary>
        internal static string MainSuccess {
            get {
                return ResourceManager.GetString("MainSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Validating arguments....
        /// </summary>
        internal static string MainValidateArgs {
            get {
                return ResourceManager.GetString("MainValidateArgs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Writing output....
        /// </summary>
        internal static string MainWriteOutput {
            get {
                return ResourceManager.GetString("MainWriteOutput", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Processes Win32 resource within PE file.
        ///
        ///RESUPD /E src out name type
        ///RESUPD /I src out name type res
        ///RESUPD /D src out name type
        ///	
        ///  /E    extract resource
        ///  /I    inject resource
        ///  /D    delete resource
        ///  
        ///  src   source file path
        ///  out   output file path
        ///  type  numeric resource type
        ///  name  numeric resource name
        ///  res   resource file path.
        /// </summary>
        internal static string Usage {
            get {
                return ResourceManager.GetString("Usage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Output file already exists
        ///{0}.
        /// </summary>
        internal static string ValidateArgsOutputFile {
            get {
                return ResourceManager.GetString("ValidateArgsOutputFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resource file does not exist
        ///{0}.
        /// </summary>
        internal static string ValidateArgsResourceFile {
            get {
                return ResourceManager.GetString("ValidateArgsResourceFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Source file does not exist
        ///{0}.
        /// </summary>
        internal static string ValidateArgsSourceFile {
            get {
                return ResourceManager.GetString("ValidateArgsSourceFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to begin update resource
        ///{0}
        ///{1}.
        /// </summary>
        internal static string WriteOutputBeginUpdateResource {
            get {
                return ResourceManager.GetString("WriteOutputBeginUpdateResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to complete update resource
        ///{0}
        ///{1}.
        /// </summary>
        internal static string WriteOutputCompleteUpdateResource {
            get {
                return ResourceManager.GetString("WriteOutputCompleteUpdateResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to delete resource
        ///{0}
        ///{1}.
        /// </summary>
        internal static string WriteOutputDeleteResource {
            get {
                return ResourceManager.GetString("WriteOutputDeleteResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to copy source file to output path
        ///{0}
        ///{1}
        ///{2}.
        /// </summary>
        internal static string WriteOutputFileCopy {
            get {
                return ResourceManager.GetString("WriteOutputFileCopy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to read resource file
        ///{0}
        ///{1}.
        /// </summary>
        internal static string WriteOutputFileRead {
            get {
                return ResourceManager.GetString("WriteOutputFileRead", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to load datafile
        ///{0}
        ///{1}.
        /// </summary>
        internal static string WriteOutputLoadDatafile {
            get {
                return ResourceManager.GetString("WriteOutputLoadDatafile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to load resource data
        ///{0}.
        /// </summary>
        internal static string WriteOutputLoadResourceBytes {
            get {
                return ResourceManager.GetString("WriteOutputLoadResourceBytes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to update resource
        ///{0}
        ///{1}.
        /// </summary>
        internal static string WriteOutputUpdateResource {
            get {
                return ResourceManager.GetString("WriteOutputUpdateResource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to write output
        ///{0}
        ///{1}.
        /// </summary>
        internal static string WriteOutputWriteResource {
            get {
                return ResourceManager.GetString("WriteOutputWriteResource", resourceCulture);
            }
        }
    }
}
