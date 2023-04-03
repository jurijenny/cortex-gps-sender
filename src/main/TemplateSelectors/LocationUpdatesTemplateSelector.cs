using System;
namespace ei8.Cortex.Gps.Sender.TemplateSelectors
{
    public class LocationUpdatesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LocationTemplate { get; set; }
        public DataTemplate StatusTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
            => item is Models.LocationModel ? LocationTemplate : StatusTemplate;
    }
}

