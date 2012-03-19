﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.239
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GreenField.LoginModule.Resources {
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
    public class ErrorMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ErrorMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GreenField.LoginModule.Resources.ErrorMessages", typeof(ErrorMessages).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Invalid Entry. Confirmation password does not match password..
        /// </summary>
        public static string InvalidConfPasswordError {
            get {
                return ResourceManager.GetString("InvalidConfPasswordError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Invalid LoginID / Password combination. Please try again..
        /// </summary>
        public static string InvalidCredentialsError {
            get {
                return ResourceManager.GetString("InvalidCredentialsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Invalid Email. An email must use the format user@company.com.
        /// </summary>
        public static string InvalidEmailError {
            get {
                return ResourceManager.GetString("InvalidEmailError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Invalid Login ID. it either already exists or contains invalid characters. Valid Login ID should contain only alphanumeric characters [a-z, A-Z, 0-9]..
        /// </summary>
        public static string InvalidLoginIDError {
            get {
                return ResourceManager.GetString("InvalidLoginIDError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Invalid Password. Valid password should have atleast 7 and atmost 50 characters. No white spaces allowed..
        /// </summary>
        public static string InvalidPasswordError {
            get {
                return ResourceManager.GetString("InvalidPasswordError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Invalid Login ID. Please try again..
        /// </summary>
        public static string InvalidPasswordResetLoginIDError {
            get {
                return ResourceManager.GetString("InvalidPasswordResetLoginIDError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Invalid Security Question / Answer combination. Please try again..
        /// </summary>
        public static string InvalidPasswordResetSecurityAnswerError {
            get {
                return ResourceManager.GetString("InvalidPasswordResetSecurityAnswerError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Account has been suspended or disabled or locked. Please contact system administrator..
        /// </summary>
        public static string LoginLockedOutError {
            get {
                return ResourceManager.GetString("LoginLockedOutError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Account is not approved or activated by system administrator..
        /// </summary>
        public static string LoginUnapprovedError {
            get {
                return ResourceManager.GetString("LoginUnapprovedError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *Highlighted field(s) is/are missing..
        /// </summary>
        public static string MissingLoginFieldsError {
            get {
                return ResourceManager.GetString("MissingLoginFieldsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *One or more fields are missing. Please complete all required fields to change password..
        /// </summary>
        public static string MissingPasswordChangeFieldsError {
            get {
                return ResourceManager.GetString("MissingPasswordChangeFieldsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *One or more fields are missing. Please complete all required fields to reset password..
        /// </summary>
        public static string MissingPasswordResetFieldsError {
            get {
                return ResourceManager.GetString("MissingPasswordResetFieldsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *One or more fields are missing. Please complete all required fields to create your account..
        /// </summary>
        public static string MissingRegistrationFieldsError {
            get {
                return ResourceManager.GetString("MissingRegistrationFieldsError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to *There was an error updating password. Please try again..
        /// </summary>
        public static string PasswordResetError {
            get {
                return ResourceManager.GetString("PasswordResetError", resourceCulture);
            }
        }
    }
}