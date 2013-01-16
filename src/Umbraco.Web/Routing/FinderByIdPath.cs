using System;
using System.Diagnostics;
using System.Xml;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using umbraco.interfaces;
using Umbraco.Core;

namespace Umbraco.Web.Routing
{
	/// <summary>
	/// Provides an implementation of <see cref="IPublishedContentFinder"/> that handles page identifiers.
	/// </summary>
	/// <remarks>
	/// <para>Handles <c>/1234</c> where <c>1234</c> is the identified of a document.</para>
	/// </remarks>
	internal class FinderByIdPath : IPublishedContentFinder
    {
		/// <summary>
		/// Tries to find and assign an Umbraco document to a <c>PublishedContentRequest</c>.
		/// </summary>
		/// <param name="docRequest">The <c>PublishedContentRequest</c>.</param>		
		/// <returns>A value indicating whether an Umbraco document was found and assigned.</returns>
		public bool TryFindDocument(PublishedContentRequest docRequest)
        {
            IPublishedContent node = null;
			var path = docRequest.Uri.GetAbsolutePathDecoded();

            int nodeId = -1;
			if (path != "/") // no id if "/"
            {
				string noSlashPath = path.Substring(1);

                if (!Int32.TryParse(noSlashPath, out nodeId))
                    nodeId = -1;

                if (nodeId > 0)
                {
					LogHelper.Debug<FinderByIdPath>("Id={0}", () => nodeId);
					node = docRequest.RoutingContext.PublishedContentStore.GetDocumentById(
						docRequest.RoutingContext.UmbracoContext,
						nodeId);

                    if (node != null)
                    {
						docRequest.PublishedContent = node;
						LogHelper.Debug<FinderByIdPath>("Found node with id={0}", () => docRequest.DocumentId);
                    }
                    else
                    {
                        nodeId = -1; // trigger message below
                    }
                }
            }

            if (nodeId == -1)
				LogHelper.Debug<FinderByIdPath>("Not a node id");

            return node != null;
        }
    }
}