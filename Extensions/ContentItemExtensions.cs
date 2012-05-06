using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;
using Orchard.Core.Common.Models;
using Orchard.Core.Title.Models;

namespace ContentSync.Extensions {
    public static class ContentItemExtensions {
        public static bool IsEqualTo(this ContentItem o1, ContentItem o2, IContentManager contentManager) {
            //todo: this is a little too generous

            if(o1.Has<IdentityPart>() && o2.Has<IdentityPart>()) {
                if (!o1.As<IdentityPart>().Identifier.Equals(o2.As<IdentityPart>().Identifier, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }

            if(o1.Has<TitlePart>() && o2.Has<TitlePart>()) {
                if (!o1.As<TitlePart>().Title.Equals(o2.As<TitlePart>().Title, StringComparison.CurrentCulture)) {
                    return false;
                }
            }

            if (o1.Has<BodyPart>() && o2.Has<BodyPart>()) {
                if (!o1.As<BodyPart>().Text.Equals(o2.As<BodyPart>().Text, StringComparison.CurrentCulture)) {
                    return false;
                }
            }

            // compare xml elements
            var export1 = contentManager.Export(o1);
            var export2 = contentManager.Export(o2);

            var attributesToCompare = new string[] {"Title", "Text"};
            var elementsToCompare = new string[]{"BodyPart", "WidgetPart", "TitlePart"};

            foreach(var element in export1.Elements()) {
                if (!elementsToCompare.Contains(element.Name.LocalName))
                    continue;

                var counterpart = export2.Element(element.Name.LocalName);
                if (counterpart == null)
                    return false;
                foreach (var attributeName in attributesToCompare) {
                    if (element.Attribute(attributeName) != null &&
                        counterpart.Attribute(attributeName) != null)
                    {
                        if (!element.Attribute(attributeName).Value.Equals(counterpart.Attribute(attributeName).Value))
                            return false;
                    }
                    
                }
            }

            return true;
        }

        public static bool SimilarTo(this ContentItem ci1, ContentItem ci2) {
            if (ci1.ContentType != ci2.ContentType)
                return false;

            if(ci1.Has<TitlePart>() && ci2.Has<TitlePart>()) {
                if (!ci1.As<TitlePart>().Title.Equals(ci2.As<TitlePart>().Title, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }
            /*if (ci1.Has<BodyPart>() && ci2.Has<BodyPart>())
            {
                if (!ci1.As<BodyPart>().Text.Equals(ci2.As<BodyPart>().Text, StringComparison.InvariantCultureIgnoreCase))
                    return false;
            }*/

            return true;
        }
    }
}