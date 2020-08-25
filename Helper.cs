using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Prototype
{
    public class StoryboardHelper : DependencyObject
    {
        public static bool GetBeginIf(DependencyObject obj)
        {
            return (bool)obj.GetValue(BeginIfProperty);
        }

        public static void SetBeginIf(DependencyObject obj, bool value)
        {
            obj.SetValue(BeginIfProperty, value);
        }

        public static readonly DependencyProperty BeginIfProperty = DependencyProperty.RegisterAttached("BeginIf", typeof(bool), typeof(StoryboardHelper), new PropertyMetadata(false, BeginIfPropertyChangedCallback));

        private static void BeginIfPropertyChangedCallback(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            var storyboard = s as Storyboard;
            if (storyboard == null)
                throw new InvalidOperationException("This attached property only supports Storyboards.");

            var begin = (bool)e.NewValue;
            if (begin)
            {
                //if (storyboard.GetCurrentState()==ClockState.Stopped) storyboard.Begin();
            }
            else
            {
                //if (storyboard.GetCurrentState() != ClockState.Stopped) storyboard.Stop();
            }
        }
    }

    class Helper
    {
        public static object FindResource(string key)
        {
            foreach(Window window in App.Current.Windows)
            {
                var res = window.TryFindResource(key);
                if (res != null)
                    return res;
            }
            return null;
        }
    }
}
