// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    public sealed class StrongNameKeys
    {
        /// <summary>
        /// The strong name key associated with the identity of this assembly. 
        /// This contains the contents of the user-supplied key file exactly as extracted.
        /// </summary>
        public readonly ImmutableArray<byte> KeyPair;

        /// <summary>
        /// Determines source assembly identity.
        /// </summary>
        public readonly ImmutableArray<byte> PublicKey;

        /// <summary>
        /// A diagnostic created in the process of determining the key.
        /// </summary>
        public readonly Diagnostic DiagnosticOpt;

        /// <summary>
        /// The CSP key container containing the public key used to produce the key,
        /// or null if the key was retrieved from <see cref="KeyFilePath"/>.
        /// </summary>
        /// <remarks>
        /// The original value as specified by <see cref="System.Reflection.AssemblyKeyNameAttribute"/> or 
        /// <see cref="CompilationOptions.CryptoKeyContainer"/>.
        /// </remarks>
        public readonly string KeyContainer;

        /// <summary>
        /// Original key file path, or null if the key is provided by the <see cref="KeyContainer"/>.
        /// </summary>
        /// <remarks>
        /// The original value as specified by <see cref="System.Reflection.AssemblyKeyFileAttribute"/> or 
        /// <see cref="CompilationOptions.CryptoKeyFile"/>
        /// </remarks>
        public readonly string KeyFilePath;

        public static readonly StrongNameKeys None = new StrongNameKeys();

        private StrongNameKeys()
        {
        }

        public StrongNameKeys(Diagnostic diagnostic)
        {
            Debug.Assert(diagnostic != null);
            this.DiagnosticOpt = diagnostic;
        }

        public StrongNameKeys(ImmutableArray<byte> keyPair, ImmutableArray<byte> publicKey, string keyContainerName, string keyFilePath)
        {
            Debug.Assert(keyContainerName == null || keyPair.IsDefault);
            Debug.Assert(keyPair.IsDefault || keyFilePath != null);

            this.KeyPair = keyPair;
            this.PublicKey = publicKey;
            this.KeyContainer = keyContainerName;
            this.KeyFilePath = keyFilePath;
        }

        public static StrongNameKeys Create(ImmutableArray<byte> publicKey, CommonMessageProvider messageProvider)
        {
            Debug.Assert(!publicKey.IsDefaultOrEmpty);

            if (MetadataHelpers.IsValidPublicKey(publicKey))
            {
                return new StrongNameKeys(default(ImmutableArray<byte>), publicKey, null, null);
            }
            else
            {
                return new StrongNameKeys(messageProvider.CreateDiagnostic(messageProvider.ERR_BadCompilationOptionValue, Location.None,
                    nameof(CompilationOptions.CryptoPublicKey), BitConverter.ToString(publicKey.ToArray())));
            }
        }

        public static StrongNameKeys Create(StrongNameProvider providerOpt, string keyFilePath, string keyContainerName, CommonMessageProvider messageProvider)
        {
            if (string.IsNullOrEmpty(keyFilePath) && string.IsNullOrEmpty(keyContainerName))
            {
                return None;
            }

            if (providerOpt == null)
            {
                var diagnostic = GetError(keyFilePath, keyContainerName, new CodeAnalysisResourcesLocalizableErrorArgument(nameof(CodeAnalysisResources.AssemblySigningNotSupported)), messageProvider);
                return new StrongNameKeys(diagnostic);
            }

            return providerOpt.CreateKeys(keyFilePath, keyContainerName, messageProvider);
        }

        /// <summary>
        /// True if the compilation can be signed using these keys.
        /// </summary>
        public bool CanSign
        {
            get
            {
                return !KeyPair.IsDefault || KeyContainer != null;
            }
        }

        /// <summary>
        /// True if a strong name can be created for the compilation using these keys.
        /// </summary>
        public bool CanProvideStrongName
        {
            get
            {
                return CanSign || !PublicKey.IsDefault;
            }
        }

        public static Diagnostic GetError(string keyFilePath, string keyContainerName, object message, CommonMessageProvider messageProvider)
        {
            if (keyContainerName != null)
            {
                return GetContainerError(messageProvider, keyContainerName, message);
            }
            else
            {
                return GetKeyFileError(messageProvider, keyFilePath, message);
            }
        }

        public static Diagnostic GetContainerError(CommonMessageProvider messageProvider, string name, object message)
        {
            return messageProvider.CreateDiagnostic(messageProvider.ERR_PublicKeyContainerFailure, Location.None, name, message);
        }

        public static Diagnostic GetKeyFileError(CommonMessageProvider messageProvider, string path, object message)
        {
            return messageProvider.CreateDiagnostic(messageProvider.ERR_PublicKeyFileFailure, Location.None, path, message);
        }

        public static bool IsValidPublicKeyString(string publicKey)
        {
            if (string.IsNullOrEmpty(publicKey) || publicKey.Length % 2 != 0)
            {
                return false;
            }

            foreach (char c in publicKey)
            {
                if (!(c >= '0' && c <= '9') &&
                    !(c >= 'a' && c <= 'f') &&
                    !(c >= 'A' && c <= 'F'))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
