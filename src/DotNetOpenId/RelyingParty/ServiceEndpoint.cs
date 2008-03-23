using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using DotNetOpenId.Yadis;

namespace DotNetOpenId.RelyingParty {
	/// <summary>
	/// Represents information discovered about a user-supplied Identifier.
	/// </summary>
	internal class ServiceEndpoint {
		/// <summary>
		/// The URL which accepts OpenID Authentication protocol messages.
		/// </summary>
		/// <remarks>
		/// Obtained by performing discovery on the User-Supplied Identifier. 
		/// This value MUST be an absolute HTTP or HTTPS URL.
		/// </remarks>
		public Uri ProviderEndpoint { get; private set; }
		/*
		/// <summary>
		/// An Identifier for an OpenID Provider.
		/// </summary>
		public Identifier ProviderIdentifier { get; private set; }
		/// <summary>
		/// An Identifier that was presented by the end user to the Relying Party, 
		/// or selected by the user at the OpenID Provider. 
		/// During the initiation phase of the protocol, an end user may enter 
		/// either their own Identifier or an OP Identifier. If an OP Identifier 
		/// is used, the OP may then assist the end user in selecting an Identifier 
		/// to share with the Relying Party.
		/// </summary>
		public Identifier UserSuppliedIdentifier { get; private set; }*/
		/// <summary>
		/// The Identifier that the end user claims to own.
		/// </summary>
		public Identifier ClaimedIdentifier { get; private set; }
		/// <summary>
		/// An alternate Identifier for an end user that is local to a 
		/// particular OP and thus not necessarily under the end user's 
		/// control.
		/// </summary>
		public Identifier ProviderLocalIdentifier { get; private set; }
		/// <summary>
		/// Gets the list of services available at this OP Endpoint for the
		/// claimed Identifier.
		/// </summary>
		public string[] ProviderSupportedServiceTypeUris { get; private set; }

		internal ServiceEndpoint(Identifier claimedIdentifier, Uri providerEndpoint, 
			Identifier providerLocalIdentifier, string[] providerSupportedServiceTypeUris) {
			if (claimedIdentifier == null) throw new ArgumentNullException("claimedIdentifier");
			if (providerEndpoint == null) throw new ArgumentNullException("providerEndpoint");
			if (providerSupportedServiceTypeUris == null) throw new ArgumentNullException("providerSupportedServiceTypeUris");
			ClaimedIdentifier = claimedIdentifier;
			ProviderEndpoint = providerEndpoint;
			ProviderLocalIdentifier = providerLocalIdentifier ?? claimedIdentifier;
			ProviderSupportedServiceTypeUris = providerSupportedServiceTypeUris;
		}

		public Version ProviderVersion {
			get {
				Protocol protocol =
					Util.FindBestVersion(p => p.OPIdentifierServiceTypeURI, ProviderSupportedServiceTypeUris) ??
					Util.FindBestVersion(p => p.ClaimedIdentifierServiceTypeURI, ProviderSupportedServiceTypeUris);
				if (protocol != null) return protocol.Version;
				throw new InvalidOperationException("Unable to determine Provider's version.");
			}
		}

		public bool UsesExtension(string extensionUri) {
			return Array.IndexOf(ProviderSupportedServiceTypeUris, extensionUri) >= 0;
		}
	}
}