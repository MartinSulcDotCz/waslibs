﻿using AppStudio.DataProviders.Facebook;
using AppStudio.Uwp.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace AppStudio.Uwp.Samples
{
    [SamplePage(Category = "DataProviders", Name = "Facebook")]
    public sealed partial class FacebookPage : SamplePage
    {
        private const string DefaultAppId = "351842111678417";
        private const string DefaultAppSecret = "74b187b46cf37a8ef6349b990bc039c2";
        private const string DefaultFacebookQueryParam = "8195378771";
        private const int DefaultMaxRecordsParam = 20;

        public FacebookPage()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        public override string Caption
        {
            get { return "Facebook Data Provider"; }
        }

        #region DataProvider Config
        public string AppId
        {
            get { return (string)GetValue(AppIdProperty); }
            set { SetValue(AppIdProperty, value); }
        }

        public static readonly DependencyProperty AppIdProperty = DependencyProperty.Register("AppId", typeof(string), typeof(FacebookPage), new PropertyMetadata(DefaultAppId));

        public string AppSecret
        {
            get { return (string)GetValue(AppSecretProperty); }
            set { SetValue(AppSecretProperty, value); }
        }

        public static readonly DependencyProperty AppSecretProperty = DependencyProperty.Register("AppSecret", typeof(string), typeof(FacebookPage), new PropertyMetadata(DefaultAppSecret));


        public string FacebookQueryParam
        {
            get { return (string)GetValue(FacebookQueryParamProperty); }
            set { SetValue(FacebookQueryParamProperty, value); }
        }

        public static readonly DependencyProperty FacebookQueryParamProperty = DependencyProperty.Register("FacebookQueryParam", typeof(string), typeof(FacebookPage), new PropertyMetadata(DefaultFacebookQueryParam));

        public int MaxRecordsParam
        {
            get { return (int)GetValue(MaxRecordsParamProperty); }
            set { SetValue(MaxRecordsParamProperty, value); }
        }

        public static readonly DependencyProperty MaxRecordsParamProperty = DependencyProperty.Register("MaxRecordsParam", typeof(int), typeof(FacebookPage), new PropertyMetadata(DefaultMaxRecordsParam));

        #endregion

        #region Items
        public ObservableCollection<object> Items
        {
            get { return (ObservableCollection<object>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<object>), typeof(FacebookPage), new PropertyMetadata(null));

        #endregion        

        #region RawData
        public string DataProviderRawData
        {
            get { return (string)GetValue(DataProviderRawDataProperty); }
            set { SetValue(DataProviderRawDataProperty, value); }
        }

        public static readonly DependencyProperty DataProviderRawDataProperty = DependencyProperty.Register("DataProviderRawData", typeof(string), typeof(FacebookPage), new PropertyMetadata(string.Empty));

        #endregion    

        #region HasErrors
        public bool HasErrors
        {
            get { return (bool)GetValue(HasErrorsProperty); }
            set { SetValue(HasErrorsProperty, value); }
        }
        public static readonly DependencyProperty HasErrorsProperty = DependencyProperty.Register("HasErrors", typeof(bool), typeof(FacebookPage), new PropertyMetadata(false));
        #endregion

        #region NoItems
        public bool NoItems
        {
            get { return (bool)GetValue(NoItemsProperty); }
            set { SetValue(NoItemsProperty, value); }
        }
        public static readonly DependencyProperty NoItemsProperty = DependencyProperty.Register("NoItems", typeof(bool), typeof(FacebookPage), new PropertyMetadata(false));
        #endregion

        #region IsBusy
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(FacebookPage), new PropertyMetadata(false));

        #endregion

        #region ICommands
        public ICommand RefreshDataCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    Request();
                });
            }
        }

        public ICommand RestoreConfigCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    RestoreConfig();
                });
            }
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.Items = new ObservableCollection<object>();
            RestoreConfig();
            Request();

            base.OnNavigatedTo(e);
        }

        protected override void OnSettings()
        {
            AppShell.Current.Shell.ShowRightPane(new FacebookSettings() { DataContext = this });
        }

        private async void Request()
        {
            try
            {
                IsBusy = true;
                HasErrors = false;
                NoItems = false;
                DataProviderRawData = string.Empty;
                Items.Clear();

                var facebookDataProvider = new FacebookDataProvider(new FacebookOAuthTokens { AppId = AppId, AppSecret = AppSecret });
                var config = new FacebookDataConfig
                {
                    UserId = FacebookQueryParam                    
                };

                var rawParser = new RawParser();
                var rawData = await facebookDataProvider.LoadDataAsync(config, MaxRecordsParam, rawParser);
                DataProviderRawData = rawData.FirstOrDefault()?.Raw;

                var items = await facebookDataProvider.LoadDataAsync(config, MaxRecordsParam);

                NoItems = !items.Any();

                foreach (var item in items)
                {
                    Items.Add(item);
                }

            }
            catch (Exception ex)
            {
                DataProviderRawData += ex.Message;
                DataProviderRawData += ex.StackTrace;
                HasErrors = true;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void RestoreConfig()
        {
            AppId = DefaultAppId;
            AppSecret = DefaultAppSecret;
            FacebookQueryParam = DefaultFacebookQueryParam;            
            MaxRecordsParam = DefaultMaxRecordsParam;
        }
    }
}
