using GooeyWpf.Controls;
using GooeyWpf.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace GooeyWpf
{
    internal class AvatarController : IDisposable
    {
        private readonly Avatar avatarControl;
        private readonly Assistant assistant;

        private readonly Storyboard blink1 = new();
        private readonly Storyboard blink2 = new();

        private readonly Timer blinkTimer;
        private readonly Timer expressionTimer;

        private Expression currentExpression;

        public AvatarController(Avatar avatarControl, Assistant assistant)
        {
            this.avatarControl = avatarControl;
            this.assistant = assistant;

            ObjectAnimationUsingKeyFrames objectAnimation1 = new()
            {
                Duration = TimeSpan.FromSeconds(0.2)
            };
            objectAnimation1.KeyFrames.Add(new DiscreteObjectKeyFrame(new BitmapImage(Common.Resource("/Images/Avatar/EyeBlink.png"))) { KeyTime = KeyTime.Uniform });
            objectAnimation1.KeyFrames.Add(new DiscreteObjectKeyFrame(new BitmapImage(Common.Resource("/Images/Avatar/EyeOpen.png"))) { KeyTime = KeyTime.Uniform });
            Storyboard.SetTargetProperty(objectAnimation1, new PropertyPath(Avatar.EyeImageProperty));
            blink1.Children.Add(objectAnimation1);

            ObjectAnimationUsingKeyFrames objectAnimation2 = new()
            {
                Duration = TimeSpan.FromSeconds(0.3)
            };
            objectAnimation2.KeyFrames.Add(new DiscreteObjectKeyFrame(new BitmapImage(Common.Resource("/Images/Avatar/EyeBlink.png"))) { KeyTime = KeyTime.Uniform });
            objectAnimation2.KeyFrames.Add(new DiscreteObjectKeyFrame(new BitmapImage(Common.Resource("/Images/Avatar/EyeOpen.png"))) { KeyTime = KeyTime.Uniform });
            objectAnimation2.KeyFrames.Add(new DiscreteObjectKeyFrame(new BitmapImage(Common.Resource("/Images/Avatar/EyeBlink.png"))) { KeyTime = KeyTime.Uniform });
            objectAnimation2.KeyFrames.Add(new DiscreteObjectKeyFrame(new BitmapImage(Common.Resource("/Images/Avatar/EyeOpen.png"))) { KeyTime = KeyTime.Uniform });
            Storyboard.SetTargetProperty(objectAnimation2, new PropertyPath(Avatar.EyeImageProperty));
            blink2.Children.Add(objectAnimation2);

            //RotateTransform rotateTransform = new RotateTransform();
            //avatarControl.RegisterName("Avatar_RotateTransform", avatarControl);
            //DoubleAnimationUsingKeyFrames doubleAnimation = new()
            //{
            //    Duration = TimeSpan.FromSeconds(1)
            //};
            //doubleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0));
            //doubleAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(90));
            //Storyboard.SetTargetName(doubleAnimation, "Avatar_RotateTransform");
            //Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(RotateTransform.AngleProperty));
            //avatarControl.RenderTransform = rotateTransform;
            //barrelRoll.Children.Add(doubleAnimation);



            blinkTimer = new(BlinkTimerCallback, avatarControl, RandomBlinkInterval(), Timeout.Infinite);

            expressionTimer = new(ExpressionTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
            currentExpression = Expression.Normal;

            assistant.LipSync += LipSync;
        }

        public void BarrelRoll()
        {
            avatarControl.Dispatcher.Invoke(() =>
            {
                RotateTransform rotateTransform = new();
                DoubleAnimation barrelRoll = new();
                barrelRoll.From = 0;
                barrelRoll.To = 360;
                barrelRoll.Duration = new Duration(TimeSpan.FromSeconds(1));
                barrelRoll.EasingFunction = new CubicEase();
                avatarControl.RenderTransform = rotateTransform;
                rotateTransform.BeginAnimation(RotateTransform.AngleProperty, barrelRoll);
            });
        }

        public void ChangeExpression(Expression expression)
        {
            currentExpression = expression;
            avatarControl.Dispatcher.Invoke(() =>
            {
                if (expression == Expression.Hal)
                {
                    avatarControl.EyeImageVisibility = Visibility.Hidden;
                    return;
                }

                EventHandler? changeAction = null;
                changeAction = ((s, e) =>
                {
                    avatarControl.FaceImage = new BitmapImage(Common.Resource($"/Images/Avatar/{GetExpressionName(expression)}Closed.png"));
                    if (avatarControl.EyeImageVisibility == Visibility.Hidden)
                        avatarControl.EyeImageVisibility = Visibility.Visible;
                    blink1.Completed -= changeAction;
                });
                blink1.Completed += changeAction;
                avatarControl.BeginStoryboard(blink1);
            });
            expressionTimer.Change(5000, Timeout.Infinite);
        }

        private void LipSync(bool mouthOpen)
        {
            avatarControl.Dispatcher.Invoke(() =>
            {
                string resourceString = $"/Images/Avatar/{GetExpressionName(currentExpression)}";
                if (mouthOpen)
                {
                    resourceString += "Open";
                }
                else
                {
                    resourceString += "Closed";
                }
                resourceString += ".png";
                avatarControl.FaceImage = new BitmapImage(Common.Resource(resourceString));
            });
        }

        private void BlinkTimerCallback(object? state)
        {
            int blinkType = RandomService.Instance.Next(1, 3);

            if (state is not Avatar avatar) return;

            avatar.Dispatcher.Invoke(() =>
            {
                avatar.BeginStoryboard(blinkType == 1 ? blink1 : blink2);
            });

            int randomInterval = RandomBlinkInterval();
            blinkTimer.Change(randomInterval, Timeout.Infinite);
        }

        private void ExpressionTimerCallback(object? state)
        {
            ResetExpression();
            expressionTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private void ResetExpression()
        {
            ChangeExpression(Expression.Normal);
            blinkTimer.Change(0, Timeout.Infinite);
        }

        private static int RandomBlinkInterval() => RandomService.Instance.Next(3000, 6000);

        private static string GetExpressionName(Expression expression) => Enum.GetName(typeof(Expression), expression) ?? "Normal";

        public void Dispose()
        {
            blinkTimer.Dispose();
        }
    }
}
