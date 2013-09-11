﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Diagnostics;
using sccmclictr.automation;
using sccmclictr.automation.functions;
using System.IO;
using System.Globalization;

namespace ClientCenter.Controls
{
    /// <summary>
    /// Interaction logic for ApplicationGrid.xaml
    /// </summary>
    public partial class ApplicationGrid : UserControl
    {
        private SCCMAgent oAgent;
        public MyTraceListener Listener;
        internal List<sccmclictr.automation.functions.softwaredistribution.CCM_Application> iApplications;

        public ApplicationGrid()
        {
            InitializeComponent();
        }

        public SCCMAgent SCCMAgentConnection
        {
            get
            {
                return oAgent;
            }
            set
            {
                if (value.isConnected)
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    try
                    {
                        //SCCMAgent oAgent = new SCCMAgent("localhost");
                        //List<softwaredistribution.CCM_Application> lApps = oAgent.Client.SoftwareDistribution.Applications.OrderBy(t => t.FullName).ToList();
                        //lApps.ToString();

                        oAgent = value;
                        iApplications = oAgent.Client.SoftwareDistribution.Applications.GroupBy(t => t.Id).Select(grp => grp.FirstOrDefault()).OrderBy(o => o.FullName).ToList();
                        //TEST.Source = BitMapConvert.ToBitmapImage(iApplications[0].IconAsImage);

                        dataGrid1.BeginInit();
                        dataGrid1.ItemsSource = iApplications;
                        dataGrid1.EndInit();
                    }
                    catch(Exception ex)
                    {
                        ex.Message.ToString();
                    }
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        }



        private void bt_Reload_Click(object sender, RoutedEventArgs e)
        {
            iApplications = oAgent.Client.SoftwareDistribution.Applications.GroupBy(t => t.Id).Select(grp => grp.FirstOrDefault()).OrderBy(o=>o.FullName).ToList();

            dataGrid1.BeginInit();
            dataGrid1.ItemsSource = iApplications;
            dataGrid1.EndInit();
        }

        private void miInstallApp_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Install all selected Updates;
                List<softwaredistribution.CCM_Application> lApps = new List<softwaredistribution.CCM_Application>();
                foreach (softwaredistribution.CCM_Application cApp in dataGrid1.SelectedItems)
                {
                    lApps.Add(cApp);
                    cApp.Install(softwaredistribution.AppPriority.Normal, false);
                }
                //oAgent.Client.SoftwareUpdates.InstallUpdates(lUpdates);
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miUnInstallApp_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Install all selected Updates;
                List<softwaredistribution.CCM_Application> lApps = new List<softwaredistribution.CCM_Application>();
                foreach (softwaredistribution.CCM_Application cApp in dataGrid1.SelectedItems)
                {
                    lApps.Add(cApp);
                    cApp.Uninstall(softwaredistribution.AppPriority.Normal, false);
                }
                //oAgent.Client.SoftwareUpdates.InstallUpdates(lUpdates);
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        //Does not work currently; Cancel command must be triggered from local System account and even then the App does not stop.
        private void miCancelApp_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Cancel all selected app;
                List<softwaredistribution.CCM_Application> lApps = new List<softwaredistribution.CCM_Application>();
                foreach (softwaredistribution.CCM_Application cApp in dataGrid1.SelectedItems)
                {
                    lApps.Add(cApp);
                    cApp.Cancel();
                }
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void miDownloadContent_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                //Cancel all selected app;
                List<softwaredistribution.CCM_Application> lApps = new List<softwaredistribution.CCM_Application>();
                foreach (softwaredistribution.CCM_Application cApp in dataGrid1.SelectedItems)
                {
                    lApps.Add(cApp);
                    cApp.DownloadContents();
                }
            }
            catch (Exception ex)
            {
                Listener.WriteError(ex.Message);
            }
            Mouse.OverrideCursor = Cursors.Arrow;
        }


    }
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (!string.IsNullOrEmpty(value as string))
                {
                    switch (value as string)
                    {
                        case "Updates":
                            return new BitmapImage(new Uri("pack://application:,,,/SCCMCliCtrWPF;component/Images/Computer_protection.ico"));
                        default:
                            return BitMapConvert.ToBitmapImage(common.Base64ToImage(value as string) as System.Drawing.Image) as BitmapImage;
                    }
                }
                else
                {
                    return new BitmapImage(new Uri("pack://application:,,,/SCCMCliCtrWPF;component/Images/Icon065.ico"));
                }
            }
            catch { }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    static class BitMapConvert
    {
        public static BitmapImage ToBitmapImage(this System.Drawing.Image image)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                image.Save(ms, image.RawFormat);
                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = ms;
                bi.EndInit();
                return bi;
            }
            catch { }

            return null;
        }
    }
}
