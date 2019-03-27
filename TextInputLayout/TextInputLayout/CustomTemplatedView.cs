using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.Forms;

namespace TextInputLayout
{
    [ContentProperty("InputView")]
    public class CustomTemplatedView : TemplatedView
    {

        public static readonly BindableProperty InputViewProperty =
            BindableProperty.Create("InputView", typeof(View), typeof(CustomTemplatedView), null, BindingMode.Default, null, OnInputViewChanged);
        public View InputView
        {
            get { return (View)GetValue(InputViewProperty); }
            set { SetValue(InputViewProperty, value); }
        }

        Grid grid;
        public CustomTemplatedView()
        {
            this.ControlTemplate = new ControlTemplate(typeof(StackLayout));
            ((StackLayout)Children[0]).Children.Add(grid = new Grid());

            this.grid.RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = new GridLength(0, GridUnitType.Absolute) },
                new RowDefinition { Height = GridLength.Auto },
            };

            grid.RowSpacing = 0;
            grid.ColumnSpacing = 0;
        }


        private static void OnInputViewChanged(BindableObject bindable, object oldValue, object newValue)
        {
            (bindable as CustomTemplatedView).OnInputViewChanged(oldValue, newValue);
        }

        private void OnInputViewChanged(object oldValue, object newValue)
        {
            var oldView = (View)oldValue;
            if (oldView != null)
            {
                if (this.grid.Children.Contains(oldView))
                {
                    oldView.BindingContext = null;
                    this.grid.Children.Remove(oldView);
                }
            }

            var newView = (View)newValue;
            if (newView != null)
            {
                this.grid.Children.Add(newView, 0, 1);
            }
        }

        protected override void OnBindingContextChanged()
        {
            if (grid != null)
            {
                SetInheritedBindingContext(this.grid, this.BindingContext);
            }
            base.OnBindingContextChanged();

        }
    }
}
