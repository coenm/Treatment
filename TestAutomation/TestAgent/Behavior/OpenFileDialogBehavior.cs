namespace TestAgent.Behavior
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;

    using Microsoft.Win32;

    public class OpenFileDialogBehavior : Behavior<Button>
    {
        public static readonly DependencyProperty FileName = DependencyProperty.RegisterAttached("FileName", typeof(string), typeof(OpenFileDialogBehavior));

        public static string GetFileName(DependencyObject obj)
        {
            return (string)obj.GetValue(FileName);
        }

        public static void SetFileName(DependencyObject obj, string value)
        {
            obj.SetValue(FileName, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += OnClick;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Click -= OnClick;
            base.OnDetaching();
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            var filename = (string)GetValue(FileName);

            var dialog = new OpenFileDialog
            {
                Title = @"Select file.",
                FileName = filename,
                Filter = "*.exe",
                Multiselect = false,
            };

            if ((bool)dialog.ShowDialog(null))
                SetValue(FileName, dialog.FileName);
        }
    }
}
