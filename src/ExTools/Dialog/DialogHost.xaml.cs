using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ExTools.Dialog
{
    public partial class DialogHost : UserControl
    {
        public static readonly DependencyProperty IsShowingProperty = DependencyProperty.Register(
            nameof(IsShowing),
            typeof(bool),
            typeof(DialogHost),
            new PropertyMetadata(false, new PropertyChangedCallback(StartFadeInFadeOutAnimation)));

        public bool IsShowing
        {
            get => (bool)GetValue(IsShowingProperty);
            set => SetValue(IsShowingProperty, value);
        }

        public DialogHost() => InitializeComponent();

        private static void StartFadeInFadeOutAnimation(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DialogHost dialogHost = (DialogHost)d;
            bool isShowing = (bool)e.NewValue;

            if (isShowing)
            {
                dialogHost.Visibility = Visibility.Visible;

                DoubleAnimationUsingKeyFrames fadeIn = new();
                fadeIn.KeyFrames.Add(new EasingDoubleKeyFrame(0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(500))));
                fadeIn.KeyFrames.Add(new EasingDoubleKeyFrame(1, KeyTime.FromTimeSpan(TimeSpan.FromSeconds(1))));

                dialogHost.BeginAnimation(fadeIn);
            }
            else
            {
                DoubleAnimation fadeOut = new(0, TimeSpan.FromMilliseconds(200));

                dialogHost.BeginAnimation(fadeOut);
            }
        }

        private void BeginAnimation(DoubleAnimationBase animation)
        {
            Storyboard storyboard = new();
            storyboard.Children.Add(animation);

            SetAnimationCompleted(storyboard);

            Storyboard.SetTarget(animation, this);
            Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.Opacity)"));

            storyboard.Begin();
        }

        private void SetAnimationCompleted(Storyboard storyboard)
        {
            storyboard.Completed += AnimationCompletedHandler;

            void AnimationCompletedHandler(object sender, EventArgs e)
            {
                storyboard.Completed -= AnimationCompletedHandler;

                if (!IsShowing)
                {
                    Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}