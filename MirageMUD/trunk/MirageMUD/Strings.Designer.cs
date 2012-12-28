﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.296
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Mirage {
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
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Mirage.Strings", typeof(Strings).Assembly);
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
        ///   Looks up a localized string similar to I&apos;m inside ${otherTemplate}..
        /// </summary>
        internal static string IncludedTemplate {
            get {
                return ResourceManager.GetString("IncludedTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ${player} has arrived..
        /// </summary>
        internal static string Movement_Arrival {
            get {
                return ResourceManager.GetString("Movement.Arrival", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Namespace test message.
        /// </summary>
        internal static string namespace_test {
            get {
                return ResourceManager.GetString("namespace.test", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///MMM      MMM
        ///MMMM    MMMM
        ///MMMMM  MMMMM
        ///MMMMMMMMMMMM  III  RRR RRR    AAAAA      GGGGGGGGGG    EEEE
        ///MMM MMMM MMM  III  RRRRRRR   AAA AAA     GGGGGGGGGG  EEE   EE 
        ///MMM  MM  MMM  III  RRRR     AAA   AAA    GGG    GGG  EEEEEEEE
        ///MMM      MMM  III  RRR     AAAAAAAAAAA   GGG    GGG  EE
        ///MMM      MMM  III  RRR     AAA     AAA   GGG    GGG   EE  EEE
        ///MMM      MMM  III  RRR     AAA     AAA   GGGGGGGGGG    EEEE
        ///                                                GGG
        ///                                               [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string negotiation_splash {
            get {
                return ResourceManager.GetString("negotiation.splash", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Welcome to MirageMUD 0.1.  Still in development..
        /// </summary>
        internal static string negotiation_welcome {
            get {
                return ResourceManager.GetString("negotiation.welcome", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A system error has occurred executing your command..
        /// </summary>
        internal static string system_error_SystemError {
            get {
                return ResourceManager.GetString("system.error.SystemError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to This template contains ( @{IncludedTemplate} ) custom = ${custom}.
        /// </summary>
        internal static string TestTemplateInclude {
            get {
                return ResourceManager.GetString("TestTemplateInclude", resourceCulture);
            }
        }
    }
}
