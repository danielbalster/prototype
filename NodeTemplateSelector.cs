using Prototype.Behaviortree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Prototype
{
    public class NodeTemplateSelector : DataTemplateSelector
    {
        //public ResourceDictionary Templates { get; set; } = new ResourceDictionary();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var nvm = item as NodeViewModel;
            if (nvm == null) return null;
            var resource = Application.Current.TryFindResource(nvm.Name);
            if (resource == null)
            {
                if (nvm.AmountChildren==AmountType.None)
                    resource = Application.Current.TryFindResource("Node");
                else if (nvm.AmountChildren == AmountType.One)
                    resource = Application.Current.TryFindResource("Decorator");
                else
                    resource = Application.Current.TryFindResource("Sequence");
            }

            return (DataTemplate) resource;
        }
    }
}
