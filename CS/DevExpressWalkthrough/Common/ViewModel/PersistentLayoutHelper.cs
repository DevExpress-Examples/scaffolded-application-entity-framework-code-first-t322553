using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DevExpress.Mvvm;

namespace DevExpressWalkthrough.Common.ViewModel {
    public class PersistentLayoutHelper {
        public static string PersistentLogicalLayout
        {
            get { return LayoutSettings.Default.LogicalLayout; }
            set { LayoutSettings.Default.LogicalLayout = value; }
        }

        static Dictionary<string, string> persistentViewsLayout;
        public static Dictionary<string, string> PersistentViewsLayout
        {
            get
            {
                if(persistentViewsLayout == null) {
                    persistentViewsLayout = LogicalLayoutSerializationHelper.Deserialize(LayoutSettings.Default.ViewsLayout);
                }
                return persistentViewsLayout;
            }
        }

        public static void TryDeserializeLayout(ILayoutSerializationService service, string viewName) {
            string state = null;
            if(service != null && PersistentLayoutHelper.PersistentViewsLayout.TryGetValue(viewName, out state)) {
                service.Deserialize(state);
            }
        }

        public static void TrySerializeLayout(ILayoutSerializationService service, string viewName) {
            if(service != null) {
                PersistentLayoutHelper.PersistentViewsLayout[viewName] = service.Serialize();
            }
        }

        public static void SaveLayout() {
            LayoutSettings.Default.ViewsLayout = LogicalLayoutSerializationHelper.Serialize(PersistentViewsLayout);
            LayoutSettings.Default.Save();
        }

        public static void ResetLayout() {
            PersistentViewsLayout.Clear();
            PersistentLogicalLayout = null;
            SaveLayout();
        }
    }
}