﻿#pragma checksum "..\..\OptionsDialog.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "924C86260E39BAA5BDA657EEBE7C703B6B33A830"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Jumper;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Jumper {
    
    
    /// <summary>
    /// OptionsDialog
    /// </summary>
    public partial class OptionsDialog : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\OptionsDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox PathText;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\OptionsDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button PathButton;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\OptionsDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox SubfolderCheck;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\OptionsDialog.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox AFCombo;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Jumper;component/optionsdialog.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\OptionsDialog.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.PathText = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.PathButton = ((System.Windows.Controls.Button)(target));
            
            #line 12 "..\..\OptionsDialog.xaml"
            this.PathButton.Click += new System.Windows.RoutedEventHandler(this.PathButton_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.SubfolderCheck = ((System.Windows.Controls.CheckBox)(target));
            
            #line 14 "..\..\OptionsDialog.xaml"
            this.SubfolderCheck.Checked += new System.Windows.RoutedEventHandler(this.SubfolderCheck_Checked);
            
            #line default
            #line hidden
            return;
            case 4:
            this.AFCombo = ((System.Windows.Controls.ComboBox)(target));
            
            #line 15 "..\..\OptionsDialog.xaml"
            this.AFCombo.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.ComboBox_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 17 "..\..\OptionsDialog.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 18 "..\..\OptionsDialog.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.Button_Click_1);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

