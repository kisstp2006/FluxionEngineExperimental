using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Threading.Tasks;

namespace FluxionEditor.Foundation.Utilities
{
    /// <summary>
    /// Small helper for animating Avalonia controls from code-behind.
    /// Usage:
    ///   await Animator.SlideToAsync(panel, -800);
    ///   await Animator.FadeToAsync(view, 0.0);
    /// </summary>
    public static class Animator
    {
        public static readonly TimeSpan DefaultDuration = TimeSpan.FromMilliseconds(250);

        /// <summary>
        /// Slides a control horizontally: animates its render-transform X
        /// from the current offset to <paramref name="toX"/>.
        /// </summary>
        public static async Task SlideToAsync(Visual target, double toX,
            TimeSpan? duration = null, Easing? easing = null, TimeSpan? delay = null)
        {
            // Reuse the existing transform so consecutive slides continue
            // from wherever the control currently is.
            if (target.RenderTransform is not TranslateTransform transform)
            {
                transform = new TranslateTransform();
                target.RenderTransform = transform;
            }

            var animation = new Animation
            {
                Duration = duration ?? DefaultDuration,
                Delay = delay ?? TimeSpan.Zero,
                Easing = easing ?? new CubicEaseOut(),
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0d),
                        Setters = { new Setter(TranslateTransform.XProperty, transform.X) }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1d),
                        Setters = { new Setter(TranslateTransform.XProperty, toX) }
                    }
                }
            };

            // Transform properties must be animated through the visual itself:
            // Avalonia's TransformAnimator resolves the RenderTransform from it.
            await animation.RunAsync(target);

            // Pin the final value as a local value so later reads/writes see it.
            transform.X = toX;
        }

        /// <summary>
        /// Fades a control's opacity from its current value to <paramref name="toOpacity"/>.
        /// </summary>
        public static async Task FadeToAsync(Visual target, double toOpacity,
            TimeSpan? duration = null, Easing? easing = null, TimeSpan? delay = null)
        {
            var animation = new Animation
            {
                Duration = duration ?? DefaultDuration,
                Delay = delay ?? TimeSpan.Zero,
                Easing = easing ?? new CubicEaseOut(),
                FillMode = FillMode.Forward,
                Children =
                {
                    new KeyFrame
                    {
                        Cue = new Cue(0d),
                        Setters = { new Setter(Visual.OpacityProperty, target.Opacity) }
                    },
                    new KeyFrame
                    {
                        Cue = new Cue(1d),
                        Setters = { new Setter(Visual.OpacityProperty, toOpacity) }
                    }
                }
            };

            await animation.RunAsync(target);
            target.Opacity = toOpacity;
        }
    }
}
