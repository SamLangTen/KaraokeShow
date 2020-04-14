using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;

namespace MusicBeePlugin.Internationalization
{
    public class EmbedResourceManager : ComponentResourceManager
    {

        private Type _contextTypeInfo;
        private CultureInfo _neutralResourcesCulture;

        public EmbedResourceManager(Type t) : base(t)
        {
            _contextTypeInfo = t;
        }

        protected override ResourceSet InternalGetResourceSet(CultureInfo culture, bool createIfNotExists, bool tryParents)
        {
            var rs = base.InternalGetResourceSet(culture, createIfNotExists, tryParents);
            if (rs == null)
            {
                Stream store = null;
                string resourceFilename = null;
                if (_neutralResourcesCulture == null)
                    _neutralResourcesCulture = GetNeutralResourcesLanguage(MainAssembly);
                if (_neutralResourcesCulture.Equals(culture))
                    culture = CultureInfo.InvariantCulture;
                resourceFilename = GetResourceFileName(culture);
                store = MainAssembly.GetManifestResourceStream(_contextTypeInfo, resourceFilename);
                if (store != null)
                    rs = new ResourceSet(store);
                else
                    rs = base.InternalGetResourceSet(culture, createIfNotExists, tryParents);
            }
            return rs;
        }

    }
}
