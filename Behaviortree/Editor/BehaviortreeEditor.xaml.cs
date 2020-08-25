using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;

namespace Prototype.Behaviortree.Editor
{
    public class TemplateSelector : DataTemplateSelector
    {
        TreeView tv;
        public TemplateSelector(TreeView tv)
        {
            this.tv = tv;
        }
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item.GetType().IsSubclassOf(typeof(Decorator)))
            {
                return tv.FindResource("One") as HierarchicalDataTemplate;
            }
            else if (item.GetType().IsSubclassOf(typeof(Composite)))
            {
                return tv.FindResource("Many") as HierarchicalDataTemplate;
            }
            else if (item.GetType().IsSubclassOf(typeof(Node)))
            {
                return tv.FindResource("None") as HierarchicalDataTemplate;
            }
            return null;
        }

    }
    /// <summary>
    /// Interaction logic for BehaviortreeEditor.xaml
    /// </summary>
    public partial class BehaviortreeEditor : Window
    {
        public BehaviortreeEditor()
        {
            InitializeComponent();

            CreateContextMenu();
            //tv.ItemTemplateSelector = new TemplateSelector(tv);
        }

        void CreateContextMenu()
        {
            var cm = new ContextMenu();
            foreach( var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                var na = type.GetCustomAttribute<NodeAttribute>();
                if (na != null)
                {
                    var mi = new MenuItem();
                    mi.Header = "Add " + type.Name;
                    mi.Command = new RelayCommand(param => AddNode(type) );
                    mi.CommandParameter = type;
                    cm.Items.Add(mi);
                }
            }
            tv.ContextMenu = cm;
        }

        void AddNode(Type type)
        {
            var node = tv.SelectedValue as Prototype.Behaviortree.Node;
            if (node == null) return;

            var obj = Activator.CreateInstance(type) as Prototype.Behaviortree.Node;
            
            switch(node.AmountChildren)
            {
                case AmountType.None:
                    break;
                case AmountType.One:
                    node.Clear();
                    node.Add(obj);
                    break;
                case AmountType.Many:
                    node.Add(obj);
                    break;
            }

            tv.Items.Refresh();
        }

        Node root;
        public Node Behavior
        {
            set
            {
                root = value;
                tv.DataContext = value;
                //tv.ItemsSource;
            }
            get
            {
                return root;
            }
        }
    }
}
