using System;
using System.Collections;
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
            var rs = (ResourceSet)ResourceSets[culture];
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
                {
                    rs = new ResourceSet(store);
                    AddResourceSet(ResourceSets, culture, ref rs);
                }
                else
                    rs = base.InternalGetResourceSet(culture, createIfNotExists, tryParents);
            }
            return rs;
        }

        private static void AddResourceSet(Hashtable localResourceSets, CultureInfo culture, ref ResourceSet rs)
        {
            lock (localResourceSets)
            {
                ResourceSet objA = (ResourceSet)localResourceSets[culture];
                if (objA != null)
                {
                    if (!object.Equals(objA, rs))
                    {
                        rs.Dispose();
                        rs = objA;
                    }
                }
                else
                {
                    localResourceSets.Add(culture, rs);
                }
            }
        }
    }
}
