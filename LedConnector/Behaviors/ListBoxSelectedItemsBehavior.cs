using Microsoft.Xaml.Behaviors;
using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace LedConnector.Behaviors
{
    public class ListBoxSelectedItemsBehavior : Behavior<ListBox>
    {
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register(
            "SelectedItems",
            typeof(IList),
            typeof(ListBoxSelectedItemsBehavior),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsChanged)
        );

        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (ListBoxSelectedItemsBehavior)d;
            behavior.OnSelectedItemsChanged((IList)e.OldValue, (IList)e.NewValue);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += OnSelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItems == null)
                return;

            foreach (var item in e.RemovedItems)
                SelectedItems.Remove(item);

            foreach (var item in e.AddedItems)
                SelectedItems.Add(item);
        }

        private void OnSelectedItemsChanged(IList oldValue, IList newValue)
        {
            if (oldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= OnSelectedItemsCollectionChanged;

            if (newValue is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += OnSelectedItemsCollectionChanged;
        }

        private void OnSelectedItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (AssociatedObject == null)
                return;

            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                AssociatedObject.SelectedItems.Clear();
                return;
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    AssociatedObject.SelectedItems.Remove(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    AssociatedObject.SelectedItems.Add(item);
                }
            }
        }
    }
}
