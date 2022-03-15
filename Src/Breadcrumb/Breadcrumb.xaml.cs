﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using Xamarin.Forms.Xaml;

namespace Breadcrumb
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Breadcrumb : ContentView
    {
        #region Control properties

        // Separator
        public static readonly BindableProperty SeparatorProperty = BindableProperty.Create(nameof(Separator), typeof(ImageSource), typeof(Breadcrumb), new FontImageSource { Glyph = " / ", Color = Color.Black, Size = 15, });

        public ImageSource Separator
        {
            get => (ImageSource)GetValue(SeparatorProperty);
            set => SetValue(SeparatorProperty, value);
        }

        // FirstBreadCrumb
        public static readonly BindableProperty FirstBreadCrumbProperty = BindableProperty.Create(nameof(FirstBreadCrumb), typeof(ImageSource), typeof(Breadcrumb), null);

        public ImageSource FirstBreadCrumb
        {
            get => (ImageSource)GetValue(FirstBreadCrumbProperty);
            set => SetValue(FirstBreadCrumbProperty, value);
        }

        // Scrollbar Visibility
        public static readonly BindableProperty ScrollBarVisibilityProperty = BindableProperty.Create(nameof(ScrollBarVisibility), typeof(ScrollBarVisibility), typeof(Breadcrumb), ScrollBarVisibility.Never);

        public ScrollBarVisibility ScrollBarVisibility
        {
            get => (ScrollBarVisibility)GetValue(ScrollBarVisibilityProperty);
            set => SetValue(ScrollBarVisibilityProperty, value);
        }

        // FontSize
        public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize), typeof(double), typeof(Breadcrumb), 15d);
        [TypeConverter(typeof(FontSizeConverter))]
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }


        // Text Color
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(Breadcrumb), Color.Black);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }

        // Corner radius
        public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(Breadcrumb), new CornerRadius(10));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        // Breadcrumb margin
        public static readonly BindableProperty BreadcrumbMarginProperty = BindableProperty.Create(nameof(BreadcrumbMargin), typeof(Thickness), typeof(Breadcrumb), new Thickness(0));

        public Thickness BreadcrumbMargin
        {
            get => (Thickness)GetValue(BreadcrumbMarginProperty);
            set => SetValue(BreadcrumbMarginProperty, value);
        }

        // BreadcrumbBackgroundColor
        public static readonly BindableProperty BreadcrumbBackgroundColorProperty = BindableProperty.Create(nameof(BreadcrumbBackgroundColor), typeof(Color), typeof(Breadcrumb), Color.Transparent);

        public Color BreadcrumbBackgroundColor
        {
            get => (Color)GetValue(BreadcrumbBackgroundColorProperty);
            set => SetValue(BreadcrumbBackgroundColorProperty, value);
        }

        // LastBreadcrumbTextColor
        public static readonly BindableProperty LastBreadcrumbTextColorProperty = BindableProperty.Create(nameof(LastBreadcrumbTextColor), typeof(Color), typeof(Breadcrumb), Color.Black);

        public Color LastBreadcrumbTextColor
        {
            get => (Color)GetValue(LastBreadcrumbTextColorProperty);
            set => SetValue(LastBreadcrumbTextColorProperty, value);
        }

        // LastBreadcrumbCornerRadius
        public static readonly BindableProperty LastBreadcrumbCornerRadiusProperty = BindableProperty.Create(nameof(LastBreadcrumbCornerRadius), typeof(CornerRadius), typeof(Breadcrumb), new CornerRadius(10));

        public CornerRadius LastBreadcrumbCornerRadius
        {
            get => (CornerRadius)GetValue(LastBreadcrumbCornerRadiusProperty);
            set => SetValue(LastBreadcrumbCornerRadiusProperty, value);
        }

        // LastBreadcrumbBackgroundColor
        public static readonly BindableProperty LastBreadcrumbBackgroundColorProperty = BindableProperty.Create(nameof(LastBreadcrumbBackgroundColor), typeof(Color), typeof(Breadcrumb), Color.Transparent);

        public Color LastBreadcrumbBackgroundColor
        {
            get => (Color)GetValue(LastBreadcrumbBackgroundColorProperty);
            set => SetValue(LastBreadcrumbBackgroundColorProperty, value);
        }

        // AnimationSpeed
        public static readonly BindableProperty AnimationSpeedProperty = BindableProperty.Create(nameof(AnimationSpeed), typeof(uint), typeof(Breadcrumb), (uint)800);

        public uint AnimationSpeed
        {
            get => (uint)GetValue(AnimationSpeedProperty);
            set => SetValue(AnimationSpeedProperty, value);
        }

        // IsNavigationEnabled
        public static readonly BindableProperty IsNavigationEnabledProperty = BindableProperty.Create(nameof(IsNavigationEnabled), typeof(bool), typeof(Breadcrumb), true);

        public bool IsNavigationEnabled
        {
            get => (bool)GetValue(IsNavigationEnabledProperty);
            set => SetValue(IsNavigationEnabledProperty, value);
        }

        #endregion Control properties

        public Breadcrumb()
        {
            InitializeComponent();
            Reload();
        }

        /// <summary>
        /// Reload the control if it currently isn't visible.
        /// </summary>
        public void Reload()
        {
            if (IsVisible == true)
                return;

            // Event fired the moment ContentView is displayed
            Device.BeginInvokeOnMainThread(async () =>
            {
                // Get list of all pages in the NavigationStack that has a selectedPage title
                List<Page> pages = Navigation.NavigationStack.Select(x => x).Where(x => !string.IsNullOrEmpty(x?.Title)).ToList();

                // If any pages, make the control visible
                IsVisible = pages.Count > 0;

                // Loop all pages
                foreach (Page page in pages)
                {
                    if (!page.Equals(pages.LastOrDefault()))
                    {
                        // Create breadcrumb
                        Frame breadCrumb1 = BreadCrumbLabelCreator(page, false, page.Equals(pages.FirstOrDefault()));

                        // Add tap gesture
                        if (IsNavigationEnabled)
                        {
                            TapGestureRecognizer tapGesture = new()
                            {
                                CommandParameter = page,
                                Command = new Command<Page>(async item => await GoBack(item).ConfigureAwait(false))
                            };
                            breadCrumb1.GestureRecognizers.Add(tapGesture);

                        }

                        // Add breadcrumb and separator to BreadCrumbContainer
                        BreadCrumbContainer.Children.Add(breadCrumb1);

                        // Add separator
                        Image separator = new()
                        {
                            Source = Separator,
                            VerticalOptions = LayoutOptions.Center
                        };
                        AutomationProperties.SetIsInAccessibleTree(separator, false);
                        BreadCrumbContainer.Children.Add(separator);

                        continue;
                    }

                    // Add ChildAdded event to trigger animation
                    BreadCrumbContainer.ChildAdded += AnimatedStack_ChildAdded;

                    // Create selectedPage title label
                    Frame breadCrumb2 = BreadCrumbLabelCreator(page, true, page.Equals(pages.FirstOrDefault()));

                    // Move BreadCrumb of selectedPage to start the animation
                    breadCrumb2.TranslationX = Application.Current.MainPage.Width;

                    // Scroll to end of control
                    await Task.Delay(10);
                    await BreadCrumbsScrollView.ScrollToAsync(BreadCrumbContainer, ScrollToPosition.End, false);

                    // Add breadcrumb to container
                    BreadCrumbContainer.Children.Add(breadCrumb2);

                    // Scroll to last breadcrumb
                    await BreadCrumbsScrollView.ScrollToAsync(BreadCrumbContainer, ScrollToPosition.End, AnimationSpeed != 0);
                }
            });
        }

        /// <summary>
        /// Creates a new Breadcrumb object
        /// </summary>
        /// <param name="page"></param>
        /// <param name="isLast"></param>
        /// <param name="isFirst"></param>
        private Frame BreadCrumbLabelCreator(Page page, bool isLast, bool isFirst)
        {
            // Create StackLayout to contain the label within a PancakeView
            StackLayout stackLayout = new()
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            AutomationProperties.SetIsInAccessibleTree(stackLayout, false);

            // Create and Add label to StackLayout
            if (isFirst && FirstBreadCrumb != null)
            {
                stackLayout.Children.Add(new Image
                {
                    Source = FirstBreadCrumb,
                    VerticalOptions = LayoutOptions.Center
                });
            }
            else
            {
                Label breadcrumbText = new()
                {
                    Text = page.Title,
                    FontSize = FontSize,
                    VerticalOptions = LayoutOptions.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };
                breadcrumbText.SetBinding(Label.TextColorProperty, new Binding(isLast ? nameof(LastBreadcrumbTextColor) : nameof(TextColor), source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestor, typeof(Breadcrumb))));
                AutomationProperties.SetIsInAccessibleTree(breadcrumbText, false);

                stackLayout.Children.Add(breadcrumbText);
            }

            PancakeView container = new PancakeView
            {
                Padding = 10,
                VerticalOptions = LayoutOptions.Center,
                CornerRadius = isLast ? LastBreadcrumbCornerRadius : CornerRadius,
                Content = stackLayout,
                Margin = BreadcrumbMargin
            };
            container.SetBinding(BackgroundColorProperty, new Binding(isLast ? nameof(LastBreadcrumbBackgroundColor) : nameof(BreadcrumbBackgroundColor), source: new RelativeBindingSource(RelativeBindingSourceMode.FindAncestor, typeof(Breadcrumb))));


            Frame accessibilityContainer = !isLast && IsNavigationEnabled ? new BreadcrumbButton() : new Frame();
            accessibilityContainer.HasShadow = false;
            accessibilityContainer.BackgroundColor = Color.Transparent;
            accessibilityContainer.Padding = 0;
            accessibilityContainer.Content = container;

            AutomationProperties.SetIsInAccessibleTree(accessibilityContainer, true);
            AutomationProperties.SetName(accessibilityContainer, page.Title);

            return accessibilityContainer;
        }

        /// <summary>
        /// Animates item added to stack
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnimatedStack_ChildAdded(object sender, ElementEventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                // iOS scroll to end fix
                if (Device.RuntimePlatform.Equals(Device.iOS))
                {
                    await BreadCrumbsScrollView.ScrollToAsync(BreadCrumbContainer.Children.LastOrDefault(), ScrollToPosition.MakeVisible, false);
                }

                Animation lastBreadcrumbAnimation = new()
                {
                    { 0, 1, new Animation(_ => BreadCrumbContainer.Children.Last().TranslationX = _, Application.Current.MainPage.Width, 0, Easing.Linear) }
                };

                if (Device.RuntimePlatform.Equals(Device.iOS))
                {
                    Point point = BreadCrumbsScrollView.GetScrollPositionForElement(BreadCrumbContainer.Children.Last(), ScrollToPosition.End);
                    lastBreadcrumbAnimation.Add(0, 1, new Animation(_ => BreadCrumbsScrollView.ScrollToAsync(BreadCrumbContainer.Children.LastOrDefault(), ScrollToPosition.MakeVisible, true), BreadCrumbsScrollView.X, point.X - 6));
                }

                lastBreadcrumbAnimation.Commit(this, nameof(lastBreadcrumbAnimation), 16, AnimationSpeed);
            });
        }

        /// <summary>
        /// Navigates the user back to chosen selectedPage in the Navigation stack
        /// </summary>
        /// <param name="selectedPage"></param>
        private async Task GoBack(Page selectedPage)
        {
            // Check if selectedPage is still in Navigation Stack
            if (Navigation.NavigationStack.All(x => x != selectedPage))
            {
                // PopToRoot triggered if selectedPage is missing from navigation stack
                await Navigation.PopToRootAsync();
                return;
            }

            // Get all pages after and including selectedPage
            List<Page> pages = Navigation.NavigationStack.SkipWhile(x => x != selectedPage).ToList();

            // Remove selectedPage
            pages.Remove(selectedPage);

            // Remove current page (this will be removed with a PopAsync after all other relevant pages are removed)
            pages = pages.Take(pages.Count - 1).ToList();

            // Remove all pages left in list (i.e. all pages after selectedPage, minus the current page)
            foreach (Page page in pages)
            {
                Navigation.RemovePage(page);
            }

            // Remove current page
            await Navigation.PopAsync();
        }
    }
}