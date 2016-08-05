﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using a7JsonViewer.Utils;
using a7JsonViewer.ViewModel;
using ICSharpCode.AvalonEdit.Folding;

namespace a7JsonViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
#region text mode switching
        private bool _isTextMode;

        public bool IsTextMode
        {
            get { return _isTextMode; }
            set
            {
                string imgName;
                _isTextMode = value;
                if (_isTextMode)
                {
                    imgName = "tree.png";
                    tbMode.Text = "Switch to tree mode";
                    textModeControl.Visibility = Visibility.Visible;
                    treeModeControl.Visibility = Visibility.Collapsed;
                    bExpandCollapseTree.Visibility = Visibility.Collapsed;
                }
                else
                {
                    imgName = "text.png";
                    tbMode.Text = "Switch to text mode";
                    textModeControl.Visibility = Visibility.Collapsed;
                    treeModeControl.Visibility = Visibility.Visible;
                    bExpandCollapseTree.Visibility = Visibility.Visible;
                }
                var uriSource = new Uri($@"/a7JsonViewer;component/Images/{imgName}", UriKind.Relative);
                imgMode.Source = new BitmapImage(uriSource);
            }
        }
#endregion

        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                _isExpanded = value;
                string imgName;
                if (_isExpanded)
                {
                    this.tbExpand.Text = "Collapse all";
                    imgName = "collapse.png";
                }
                else
                {
                    tbExpand.Text = "Expand all";
                    imgName = "expand.png";
                }
                var uriSource = new Uri($@"/a7JsonViewer;component/Images/{imgName}", UriKind.Relative);
                imgExpand.Source = new BitmapImage(uriSource);
            }
        }

        public MainWindow()
        {
            this.Loaded += OnLoaded;
            this.DragEnter += (sender, args) => args.Effects = DragDropEffects.Move;
            this.Drop += OnDrop;
            InitializeComponent();
            this.DataContext = new DocumentVM(
                @"{ 'message' : 'try to drop a json file!'}");
            this.IsTextMode = false;
            this.bMode.Click += (sender, args) => IsTextMode = !IsTextMode;
            // Setter in JsonTree sets it to true, so we init here it to true as well (ugly code)
            IsExpanded = true;
        }

        private void OnDrop(object sender, DragEventArgs dragEventArgs)
        {
            if (dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])dragEventArgs.Data.GetData(DataFormats.FileDrop);
                if (files.Any() && File.Exists(files[0]))
                {
                    var content = File.ReadAllText(files[0]);
                    this.DataContext = new DocumentVM(content);
                }
            }
        }

#region avalon text editor handling
        FoldingManager foldingManager;
        BraceFoldingStrategy foldingStrategy;

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.textModeControl.TextChanged += Te_TextChanged;
            foldingManager = FoldingManager.Install(this.textModeControl.TextArea);
            foldingStrategy = new BraceFoldingStrategy();
            foldingStrategy.UpdateFoldings(foldingManager, textModeControl.Document);
        }

        private void Te_TextChanged(object sender, EventArgs e)
        {
            foldingStrategy.UpdateFoldings(foldingManager, textModeControl.Document);
        }
        #endregion

        private void BExpandCollapseTree_OnClick(object sender, RoutedEventArgs e)
        {
            this.treeModeControl.ExpandCollapseAll();
            IsExpanded = !IsExpanded;
        }
    }
}
